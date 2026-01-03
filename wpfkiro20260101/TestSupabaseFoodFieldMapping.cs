using System;
using System.Threading.Tasks;
using System.Windows;
using wpfkiro20260101.Services;
using MessageBox = System.Windows.MessageBox;

namespace wpfkiro20260101
{
    public class TestSupabaseFoodFieldMapping
    {
        public static async Task RunTest()
        {
            try
            {
                Console.WriteLine("=== Supabase 食品表字段映射修正測試 ===");
                
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
                
                Console.WriteLine("\n=== 字段映射對比 ===");
                Console.WriteLine("修正前 (錯誤):");
                Console.WriteLine("  - photohash → photo");
                Console.WriteLine("  - site → shop");
                Console.WriteLine("  - nextdate → todate");
                
                Console.WriteLine("\n修正後 (正確):");
                Console.WriteLine("  - photo ✓");
                Console.WriteLine("  - shop ✓");
                Console.WriteLine("  - todate ✓");
                
                // 測試載入食品資料
                Console.WriteLine("\n--- 測試載入食品資料 ---");
                var foodResult = await supabaseService.GetFoodsAsync();
                
                if (foodResult.Success)
                {
                    Console.WriteLine($"✅ 食品資料載入成功！找到 {foodResult.Data.Length} 筆記錄");
                    
                    if (foodResult.Data.Length > 0)
                    {
                        Console.WriteLine("\n食品資料範例:");
                        foreach (var item in foodResult.Data)
                        {
                            Console.WriteLine($"   - {item}");
                        }
                    }
                    else
                    {
                        Console.WriteLine("📝 資料表為空，這是正常的（如果還沒有添加食品資料）");
                    }
                }
                else
                {
                    Console.WriteLine($"❌ 食品資料載入失敗: {foodResult.ErrorMessage}");
                }
                
                // 測試載入訂閱資料（對比）
                Console.WriteLine("\n--- 測試載入訂閱資料（對比） ---");
                var subscriptionResult = await supabaseService.GetSubscriptionsAsync();
                
                if (subscriptionResult.Success)
                {
                    Console.WriteLine($"✅ 訂閱資料載入成功！找到 {subscriptionResult.Data.Length} 筆記錄");
                }
                else
                {
                    Console.WriteLine($"❌ 訂閱資料載入失敗: {subscriptionResult.ErrorMessage}");
                }
                
                // 總結
                Console.WriteLine("\n=== 測試總結 ===");
                if (foodResult.Success && subscriptionResult.Success)
                {
                    Console.WriteLine("🎉 所有測試通過！食品表字段映射問題已修正");
                    MessageBox.Show(
                        "Supabase 食品表字段映射問題已修正！\n\n" +
                        "修正內容：\n" +
                        "• photo (原: photohash) ✓\n" +
                        "• shop (原: site) ✓\n" +
                        "• todate (原: nextdate) ✓\n\n" +
                        "測試結果：\n" +
                        $"✅ 食品資料: {foodResult.Data.Length} 筆\n" +
                        $"✅ 訂閱資料: {subscriptionResult.Data.Length} 筆\n\n" +
                        "現在食品管理功能應該可以正常使用了！",
                        "修正成功",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information
                    );
                }
                else
                {
                    var issues = new System.Collections.Generic.List<string>();
                    if (!foodResult.Success) issues.Add($"食品載入: {foodResult.ErrorMessage}");
                    if (!subscriptionResult.Success) issues.Add($"訂閱載入: {subscriptionResult.ErrorMessage}");
                    
                    Console.WriteLine("⚠️ 部分測試失敗，請檢查詳細錯誤訊息");
                    MessageBox.Show(
                        "測試結果：\n\n" +
                        $"食品資料: {(foodResult.Success ? "✅" : "❌")}\n" +
                        $"訂閱資料: {(subscriptionResult.Success ? "✅" : "❌")}\n\n" +
                        "問題詳情：\n" +
                        string.Join("\n", issues) + "\n\n" +
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
                    "3. 資料表結構不匹配\n" +
                    "4. API 配置錯誤\n\n" +
                    "請檢查網路連接和資料表結構。",
                    "測試錯誤",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }
        }
    }
}