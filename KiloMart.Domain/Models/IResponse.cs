namespace KiloMart.Presentation.Authentication.Models;

public interface IResponse
{
    bool Success { get; set; }
    string[] Errors { get; set; }
}
