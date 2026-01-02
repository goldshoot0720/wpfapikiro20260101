using System;
using System.Threading.Tasks;
using System.Windows;
using wpfkiro20260101.Services;

namespace wpfkiro20260101
{
    public class TestSupabaseCsvFixed
    {
        public static async Task RunTest()
        {
            try
            {
                Console.WriteLine("=== 測試修正後的 Supabase CSV 生成 ===");
                
                // 確保使用 Supabase 設定
                var settings = AppSettings.Instance;
                settings.BackendService = BackendServiceType.Supabase;
                settings.Save();
                
                Console.WriteLine($"✅ 後端服務: {settings.GetServiceDisplayName()}");
                
                // 創建 Supabase 服務實例
                var supabaseService = new SupabaseService();
                
                // 測試連接
                Console.WriteLine("\n--- 測試 Supabase 連接 ---");
                var connectionTest = await supabaseService.TestConnectionAsync();
                Console.WriteLine($"連接測試: {(connectionTest ? "✅ 成功" : "❌ 失敗")}");
                
                if (!connectionTest)
                {
                    Console.WriteLine("⚠️ 無法連接到 Supabase，請檢查設定");
                    return;
                }
                
                // 測試 Food 資料獲取
                Console.WriteLine("\n--- 測試 Food 資料 ---");
                var foodResult = await supabaseService.GetFoodsAsync();
                Console.WriteLine($"Food 資料獲取: {(foodResult.Success ? "✅ 成功" : "❌ 失敗")}");
                
                if (foodResult.Success)
                {
                    Console.WriteLine($"Food 記錄數: {foodResult.Data.Length}");
                    
                    // 模擬 CSV 生成（使用 SettingsPage 的邏輯）
                    var csvContent = GenerateTestFoodCsv(foodResult.Data);
                    Console.WriteLine("\n--- Food CSV 預覽 ---");
                    var lines = csvContent.Split('\n');
                    for (int i = 0; i < Math.Min(3, lines.Length); i++)
                    {
                        Console.WriteLine($"第 {i + 1} 行: {lines[i]}");
                    }
                }
                else
                {
                    Console.WriteLine($"Food 錯誤: {foodResult.ErrorMessage}");
                }
                
                // 測試 Subscription 資料獲取
                Console.WriteLine("\n--- 測試 Subscription 資料 ---");
                var subscriptionResult = await supabaseService.GetSubscriptionsAsync();
                Console.WriteLine($"Subscription 資料獲取: {(subscriptionResult.Success ? "✅ 成功" : "❌ 失敗")}");
                
                if (subscriptionResult.Success)
                {
                    Console.WriteLine($"Subscription 記錄數: {subscriptionResult.Data.Length}");
                    
                    // 模擬 CSV 生成
                    var csvContent = GenerateTestSubscriptionCsv(subscriptionResult.Data);
                    Console.WriteLine("\n--- Subscription CSV 預覽 ---");
                    var lines = csvContent.Split('\n');
                    for (int i = 0; i < Math.Min(3, lines.Length); i++)
                    {
                        Console.WriteLine($"第 {i + 1} 行: {lines[i]}");
                    }
                }
                else
                {
                    Console.WriteLine($"Subscription 錯誤: {subscriptionResult.ErrorMessage}");
                }
                
                // 顯示結果
                var message = "Supabase CSV 測試完成！\n\n";
                message += $"Food 資料: {(foodResult.Success ? "✅ 正常" : "❌ 有問題")}\n";
                message += $"Subscription 資料: {(subscriptionResult.Success ? "✅ 正常" : "❌ 有問題")}\n\n";
                
                if (foodResult.Success && subscriptionResult.Success)
                {
                    message += "✅ 所有測試通過！\n";
                    message += "現在可以重新導出 CSV 並嘗試導入 Supabase。\n\n";
                    message += "CSV 格式已修正為:\n";
                    message += "• Food: id,created_at,updated_at,name,price,photo,shop,todate,account\n";
                    message += "• Subscription: id,created_at,updated_at,name,nextdate,price,site,account,note";
                }
                else
                {
                    message += "❌ 發現問題，建議:\n";
                    message += "1. 確認 Supabase 中的表是否存在\n";
                    message += "2. 檢查表名是否正確 (food, subscription)\n";
                    message += "3. 驗證 API Key 權限\n";
                    message += "4. 執行 CREATE_SUBSCRIPTION_TABLE.sql 創建 subscription 表";
                }
                
                MessageBox.Show(message, "CSV 測試結果", MessageBoxButton.OK, 
                    (foodResult.Success && subscriptionResult.Success) ? MessageBoxImage.Information : MessageBoxImage.Warning);
                
                Console.WriteLine("\n=== 測試完成 ===");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ 測試過程中發生異常: {ex.Message}");
                MessageBox.Show($"測試失敗：{ex.Message}", "錯誤", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        
        private static string GenerateTestFoodCsv(object[] foods)
        {
            var csv = new System.Text.StringBuilder();
            
            // Supabase 格式標題行
            csv.AppendLine("id,created_at,updated_at,name,price,photo,shop,todate,account");
            
            foreach (var item in foods)
            {
                try
                {
                    var id = GetPropertyValue(item, "id") ?? "";
                    var createdAt = GetPropertyValue(item, "createdAt", "created_at") ?? "";
                    var updatedAt = GetPropertyValue(item, "updatedAt", "updated_at") ?? "";
                    var name = GetPropertyValue(item, "foodName", "name") ?? "";
                    var price = GetPropertyValue(item, "price") ?? "0";
                    var photo = GetPropertyValue(item, "photo") ?? "";
                    var shop = GetPropertyValue(item, "shop") ?? "";
                    var todate = GetPropertyValue(item, "toDate", "todate") ?? "";
                    var account = GetPropertyValue(item, "account") ?? "";
                    
                    csv.AppendLine($"\"{id}\",\"{createdAt}\",\"{updatedAt}\",\"{name}\",\"{price}\",\"{photo}\",\"{shop}\",\"{todate}\",\"{account}\"");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"處理 Food 項目時發生錯誤：{ex.Message}");
                }
            }
            
            return csv.ToString();
        }
        
        private static string GenerateTestSubscriptionCsv(object[] subscriptions)
        {
            var csv = new System.Text.StringBuilder();
            
            // Supabase 格式標題行
            csv.AppendLine("id,created_at,updated_at,name,nextdate,price,site,account,note");
            
            foreach (var item in subscriptions)
            {
                try
                {
                    var id = GetPropertyValue(item, "id") ?? "";
                    var createdAt = GetPropertyValue(item, "createdAt", "created_at") ?? "";
                    var updatedAt = GetPropertyValue(item, "updatedAt", "updated_at") ?? "";
                    var name = GetPropertyValue(item, "subscriptionName", "name") ?? "";
                    var nextdate = GetPropertyValue(item, "nextDate", "nextdate") ?? "";
                    var price = GetPropertyValue(item, "price") ?? "0";
                    var site = GetPropertyValue(item, "site") ?? "";
                    var account = GetPropertyValue(item, "account") ?? "";
                    var note = GetPropertyValue(item, "note") ?? "";
                    
                    csv.AppendLine($"\"{id}\",\"{createdAt}\",\"{updatedAt}\",\"{name}\",\"{nextdate}\",\"{price}\",\"{site}\",\"{account}\",\"{note}\"");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"處理 Subscription 項目時發生錯誤：{ex.Message}");
                }
            }
            
            return csv.ToString();
        }
        
        private static string GetPropertyValue(object obj, params string[] propertyNames)
        {
            foreach (var propertyName in propertyNames)
            {
                var property = obj.GetType().GetProperty(propertyName);
                if (property != null)
                {
                    var value = property.GetValue(obj);
                    return value?.ToString() ?? "";
                }
            }
            return "";
        }
    }
}