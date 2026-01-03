using System;
using System.IO;
using System.Text.Json;
using System.Windows;
using MessageBox = System.Windows.MessageBox;
using Application = System.Windows.Application;

namespace wpfkiro20260101
{
    public class ForceSettingsRefresh
    {
        public static void RefreshSettings()
        {
            try
            {
                Console.WriteLine("=== 強制刷新設定 ===");
                
                // 1. 顯示當前設定
                var currentSettings = AppSettings.Instance;
                Console.WriteLine($"當前設定:");
                Console.WriteLine($"後端服務: {currentSettings.BackendService}");
                Console.WriteLine($"API URL: {currentSettings.ApiUrl}");
                Console.WriteLine($"Project ID: {currentSettings.ProjectId}");
                Console.WriteLine($"Database ID: {currentSettings.DatabaseId}");
                Console.WriteLine($"Bucket ID: {currentSettings.BucketId}");
                
                // 2. 強制重新載入設定
                Console.WriteLine($"\n強制重新載入設定...");
                AppSettings.ReloadSettings();
                
                // 3. 驗證新設定
                var newSettings = AppSettings.Instance;
                Console.WriteLine($"重新載入後的設定:");
                Console.WriteLine($"後端服務: {newSettings.BackendService}");
                Console.WriteLine($"API URL: {newSettings.ApiUrl}");
                Console.WriteLine($"Project ID: {newSettings.ProjectId}");
                Console.WriteLine($"Database ID: {newSettings.DatabaseId}");
                Console.WriteLine($"Bucket ID: {newSettings.BucketId}");
                
                // 4. 測試連線
                Console.WriteLine($"\n測試連線...");
                var service = Services.BackendServiceFactory.CreateCurrentService();
                Console.WriteLine($"創建的服務: {service.ServiceName}");
                
                // 5. 顯示設定檔案內容
                var settingsPath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "wpfkiro20260101",
                    "settings.json"
                );
                
                if (File.Exists(settingsPath))
                {
                    var fileContent = File.ReadAllText(settingsPath);
                    Console.WriteLine($"\n設定檔案內容:");
                    Console.WriteLine(fileContent);
                }
                
                Console.WriteLine($"\n=== 刷新完成 ===");
                
                // 如果是在 WPF 環境中，顯示訊息框
                if (Application.Current != null)
                {
                    MessageBox.Show($"設定已刷新！\n" +
                                  $"後端服務: {newSettings.BackendService}\n" +
                                  $"Project ID: {newSettings.ProjectId}\n" +
                                  $"API URL: {newSettings.ApiUrl}",
                                  "設定刷新", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"刷新設定時發生錯誤: {ex.Message}");
                if (Application.Current != null)
                {
                    MessageBox.Show($"刷新設定時發生錯誤: {ex.Message}", 
                                  "錯誤", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        
        public static void ValidateCurrentSettings()
        {
            try
            {
                var settings = AppSettings.Instance;
                Console.WriteLine("=== 設定驗證 ===");
                Console.WriteLine($"後端服務: {settings.BackendService}");
                Console.WriteLine($"API URL: {settings.ApiUrl}");
                Console.WriteLine($"Project ID: {settings.ProjectId}");
                Console.WriteLine($"API Key 長度: {settings.ApiKey?.Length ?? 0}");
                Console.WriteLine($"Database ID: {settings.DatabaseId}");
                Console.WriteLine($"Bucket ID: {settings.BucketId}");
                Console.WriteLine($"Food Collection ID: {settings.FoodCollectionId}");
                Console.WriteLine($"Subscription Collection ID: {settings.SubscriptionCollectionId}");
                
                // 檢查是否配置完整
                bool isConfigured = settings.IsConfigured();
                Console.WriteLine($"設定是否完整: {isConfigured}");
                
                if (!isConfigured)
                {
                    Console.WriteLine("⚠ 設定不完整，請檢查必要欄位");
                }
                else
                {
                    Console.WriteLine("✓ 設定看起來完整");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"驗證設定時發生錯誤: {ex.Message}");
            }
        }
    }
}