using System;
using System.Threading.Tasks;
using wpfkiro20260101.Services;
using MessageBox = System.Windows.MessageBox;
using MessageBoxButton = System.Windows.MessageBoxButton;
using MessageBoxImage = System.Windows.MessageBoxImage;

namespace wpfkiro20260101
{
    public class TestNHostQuick
    {
        public static async Task RunQuickTest()
        {
            try
            {
                var results = new System.Text.StringBuilder();
                results.AppendLine("NHost 快速連線測試");
                results.AppendLine("================");
                
                // 創建 NHost 服務實例
                var nhostService = new NHostService();
                
                results.AppendLine($"服務名稱: {nhostService.ServiceName}");
                results.AppendLine($"服務類型: {nhostService.ServiceType}");
                results.AppendLine();
                
                // 測試初始化
                results.AppendLine("正在測試初始化...");
                var initResult = await nhostService.InitializeAsync();
                results.AppendLine($"初始化結果: {(initResult ? "✅ 成功" : "❌ 失敗")}");
                results.AppendLine();
                
                // 測試連線
                results.AppendLine("正在測試連線...");
                var connectionResult = await nhostService.TestConnectionAsync();
                results.AppendLine($"連線測試: {(connectionResult ? "✅ 成功" : "❌ 失敗")}");
                results.AppendLine();
                
                // 顯示端點信息
                results.AppendLine("NHost 端點配置:");
                results.AppendLine("- Region: eu-central-1");
                results.AppendLine("- Subdomain: uxgwdiuehabbzenwtcqo");
                results.AppendLine("- GraphQL: https://uxgwdiuehabbzenwtcqo.graphql.eu-central-1.nhost.run/v1");
                results.AppendLine("- Auth: https://uxgwdiuehabbzenwtcqo.auth.eu-central-1.nhost.run/v1");
                results.AppendLine();
                
                if (initResult && connectionResult)
                {
                    results.AppendLine("🎉 NHost 服務配置成功！");
                    results.AppendLine("可以開始使用 NHost 作為後端服務。");
                }
                else
                {
                    results.AppendLine("⚠️ NHost 連線有問題，請檢查:");
                    results.AppendLine("1. 網路連線是否正常");
                    results.AppendLine("2. NHost 專案是否已啟動");
                    results.AppendLine("3. Region 和 Subdomain 是否正確");
                }
                
                MessageBox.Show(results.ToString(), "NHost 快速測試結果", 
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