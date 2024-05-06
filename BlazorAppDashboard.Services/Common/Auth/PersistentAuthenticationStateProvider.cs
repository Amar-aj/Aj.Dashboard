using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using BlazorAppDashboard.Services.Models;
using Blazored.LocalStorage;
using System.Text.Json;
using Microsoft.Extensions.Options;

namespace BlazorAppDashboard.Services.Common.Auth;

public class PersistentAuthenticationStateProvider(PersistentComponentState persistentState, ILocalStorageService _localStorage) 
                                                                    : AuthenticationStateProvider, IAuthenticationService
{
    private static readonly Task<AuthenticationState> _unauthenticatedTask =
        Task.FromResult(new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity())));

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        string token = await _localStorage.GetItemAsStringAsync("token");
        //string userData = await _localStorage.GetItemAsStringAsync("user");

        var claimsIdentity = new ClaimsIdentity(GetClaimsFromJwt(token), "jwt");

        //var authenticationState = await authenticationStateTask;
        //var principal = authenticationState.User;

        if (claimsIdentity.IsAuthenticated == true)
        {
            var userId = claimsIdentity.FindFirst("nameid");
            var email = claimsIdentity.FindFirst("unique_name");

            if (userId != null && email != null)
            {
                persistentState.PersistAsJson(nameof(UserInfo), new UserInfo
                {
                    UserId = userId.Value,
                    Email = email.Value,
                });
            }
        }


        if (!persistentState.TryTakeFromJson<UserInfo>(nameof(UserInfo), out var userInfo) || userInfo is null)
        {
            return await _unauthenticatedTask;
        }

        Claim[] claims = [
            new Claim(ClaimTypes.NameIdentifier, userInfo.UserId),
                new Claim(ClaimTypes.Name, userInfo.Email),
                new Claim(ClaimTypes.Email, userInfo.Email)];



        //return new AuthenticationState(new ClaimsPrincipal(claimsIdentity));
        return await Task.FromResult(
            new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity(claims,
                authenticationType: nameof(PersistentAuthenticationStateProvider)))));
    }


    private IEnumerable<Claim> GetClaimsFromJwt(string jwt)
    {
        var claims = new List<Claim>();
        string payload = jwt.Split('.')[1];
        byte[] jsonBytes = ParseBase64WithoutPadding(payload);
        var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);

        if (keyValuePairs is not null)
        {
            keyValuePairs.TryGetValue(ClaimTypes.Role, out object? roles);

            if (roles is not null)
            {
                string? rolesString = roles.ToString();
                if (!string.IsNullOrEmpty(rolesString))
                {
                    if (rolesString.Trim().StartsWith("["))
                    {
                        string[]? parsedRoles = JsonSerializer.Deserialize<string[]>(rolesString);

                        if (parsedRoles is not null)
                        {
                            claims.AddRange(parsedRoles.Select(role => new Claim(ClaimTypes.Role, role)));
                        }
                    }
                    else
                    {
                        claims.Add(new Claim(ClaimTypes.Role, rolesString));
                    }
                }

                keyValuePairs.Remove(ClaimTypes.Role);
            }

            claims.AddRange(keyValuePairs.Select(kvp => new Claim(kvp.Key, kvp.Value.ToString() ?? string.Empty)));
        }

        return claims;
    }

    private byte[] ParseBase64WithoutPadding(string payload)
    {
        payload = payload.Trim().Replace('-', '+').Replace('_', '/');
        string base64 = payload.PadRight(payload.Length + ((4 - (payload.Length % 4)) % 4), '=');
        return Convert.FromBase64String(base64);
    }

    public void NavigateToExternalLogin(string returnUrl)
    {
        throw new NotImplementedException();
    }

    public Task<bool> LoginAsync(string tenantId, LoginRequest request)
    {
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        throw new NotImplementedException();
    }

    public Task LogoutAsync()
    {
        throw new NotImplementedException();
    }

    public Task ReLoginAsync(string returnUrl)
    {
        throw new NotImplementedException();
    }
}