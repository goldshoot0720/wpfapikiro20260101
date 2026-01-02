using System;
using System.IO;
using System.Text.Json;
using System.Windows;

namespace wpfkiro20260101
{
    public class FixSettingsProblem
    {
        public static void DiagnoseAndFix()
        {
            try
            {
                var settingsPath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "wpfkiro20260101",
                    "settings.json"
                );

                Console.WriteLine("=== 設定診斷工具 ===");
                Console.WriteLine($"設定檔案路徑: {settingsPath}");
                
                if (File.Exists(settingsPath))
                {
                    var jsonContent = File.ReadAllText(settingsPath);
                    Console.WriteLine($"當前設定檔案內容:\n{jsonContent}");
                    
                    // 載入當前設定
                    var currentSettings = AppSettings.Instance;
                    Console.WriteLine($"\n載入的設定:");
                    Console.WriteLine($"後端服務: {currentSettings.BackendService} ({(int)currentSettings.BackendService})");
                    Console.WriteLine($"API URL: {currentSettings.ApiUrl}");
                    Console.WriteLine($"Project ID: {currentSettings.ProjectId}");
                    Console.WriteLine($"API Key: {currentSettings.ApiKey.Substring(0, Math.Min(50, currentSettings.ApiKey.Length))}...");
                    
                    // 檢查是否需要修復
                    Console.WriteLine($"\n=== 修復建議 ===");
                    
                    if (currentSettings.BackendService == BackendServiceType.Appwrite)
                    {
                        Console.WriteLine("當前選擇的是 Appwrite 服務");
                        
                        // 檢查是否使用了新的帳戶資訊
                        if (currentSettings.ProjectId == "680c76af0037a7d23e44")
                        {
                            Console.WriteLine("✓ 已使用新的 Appwrite 帳戶資訊");
                            Console.WriteLine("設定看起來是正確的。");
                        }
                        else
                        {
                            Console.WriteLine("⚠ 使用的是舊的 Project ID");
                            Console.WriteLine("建議更新為新的帳戶資訊");
                        }
                    }
                }
                else
                {
                    Console.WriteLine("設定檔案不存在");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"診斷過程中發生錯誤: {ex.Message}");
            }
        }
    }
}