using System;
using System.Threading.Tasks;
using wpfkiro20260101.Services;

namespace wpfkiro20260101
{
    /// <summary>
    /// NHost 資料庫架構修正測試
    /// 用於診斷和修正 NHost GraphQL 架構問題
    /// </summary>
    public class TestNHostSchemaFix
    {
        /// <summary>
        /// 執行 NHost 架構診斷
        /// </summary>
        public static async Task RunSchemaDiagnosticsAsync()
        {
            Console.WriteLine("=== NHost 資料庫架構診斷 ===");
            Console.WriteLine();

            var nHostService = new NHostService();

            try
            {
                // 1. 測試基本連線
                Console.WriteLine("1. 測試 NHost 連線...");
                var initResult = await nHostService.InitializeAsync();
                Console.WriteLine($"   初始化結果: {(initResult ? "✅ 成功" : "❌ 失敗")}");

                if (!initResult)
                {
                    Console.WriteLine("   ⚠️ NHost 連線失敗，無法繼續診斷");
                    return;
                }

                // 2. 測試 GraphQL 端點
                Console.WriteLine("2. 測試 GraphQL 端點...");
                var connectionResult = await nHostService.TestConnectionAsync();
                Console.WriteLine($"   連線測試: {(connectionResult ? "✅ 成功" : "❌ 失敗")}");

                // 3. 測試食品資料表
                Console.WriteLine("3. 測試食品資料表存取...");
                var foodsResult = await nHostService.GetFoodsAsync();
                if (foodsResult.Success)
                {
                    Console.WriteLine($"   ✅ 食品資料表正常 ({foodsResult.Data?.Length ?? 0} 筆資料)");
                }
                else
                {
                    Console.WriteLine($"   ❌ 食品資料表錯誤: {foodsResult.ErrorMessage}");
                    
                    if (foodsResult.ErrorMessage?.Contains("未找到 'foods' 資料表") == true)
                    {
                        Console.WriteLine("   💡 建議: 請執行 CREATE_NHOST_TABLES.sql 腳本來創建資料表");
                    }
                }

                // 4. 測試訂閱資料表
                Console.WriteLine("4. 測試訂閱資料表存取...");
                var subscriptionsResult = await nHostService.GetSubscriptionsAsync();
                if (subscriptionsResult.Success)
                {
                    Console.WriteLine($"   ✅ 訂閱資料表正常 ({subscriptionsResult.Data?.Length ?? 0} 筆資料)");
                }
                else
                {
                    Console.WriteLine($"   ❌ 訂閱資料表錯誤: {subscriptionsResult.ErrorMessage}");
                    
                    if (subscriptionsResult.ErrorMessage?.Contains("未找到 'subscriptions' 資料表") == true)
                    {
                        Console.WriteLine("   💡 建議: 請執行 CREATE_NHOST_TABLES.sql 腳本來創建資料表");
                    }
                }

                // 5. 提供修正建議
                Console.WriteLine();
                Console.WriteLine("=== 修正建議 ===");
                
                if (!foodsResult.Success || !subscriptionsResult.Success)
                {
                    Console.WriteLine("🔧 資料表設定問題修正步驟:");
                    Console.WriteLine("   1. 登入 NHost 控制台: https://app.nhost.io/");
                    Console.WriteLine("   2. 選擇您的專案 (uxgwdiuehabbzenwtcqo)");
                    Console.WriteLine("   3. 進入 Database 頁面");
                    Console.WriteLine("   4. 在 SQL Editor 中執行 CREATE_NHOST_TABLES.sql 腳本");
                    Console.WriteLine("   5. 確認 foods 和 subscriptions 資料表已創建");
                    Console.WriteLine("   6. 重新測試應用程式");
                }
                else
                {
                    Console.WriteLine("✅ NHost 資料庫架構正常，所有資料表都可正常存取");
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ 診斷過程中發生錯誤: {ex.Message}");
                Console.WriteLine($"詳細錯誤: {ex}");
            }
            finally
            {
                nHostService.Dispose();
            }
        }

        /// <summary>
        /// 測試 NHost 資料表創建
        /// </summary>
        public static async Task TestTableCreationAsync()
        {
            Console.WriteLine("=== NHost 資料表創建測試 ===");
            Console.WriteLine();

            var nHostService = new NHostService();

            try
            {
                // 測試創建測試資料
                Console.WriteLine("1. 測試創建食品資料...");
                var testFood = new
                {
                    name = $"測試食品_{DateTime.Now:HHmmss}",
                    price = 99.99,
                    photo = "test.jpg",
                    shop = "測試商店",
                    todate = DateTime.Now.AddDays(7).ToString("yyyy-MM-ddTHH:mm:ssZ"),
                    photohash = $"hash_{Guid.NewGuid():N}"[..16]
                };

                var createFoodResult = await nHostService.CreateFoodAsync(testFood);
                if (createFoodResult.Success)
                {
                    Console.WriteLine("   ✅ 食品資料創建成功");
                }
                else
                {
                    Console.WriteLine($"   ❌ 食品資料創建失敗: {createFoodResult.ErrorMessage}");
                }

                Console.WriteLine("2. 測試創建訂閱資料...");
                var testSubscription = new
                {
                    name = $"測試訂閱_{DateTime.Now:HHmmss}",
                    nextdate = DateTime.Now.AddDays(30).ToString("yyyy-MM-ddTHH:mm:ssZ"),
                    price = 199.00,
                    site = "test.com",
                    note = "測試訂閱",
                    account = "test@example.com"
                };

                var createSubscriptionResult = await nHostService.CreateSubscriptionAsync(testSubscription);
                if (createSubscriptionResult.Success)
                {
                    Console.WriteLine("   ✅ 訂閱資料創建成功");
                }
                else
                {
                    Console.WriteLine($"   ❌ 訂閱資料創建失敗: {createSubscriptionResult.ErrorMessage}");
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ 測試過程中發生錯誤: {ex.Message}");
            }
            finally
            {
                nHostService.Dispose();
            }
        }

        /// <summary>
        /// 顯示 NHost 設定資訊
        /// </summary>
        public static void ShowNHostConfiguration()
        {
            Console.WriteLine("=== NHost 設定資訊 ===");
            Console.WriteLine();
            Console.WriteLine("🔧 當前 NHost 配置:");
            Console.WriteLine("   Region: eu-central-1");
            Console.WriteLine("   Subdomain: uxgwdiuehabbzenwtcqo");
            Console.WriteLine("   GraphQL URL: https://uxgwdiuehabbzenwtcqo.hasura.eu-central-1.nhost.run/v1/graphql");
            Console.WriteLine("   Admin Secret: 已設定");
            Console.WriteLine();
            Console.WriteLine("📋 必要資料表:");
            Console.WriteLine("   - foods (食品資料)");
            Console.WriteLine("   - subscriptions (訂閱資料)");
            Console.WriteLine();
            Console.WriteLine("📁 相關檔案:");
            Console.WriteLine("   - CREATE_NHOST_TABLES.sql (資料表創建腳本)");
            Console.WriteLine("   - Services/NHostService.cs (服務實作)");
            Console.WriteLine("   - TestNHostSchemaFix.cs (本診斷工具)");
        }

        /// <summary>
        /// 快速修正驗證
        /// </summary>
        public static async Task QuickFixVerificationAsync()
        {
            Console.WriteLine("🚀 NHost 快速修正驗證");
            Console.WriteLine("-" + new string('-', 30));

            try
            {
                var nHostService = new NHostService();
                
                // 快速測試
                var initResult = await nHostService.InitializeAsync();
                Console.WriteLine($"初始化: {(initResult ? "✅" : "❌")}");

                if (initResult)
                {
                    var foodsResult = await nHostService.GetFoodsAsync();
                    Console.WriteLine($"食品資料表: {(foodsResult.Success ? "✅" : "❌")}");

                    var subscriptionsResult = await nHostService.GetSubscriptionsAsync();
                    Console.WriteLine($"訂閱資料表: {(subscriptionsResult.Success ? "✅" : "❌")}");

                    if (foodsResult.Success && subscriptionsResult.Success)
                    {
                        Console.WriteLine("🎉 NHost 架構修正成功！");
                    }
                    else
                    {
                        Console.WriteLine("⚠️ 仍有資料表問題，請執行完整診斷");
                    }
                }

                nHostService.Dispose();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ 驗證失敗: {ex.Message}");
            }
        }
    }
}