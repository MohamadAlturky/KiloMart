using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Text.Json;
using System.Threading.Tasks;
namespace KiloMart.Presentation.Middlewares;
public class ExceptionHandlingMiddleware : IMiddleware
{

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context); // Call the next middleware in the pipeline
        }
        catch (Exception ex)
        {

            // Prepare the response object
            var response = new
            {
                Status = false,
                Message = "un expected error occured try again or contant the backend team.",
                Errors = new
                {
                    ExceptionMessage = ex.Message,
                    ExceptionStackTrace = ex.StackTrace,
                }
            };

            // Set response details
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;

            // Write the response as JSON
            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}