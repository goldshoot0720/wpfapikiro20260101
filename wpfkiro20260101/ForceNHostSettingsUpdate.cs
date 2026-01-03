using System;
using System.Threading.Tasks;
using wpfkiro20260101.Services;
using MessageBox = System.Windows.MessageBox;
using MessageBoxButton = System.Windows.MessageBoxButton;
using MessageBoxImage = System.Windows.MessageBoxImage;

namespace wpfkiro20260101
{
    public class ForceNHostSettingsUpdate
    {
        public static async Task RunUpdate()
        {
            try
            {
                var results = new System.Text.StringBuilder();
                results.AppendLine("強制更新 NHost 連線設定");
                results.AppendLine("===================");
                results.AppendLine();
                
                // 獲取當前設定
                var settings = AppSettings.Instance;
                results.AppendLine("📋 當前設定狀態:");
                results.AppendLine($"   當前後端服務: {settings.BackendService}");
                results.AppendLine($"   NHost API URL: {settings.NHost.ApiUrl}");
                results.AppendLine($"   NHost Project ID: {settings.NHost.ProjectId}");
                results.AppendLine();
                
                // 檢查是否需要更新
                bool needsUpdate = false;
                if (settings.NHost.ApiUrl != "https://uxgwdiuehabbzenwtcqo.hasura.eu-central-1.nhost.run/v1/graphql")
                {
                    results.AppendLine("❌ NHost API URL 需要更新");
                    needsUpdate = true;
                }
                else
                {
                    results.AppendLine("✅ NHost API URL 正確");
                }
                
                if (settings.NHost.ProjectId != "cu#34&yjF3Cr%fgxB#WA,4r4^c=Igcwr")
                {
                    results.AppendLine("❌ NHost Project ID 需要更新");
                    needsUpdate = true;
                }
                else
                {
                    results.AppendLine("✅ NHost Project ID 正確");
                }
                results.AppendLine();
                
                // 強制更新設定
                if (needsUpdate)
                {
                    results.AppendLine("🔧 執行強制更新:");
                    
                    // 更新 NHost 設定
                    settings.NHost.ApiUrl = "https://uxgwdiuehabbzenwtcqo.hasura.eu-central-1.nhost.run/v1/graphql";
                    settings.NHost.ProjectId = "cu#34&yjF3Cr%fgxB#WA,4r4^c=Igcwr";
                    settings.NHost.ApiKey = "cu#34&yjF3Cr%fgxB#WA,4r4^c=Igcwr";
                    
                    results.AppendLine("   ✅ NHost 設定已更新");
                    
                    // 儲存設定
                    settings.Save();
                    results.AppendLine("   ✅ 設定已儲存");
                    
                    // 重新載入設定
                    AppSettings.ReloadSettings();
                    results.AppendLine("   ✅ 設定已重新載入");
                    results.AppendLine();
                    
                    // 驗證更新結果
                    var updatedSettings = AppSettings.Instance;
                    results.AppendLine("🔍 更新後驗證:");
                    results.AppendLine($"   NHost API URL: {updatedSettings.NHost.ApiUrl}");
                    results.AppendLine($"   NHost Project ID: {updatedSettings.NHost.ProjectId}");
                    
                    bool updateSuccess = 
                        updatedSettings.NHost.ApiUrl == "https://uxgwdiuehabbzenwtcqo.hasura.eu-central-1.nhost.run/v1/graphql" &&
                        updatedSettings.NHost.ProjectId == "cu#34&yjF3Cr%fgxB#WA,4r4^c=Igcwr";
                    
                    if (updateSuccess)
                    {
                        results.AppendLine("   🎉 更新成功！");
                    }
                    else
                    {
                        results.AppendLine("   ❌ 更新失敗，請檢查設定");
                    }
                }
                else
                {
                    results.AppendLine("✅ 設定已經正確，無需更新");
                }
                results.AppendLine();
                
                // 測試 NHost 服務
                results.AppendLine("🚀 測試 NHost 服務:");
                try
                {
                    var nhostService = new NHostService();
                    results.AppendLine($"   ✅ NHost 服務創建成功");
                    
                    var initResult = await nhostService.InitializeAsync();
                    results.AppendLine($"   初始化結果: {(initResult ? "✅ 成功" : "❌ 失敗")}");
                }
                catch (Exception ex)
                {
                    results.AppendLine($"   ❌ NHost 服務測試失敗: {ex.Message}");
                }
                results.AppendLine();
                
                // 使用說明
                results.AppendLine("📖 使用說明:");
                results.AppendLine("============");
                results.AppendLine("1. 重新開啟系統設定頁面");
                results.AppendLine("2. 選擇 NHost 作為後端服務");
                results.AppendLine("3. 應該會看到正確的連線設定:");
                results.AppendLine("   • NHOST_GRAPHQL_URL: https://uxgwdiuehabbzenwtcqo.hasura.eu-central-1.nhost.run/v1/graphql");
                results.AppendLine("   • NHOST_ADMIN_SECRET: cu#34&yjF3Cr%fgxB#WA,4r4^c=Igcwr");
                
                MessageBox.Show(results.ToString(), "NHost 設定強制更新結果", 
                    MessageBoxButton.OK, MessageBoxImage.Information);
                
            }
            catch (Exception ex)
            {
                MessageBox.Show($"強制更新過程中發生錯誤：\n{ex.Message}", "錯誤", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// 快速修正 NHost 設定
        /// </summary>
        public static void QuickFix()
        {
            try
            {
                var settings = AppSettings.Instance;
                
                // 強制設定正確的值
                settings.NHost.ApiUrl = "https://uxgwdiuehabbzenwtcqo.hasura.eu-central-1.nhost.run/v1/graphql";
                settings.NHost.ProjectId = "cu#34&yjF3Cr%fgxB#WA,4r4^c=Igcwr";
                settings.NHost.ApiKey = "cu#34&yjF3Cr%fgxB#WA,4r4^c=Igcwr";
                
                // 儲存設定
                settings.Save();
                
                // 重新載入
                AppSettings.ReloadSettings();
                
                MessageBox.Show("NHost 設定已快速修正！\n\n請重新開啟系統設定頁面查看結果。", 
                    "快速修正完成", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"快速修正失敗：\n{ex.Message}", "錯誤", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}