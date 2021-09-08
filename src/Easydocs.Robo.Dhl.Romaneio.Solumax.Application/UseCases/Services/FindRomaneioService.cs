using FileHelpers;
using Newtonsoft.Json;
using RestSharp;
using Easydocs.Robo.Dhl.Romaneio.Solumax.Application.UseCases.Commands.Romaneio.AddRomaneio;
using Easydocs.Robo.Dhl.Romaneio.Solumax.Application.UseCases.Queries.Occurrences;
using Easydocs.Robo.Dhl.Romaneio.Solumax.Domain.Dto;
using Easydocs.Robo.Dhl.Romaneio.Solumax.Domain.Services;
using Easydocs.Robo.Dhl.Romaneio.Solumax.Infrastructure.Shared.Comunication;
using Easydocs.Robo.Dhl.Romaneio.Solumax.Infrastructure.Shared.Constants;
using Easydocs.Robo.Dhl.Romaneio.Solumax.Infrastructure.Shared.DomainObjects;
using Easydocs.Robo.Dhl.Romaneio.Solumax.Infrastructure.Shared.Extensions;
using Easydocs.Robo.Dhl.Romaneio.Solumax.Infrastructure.Shared.Services;
using Easydocs.Robo.Dhl.Romaneio.Solumax.Infrastructure.Shared.Settings;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Easydocs.Robo.Dhl.Romaneio.Solumax.Infrastructure.Shared.Helpers;
using Npoi.Mapper;
using System.Linq;
using NPOI.SS.Formula.Functions;
using System.Security.Policy;
using OpenQA.Selenium;
using Polly;
using Microsoft.AspNetCore.Http;
using System.Web;
using System.Collections.ObjectModel;
using Easydocs.Robo.Dhl.Romaneio.Solumax.Domain.Entities;
using System.Threading;
using Easydocs.Robo.Dhl.Romaneio.Solumax.Application.UseCases.Commands.Romaneio.UpdateRomaneio;
using OpenQA.Selenium.Support.UI;

