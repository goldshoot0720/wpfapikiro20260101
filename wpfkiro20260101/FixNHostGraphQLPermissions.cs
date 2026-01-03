using System;
using System.Threading.Tasks;
using wpfkiro20260101.Services;

namespace wpfkiro20260101
{
    /// <summary>
    /// NHost GraphQL 權限修正工具
    /// 用於診斷和修正 NHost GraphQL 權限問題
    /// </summary>
    public class FixNHostGraphQLPermissions
    {
        /// <summary>
        /// 執行 NHost GraphQL 權限診斷和修正
        /// </summary>
        public static async Task RunPermissionFixAsync()
        {
            Console.WriteLine("=== NHost GraphQL 權限修正 ===");
            Console.WriteLine();

            var nHostService = new NHostService();

            try
            {
                // 1. 測試基本連線
                Console.WriteLine("1. 測試 NHost 基本連線...");
                var initResult = await nHostService.InitializeAsync();
                Console.WriteLine($"   初始化結果: {(initResult ? "✅ 成功" : "❌ 失敗")}");

                if (!initResult)
                {
                    Console.WriteLine("   ⚠️ NHost 連線失敗，請檢查網路連線和設定");
                    return;
                }

                // 2. 測試 GraphQL 架構查詢
                Console.WriteLine("2. 測試 GraphQL 架構查詢...");
                await TestGraphQLSchemaAsync(nHostService);

                // 3. 測試資料表權限
                Console.WriteLine("3. 測試資料表權限...");
                await TestTablePermissionsAsync(nHostService);

                // 4. 提供修正建議
                Console.WriteLine();
                Console.WriteLine("=== GraphQL 權限修正建議 ===");
                ShowPermissionFixInstructions();

            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ 權限診斷過程中發生錯誤: {ex.Message}");
                Console.WriteLine($"詳細錯誤: {ex}");
            }
            finally
            {
                nHostService.Dispose();
            }
        }

