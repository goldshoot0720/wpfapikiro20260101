using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using wpfkiro20260101.Services;

namespace wpfkiro20260101
{
    public enum BackendServiceType
    {
        Appwrite,
        Supabase,
        NHost,
        Contentful,
        Back4App,
        MySQL,
        Strapi,
        Sanity
    }

    // 服務設定介面
    public interface IServiceSettings
    {
        string ApiUrl { get; set; }
        string ProjectId { get; set; }
        string ApiKey { get; set; }
    }

    // 各服務的獨立設定類別
    public class AppwriteSettings : IServiceSettings
    {
        public string ApiUrl { get; set; } = "https://fra.cloud.appwrite.io/v1";
        public string ProjectId { get; set; } = "69565017002c03b93af8";
        public string ApiKey { get; set; } = "standard_bb04794eeaa3f7b9a993866f231d1b2146b595f3645c31fd61c17cd5a684b59440aa85f9fea44573f859dd33d607144167f13016be9de4004f60f9823d1adf63528ca120acceb77c53c1810b6fa811f65976cb5e3ac0d1c30a5824ce24e9884b8301708ae9339f4774fabd826273f99126f3c68f6d3520de50304226136bb1c5";
        public string DatabaseId { get; set; } = "69565a2800074e1d96c5";
        public string BucketId { get; set; } = "6956530b0018bc91e180";
        public string FoodCollectionId { get; set; } = "food";
        public string SubscriptionCollectionId { get; set; } = "subscription";
    }

    public class SupabaseSettings : IServiceSettings
    {
        public string ApiUrl { get; set; } = "https://lobezwpworbfktlkxuyo.supabase.co";
        public string ProjectId { get; set; } = "lobezwpworbfktlkxuyo";
        public string ApiKey { get; set; } = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6ImxvYmV6d3B3b3JiZmt0bGt4dXlvIiwicm9sZSI6InNlcnZpY2Vfcm9sZSIsImlhdCI6MTc2NzI1ODU5MSwiZXhwIjoyMDgyODM0NTkxfQ.tFcCP7kvcfV1CznhIHXBF0TenGlYD1XRlAWdCYYEnlc";
    }

    public class NHostSettings : IServiceSettings
    {
        public string ApiUrl { get; set; } = "https://your-project.nhost.run";
        public string ProjectId { get; set; } = "your-project-id";
        public string ApiKey { get; set; } = "";
    }

    public class ContentfulSettings : IServiceSettings
    {
        public string ApiUrl { get; set; } = "https://api.contentful.com";
        public string ProjectId { get; set; } = "your-space-id";
        public string ApiKey { get; set; } = "";
    }

    public class Back4AppSettings : IServiceSettings
    {
        public string ApiUrl { get; set; } = "https://parseapi.back4app.com";
        public string ProjectId { get; set; } = "your-app-id";
        public string ApiKey { get; set; } = "";
    }

    public class MySQLSettings : IServiceSettings
    {
        public string ApiUrl { get; set; } = "localhost:3306";
        public string ProjectId { get; set; } = "your-database-name";
        public string ApiKey { get; set; } = "";
    }

    public class StrapiSettings : IServiceSettings
    {
        public string ApiUrl { get; set; } = "http://localhost:1337";
        public string ProjectId { get; set; } = "your-strapi-project";
        public string ApiKey { get; set; } = "your-strapi-api-token";
    }

    public class SanitySettings : IServiceSettings
    {
        public string ApiUrl { get; set; } = "https://your-project.api.sanity.io";
        public string ProjectId { get; set; } = "your-sanity-project-id";
        public string ApiKey { get; set; } = "your-sanity-token";
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

        // 當前選擇的後端服務
        public BackendServiceType BackendService { get; set; } = BackendServiceType.Appwrite;
        
        // 各服務的獨立設定
        public AppwriteSettings Appwrite { get; set; } = new AppwriteSettings();
        public SupabaseSettings Supabase { get; set; } = new SupabaseSettings();
        public NHostSettings NHost { get; set; } = new NHostSettings();
        public ContentfulSettings Contentful { get; set; } = new ContentfulSettings();
        public Back4AppSettings Back4App { get; set; } = new Back4AppSettings();
        public MySQLSettings MySQL { get; set; } = new MySQLSettings();
        public StrapiSettings Strapi { get; set; } = new StrapiSettings();
        public SanitySettings Sanity { get; set; } = new SanitySettings();

        // 向後相容的屬性 - 返回當前選擇服務的設定
        public string ApiUrl 
        { 
            get => GetCurrentServiceSettings().ApiUrl;
            set => GetCurrentServiceSettings().ApiUrl = value;
        }
        
        public string ProjectId 
        { 
            get => GetCurrentServiceSettings().ProjectId;
            set => GetCurrentServiceSettings().ProjectId = value;
        }
        
