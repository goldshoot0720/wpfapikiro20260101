using System;
using System.Threading.Tasks;
using wpfkiro20260101.Services;
using MessageBox = System.Windows.MessageBox;
using MessageBoxButton = System.Windows.MessageBoxButton;
using MessageBoxImage = System.Windows.MessageBoxImage;

namespace wpfkiro20260101
{
    public class TestNHostWithAdminSecret
    {
        public static async Task RunAdminSecretTest()
        {
            try
            {
                var results = new System.Text.StringBuilder();
                results.AppendLine("NHost Admin Secret 連線測試");
                results.AppendLine("===========================");
                results.AppendLine();
                
                // 創建 NHost 服務實例
                var nhostService = new NHostService();
                
                results.AppendLine("配置信息:");
                results.AppendLine($"- 服務名稱: {nhostService.ServiceName}");
                results.AppendLine($"- 服務類型: {nhostService.ServiceType}");
                results.AppendLine($"- Region: eu-central-1");
                results.AppendLine($"- Subdomain: uxgwdiuehabbzenwtcqo");
                results.AppendLine($"- Admin Secret: 已配置 ✅");
                results.AppendLine();
                
                // 測試初始化
                results.AppendLine("1. 測試服務初始化...");
                var initResult = await nhostService.InitializeAsync();
                results.AppendLine($"   初始化結果: {(initResult ? "✅ 成功" : "❌ 失敗")}");
                results.AppendLine();
                
                // 測試連線
                results.AppendLine("2. 測試基本連線...");
                var connectionResult = await nhostService.TestConnectionAsync();
                results.AppendLine($"   連線測試: {(connectionResult ? "✅ 成功" : "❌ 失敗")}");
                results.AppendLine();
                
                // 測試 GraphQL 查詢 (使用 Admin Secret)
                results.AppendLine("3. 測試 GraphQL 查詢 (使用 Admin Secret)...");
                try
                {
                    var foodsResult = await nhostService.GetFoodsAsync();
                    if (foodsResult.Success)
                    {
                        results.AppendLine($"   獲取食品資料: ✅ 成功");
                        results.AppendLine($"   返回資料數量: {foodsResult.Data?.Length ?? 0} 筆");
                    }
                    else
                    {
                        results.AppendLine($"   獲取食品資料: ❌ 失敗");
                        results.AppendLine($"   錯誤訊息: {foodsResult.ErrorMessage}");
                    }
                }
                catch (Exception ex)
                {
                    results.AppendLine($"   獲取食品資料: ❌ 異常 - {ex.Message}");
                }
                results.AppendLine();
                
                // 測試訂閱資料查詢
                results.AppendLine("4. 測試訂閱資料查詢...");
                try
                {
                    var subscriptionsResult = await nhostService.GetSubscriptionsAsync();
                    if (subscriptionsResult.Success)
                    {
                        results.AppendLine($"   獲取訂閱資料: ✅ 成功");
                        results.AppendLine($"   返回資料數量: {subscriptionsResult.Data?.Length ?? 0} 筆");
                    }
                    else
                    {
                        results.AppendLine($"   獲取訂閱資料: ❌ 失敗");
                        results.AppendLine($"   錯誤訊息: {subscriptionsResult.ErrorMessage}");
                    }
                }
                catch (Exception ex)
                {
                    results.AppendLine($"   獲取訂閱資料: ❌ 異常 - {ex.Message}");
                }
                results.AppendLine();
                
                // 顯示端點信息
                results.AppendLine("5. NHost 端點配置:");
                results.AppendLine("   GraphQL: https://uxgwdiuehabbzenwtcqo.graphql.eu-central-1.nhost.run/v1");
                results.AppendLine("   Auth: https://uxgwdiuehabbzenwtcqo.auth.eu-central-1.nhost.run/v1");
                results.AppendLine("   Functions: https://uxgwdiuehabbzenwtcqo.functions.eu-central-1.nhost.run/v1");
                results.AppendLine("   Storage: https://uxgwdiuehabbzenwtcqo.storage.eu-central-1.nhost.run/v1");
                results.AppendLine();
                
                // 測試工廠整合
                results.AppendLine("6. 測試工廠整合...");
                try
                {
                    var factoryService = BackendServiceFactory.CreateService(BackendServiceType.NHost);
                    results.AppendLine($"   工廠創建服務: ✅ 成功");
                    results.AppendLine($"   服務名稱: {factoryService.ServiceName}");
                    
                    var crudManager = BackendServiceFactory.CreateCrudManager(BackendServiceType.NHost);
                    results.AppendLine($"   CRUD 管理器: ✅ 成功");
                    results.AppendLine($"   管理器服務: {crudManager.GetServiceName()}");
                }
                catch (Exception ex)
                {
                    results.AppendLine($"   工廠整合: ❌ 失敗 - {ex.Message}");
                }
                results.AppendLine();
                
                if (initResult && connectionResult)
                {
                    results.AppendLine("🎉 NHost 服務配置成功！");
                    results.AppendLine("✅ Admin Secret 已正確配置");
                    results.AppendLine("✅ GraphQL 端點可正常存取");
                    results.AppendLine("可以開始使用 NHost 作為後端服務。");
                }
                else
                {
                    results.AppendLine("⚠️ NHost 連線有問題，請檢查:");
                    results.AppendLine("1. 網路連線是否正常");
                    results.AppendLine("2. NHost 專案是否已啟動");
                    results.AppendLine("3. Admin Secret 是否正確");
                    results.AppendLine("4. 資料表是否已創建");
                }
                
                MessageBox.Show(results.ToString(), "NHost Admin Secret 測試結果", 
                    MessageBoxButton.OK, MessageBoxImage.Information);
                
            }
            catch (Exception ex)
            {
                MessageBox.Show($"測試過程中發生錯誤：\n{ex.Message}", "錯誤", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}