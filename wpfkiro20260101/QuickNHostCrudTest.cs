using System;
using System.Threading.Tasks;
using wpfkiro20260101.Services;

namespace wpfkiro20260101
{
    /// <summary>
    /// NHost CRUD 快速測試工具
    /// 用於快速驗證 NHost 食品和訂閱操作是否正常運作
    /// </summary>
    public class QuickNHostCrudTest
    {
        /// <summary>
        /// 執行快速測試
        /// </summary>
        public static async Task RunAsync()
        {
            Console.WriteLine("🚀 開始 NHost CRUD 快速測試");
            Console.WriteLine("=" + new string('=', 50));
            
            var tester = new TestNHostCrudOperations();
            
            try
            {
                // 執行快速測試
                await tester.QuickTestAsync();
                
                Console.WriteLine();
                Console.WriteLine("✅ NHost CRUD 快速測試完成");
                Console.WriteLine("如需完整測試，請執行 RunAllTestsAsync()");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ 測試過程中發生錯誤: {ex.Message}");
                Console.WriteLine($"詳細錯誤: {ex}");
            }
        }

        /// <summary>
        /// 執行完整測試
        /// </summary>
        public static async Task RunFullTestAsync()
        {
            Console.WriteLine("🔧 開始 NHost CRUD 完整測試");
            Console.WriteLine("=" + new string('=', 50));
            
            var tester = new TestNHostCrudOperations();
            
            try
            {
                // 執行完整測試
                await tester.RunAllTestsAsync();
                
                Console.WriteLine();
                Console.WriteLine("✅ NHost CRUD 完整測試完成");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ 測試過程中發生錯誤: {ex.Message}");
                Console.WriteLine($"詳細錯誤: {ex}");
            }
        }

        /// <summary>
        /// 測試單一食品操作
        /// </summary>
        public static async Task TestSingleFoodOperationAsync()
        {
            Console.WriteLine("🍎 測試單一食品操作");
            Console.WriteLine("-" + new string('-', 30));
            
            var nHostService = new NHostService();
            
            try
            {
                // 初始化服務
                var initResult = await nHostService.InitializeAsync();
                Console.WriteLine($"服務初始化: {(initResult ? "✓" : "✗")}");
                
                if (!initResult)
                {
                    Console.WriteLine("❌ 服務初始化失敗，無法繼續測試");
                    return;
                }

                // 創建測試食品
                var testFood = new
                {
                    name = $"快速測試食品_{DateTime.Now:HHmmss}",
                    price = 99.99,
                    photo = "quick_test.jpg",
                    shop = "測試商店",
                    todate = DateTime.Now.AddDays(5).ToString("yyyy-MM-ddTHH:mm:ssZ"),
                    photohash = $"hash_{Guid.NewGuid():N}"[..16]
                };

                Console.WriteLine($"創建測試食品: {testFood.name}");
                var createResult = await nHostService.CreateFoodAsync(testFood);
                
                if (createResult.Success)
                {
                    Console.WriteLine("✅ 食品創建成功");
                    
                    // 嘗試獲取所有食品來驗證
                    var getFoodsResult = await nHostService.GetFoodsAsync();
                    if (getFoodsResult.Success)
                    {
                        Console.WriteLine($"✅ 成功獲取 {getFoodsResult.Data?.Length ?? 0} 筆食品資料");
                    }
                    else
                    {
                        Console.WriteLine($"⚠️ 獲取食品資料失敗: {getFoodsResult.ErrorMessage}");
                    }
                }
                else
                {
                    Console.WriteLine($"❌ 食品創建失敗: {createResult.ErrorMessage}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ 測試錯誤: {ex.Message}");
            }
            finally
            {
                nHostService.Dispose();
            }
        }

        /// <summary>
        /// 測試單一訂閱操作
        /// </summary>
        public static async Task TestSingleSubscriptionOperationAsync()
        {
            Console.WriteLine("📱 測試單一訂閱操作");
            Console.WriteLine("-" + new string('-', 30));
            
            var nHostService = new NHostService();
            
            try
            {
                // 初始化服務
                var initResult = await nHostService.InitializeAsync();
                Console.WriteLine($"服務初始化: {(initResult ? "✓" : "✗")}");
                
                if (!initResult)
                {
                    Console.WriteLine("❌ 服務初始化失敗，無法繼續測試");
                    return;
                }

                // 創建測試訂閱
                var testSubscription = new
                {
                    name = $"快速測試訂閱_{DateTime.Now:HHmmss}",
                    nextdate = DateTime.Now.AddDays(30).ToString("yyyy-MM-ddTHH:mm:ssZ"),
                    price = 199.00,
                    site = "quick-test.com",
                    note = "這是快速測試訂閱",
                    account = "test@quicktest.com"
                };

                Console.WriteLine($"創建測試訂閱: {testSubscription.name}");
                var createResult = await nHostService.CreateSubscriptionAsync(testSubscription);
                
                if (createResult.Success)
                {
                    Console.WriteLine("✅ 訂閱創建成功");
                    
                    // 嘗試獲取所有訂閱來驗證
                    var getSubscriptionsResult = await nHostService.GetSubscriptionsAsync();
                    if (getSubscriptionsResult.Success)
                    {
                        Console.WriteLine($"✅ 成功獲取 {getSubscriptionsResult.Data?.Length ?? 0} 筆訂閱資料");
                    }
                    else
                    {
                        Console.WriteLine($"⚠️ 獲取訂閱資料失敗: {getSubscriptionsResult.ErrorMessage}");
                    }
                }
                else
                {
                    Console.WriteLine($"❌ 訂閱創建失敗: {createResult.ErrorMessage}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ 測試錯誤: {ex.Message}");
            }
            finally
            {
                nHostService.Dispose();
            }
        }

        /// <summary>
        /// 顯示測試選項菜單
        /// </summary>
        public static void ShowTestMenu()
        {
            Console.WriteLine("🎯 NHost CRUD 測試選項");
            Console.WriteLine("=" + new string('=', 40));
            Console.WriteLine("1. 快速測試 (連線 + 讀取)");
            Console.WriteLine("2. 完整測試 (所有 CRUD 操作)");
            Console.WriteLine("3. 單一食品操作測試");
            Console.WriteLine("4. 單一訂閱操作測試");
            Console.WriteLine("5. 認證功能測試");
            Console.WriteLine("=" + new string('=', 40));
            Console.WriteLine();
            Console.WriteLine("使用方法:");
            Console.WriteLine("await QuickNHostCrudTest.RunAsync();                    // 選項 1");
            Console.WriteLine("await QuickNHostCrudTest.RunFullTestAsync();           // 選項 2");
            Console.WriteLine("await QuickNHostCrudTest.TestSingleFoodOperationAsync();      // 選項 3");
            Console.WriteLine("await QuickNHostCrudTest.TestSingleSubscriptionOperationAsync(); // 選項 4");
            Console.WriteLine();
        }
    }
}