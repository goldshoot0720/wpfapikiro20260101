using System;
using System.Threading.Tasks;
using wpfkiro20260101.Services;
using wpfkiro20260101.Models;

namespace wpfkiro20260101
{
    /// <summary>
    /// NHost CRUD 操作綜合測試
    /// 測試食品和訂閱的完整 CRUD 功能
    /// </summary>
    public class TestNHostCrudOperations
    {
        private readonly NHostService _nHostService;

        public TestNHostCrudOperations()
        {
            _nHostService = new NHostService();
        }

        /// <summary>
        /// 執行完整的 NHost CRUD 測試
        /// </summary>
        public async Task RunAllTestsAsync()
        {
            Console.WriteLine("=== NHost CRUD 操作測試開始 ===");
            Console.WriteLine();

            // 1. 測試連線
            await TestConnectionAsync();
            Console.WriteLine();

            // 2. 測試食品 CRUD 操作
            await TestFoodCrudOperationsAsync();
            Console.WriteLine();

            // 3. 測試訂閱 CRUD 操作
            await TestSubscriptionCrudOperationsAsync();
            Console.WriteLine();

            Console.WriteLine("=== NHost CRUD 操作測試完成 ===");
        }

        /// <summary>
        /// 測試 NHost 連線
        /// </summary>
        private async Task TestConnectionAsync()
        {
            Console.WriteLine("--- 測試 NHost 連線 ---");
            
            try
            {
                var initResult = await _nHostService.InitializeAsync();
                Console.WriteLine($"初始化結果: {(initResult ? "成功" : "失敗")}");

                var connectionResult = await _nHostService.TestConnectionAsync();
                Console.WriteLine($"連線測試結果: {(connectionResult ? "成功" : "失敗")}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"連線測試錯誤: {ex.Message}");
            }
        }

        /// <summary>
        /// 測試食品 CRUD 操作
        /// </summary>
        private async Task TestFoodCrudOperationsAsync()
        {
            Console.WriteLine("--- 測試食品 CRUD 操作 ---");
            
            string? createdFoodId = null;

            try
            {
                // 1. 測試獲取所有食品
                Console.WriteLine("1. 測試獲取所有食品...");
                var getFoodsResult = await _nHostService.GetFoodsAsync();
                if (getFoodsResult.Success)
                {
                    Console.WriteLine($"   成功獲取 {getFoodsResult.Data?.Length ?? 0} 筆食品資料");
                }
                else
                {
                    Console.WriteLine($"   獲取食品失敗: {getFoodsResult.ErrorMessage}");
                }

                // 2. 測試創建食品
                Console.WriteLine("2. 測試創建食品...");
                var newFood = new
                {
                    name = "測試食品_" + DateTime.Now.ToString("yyyyMMdd_HHmmss"),
                    price = 99.99,
                    photo = "test_food.jpg",
                    shop = "測試商店",
                    todate = DateTime.Now.AddDays(7).ToString("yyyy-MM-ddTHH:mm:ssZ"),
                    photohash = "test_hash_" + Guid.NewGuid().ToString("N")[..8]
                };

                var createResult = await _nHostService.CreateFoodAsync(newFood);
                if (createResult.Success)
                {
                    Console.WriteLine("   食品創建成功");
                    // 嘗試從結果中提取 ID
                    try
                    {
                        var foodJson = System.Text.Json.JsonSerializer.Serialize(createResult.Data);
                        var foodElement = System.Text.Json.JsonSerializer.Deserialize<System.Text.Json.JsonElement>(foodJson);
                        if (foodElement.TryGetProperty("id", out var idProperty))
                        {
                            createdFoodId = idProperty.GetString();
                            Console.WriteLine($"   創建的食品 ID: {createdFoodId}");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"   無法提取食品 ID: {ex.Message}");
                    }
                }
                else
                {
                    Console.WriteLine($"   食品創建失敗: {createResult.ErrorMessage}");
                }

                // 3. 測試更新食品 (如果創建成功)
                if (!string.IsNullOrEmpty(createdFoodId))
                {
                    Console.WriteLine("3. 測試更新食品...");
                    var updateData = new
                    {
                        name = "更新後的測試食品",
                        price = 149.99,
                        shop = "更新後的商店"
                    };

                    var updateResult = await _nHostService.UpdateFoodAsync(createdFoodId, updateData);
                    if (updateResult.Success)
                    {
                        Console.WriteLine("   食品更新成功");
                    }
                    else
                    {
                        Console.WriteLine($"   食品更新失敗: {updateResult.ErrorMessage}");
                    }
                }

                // 4. 測試刪除食品 (如果創建成功)
                if (!string.IsNullOrEmpty(createdFoodId))
                {
                    Console.WriteLine("4. 測試刪除食品...");
                    var deleteResult = await _nHostService.DeleteFoodAsync(createdFoodId);
                    if (deleteResult.Success && deleteResult.Data)
                    {
                        Console.WriteLine("   食品刪除成功");
                    }
                    else
                    {
                        Console.WriteLine($"   食品刪除失敗: {deleteResult.ErrorMessage}");
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"食品 CRUD 測試錯誤: {ex.Message}");
            }
        }

        /// <summary>
        /// 測試訂閱 CRUD 操作
        /// </summary>
        private async Task TestSubscriptionCrudOperationsAsync()
        {
            Console.WriteLine("--- 測試訂閱 CRUD 操作 ---");
            
            string? createdSubscriptionId = null;

            try
            {
                // 1. 測試獲取所有訂閱
                Console.WriteLine("1. 測試獲取所有訂閱...");
                var getSubscriptionsResult = await _nHostService.GetSubscriptionsAsync();
                if (getSubscriptionsResult.Success)
                {
                    Console.WriteLine($"   成功獲取 {getSubscriptionsResult.Data?.Length ?? 0} 筆訂閱資料");
                }
                else
                {
                    Console.WriteLine($"   獲取訂閱失敗: {getSubscriptionsResult.ErrorMessage}");
                }

                // 2. 測試創建訂閱
                Console.WriteLine("2. 測試創建訂閱...");
                var newSubscription = new
                {
                    name = "測試訂閱_" + DateTime.Now.ToString("yyyyMMdd_HHmmss"),
                    nextdate = DateTime.Now.AddDays(30).ToString("yyyy-MM-ddTHH:mm:ssZ"),
                    price = 299.00,
                    site = "test-service.com",
                    note = "這是一個測試訂閱",
                    account = "test@example.com"
                };

                var createResult = await _nHostService.CreateSubscriptionAsync(newSubscription);
                if (createResult.Success)
                {
                    Console.WriteLine("   訂閱創建成功");
                    // 嘗試從結果中提取 ID
                    try
                    {
                        var subscriptionJson = System.Text.Json.JsonSerializer.Serialize(createResult.Data);
                        var subscriptionElement = System.Text.Json.JsonSerializer.Deserialize<System.Text.Json.JsonElement>(subscriptionJson);
                        if (subscriptionElement.TryGetProperty("id", out var idProperty))
                        {
                            createdSubscriptionId = idProperty.GetString();
                            Console.WriteLine($"   創建的訂閱 ID: {createdSubscriptionId}");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"   無法提取訂閱 ID: {ex.Message}");
                    }
                }
                else
                {
                    Console.WriteLine($"   訂閱創建失敗: {createResult.ErrorMessage}");
                }

                // 3. 測試更新訂閱 (如果創建成功)
                if (!string.IsNullOrEmpty(createdSubscriptionId))
                {
                    Console.WriteLine("3. 測試更新訂閱...");
                    var updateData = new
                    {
                        name = "更新後的測試訂閱",
                        price = 399.00,
                        note = "這是更新後的測試訂閱"
                    };

                    var updateResult = await _nHostService.UpdateSubscriptionAsync(createdSubscriptionId, updateData);
                    if (updateResult.Success)
                    {
                        Console.WriteLine("   訂閱更新成功");
                    }
                    else
                    {
                        Console.WriteLine($"   訂閱更新失敗: {updateResult.ErrorMessage}");
                    }
                }

                // 4. 測試刪除訂閱 (如果創建成功)
                if (!string.IsNullOrEmpty(createdSubscriptionId))
                {
                    Console.WriteLine("4. 測試刪除訂閱...");
                    var deleteResult = await _nHostService.DeleteSubscriptionAsync(createdSubscriptionId);
                    if (deleteResult.Success && deleteResult.Data)
                    {
                        Console.WriteLine("   訂閱刪除成功");
                    }
                    else
                    {
                        Console.WriteLine($"   訂閱刪除失敗: {deleteResult.ErrorMessage}");
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"訂閱 CRUD 測試錯誤: {ex.Message}");
            }
        }

        /// <summary>
        /// 測試認證功能 (可選)
        /// </summary>
        public async Task TestAuthenticationAsync()
        {
            Console.WriteLine("--- 測試 NHost 認證功能 ---");
            
            try
            {
                // 測試註冊 (使用測試帳號)
                var testEmail = $"test_{DateTime.Now:yyyyMMddHHmmss}@example.com";
                var testPassword = "TestPassword123!";

                Console.WriteLine("1. 測試用戶註冊...");
                var registerResult = await _nHostService.RegisterAsync(testEmail, testPassword);
                if (registerResult.Success)
                {
                    Console.WriteLine($"   註冊成功，用戶 ID: {registerResult.Data}");
                }
                else
                {
                    Console.WriteLine($"   註冊失敗: {registerResult.ErrorMessage}");
                }

                // 測試登入
                Console.WriteLine("2. 測試用戶登入...");
                var loginResult = await _nHostService.LoginAsync(testEmail, testPassword);
                if (loginResult.Success)
                {
                    Console.WriteLine($"   登入成功，Access Token: {registerResult.Data?[..20]}...");
                }
                else
                {
                    Console.WriteLine($"   登入失敗: {loginResult.ErrorMessage}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"認證測試錯誤: {ex.Message}");
            }
        }

        /// <summary>
        /// 快速測試方法 - 僅測試基本連線和讀取功能
        /// </summary>
        public async Task QuickTestAsync()
        {
            Console.WriteLine("=== NHost 快速測試 ===");
            
            try
            {
                // 測試初始化
                var initResult = await _nHostService.InitializeAsync();
                Console.WriteLine($"初始化: {(initResult ? "✓" : "✗")}");

                // 測試獲取食品
                var foodsResult = await _nHostService.GetFoodsAsync();
                Console.WriteLine($"獲取食品: {(foodsResult.Success ? "✓" : "✗")} ({foodsResult.Data?.Length ?? 0} 筆)");

                // 測試獲取訂閱
                var subscriptionsResult = await _nHostService.GetSubscriptionsAsync();
                Console.WriteLine($"獲取訂閱: {(subscriptionsResult.Success ? "✓" : "✗")} ({subscriptionsResult.Data?.Length ?? 0} 筆)");

                Console.WriteLine("快速測試完成");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"快速測試錯誤: {ex.Message}");
            }
        }
    }
}