using System.Text.Json;

namespace Tasker
{
    partial class Program
    {
        private static TaskerConfig config = new();

        public class ConfigColors
        {
            public string ErrorColor { get; set; }
            public string DescriptionColor { get; set; }
            public string TitleColor { get; set; }
            public string DefaultColor { get; set; }

            public ConfigColors()
            {
                ErrorColor = "lime";
                DescriptionColor = "teal";
                TitleColor = "white";
                DefaultColor = "silver";
            }
        }

        public class TaskerConfig
        {
            public bool UseColor { get; set; }
            public ConfigColors Colors { get; set; }
            public string TasksPath { get; set; }
            public string DonePath { get; set; }
            public bool Verbose { get; set; }

            public TaskerConfig()
            {
                UseColor = true;
                Colors = new ConfigColors();
                TasksPath = string.Empty;
                DonePath = string.Empty;
                Verbose = true;
            }

        }


        private static void LoadConfig()
        {
            var file = Path.Combine(AppContext.BaseDirectory, "tasker.cfg");

            if (!File.Exists(file))
            {
                // no config file found. Using defaults.
                return;
            }

            try
            {
                var text = File.ReadAllText(file);
                // Deserialize the JSON to a Config object
                TaskerConfig? _config = JsonSerializer.Deserialize<TaskerConfig>(text);
                if (_config != null)
                {
                    config = _config;
                }
            }
            catch (Exception ex)
            {
                GeneralError($"Error reading config file: {ex.Message}");
            }
        }

    }

}