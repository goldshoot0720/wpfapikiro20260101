using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace wpfkiro20260101
{
    public enum BackendServiceType
    {
        Appwrite,
        Supabase,
        NHost,
        Contentful,
        Back4App,
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
                            System.Diagnostics.Debug.WriteLine($"首次載入 AppSettings 實例 - 後端服務: {_instance.BackendService}");
                        }
                    }
                }
                return _instance;
            }
        }

        public BackendServiceType BackendService { get; set; } = BackendServiceType.Appwrite;
        public string ApiUrl { get; set; } = Defaults.Appwrite.ApiUrl;
        public string ProjectId { get; set; } = Defaults.Appwrite.ProjectId;
        public string ApiKey { get; set; } = Defaults.Appwrite.ApiKey;
        
        // Appwrite 專用設定
        public string DatabaseId { get; set; } = Defaults.Appwrite.DatabaseId;
        public string BucketId { get; set; } = Defaults.Appwrite.BucketId;
        public string FoodCollectionId { get; set; } = Defaults.Appwrite.FoodCollectionId;
        public string SubscriptionCollectionId { get; set; } = Defaults.Appwrite.SubscriptionCollectionId;

        // 預設設定值
        public static class Defaults
        {
            public static class Appwrite
            {
                public const string ApiUrl = "https://fra.cloud.appwrite.io/v1";
                public const string ProjectId = "69565017002c03b93af8";
                public const string DatabaseId = "69565a2800074e1d96c5";
                public const string BucketId = "6956530b0018bc91e180";
                public const string FoodCollectionId = "food";
                public const string SubscriptionCollectionId = "subscription";
                public const string ApiKey = "standard_bb04794eeaa3f7b9a993866f231d1b2146b595f3645c31fd61c17cd5a684b59440aa85f9fea44573f859dd33d607144167f13016be9de4004f60f9823d1adf63528ca120acceb77c53c1810b6fa811f65976cb5e3ac0d1c30a5824ce24e9884b8301708ae9339f4774fabd826273f99126f3c68f6d3520de50304226136bb1c5";
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

            public static class Back4App
            {
                public const string ApiUrl = "https://parseapi.back4app.com";
                public const string ProjectId = "your-app-id";
            }

            public static class MySQL
            {
                public const string ApiUrl = "localhost:3306";
                public const string ProjectId = "your-database-name";
            }
        }

        public static void ReloadSettings()
        {
            lock (_lock)
            {
                var oldInstance = _instance;
                _instance = Load();
                System.Diagnostics.Debug.WriteLine($"強制重新載入設定 - 舊後端服務: {oldInstance?.BackendService}, 新後端服務: {_instance.BackendService}");
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
                    // 移除 JsonStringEnumConverter，使用數字表示枚舉
                });

                File.WriteAllText(SettingsFilePath, json);
                System.Diagnostics.Debug.WriteLine($"設定已保存 - 後端服務: {BackendService}");
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
                    System.Diagnostics.Debug.WriteLine($"載入的 JSON 內容: {json}");
                    
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                        // 移除 JsonStringEnumConverter，使用數字反序列化枚舉
                    };
                    var settings = JsonSerializer.Deserialize<AppSettings>(json, options);
                    
                    // 調試：顯示載入的設定
                    System.Diagnostics.Debug.WriteLine($"從檔案載入的後端服務: {settings?.BackendService}");
                    
                    if (settings != null)
                    {
                        // 驗證枚舉值是否正確載入
                        System.Diagnostics.Debug.WriteLine($"枚舉值驗證: {settings.BackendService} == BackendServiceType.Supabase: {settings.BackendService == BackendServiceType.Supabase}");
                        return settings;
                    }
                }
            }
            catch (Exception ex)
            {
                // 如果載入失敗，返回預設設定
                System.Diagnostics.Debug.WriteLine($"載入設定失敗: {ex.Message}");
            }

            System.Diagnostics.Debug.WriteLine("返回預設設定");
            return new AppSettings();
        }

        public void Reset()
        {
            BackendService = BackendServiceType.Appwrite;
            ApiUrl = "";
            ProjectId = "";
            ApiKey = "";
            DatabaseId = "";
            BucketId = "";
        }

        public string GetDefaultApiUrl()
        {
            return BackendService switch
            {
                BackendServiceType.Appwrite => Defaults.Appwrite.ApiUrl,
                BackendServiceType.Supabase => Defaults.Supabase.ApiUrl,
                BackendServiceType.NHost => Defaults.NHost.ApiUrl,
                BackendServiceType.Contentful => Defaults.Contentful.ApiUrl,
                BackendServiceType.Back4App => Defaults.Back4App.ApiUrl,
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
                BackendServiceType.Back4App => Defaults.Back4App.ProjectId,
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
                BackendServiceType.Back4App => "Back4App",
                BackendServiceType.MySQL => "MySQL",
                _ => "未知"
            };
        }
    }
}