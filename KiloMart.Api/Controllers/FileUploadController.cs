using KiloMart.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace KiloMart.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class FileUploadController : ControllerBase
{
    private readonly IWebHostEnvironment _environment;

    public FileUploadController(IWebHostEnvironment environment)
    {
        _environment = environment;
    }

    [HttpPost("upload")]
    public async Task<IActionResult> UploadFile(IFormFile file)
    {
        try
        {
            Guid fileName = Guid.NewGuid();
            var filePath = await FileService.SaveFileAsync(file,
                _environment.WebRootPath,
                fileName);

            return Ok($"File Name : {fileName}");
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch
        {
            return StatusCode(500, "An error occurred while uploading the file.");
        }
    }
}
