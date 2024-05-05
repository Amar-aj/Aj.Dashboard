using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorAppDashboard.Services.Models;

public  class LoginRequest
{
    public string Email { get; set; }
    public string Password { get; set; }
}


public class UserInfo
{
    public required string UserId { get; set; }
    public required string Email { get; set; }
}

public record LoginResponse(long user_id, string email, string username);