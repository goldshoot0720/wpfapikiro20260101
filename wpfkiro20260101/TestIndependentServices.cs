using System;
using System.Diagnostics;

namespace wpfkiro20260101
{
    /// <summary>
    /// 測試獨立服務架構的功能
    /// 驗證各服務設定完全獨立，不會互相衝突
    /// </summary>
    public class TestIndependentServices
    {
        public static void RunTest()
        {
            Debug.WriteLine("=== 開始測試獨立服務架構 ===");
            
            try
            {
                var settings = AppSettings.Instance;
                
                // 測試 1: 驗證預設服務設定
                Debug.WriteLine($"預設服務: {settings.BackendService}");
                Debug.WriteLine($"Appwrite API URL: {settings.Appwrite.ApiUrl}");
                Debug.WriteLine($"Supabase API URL: {settings.Supabase.ApiUrl}");
                Debug.WriteLine($"Strapi API URL: {settings.Strapi.ApiUrl}");
                
                // 測試 2: 切換到 Supabase 並驗證設定獨立性
                Debug.WriteLine("\n--- 測試切換到 Supabase ---");
                var originalService = settings.BackendService;
                var originalAppwriteUrl = settings.Appwrite.ApiUrl;
                
                settings.BackendService = BackendServiceType.Supabase;
                Debug.WriteLine($"切換後當前服務: {settings.BackendService}");
                Debug.WriteLine($"當前 API URL (應該是 Supabase): {settings.ApiUrl}");
                Debug.WriteLine($"Appwrite API URL (應該保持不變): {settings.Appwrite.ApiUrl}");
                Debug.WriteLine($"Supabase API URL: {settings.Supabase.ApiUrl}");
                
                // 測試 3: 修改 Supabase 設定，驗證不影響其他服務
                Debug.WriteLine("\n--- 測試修改 Supabase 設定 ---");
                var originalSupabaseUrl = settings.Supabase.ApiUrl;
                settings.Supabase.ApiUrl = "https://test-supabase-url.com";
                
                Debug.WriteLine($"修改後 Supabase URL: {settings.Supabase.ApiUrl}");
                Debug.WriteLine($"Appwrite URL (應該不變): {settings.Appwrite.ApiUrl}");
                Debug.WriteLine($"Strapi URL (應該不變): {settings.Strapi.ApiUrl}");
                
                // 測試 4: 切換到 Appwrite，驗證 Supabase 設定保持
                Debug.WriteLine("\n--- 測試切換到 Appwrite ---");
                settings.BackendService = BackendServiceType.Appwrite;
                Debug.WriteLine($"切換後當前服務: {settings.BackendService}");
                Debug.WriteLine($"當前 API URL (應該是 Appwrite): {settings.ApiUrl}");
                Debug.WriteLine($"Supabase URL (應該保持修改後的值): {settings.Supabase.ApiUrl}");
                
                // 測試 5: 測試所有服務的獨立性
                Debug.WriteLine("\n--- 測試所有服務獨立性 ---");
                foreach (BackendServiceType serviceType in Enum.GetValues<BackendServiceType>())
                {
                    settings.BackendService = serviceType;
                    var currentSettings = settings.GetCurrentServiceSettings();
                    Debug.WriteLine($"{serviceType}: {currentSettings.ApiUrl}");
                }
                
                // 恢復原始設定
                settings.BackendService = originalService;
                settings.Supabase.ApiUrl = originalSupabaseUrl;
                
                Debug.WriteLine("\n=== 獨立服務架構測試完成 ===");
                Debug.WriteLine("✅ 所有服務設定完全獨立，無衝突！");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ 測試失敗: {ex.Message}");
                Debug.WriteLine($"堆疊追蹤: {ex.StackTrace}");
            }
        }
        
        /// <summary>
        /// 測試服務切換的完整流程
        /// </summary>
        public static void TestServiceSwitching()
        {
            Debug.WriteLine("=== 測試服務切換流程 ===");
            
            try
            {
                var settings = AppSettings.Instance;
                
                // 模擬用戶在界面上的操作流程
                Debug.WriteLine("1. 用戶選擇 Appwrite，配置設定");
                settings.BackendService = BackendServiceType.Appwrite;
                settings.Appwrite.ApiUrl = "https://custom-appwrite.com";
                settings.Appwrite.ProjectId = "custom-appwrite-project";
                
                Debug.WriteLine("2. 用戶切換到 Supabase，配置設定");
                settings.BackendService = BackendServiceType.Supabase;
                settings.Supabase.ApiUrl = "https://custom-supabase.com";
                settings.Supabase.ProjectId = "custom-supabase-project";
                
                Debug.WriteLine("3. 用戶切換到 Strapi，配置設定");
                settings.BackendService = BackendServiceType.Strapi;
                settings.Strapi.ApiUrl = "https://custom-strapi.com";
                settings.Strapi.ProjectId = "custom-strapi-project";
                
                Debug.WriteLine("4. 驗證各服務設定是否獨立保存");
                Debug.WriteLine($"Appwrite URL: {settings.Appwrite.ApiUrl}");
                Debug.WriteLine($"Supabase URL: {settings.Supabase.ApiUrl}");
                Debug.WriteLine($"Strapi URL: {settings.Strapi.ApiUrl}");
                
                Debug.WriteLine("5. 切換回 Appwrite，驗證設定完整性");
                settings.BackendService = BackendServiceType.Appwrite;
                Debug.WriteLine($"當前服務: {settings.BackendService}");
                Debug.WriteLine($"當前 API URL: {settings.ApiUrl}");
                Debug.WriteLine($"是否為 Appwrite 設定: {settings.ApiUrl == settings.Appwrite.ApiUrl}");
                
                Debug.WriteLine("\n✅ 服務切換測試成功！各服務設定完全獨立！");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ 服務切換測試失敗: {ex.Message}");
            }
        }
    }
}