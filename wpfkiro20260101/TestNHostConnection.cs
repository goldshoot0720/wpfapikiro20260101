using System;
using System.Threading.Tasks;
using System.Windows;
using wpfkiro20260101.Services;
using MessageBox = System.Windows.MessageBox;

namespace wpfkiro20260101
{
    public class TestNHostConnection
    {
        public static async Task RunTest()
        {
            try
            {
                MessageBox.Show("開始測試 NHost 連線...", "測試", MessageBoxButton.OK, MessageBoxImage.Information);

                var nhostService = new NHostService();
                var results = new System.Text.StringBuilder();
                
                results.AppendLine("NHost 連線測試結果：");
                results.AppendLine("==================");
                results.AppendLine($"服務名稱: {nhostService.ServiceName}");
                results.AppendLine($"服務類型: {nhostService.ServiceType}");
                results.AppendLine($"Region: eu-central-1");
                results.AppendLine($"Subdomain: uxgwdiuehabbzenwtcqo");
                results.AppendLine();

                // 測試初始化
                results.AppendLine("1. 測試服務初始化...");
                var initResult = await nhostService.InitializeAsync();
                results.AppendLine($"   初始化結果: {(initResult ? "成功" : "失敗")}");
                results.AppendLine();

                // 測試連線
                results.AppendLine("2. 測試連線...");
                var connectionResult = await nhostService.TestConnectionAsync();
                results.AppendLine($"   連線結果: {(connectionResult ? "成功" : "失敗")}");
                results.AppendLine();

                // 測試 GraphQL 端點
                results.AppendLine("3. 測試 GraphQL 端點...");
                try
                {
                    var foodsResult = await nhostService.GetFoodsAsync();
                    results.AppendLine($"   獲取食品資料: {(foodsResult.Success ? "成功" : "失敗")}");
                    if (!foodsResult.Success)
                    {
                        results.AppendLine($"   錯誤訊息: {foodsResult.ErrorMessage}");
                    }
                }
                catch (Exception ex)
                {
                    results.AppendLine($"   獲取食品資料: 失敗 - {ex.Message}");
                }
                results.AppendLine();

                // 測試訂閱資料
                results.AppendLine("4. 測試訂閱資料...");
                try
                {
                    var subscriptionsResult = await nhostService.GetSubscriptionsAsync();
                    results.AppendLine($"   獲取訂閱資料: {(subscriptionsResult.Success ? "成功" : "失敗")}");
                    if (!subscriptionsResult.Success)
                    {
                        results.AppendLine($"   錯誤訊息: {subscriptionsResult.ErrorMessage}");
                    }
                }
                catch (Exception ex)
                {
                    results.AppendLine($"   獲取訂閱資料: 失敗 - {ex.Message}");
                }
                results.AppendLine();

                // 測試 URL 構建
                results.AppendLine("5. NHost 端點 URL:");
                results.AppendLine($"   GraphQL: https://uxgwdiuehabbzenwtcqo.graphql.eu-central-1.nhost.run/v1");
                results.AppendLine($"   Auth: https://uxgwdiuehabbzenwtcqo.auth.eu-central-1.nhost.run/v1");
                results.AppendLine($"   Functions: https://uxgwdiuehabbzenwtcqo.functions.eu-central-1.nhost.run/v1");
                results.AppendLine($"   Storage: https://uxgwdiuehabbzenwtcqo.storage.eu-central-1.nhost.run/v1");
                results.AppendLine();

                results.AppendLine("測試完成！");
                
                MessageBox.Show(results.ToString(), "NHost 連線測試結果", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"測試過程中發生錯誤：{ex.Message}", "錯誤", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}