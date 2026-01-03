using System;
using System.Threading.Tasks;
using wpfkiro20260101.Services;
using MessageBox = System.Windows.MessageBox;
using MessageBoxButton = System.Windows.MessageBoxButton;
using MessageBoxImage = System.Windows.MessageBoxImage;

namespace wpfkiro20260101
{
    public class TestNHostSettingsDisplay
    {
        public static async Task RunTest()
        {
            try
            {
                var results = new System.Text.StringBuilder();
                results.AppendLine("NHost 連線設定顯示測試");
                results.AppendLine("====================");
                results.AppendLine();
                
                // 測試 AppSettings 中的 NHost 預設值
                results.AppendLine("📋 AppSettings 預設值測試:");
                results.AppendLine($"   NHOST_GRAPHQL_URL: {AppSettings.Defaults.NHost.ApiUrl}");
                results.AppendLine($"   NHOST_ADMIN_SECRET: {AppSettings.Defaults.NHost.ProjectId}");
                results.AppendLine();
                
                // 測試 NHostSettings 實例
                results.AppendLine("🔧 NHostSettings 實例測試:");
                var nhostSettings = new NHostSettings();
                results.AppendLine($"   NHOST_GRAPHQL_URL: {nhostSettings.ApiUrl}");
                results.AppendLine($"   NHOST_ADMIN_SECRET: {nhostSettings.ProjectId}");
                results.AppendLine();
                
                // 測試 AppSettings 實例中的 NHost 設定
                results.AppendLine("⚙️ AppSettings 實例中的 NHost 設定:");
                var appSettings = AppSettings.Instance;
                results.AppendLine($"   NHOST_GRAPHQL_URL: {appSettings.NHost.ApiUrl}");
                results.AppendLine($"   NHOST_ADMIN_SECRET: {appSettings.NHost.ProjectId}");
                results.AppendLine();
                
                // 測試當選擇 NHost 時的設定
                results.AppendLine("🎯 選擇 NHost 時的設定測試:");
                var originalService = appSettings.BackendService;
                appSettings.BackendService = BackendServiceType.NHost;
                
                var currentSettings = appSettings.GetCurrentServiceSettings();
                results.AppendLine($"   當前服務: {appSettings.BackendService}");
                results.AppendLine($"   NHOST_GRAPHQL_URL: {currentSettings.ApiUrl}");
                results.AppendLine($"   NHOST_ADMIN_SECRET: {currentSettings.ProjectId}");
                results.AppendLine();
                
                // 恢復原始設定
                appSettings.BackendService = originalService;
                
                // 測試預設值方法
                results.AppendLine("🔍 預設值方法測試:");
                appSettings.BackendService = BackendServiceType.NHost;
                results.AppendLine($"   GetDefaultApiUrl(): {appSettings.GetDefaultApiUrl()}");
                results.AppendLine($"   GetDefaultProjectId(): {appSettings.GetDefaultProjectId()}");
                results.AppendLine($"   GetDefaultApiKey(): {appSettings.GetDefaultApiKey()}");
                results.AppendLine();
                
                // 恢復原始設定
                appSettings.BackendService = originalService;
                
                // 驗證設定正確性
                results.AppendLine("✅ 設定驗證:");
                bool isGraphQLUrlCorrect = AppSettings.Defaults.NHost.ApiUrl == "https://uxgwdiuehabbzenwtcqo.hasura.eu-central-1.nhost.run/v1/graphql";
                bool isAdminSecretCorrect = AppSettings.Defaults.NHost.ProjectId == "cu#34&yjF3Cr%fgxB#WA,4r4^c=Igcwr";
                
                results.AppendLine($"   NHOST_GRAPHQL_URL 正確: {(isGraphQLUrlCorrect ? "✅" : "❌")}");
                results.AppendLine($"   NHOST_ADMIN_SECRET 正確: {(isAdminSecretCorrect ? "✅" : "❌")}");
                results.AppendLine();
                
                // 測試 NHost 服務創建
                results.AppendLine("🚀 NHost 服務創建測試:");
                try
                {
                    var nhostService = new NHostService();
                    results.AppendLine($"   ✅ NHost 服務創建成功");
                    results.AppendLine($"   服務名稱: {nhostService.ServiceName}");
                    results.AppendLine($"   服務類型: {nhostService.ServiceType}");
                    
                    // 測試初始化
                    var initResult = await nhostService.InitializeAsync();
                    results.AppendLine($"   初始化結果: {(initResult ? "✅ 成功" : "❌ 失敗")}");
                }
                catch (Exception ex)
                {
                    results.AppendLine($"   ❌ NHost 服務創建失敗: {ex.Message}");
                }
                results.AppendLine();
                
                // 總結
                results.AppendLine("📊 測試總結:");
                results.AppendLine("============");
                
                if (isGraphQLUrlCorrect && isAdminSecretCorrect)
                {
                    results.AppendLine("🎉 所有 NHost 設定都正確！");
                    results.AppendLine();
                    results.AppendLine("在系統設定頁面選擇 NHost 時應該顯示:");
                    results.AppendLine("• NHOST_GRAPHQL_URL: https://uxgwdiuehabbzenwtcqo.hasura.eu-central-1.nhost.run/v1/graphql");
                    results.AppendLine("• NHOST_ADMIN_SECRET: cu#34&yjF3Cr%fgxB#WA,4r4^c=Igcwr");
                    results.AppendLine();
                    results.AppendLine("欄位標籤應該顯示為:");
                    results.AppendLine("• NHOST_GRAPHQL_URL: (第一個欄位)");
                    results.AppendLine("• NHOST_ADMIN_SECRET: (第二個欄位)");
                    results.AppendLine("• API Key 欄位應該被隱藏");
                }
                else
                {
                    results.AppendLine("⚠️ 發現設定問題:");
                    if (!isGraphQLUrlCorrect)
                        results.AppendLine("- NHOST_GRAPHQL_URL 不正確");
                    if (!isAdminSecretCorrect)
                        results.AppendLine("- NHOST_ADMIN_SECRET 不正確");
                }
                
                MessageBox.Show(results.ToString(), "NHost 連線設定顯示測試結果", 
                    MessageBoxButton.OK, MessageBoxImage.Information);
                
            }
            catch (Exception ex)
            {
                MessageBox.Show($"測試過程中發生錯誤：\n{ex.Message}", "錯誤", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// 快速檢查 NHost 設定是否正確配置
        /// </summary>
        public static void QuickCheck()
        {
            var summary = $@"NHost 兩欄位設定快速檢查

預設值檢查:
• NHOST_GRAPHQL_URL: {AppSettings.Defaults.NHost.ApiUrl}
• NHOST_ADMIN_SECRET: {AppSettings.Defaults.NHost.ProjectId}

設定狀態: {(
    AppSettings.Defaults.NHost.ApiUrl == "https://uxgwdiuehabbzenwtcqo.hasura.eu-central-1.nhost.run/v1/graphql" &&
    AppSettings.Defaults.NHost.ProjectId == "cu#34&yjF3Cr%fgxB#WA,4r4^c=Igcwr"
    ? "✅ 正確" : "❌ 需要修正")}

在系統設定中選擇 NHost 時，應該只會顯示上述兩個欄位。";

            MessageBox.Show(summary, "NHost 兩欄位設定快速檢查", 
                MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}