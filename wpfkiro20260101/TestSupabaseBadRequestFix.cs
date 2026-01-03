using System;
using System.Threading.Tasks;
using System.Windows;
using wpfkiro20260101.Services;
using MessageBox = System.Windows.MessageBox;

namespace wpfkiro20260101
{
    public class TestSupabaseBadRequestFix
    {
        public static async Task RunTest()
        {
            try
            {
                Console.WriteLine("=== Supabase Content-Type 標頭修正測試 ===");
                
                // 確保使用 Supabase 設定
                var settings = AppSettings.Instance;
                if (settings.BackendService != BackendServiceType.Supabase)
                {
                    Console.WriteLine("⚠️ 當前後端服務不是 Supabase，正在切換...");
                    settings.BackendService = BackendServiceType.Supabase;
                    settings.Save();
                }
                
                Console.WriteLine($"✅ 後端服務: {settings.GetServiceDisplayName()}");
                Console.WriteLine($"✅ API URL: {settings.ApiUrl}");
                Console.WriteLine($"✅ Project ID: {settings.ProjectId}");
                Console.WriteLine($"✅ API Key: {settings.ApiKey.Substring(0, 20)}...");
                
                // 創建 Supabase 服務實例
                var supabaseService = new SupabaseService();
                
                // 測試連接
                Console.WriteLine("\n--- 測試 Supabase 連接 ---");
                var connectionResult = await supabaseService.TestConnectionAsync();
                Console.WriteLine($"連接測試結果: {(connectionResult ? "✅ 成功" : "❌ 失敗")}");
                
                if (!connectionResult)
                {
                    Console.WriteLine("❌ 連接測試失敗，請檢查設定");
                    MessageBox.Show(
                        "連接測試失敗！\n\n請檢查：\n" +
                        "1. 網路連接是否正常\n" +
                        "2. Supabase API URL 是否正確\n" +
                        "3. API Key 是否有效\n\n" +
                        "查看 Visual Studio 輸出視窗獲取詳細錯誤訊息。",
                        "連接失敗",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning
                    );
                    return;
                }
                
                // 測試載入訂閱資料
                Console.WriteLine("\n--- 測試載入訂閱資料 ---");
                var subscriptionResult = await supabaseService.GetSubscriptionsAsync();
                
                if (subscriptionResult.Success)
                {
                    Console.WriteLine($"✅ 訂閱資料載入成功！找到 {subscriptionResult.Data.Length} 筆記錄");
                    
                    foreach (var item in subscriptionResult.Data)
                    {
                        Console.WriteLine($"   - {item}");
                    }
                }
                else
                {
                    Console.WriteLine($"❌ 訂閱資料載入失敗: {subscriptionResult.ErrorMessage}");
                }
                
                // 測試載入食品資料
                Console.WriteLine("\n--- 測試載入食品資料 ---");
                var foodResult = await supabaseService.GetFoodsAsync();
                
                if (foodResult.Success)
                {
                    Console.WriteLine($"✅ 食品資料載入成功！找到 {foodResult.Data.Length} 筆記錄");
                    
                    foreach (var item in foodResult.Data)
                    {
                        Console.WriteLine($"   - {item}");
                    }
                }
                else
                {
                    Console.WriteLine($"❌ 食品資料載入失敗: {foodResult.ErrorMessage}");
                }
                
                // 總結
                Console.WriteLine("\n=== 測試總結 ===");
                if (connectionResult && subscriptionResult.Success && foodResult.Success)
                {
                    Console.WriteLine("🎉 所有測試通過！Supabase Content-Type 標頭問題已修正");
                    MessageBox.Show(
                        "Supabase HTTP 標頭問題已修正！\n\n" +
                        "✅ 連接測試成功\n" +
                        $"✅ 訂閱資料: {subscriptionResult.Data.Length} 筆\n" +
                        $"✅ 食品資料: {foodResult.Data.Length} 筆\n\n" +
                        "修正內容：\n" +
                        "• 移除了 GET 請求中不當的 Content-Type 標頭\n" +
                        "• 正確配置了 POST 請求的 Content-Type\n" +
                        "• 保留了必要的 apikey 和 Authorization 標頭\n\n" +
                        "現在可以正常使用所有功能了。",
                        "修正成功",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information
                    );
                }
                else
                {
                    Console.WriteLine("⚠️ 部分測試失敗，請檢查詳細錯誤訊息");
                    
                    var failedTests = new System.Collections.Generic.List<string>();
                    if (!connectionResult) failedTests.Add("連接測試");
                    if (!subscriptionResult.Success) failedTests.Add("訂閱資料載入");
                    if (!foodResult.Success) failedTests.Add("食品資料載入");
                    
                    MessageBox.Show(
                        "測試結果：\n\n" +
                        $"連接測試: {(connectionResult ? "✅" : "❌")}\n" +
                        $"訂閱資料: {(subscriptionResult.Success ? "✅" : "❌")}\n" +
                        $"食品資料: {(foodResult.Success ? "✅" : "❌")}\n\n" +
                        $"失敗的測試: {string.Join(", ", failedTests)}\n\n" +
                        "請查看 Visual Studio 輸出視窗獲取詳細錯誤訊息。",
                        "測試結果",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning
                    );
                }
                
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ 測試過程中發生異常: {ex.Message}");
                MessageBox.Show(
                    $"測試過程中發生錯誤：\n\n{ex.Message}\n\n" +
                    "這可能是因為：\n" +
                    "1. 網路連接問題\n" +
                    "2. Supabase 服務暫時不可用\n" +
                    "3. API 配置錯誤\n\n" +
                    "請檢查網路連接和 API 設定。",
                    "測試錯誤",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }
        }
    }
}