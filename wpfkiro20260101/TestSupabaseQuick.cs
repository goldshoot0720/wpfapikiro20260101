using System;
using System.Threading.Tasks;
using System.Windows;
using wpfkiro20260101.Services;
using MessageBox = System.Windows.MessageBox;

namespace wpfkiro20260101
{
    /// <summary>
    /// Supabase 快速測試
    /// </summary>
    public static class TestSupabaseQuick
    {
        public static async Task RunQuickTest()
        {
            Console.WriteLine("=== Supabase 快速測試 ===");
            
            try
            {
                var settings = AppSettings.Instance;
                var originalService = settings.BackendService;
                
                // 切換到 Supabase
                settings.BackendService = BackendServiceType.Supabase;
                Console.WriteLine("✅ 切換到 Supabase 服務");
                
                // 測試服務創建
                var service = BackendServiceFactory.CreateCurrentService();
                Console.WriteLine($"✅ 創建服務: {service.GetType().Name}");
                
                if (service is SupabaseService supabaseService)
                {
                    // 測試連線
                    Console.WriteLine("🔗 測試連線...");
                    var connectionResult = await supabaseService.TestConnectionAsync();
                    Console.WriteLine($"連線結果: {(connectionResult ? "✅ 成功" : "❌ 失敗")}");
                    
                    if (connectionResult)
                    {
                        // 測試食品資料
                        Console.WriteLine("🍎 測試食品資料...");
                        var foodsResult = await supabaseService.GetFoodsAsync();
                        if (foodsResult.Success)
                        {
                            Console.WriteLine($"✅ 成功載入 {foodsResult.Data?.Length ?? 0} 項食品");
                        }
                        else
                        {
                            Console.WriteLine($"❌ 食品載入失敗: {foodsResult.ErrorMessage}");
                        }
                        
                        // 測試訂閱資料
                        Console.WriteLine("📋 測試訂閱資料...");
                        var subscriptionsResult = await supabaseService.GetSubscriptionsAsync();
                        if (subscriptionsResult.Success)
                        {
                            Console.WriteLine($"✅ 成功載入 {subscriptionsResult.Data?.Length ?? 0} 項訂閱");
                        }
                        else
                        {
                            Console.WriteLine($"❌ 訂閱載入失敗: {subscriptionsResult.ErrorMessage}");
                        }
                    }
                }
                else
                {
                    Console.WriteLine($"❌ 服務類型錯誤: {service.GetType().Name}");
                }
                
                // 恢復原始服務
                settings.BackendService = originalService;
                Console.WriteLine($"✅ 恢復原始服務: {originalService}");
                
                Console.WriteLine("=== Supabase 快速測試完成 ===");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ 測試失敗: {ex.Message}");
            }
        }
        
        public static void ShowSupabaseQuickGuide()
        {
            var message = @"
🚀 Supabase 快速測試指南

📋 測試項目:
✅ 服務創建和初始化
✅ 基本連線測試
✅ 食品資料載入測試
✅ 訂閱資料載入測試

🔧 使用方式:
1. 確保已選擇 Supabase 作為後端服務
2. 點擊「測試連線」按鈕進行自動診斷
3. 或點擊「⚡ 快速測試」執行綜合測試
4. 查看控制台輸出了解詳細結果

📊 預期結果:
• 連線成功: 顯示綠色成功訊息
• 資料載入: 顯示載入的記錄數量
• 錯誤處理: 顯示具體的錯誤訊息和建議

🔍 故障排除:
• 連線失敗: 檢查 API URL 和 API Key
• 資料表錯誤: 確認 Supabase 中存在 'food' 和 'subscription' 資料表
• 權限錯誤: 檢查 Row Level Security 設定
• 網路問題: 確認網路連線正常

💡 提示:
測試完成後，可以前往食品管理或訂閱管理頁面
查看實際的資料載入效果。
";
            
            MessageBox.Show(message, "Supabase 快速測試指南", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}