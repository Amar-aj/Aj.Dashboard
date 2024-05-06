using BlazorAppDashboard.Services.Common;
using BlazorAppDashboard.Services.Common.Auth;
using BlazorAppDashboard.Services.Models;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Radzen;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BlazorAppDashboard.Services.ApiServices;
public interface IAuth 
{
    Task LoginAsync(LoginRequest request);
}
public class Auth : IAuth
{
    private readonly HttpClient _httpClient;
    private readonly ILocalStorageService _localStorage;
    private readonly NotificationService _notificationService;
    private readonly NavigationManager _navigation;


    public Auth(IHttpClientFactory httpClientFactory, ILocalStorageService localStorage, NotificationService notificationService,
        NavigationManager navigation)
    {
        _httpClient = httpClientFactory.CreateClient("FullStackHero.API");
        _localStorage = localStorage;
        this._notificationService = notificationService;
        _navigation = navigation;
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
                var data = JsonConvert.DeserializeObject<ApiTokenResponse<LoginResponse>>(responseContent);
                if (data.Data is not null)
                {
                    await _localStorage.RemoveItemAsync("token");
                    await _localStorage.SetItemAsync("user", data.Data);
                    await _localStorage.SetItemAsync("token", data.Token);

                    Console.WriteLine("Successful login response: " + responseContent); // Or handle data appropriately
                    //await _authenticationService.LoginAsync("", model);

                    //await _authStateProvider.AuthenticateUser();
                    _notificationService.Notify(new NotificationMessage { Severity = NotificationSeverity.Success, Summary = data.Message, Duration = 4000 });
                    _navigation.NavigateTo("/weather");
                }
                else
                {
                    _notificationService.Notify(new NotificationMessage { Severity = NotificationSeverity.Warning, Summary = data.Message, Duration = 4000 });
                }
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
