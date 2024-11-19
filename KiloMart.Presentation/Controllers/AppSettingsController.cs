using Microsoft.AspNetCore.Mvc;
using KiloMart.Core.Settings;

namespace KiloMart.API.Controllers
{
    [ApiController]
    [Route("api/admin/settings")]
    public class AppSettingsController : ControllerBase
    {
        private readonly IAppSettingsProvider _settingsProvider;

        public AppSettingsController(IAppSettingsProvider settingsProvider)
        {
            _settingsProvider = settingsProvider ?? throw new ArgumentNullException(nameof(settingsProvider));
        }

        [HttpGet("{key}")]
        public async Task<ActionResult<string>> GetSetting(ConstantSettings key)
        {
            var setting = await _settingsProvider.GetSettingAsync((int)key);
            if (setting is null)
            {
                return NotFound($"Setting with key {key} not found.");
            }
            return Ok(setting);
        }

        [HttpPut("{key}")]
        public async Task<IActionResult> UpdateSetting(ConstantSettings key, [FromBody] string value)
        {
            await _settingsProvider.UpdateSettingAsync((int)key, value);
            return NoContent();
        }

        [HttpPost("invalidate-cache")]
        public async Task<IActionResult> InvalidateCache()
        {
            await _settingsProvider.InvalidateCacheAsync();
            return NoContent();
        }

        [HttpGet("cancel-multi-provider-order")]
        public async Task<ActionResult<bool>> GetCancelWhenOrderFromMultiProviders()
        {
            var setting = await _settingsProvider.GetSettingAsync((int)ConstantSettings.CancelWhenOrderFromMultiProviders);
            if (setting == null)
            {
                return NotFound("Setting not found.");
            }
            return Ok(bool.Parse(setting));
        }

        [HttpPut("cancel-multi-provider-order")]
        public async Task<IActionResult> SetCancelWhenOrderFromMultiProviders([FromBody] bool value)
        {
            await _settingsProvider.UpdateSettingAsync((int)ConstantSettings.CancelWhenOrderFromMultiProviders, value.ToString());
            return NoContent();
        }
    }
}