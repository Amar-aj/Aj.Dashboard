

using BlazorAppDashboard.Services.ApiServices;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Globalization;

namespace BlazorAppDashboard.Services
{

    public static class Extension
    {
        private const string ClientName = "FullStackHero.API";
        public static IServiceCollection AddClientServices(this IServiceCollection services, IConfiguration config) =>
         services.AutoRegisterInterfaces<IAppService>()


            .AddHttpClient(ClientName, client =>
         {
             client.DefaultRequestHeaders.AcceptLanguage.Clear();
             client.DefaultRequestHeaders.AcceptLanguage.ParseAdd(CultureInfo.DefaultThreadCurrentCulture?.TwoLetterISOLanguageName);
             client.BaseAddress = new Uri(config["BackendApiBaseUrl"]);
         })
                //.AddAuthenticationHandler(config)
          .Services
            .AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient(ClientName));












        public static IServiceCollection AutoRegisterInterfaces<T>(this IServiceCollection services)
        {
            var @interface = typeof(T);

            var types = @interface
                .Assembly
                .GetExportedTypes()
                .Where(t => t.IsClass && !t.IsAbstract)
                .Select(t => new
                {
                    Service = t.GetInterface($"I{t.Name}"),
                    Implementation = t
                })
                .Where(t => t.Service != null);

            foreach (var type in types)
            {
                if (@interface.IsAssignableFrom(type.Service))
                {
                    services.AddTransient(type.Service, type.Implementation);
                }
            }

            return services;
        }
    }




   
}
