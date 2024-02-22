using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.DevTools.V120.Animation;
using System;

namespace SeleniumParser.Lib
{
    public class PageParser 
    {
        public string GetWholePage(string url)
        {
            try
            {
                VirtualBrowser _browser = new VirtualBrowser();
                _browser.GoTo(url);
                _browser.WaitForPageLoading();
                string body = _browser.GetContent(By.TagName("body"));
                string css = _browser.GetCss();
                _browser.Dispose();
                return  $"<html><body>{body}</body><style>{css}</style></html>";
            }
            catch (Exception ex)
            {
                throw;
            }
        }

    }
}
