

using BlazorAppDashboard.Services.ApiServices;
using BlazorAppDashboard.Services.Common;
using BlazorAppDashboard.Services.Common.Auth;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Globalization;

namespace BlazorAppDashboard.Services
{

    public static class Extension
    {
        private const string ClientName = "FullStackHero.API";
        public static IServiceCollection AddClientServices(this IServiceCollection services, IConfiguration config) =>
         services
            .AddScoped<IAuth, Auth>()
            .AutoRegisterInterfaces<IAppService>()
            .AddBlazoredLocalStorage()
            .AddHttpClient(ClientName, client =>
                 {
                     client.DefaultRequestHeaders.AcceptLanguage.Clear();
                     client.DefaultRequestHeaders.AcceptLanguage.ParseAdd(CultureInfo.DefaultThreadCurrentCulture?.TwoLetterISOLanguageName);
                     client.BaseAddress = new Uri(config["BackendApiBaseUrl"]);
                 })
            //.AddAuthenticationHandler(config)
            .Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient(ClientName))

            .AddAuthorizationCore()
            .AddCascadingAuthenticationState()
            .AddScoped<AuthenticationStateProvider, PersistentAuthenticationStateProvider>()
            .AddScoped<IAuthenticationService, PersistentAuthenticationStateProvider>()
            //.AddScoped(sp => (IAuthenticationService)sp.GetRequiredService<AuthenticationStateProvider>())
            //.AddScoped(sp => (IAuthenticationService)sp.GetRequiredService<AuthenticationStateProvider>())
            /*.AddAuthorizationCore().AddScoped<CustomAuthStateProvider>().AddScoped<AuthenticationStateProvider>(s => s.GetRequiredService<CustomAuthStateProvider>())*/;












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
