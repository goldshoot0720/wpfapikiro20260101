using System;
using System.Threading.Tasks;
using System.Windows;
using wpfkiro20260101.Services;
using wpfkiro20260101.Models;

namespace wpfkiro20260101
{
    /// <summary>
    /// Supabase 綜合功能測試
    /// </summary>
    public static class TestSupabaseComprehensive
    {
        public static async Task RunComprehensiveTest()
        {
            Console.WriteLine("=== Supabase 綜合功能測試 ===");
            
            try
            {
                // 檢查當前設定
                var settings = AppSettings.Instance;
                Console.WriteLine($"當前後端服務: {settings.BackendService}");
                Console.WriteLine($"API URL: {settings.ApiUrl}");
                Console.WriteLine($"Project ID: {settings.ProjectId}");
                Console.WriteLine($"API Key: {(string.IsNullOrEmpty(settings.ApiKey) ? "未設定" : "已設定")}");
                
                // 如果不是 Supabase，暫時切換
                var originalService = settings.BackendService;
                if (settings.BackendService != BackendServiceType.Supabase)
                {
                    Console.WriteLine("⚠️ 當前不是 Supabase 服務，暫時切換進行測試");
                    settings.BackendService = BackendServiceType.Supabase;
                }
                
                // 創建 Supabase 服務實例
                var supabaseService = new SupabaseService();
                
                // 1. 測試連線
                Console.WriteLine("\n--- 測試 Supabase 連線 ---");
                var connectionResult = await supabaseService.TestConnectionAsync();
                if (connectionResult)
                {
                    Console.WriteLine("✅ Supabase 連線成功");
                }
                else
                {
                    Console.WriteLine("❌ Supabase 連線失敗");
                    Console.WriteLine("請檢查以下設定：");
                    Console.WriteLine($"- API URL: {settings.ApiUrl}");
                    Console.WriteLine($"- API Key: {(string.IsNullOrEmpty(settings.ApiKey) ? "未設定" : "已設定")}");
                    return;
                }
                
                // 2. 測試食品功能
                Console.WriteLine("\n--- 測試 Supabase 食品功能 ---");
                await TestFoodOperations(supabaseService);
                
                // 3. 測試訂閱功能
                Console.WriteLine("\n--- 測試 Supabase 訂閱功能 ---");
                await TestSubscriptionOperations(supabaseService);
                
                // 4. 測試 CRUD Manager 整合
                Console.WriteLine("\n--- 測試 CRUD Manager 整合 ---");
                await TestCrudManagerIntegration();
                
                // 恢復原始設定
                if (originalService != BackendServiceType.Supabase)
                {
                    settings.BackendService = originalService;
                    Console.WriteLine($"✅ 恢復原始後端服務: {originalService}");
                }
                
                Console.WriteLine("\n=== Supabase 綜合功能測試完成 ===");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ 測試過程中發生錯誤: {ex.Message}");
                Console.WriteLine($"詳細錯誤: {ex}");
            }
        }
        
