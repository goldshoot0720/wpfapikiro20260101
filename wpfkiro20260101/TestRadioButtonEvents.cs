using System;

namespace wpfkiro20260101
{
    public class TestRadioButtonEvents
    {
        public static void DebugRadioButtonState()
        {
            Console.WriteLine("=== RadioButton 狀態調試 ===");
            
            try
            {
                var settings = AppSettings.Instance;
                
                Console.WriteLine($"當前 AppSettings 後端服務: {settings.BackendService}");
                Console.WriteLine($"當前 AppSettings API URL: {settings.ApiUrl}");
                Console.WriteLine($"當前 AppSettings Project ID: {settings.ProjectId}");
                
                Console.WriteLine("\n=== Supabase 預設值 ===");
                Console.WriteLine($"預設 API URL: {AppSettings.Defaults.Supabase.ApiUrl}");
                Console.WriteLine($"預設 Project ID: {AppSettings.Defaults.Supabase.ProjectId}");
                Console.WriteLine($"預設 API Key 前20字元: {AppSettings.Defaults.Supabase.ApiKey.Substring(0, 20)}...");
                
                // 檢查是否為正確的 Supabase 設定
                bool isSupabaseSelected = settings.BackendService == BackendServiceType.Supabase;
                bool hasCorrectUrl = settings.ApiUrl == AppSettings.Defaults.Supabase.ApiUrl;
                bool hasCorrectProjectId = settings.ProjectId == AppSettings.Defaults.Supabase.ProjectId;
                bool hasCorrectApiKey = settings.ApiKey == AppSettings.Defaults.Supabase.ApiKey;
                
                Console.WriteLine("\n=== 設定檢查結果 ===");
                Console.WriteLine($"✅ Supabase 已選擇: {isSupabaseSelected}");
                Console.WriteLine($"✅ API URL 正確: {hasCorrectUrl}");
                Console.WriteLine($"✅ Project ID 正確: {hasCorrectProjectId}");
                Console.WriteLine($"✅ API Key 正確: {hasCorrectApiKey}");
                
                if (!isSupabaseSelected || !hasCorrectUrl || !hasCorrectProjectId || !hasCorrectApiKey)
                {
                    Console.WriteLine("\n⚠️ 發現問題，正在修正...");
                    
                    // 強制設定為 Supabase
                    settings.BackendService = BackendServiceType.Supabase;
                    settings.ApiUrl = AppSettings.Defaults.Supabase.ApiUrl;
                    settings.ProjectId = AppSettings.Defaults.Supabase.ProjectId;
                    settings.ApiKey = AppSettings.Defaults.Supabase.ApiKey;
                    settings.DatabaseId = "";
                    settings.BucketId = "";
                    
                    settings.Save();
                    
                    Console.WriteLine("✅ 設定已修正並儲存");
                    
                    // 重新載入並驗證
                    AppSettings.ReloadSettings();
                    var newSettings = AppSettings.Instance;
                    
                    Console.WriteLine("\n=== 修正後的設定 ===");
                    Console.WriteLine($"後端服務: {newSettings.BackendService}");
                    Console.WriteLine($"API URL: {newSettings.ApiUrl}");
                    Console.WriteLine($"Project ID: {newSettings.ProjectId}");
                }
                else
                {
                    Console.WriteLine("\n🎉 所有設定都正確！");
                }
                
                Console.WriteLine("\n=== 使用說明 ===");
                Console.WriteLine("1. 重新啟動應用程式");
                Console.WriteLine("2. 進入系統設定頁面");
                Console.WriteLine("3. 點擊 Supabase 選項");
                Console.WriteLine("4. 確認欄位自動更新為正確值");
                Console.WriteLine("5. 點擊「測試設定」查看當前界面設定");
                Console.WriteLine("6. 點擊「儲存設定」保存");
                
            }
            catch (Exception ex)
            {
                Console.WriteLine($"調試過程中發生錯誤: {ex.Message}");
                Console.WriteLine($"詳細錯誤: {ex}");
            }
            
            Console.WriteLine("\n=== 調試完成 ===");
        }
    }
}