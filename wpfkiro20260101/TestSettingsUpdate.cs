using System;
using System.IO;
using System.Text.Json;

namespace wpfkiro20260101
{
    public class TestSettingsUpdate
    {
        public static void RunTest()
        {
            Console.WriteLine("=== 設定更新測試 ===");
            
            try
            {
                // 1. 檢查當前設定
                var settings = AppSettings.Instance;
                Console.WriteLine($"步驟 1 - 當前設定:");
                Console.WriteLine($"後端服務: {settings.BackendService}");
                Console.WriteLine($"API URL: {settings.ApiUrl}");
                Console.WriteLine($"Project ID: {settings.ProjectId}");
                Console.WriteLine($"Database ID: {settings.DatabaseId}");
                Console.WriteLine($"Bucket ID: {settings.BucketId}");
                Console.WriteLine($"Food Collection ID: {settings.FoodCollectionId}");
                Console.WriteLine($"Subscription Collection ID: {settings.SubscriptionCollectionId}");
                
                // 2. 強制重新載入設定
                Console.WriteLine($"\n步驟 2 - 強制重新載入設定");
                AppSettings.ReloadSettings();
                var reloadedSettings = AppSettings.Instance;
                
                Console.WriteLine($"重新載入後的設定:");
                Console.WriteLine($"後端服務: {reloadedSettings.BackendService}");
                Console.WriteLine($"API URL: {reloadedSettings.ApiUrl}");
                Console.WriteLine($"Project ID: {reloadedSettings.ProjectId}");
                
                // 3. 檢查設定檔案內容
                var settingsPath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "wpfkiro20260101",
                    "settings.json"
                );
                
                if (File.Exists(settingsPath))
                {
                    var fileContent = File.ReadAllText(settingsPath);
                    Console.WriteLine($"\n步驟 3 - 設定檔案內容:");
                    Console.WriteLine(fileContent);
                    
                    // 4. 解析 JSON 檢查數值
                    var jsonDoc = JsonDocument.Parse(fileContent);
                    var backendServiceValue = jsonDoc.RootElement.GetProperty("BackendService").GetInt32();
                    Console.WriteLine($"\n步驟 4 - JSON 解析結果:");
                    Console.WriteLine($"BackendService 數值: {backendServiceValue}");
                    Console.WriteLine($"對應的枚舉: {(BackendServiceType)backendServiceValue}");
                }
                
                Console.WriteLine($"\n=== 測試完成 ===");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"測試過程中發生錯誤: {ex.Message}");
                Console.WriteLine($"錯誤詳情: {ex.StackTrace}");
            }
        }
    }
}