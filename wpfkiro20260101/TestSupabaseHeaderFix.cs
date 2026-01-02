using System;
using System.Threading.Tasks;
using System.Windows;
using wpfkiro20260101.Services;

namespace wpfkiro20260101
{
    /// <summary>
    /// Supabase HTTP 標頭修正測試
    /// </summary>
    public static class TestSupabaseHeaderFix
    {
        public static async Task RunHeaderFixTest()
        {
            Console.WriteLine("=== Supabase HTTP 標頭修正測試 ===");
            
            try
            {
                var settings = AppSettings.Instance;
                var originalService = settings.BackendService;
                
                // 切換到 Supabase
                settings.BackendService = BackendServiceType.Supabase;
                Console.WriteLine("✅ 切換到 Supabase 服務");
                
                // 測試修正後的 SupabaseService
                var supabaseService = new SupabaseService();
                
                Console.WriteLine("\n🔧 測試修正後的 HTTP 標頭設定...");
                Console.WriteLine($"API URL: {settings.Supabase.ApiUrl}");
                Console.WriteLine($"API Key: {(string.IsNullOrEmpty(settings.Supabase.ApiKey) ? "未設定" : "已設定")}");
                
                // 測試連線
                Console.WriteLine("\n🌐 測試基本連線...");
                var connectionResult = await supabaseService.TestConnectionAsync();
                Console.WriteLine($"連線結果: {(connectionResult ? "✅ 成功" : "❌ 失敗")}");
                
                if (connectionResult)
                {
                    // 測試食品資料載入
                    Console.WriteLine("\n🍎 測試食品資料載入...");
                    var foodsResult = await supabaseService.GetFoodsAsync();
                    
                    if (foodsResult.Success)
                    {
                        Console.WriteLine($"✅ 食品資料載入成功: {foodsResult.Data?.Length ?? 0} 項");
                        
                        if (foodsResult.Data != null && foodsResult.Data.Length > 0)
                        {
                            Console.WriteLine("📝 食品資料範例:");
                            var firstFood = foodsResult.Data[0];
                            Console.WriteLine($"   {firstFood}");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"❌ 食品資料載入失敗: {foodsResult.ErrorMessage}");
                    }
                    
                    // 測試訂閱資料載入
                    Console.WriteLine("\n📋 測試訂閱資料載入...");
                    var subscriptionsResult = await supabaseService.GetSubscriptionsAsync();
                    
                    if (subscriptionsResult.Success)
                    {
                        Console.WriteLine($"✅ 訂閱資料載入成功: {subscriptionsResult.Data?.Length ?? 0} 項");
                        
                        if (subscriptionsResult.Data != null && subscriptionsResult.Data.Length > 0)
                        {
                            Console.WriteLine("📝 訂閱資料範例:");
                            var firstSubscription = subscriptionsResult.Data[0];
                            Console.WriteLine($"   {firstSubscription}");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"❌ 訂閱資料載入失敗: {subscriptionsResult.ErrorMessage}");
                    }
                }
                else
                {
                    Console.WriteLine("⚠️ 連線失敗，無法進行資料載入測試");
                    Console.WriteLine("請檢查：");
                    Console.WriteLine("1. Supabase 專案是否正常運行");
                    Console.WriteLine("2. API Key 是否正確");
                    Console.WriteLine("3. 網路連線是否正常");
                    Console.WriteLine("4. 資料表是否存在");
                }
                
                // 恢復原始服務
                settings.BackendService = originalService;
                Console.WriteLine($"\n✅ 恢復原始服務: {originalService}");
                
                Console.WriteLine("\n=== Supabase HTTP 標頭修正測試完成 ===");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ 測試失敗: {ex.Message}");
                Console.WriteLine($"詳細錯誤: {ex}");
            }
        }
        
        public static void ShowHeaderFixInfo()
        {
            var message = @"
🔧 Supabase HTTP 標頭修正

❌ 修正前的問題:
• 錯誤: 在 GET 請求中設定 Content-Type 標頭
• 錯誤訊息: ""Misused header name, 'Content-Type'""
• 原因: GET 請求不應該包含 Content-Type 標頭

✅ 修正後的改善:
• 移除 GET 請求中的 Content-Type 標頭
• 移除 GET 請求中的 Accept 標頭 (非必要)
• 保留必要的 apikey 和 Authorization 標頭
• POST/PATCH 請求仍正確使用 Content-Type

🔍 修正的方法:
• GetFoodsAsync() - 移除不必要的標頭
• GetSubscriptionsAsync() - 移除不必要的標頭
• CreateFoodAsync() - 保持正確的標頭設定
• UpdateFoodAsync() - 保持正確的標頭設定

📊 測試結果:
• 連線測試應該成功
• 資料載入應該正常
• 不再出現標頭錯誤訊息

💡 使用建議:
1. 確保 Supabase 專案中存在 'food' 和 'subscription' 資料表
2. 檢查 Row Level Security (RLS) 政策設定
3. 確認 API Key 具備正確的權限
4. 測試完成後可以在食品/訂閱頁面查看實際效果

現在 Supabase 服務應該可以正常載入資料了！
";
            
            MessageBox.Show(message, "Supabase HTTP 標頭修正", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}