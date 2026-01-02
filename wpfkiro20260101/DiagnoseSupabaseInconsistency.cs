using System;
using System.Threading.Tasks;
using wpfkiro20260101.Services;

namespace wpfkiro20260101
{
    public class DiagnoseSupabaseInconsistency
    {
        public static async Task RunDiagnosis()
        {
            Console.WriteLine("=== Supabase 資料不一致診斷 ===");
            
            try
            {
                var settings = AppSettings.Instance;
                
                // 檢查設定
                Console.WriteLine("1. 檢查 Supabase 設定:");
                Console.WriteLine($"   API URL: {settings.ApiUrl}");
                Console.WriteLine($"   API Key: {(string.IsNullOrWhiteSpace(settings.ApiKey) ? "未設定" : "已設定")}");
                
                if (string.IsNullOrWhiteSpace(settings.ApiUrl) || 
                    string.IsNullOrWhiteSpace(settings.ApiKey))
                {
                    Console.WriteLine("❌ Supabase 設定不完整，無法進行診斷");
                    return;
                }
                
                var supabaseService = new SupabaseService();
                
                // 2. 測試連接
                Console.WriteLine("\n2. 測試基本連接:");
                var connectionResult = await supabaseService.TestConnectionAsync();
                Console.WriteLine($"   連接結果: {(connectionResult ? "✅ 成功" : "❌ 失敗")}");
                
                // 3. 詳細檢查 Food 資料表
                Console.WriteLine("\n3. 檢查 Food 資料表:");
                await DiagnoseTable("food", settings);
                
                // 4. 詳細檢查 Subscription 資料表
                Console.WriteLine("\n4. 檢查 Subscription 資料表:");
                await DiagnoseTable("subscription", settings);
                
                // 5. 使用服務方法檢查
                Console.WriteLine("\n5. 使用服務方法檢查:");
                
                var foodsResult = await supabaseService.GetFoodsAsync();
                if (foodsResult.Success)
                {
                    Console.WriteLine($"   GetFoodsAsync: ✅ 成功，{foodsResult.Data.Length} 筆資料");
                }
                else
                {
                    Console.WriteLine($"   GetFoodsAsync: ❌ 失敗 - {foodsResult.ErrorMessage}");
                }
                
                var subscriptionsResult = await supabaseService.GetSubscriptionsAsync();
                if (subscriptionsResult.Success)
                {
                    Console.WriteLine($"   GetSubscriptionsAsync: ✅ 成功，{subscriptionsResult.Data.Length} 筆資料");
                    
                    // 顯示詳細內容
                    if (subscriptionsResult.Data.Length > 0)
                    {
                        Console.WriteLine("   第一筆 Subscription 資料:");
                        var firstSub = subscriptionsResult.Data[0];
                        var properties = firstSub.GetType().GetProperties();
                        foreach (var prop in properties)
                        {
                            var value = prop.GetValue(firstSub);
                            Console.WriteLine($"     {prop.Name}: {value}");
                        }
                    }
                }
                else
                {
                    Console.WriteLine($"   GetSubscriptionsAsync: ❌ 失敗 - {subscriptionsResult.ErrorMessage}");
                }
                
                Console.WriteLine("\n=== 診斷完成 ===");
                
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ 診斷過程發生異常: {ex.Message}");
                Console.WriteLine($"詳細錯誤: {ex.StackTrace}");
            }
        }
        
        private static async Task DiagnoseTable(string tableName, AppSettings settings)
        {
            try
            {
                using var httpClient = new System.Net.Http.HttpClient();
                httpClient.DefaultRequestHeaders.Clear();
                httpClient.DefaultRequestHeaders.Add("apikey", settings.ApiKey);
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {settings.ApiKey}");
                
                var apiUrl = $"{settings.ApiUrl}/rest/v1/{tableName}";
                Console.WriteLine($"   API URL: {apiUrl}");
                
                var response = await httpClient.GetAsync(apiUrl);
                Console.WriteLine($"   HTTP 狀態: {response.StatusCode}");
                
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"   回應內容: {content}");
                    Console.WriteLine($"   內容長度: {content.Length} 字元");
                    
                    // 檢查是否為空陣列
                    if (content.Trim() == "[]")
                    {
                        Console.WriteLine("   ⚠️  資料表為空 (空陣列)");
                    }
                    else if (content.Contains("\"id\""))
                    {
                        // 簡單計算記錄數
                        var idCount = content.Split("\"id\"").Length - 1;
                        Console.WriteLine($"   📊 估計記錄數: {idCount}");
                    }
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"   ❌ 錯誤回應: {errorContent}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"   ❌ 檢查 {tableName} 資料表時發生錯誤: {ex.Message}");
            }
        }
    }
}