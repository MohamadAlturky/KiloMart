//using KiloMart.Api.Authentication;
//using KiloMart.Api.Services;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.AspNetCore.Mvc;

//namespace KiloMart.Api.Controllers;

//[Route("api/[controller]")]
//[ApiController]
//public class FileUploadController : ControllerBase
//{
//    private readonly IWebHostEnvironment _environment;
//    private readonly UserManager<MemberShipUser> _userManager;

//    public FileUploadController(IWebHostEnvironment environment, UserManager<MemberShipUser> userManager)
//    {
//        _environment=environment;
//        _userManager=userManager;
//    }

//    [HttpPost("upload")]
//    public async Task<IActionResult> UploadFile(IFormFile file)
//    {
//        try
//        {
//            Guid fileName = Guid.NewGuid();
//            var filePath = await FileService.SaveFileAsync(file,
//                _environment.WebRootPath,
//                fileName);

//            return Ok($"File Name : {fileName}");
//        }
//        catch (ArgumentException ex)
//        {
//            return BadRequest(ex.Message);
//        }
//        catch
//        {
//            return StatusCode(500, "An error occurred while uploading the file.");
//        }
//    }
//    public class ConfirmEmailDto
//    {
//        public string Email { get; set; }
//        public string Token { get; set; }
//    }
//    [HttpPost("ConfirmEmail")]
//    public async Task<IActionResult> ConfirmEmail([FromBody] ConfirmEmailDto model)
//    {
//        var user = await _userManager.FindByEmailAsync(model.Email);
//        if (user == null)
//            return BadRequest("Invalid user.");

//        // Retrieve the saved token
//        var savedToken = await _userManager.GetAuthenticationTokenAsync(user, "Default", "EmailConfirmationToken");
//        if (savedToken != model.Token)
//            return BadRequest("Invalid token.");

//        // Confirm the email
//        user.EmailConfirmed = true;
//        await _userManager.UpdateAsync(user);

//        // Optionally, remove the token after confirmation
//        await _userManager.RemoveAuthenticationTokenAsync(user, "Default", "EmailConfirmationToken");

//        return Ok("Email confirmed successfully.");
//    }

//    public static string GenerateEmailConfirmationToken()
//    {
//        Random random = new Random();
//        return random.Next(1000, 9999).ToString("D4"); // Generates a 4-digit token (e.g., "1234")
//    }
//    public async Task SendEmailConfirmationAsync(UserManager<MemberShipUser> userManager, MemberShipUser user, string emailToken)
//    {
//        var token = GenerateEmailConfirmationToken();

//        // Save the token in the user’s claims or database
//        await userManager.SetAuthenticationTokenAsync(user, "Default", "EmailConfirmationToken", token);

//        // Prepare the email message
//        string emailBody = $"Your email confirmation code is: {token}";

//        // Use an email service to send the email
//        //await emailService.SendEmailAsync(user.Email, "Email Confirmation", emailBody);
//    }


//}
//public class ConfirmEmailDto
//{
//    public string Email { get; set; }
//    public string Token { get; set; }
//}
