using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace wpfkiro20260101
{
    /// <summary>
    /// 快速 Supabase 連接測試
    /// 使用正確的 API 金鑰和 URL 測試連接
    /// </summary>
    public class QuickSupabaseTest
    {
        private static readonly string ApiUrl = "https://lobezwpworbfktlkxuyo.supabase.co";
        private static readonly string ApiKey = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6ImxvYmV6d3B3b3JiZmt0bGt4dXlvIiwicm9sZSI6InNlcnZpY2Vfcm9sZSIsImlhdCI6MTc2NzI1ODU5MSwiZXhwIjoyMDgyODM0NTkxfQ.tFcCP7kvcfV1CznhIHXBF0TenGlYD1XRlAWdCYYEnlc";

        public static async Task TestConnection()
        {
            Console.WriteLine("🔍 快速 Supabase 連接測試");
            Console.WriteLine("================================");
            Console.WriteLine($"API URL: {ApiUrl}");
            Console.WriteLine($"API Key: {ApiKey.Substring(0, 20)}...");
            Console.WriteLine();

            using var httpClient = new HttpClient();
            
            // 設定標頭
            httpClient.DefaultRequestHeaders.Clear();
            httpClient.DefaultRequestHeaders.Add("apikey", ApiKey);
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {ApiKey}");

            try
            {
                // 1. 測試基本 API 連接
                Console.WriteLine("📡 測試基本 API 連接...");
                var response = await httpClient.GetAsync($"{ApiUrl}/rest/v1/");
                Console.WriteLine($"   狀態碼: {response.StatusCode}");
                Console.WriteLine($"   成功: {response.IsSuccessStatusCode}");
                
                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"   錯誤: {errorContent}");
                    return;
                }

                // 2. 測試 food 資料表
                Console.WriteLine("\n🍎 測試 food 資料表...");
                var foodResponse = await httpClient.GetAsync($"{ApiUrl}/rest/v1/food");
                Console.WriteLine($"   狀態碼: {foodResponse.StatusCode}");
                Console.WriteLine($"   成功: {foodResponse.IsSuccessStatusCode}");
                
                if (foodResponse.IsSuccessStatusCode)
                {
                    var foodContent = await foodResponse.Content.ReadAsStringAsync();
                    Console.WriteLine($"   ✅ food 資料表可用");
                    Console.WriteLine($"   回應長度: {foodContent.Length} 字元");
                    if (foodContent.Length < 500)
                    {
                        Console.WriteLine($"   內容: {foodContent}");
                    }
                }
                else
                {
                    var errorContent = await foodResponse.Content.ReadAsStringAsync();
                    Console.WriteLine($"   ❌ food 資料表錯誤: {errorContent}");
                }

                // 3. 測試 subscription 資料表
                Console.WriteLine("\n📋 測試 subscription 資料表...");
                var subResponse = await httpClient.GetAsync($"{ApiUrl}/rest/v1/subscription");
                Console.WriteLine($"   狀態碼: {subResponse.StatusCode}");
                Console.WriteLine($"   成功: {subResponse.IsSuccessStatusCode}");
                
                if (subResponse.IsSuccessStatusCode)
                {
                    var subContent = await subResponse.Content.ReadAsStringAsync();
                    Console.WriteLine($"   ✅ subscription 資料表可用");
                    Console.WriteLine($"   回應長度: {subContent.Length} 字元");
                    if (subContent.Length < 500)
                    {
                        Console.WriteLine($"   內容: {subContent}");
                    }
                }
                else
                {
                    var errorContent = await subResponse.Content.ReadAsStringAsync();
                    Console.WriteLine($"   ❌ subscription 資料表錯誤: {errorContent}");
                }

                Console.WriteLine("\n✅ 測試完成！");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n❌ 測試失敗: {ex.Message}");
            }
        }
    }
}