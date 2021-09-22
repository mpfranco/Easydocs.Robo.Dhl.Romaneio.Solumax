using RestSharp;
using Easydocs.Robo.Dhl.Romaneio.Solumax.Application.UseCases.Queries.Occurrences;
using Easydocs.Robo.Dhl.Romaneio.Solumax.Domain.Services;
using Easydocs.Robo.Dhl.Romaneio.Solumax.Infrastructure.Shared.Comunication;
using Easydocs.Robo.Dhl.Romaneio.Solumax.Infrastructure.Shared.Constants;
using Easydocs.Robo.Dhl.Romaneio.Solumax.Infrastructure.Shared.Services;
using Easydocs.Robo.Dhl.Romaneio.Solumax.Infrastructure.Shared.Settings;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Easydocs.Robo.Dhl.Romaneio.Solumax.Infrastructure.Shared.Helpers;
using System.Linq;
using OpenQA.Selenium;
using Polly;
using Easydocs.Robo.Dhl.Romaneio.Solumax.Domain.Entities;
using Easydocs.Robo.Dhl.Romaneio.Solumax.Application.UseCases.Commands.Romaneio.UpdateRomaneio;
using OpenQA.Selenium.Support.UI;


namespace Easydocs.Robo.Dhl.Romaneio.Solumax.Application.UseCases.Services
{
    public class FindRomaneioService : IFindRomaneioService
    {
        private static int re = 0;
        SeleniumHelper selenium;
        private readonly RestClient _restClient;
        private readonly RoboSolumaxSettings _roboVazFielSettings;
        private readonly IMediatorBus _mediator;
        private readonly ILoggerRomaneio _logger;
        private readonly IFindInvoiceQuerie _querieInvoice;
        char leftCnpj = '0';