namespace Easydocs.Robo.Dhl.Romaneio.Solumax.Application.UseCases.Services
{
    public class FindRomaneioService : IFindRomaneioService
    {
        private static int re = 0;
        SeleniumHelper selenium;
        private readonly RestClient _restClient;
        private readonly RoboVazFielSettings _roboVazFielSettings;
        private readonly IMediatorBus _mediator;
        private readonly ILoggerOccurrency _logger;
        private readonly IFindInvoiceQuerie _querieInvoice;
        char leftCnpj = '0';
        public FindRomaneioService(ILoggerOccurrency logger
                                  , RoboVazFielSettings roboVazFielSettings
                                  , IFindInvoiceQuerie querieInvoice
                                  , IMediatorBus mediator
                                  , RestClient restClient
                                  )
        {

            _roboVazFielSettings = roboVazFielSettings;
            selenium = new SeleniumHelper(_roboVazFielSettings.directoryTemp);
            _mediator = mediator;
            _logger = logger;
            _querieInvoice = querieInvoice;
            _restClient = restClient;

        }
        void WaitAndRetry()
        {
            var waitRetry = Policy
                              .Handle<Exception>()
                              .WaitAndRetryAsync(50, i => TimeSpan.FromSeconds(10), (result, timeSpan, retryCount, context) =>
                              {
                                  Console.WriteLine($"Request failed. Waiting {timeSpan} before next retry. Retry attempt {retryCount}");
                                  //selenium.Dispose();
                                  //selenium = new SeleniumHelper(_roboVazFielSettings.directoryTemp);
                              });

            waitRetry.ExecuteAsync(async () =>
            {
                await readFile();
            });
        }
        public async Task Executar()
        {
            WaitAndRetry();
        }
        private async Task readFile()
        {
            //var urlImg = "";

            try
            {


                var invoicesPending = _querieInvoice.FindInvoicesPending().GetAwaiter().GetResult();

                //verificarSeArquivoExiste(invoicesPending);


                if (!login(_roboVazFielSettings.user, _roboVazFielSettings.password))
                {
                    LogError("Não foi possível efetuar login!");
                    return;
                }


                foreach (var romaneio in invoicesPending)
                {

                    //LogInformation($"Processando romaneio {romaneio.Nr_romaneio} - NF {romaneio.NF}");
                    //if (romaneio.Download == "Sim") continue;
                    var romaneioResult = findRomaneio(romaneio);

                    if (romaneio.Download != "Sim")
                    {
                        romaneio.Dt_Download = calculaDataProximaTentativa(romaneio.Emissao);
                        var result = _mediator.SendCommadAsync(new UpdateRomaneioCommand(romaneio.Id,
                                                                            romaneio.Dt_Download,
                                                                            romaneio.NF,
                                                                            romaneio.CNPJ,
                                                                            romaneio.Serie,
                                                                            romaneioResult.Download,
                                                                            romaneio.Emissao,
                                                                            romaneioResult.Nr_Paginas)).GetAwaiter().GetResult();

                        if (result.StatusCode == StatusCode.Invalid) LogError(result.Data.ToString());

                    }



                }
                selenium.Dispose();
            }
            catch (Exception err)
            {
                LogError(err.Message);
                throw new Exception(err.ToString());
            }



        }
        private DateTime calculaDataProximaTentativa(DateTime emissao)
        {
            DateTime DataProximaTentativa;
            if (emissao >= DateTime.Today.AddDays(-30))
            {
                DataProximaTentativa = DateTime.Today.AddDays(+1);
            }
            else if (emissao < DateTime.Today.AddDays(-30) && emissao >= DateTime.Today.AddDays(-90))
            {
                DataProximaTentativa = DateTime.Today.AddDays(+7);
            }
            else
            {
                DataProximaTentativa = DateTime.Today.AddDays(+30);
            }
            return DataProximaTentativa;
        }
        public bool login(string user, string password)
        {
            try
            {
                selenium.acessarUrl(_roboVazFielSettings.urlBase);
                selenium.PreencherTextBoxPorId("login_form", user);
                selenium.PreencherTextBoxPorId("senha_form", password);
                selenium.ClicarPorXPath("//*[@id='li_x']/input[1]");
            }
            catch (Exception err)
            {
                LogError(err.Message);
                return false;
            }

            return true;

        }
        private romaneio findRomaneio(romaneio romaneio)
        {

            try
            {
                var selectSite = new SelectElement(selenium.ObterElementoPorId("selectsite"));
                selectSite.SelectByValue("MDTCID");
             
                selenium.ClicarPorXPath("//*[@id='cssmenu']/ul/li[3]/a");
                selenium.PreencherTextBoxPorId("date_ini", "01/01/2021");
                selenium.PreencherTextBoxPorId("date_fim", DateTime.Now.ToString("dd/MM/yyyy"));
                selenium.ClicarPorXPath("//*[@id='li_3']/div/label[6]/input");
                
                selenium.ClicarBotaoPorName("enviar");

                selenium.PreencherTextBoxPorId("search", "CMV0139996");
                //https://solumaxsolutions.com.br/osasco/
                // tabela filtro id : example
                //visualiza PDF
                //selenium.ClicarPorXPath("/html/body/table/tbody/tr/td/table[3]/tbody/tr[2]/td[1]/table/tbody/tr/td[3]/form/input[1]", "main");  
                var urlsImgs = selenium.ObterAtributoElementoPorName("main_view", "src", romaneio.Nr_romaneio, "main");
                if (urlsImgs == null)
                {
                    romaneio.Download = "404";
                    return romaneio;
                }
                int paginas = 0;
                foreach (var urlImg in urlsImgs)
                {
                    using (var webClient = new WebClient())
                    {
                        paginas++;
                        var fileInfo = new FileInfo(urlImg);
                        var extesion = fileInfo.Extension == "" ? ".jpg" : fileInfo.Extension;
                        var fileName = $"{romaneio.CNPJ.PadLeft(14, leftCnpj)}_{romaneio.Serie}_{romaneio.NF}_{paginas}{extesion}".Trim();
                        webClient.DownloadFile(urlImg, Path.Combine(_roboVazFielSettings.directoryOutputFile, fileName));
                    }
                }
                romaneio.Nr_Paginas = paginas;
                romaneio.Download = "Sim";
                return romaneio;
            }
            catch (Exception err)
            {
                LogError(err.Message);
                romaneio.Download = err.Message.ToString();
                throw new Exception(err.Message);
                //return romaneio;
            }

        }
        private void clearDirectoryTemp()
        {
            var files = Directory.GetFiles(_roboVazFielSettings.directoryTemp);
            foreach (var file in files)
            {
                File.Delete(file);
            }
        }



