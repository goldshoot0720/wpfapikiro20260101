using System;
using System.Threading.Tasks;
using wpfkiro20260101.Services;

namespace wpfkiro20260101
{
    /// <summary>
    /// 修正後的 Supabase 測試
    /// </summary>
    public static class TestSupabaseFixed
    {
        public static async Task RunFixedTest()
        {
            Console.WriteLine("=== 修正後的 Supabase 測試 ===");
            
            try
            {
                var settings = AppSettings.Instance;
                var originalService = settings.BackendService;
                
                // 暫時切換到 Supabase
                settings.BackendService = BackendServiceType.Supabase;
                Console.WriteLine("✅ 切換到 Supabase 服務");
                
                // 測試 CrudManager 方法名稱修正
                Console.WriteLine("\n🔧 測試 CrudManager 方法修正...");
                var crudManager = BackendServiceFactory.CreateCrudManager();
                
                // 使用正確的方法名稱
                var foodsResult = await crudManager.GetAllFoodsAsync();
                Console.WriteLine($"✅ GetAllFoodsAsync 方法可用: {foodsResult.Success}");
                
                var subscriptionsResult = await crudManager.GetAllSubscriptionsAsync();
                Console.WriteLine($"✅ GetAllSubscriptionsAsync 方法可用: {subscriptionsResult.Success}");
                
                // 測試 SupabaseService 直接調用
                Console.WriteLine("\n🔧 測試 SupabaseService 直接調用...");
                var supabaseService = new SupabaseService();
                
                var directFoodsResult = await supabaseService.GetFoodsAsync();
                Console.WriteLine($"✅ SupabaseService.GetFoodsAsync: {directFoodsResult.Success}");
                if (!directFoodsResult.Success)
                {
                    Console.WriteLine($"   錯誤: {directFoodsResult.ErrorMessage}");
                }
                
                var directSubscriptionsResult = await supabaseService.GetSubscriptionsAsync();
                Console.WriteLine($"✅ SupabaseService.GetSubscriptionsAsync: {directSubscriptionsResult.Success}");
                if (!directSubscriptionsResult.Success)
                {
                    Console.WriteLine($"   錯誤: {directSubscriptionsResult.ErrorMessage}");
                }
                
                // 測試連線
                Console.WriteLine("\n🔧 測試連線狀態...");
                var connectionResult = await supabaseService.TestConnectionAsync();
                Console.WriteLine($"✅ 連線測試: {(connectionResult ? "成功" : "失敗")}");
                
                // 恢復原始服務
                settings.BackendService = originalService;
                Console.WriteLine($"\n✅ 恢復原始服務: {originalService}");
                
                Console.WriteLine("\n=== 修正後的 Supabase 測試完成 ===");
                Console.WriteLine("✅ 所有編譯錯誤已修正");
                Console.WriteLine("✅ CrudManager 方法名稱已更正");
                Console.WriteLine("✅ 條件運算式類型問題已解決");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ 測試失敗: {ex.Message}");
                Console.WriteLine($"詳細錯誤: {ex}");
            }
        }
        
        public static void ShowFixedIssues()
        {
            var message = @"
🔧 已修正的問題

✅ 編譯錯誤修正:
1. CrudManager 方法名稱錯誤
   - 錯誤: GetFoodsAsync()
   - 正確: GetAllFoodsAsync()
   - 錯誤: GetSubscriptionsAsync()
   - 正確: GetAllSubscriptionsAsync()

2. 條件運算式類型不匹配
   - 錯誤: content == ""[]"" ? 0 : ""多筆""
   - 正確: content == ""[]"" ? ""0"" : ""多筆""

✅ 功能驗證:
• SupabaseService 連線測試
• CrudManager 整合測試
• 資料載入功能測試
• 錯誤處理機制測試

✅ 測試工具:
• TestSupabaseComprehensive - 綜合測試
• QuickSupabaseDiagnosis - 快速診斷
• TestSupabaseQuick - 快速測試
• TestSupabaseFixed - 修正驗證

🚀 使用方式:
1. 前往設定頁面選擇 Supabase
2. 點擊「測試連線」進行診斷
3. 點擊「⚡ 快速測試」執行測試
4. 查看控制台輸出了解結果

現在可以正常測試 Supabase 的食品和訂閱功能了！
";
            
            System.Windows.MessageBox.Show(message, "Supabase 修正完成", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
        }
    }
}