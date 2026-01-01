using System;
using System.Threading.Tasks;
using wpfkiro20260101.Services;
using wpfkiro20260101.Models;

namespace wpfkiro20260101
{
    /// <summary>
    /// Supabase 連接測試類別
    /// 用於驗證 Supabase 服務的連接和基本 CRUD 操作
    /// </summary>
    public class TestSupabaseConnection
    {
        private readonly SupabaseService _supabaseService;

        public TestSupabaseConnection()
        {
            _supabaseService = new SupabaseService();
        }

        /// <summary>
        /// 執行完整的 Supabase 連接測試
        /// </summary>
        public async Task RunAllTestsAsync()
        {
            Console.WriteLine("=================================");
            Console.WriteLine("開始 Supabase 連接測試");
            Console.WriteLine("=================================");

            try
            {
                // 1. 測試基本連接
                await TestBasicConnection();

                // 2. 測試食品 CRUD 操作
                await TestFoodOperations();

                // 3. 測試訂閱 CRUD 操作
                await TestSubscriptionOperations();

                Console.WriteLine("\n=================================");
                Console.WriteLine("✅ 所有測試完成！");
                Console.WriteLine("=================================");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n❌ 測試過程中發生錯誤：{ex.Message}");
                Console.WriteLine("=================================");
            }
        }