        private string getReference(ReadOnlyCollection<IWebElement> cols)
        {
            foreach (var td in cols)
            {
                if (td.GetAttribute("data-th") == "Reference") return td.GetAttribute("innerText");
            }
            return "";
        }

        private bool getImgDownloadAutomatically(Domain.Entities.romaneio romaneio)
        {
            int pagina = 0;

            if (!Directory.Exists(_roboVazFielSettings.directoryTemp))
            {
                LogError($"Diretório {_roboVazFielSettings.directoryTemp} não localizado!");
                return false;
            }

            var files = Directory.GetFiles(_roboVazFielSettings.directoryTemp);
            var fileNameValidation = files.Count() > 0 ? files[0] : ".tmp";
            do
            {
                files = Directory.GetFiles(_roboVazFielSettings.directoryTemp);
                fileNameValidation = files.Count() > 0 ? files[0] : ".tmp";
            } while (files.Count() <= 0 || fileNameValidation.Contains(".tmp") || fileNameValidation.Contains("download"));

            if (files.Count() <= 0) return false;

            foreach (var file in files)
            {
                var fileInfo = new FileInfo(file);
                var extesion = fileInfo.Extension == "" ? ".jpg" : fileInfo.Extension;
                var fileName = $"{romaneio.CNPJ.PadLeft(14, leftCnpj)}_{romaneio.Serie}_{romaneio.NF}_{romaneio.Nr_romaneio}_{pagina + 1}{extesion}".Trim();
                File.Move(fileInfo.FullName, Path.Combine(_roboVazFielSettings.directoryOutputFile, fileName));
                pagina++;
            }
            if (pagina > 0)
                return true;
            else
                return false;
        }



        private void verificarSeArquivoExiste(IEnumerable<romaneio> romaneios)
        {
            StreamWriter x;


            string CaminhoNome = $"{_roboVazFielSettings.directoryOutputFile}\\404.txt";


            x = File.CreateText(CaminhoNome);

            foreach (var romaneio in romaneios)
            {
                for (int i = 1; i == romaneio.Nr_Paginas; i++)
                {
                    var fileName = $"{romaneio.CNPJ.PadLeft(14, leftCnpj)}_{romaneio.Serie}_{romaneio.NF}_{i}.pdf".Trim();
                    var filePath = Path.Combine(_roboVazFielSettings.directoryOutputFile, fileName);
                    if (!File.Exists(filePath))
                    {
                        x.WriteLine($"{romaneio.NF},");
                    }
                }
                
            }

            x.Close();
        }





        //private async Task<Token> Autenticate()
        //{
        //    try
        //    {

        //        var autenticate = new Autenticate();
        //        autenticate.client_id = _ocurrencySettings.client_id;
        //        autenticate.client_secret = _ocurrencySettings.client_secret;
        //        autenticate.grant_type = _ocurrencySettings.grant_type;
        //        var payload = JsonConvert.SerializeObject(autenticate);
        //        var method = new MultipartFormDataContent();
        //        method.Add(new StringContent(autenticate.client_id), "client_id");
        //        method.Add(new StringContent(autenticate.grant_type), "grant_type");
        //        method.Add(new StringContent(autenticate.client_secret), "client_secret");                
        //        var response = await _httpClient.PostAsync("/v5/token", method);

        //        var jsonString = await response.Content.ReadAsStringAsync();
        //        var result = JsonConvert.DeserializeObject<Token>(jsonString);

