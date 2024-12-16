using Dapper;
using KiloMart.Core.Authentication;
using KiloMart.Core.Contracts;
using KiloMart.Domain.Register.Utils;
using KiloMart.Presentation.Authorization;
using KiloMart.Presentation.Controllers;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class DocumentTypeController(IDbFactory dbFactory, IUserContext userContext) 
    : AppController(dbFactory, userContext)
{
    [HttpPost("insert")]
    [Guard([Roles.Admin])]

    public async Task<IActionResult> Insert([FromBody] DocumentTypeModel request)
    {
        (bool success, string[] errors) = request.Validate();
        if (!success)
        {
            return ValidationError(errors);
        }

        using var connection = _dbFactory.CreateDbConnection();

        // Check if the document type already exists
        const string checkQuery = "SELECT COUNT(1) FROM [dbo].[DocumentType] WHERE [Name] = @Name";
        var exists = await connection.ExecuteScalarAsync<bool>(checkQuery, new { Name = request.Name });

        if (exists)
        {
            return Fail("Document type already exists.");
        }

        // Insert new document type
        const string insertQuery = @"INSERT INTO [dbo].[DocumentType] ([Name]) VALUES (@Name)";
        
        await connection.ExecuteAsync(insertQuery, new { Name = request.Name });

        return Success(new { Message = "Document type inserted successfully." });
    }
}
public class DocumentTypeModel
{
    /// <summary>
    /// Name of the document type
    /// </summary>
    public string Name { get; set; } = null!;

    public (bool Success, string[] Errors) Validate()
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(Name))
            errors.Add("Name is required.");

        return (errors.Count == 0, errors.ToArray());
    }
}