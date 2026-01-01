using System;
using System.IO;
using System.Text.Json;

namespace wpfkiro20260101
{
    public enum BackendServiceType
    {
        Appwrite,
        Supabase,
        NHost,
        Contentful,
        MySQL
    }

    public class AppSettings
    {
        private static AppSettings? _instance;
        private static readonly object _lock = new object();
        private static readonly string SettingsFilePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "wpfkiro20260101",
            "settings.json"
        );

        public static AppSettings Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = Load();
                        }
                    }
                }
                return _instance;
            }
        }

        public BackendServiceType BackendService { get; set; } = BackendServiceType.Appwrite;
        public string ApiUrl { get; set; } = "";
        public string ProjectId { get; set; } = "";
        public string ApiKey { get; set; } = "";

        // 預設設定值
        public static class Defaults
        {
            public static class Appwrite
            {
                public const string ApiUrl = "https://cloud.appwrite.io/v1";
                public const string ProjectId = "your-project-id";
            }

            public static class Supabase
            {
                public const string ApiUrl = "https://your-project.supabase.co";
            }

            public static class NHost
            {
                public const string ApiUrl = "https://your-project.nhost.run";
                public const string ProjectId = "your-project-id";
            }

            public static class Contentful
            {
                public const string ApiUrl = "https://api.contentful.com";
                public const string ProjectId = "your-space-id";
            }

            public static class MySQL
            {
                public const string ApiUrl = "localhost:3306";
                public const string ProjectId = "your-database-name";
            }
        }

        private AppSettings() { }

        public void Save()
        {
            try
            {
                var directory = Path.GetDirectoryName(SettingsFilePath);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory!);
                }

                var json = JsonSerializer.Serialize(this, new JsonSerializerOptions
                {
                    WriteIndented = true
                });

                File.WriteAllText(SettingsFilePath, json);
            }
            catch (Exception ex)
            {
                throw new Exception($"無法儲存設定檔：{ex.Message}", ex);
            }
        }

        private static AppSettings Load()
        {
            try
            {
                if (File.Exists(SettingsFilePath))
                {
                    var json = File.ReadAllText(SettingsFilePath);
                    var settings = JsonSerializer.Deserialize<AppSettings>(json);
                    return settings ?? new AppSettings();
                }
            }
            catch (Exception)
            {
                // 如果載入失敗，返回預設設定
            }

            return new AppSettings();
        }

        public void Reset()
        {
            BackendService = BackendServiceType.Appwrite;
            ApiUrl = "";
            ProjectId = "";
            ApiKey = "";
        }

        public string GetDefaultApiUrl()
        {
            return BackendService switch
            {
                BackendServiceType.Appwrite => Defaults.Appwrite.ApiUrl,
                BackendServiceType.Supabase => Defaults.Supabase.ApiUrl,
                BackendServiceType.NHost => Defaults.NHost.ApiUrl,
                BackendServiceType.Contentful => Defaults.Contentful.ApiUrl,
                BackendServiceType.MySQL => Defaults.MySQL.ApiUrl,
                _ => ""
            };
        }

        public string GetDefaultProjectId()
        {
            return BackendService switch
            {
                BackendServiceType.Appwrite => Defaults.Appwrite.ProjectId,
                BackendServiceType.NHost => Defaults.NHost.ProjectId,
                BackendServiceType.Contentful => Defaults.Contentful.ProjectId,
                BackendServiceType.MySQL => Defaults.MySQL.ProjectId,
                _ => ""
            };
        }

        public bool IsConfigured()
        {
            return !string.IsNullOrWhiteSpace(ApiUrl) && 
                   !string.IsNullOrWhiteSpace(ProjectId) && 
                   !string.IsNullOrWhiteSpace(ApiKey);
        }

        public string GetServiceDisplayName()
        {
            return BackendService switch
            {
                BackendServiceType.Appwrite => "Appwrite",
                BackendServiceType.Supabase => "Supabase",
                BackendServiceType.NHost => "NHost",
                BackendServiceType.Contentful => "Contentful",
                BackendServiceType.MySQL => "MySQL",
                _ => "未知"
            };
        }
    }
}