using KiloMart.Presentation.Authentication.Models;
using KiloMart.Requests.Queries;

namespace KiloMart.Domain.Login.Models;

// public class LoginResult
// {
//     public bool Success { get; set; }
//     public string Email { get; set; }
//     public string Role { get; set; }
//     public string UserName { get; set; }
//     public string? Token { get; set; }
//     public string[] Errors { get; set; } = [];
//     public byte Language { get; set; }
//     public int Party { get; set; }
//     public int UserId { get; set; }
//     public short RoleNumber { get; set; }
//     public Object? ActiveProfile { get; set; }
//     public Object? AllProfiles { get; set; }
//     public Object? UserInfo { get; set; }
//     public Object? PartyInfo { get; set; }
// }


public class LoginResult : IResponse
{
    public bool Success { get; set; }
    public string Email { get; set; }
    public string Role { get; set; }
    public string UserName { get; set; }
    public string? Token { get; set; }
    public string[] Errors { get; set; } = [];
    public byte Language { get; set; }
    public int Party { get; set; }
    public int UserId { get; set; }
    public short RoleNumber { get; set; }
    public object? ActiveProfile { get; set; }
    public object? AllProfiles { get; set; }
    public object? UserInfo { get; set; }
    public object? PartyInfo { get; set; }
}