        //        switch (response.StatusCode)
        //        {
        //            case HttpStatusCode.OK:
        //                return result;
        //            case HttpStatusCode.Unauthorized:
        //                LogError($"Autenticate : {result.message}");
        //                return result;
        //            case HttpStatusCode.BadRequest:
        //                LogError($"Autenticate : {result.message}");
        //                return result;
        //            default:
        //                return result;
        //        }
        //    }
        //    catch (Exception err)
        //    {
        //        LogError(err.Message);
        //        return null;
        //    }
        //}


        //private async Task<occurrency> GetOccurrences(Token token)
        //{
        //    try
        //    {
        //        var tokenType = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(token.token_type);
        //        _httpClient.DefaultRequestHeaders.Clear();
        //        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue($"{tokenType}", token.access_token);
        //        var dataInicio = DateTime.Now.AddDays(_ocurrencySettings.StartDateDays).ToString("MM/dd/yyyy");
        //        var dataFim = DateTime.Now.AddDays(_ocurrencySettings.EndDateDays).ToString("MM/dd/yyyy");
        //        var response = await _httpClient.GetAsync($"/v5/transactions/occurrency?DataInicio={dataInicio}&DataFim={dataFim}");
        //        var json = await response.Content.ReadAsStringAsync();

        //        var result = JsonConvert.DeserializeObject<occurrency>(json);

        //        switch (response.StatusCode)
        //        {
        //            case HttpStatusCode.OK:
        //                return result;
        //            case HttpStatusCode.Unauthorized:
        //                LogError($"GetOccurrences : {result.message}");
        //                return result;
        //            case HttpStatusCode.BadRequest:
        //                LogError($"GetOccurrences : {result.message}");
        //                return result;
        //            default:
        //                return result;
        //        }
        //    }
        //    catch (Exception err)
        //    {
        //        LogError(err.Message);
        //        return null;
        //    }


        //}

        //private async Task PersitOccurrences()
        //{
        //    try
        //    {
        //        var token = await Autenticate();
        //        if (token.message != null || token == null) return;
        //        if (string.IsNullOrEmpty(token.access_token)) 
        //        {
        //            LogError("Não foi possível capturar o token!");
        //            return;
        //        }

        //        var occurrency = await GetOccurrences(token);

        //        if (occurrency?.message.ToUpper() != "SUCESSO") return;

        //        if (occurrency.occurrences == null ||
        //            occurrency?.occurrences?.Count <= 0)
        //        {
        //            LogInformation("Ocorrências não localzadas para data informada");
        //            return;
        //        }

        //        foreach (var occurency in occurrency.occurrences)
        //        {
        //            if (await _querieOccurrency.FindOccurrencyByTransactionId(occurency.transactionId) == null)
        //            {
        //                var result = await _mediator.SendCommadAsync(new AddOccurrencyCommand(
        //                                                             occurency.date, occurency.createDate,
        //                                                             occurency.descriptionMotivo, occurency.externalNSU,
        //                                                             occurency.transactionId, occurency.externalTerminal,
        //                                                             occurency.linhaDigitavel, occurency.value
        //                                                         ));
        //                if (result.StatusCode.Value != StatusCode.IsSuccess) LogError(result.Data.ToString());
        //            }
        //        }
        //        await ExportToCSV(occurrency.occurrences);

        //    }
        //    catch (Exception err)
        //    {
        //        LogError(err.Message);
        //    }
        //}

        //private async Task ExportToCSV(List<occurrences> occurrences)
        //{
        //    try
        //    {
        //        var directoryOutput = _ocurrencySettings.DirectoryOutputFile.EndsWith("\\") ? _ocurrencySettings.DirectoryOutputFile : _ocurrencySettings.DirectoryOutputFile + "\\";
        //        if (!Directory.Exists(directoryOutput)) Directory.CreateDirectory(directoryOutput);
        //        var file = "occurrences.csv";
        //        var fileOutput = $"{directoryOutput}{file.FileName()}";                
        //        var engine = new FileHelperEngine<OccurencyDto> { HeaderText = _ocurrencySettings.HeaderFile};
        //        engine.WriteFile(fileOutput, ParseOccurencyDto(occurrences));
        //    }
        //    catch (Exception err)
        //    {
        //        LogError(err.Message);
        //    }
        //}

