using System.IO;
using System.Text.Json;
using GrayColorMatching.BL.Models;

namespace GrayColorMatching.BL.Services
{
    public class AppSettingsService : IAppSettingsService
    {
        private const string SettingsPath = "Settings/settings.json";

        public AppSettings Settings { get; set; }

        public AppSettingsService()
        {
            Settings = LoadSettings();
        }

        public void SaveSettings()
        {
            using FileStream stream = new FileStream(SettingsPath, FileMode.CreateNew);
            using StreamWriter writer = new StreamWriter(stream);
            var settings = JsonSerializer.Serialize(Settings);
            writer.Write(settings);
            writer.Close();
            stream.Close();
        }

        private AppSettings LoadSettings()
        {
            try
            {
                using FileStream stream = new(SettingsPath, FileMode.Open);
                using StreamReader reader = new(stream);
                var settingsString = reader.ReadToEnd();
                reader.Close();
                stream.Close();
                return JsonSerializer.Deserialize<AppSettings>(settingsString);
            }
            catch (DirectoryNotFoundException)
            {
                return new AppSettings();
            }
        }
    }
}
