using BlazorAppDashboard.Services.Common;
using BlazorAppDashboard.Services.Common.Auth;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Radzen;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.Services.AddRadzenComponents();

;
/*builder.Services.AddScoped(sp => (IAuthenticationService)sp.GetRequiredService<AuthenticationStateProvider>())*/;

//builder.Services.AddScoped<HostingEnvironmentService>();

//builder.Services
//    .AddTransient<CookieHandler>()
//    .AddScoped(sp => sp
//        .GetRequiredService<IHttpClientFactory>()
//        .CreateClient("API"))
//    .AddHttpClient("API", client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)).AddHttpMessageHandler<CookieHandler>();



//builder.Services.AddAuthorizationCore();
//builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();

//builder.Services.AddHttpClient("ServerAPI", client =>
//{
//    client.DefaultRequestHeaders.AcceptLanguage.Clear();
//    client.BaseAddress = new Uri(builder.Configuration["BackendApiBaseUrl"]);
//})
//                //.AddAuthenticationHandler(config)
//                .Services
//            .AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("ServerAPI"));

await builder.Build().RunAsync();
