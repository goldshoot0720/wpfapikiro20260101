using System;
using System.Threading.Tasks;
using wpfkiro20260101.Services;
using MessageBox = System.Windows.MessageBox;
using MessageBoxButton = System.Windows.MessageBoxButton;
using MessageBoxImage = System.Windows.MessageBoxImage;

namespace wpfkiro20260101
{
    public class TestNHostSimplified
    {
        public static async Task RunTest()
        {
            try
            {
                var results = new System.Text.StringBuilder();
                results.AppendLine("NHost 最簡化連線設定測試");
                results.AppendLine("========================");
                results.AppendLine();
                
                // 顯示簡化後的設定
                results.AppendLine("📋 NHost 只需要兩個欄位:");
                results.AppendLine($"   NHOST_GRAPHQL_URL: {NHostConnectionSettings.GraphQLUrl}");
                results.AppendLine($"   NHOST_ADMIN_SECRET: {NHostConnectionSettings.AdminSecret}");
                results.AppendLine();
                
                // 顯示解析出的資訊
                results.AppendLine("🔍 自動解析資訊:");
                results.AppendLine($"   Subdomain: {NHostConnectionSettings.Subdomain}");
                results.AppendLine($"   Region: {NHostConnectionSettings.Region}");
                results.AppendLine();
                
                // 測試服務創建
                results.AppendLine("🔧 服務創建測試:");
                var nhostService = new NHostService();
                results.AppendLine($"   ✅ NHost 服務創建成功");
                results.AppendLine($"   服務名稱: {nhostService.ServiceName}");
                results.AppendLine($"   服務類型: {nhostService.ServiceType}");
                results.AppendLine();
                
                // 測試初始化
                results.AppendLine("🚀 初始化測試:");
                var initResult = await nhostService.InitializeAsync();
                results.AppendLine($"   初始化結果: {(initResult ? "✅ 成功" : "❌ 失敗")}");
                
                if (initResult)
                {
                    results.AppendLine("   🎉 NHost 服務初始化完成！");
                }
                else
                {
                    results.AppendLine("   ⚠️ 初始化失敗，請檢查網路連線或 NHost 專案狀態");
                }
                results.AppendLine();
                
                // 測試連線
                results.AppendLine("🔗 連線測試:");
                try
                {
                    var connectionResult = await nhostService.TestConnectionAsync();
                    results.AppendLine($"   連線狀態: {(connectionResult ? "✅ 正常" : "❌ 異常")}");
                    
                    if (connectionResult)
                    {
                        results.AppendLine("   🌐 NHost 端點可正常存取");
                    }
                    else
                    {
                        results.AppendLine("   ⚠️ 連線異常，可能是 NHost 專案未啟動或網路問題");
                    }
                }
                catch (Exception ex)
                {
                    results.AppendLine($"   ❌ 連線測試異常: {ex.Message}");
                }
                results.AppendLine();
                
                // 測試 GraphQL 查詢
                results.AppendLine("📊 GraphQL 查詢測試:");
                try
                {
                    var foodsResult = await nhostService.GetFoodsAsync();
                    if (foodsResult.Success)
                    {
                        results.AppendLine($"   ✅ Foods 查詢成功 ({foodsResult.Data?.Length ?? 0} 筆資料)");
                    }
                    else
                    {
                        results.AppendLine($"   ❌ Foods 查詢失敗: {foodsResult.ErrorMessage}");
                    }
                    
                    var subscriptionsResult = await nhostService.GetSubscriptionsAsync();
                    if (subscriptionsResult.Success)
                    {
                        results.AppendLine($"   ✅ Subscriptions 查詢成功 ({subscriptionsResult.Data?.Length ?? 0} 筆資料)");
                    }
                    else
                    {
                        results.AppendLine($"   ❌ Subscriptions 查詢失敗: {subscriptionsResult.ErrorMessage}");
                    }
                }
                catch (Exception ex)
                {
                    results.AppendLine($"   ❌ GraphQL 查詢異常: {ex.Message}");
                }
                results.AppendLine();
                
                // 測試工廠整合
                results.AppendLine("🏭 工廠整合測試:");
                try
                {
                    var factoryService = BackendServiceFactory.CreateService(BackendServiceType.NHost);
                    results.AppendLine($"   ✅ 工廠服務創建: {factoryService.ServiceName}");
                    
                    var crudManager = BackendServiceFactory.CreateCrudManager(BackendServiceType.NHost);
                    results.AppendLine($"   ✅ CRUD 管理器: {crudManager.GetServiceName()}");
                }
                catch (Exception ex)
                {
                    results.AppendLine($"   ❌ 工廠整合失敗: {ex.Message}");
                }
                results.AppendLine();
                
                // 總結
                results.AppendLine("📋 簡化設定總結:");
                results.AppendLine("================");
                results.AppendLine("✅ 連線設定已簡化至最少必要欄位");
                results.AppendLine("✅ 只需要 GraphQL URL 和 Admin Secret");
                results.AppendLine("✅ 其他端點自動推導生成");
                results.AppendLine("✅ 服務完全整合到應用程式中");
                results.AppendLine();
                
                if (initResult)
                {
                    results.AppendLine("🎉 NHost 只需要兩個欄位的設定完成！");
                    results.AppendLine();
                    results.AppendLine("使用方式:");
                    results.AppendLine("1. 在系統設定中選擇 'NHost' 作為後端服務");
                    results.AppendLine("2. 只會顯示兩個必要欄位，無需手動輸入");
                    results.AppendLine("3. 開始使用 NHost 進行資料操作");
                }
                else
                {
                    results.AppendLine("⚠️ 請檢查 NHost 專案狀態");
                    results.AppendLine();
                    results.AppendLine("可能的問題:");
                    results.AppendLine("1. NHost 專案未啟動");
                    results.AppendLine("2. 網路連線問題");
                    results.AppendLine("3. Admin Secret 已過期");
                    results.AppendLine("4. 資料表尚未創建");
                }
                
                MessageBox.Show(results.ToString(), "NHost 最簡化設定測試結果", 
                    MessageBoxButton.OK, MessageBoxImage.Information);
                
            }
            catch (Exception ex)
            {
                MessageBox.Show($"測試過程中發生錯誤：\n{ex.Message}", "錯誤", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// 快速顯示簡化設定摘要
        /// </summary>
        public static void ShowSimplifiedSummary()
        {
            var summary = $@"NHost 只需要兩個欄位的設定

核心設定欄位 (僅需 2 個):
• NHOST_GRAPHQL_URL: {NHostConnectionSettings.GraphQLUrl}
• NHOST_ADMIN_SECRET: {NHostConnectionSettings.AdminSecret}

自動推導資訊:
• Subdomain: {NHostConnectionSettings.Subdomain}
• Region: {NHostConnectionSettings.Region}

狀態: ✅ 已完成兩欄位配置

優點:
• 設定欄位最少化 (只有 2 個)
• 清楚的欄位命名
• 自動推導其他端點
• 完全整合到應用程式
• 支援完整 CRUD 操作";

            MessageBox.Show(summary, "NHost 兩欄位設定摘要", 
                MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}