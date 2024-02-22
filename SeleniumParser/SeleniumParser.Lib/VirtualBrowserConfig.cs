namespace SeleniumParser.Lib
{
    internal class VirtualBrowserConfig
    {
        public string? BrowserVersion { get; set; }

        public string[]? DriverOptions { get; set; }
        public string? CDPRemote { get; set; }
        public string? RemoteWebDriverAddress { get; set; }
    }
}
