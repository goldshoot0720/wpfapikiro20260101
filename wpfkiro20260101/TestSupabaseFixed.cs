using System;
using System.Threading.Tasks;
using wpfkiro20260101.Services;

namespace wpfkiro20260101
{
    public class TestSupabaseFixed
    {
        public static async Task RunTest()
        {
            Console.WriteLine("=== Supabase 修正後連接測試 ===");
            
            try
            {
                // 確保使用正確的設定
                var settings = AppSettings.Instance;
                Console.WriteLine($"當前後端服務: {settings.BackendService}");
                Console.WriteLine($"API URL: {settings.ApiUrl}");
                Console.WriteLine($"Project ID: {settings.ProjectId}");
                Console.WriteLine($"API Key: {settings.ApiKey.Substring(0, 20)}...");
                
                // 如果不是 Supabase，切換到 Supabase
                if (settings.BackendService != BackendServiceType.Supabase)
                {
                    Console.WriteLine("切換到 Supabase 設定...");
                    settings.BackendService = BackendServiceType.Supabase;
                    settings.ApiUrl = AppSettings.Defaults.Supabase.ApiUrl;
                    settings.ProjectId = AppSettings.Defaults.Supabase.ProjectId;
                    settings.ApiKey = AppSettings.Defaults.Supabase.ApiKey;
                    settings.Save();
                    Console.WriteLine("設定已更新並儲存");
                }
                
                // 測試 Supabase 服務
                var supabaseService = new SupabaseService();
                
                Console.WriteLine("\n1. 測試基本連接...");
                var connectionTest = await supabaseService.TestConnectionAsync();
                Console.WriteLine($"連接測試結果: {(connectionTest ? "成功" : "失敗")}");
                
                Console.WriteLine("\n2. 測試初始化...");
                var initResult = await supabaseService.InitializeAsync();
                Console.WriteLine($"初始化結果: {(initResult ? "成功" : "失敗")}");
                
                Console.WriteLine("\n3. 測試載入食品資料...");
                var foodsResult = await supabaseService.GetFoodsAsync();
                if (foodsResult.Success)
                {
                    Console.WriteLine($"食品資料載入成功，共 {foodsResult.Data?.Length ?? 0} 筆記錄");
                    if (foodsResult.Data != null && foodsResult.Data.Length > 0)
                    {
                        Console.WriteLine("第一筆食品資料:");
                        var firstFood = foodsResult.Data[0];
                        Console.WriteLine($"  資料: {firstFood}");
                    }
                }
                else
                {
                    Console.WriteLine($"食品資料載入失敗: {foodsResult.ErrorMessage}");
                }
                
                Console.WriteLine("\n4. 測試載入訂閱資料...");
                var subscriptionsResult = await supabaseService.GetSubscriptionsAsync();
                if (subscriptionsResult.Success)
                {
                    Console.WriteLine($"訂閱資料載入成功，共 {subscriptionsResult.Data?.Length ?? 0} 筆記錄");
                    if (subscriptionsResult.Data != null && subscriptionsResult.Data.Length > 0)
                    {
                        Console.WriteLine("第一筆訂閱資料:");
                        var firstSub = subscriptionsResult.Data[0];
                        Console.WriteLine($"  資料: {firstSub}");
                    }
                }
                else
                {
                    Console.WriteLine($"訂閱資料載入失敗: {subscriptionsResult.ErrorMessage}");
                }
                
                Console.WriteLine("\n=== 測試完成 ===");
                
                // 清理資源
                supabaseService.Dispose();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"測試過程中發生異常: {ex.Message}");
                Console.WriteLine($"詳細錯誤: {ex}");
            }
        }
    }
}