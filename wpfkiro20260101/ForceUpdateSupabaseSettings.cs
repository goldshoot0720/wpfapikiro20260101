using System;
using System.IO;

namespace wpfkiro20260101
{
    public class ForceUpdateSupabaseSettings
    {
        public static void UpdateSettings()
        {
            try
            {
                Console.WriteLine("=== 強制更新 Supabase 設定 ===");
                
                // 獲取設定實例
                var settings = AppSettings.Instance;
                
                Console.WriteLine($"當前後端服務: {settings.BackendService}");
                Console.WriteLine($"當前 API URL: {settings.ApiUrl}");
                Console.WriteLine($"當前 Project ID: {settings.ProjectId}");
                Console.WriteLine($"當前 API Key: {settings.ApiKey.Substring(0, 20)}...");
                
                // 強制設定為 Supabase
                settings.BackendService = BackendServiceType.Supabase;
                settings.ApiUrl = AppSettings.Defaults.Supabase.ApiUrl;
                settings.ProjectId = AppSettings.Defaults.Supabase.ProjectId;
                settings.ApiKey = AppSettings.Defaults.Supabase.ApiKey;
                
                // 清空 Appwrite 專用欄位
                settings.DatabaseId = "";
                settings.BucketId = "";
                
                Console.WriteLine("\n更新後的設定:");
                Console.WriteLine($"後端服務: {settings.BackendService}");
                Console.WriteLine($"API URL: {settings.ApiUrl}");
                Console.WriteLine($"Project ID: {settings.ProjectId}");
                Console.WriteLine($"API Key: {settings.ApiKey.Substring(0, 20)}...");
                
                // 儲存設定
                settings.Save();
                Console.WriteLine("\n設定已儲存到檔案");
                
                // 驗證設定檔案內容
                var settingsPath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "wpfkiro20260101",
                    "settings.json"
                );
                
                if (File.Exists(settingsPath))
                {
                    var fileContent = File.ReadAllText(settingsPath);
                    Console.WriteLine($"\n設定檔案內容:\n{fileContent}");
                }
                else
                {
                    Console.WriteLine("\n設定檔案不存在！");
                }
                
                // 強制重新載入設定
                AppSettings.ReloadSettings();
                var reloadedSettings = AppSettings.Instance;
                
                Console.WriteLine("\n重新載入後的設定:");
                Console.WriteLine($"後端服務: {reloadedSettings.BackendService}");
                Console.WriteLine($"API URL: {reloadedSettings.ApiUrl}");
                Console.WriteLine($"Project ID: {reloadedSettings.ProjectId}");
                
                Console.WriteLine("\n=== 設定更新完成 ===");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"更新設定時發生錯誤: {ex.Message}");
                Console.WriteLine($"詳細錯誤: {ex}");
            }
        }
    }
}