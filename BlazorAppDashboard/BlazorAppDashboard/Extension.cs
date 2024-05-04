using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

namespace BlazorAppDashboard
{
    public static class Extension
    {
        public static WebAssemblyHostBuilder AddServices(this WebAssemblyHostBuilder builder)
        {
            builder.Services.AddHttpClient();
            return builder;
        }
    }
}