        public FindRomaneioService(ILoggerRomaneio logger
                                  , RoboSolumaxSettings roboVazFielSettings
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

            try
            {

                LogInformation("Buscando romaneios pendentes");
                var romaneiosPesquisa = _querieInvoice.FindInvoicesPending().GetAwaiter().GetResult();

                if (!login(_roboVazFielSettings.user, _roboVazFielSettings.password))
                {
                    LogError("Não foi possível efetuar login!");
                    return;
                }
                var selectSite = new SelectElement(selenium.ObterElementoPorId("selectsite"));

                selectSite.SelectByValue(_roboVazFielSettings.conta);

                LogInformation("Gerando relatório no site solumax.");
                gerarRelatorioPorData();

                LogInformation("Comparando dados para localizar imagens.");
                var romaneiosPendentes = findRomaneio(romaneiosPesquisa);

                LogInformation("Atualizando romaneios pendentes.");
                foreach (var romaneio in romaneiosPendentes)
                {

                    romaneio.Dt_Download = calculaDataProximaTentativa(romaneio.Emissao);
                    var result = _mediator.SendCommadAsync(new UpdateRomaneioCommand(romaneio.Id,
                                                                        romaneio.Dt_Download,
                                                                        romaneio.NF,
                                                                        romaneio.CNPJ,
                                                                        romaneio.Serie,
                                                                        "404",
                                                                        romaneio.Emissao,
                                                                        0)).GetAwaiter()
                                                                           .GetResult();

                    if (result.StatusCode == StatusCode.Invalid) LogError(result.Data.ToString());
                }
                selenium.Dispose();

                LogInformation("Finalizado.");
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
        private void gerarRelatorioPorData()
        {
            selenium.ClicarPorXPath("//*[@id='cssmenu']/ul/li[3]/a");
            selenium.PreencherTextBoxPorId("date_ini", _roboVazFielSettings.dataInicial);
            selenium.PreencherTextBoxPorId("date_fim", _roboVazFielSettings.dataFinal);
            selenium.ClicarPorXPath("//*[@id='li_3']/div/label[6]/input");
            selenium.ClicarBotaoPorName("enviar");
        }
        private List<romaneio> findRomaneio(IEnumerable<romaneio> romaneioPesquisa)
        {
            try
            {
                var table = selenium.ObterElementoPorId("example");
                var bodyTable = table.FindElement(By.TagName("tbody"));
                var romaneiosDigitalizados = new List<romaneio>();
                foreach (var row in bodyTable.FindElements(By.TagName("tr")))
                {
                    var columns = row.FindElements(By.TagName("td"));
                    var nrRomaneio = columns[0].GetAttribute("innerHTML");
                    var romaneios = romaneioPesquisa.Where(x => x.Nr_romaneio == nrRomaneio);
                    Int16 paginas = 1;
                    foreach (var romaneio in romaneios)
                    {
                        romaneiosDigitalizados.Add(new romaneio
                        {
                            Nr_romaneio = columns[0].GetAttribute("innerHTML"),
                            UrlImagem = columns[6].FindElements(By.TagName("a")).FirstOrDefault().GetAttribute("href"),
                            CNPJ = romaneio.CNPJ,
                            NF = romaneio.NF,
                            Serie = romaneio.Serie,
                            Pagina = paginas
                        });
                        paginas++;
                    }

                }

                var romaneiosLocalizados = romaneiosDigitalizados.Intersect(romaneioPesquisa, new NrRomaneioComparer()).ToList();
                var romaneiosPendentes = romaneioPesquisa.Except(romaneiosDigitalizados, new NrRomaneioComparer()).ToList();

               
                LogInformation("Baixando imagens localizadas.");
                var totalImagens = romaneiosDigitalizados.Count();
                var totalBaixado = 0;
                var totalPaginas = 0;
                romaneiosLocalizados.ForEach(romaneio =>
                {
                    totalPaginas = romaneiosLocalizados.Count(x => x.Nr_romaneio == romaneio.Nr_romaneio);
                    using (var webClient = new WebClient())
                    {
                        var fileInfo = new FileInfo(romaneio.UrlImagem);
                        var extesion = fileInfo.Extension == "" ? ".jpg" : fileInfo.Extension;
                        var fileName = $"{romaneio.CNPJ.PadLeft(14, leftCnpj)}_{romaneio.Serie}_{romaneio.NF}_{romaneio.Pagina}{extesion}".Trim();
                        webClient.DownloadFile(romaneio.UrlImagem, Path.Combine(_roboVazFielSettings.directoryOutputFile, fileName));
                    }
                    totalBaixado++;
                   
                    LogInformation($"Imagem {totalBaixado} de {totalImagens} Páginas : { totalPaginas}");
                    romaneio.Nr_Paginas = totalPaginas;
                    romaneio.Download = "Sim";

                });
                return romaneiosPendentes;

            }
            catch (Exception err)
            {
                LogError(err.Message);
                throw new Exception(err.Message);
            }

        }


        public class NrRomaneioComparer : IEqualityComparer<romaneio>
        {
            public int GetHashCode(romaneio r)
            {
                if (r == null)
                {
                    return 0;
                }
                return r.Nr_romaneio.GetHashCode();
            }

            public bool Equals(romaneio x1, romaneio x2)
            {
                if (object.ReferenceEquals(x1, x2))
                {
                    return true;
                }
                if (object.ReferenceEquals(x1, null) ||
                    object.ReferenceEquals(x2, null))
                {
                    return false;
                }
                return x1.Nr_romaneio == x2.Nr_romaneio && x1.NF == x2.NF && x1.Serie == x2.Serie && x1.CNPJ == x2.CNPJ;
            }
        }



        private void LogInformation(string menssage)
        {
            _logger.LogInformation($"{nameof(FindRomaneioService)} - {menssage} --- hora: { DateTime.Now}");

        }
        private void LogError(string menssage)
        {
            _logger.LogError($"{nameof(FindRomaneioService)} - {menssage} --- hora: { DateTime.Now}");

        }



    }

}
