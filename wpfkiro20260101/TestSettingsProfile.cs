using System;
using System.Threading.Tasks;
using wpfkiro20260101.Models;
using wpfkiro20260101.Services;

namespace wpfkiro20260101
{
    public class TestSettingsProfile
    {
        public static async Task RunTests()
        {
            try
            {
                Console.WriteLine("=== 設定檔管理功能測試 ===");
                
                var profileService = SettingsProfileService.Instance;
                
                // 測試 1: 創建設定檔
                Console.WriteLine("\n1. 測試創建設定檔...");
                var result1 = await profileService.CreateFromCurrentSettingsAsync("測試設定檔", "這是一個測試設定檔");
                if (result1.Success)
                {
                    Console.WriteLine($"✓ 設定檔創建成功: {result1.Data?.ProfileName}");
                }
                else
                {
                    Console.WriteLine($"✗ 設定檔創建失敗: {result1.ErrorMessage}");
                }
                
                // 測試 2: 獲取所有設定檔
                Console.WriteLine("\n2. 測試獲取所有設定檔...");
                var profiles = await profileService.GetAllProfilesAsync();
                Console.WriteLine($"✓ 找到 {profiles.Count} 個設定檔");
                
                foreach (var profile in profiles)
                {
                    Console.WriteLine($"  - {profile.ProfileName} ({profile.BackendService})");
                }
                
                // 測試 3: 測試數量限制
                Console.WriteLine("\n3. 測試數量限制...");
                Console.WriteLine($"✓ 當前設定檔數量: {profileService.GetProfileCount()}/100");
                Console.WriteLine($"✓ 可以添加更多設定檔: {profileService.CanAddMoreProfiles()}");
                
                // 測試 4: 匯出設定檔
                Console.WriteLine("\n4. 測試匯出設定檔...");
                var exportResult = await profileService.ExportProfilesAsync();
                if (exportResult.Success)
                {
                    Console.WriteLine($"✓ 匯出成功，JSON 長度: {exportResult.Data?.Length ?? 0} 字元");
                }
                else
                {
                    Console.WriteLine($"✗ 匯出失敗: {exportResult.ErrorMessage}");
                }
                
                // 測試 5: 驗證設定檔資料
                if (profiles.Count > 0)
                {
                    var firstProfile = profiles[0];
                    Console.WriteLine("\n5. 驗證設定檔資料...");
                    Console.WriteLine($"✓ ID: {firstProfile.Id}");
                    Console.WriteLine($"✓ 名稱: {firstProfile.ProfileName}");
                    Console.WriteLine($"✓ 後端服務: {firstProfile.BackendService}");
                    Console.WriteLine($"✓ API URL: {firstProfile.ApiUrl}");
                    Console.WriteLine($"✓ 創建時間: {firstProfile.CreatedAt}");
                }
                
                Console.WriteLine("\n=== 測試完成 ===");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n✗ 測試過程中發生錯誤: {ex.Message}");
                Console.WriteLine($"詳細錯誤: {ex}");
            }
        }
        
        public static async Task TestAppwriteIntegration()
        {
            try
            {
                Console.WriteLine("\n=== Appwrite 整合測試 ===");
                
                var appwriteService = new AppwriteService();
                
                // 測試連線
                Console.WriteLine("1. 測試 Appwrite 連線...");
                var connectionResult = await appwriteService.TestConnectionAsync();
                Console.WriteLine($"連線結果: {(connectionResult ? "成功" : "失敗")}");
                
                if (connectionResult)
                {
                    // 測試設定檔 CRUD 操作
                    Console.WriteLine("\n2. 測試設定檔 CRUD 操作...");
                    
                    // 創建測試設定檔
                    var testProfile = new SettingsProfile
                    {
                        ProfileName = "Appwrite 測試設定檔",
                        BackendService = BackendServiceType.Appwrite,
                        ApiUrl = "https://fra.cloud.appwrite.io/v1",
                        ProjectId = "test-project",
                        ApiKey = "test-key",
                        Description = "這是一個 Appwrite 測試設定檔"
                    };
                    
                    var createResult = await appwriteService.CreateSettingsProfileAsync(testProfile);
                    if (createResult.Success)
                    {
                        Console.WriteLine("✓ 設定檔創建成功");
                        
                        // 獲取設定檔列表
                        var getResult = await appwriteService.GetSettingsProfilesAsync();
                        if (getResult.Success)
                        {
                            Console.WriteLine($"✓ 獲取設定檔成功，共 {getResult.Data?.Length ?? 0} 筆");
                        }
                        else
                        {
                            Console.WriteLine($"✗ 獲取設定檔失敗: {getResult.ErrorMessage}");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"✗ 設定檔創建失敗: {createResult.ErrorMessage}");
                    }
                }
                
                Console.WriteLine("\n=== Appwrite 整合測試完成 ===");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n✗ Appwrite 整合測試失敗: {ex.Message}");
            }
        }
    }
}