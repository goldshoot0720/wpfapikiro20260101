using System;

namespace wpfkiro20260101
{
    public class VerifySupabaseSettings
    {
        public static void CheckSettings()
        {
            Console.WriteLine("=== Supabase 設定驗證 ===");
            
            try
            {
                var settings = AppSettings.Instance;
                
                Console.WriteLine($"當前後端服務: {settings.BackendService}");
                Console.WriteLine($"API URL: {settings.ApiUrl}");
                Console.WriteLine($"Project ID: {settings.ProjectId}");
                Console.WriteLine($"API Key 前20字元: {settings.ApiKey.Substring(0, 20)}...");
                
                // 檢查是否為正確的 Supabase 設定
                bool isCorrectUrl = settings.ApiUrl == "https://lobezwpworbfktlkxuyo.supabase.co";
                bool isCorrectProjectId = settings.ProjectId == "lobezwpworbfktlkxuyo";
                bool isCorrectApiKey = settings.ApiKey.StartsWith("eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9");
                
                Console.WriteLine("\n=== 設定驗證結果 ===");
                Console.WriteLine($"✅ API URL 正確: {isCorrectUrl}");
                Console.WriteLine($"✅ Project ID 正確: {isCorrectProjectId}");
                Console.WriteLine($"✅ API Key 格式正確: {isCorrectApiKey}");
                
                if (isCorrectUrl && isCorrectProjectId && isCorrectApiKey)
                {
                    Console.WriteLine("\n🎉 所有 Supabase 設定都正確！");
                }
                else
                {
                    Console.WriteLine("\n⚠️ 發現設定問題，需要修正：");
                    
                    if (!isCorrectUrl)
                    {
                        Console.WriteLine($"   - API URL 不正確，應該是: https://lobezwpworbfktlkxuyo.supabase.co");
                        Console.WriteLine($"   - 當前值: {settings.ApiUrl}");
                    }
                    
                    if (!isCorrectProjectId)
                    {
                        Console.WriteLine($"   - Project ID 不正確，應該是: lobezwpworbfktlkxuyo");
                        Console.WriteLine($"   - 當前值: {settings.ProjectId}");
                    }
                    
                    if (!isCorrectApiKey)
                    {
                        Console.WriteLine($"   - API Key 格式不正確，應該以 eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9 開頭");
                        Console.WriteLine($"   - 當前值前20字元: {settings.ApiKey.Substring(0, Math.Min(20, settings.ApiKey.Length))}...");
                    }
                    
                    Console.WriteLine("\n正在自動修正設定...");
                    
                    // 自動修正設定
                    settings.BackendService = BackendServiceType.Supabase;
                    settings.ApiUrl = AppSettings.Defaults.Supabase.ApiUrl;
                    settings.ProjectId = AppSettings.Defaults.Supabase.ProjectId;
                    settings.ApiKey = AppSettings.Defaults.Supabase.ApiKey;
                    settings.DatabaseId = "";
                    settings.BucketId = "";
                    
                    settings.Save();
                    
                    Console.WriteLine("✅ 設定已自動修正並儲存");
                    
                    // 重新驗證
                    AppSettings.ReloadSettings();
                    var newSettings = AppSettings.Instance;
                    
                    Console.WriteLine("\n=== 修正後的設定 ===");
                    Console.WriteLine($"後端服務: {newSettings.BackendService}");
                    Console.WriteLine($"API URL: {newSettings.ApiUrl}");
                    Console.WriteLine($"Project ID: {newSettings.ProjectId}");
                    Console.WriteLine($"API Key 前20字元: {newSettings.ApiKey.Substring(0, 20)}...");
                }
                
                // 檢查預設值
                Console.WriteLine("\n=== 預設值檢查 ===");
                Console.WriteLine($"預設 API URL: {AppSettings.Defaults.Supabase.ApiUrl}");
                Console.WriteLine($"預設 Project ID: {AppSettings.Defaults.Supabase.ProjectId}");
                Console.WriteLine($"預設 API Key 前20字元: {AppSettings.Defaults.Supabase.ApiKey.Substring(0, 20)}...");
                
            }
            catch (Exception ex)
            {
                Console.WriteLine($"驗證設定時發生錯誤: {ex.Message}");
                Console.WriteLine($"詳細錯誤: {ex}");
            }
            
            Console.WriteLine("\n=== 驗證完成 ===");
        }
    }
}