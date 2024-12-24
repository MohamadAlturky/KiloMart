using Dapper;
using KiloMart.Commands.Services;
using KiloMart.Core.Authentication;
using KiloMart.Core.Contracts;
using KiloMart.DataAccess.Database;
using KiloMart.Domain.Register.Utils;
using KiloMart.Presentation.Authorization;
using KiloMart.Presentation.Services;
using KiloMart.Requests.Queries;
using Microsoft.AspNetCore.Mvc;

namespace KiloMart.Presentation.Controllers;

[ApiController]
[Route("api/slider-item")]
public class SliderItemController(IDbFactory dbFactory, IUserContext userContext,
  IWebHostEnvironment environment) : AppController(dbFactory, userContext)
{
    public class UploadSliderItemModel
    {
        public IFormFile? ImageFile { get; set; }
        public int? Target { get; set; }

        public (bool Success, string[] Errors) Validate()
        {
            var errors = new List<string>();

            if (ImageFile is null)
                errors.Add("ImageFile is required.");

            return (errors.Count == 0, errors.ToArray());
        }
    }

    [HttpPost("upload")]
    [Guard([Roles.Admin])]
    public async Task<IActionResult> Insert([FromForm] UploadSliderItemModel request)
    {
        (bool success, string[] errors) = request.Validate();
        if (!success)
        {
            return ValidationError(errors);
        }

        if (request.ImageFile is null)
        {
            return ValidationError(new List<string> { "File is required" });
        }

        Guid fileName = Guid.NewGuid();
        var filePath = await FileService.SaveImageFileAsync(request.ImageFile,
            environment.WebRootPath,
            fileName);

        using var connection = _dbFactory.CreateDbConnection();

        int sliderId = await Db.InsertSliderItemAsync(
            connection,
            filePath,
            request.Target);

        return Success(new { sliderId });
    }

    [HttpGet("")]
    public async Task<IActionResult> GetById([FromQuery] int id)
    {
        using var connection = _dbFactory.CreateDbConnection();
        SliderItem? sliderItem = await Db.GetSliderItemByIdAsync(id, connection);

        if (sliderItem is null)
        {
            return DataNotFound();
        }

        return Success(sliderItem);
    }
    [HttpGet("list")]
    public async Task<IActionResult> List()
    {
        using var connection = _dbFactory.CreateDbConnection();
        var sliderItem = await Db.GetSliderItemListAsync(connection);
        return Success(sliderItem.ToList());
    }
    [HttpPut("")]
    [Guard([Roles.Admin])]
    public async Task<IActionResult> Update([FromQuery] int id, [FromForm] UploadSliderItemModel request)
    {
        using var connection = _dbFactory.CreateDbConnection();

        var existingItem = await Db.GetSliderItemByIdAsync(id, connection);
        if (existingItem is null)
        {
            return DataNotFound();
        }

        string filePath = existingItem.ImageUrl;
        if (request.ImageFile != null)
        {
            Guid fileName = Guid.NewGuid();
            filePath = await FileService.SaveImageFileAsync(request.ImageFile,
                environment.WebRootPath,
                fileName);
        }

        bool updated = await Db.UpdateSliderItemAsync(
            connection,
            id,
            filePath,
            request.Target);

        if (!updated)
        {
            return ValidationError(new List<string>{"Failed to update slider item"});
        }

        return Success();
    }

    [HttpDelete("")]
    [Guard([Roles.Admin])]
    public async Task<IActionResult> Delete([FromQuery] int id)
    {
        using var connection = _dbFactory.CreateDbConnection();
        bool deleted = await Db.DeleteSliderItemAsync(connection, id);

        if (!deleted)
        {
            return DataNotFound();
        }

        return Success();
    }
}