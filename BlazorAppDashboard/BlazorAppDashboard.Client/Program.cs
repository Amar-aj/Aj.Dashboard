using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Radzen;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.Services.AddRadzenComponents();


//builder.Services.AddHttpClient("ServerAPI", client =>
//{
//    client.DefaultRequestHeaders.AcceptLanguage.Clear();
//    client.BaseAddress = new Uri(builder.Configuration["BackendApiBaseUrl"]);
//})
//                //.AddAuthenticationHandler(config)
//                .Services
//            .AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("ServerAPI"));

await builder.Build().RunAsync();
