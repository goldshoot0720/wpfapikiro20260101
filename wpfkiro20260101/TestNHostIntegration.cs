using System;
using System.Threading.Tasks;
using wpfkiro20260101.Services;
using MessageBox = System.Windows.MessageBox;
using MessageBoxButton = System.Windows.MessageBoxButton;
using MessageBoxImage = System.Windows.MessageBoxImage;

namespace wpfkiro20260101
{
    public class TestNHostIntegration
    {
        public static async Task RunIntegrationTest()
        {
            try
            {
                var results = new System.Text.StringBuilder();
                results.AppendLine("NHost 整合測試");
                results.AppendLine("==============");
                results.AppendLine();
                
                // 測試 BackendServiceFactory 支援
                results.AppendLine("1. 測試 BackendServiceFactory 支援");
                var supportedServices = BackendServiceFactory.GetSupportedServices();
                var nhostSupported = BackendServiceFactory.IsServiceSupported(BackendServiceType.NHost);
                
                results.AppendLine($"   NHost 是否受支援: {(nhostSupported ? "✅ 是" : "❌ 否")}");
                results.AppendLine($"   支援的服務數量: {supportedServices.Length}");
                
                foreach (var service in supportedServices)
                {
                    results.AppendLine($"   - {service}");
                }
                results.AppendLine();
                
                // 測試透過 Factory 創建 NHost 服務
                results.AppendLine("2. 測試透過 Factory 創建 NHost 服務");
                try
                {
                    var nhostService = BackendServiceFactory.CreateService(BackendServiceType.NHost);
                    results.AppendLine($"   創建服務: ✅ 成功");
                    results.AppendLine($"   服務名稱: {nhostService.ServiceName}");
                    results.AppendLine($"   服務類型: {nhostService.ServiceType}");
                    
                    // 測試服務功能
                    results.AppendLine();
                    results.AppendLine("3. 測試服務功能");
                    
                    var initResult = await nhostService.InitializeAsync();
                    results.AppendLine($"   初始化: {(initResult ? "✅ 成功" : "❌ 失敗")}");
                    
                    var connectionResult = await nhostService.TestConnectionAsync();
                    results.AppendLine($"   連線測試: {(connectionResult ? "✅ 成功" : "❌ 失敗")}");
                    
                    // 測試 CRUD 管理器
                    results.AppendLine();
                    results.AppendLine("4. 測試 CRUD 管理器整合");
                    try
                    {
                        var crudManager = BackendServiceFactory.CreateCrudManager(BackendServiceType.NHost);
                        results.AppendLine($"   CRUD 管理器創建: ✅ 成功");
                        results.AppendLine($"   後端服務: {crudManager.GetServiceName()}");
                        results.AppendLine($"   服務類型: {crudManager.GetServiceType()}");
                    }
                    catch (Exception ex)
                    {
                        results.AppendLine($"   CRUD 管理器創建: ❌ 失敗 - {ex.Message}");
                    }
                    
                }
                catch (Exception ex)
                {
                    results.AppendLine($"   創建服務: ❌ 失敗 - {ex.Message}");
                }
                
                results.AppendLine();
                results.AppendLine("5. NHost 配置摘要");
                results.AppendLine("   Region: eu-central-1");
                results.AppendLine("   Subdomain: uxgwdiuehabbzenwtcqo");
                results.AppendLine("   GraphQL 端點: https://uxgwdiuehabbzenwtcqo.graphql.eu-central-1.nhost.run/v1");
                results.AppendLine("   認證端點: https://uxgwdiuehabbzenwtcqo.auth.eu-central-1.nhost.run/v1");
                results.AppendLine();
                
                results.AppendLine("整合測試完成！");
                
                MessageBox.Show(results.ToString(), "NHost 整合測試結果", 
                    MessageBoxButton.OK, MessageBoxImage.Information);
                
            }
            catch (Exception ex)
            {
                MessageBox.Show($"整合測試過程中發生錯誤：\n{ex.Message}", "錯誤", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}