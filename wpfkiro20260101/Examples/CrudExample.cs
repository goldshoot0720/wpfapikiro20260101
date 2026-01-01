using System;
using System.Threading.Tasks;
using wpfkiro20260101.Models;
using wpfkiro20260101.Services;

namespace wpfkiro20260101.Examples
{
    /// <summary>
    /// CRUD 操作範例類別
    /// 展示如何使用 CrudManager 進行完整的 CRUD 操作
    /// </summary>
    public class CrudExample
    {
        private readonly CrudManager _crudManager;

        public CrudExample()
        {
            // 使用工廠方法創建 CRUD 管理器，會自動使用當前設定的後端服務
            _crudManager = BackendServiceFactory.CreateCrudManager();
        }

        public CrudExample(BackendServiceType serviceType)
        {
            // 使用指定的後端服務類型創建 CRUD 管理器
            _crudManager = BackendServiceFactory.CreateCrudManager(serviceType);
        }

        /// <summary>
        /// 完整的 CRUD 操作示範 - 食品
        /// </summary>
        public async Task RunFoodCrudExampleAsync()
        {
            Console.WriteLine($"=== 食品 CRUD 操作示範 - 使用 {_crudManager.GetServiceName()} ===");

            // 1. 測試連線
            Console.WriteLine("\n1. 測試後端服務連線...");
            var connectionTest = await _crudManager.TestConnectionAsync();
            Console.WriteLine($"連線測試結果: {(connectionTest ? "成功" : "失敗")}");

            if (!connectionTest)
            {
                Console.WriteLine("連線失敗，請檢查後端服務設定");
                return;
            }

            // 2. Create - 創建新的食品
            Console.WriteLine("\n2. 創建新的食品...");
            var newFood = new Food
            {
                FoodName = "有機蘋果",
                Price = 120,
                Shop = "有機農場直送",
                ToDate = "2026-01-15",
                Description = "來自台灣有機農場的新鮮蘋果",
                Category = "水果",
                StorageLocation = "冰箱",
                Note = "新鮮有機蘋果",
                Photo = "apple.jpg",
                PhotoHash = "abc123"
            };

            var createResult = await _crudManager.CreateFoodAsync(newFood);
            if (createResult.Success)
            {
                Console.WriteLine("✅ 創建成功！");
                Console.WriteLine($"創建的資料: {createResult.Data}");
            }
            else
            {
                Console.WriteLine($"❌ 創建失敗: {createResult.ErrorMessage}");
                return;
            }

            // 3. Read - 讀取所有食品
            Console.WriteLine("\n3. 讀取所有食品...");
            var readResult = await _crudManager.GetAllFoodsAsync();
            if (readResult.Success && readResult.Data != null)
            {
                Console.WriteLine($"✅ 讀取成功！找到 {readResult.Data.Length} 項記錄");
                foreach (var item in readResult.Data)
                {
                    Console.WriteLine($"  - {item}");
                }
            }
            else
            {
                Console.WriteLine($"❌ 讀取失敗: {readResult.ErrorMessage}");
            }

            // 4. Update - 更新食品（假設我們有ID）
            Console.WriteLine("\n4. 更新食品...");
            var foodId = "example-id"; // 這應該是實際的ID
            
            var updatedFood = new Food
            {
                FoodName = "有機蘋果 (更新版)",
                Price = 150,
                Shop = "有機農場直送",
                ToDate = "2026-01-20",
                Description = "來自台灣有機農場的新鮮蘋果 - 升級版",
                Category = "水果",
                StorageLocation = "冰箱",
                Note = "新鮮有機蘋果 - 已升級包裝",
                Photo = "apple_premium.jpg",
                PhotoHash = "def456"
            };

            var updateResult = await _crudManager.UpdateFoodAsync(foodId, updatedFood);
            if (updateResult.Success)
            {
                Console.WriteLine("✅ 更新成功！");
                Console.WriteLine($"更新的資料: {updateResult.Data}");
            }
            else
            {
                Console.WriteLine($"❌ 更新失敗: {updateResult.ErrorMessage}");
            }

            // 5. Delete - 刪除食品
            Console.WriteLine("\n5. 刪除食品...");
            var deleteResult = await _crudManager.DeleteFoodAsync(foodId);
            if (deleteResult.Success)
            {
                Console.WriteLine("✅ 刪除成功！");
            }
            else
            {
                Console.WriteLine($"❌ 刪除失敗: {deleteResult.ErrorMessage}");
            }

            // 6. 批量刪除示範
            Console.WriteLine("\n6. 批量刪除示範...");
            var idsToDelete = new[] { "id1", "id2", "id3" }; // 這些應該是實際的ID
            var batchDeleteResult = await _crudManager.DeleteMultipleFoodsAsync(idsToDelete);
            if (batchDeleteResult.Success)
            {
                Console.WriteLine($"✅ 批量刪除成功！刪除了 {batchDeleteResult.Data} 項記錄");
            }
            else
            {
                Console.WriteLine($"❌ 批量刪除失敗: {batchDeleteResult.ErrorMessage}");
            }

            Console.WriteLine("\n=== 食品 CRUD 操作示範完成 ===");
        }

