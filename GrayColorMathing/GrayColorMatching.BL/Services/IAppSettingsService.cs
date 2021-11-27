using GrayColorMatching.BL.Models;

namespace GrayColorMatching.BL.Services
{
    public interface IAppSettingsService
    {
        public void SaveSettings();

        public AppSettings Settings { get; }
    }
}
