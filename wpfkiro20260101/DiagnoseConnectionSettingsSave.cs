using System;
using System.IO;
using System.Text.Json;

namespace wpfkiro20260101
{
    /// <summary>
    /// 診斷連線設定儲存問題
    /// </summary>
    public class DiagnoseConnectionSettingsSave
    {
        public static void RunDiagnosis()
        {
            try
            {
                Console.WriteLine("=== 診斷連線設定儲存問題 ===\n");

                // 1. 檢查設定檔案路徑
                var settingsPath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "wpfkiro20260101",
                    "settings.json"
                );
                
                Console.WriteLine($"設定檔案路徑: {settingsPath}");
                Console.WriteLine($"檔案是否存在: {File.Exists(settingsPath)}");
                
                if (File.Exists(settingsPath))
                {
                    var fileInfo = new FileInfo(settingsPath);
                    Console.WriteLine($"檔案大小: {fileInfo.Length} bytes");
                    Console.WriteLine($"最後修改時間: {fileInfo.LastWriteTime}");
                }

                // 2. 載入當前設定
                Console.WriteLine("\n--- 當前設定狀態 ---");
                var settings = AppSettings.Instance;
                Console.WriteLine($"當前後端服務: {settings.BackendService}");
                Console.WriteLine($"當前 API URL: {settings.ApiUrl}");
                Console.WriteLine($"當前 Project ID: {settings.ProjectId}");
                Console.WriteLine($"當前 API Key: {(string.IsNullOrEmpty(settings.ApiKey) ? "空" : "已設定")}");

                // 3. 檢查各服務的獨立設定
                Console.WriteLine("\n--- 各服務獨立設定 ---");
                
                Console.WriteLine($"Appwrite:");
                Console.WriteLine($"  API URL: {settings.Appwrite.ApiUrl}");
                Console.WriteLine($"  Project ID: {settings.Appwrite.ProjectId}");
                Console.WriteLine($"  API Key: {(string.IsNullOrEmpty(settings.Appwrite.ApiKey) ? "空" : "已設定")}");
                Console.WriteLine($"  Database ID: {settings.Appwrite.DatabaseId}");
                Console.WriteLine($"  Bucket ID: {settings.Appwrite.BucketId}");

                Console.WriteLine($"Supabase:");
                Console.WriteLine($"  API URL: {settings.Supabase.ApiUrl}");
                Console.WriteLine($"  Project ID: {settings.Supabase.ProjectId}");
                Console.WriteLine($"  API Key: {(string.IsNullOrEmpty(settings.Supabase.ApiKey) ? "空" : "已設定")}");

                Console.WriteLine($"NHost:");
                Console.WriteLine($"  API URL: {settings.NHost.ApiUrl}");
                Console.WriteLine($"  Project ID: {settings.NHost.ProjectId}");
                Console.WriteLine($"  API Key: {(string.IsNullOrEmpty(settings.NHost.ApiKey) ? "空" : "已設定")}");

                // 4. 測試設定儲存
                Console.WriteLine("\n--- 測試設定儲存 ---");
                
                var originalService = settings.BackendService;
                var originalApiUrl = settings.ApiUrl;
                
                // 測試修改設定
                var testApiUrl = $"https://test-{DateTime.Now.Ticks}.example.com";
                settings.ApiUrl = testApiUrl;
                
                Console.WriteLine($"修改 API URL 為: {testApiUrl}");
                
                // 儲存設定
                settings.Save();
                Console.WriteLine("設定已儲存");
                
                // 重新載入設定驗證
                AppSettings.ReloadSettings();
                var reloadedSettings = AppSettings.Instance;
                
                Console.WriteLine($"重新載入後的 API URL: {reloadedSettings.ApiUrl}");
                Console.WriteLine($"設定是否正確儲存: {reloadedSettings.ApiUrl == testApiUrl}");
                
                // 恢復原始設定
                reloadedSettings.ApiUrl = originalApiUrl;
                reloadedSettings.Save();
                Console.WriteLine("已恢復原始設定");

                // 5. 檢查設定檔案內容
                if (File.Exists(settingsPath))
                {
                    Console.WriteLine("\n--- 設定檔案內容 ---");
                    var jsonContent = File.ReadAllText(settingsPath);
                    
                    try
                    {
                        // 格式化 JSON 以便閱讀
                        var jsonDoc = JsonDocument.Parse(jsonContent);
                        var formattedJson = JsonSerializer.Serialize(jsonDoc, new JsonSerializerOptions 
                        { 
                            WriteIndented = true 
                        });
                        Console.WriteLine(formattedJson);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"無法解析 JSON: {ex.Message}");
                        Console.WriteLine($"原始內容: {jsonContent}");
                    }
                }

                Console.WriteLine("\n=== 診斷完成 ===");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"診斷過程中發生錯誤: {ex.Message}");
                Console.WriteLine($"堆疊追蹤: {ex.StackTrace}");
            }
        }

        public static void TestServiceSwitching()
        {
            try
            {
                Console.WriteLine("\n=== 測試服務切換和設定儲存 ===\n");

                var settings = AppSettings.Instance;
                var originalService = settings.BackendService;
                
                Console.WriteLine($"原始服務: {originalService}");

                // 測試切換到不同服務並儲存設定
                var testServices = new[] 
                { 
                    BackendServiceType.Appwrite, 
                    BackendServiceType.Supabase, 
                    BackendServiceType.NHost 
                };

                foreach (var service in testServices)
                {
                    Console.WriteLine($"\n--- 測試 {service} ---");
                    
                    // 切換服務
                    settings.BackendService = service;
                    
                    // 設定測試值
                    var testUrl = $"https://{service.ToString().ToLower()}-test.example.com";
                    var testProjectId = $"{service.ToString().ToLower()}-project-123";
                    var testApiKey = $"{service.ToString().ToLower()}-key-456";
                    
                    settings.ApiUrl = testUrl;
                    settings.ProjectId = testProjectId;
                    settings.ApiKey = testApiKey;
                    
                    Console.WriteLine($"設定 API URL: {testUrl}");
                    Console.WriteLine($"設定 Project ID: {testProjectId}");
                    Console.WriteLine($"設定 API Key: {testApiKey}");
                    
                    // 儲存設定
                    settings.Save();
                    
                    // 重新載入驗證
                    AppSettings.ReloadSettings();
                    var reloadedSettings = AppSettings.Instance;
                    
                    Console.WriteLine($"重新載入後:");
                    Console.WriteLine($"  服務: {reloadedSettings.BackendService}");
                    Console.WriteLine($"  API URL: {reloadedSettings.ApiUrl}");
                    Console.WriteLine($"  Project ID: {reloadedSettings.ProjectId}");
                    Console.WriteLine($"  API Key: {reloadedSettings.ApiKey}");
                    
                    // 驗證設定是否正確
                    var isCorrect = reloadedSettings.BackendService == service &&
                                   reloadedSettings.ApiUrl == testUrl &&
                                   reloadedSettings.ProjectId == testProjectId &&
                                   reloadedSettings.ApiKey == testApiKey;
                    
                    Console.WriteLine($"設定是否正確: {isCorrect}");
                }

                // 恢復原始服務
                settings.BackendService = originalService;
                settings.Save();
                
                Console.WriteLine($"\n已恢復原始服務: {originalService}");
                Console.WriteLine("\n=== 服務切換測試完成 ===");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"測試過程中發生錯誤: {ex.Message}");
            }
        }
    }
}