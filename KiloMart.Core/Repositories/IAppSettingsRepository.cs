using System.Threading.Tasks;

namespace KiloMart.Core.Repositories
{
    public interface IAppSettingsRepository
    {
        Task<string?> GetSettingAsync(int key);
        Task UpdateSettingAsync(int key, string value);
    }
}