        /// <summary>
        /// 測試基本連接
        /// </summary>
        private async Task TestBasicConnection()
        {
            Console.WriteLine("\n📡 測試 Supabase 基本連接...");

            try
            {
                var isConnected = await _supabaseService.TestConnectionAsync();
                
                if (isConnected)
                {
                    Console.WriteLine("✅ Supabase 連接成功！");
                }
                else
                {
                    Console.WriteLine("❌ Supabase 連接失敗！");
                    Console.WriteLine("請檢查：");
                    Console.WriteLine("1. API URL 是否正確");
                    Console.WriteLine("2. API Key 是否有效");
                    Console.WriteLine("3. 網路連接是否正常");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ 連接測試失敗：{ex.Message}");
            }
        }

        /// <summary>
        /// 測試食品 CRUD 操作
        /// </summary>
        private async Task TestFoodOperations()
        {
            Console.WriteLine("\n🍎 測試食品 CRUD 操作...");

            try
            {
                // 創建測試食品
                var testFood = new Food
                {
                    Id = Guid.NewGuid().ToString(),
                    FoodName = "測試蘋果",
                    Price = 50,
                    Quantity = 2,
                    Photo = "https://example.com/apple.jpg",
                    PhotoHash = "test_hash_123",
                    Shop = "測試商店",
                    ToDate = "2026-02-01",
                    Description = "這是一個測試用的蘋果",
                    Category = "水果",
                    StorageLocation = "冰箱",
                    Note = "測試備註",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                // 1. 測試創建食品
                Console.WriteLine("  📝 測試創建食品...");
                var createResult = await _supabaseService.CreateFoodAsync(testFood);
                
                if (createResult.Success)
                {
                    Console.WriteLine("  ✅ 食品創建成功！");
                }
                else
                {
                    Console.WriteLine($"  ❌ 食品創建失敗：{createResult.ErrorMessage}");
                    return;
                }

                // 2. 測試讀取食品
                Console.WriteLine("  📖 測試讀取食品列表...");
                var readResult = await _supabaseService.GetFoodsAsync();
                
                if (readResult.Success)
                {
                    Console.WriteLine($"  ✅ 成功讀取 {readResult.Data.Length} 項食品資料");
                    
                    // 顯示前 3 項資料
                    var displayCount = Math.Min(3, readResult.Data.Length);
                    for (int i = 0; i < displayCount; i++)
                    {
                        Console.WriteLine($"    - 食品 {i + 1}: {readResult.Data[i]}");
                    }
                }
                else
                {
                    Console.WriteLine($"  ❌ 讀取食品失敗：{readResult.ErrorMessage}");
                }

                // 3. 測試更新食品
                Console.WriteLine("  ✏️ 測試更新食品...");
                testFood.FoodName = "更新後的測試蘋果";
                testFood.Price = 60;
                testFood.UpdatedAt = DateTime.UtcNow;

                var updateResult = await _supabaseService.UpdateFoodAsync(testFood.Id, testFood);
                
                if (updateResult.Success)
                {
                    Console.WriteLine("  ✅ 食品更新成功！");
                }
                else
                {
                    Console.WriteLine($"  ❌ 食品更新失敗：{updateResult.ErrorMessage}");
                }

                // 4. 測試刪除食品
                Console.WriteLine("  🗑️ 測試刪除食品...");
                var deleteResult = await _supabaseService.DeleteFoodAsync(testFood.Id);
                
                if (deleteResult.Success)
                {
                    Console.WriteLine("  ✅ 食品刪除成功！");
                }
                else
                {
                    Console.WriteLine($"  ❌ 食品刪除失敗：{deleteResult.ErrorMessage}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  ❌ 食品操作測試失敗：{ex.Message}");
            }
        }

        /// <summary>
        /// 測試訂閱 CRUD 操作
        /// </summary>
        private async Task TestSubscriptionOperations()
        {
            Console.WriteLine("\n📋 測試訂閱 CRUD 操作...");

            try
            {
                // 創建測試訂閱
                var testSubscription = new Subscription
                {
                    Id = Guid.NewGuid().ToString(),
                    SubscriptionName = "測試 Netflix",
                    NextDate = DateTime.UtcNow.AddDays(30),
                    Price = 390,
                    Site = "https://netflix.com",
                    Account = "test@example.com",
                    Note = "測試訂閱備註",
                    StringToDate = DateTime.UtcNow.AddDays(30).ToString("yyyy-MM-dd"),
                    DateTime = DateTime.UtcNow.AddDays(30),
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                // 1. 測試創建訂閱
                Console.WriteLine("  📝 測試創建訂閱...");
                var createResult = await _supabaseService.CreateSubscriptionAsync(testSubscription);
                
                if (createResult.Success)
                {
                    Console.WriteLine("  ✅ 訂閱創建成功！");
                }
                else
                {
                    Console.WriteLine($"  ❌ 訂閱創建失敗：{createResult.ErrorMessage}");
                    return;
                }

                // 2. 測試讀取訂閱
                Console.WriteLine("  📖 測試讀取訂閱列表...");
                var readResult = await _supabaseService.GetSubscriptionsAsync();
                
                if (readResult.Success)
                {
                    Console.WriteLine($"  ✅ 成功讀取 {readResult.Data.Length} 項訂閱資料");
                    
                    // 顯示前 3 項資料
                    var displayCount = Math.Min(3, readResult.Data.Length);
                    for (int i = 0; i < displayCount; i++)
                    {
                        Console.WriteLine($"    - 訂閱 {i + 1}: {readResult.Data[i]}");
                    }
                }
                else
                {
                    Console.WriteLine($"  ❌ 讀取訂閱失敗：{readResult.ErrorMessage}");
                }

                // 3. 測試更新訂閱
                Console.WriteLine("  ✏️ 測試更新訂閱...");
                testSubscription.SubscriptionName = "更新後的測試 Netflix";
                testSubscription.Price = 450;
                testSubscription.UpdatedAt = DateTime.UtcNow;

                var updateResult = await _supabaseService.UpdateSubscriptionAsync(testSubscription.Id, testSubscription);
                
                if (updateResult.Success)
                {
                    Console.WriteLine("  ✅ 訂閱更新成功！");
                }
                else
                {
                    Console.WriteLine($"  ❌ 訂閱更新失敗：{updateResult.ErrorMessage}");
                }

                // 4. 測試刪除訂閱
                Console.WriteLine("  🗑️ 測試刪除訂閱...");
                var deleteResult = await _supabaseService.DeleteSubscriptionAsync(testSubscription.Id);
                
                if (deleteResult.Success)
                {
                    Console.WriteLine("  ✅ 訂閱刪除成功！");
                }
                else
                {
                    Console.WriteLine($"  ❌ 訂閱刪除失敗：{deleteResult.ErrorMessage}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  ❌ 訂閱操作測試失敗：{ex.Message}");
            }
        }

        /// <summary>
        /// 快速連接測試（僅測試連接）
        /// </summary>
        public async Task<bool> QuickConnectionTestAsync()
        {
            try
            {
                Console.WriteLine("🔍 執行 Supabase 快速連接測試...");
                
                var isConnected = await _supabaseService.TestConnectionAsync();
                
                if (isConnected)
                {
                    Console.WriteLine("✅ Supabase 連接正常！");
                    return true;
                }
                else
                {
                    Console.WriteLine("❌ Supabase 連接失敗！");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ 連接測試錯誤：{ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// 顯示 Supabase 配置資訊
        /// </summary>
        public void DisplayConfiguration()
        {
            var settings = AppSettings.Instance;
            
            Console.WriteLine("\n📋 當前 Supabase 配置：");
            Console.WriteLine($"API URL: {settings.ApiUrl ?? "未設定"}");
            Console.WriteLine($"API Key: {(string.IsNullOrEmpty(settings.ApiKey) ? "未設定" : "已設定 (隱藏)")}");
            Console.WriteLine($"Project ID: {settings.ProjectId ?? "未設定"}");
            Console.WriteLine($"服務類型: {settings.BackendService}");
        }

        /// <summary>
        /// 釋放資源
        /// </summary>
        public void Dispose()
        {
            _supabaseService?.Dispose();
        }
    }

    /// <summary>
    /// 測試程式進入點
    /// </summary>
    public class SupabaseTestProgram
    {
        public static async Task RunSupabaseTests()
        {
            var tester = new TestSupabaseConnection();
            
            try
            {
                // 顯示配置資訊
                tester.DisplayConfiguration();
                
                // 執行完整測試
                await tester.RunAllTestsAsync();
            }
            finally
            {
                tester.Dispose();
            }
        }

        public static async Task<bool> QuickTest()
        {
            var tester = new TestSupabaseConnection();
            
            try
            {
                return await tester.QuickConnectionTestAsync();
            }
            finally
            {
                tester.Dispose();
            }
        }
    }
}