        //private List<OccurencyDto> ParseOccurencyDto(List<occurrences> occurrences)
        //{
        //    var occurrencesDto = new List<OccurencyDto>();
        //    foreach (var occurrence in occurrences)
        //    {
        //        occurrencesDto.Add(new OccurencyDto(occurrence.transactionId,
        //                            occurrence.externalNSU,
        //                            "SuperDigital",
        //                            $"R$ {occurrence.value}",
        //                            occurrence.createDate.DateTimeToFormatString(),
        //                            occurrence.descriptionMotivo));
        //    }
        //    return occurrencesDto;
        //}

        //private HttpClient UseProxy()
        //{

        //    var webProxy = new WebProxy(new Uri(_ocurrencySettings.Proxy), BypassOnLocal: false);

        //    var proxyHttpClientHandler = new HttpClientHandler
        //    {
        //        Proxy = webProxy,
        //        UseProxy = true,
        //    };
        //    return new HttpClient(proxyHttpClientHandler)
        //    {
        //        BaseAddress = new Uri(_ocurrencySettings.UrlBase)
        //    };

        //}
        private void LogInformation(string menssage)
        {
            _logger.LogInformation($"{nameof(FindRomaneioService)} - {menssage} --- hora: { DateTime.Now}");

        }
        private void LogError(string menssage)
        {
            _logger.LogError($"{nameof(FindRomaneioService)} - {menssage} --- hora: { DateTime.Now}");

        }

        //private string AutenticateRestSharp()
        //{
        //    var token = "";
        //    try
        //    {

        //        var autenticate = new Autenticate();
        //        autenticate.client_id = _ocurrencySettings.client_id;
        //        autenticate.client_secret = _ocurrencySettings.client_secret;
        //        autenticate.grant_type = _ocurrencySettings.grant_type;
        //        var request = new RestRequest("/v5/token", Method.POST);
        //        request.AlwaysMultipartFormData = true;
        //        request.AddParameter("client_id", autenticate.client_id);
        //        request.AddParameter("grant_type", autenticate.grant_type);
        //        request.AddParameter("client_secret", autenticate.client_secret);
        //        var response = _restClient.Execute(request);
        //        if (response.StatusCode == HttpStatusCode.OK)
        //        {
        //            var result = JsonConvert.DeserializeObject<Token>(response.Content);
        //            token = result.access_token;
        //        }
        //        return token;
        //    }
        //    catch (Exception err)
        //    {
        //        LogError(err.Message);
        //        return token; 
        //    }

        //}

        //private occurrency GetOccurrencesRestSharp(string token)
        //{
        //    //{"errorCode":"633","message":"DataInicio maior que a DataFim"}
        //    try
        //    {
        //        var dataInicio = DateTime.Now.AddDays(-15).ToString("MM/dd/yyyy");
        //        var dataFim = DateTime.Now.AddDays(-10).ToString("MM/dd/yyyy");
        //        var request = new RestRequest($"/v5/transactions/occurrency?DataInicio={dataInicio}&DataFim={dataFim}", Method.GET);
        //        request.AddHeader("Authorization", $"Bearer {token}");
        //        var response = _restClient.Execute(request);
        //        var result = JsonConvert.DeserializeObject<occurrency>(response.Content);

        //        switch (response.StatusCode)
        //        {
        //            case HttpStatusCode.OK:
        //                return result;
        //            case HttpStatusCode.Unauthorized:
        //                LogError(result.message);
        //                return result;
        //            case HttpStatusCode.BadRequest:
        //                LogError(result.message);
        //                return result;
        //            default:
        //                return result;
        //        }
        //    }
        //    catch (Exception err)
        //    {
        //        LogError(err.Message);
        //        return null;
        //    }


        //}
    }
}
