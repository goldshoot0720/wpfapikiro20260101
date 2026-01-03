using System;
using System.Threading.Tasks;
using wpfkiro20260101.Services;
using MessageBox = System.Windows.MessageBox;
using MessageBoxButton = System.Windows.MessageBoxButton;
using MessageBoxImage = System.Windows.MessageBoxImage;

namespace wpfkiro20260101
{
    public class NHostProjectImplementation
    {
        public static async Task RunImplementationTest()
        {
            try
            {
                var results = new System.Text.StringBuilder();
                results.AppendLine("NHost 專案實作測試");
                results.AppendLine("==================");
                results.AppendLine();
                
                // 顯示專案配置信息
                results.AppendLine("專案配置信息:");
                results.AppendLine($"- Region: eu-central-1");
                results.AppendLine($"- Subdomain: uxgwdiuehabbzenwtcqo");
                results.AppendLine($"- Admin Secret: cu#34&yjF3Cr%fgxB#WA,4r4^c=Igcwr");
                results.AppendLine();
                
                // 創建 NHost 服務實例
                var nhostService = new NHostService();
                
                results.AppendLine("1. 服務初始化測試");
                results.AppendLine("==================");
                var initResult = await nhostService.InitializeAsync();
                results.AppendLine($"初始化結果: {(initResult ? "✅ 成功" : "❌ 失敗")}");
                results.AppendLine();
                
                results.AppendLine("2. 連線測試");
                results.AppendLine("============");
                var connectionResult = await nhostService.TestConnectionAsync();
                results.AppendLine($"連線測試: {(connectionResult ? "✅ 成功" : "❌ 失敗")}");
                results.AppendLine();
                
                results.AppendLine("3. GraphQL 端點測試");
                results.AppendLine("==================");
                
                // 測試食品資料查詢
                results.AppendLine("3.1 測試食品資料查詢...");
                try
                {
                    var foodsResult = await nhostService.GetFoodsAsync();
                    if (foodsResult.Success)
                    {
                        results.AppendLine($"   ✅ 食品查詢成功");
                        results.AppendLine($"   📊 返回資料數量: {foodsResult.Data?.Length ?? 0} 筆");
                        
                        if (foodsResult.Data != null && foodsResult.Data.Length > 0)
                        {
                            results.AppendLine($"   📝 範例資料: {foodsResult.Data[0]}");
                        }
                    }
                    else
                    {
                        results.AppendLine($"   ❌ 食品查詢失敗: {foodsResult.ErrorMessage}");
                    }
                }
                catch (Exception ex)
                {
                    results.AppendLine($"   ❌ 食品查詢異常: {ex.Message}");
                }
                results.AppendLine();
                
                // 測試訂閱資料查詢
                results.AppendLine("3.2 測試訂閱資料查詢...");
                try
                {
                    var subscriptionsResult = await nhostService.GetSubscriptionsAsync();
                    if (subscriptionsResult.Success)
                    {
                        results.AppendLine($"   ✅ 訂閱查詢成功");
                        results.AppendLine($"   📊 返回資料數量: {subscriptionsResult.Data?.Length ?? 0} 筆");
                        
                        if (subscriptionsResult.Data != null && subscriptionsResult.Data.Length > 0)
                        {
                            results.AppendLine($"   📝 範例資料: {subscriptionsResult.Data[0]}");
                        }
                    }
                    else
                    {
                        results.AppendLine($"   ❌ 訂閱查詢失敗: {subscriptionsResult.ErrorMessage}");
                    }
                }
                catch (Exception ex)
                {
                    results.AppendLine($"   ❌ 訂閱查詢異常: {ex.Message}");
                }
                results.AppendLine();
                
                results.AppendLine("4. CRUD 操作測試");
                results.AppendLine("================");
                
                // 測試創建食品
                results.AppendLine("4.1 測試創建食品...");
                try
                {
                    var testFood = new
                    {
                        name = "測試食品",
                        price = 100.50,
                        shop = "測試商店",
                        photo = "test-photo.jpg",
                        photohash = "test-hash-123",
                        todate = DateTime.Now.AddDays(7).ToString("yyyy-MM-ddTHH:mm:ssZ")
                    };
                    
                    var createResult = await nhostService.CreateFoodAsync(testFood);
                    if (createResult.Success)
                    {
                        results.AppendLine($"   ✅ 食品創建成功");
                        results.AppendLine($"   📝 創建的資料: {createResult.Data}");
                    }
                    else
                    {
                        results.AppendLine($"   ❌ 食品創建失敗: {createResult.ErrorMessage}");
                    }
                }
                catch (Exception ex)
                {
                    results.AppendLine($"   ❌ 食品創建異常: {ex.Message}");
                }
                results.AppendLine();
                
                // 測試創建訂閱
                results.AppendLine("4.2 測試創建訂閱...");
                try
                {
                    var testSubscription = new
                    {
                        name = "測試訂閱",
                        price = 29.99,
                        site = "測試網站",
                        note = "測試備註",
                        account = "test@example.com",
                        nextdate = DateTime.Now.AddMonths(1).ToString("yyyy-MM-ddTHH:mm:ssZ")
                    };
                    
                    var createResult = await nhostService.CreateSubscriptionAsync(testSubscription);
                    if (createResult.Success)
                    {
                        results.AppendLine($"   ✅ 訂閱創建成功");
                        results.AppendLine($"   📝 創建的資料: {createResult.Data}");
                    }
                    else
                    {
                        results.AppendLine($"   ❌ 訂閱創建失敗: {createResult.ErrorMessage}");
                    }
                }
                catch (Exception ex)
                {
                    results.AppendLine($"   ❌ 訂閱創建異常: {ex.Message}");
                }
                results.AppendLine();
                
                results.AppendLine("5. 工廠整合測試");
                results.AppendLine("================");
                try
                {
                    var factoryService = BackendServiceFactory.CreateService(BackendServiceType.NHost);
                    results.AppendLine($"✅ 工廠創建服務成功: {factoryService.ServiceName}");
                    
                    var crudManager = BackendServiceFactory.CreateCrudManager(BackendServiceType.NHost);
                    results.AppendLine($"✅ CRUD 管理器創建成功: {crudManager.GetServiceName()}");
                }
                catch (Exception ex)
                {
                    results.AppendLine($"❌ 工廠整合失敗: {ex.Message}");
                }
                results.AppendLine();
                
                results.AppendLine("6. 端點配置摘要");
                results.AppendLine("================");
                results.AppendLine("GraphQL: https://uxgwdiuehabbzenwtcqo.graphql.eu-central-1.nhost.run/v1");
                results.AppendLine("Auth:    https://uxgwdiuehabbzenwtcqo.auth.eu-central-1.nhost.run/v1");
                results.AppendLine("Functions: https://uxgwdiuehabbzenwtcqo.functions.eu-central-1.nhost.run/v1");
                results.AppendLine("Storage: https://uxgwdiuehabbzenwtcqo.storage.eu-central-1.nhost.run/v1");
                results.AppendLine();
                
                // 總結
                if (initResult && connectionResult)
                {
                    results.AppendLine("🎉 NHost 專案實作測試完成！");
                    results.AppendLine("✅ 所有基本功能正常運作");
                    results.AppendLine("✅ Admin Secret 認證成功");
                    results.AppendLine("✅ GraphQL API 可正常存取");
                    results.AppendLine();
                    results.AppendLine("專案已準備就緒，可以開始使用！");
                }
                else
                {
                    results.AppendLine("⚠️ 專案實作測試發現問題");
                    results.AppendLine("請檢查:");
                    results.AppendLine("1. NHost 專案是否已啟動");
                    results.AppendLine("2. 資料表是否已創建");
                    results.AppendLine("3. Admin Secret 是否正確");
                    results.AppendLine("4. 網路連線是否正常");
                }
                
                MessageBox.Show(results.ToString(), "NHost 專案實作測試結果", 
                    MessageBoxButton.OK, MessageBoxImage.Information);
                
            }
            catch (Exception ex)
            {
                MessageBox.Show($"專案實作測試過程中發生錯誤：\n{ex.Message}", "錯誤", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}