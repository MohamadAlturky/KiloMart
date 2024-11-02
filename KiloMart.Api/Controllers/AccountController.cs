using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System;
using KiloMart.Api.Authentication;

[ApiController]
[Route("api/[controller]")]
public class AccountController : ControllerBase
{
    private readonly UserManager<MemberShipUser> _userManager;

    public AccountController(UserManager<MemberShipUser> userManager)
    {
        _userManager = userManager;
    }

    // Action to generate a verification token
    [HttpPost("GenerateVerificationToken")]
    public async Task<IActionResult> GenerateVerificationToken([FromBody] GenerateTokenDto model)
    {
        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null)
            return BadRequest("User not found.");

        // Generate a 4-digit random token
        var token = GenerateEmailConfirmationToken();

        // Save the token in the user’s tokens
        await _userManager.SetAuthenticationTokenAsync(user, "Default", "EmailConfirmationToken", token);

        // Optionally, send this token via email if you want to
        // (Uncomment and configure the emailService if needed)
        // await emailService.SendEmailAsync(user.Email, "Your Verification Code", $"Your verification code is: {token}");

        // Return the token (for testing or demo purposes only - avoid this in production)
        return Ok(new { Token = token });
    }

    // Action to activate the account using the token
    [HttpPost("ActivateAccount")]
    public async Task<IActionResult> ActivateAccount([FromBody] ConfirmEmailDto model)
    {
        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null)
            return BadRequest("Invalid user.");

        // Retrieve the saved token
        var savedToken = await _userManager.GetAuthenticationTokenAsync(user, "Default", "EmailConfirmationToken");
        if (savedToken != model.Token)
            return BadRequest("Invalid token.");

        // Confirm the email
        user.EmailConfirmed = true;
        var result = await _userManager.UpdateAsync(user);

        if (!result.Succeeded)
            return StatusCode(500, "Failed to confirm email.");

        // Optionally, remove the token after confirmation
        await _userManager.RemoveAuthenticationTokenAsync(user, "Default", "EmailConfirmationToken");

        return Ok("Account activated successfully.");
    }

    // Helper function to generate a 4-digit token
    private static string GenerateEmailConfirmationToken()
    {
        Random random = new Random();
        return random.Next(1000, 9999).ToString("D4"); // Generates a 4-digit token
    }
}

public class GenerateTokenDto
{
    public string Email { get; set; }
}

public class ConfirmEmailDto
{
    public string Email { get; set; }
    public string Token { get; set; }
}
