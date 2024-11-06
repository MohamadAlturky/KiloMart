using KiloMart.Domain.Requests;
using KiloMart.Domain.Responses;

namespace KiloMart.Domain.Services;

public static class LanguageService
{
    public static AddLanguageResponse AddLanguage(AddLanguageRequest request)
    {
        // Implement the business logic to add a language here
        // For example, interacting with the database or another service

        // Return a response based on the operation’s success
        return new AddLanguageResponse
        {
            Success = true,
            Message = "Language added successfully."
        };
    }
}
