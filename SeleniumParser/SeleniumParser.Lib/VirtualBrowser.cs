using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.DevTools;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.UI;
using System.Collections.ObjectModel;

namespace SeleniumParser.Lib
{
    internal class VirtualBrowser
    {

        private readonly IWebDriver _driver;

        public VirtualBrowser()
        {
            VirtualBrowserConfig virtualBrowserConfig = new VirtualBrowserConfig()
            {
                BrowserVersion = "122",
                DriverOptions =
                [
                    "--start-maximized",
                    "--ignore-certificate-errors",
                    "--allow-running-insecure-content",
                    "headless",
                    //"disable-gpu"
                ]
            };
            bool enableDevTools = false;
            _driver = ConfigureDriver(virtualBrowserConfig);
            if (enableDevTools)
                ConfigureDevtools(virtualBrowserConfig);
        }


        private IWebDriver ConfigureDriver(VirtualBrowserConfig config)
        {
            var chromeOptions = new ChromeOptions();
            if (config.DriverOptions != null)
                foreach (string argument in config.DriverOptions)
                    chromeOptions.AddArguments(argument);

            if (config.BrowserVersion != null)
                chromeOptions.BrowserVersion = config.BrowserVersion;

            IWebDriver driver;
            if (!string.IsNullOrEmpty(config.RemoteWebDriverAddress))
                driver = new RemoteWebDriver(new Uri(config.RemoteWebDriverAddress), chromeOptions);
            else driver = new ChromeDriver(chromeOptions);
            return driver;
        }


        private void ConfigureDevtools(VirtualBrowserConfig config)
        {
            //DevToolsSession? devToolsSession;
            //var sessionId = ((IHasSessionId)_driver).SessionId.ToString();
            //var capabilitiesDictionary = (_driver as WebDriver)?.Capabilities.GetFieldValue<Dictionary<string, object>>("capabilities");
            //capabilitiesDictionary["se:cdp"] = config.CDPRemote.Replace("$SESSIONID", sessionId);
            //capabilitiesDictionary["se:cdpVersion"] = capabilitiesDictionary["browserVersion"];
            //try
            //{
            //    devToolsSession = ((IDevTools)_driver).GetDevToolsSession();
            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine("dev tools could not be initialized");
            //    Console.WriteLine(ex);
            //}
        }


        internal ReadOnlyCollection<IWebElement> FindElements(By by)
        {
            ReadOnlyCollection<IWebElement> elts;
            elts = _driver.FindElements(by);
            if (elts.Count == 0)
                Console.WriteLine("elements not found");
            return elts;
        }


        public string GetContent(By locator)
        {
            string content = "";
            try
            {
                string url = _driver.Url;
                Uri uri = new Uri(url);
                string hostname = uri.GetLeftPart(UriPartial.Authority);
                string clearScripts = @"
                            scripts = document.getElementsByTagName('script') 
                            for(let i=scripts.length - 1;i>=0;i--)
	                        scripts[i].remove();
                        ";
                string clearHrefs = @"
                    var anchors = document.getElementsByTagName(""a"");
                    for(let i=0;i<anchors.length;i++) anchors[i].removeAttribute(""href"");
                ";
                string replaceSrcs = @"
                    var images = document.getElementsByTagName('img');
                    for(var i = 0; i < images.length; i++)
                    {
                    images[i].src = images[i].src
                    } 
                ";

                IJavaScriptExecutor executor = (IJavaScriptExecutor)_driver;
                executor.ExecuteScript(clearScripts);
                executor.ExecuteScript(clearHrefs);
                executor.ExecuteScript(replaceSrcs);
                content = _driver.FindElement(locator).GetAttribute("innerHTML");
            }
            catch (Exception ex)
            {
                throw;
            }
            return content;
        }


        public void GoTo(string url)
        {
            _driver.Navigate().GoToUrl(url);
        }


        public void WaitForPageLoading()
        {
            try
            {
                new WebDriverWait(_driver, TimeSpan.FromSeconds(5))
                .Until(d =>
                {
                    var result = ((IJavaScriptExecutor)d).ExecuteScript("return document.readyState");
                    return result.Equals("interactive") || result.Equals("complete");
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine("document.readystate is not complete");
                Console.WriteLine(ex);
            }
        }


        public Task Dispose()
        {
            if (_driver != null)
            {
                _driver.Quit();
                _driver.Dispose();
            }

            return Task.CompletedTask;
        }


        internal string GetReadableText(By locator)
        {
            return _driver.FindElement(locator).GetAttribute("innerText");
        }
        public string GetCss()
        {
            string css = "";
            string getCssScript =
                @"totalCssText = """";
                    for(let i=0;i<document.styleSheets.length -1;i++){
                        try {
                      var cssRuleList = document.styleSheets[i].cssRules;
                        for(let j=0;j<cssRuleList.length -1;j++){
                          try {
                          totalCssText+= "" \n "" + document.styleSheets[i].cssRules[j].cssText +"" \n "";
                          }catch(error){
        
                          }
                        }
                           }catch(error){
        
                          }
                    } 
                    return totalCssText
                ";
            ReadOnlyCollection<IWebElement> StyleTags = FindElements(By.TagName("style"));
            foreach (var styleTag in StyleTags)
            {
                css += styleTag.GetAttribute("innerHTML");
                css += '\n';
            }
            css += ((IJavaScriptExecutor)_driver)
                .ExecuteScript(getCssScript);
            css = css.Replace("}#", "}\r\n#");
            return css;
        }
    }
}
