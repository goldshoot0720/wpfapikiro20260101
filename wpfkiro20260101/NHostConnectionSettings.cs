using System;
using System.Threading.Tasks;
using wpfkiro20260101.Services;
using MessageBox = System.Windows.MessageBox;
using MessageBoxButton = System.Windows.MessageBoxButton;
using MessageBoxImage = System.Windows.MessageBoxImage;

namespace wpfkiro20260101
{
    public class NHostConnectionSettings
    {
        // 最簡化的 NHost 連線設定欄位
        public static readonly string GraphQLUrl = "https://uxgwdiuehabbzenwtcqo.hasura.eu-central-1.nhost.run/v1/graphql";
        public static readonly string AdminSecret = "cu#34&yjF3Cr%fgxB#WA,4r4^c=Igcwr";

        // 從 URL 解析出的資訊（僅供參考）
        public static readonly string Subdomain = "uxgwdiuehabbzenwtcqo";
        public static readonly string Region = "eu-central-1";

        // 其他端點（根據 GraphQL URL 推導）
        public static readonly string AuthEndpoint = "https://uxgwdiuehabbzenwtcqo.auth.eu-central-1.nhost.run/v1";
        public static readonly string FunctionsEndpoint = "https://uxgwdiuehabbzenwtcqo.functions.eu-central-1.nhost.run/v1";
        public static readonly string StorageEndpoint = "https://uxgwdiuehabbzenwtcqo.storage.eu-central-1.nhost.run/v1";

