using BlazorAppDashboard.Services.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BlazorAppDashboard.Services.ApiServices;
public interface IAuth : IAppService
{
    Task LoginAsync(LoginRequest request);
}
public class Auth : IAuth
{
    private readonly HttpClient _httpClient;

    public Auth(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient("FullStackHero.API");
    }

    public async Task LoginAsync(LoginRequest model)
    {
       
        try
        {
            var url = "auth";
          
            var serializedModel = Newtonsoft.Json.JsonConvert.SerializeObject(model);
            var content = new StringContent(serializedModel, System.Net.Http.Headers.MediaTypeHeaderValue.Parse("application/json"));

            var request = new HttpRequestMessage(HttpMethod.Post, url);
            request.Headers.Accept.Add(System.Net.Http.Headers.MediaTypeWithQualityHeaderValue.Parse("application/json"));
            request.Content = content; // Assign the content to the request

            var response = await _httpClient.SendAsync(request); // Send the request

            if (response.IsSuccessStatusCode)
            {
                using var responseStream = await response.Content.ReadAsStreamAsync();
                // Process successful response
                string responseContent = await new StreamReader(responseStream).ReadToEndAsync();
                Console.WriteLine("Successful login response: " + responseContent); // Or handle data appropriately
            }
            else
            {
                string errorMessage = await response.Content.ReadAsStringAsync();
                Console.WriteLine("Login failed with status code: " + response.StatusCode);
                Console.WriteLine("Error message: " + errorMessage); // Or handle the error gracefully
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error during login request: " + ex.Message); // Log or handle exceptions appropriately
        }
        // Process the response...
    }
}