        private static async Task TestFoodOperations(SupabaseService service)
        {
            try
            {
                // 測試獲取食品列表
                Console.WriteLine("📋 測試獲取食品列表...");
                var getFoodsResult = await service.GetFoodsAsync();
                
                if (getFoodsResult.Success)
                {
                    Console.WriteLine($"✅ 成功獲取 {getFoodsResult.Data?.Length ?? 0} 項食品資料");
                    
                    if (getFoodsResult.Data != null && getFoodsResult.Data.Length > 0)
                    {
                        Console.WriteLine("📝 食品資料範例:");
                        var firstFood = getFoodsResult.Data[0];
                        Console.WriteLine($"   - 食品: {firstFood}");
                    }
                    else
                    {
                        Console.WriteLine("ℹ️ 目前沒有食品資料");
                    }
                }
                else
                {
                    Console.WriteLine($"❌ 獲取食品列表失敗: {getFoodsResult.ErrorMessage}");
                }
                
                // 測試創建食品（如果需要）
                Console.WriteLine("\n🆕 測試創建食品...");
                var testFood = new Food
                {
                    FoodName = "測試食品 - Supabase",
                    Price = 99,
                    Shop = "測試商店",
                    Photo = "https://picsum.photos/200/200?random=1",
                    ToDate = DateTime.Now.AddDays(7).ToString("yyyy-MM-dd")
                };
                
                var createResult = await service.CreateFoodAsync(testFood);
                if (createResult.Success)
                {
                    Console.WriteLine("✅ 食品創建成功");
                }
                else
                {
                    Console.WriteLine($"❌ 食品創建失敗: {createResult.ErrorMessage}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ 食品功能測試異常: {ex.Message}");
            }
        }
        
        private static async Task TestSubscriptionOperations(SupabaseService service)
        {
            try
            {
                // 測試獲取訂閱列表
                Console.WriteLine("📋 測試獲取訂閱列表...");
                var getSubscriptionsResult = await service.GetSubscriptionsAsync();
                
                if (getSubscriptionsResult.Success)
                {
                    Console.WriteLine($"✅ 成功獲取 {getSubscriptionsResult.Data?.Length ?? 0} 項訂閱資料");
                    
                    if (getSubscriptionsResult.Data != null && getSubscriptionsResult.Data.Length > 0)
                    {
                        Console.WriteLine("📝 訂閱資料範例:");
                        var firstSubscription = getSubscriptionsResult.Data[0];
                        Console.WriteLine($"   - 訂閱: {firstSubscription}");
                    }
                    else
                    {
                        Console.WriteLine("ℹ️ 目前沒有訂閱資料");
                    }
                }
                else
                {
                    Console.WriteLine($"❌ 獲取訂閱列表失敗: {getSubscriptionsResult.ErrorMessage}");
                }
                
                // 測試創建訂閱（如果需要）
                Console.WriteLine("\n🆕 測試創建訂閱...");
                var testSubscription = new Subscription
                {
                    SubscriptionName = "測試訂閱 - Supabase",
                    NextDate = DateTime.Now.AddDays(30),
                    Price = 299,
                    Site = "https://test.example.com",
                    Account = "test@example.com",
                    Note = "Supabase 測試訂閱"
                };
                
                var createResult = await service.CreateSubscriptionAsync(testSubscription);
                if (createResult.Success)
                {
                    Console.WriteLine("✅ 訂閱創建成功");
                }
                else
                {
                    Console.WriteLine($"❌ 訂閱創建失敗: {createResult.ErrorMessage}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ 訂閱功能測試異常: {ex.Message}");
            }
        }
        
        private static async Task TestCrudManagerIntegration()
        {
            try
            {
                var crudManager = BackendServiceFactory.CreateCrudManager();
                
                // 測試透過 CRUD Manager 獲取資料
                Console.WriteLine("🔄 測試透過 CRUD Manager 獲取食品資料...");
                var foodsResult = await crudManager.GetAllFoodsAsync();
                
                if (foodsResult.Success)
                {
                    Console.WriteLine($"✅ CRUD Manager 成功獲取 {foodsResult.Data?.Length ?? 0} 項食品資料");
                }
                else
                {
                    Console.WriteLine($"❌ CRUD Manager 獲取食品失敗: {foodsResult.ErrorMessage}");
                }
                
                Console.WriteLine("🔄 測試透過 CRUD Manager 獲取訂閱資料...");
                var subscriptionsResult = await crudManager.GetAllSubscriptionsAsync();
                
                if (subscriptionsResult.Success)
                {
                    Console.WriteLine($"✅ CRUD Manager 成功獲取 {subscriptionsResult.Data?.Length ?? 0} 項訂閱資料");
                }
                else
                {
                    Console.WriteLine($"❌ CRUD Manager 獲取訂閱失敗: {subscriptionsResult.ErrorMessage}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ CRUD Manager 整合測試異常: {ex.Message}");
            }
        }
        
        public static void ShowSupabaseStatus()
        {
            var settings = AppSettings.Instance;
            
            var message = $@"
🔍 Supabase 狀態檢查

⚙️ 當前設定:
• 後端服務: {settings.BackendService}
• API URL: {settings.ApiUrl}
• Project ID: {settings.ProjectId}
• API Key: {(string.IsNullOrEmpty(settings.ApiKey) ? "❌ 未設定" : "✅ 已設定")}

📊 預期的 Supabase 資料表結構:

🍎 Food 資料表 (food):
• id (主鍵)
• name (食品名稱)
• price (價格)
• photo (圖片 URL)
• shop (商店)
• todate (到期日期)
• account (帳戶)
• created_at (創建時間)
• updated_at (更新時間)

📋 Subscription 資料表 (subscription):
• id (主鍵)
• name (訂閱名稱)
• nextdate (下次付款日期)
• price (價格)
• site (網站)
• account (帳戶)
• note (備註)
• created_at (創建時間)
• updated_at (更新時間)

💡 使用建議:
1. 確保 Supabase 專案中已創建對應的資料表
2. 檢查 API Key 是否有正確的權限
3. 確認 Row Level Security (RLS) 設定
4. 測試 API 端點是否可正常訪問

🔧 故障排除:
• 如果連線失敗，請檢查 API URL 和 API Key
• 如果資料表不存在，請在 Supabase 控制台創建
• 如果權限錯誤，請檢查 RLS 政策設定
";
            
            MessageBox.Show(message, "Supabase 狀態檢查", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        
        public static async Task TestSupabaseTableStructure()
        {
            Console.WriteLine("=== 測試 Supabase 資料表結構 ===");
            
            try
            {
                var settings = AppSettings.Instance;
                var service = new SupabaseService();
                
                // 測試不同的資料表名稱
                var tableTests = new[]
                {
                    new { Name = "food", Description = "食品資料表" },
                    new { Name = "subscription", Description = "訂閱資料表" },
                    new { Name = "foods", Description = "食品資料表 (複數)" },
                    new { Name = "subscriptions", Description = "訂閱資料表 (複數)" }
                };
                
                foreach (var table in tableTests)
                {
                    Console.WriteLine($"\n🔍 測試資料表: {table.Name} ({table.Description})");
                    
                    try
                    {
                        using var httpClient = new System.Net.Http.HttpClient();
                        httpClient.DefaultRequestHeaders.Add("apikey", settings.ApiKey);
                        httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {settings.ApiKey}");
                        
                        var response = await httpClient.GetAsync($"{settings.ApiUrl}/rest/v1/{table.Name}");
                        
                        if (response.IsSuccessStatusCode)
                        {
                            var content = await response.Content.ReadAsStringAsync();
                            Console.WriteLine($"✅ 資料表 {table.Name} 存在且可訪問");
                            Console.WriteLine($"   回應長度: {content.Length} 字元");
                            
                            if (content.Length > 2) // 不只是 "[]"
                            {
                                Console.WriteLine($"   包含資料: 是");
                            }
                            else
                            {
                                Console.WriteLine($"   包含資料: 否 (空資料表)");
                            }
                        }
                        else
                        {
                            Console.WriteLine($"❌ 資料表 {table.Name} 不可訪問: {response.StatusCode}");
                            var errorContent = await response.Content.ReadAsStringAsync();
                            Console.WriteLine($"   錯誤詳情: {errorContent}");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"❌ 測試資料表 {table.Name} 時發生異常: {ex.Message}");
                    }
                }
                
                Console.WriteLine("\n=== Supabase 資料表結構測試完成 ===");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ 資料表結構測試失敗: {ex.Message}");
            }
        }
    }
}