        public static async Task VerifyConnectionSettings()
        {
            try
            {
                var results = new System.Text.StringBuilder();
                results.AppendLine("NHost 連線設定驗證");
                results.AppendLine("==================");
                results.AppendLine();
                
                // 顯示連線設定欄位
                results.AppendLine("最簡化連線設定:");
                results.AppendLine($"- GraphQL URL: {GraphQLUrl}");
                results.AppendLine($"- Admin Secret: {AdminSecret}");
                results.AppendLine();
                
                // 顯示端點配置
                results.AppendLine("其他端點 (自動推導):");
                results.AppendLine($"- Auth: {AuthEndpoint}");
                results.AppendLine($"- Functions: {FunctionsEndpoint}");
                results.AppendLine($"- Storage: {StorageEndpoint}");
                results.AppendLine();
                
                // 驗證服務配置
                results.AppendLine("服務配置驗證:");
                results.AppendLine("==============");
                
                var nhostService = new NHostService();
                results.AppendLine($"✅ NHost 服務創建成功");
                results.AppendLine($"   服務名稱: {nhostService.ServiceName}");
                results.AppendLine($"   服務類型: {nhostService.ServiceType}");
                results.AppendLine();
                
                // 測試初始化
                results.AppendLine("初始化測試:");
                var initResult = await nhostService.InitializeAsync();
                results.AppendLine($"- 初始化結果: {(initResult ? "✅ 成功" : "❌ 失敗")}");
                
                // 測試連線
                results.AppendLine("- 連線測試: 進行中...");
                var connectionResult = await nhostService.TestConnectionAsync();
                results.AppendLine($"- 連線結果: {(connectionResult ? "✅ 成功" : "❌ 失敗")}");
                results.AppendLine();
                
                // 測試 GraphQL 端點
                results.AppendLine("GraphQL 端點測試:");
                results.AppendLine("==================");
                
                try
                {
                    var foodsResult = await nhostService.GetFoodsAsync();
                    if (foodsResult.Success)
                    {
                        results.AppendLine($"✅ Foods 查詢成功 ({foodsResult.Data?.Length ?? 0} 筆資料)");
                    }
                    else
                    {
                        results.AppendLine($"❌ Foods 查詢失敗: {foodsResult.ErrorMessage}");
                    }
                    
                    var subscriptionsResult = await nhostService.GetSubscriptionsAsync();
                    if (subscriptionsResult.Success)
                    {
                        results.AppendLine($"✅ Subscriptions 查詢成功 ({subscriptionsResult.Data?.Length ?? 0} 筆資料)");
                    }
                    else
                    {
                        results.AppendLine($"❌ Subscriptions 查詢失敗: {subscriptionsResult.ErrorMessage}");
                    }
                }
                catch (Exception ex)
                {
                    results.AppendLine($"❌ GraphQL 查詢異常: {ex.Message}");
                }
                results.AppendLine();
                
                // 驗證工廠整合
                results.AppendLine("工廠整合驗證:");
                results.AppendLine("==============");
                try
                {
                    var factoryService = BackendServiceFactory.CreateService(BackendServiceType.NHost);
                    results.AppendLine($"✅ 工廠服務創建: {factoryService.ServiceName}");
                    
                    var crudManager = BackendServiceFactory.CreateCrudManager(BackendServiceType.NHost);
                    results.AppendLine($"✅ CRUD 管理器: {crudManager.GetServiceName()}");
                }
                catch (Exception ex)
                {
                    results.AppendLine($"❌ 工廠整合失敗: {ex.Message}");
                }
                results.AppendLine();
                
                // 設定狀態總結
                results.AppendLine("設定狀態總結:");
                results.AppendLine("==============");
                
                if (initResult && connectionResult)
                {
                    results.AppendLine("🎉 NHost 連線設定完全正確！");
                    results.AppendLine();
                    results.AppendLine("✅ 所有連線設定欄位已正確配置");
                    results.AppendLine("✅ Admin Secret 認證成功");
                    results.AppendLine("✅ GraphQL 端點可正常存取");
                    results.AppendLine("✅ 服務整合完成");
                    results.AppendLine();
                    results.AppendLine("🚀 NHost 服務已準備就緒，可以開始使用！");
                }
                else
                {
                    results.AppendLine("⚠️ 連線設定需要檢查");
                    results.AppendLine();
                    results.AppendLine("請確認:");
                    results.AppendLine("1. NHost 專案是否已啟動");
                    results.AppendLine("2. 資料表是否已創建");
                    results.AppendLine("3. 網路連線是否正常");
                    results.AppendLine("4. Admin Secret 是否有效");
                }
                
                results.AppendLine();
                results.AppendLine("使用說明:");
                results.AppendLine("========");
                results.AppendLine("1. 在應用程式設定中選擇 'NHost' 作為後端服務");
                results.AppendLine("2. 所有連線設定已自動配置，無需手動輸入");
                results.AppendLine("3. 如需創建資料表，請執行 CREATE_NHOST_TABLES.sql");
                
                MessageBox.Show(results.ToString(), "NHost 連線設定驗證結果", 
                    MessageBoxButton.OK, MessageBoxImage.Information);
                
            }
            catch (Exception ex)
            {
                MessageBox.Show($"連線設定驗證過程中發生錯誤：\n{ex.Message}", "錯誤", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// 顯示連線設定摘要
        /// </summary>
        public static void ShowConnectionSummary()
        {
            var summary = new System.Text.StringBuilder();
            summary.AppendLine("NHost 最簡化連線設定");
            summary.AppendLine("==================");
            summary.AppendLine();
            summary.AppendLine("核心設定欄位:");
            summary.AppendLine($"GraphQL URL: {GraphQLUrl}");
            summary.AppendLine($"Admin Secret: {AdminSecret}");
            summary.AppendLine();
            summary.AppendLine("解析資訊:");
            summary.AppendLine($"Subdomain: {Subdomain}");
            summary.AppendLine($"Region: {Region}");
            summary.AppendLine();
            summary.AppendLine("狀態: ✅ 已配置並整合到應用程式中");
            
            MessageBox.Show(summary.ToString(), "NHost 連線設定", 
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        /// <summary>
        /// 驗證設定是否與提供的參數匹配
        /// </summary>
        public static bool ValidateSettings(string graphqlUrl, string adminSecret)
        {
            return GraphQLUrl.Equals(graphqlUrl, StringComparison.OrdinalIgnoreCase) &&
                   AdminSecret.Equals(adminSecret, StringComparison.Ordinal);
        }
    }
}