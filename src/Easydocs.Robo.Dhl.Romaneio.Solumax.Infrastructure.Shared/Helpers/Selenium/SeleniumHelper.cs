using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Internal;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace Easydocs.Robo.Dhl.Romaneio.Solumax.Infrastructure.Shared.Helpers
{
    public class SeleniumHelper
    {
        public WebDriverWait Wait;
        public ChromeDriver driver;
        public SeleniumHelper(string directoryTemp)
        {
            

            var chromeOptions = new ChromeOptions();
            chromeOptions.AddArguments("--disable-notifications"); 
            chromeOptions.AddArguments("headless");
            chromeOptions.AddUserProfilePreference("download.default_directory", directoryTemp);
            chromeOptions.AddUserProfilePreference("intl.accept_languages", "nl");
            chromeOptions.AddUserProfilePreference("disable-popup-blocking", "true");
        

            driver = new ChromeDriver(@"C:\WebDriver", chromeOptions);
            //driver.Manage().Window.Maximize();
            Wait = new WebDriverWait(driver, TimeSpan.FromSeconds(60));
        }

        public void acessarUrl(string url)
        {
            driver.Navigate().GoToUrl(url);
        }

        public string obterUrl()
        {
            return driver.Url;
        }

        public string waitElementInvisible(string path)
        {
            try
            {
                do
                {
                }
                while (driver.WindowHandles.Count < 2);
                driver.SwitchTo().Window(driver.WindowHandles[1]);
                var url = driver.Url;
                driver.Close();
                driver.SwitchTo().Window(driver.WindowHandles[0]);
                Wait.Until(ExpectedConditions.ElementIsVisible(By.Name(path)));
                return url;
            }
            catch (Exception err)
            {
                return "";
            }

        }

        public void ClicarBotaoPorName(string botaoName, string SwitchToFrame = "")
        {
            if (SwitchToFrame.Length > 0) driver.SwitchTo().Frame(SwitchToFrame);
            var botao = Wait.Until(ExpectedConditions.ElementIsVisible(By.Name(botaoName)));
            botao.Click();
            if (SwitchToFrame.Length > 0) driver.SwitchTo().DefaultContent();
        }


        public void PreencherTextBoxPorId(string idCampo, string valorCampo)
        {
            var campo = Wait.Until(ExpectedConditions.ElementIsVisible(By.Id(idCampo)));
            System.Threading.Thread.Sleep(1000);
            campo.SendKeys(valorCampo);
        }

        public void PreencherTextBoxPorName(string nameCampo, string valorCampo, string SwitchToFrame = "")
        {
            if (SwitchToFrame.Length > 0) driver.SwitchTo().Frame(SwitchToFrame);
            var campo = Wait.Until(ExpectedConditions.ElementIsVisible(By.Name(nameCampo)));
            campo.SendKeys(valorCampo);
            if (SwitchToFrame.Length > 0) driver.SwitchTo().DefaultContent();
        }
        public void PreencherTextBoxPorXPath(string xPath, string valorCampo)
        {
            var campo = Wait.Until(ExpectedConditions.ElementIsVisible(By.XPath(xPath)));
            campo.Clear();
            campo.SendKeys(valorCampo);
        }

        public void ClicarPorXPath(string xPath, string SwitchToFrame = "")
        {
            if (SwitchToFrame.Length > 0) driver.SwitchTo().Frame(SwitchToFrame);
            var elemento = Wait.Until(ExpectedConditions.ElementIsVisible(By.XPath(xPath)));
            elemento.Click();
            if (SwitchToFrame.Length > 0) driver.SwitchTo().DefaultContent();
        }
        public IWebElement ObterElementoPorClasse(string classeCss)
        {
            return Wait.Until(ExpectedConditions.ElementIsVisible(By.ClassName(classeCss)));
        }
        public void ClicarPorLinkText(string linkText)
        {
            var elemento = Wait.Until(ExpectedConditions.ElementIsVisible(By.PartialLinkText(linkText)));
            elemento.Click();
        }
        public void AguardaBotaoHabilitar(string classButton)
        {
            Wait.Until(ExpectedConditions.ElementToBeClickable(By.ClassName(classButton)));

        }          
        public IWebElement ObterElementoPorId(string id, string SwitchToFrame = "")
        {
            if (SwitchToFrame.Length > 0) driver.SwitchTo().Frame(SwitchToFrame);
            var element = Wait.Until(ExpectedConditions.ElementIsVisible(By.Id(id)));
            if (SwitchToFrame.Length > 0) driver.SwitchTo().DefaultContent();
            return element;
        }
        public IWebElement ObterElementoPorName(string name, string SwitchToFrame = "")
        {
            if (SwitchToFrame.Length > 0) driver.SwitchTo().Frame(SwitchToFrame);
            var element = Wait.Until(ExpectedConditions.ElementIsVisible(By.Name(name)));
            if (SwitchToFrame.Length > 0) driver.SwitchTo().DefaultContent();
            return element;
        }
        public IEnumerable<string> ObterAtributoElementoPorName(string name, string atributo,string romaneio, string SwitchToFrame = "")
        {

            try
            {
                List<string> lstSrc = new List<string>();
                if (SwitchToFrame.Length > 0) driver.SwitchTo().Frame(SwitchToFrame);                
                while (driver.FindElements(By.Name("ems_acao"))?.Count <= 0 
                    && driver.FindElements(By.XPath("/html/body/table/tbody/tr/td/table[2]/tbody/tr/td[1]/span"))?.Count <= 0)
                {}
                
                if (driver.FindElements(By.Name("ems_acao"))?.Count > 0)
                {                    
                    if (SwitchToFrame.Length > 0) driver.SwitchTo().DefaultContent();
                    return null;
                }                    
                var table = Wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("/html/body/table/tbody/tr/td/table[3]")));
                var rows = table.FindElements(By.TagName("tr"));                
                for (var count = 0; count <= rows.Count - 1; count++)
                {
                    var columns = rows[count].FindElements(By.TagName("td"));
                    if (columns.Count <= 0) continue;
                    var form = columns[0].FindElements(By.TagName("form"));
                    if (form.Count <= 1) continue;
                    var carrier = columns[6]?.FindElement(By.TagName("span")).GetAttribute("innerHTML");
                    if(romaneio != carrier)
                    {
                        continue;
                    }
                    var button = form[2].FindElement(By.TagName("input"));
                    button.Click();
                    var element = Wait.Until(ExpectedConditions.ElementIsVisible(By.Name(name)));
                    lstSrc.Add(element.GetAttribute(atributo));
                    ClicarPorXPath("//*[@id='Layer1']/form/table/tbody/tr/td[2]/div/input", "top_view");
                    if (SwitchToFrame.Length > 0) driver.SwitchTo().Frame(SwitchToFrame);
                    var btnProximo = driver.FindElements(By.Name("dir"));
                    //Ignora ultima linha, rodapé
                    if (count == rows.Count - 2
                    && btnProximo.Count > 0)
                    {
                        if (btnProximo[0].GetAttribute("value") == "<< Anterior")
                        {
                            if (SwitchToFrame.Length > 0) driver.SwitchTo().DefaultContent();
                            return lstSrc;
                        }
                        btnProximo[0].Click();
                        count = 0;
                    }
                    table = Wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("/html/body/table/tbody/tr/td/table[3]")));
                    rows = table.FindElements(By.TagName("tr"));

                }
                
                if (SwitchToFrame.Length > 0) driver.SwitchTo().DefaultContent();
                return lstSrc;
            }
            catch(Exception err)
            {
                return null;
            }
            
        }
        
        public ReadOnlyCollection<IWebElement> ObterElementosPorName(string nameFind, string nameElementWait)
        {
            Wait.Until(ExpectedConditions.ElementIsVisible(By.Name(nameElementWait)));
            return driver.FindElementsByName(nameFind);
        }
        public ReadOnlyCollection<IWebElement> ObterElementosPorXPath(string xpath, string SwitchToFrame = "")
        {
            if (SwitchToFrame.Length > 0) driver.SwitchTo().Frame(SwitchToFrame);
            Wait.Until(ExpectedConditions.ElementIsVisible(By.XPath(xpath)));
            var elements = driver.FindElementsByName(xpath);
            if (SwitchToFrame.Length > 0) driver.SwitchTo().DefaultContent();
            return elements;
        }
        public IWebElement ObterElementoPorXPath(string xpath, string SwitchToFrame = "")
        {
            if (SwitchToFrame.Length > 0) driver.SwitchTo().Frame(SwitchToFrame);
            Wait.Until(ExpectedConditions.ElementIsVisible(By.XPath(xpath)));
            var element = driver.FindElementByName(xpath);
            if (SwitchToFrame.Length > 0) driver.SwitchTo().DefaultContent();
            return element;
        }

        public IWebElement ObterElementoPorLinkText(string linkText)
        {
            return Wait.Until(ExpectedConditions.ElementIsVisible(By.LinkText(linkText)));
        }
        public IWebElement ObterElementosPorLinkText(string linkText)
        {
            return Wait.Until(ExpectedConditions.ElementIsVisible(By.LinkText(linkText)));
        }
        public List<string> finElementByRoot(string idRoot, string idElement, string valorCampo, string XPath)
        {
            List<string> result = new List<string>();
            ReadOnlyCollection<IWebElement> columns;
            IWebElement divNf;
            int nf = 0;
            try
            {
                var formElement = Wait.Until(ExpectedConditions.ElementIsVisible(By.Id(idRoot)));
                var element = formElement.FindElement(By.Id(idElement));
                var button = Wait.Until(ExpectedConditions.ElementIsVisible(By.TagName("img")));
                element.SendKeys(valorCampo);
                button.Click();
                //500 - Erro de servidor interno.
                if (driver.FindElementByTagName("title").GetAttribute("innerText").Contains("500")) return new List<string>() { "500" };

                var divAux = driver.FindElement(By.Id("aux"));
                var tableRoot = divAux.FindElement(By.TagName("table"));
                var table = tableRoot.FindElements(By.TagName("table"))?[1];
                var rows = table.FindElements(By.TagName("tr"));
                var contRow = 0;
                foreach (var item in rows)
                {
                    if (item.FindElements(By.TagName("td"))?[0].FindElements(By.TagName("p")).Count > 0
                        && item.FindElements(By.LinkText("Link Comprovante")).Count <= 0)
                    {
                        columns = item.FindElements(By.TagName("td"));
                        divNf = columns[1].FindElement(By.TagName("div"));
                        nf = Convert.ToInt32(divNf.FindElements(By.TagName("p"))?[1].GetAttribute("innerText").ToString().Split("/")?[0]);

                    }

                    if (item.FindElements(By.LinkText("Link Comprovante")).Count > 0)
                    {
                        if (nf.ToString() != valorCampo) continue;
                        var comprovantes = item.FindElements(By.LinkText("Link Comprovante"));
                        foreach (var itemLink in comprovantes)
                        {
                            result.Add(itemLink.GetAttribute("href"));
                        }
                        contRow++;
                    }

                }


                //var comprovantes = driver.FindElementsByLinkText("Link Comprovante");
                //if (comprovantes == null) return new List<string>() { "404" };

                //foreach (var item in comprovantes)
                //{
                //    result.Add(item.GetAttribute("href"));
                //}

            }
            catch (Exception err)
            {
                new List<string>() { "500" };
            }

            return result.Count <= 0 ? new List<string>() { "404" } : result;
        }

        public int getAbas()
        {
            return driver.WindowHandles.Count;
        }
        public void Dispose()
        {
            driver.Quit();
            driver.Dispose();
        }
    }
}
