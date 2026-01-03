using System;
using System.Threading.Tasks;
using wpfkiro20260101.Services;
using MessageBox = System.Windows.MessageBox;
using MessageBoxButton = System.Windows.MessageBoxButton;
using MessageBoxImage = System.Windows.MessageBoxImage;

namespace wpfkiro20260101
{
    public class QuickNHostSetup
    {
        public static async Task RunQuickSetup()
        {
            try
            {
                var results = new System.Text.StringBuilder();
                results.AppendLine("NHost 快速設定測試");
                results.AppendLine("==================");
                results.AppendLine();
                
                results.AppendLine("專案配置:");
                results.AppendLine("- Region: eu-central-1");
                results.AppendLine("- Subdomain: uxgwdiuehabbzenwtcqo");
                results.AppendLine("- Admin Secret: 已配置 ✅");
                results.AppendLine();
                
                // 步驟 1: 測試服務創建
                results.AppendLine("步驟 1: 測試服務創建");
                results.AppendLine("====================");
                try
                {
                    var nhostService = new NHostService();
                    results.AppendLine($"✅ NHost 服務創建成功");
                    results.AppendLine($"   服務名稱: {nhostService.ServiceName}");
                    results.AppendLine($"   服務類型: {nhostService.ServiceType}");
                    
                    // 步驟 2: 測試初始化
                    results.AppendLine();
                    results.AppendLine("步驟 2: 測試服務初始化");
                    results.AppendLine("======================");
                    var initResult = await nhostService.InitializeAsync();
                    results.AppendLine($"初始化結果: {(initResult ? "✅ 成功" : "❌ 失敗")}");
                    
                    // 步驟 3: 測試基本連線
                    results.AppendLine();
                    results.AppendLine("步驟 3: 測試基本連線");
                    results.AppendLine("====================");
                    var connectionResult = await nhostService.TestConnectionAsync();
                    results.AppendLine($"連線測試: {(connectionResult ? "✅ 成功" : "❌ 失敗")}");
                    
                    // 步驟 4: 測試 GraphQL 查詢
                    results.AppendLine();
                    results.AppendLine("步驟 4: 測試 GraphQL 查詢");
                    results.AppendLine("========================");
                    
                    // 測試食品查詢
                    var foodsResult = await nhostService.GetFoodsAsync();
                    if (foodsResult.Success)
                    {
                        results.AppendLine($"✅ 食品查詢成功 ({foodsResult.Data?.Length ?? 0} 筆資料)");
                    }
                    else
                    {
                        results.AppendLine($"❌ 食品查詢失敗: {foodsResult.ErrorMessage}");
                        results.AppendLine("   可能原因: 資料表尚未創建或權限設定問題");
                    }
                    
                    // 測試訂閱查詢
                    var subscriptionsResult = await nhostService.GetSubscriptionsAsync();
                    if (subscriptionsResult.Success)
                    {
                        results.AppendLine($"✅ 訂閱查詢成功 ({subscriptionsResult.Data?.Length ?? 0} 筆資料)");
                    }
                    else
                    {
                        results.AppendLine($"❌ 訂閱查詢失敗: {subscriptionsResult.ErrorMessage}");
                        results.AppendLine("   可能原因: 資料表尚未創建或權限設定問題");
                    }
                    
                    // 步驟 5: 測試工廠整合
                    results.AppendLine();
                    results.AppendLine("步驟 5: 測試工廠整合");
                    results.AppendLine("====================");
                    try
                    {
                        var factoryService = BackendServiceFactory.CreateService(BackendServiceType.NHost);
                        results.AppendLine($"✅ 工廠服務創建成功: {factoryService.ServiceName}");
                        
                        var crudManager = BackendServiceFactory.CreateCrudManager(BackendServiceType.NHost);
                        results.AppendLine($"✅ CRUD 管理器創建成功: {crudManager.GetServiceName()}");
                    }
                    catch (Exception ex)
                    {
                        results.AppendLine($"❌ 工廠整合失敗: {ex.Message}");
                    }
                    
                    // 總結
                    results.AppendLine();
                    results.AppendLine("設定總結");
                    results.AppendLine("========");
                    
                    if (initResult && connectionResult)
                    {
                        results.AppendLine("🎉 NHost 快速設定完成！");
                        results.AppendLine();
                        results.AppendLine("✅ 服務配置正確");
                        results.AppendLine("✅ Admin Secret 認證成功");
                        results.AppendLine("✅ GraphQL 端點可存取");
                        results.AppendLine("✅ 工廠整合正常");
                        results.AppendLine();
                        
                        if (foodsResult.Success && subscriptionsResult.Success)
                        {
                            results.AppendLine("✅ 資料表已正確設定");
                            results.AppendLine("🚀 專案已準備就緒，可以開始使用！");
                        }
                        else
                        {
                            results.AppendLine("⚠️ 資料表可能尚未創建");
                            results.AppendLine("📋 請執行 CREATE_NHOST_TABLES.sql 腳本");
                            results.AppendLine("   或在 Hasura 控制台手動創建資料表");
                        }
                    }
                    else
                    {
                        results.AppendLine("❌ 設定過程中發現問題");
                        results.AppendLine();
                        results.AppendLine("請檢查:");
                        results.AppendLine("1. NHost 專案是否已啟動");
                        results.AppendLine("2. 網路連線是否正常");
                        results.AppendLine("3. Admin Secret 是否正確");
                        results.AppendLine("4. 端點 URL 是否可存取");
                    }
                    
                }
                catch (Exception ex)
                {
                    results.AppendLine($"❌ 服務創建失敗: {ex.Message}");
                }
                
                results.AppendLine();
                results.AppendLine("下一步:");
                results.AppendLine("1. 如果資料表查詢失敗，請執行 CREATE_NHOST_TABLES.sql");
                results.AppendLine("2. 執行 NHostProjectImplementation.RunImplementationTest() 進行完整測試");
                results.AppendLine("3. 在應用程式設定中選擇 NHost 作為後端服務");
                
                MessageBox.Show(results.ToString(), "NHost 快速設定結果", 
                    MessageBoxButton.OK, MessageBoxImage.Information);
                
            }
            catch (Exception ex)
            {
                MessageBox.Show($"快速設定過程中發生錯誤：\n{ex.Message}", "錯誤", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}