        /// <summary>
        /// 完整的 CRUD 操作示範 - 訂閱
        /// </summary>
        public async Task RunSubscriptionCrudExampleAsync()
        {
            Console.WriteLine($"=== 訂閱 CRUD 操作示範 - 使用 {_crudManager.GetServiceName()} ===");

            // 1. 測試連線
            Console.WriteLine("\n1. 測試後端服務連線...");
            var connectionTest = await _crudManager.TestConnectionAsync();
            Console.WriteLine($"連線測試結果: {(connectionTest ? "成功" : "失敗")}");

            if (!connectionTest)
            {
                Console.WriteLine("連線失敗，請檢查後端服務設定");
                return;
            }

            // 2. Create - 創建新的訂閱
            Console.WriteLine("\n2. 創建新的訂閱...");
            var newSubscription = new Subscription
            {
                SubscriptionName = "每週有機水果訂閱",
                Price = 150,
                NextDate = DateTime.Parse("2026-01-15"),
                Site = "https://organic-farm.com",
                Note = "每週配送新鮮有機水果",
                Account = "user@example.com",
                StringToDate = "2026-01-15",
                DateTime = DateTime.Parse("2026-01-15")
            };

            var createResult = await _crudManager.CreateSubscriptionAsync(newSubscription);
            if (createResult.Success)
            {
                Console.WriteLine("✅ 創建成功！");
                Console.WriteLine($"創建的資料: {createResult.Data}");
            }
            else
            {
                Console.WriteLine($"❌ 創建失敗: {createResult.ErrorMessage}");
                return;
            }

            // 3. Read - 讀取所有訂閱
            Console.WriteLine("\n3. 讀取所有訂閱...");
            var readResult = await _crudManager.GetAllSubscriptionsAsync();
            if (readResult.Success && readResult.Data != null)
            {
                Console.WriteLine($"✅ 讀取成功！找到 {readResult.Data.Length} 項記錄");
                foreach (var item in readResult.Data)
                {
                    Console.WriteLine($"  - {item}");
                }
            }
            else
            {
                Console.WriteLine($"❌ 讀取失敗: {readResult.ErrorMessage}");
            }

            // 4. Update - 更新訂閱（假設我們有ID）
            Console.WriteLine("\n4. 更新訂閱...");
            var subscriptionId = "example-id"; // 這應該是實際的ID
            
            var updatedSubscription = new Subscription
            {
                SubscriptionName = "每週有機水果訂閱 - 升級版",
                Price = 180,
                NextDate = DateTime.Parse("2026-01-20"),
                Site = "https://organic-farm.com",
                Note = "每週配送新鮮有機水果 - 已升級包裝",
                Account = "user@example.com",
                StringToDate = "2026-01-20",
                DateTime = DateTime.Parse("2026-01-20")
            };

            var updateResult = await _crudManager.UpdateSubscriptionAsync(subscriptionId, updatedSubscription);
            if (updateResult.Success)
            {
                Console.WriteLine("✅ 更新成功！");
                Console.WriteLine($"更新的資料: {updateResult.Data}");
            }
            else
            {
                Console.WriteLine($"❌ 更新失敗: {updateResult.ErrorMessage}");
            }

            // 5. Delete - 刪除訂閱
            Console.WriteLine("\n5. 刪除訂閱...");
            var deleteResult = await _crudManager.DeleteSubscriptionAsync(subscriptionId);
            if (deleteResult.Success)
            {
                Console.WriteLine("✅ 刪除成功！");
            }
            else
            {
                Console.WriteLine($"❌ 刪除失敗: {deleteResult.ErrorMessage}");
            }

            // 6. 批量刪除示範
            Console.WriteLine("\n6. 批量刪除示範...");
            var idsToDelete = new[] { "id1", "id2", "id3" }; // 這些應該是實際的ID
            var batchDeleteResult = await _crudManager.DeleteMultipleSubscriptionsAsync(idsToDelete);
            if (batchDeleteResult.Success)
            {
                Console.WriteLine($"✅ 批量刪除成功！刪除了 {batchDeleteResult.Data} 項記錄");
            }
            else
            {
                Console.WriteLine($"❌ 批量刪除失敗: {batchDeleteResult.ErrorMessage}");
            }

            Console.WriteLine("\n=== 訂閱 CRUD 操作示範完成 ===");
        }

