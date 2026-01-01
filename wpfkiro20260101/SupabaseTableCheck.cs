using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace wpfkiro20260101
{
    /// <summary>
    /// Supabase 資料表檢查工具
    /// 用於確認哪些資料表存在
    /// </summary>
    public class SupabaseTableCheck
    {
        private static readonly string ApiUrl = "https://lobezwpworbfktlkxuyo.supabase.co";
        private static readonly string ApiKey = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6ImxvYmV6d3B3b3JiZmt0bGt4dXlvIiwicm9sZSI6InNlcnZpY2Vfcm9sZSIsImlhdCI6MTc2NzI1ODU5MSwiZXhwIjoyMDgyODM0NTkxfQ.tFcCP7kvcfV1CznhIHXBF0TenGlYD1XRlAWdCYYEnlc";

        public static async Task CheckTables()
        {
            Console.WriteLine("🔍 檢查 Supabase 資料表狀態");
            Console.WriteLine("================================");

            using var httpClient = new HttpClient();
            
            // 設定標頭
            httpClient.DefaultRequestHeaders.Clear();
            httpClient.DefaultRequestHeaders.Add("apikey", ApiKey);
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {ApiKey}");

            var tablesToCheck = new[] { "food", "foods", "subscription", "subscriptions" };

            foreach (var table in tablesToCheck)
            {
                try
                {
                    Console.WriteLine($"\n📋 測試資料表: {table}");
                    var response = await httpClient.GetAsync($"{ApiUrl}/rest/v1/{table}");
                    
                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        Console.WriteLine($"   ✅ {table} 存在 (狀態: {response.StatusCode})");
                        Console.WriteLine($"   📊 回應內容: {content}");
                    }
                    else
                    {
                        Console.WriteLine($"   ❌ {table} 不存在或無法存取 (狀態: {response.StatusCode})");
                        var errorContent = await response.Content.ReadAsStringAsync();
                        Console.WriteLine($"   錯誤: {errorContent}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"   ❌ {table} 測試失敗: {ex.Message}");
                }
            }

            Console.WriteLine("\n================================");
            Console.WriteLine("✅ 資料表檢查完成");
        }
    }
}