        public string ApiKey 
        { 
            get => GetCurrentServiceSettings().ApiKey;
            set => GetCurrentServiceSettings().ApiKey = value;
        }
        
        // Appwrite 專用設定（向後相容）
        public string DatabaseId 
        { 
            get => Appwrite.DatabaseId;
            set => Appwrite.DatabaseId = value;
        }
        
        public string BucketId 
        { 
            get => Appwrite.BucketId;
            set => Appwrite.BucketId = value;
        }
        
        public string FoodCollectionId 
        { 
            get => Appwrite.FoodCollectionId;
            set => Appwrite.FoodCollectionId = value;
        }
        
        public string SubscriptionCollectionId 
        { 
            get => Appwrite.SubscriptionCollectionId;
            set => Appwrite.SubscriptionCollectionId = value;
        }

        // 獲取當前服務的設定物件
        public IServiceSettings GetCurrentServiceSettings()
        {
            return BackendService switch
            {
                BackendServiceType.Appwrite => Appwrite,
                BackendServiceType.Supabase => Supabase,
                BackendServiceType.NHost => NHost,
                BackendServiceType.Contentful => Contentful,
                BackendServiceType.Back4App => Back4App,
                BackendServiceType.MySQL => MySQL,
                BackendServiceType.Strapi => Strapi,
                BackendServiceType.Sanity => Sanity,
                _ => Appwrite
            };
        }

        // 預設設定值（向後相容）
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
                public const string ApiUrl = "https://lobezwpworbfktlkxuyo.supabase.co";
                public const string ProjectId = "lobezwpworbfktlkxuyo";
                public const string ApiKey = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6ImxvYmV6d3B3b3JiZmt0bGt4dXlvIiwicm9sZSI6InNlcnZpY2Vfcm9sZSIsImlhdCI6MTc2NzI1ODU5MSwiZXhwIjoyMDgyODM0NTkxfQ.tFcCP7kvcfV1CznhIHXBF0TenGlYD1XRlAWdCYYEnlc";
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

            public static class Strapi
            {
                public const string ApiUrl = "http://localhost:1337";
                public const string ProjectId = "your-strapi-project";
                public const string ApiKey = "your-strapi-api-token";
            }

            public static class Sanity
            {
                public const string ApiUrl = "https://your-project.api.sanity.io";
                public const string ProjectId = "your-sanity-project-id";
                public const string ApiKey = "your-sanity-token";
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

        public AppSettings()
        {
            // 訂閱設定更新事件
            SettingsProfileService.OnSettingsUpdated += OnSettingsChanged;
        }

        private void OnSettingsChanged()
        {
            // 觸發設定變更事件，通知 UI 更新
            SettingsChanged?.Invoke();
        }

        // 設定變更事件，供 UI 組件訂閱
        public static event Action? SettingsChanged;

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
                BackendServiceType.Appwrite => new AppwriteSettings().ApiUrl,
                BackendServiceType.Supabase => new SupabaseSettings().ApiUrl,
                BackendServiceType.NHost => new NHostSettings().ApiUrl,
                BackendServiceType.Contentful => new ContentfulSettings().ApiUrl,
                BackendServiceType.Back4App => new Back4AppSettings().ApiUrl,
                BackendServiceType.MySQL => new MySQLSettings().ApiUrl,
                BackendServiceType.Strapi => new StrapiSettings().ApiUrl,
                BackendServiceType.Sanity => new SanitySettings().ApiUrl,
                _ => ""
            };
        }

        public string GetDefaultProjectId()
        {
            return BackendService switch
            {
                BackendServiceType.Appwrite => new AppwriteSettings().ProjectId,
                BackendServiceType.Supabase => new SupabaseSettings().ProjectId,
                BackendServiceType.NHost => new NHostSettings().ProjectId,
                BackendServiceType.Contentful => new ContentfulSettings().ProjectId,
                BackendServiceType.Back4App => new Back4AppSettings().ProjectId,
                BackendServiceType.MySQL => new MySQLSettings().ProjectId,
                BackendServiceType.Strapi => new StrapiSettings().ProjectId,
                BackendServiceType.Sanity => new SanitySettings().ProjectId,
                _ => ""
            };
        }

        public string GetDefaultApiKey()
        {
            return BackendService switch
            {
                BackendServiceType.Appwrite => new AppwriteSettings().ApiKey,
                BackendServiceType.Supabase => new SupabaseSettings().ApiKey,
                BackendServiceType.Strapi => new StrapiSettings().ApiKey,
                BackendServiceType.Sanity => new SanitySettings().ApiKey,
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
                BackendServiceType.Strapi => "Strapi",
                BackendServiceType.Sanity => "Sanity",
                _ => "未知"
            };
        }
    }
}