        /// <summary>
        /// 資料驗證示範
        /// </summary>
        public void RunValidationExample()
        {
            Console.WriteLine("\n=== 資料驗證示範 ===");

            // 測試有效食品資料
            var validFood = new Food
            {
                FoodName = "有機蘋果"
            };

            var foodValidationResult = CrudManager.ValidateFood(validFood);
            Console.WriteLine($"有效食品資料驗證: {(foodValidationResult.Success ? "通過" : foodValidationResult.ErrorMessage)}");

            // 測試無效食品資料 - 空名稱
            var invalidFood = new Food();
            var invalidFoodValidationResult = CrudManager.ValidateFood(invalidFood);
            Console.WriteLine($"無效食品資料驗證 (空名稱): {(invalidFoodValidationResult.Success ? "通過" : invalidFoodValidationResult.ErrorMessage)}");

            // 測試有效訂閱資料
            var validSubscription = new Subscription
            {
                SubscriptionName = "每週水果訂閱",
                Price = 150
            };

            var subscriptionValidationResult = CrudManager.ValidateSubscription(validSubscription);
            Console.WriteLine($"有效訂閱資料驗證: {(subscriptionValidationResult.Success ? "通過" : subscriptionValidationResult.ErrorMessage)}");

            // 測試無效訂閱資料 - 負價格
            var invalidSubscription = new Subscription
            {
                SubscriptionName = "水果訂閱",
                Price = -50
            };

            var invalidSubscriptionValidationResult = CrudManager.ValidateSubscription(invalidSubscription);
            Console.WriteLine($"無效訂閱資料驗證 (負價格): {(invalidSubscriptionValidationResult.Success ? "通過" : invalidSubscriptionValidationResult.ErrorMessage)}");

            // 測試 null 資料
            var nullFoodValidationResult = CrudManager.ValidateFood(null);
            Console.WriteLine($"Null 食品資料驗證: {(nullFoodValidationResult.Success ? "通過" : nullFoodValidationResult.ErrorMessage)}");

            var nullSubscriptionValidationResult = CrudManager.ValidateSubscription(null);
            Console.WriteLine($"Null 訂閱資料驗證: {(nullSubscriptionValidationResult.Success ? "通過" : nullSubscriptionValidationResult.ErrorMessage)}");
        }

        /// <summary>
        /// 不同後端服務比較示範
        /// </summary>
        public async Task RunServiceComparisonAsync()
        {
            Console.WriteLine("\n=== 不同後端服務比較 ===");

            var supportedServices = BackendServiceFactory.GetSupportedServices();
            
            foreach (var serviceType in supportedServices)
            {
                try
                {
                    Console.WriteLine($"\n測試 {serviceType} 服務:");
                    
                    var crudManager = BackendServiceFactory.CreateCrudManager(serviceType);
                    var connectionTest = await crudManager.TestConnectionAsync();
                    
                    Console.WriteLine($"  服務名稱: {crudManager.GetServiceName()}");
                    Console.WriteLine($"  連線狀態: {(connectionTest ? "✅ 成功" : "❌ 失敗")}");
                    
                    if (connectionTest)
                    {
                        var foodReadResult = await crudManager.GetAllFoodsAsync();
                        if (foodReadResult.Success && foodReadResult.Data != null)
                        {
                            Console.WriteLine($"  食品資料筆數: {foodReadResult.Data.Length}");
                        }
                        else
                        {
                            Console.WriteLine($"  食品資料讀取: ❌ {foodReadResult.ErrorMessage}");
                        }

                        var subscriptionReadResult = await crudManager.GetAllSubscriptionsAsync();
                        if (subscriptionReadResult.Success && subscriptionReadResult.Data != null)
                        {
                            Console.WriteLine($"  訂閱資料筆數: {subscriptionReadResult.Data.Length}");
                        }
                        else
                        {
                            Console.WriteLine($"  訂閱資料讀取: ❌ {subscriptionReadResult.ErrorMessage}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"  錯誤: {ex.Message}");
                }
            }
        }
    }
}