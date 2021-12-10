using System;
using System.IO;
using System.Text.Json;
using GrayColorMatching.BL.Models;

namespace GrayColorMatching.BL.Services
{
    public class AppSettingsService : IAppSettingsService
    {
        private const string SettingsPath = "settings.json";

        public AppSettings Settings { get; set; }

        public AppSettingsService()
        {
            Settings = LoadSettings();
            VerifySettingsAndSetDefaultsIfNecessary();
        }

        public void SaveSettings()
        {
            using FileStream stream = new FileStream(SettingsPath, FileMode.Create);
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
            catch (FileNotFoundException)
            {
                return new AppSettings();
            }
        }

        private void VerifySettingsAndSetDefaultsIfNecessary()
        {
            if (Settings.Delta < 0 || Settings.Delta > 5)
                Settings.Delta = 0;

            if (Settings.Delta < 250 || Settings.MinWhiteComponent > 254)
                Settings.MinWhiteComponent = 254;

            if (Settings.MaxBlackComponent < 1 || Settings.MaxBlackComponent > 5)
                Settings.MaxBlackComponent = 1;
        }
    }
}
