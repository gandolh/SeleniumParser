using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SeleniumParser.Lib;

namespace SeleniumParser.UI.Components.Pages
{
    public partial class Home
    {
        private string url = string.Empty;
        private PageParser? parser;
        private string srcdoc = string.Empty;
        [Inject] public IJSRuntime JsRuntime { get; set; } = default!;

        protected override Task OnInitializedAsync()
        {
            return base.OnInitializedAsync();
        }
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await JsRuntime.InvokeVoidAsync("ResizeIframe");
            await base.OnAfterRenderAsync(firstRender);
        }

        private async void GetPage()
        {
            if (parser == null)
                parser = new PageParser();
            srcdoc = parser!.GetWholePage(url);

        }

    }
}