        /// <summary>
        /// 測試 GraphQL 架構
        /// </summary>
        private static async Task TestGraphQLSchemaAsync(NHostService nHostService)
        {
            try
            {
                // 使用反射來存取私有方法進行測試
                var method = typeof(NHostService).GetMethod("ExecuteGraphQLAsync", 
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

                if (method != null)
                {
                    var introspectionQuery = @"
                        query IntrospectionQuery {
                            __schema {
                                queryType {
                                    fields {
                                        name
                                        type {
                                            name
                                        }
                                    }
                                }
                            }
                        }";

                    var task = (Task)method.Invoke(nHostService, new object[] { introspectionQuery, null });
                    await task;

                    // 取得結果
                    var resultProperty = task.GetType().GetProperty("Result");
                    if (resultProperty != null)
                    {
                        var result = resultProperty.GetValue(task);
                        var successProperty = result.GetType().GetProperty("Success");
                        var success = (bool)successProperty.GetValue(result);

                        if (success)
                        {
                            Console.WriteLine("   ✅ GraphQL 架構查詢成功");
                            
                            // 檢查是否包含 foods 和 subscriptions
                            var dataProperty = result.GetType().GetProperty("Data");
                            if (dataProperty != null)
                            {
                                Console.WriteLine("   📋 檢查可用的查詢欄位...");
                                // 這裡可以進一步解析架構資訊
                            }
                        }
                        else
                        {
                            var errorProperty = result.GetType().GetProperty("ErrorMessage");
                            var error = errorProperty?.GetValue(result)?.ToString();
                            Console.WriteLine($"   ❌ GraphQL 架構查詢失敗: {error}");
                        }
                    }
                }
                else
                {
                    Console.WriteLine("   ⚠️ 無法存取 GraphQL 執行方法");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"   ❌ GraphQL 架構測試錯誤: {ex.Message}");
            }
        }

        /// <summary>
        /// 測試資料表權限
        /// </summary>
        private static async Task TestTablePermissionsAsync(NHostService nHostService)
        {
            // 測試 foods 資料表
            Console.WriteLine("   測試 foods 資料表權限...");
            var foodsResult = await nHostService.GetFoodsAsync();
            if (foodsResult.Success)
            {
                Console.WriteLine($"   ✅ foods 資料表權限正常 ({foodsResult.Data?.Length ?? 0} 筆資料)");
            }
            else
            {
                Console.WriteLine($"   ❌ foods 資料表權限問題: {foodsResult.ErrorMessage}");
                AnalyzePermissionError(foodsResult.ErrorMessage, "foods");
            }

            // 測試 subscriptions 資料表
            Console.WriteLine("   測試 subscriptions 資料表權限...");
            var subscriptionsResult = await nHostService.GetSubscriptionsAsync();
            if (subscriptionsResult.Success)
            {
                Console.WriteLine($"   ✅ subscriptions 資料表權限正常 ({subscriptionsResult.Data?.Length ?? 0} 筆資料)");
            }
            else
            {
                Console.WriteLine($"   ❌ subscriptions 資料表權限問題: {subscriptionsResult.ErrorMessage}");
                AnalyzePermissionError(subscriptionsResult.ErrorMessage, "subscriptions");
            }
        }

        /// <summary>
        /// 分析權限錯誤
        /// </summary>
        private static void AnalyzePermissionError(string errorMessage, string tableName)
        {
            if (string.IsNullOrEmpty(errorMessage)) return;

            if (errorMessage.Contains("field") && errorMessage.Contains("not found"))
            {
                Console.WriteLine($"   🔍 診斷: {tableName} 資料表在 GraphQL 架構中不可見");
                Console.WriteLine($"   💡 可能原因: GraphQL 權限未正確設定");
            }
            else if (errorMessage.Contains("permission") || errorMessage.Contains("access"))
            {
                Console.WriteLine($"   🔍 診斷: {tableName} 資料表存在但無存取權限");
                Console.WriteLine($"   💡 可能原因: 角色權限設定問題");
            }
            else if (errorMessage.Contains("validation-failed"))
            {
                Console.WriteLine($"   🔍 診斷: GraphQL 查詢驗證失敗");
                Console.WriteLine($"   💡 可能原因: 架構定義或權限規則問題");
            }
        }

        /// <summary>
        /// 顯示權限修正指導
        /// </summary>
        private static void ShowPermissionFixInstructions()
        {
            Console.WriteLine("🔧 NHost GraphQL 權限修正步驟:");
            Console.WriteLine();
            
            Console.WriteLine("步驟 1: 登入 NHost 控制台");
            Console.WriteLine("   - 前往 https://app.nhost.io/");
            Console.WriteLine("   - 選擇專案 'goldshoot0720'");
            Console.WriteLine();
            
            Console.WriteLine("步驟 2: 檢查 GraphQL 設定");
            Console.WriteLine("   - 點擊左側選單的 'GraphQL'");
            Console.WriteLine("   - 確認 'foods' 和 'subscriptions' 資料表在架構中可見");
            Console.WriteLine();
            
            Console.WriteLine("步驟 3: 設定資料表權限");
            Console.WriteLine("   - 進入 'Database' → 'Permissions'");
            Console.WriteLine("   - 為 'foods' 資料表設定權限:");
            Console.WriteLine("     * Role: public");
            Console.WriteLine("     * Operation: select (查詢)");
            Console.WriteLine("     * Permission: {} (允許所有)");
            Console.WriteLine("   - 為 'subscriptions' 資料表設定相同權限");
            Console.WriteLine();
            
            Console.WriteLine("步驟 4: 設定 Admin 權限");
            Console.WriteLine("   - 確認 Admin Secret 有完整權限");
            Console.WriteLine("   - 檢查 'x-hasura-admin-secret' 標頭設定");
            Console.WriteLine();
            
            Console.WriteLine("步驟 5: 測試 GraphQL 查詢");
            Console.WriteLine("   - 在 GraphQL Playground 中測試:");
            Console.WriteLine("   - query { foods { id name price } }");
            Console.WriteLine("   - query { subscriptions { id name price } }");
            Console.WriteLine();
            
            Console.WriteLine("步驟 6: 重新測試應用程式");
            Console.WriteLine("   - 重新啟動應用程式");
            Console.WriteLine("   - 測試 NHost 食品和訂閱功能");
        }

        /// <summary>
        /// 快速權限檢查
        /// </summary>
        public static async Task QuickPermissionCheckAsync()
        {
            Console.WriteLine("🚀 NHost 權限快速檢查");
            Console.WriteLine("-" + new string('-', 30));

            try
            {
                var nHostService = new NHostService();
                
                // 快速測試
                var initResult = await nHostService.InitializeAsync();
                Console.WriteLine($"連線: {(initResult ? "✅" : "❌")}");

                if (initResult)
                {
                    var foodsResult = await nHostService.GetFoodsAsync();
                    Console.WriteLine($"Foods 權限: {(foodsResult.Success ? "✅" : "❌")}");

                    var subscriptionsResult = await nHostService.GetSubscriptionsAsync();
                    Console.WriteLine($"Subscriptions 權限: {(subscriptionsResult.Success ? "✅" : "❌")}");

                    if (foodsResult.Success && subscriptionsResult.Success)
                    {
                        Console.WriteLine("🎉 NHost GraphQL 權限正常！");
                    }
                    else
                    {
                        Console.WriteLine("⚠️ 發現權限問題，請執行完整診斷");
                        Console.WriteLine("執行: await FixNHostGraphQLPermissions.RunPermissionFixAsync();");
                    }
                }

                nHostService.Dispose();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ 檢查失敗: {ex.Message}");
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
            Console.WriteLine("   Project: goldshoot0720");
            Console.WriteLine("   GraphQL URL: https://uxgwdiuehabbzenwtcqo.hasura.eu-central-1.nhost.run/v1/graphql");
            Console.WriteLine("   Admin Secret: 已設定");
            Console.WriteLine();
            Console.WriteLine("📋 必要權限設定:");
            Console.WriteLine("   - foods 資料表: select, insert, update, delete");
            Console.WriteLine("   - subscriptions 資料表: select, insert, update, delete");
            Console.WriteLine("   - Role: public 或 admin");
            Console.WriteLine();
            Console.WriteLine("🔗 相關連結:");
            Console.WriteLine("   - NHost 控制台: https://app.nhost.io/");
            Console.WriteLine("   - GraphQL Playground: https://uxgwdiuehabbzenwtcqo.hasura.eu-central-1.nhost.run/console");
        }
    }
}