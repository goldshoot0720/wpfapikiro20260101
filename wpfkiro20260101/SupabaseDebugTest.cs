using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;

namespace wpfkiro20260101
{
    /// <summary>
    /// Supabase 調試測試工具
    /// 用於診斷 Supabase 連接問題
    /// </summary>
    public class SupabaseDebugTest
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiUrl = "https://lobezwpworbfktlkxuyo.supabase.co";
        private readonly string _apiKey = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6ImxvYmV6d3B3b3JiZmt0bGt4dXlvIiwicm9sZSI6InNlcnZpY2Vfcm9sZSIsImlhdCI6MTc2NzI1ODU5MSwiZXhwIjoyMDgyODM0NTkxfQ.tFcCP7kvcfV1CznhIHXBF0TenGlYD1XRlAWdCYYEnlc";

        public SupabaseDebugTest()
        {
            _httpClient = new HttpClient();
        }

        /// <summary>
        /// 執行完整的 Supabase 診斷測試
        /// </summary>
        public async Task RunDiagnosticTests()
        {
            Console.WriteLine("=================================");
            Console.WriteLine("🔍 Supabase 診斷測試開始");
            Console.WriteLine("=================================");

            // 1. 測試基本連接
            await TestBasicConnection();

            // 2. 測試 API 根路徑
            await TestApiRoot();

            // 3. 測試資料表列表
            await TestTableList();

            // 4. 測試 food 資料表
            await TestFoodTable();

            // 5. 測試 subscriptions 資料表
            await TestSubscriptionsTable();

            Console.WriteLine("\n=================================");
            Console.WriteLine("🔍 診斷測試完成");
            Console.WriteLine("=================================");
        }

        /// <summary>
        /// 測試基本連接
        /// </summary>
        private async Task TestBasicConnection()
        {
            Console.WriteLine("\n📡 測試基本連接...");
            
            try
            {
                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("apikey", _apiKey);
                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");

                var response = await _httpClient.GetAsync($"{_apiUrl}/rest/v1/");
                
                Console.WriteLine($"   狀態碼: {response.StatusCode}");
                Console.WriteLine($"   成功: {response.IsSuccessStatusCode}");
                
                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"   錯誤內容: {errorContent}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"   ❌ 連接失敗: {ex.Message}");
            }
        }

        /// <summary>
        /// 測試 API 根路徑
        /// </summary>
        private async Task TestApiRoot()
        {
            Console.WriteLine("\n🌐 測試 API 根路徑...");
            
            try
            {
                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("apikey", _apiKey);
                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");

                var response = await _httpClient.GetAsync(_apiUrl);
                
                Console.WriteLine($"   狀態碼: {response.StatusCode}");
                Console.WriteLine($"   成功: {response.IsSuccessStatusCode}");
                
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"   回應內容長度: {content.Length} 字元");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"   ❌ 測試失敗: {ex.Message}");
            }
        }

        /// <summary>
        /// 測試資料表列表
        /// </summary>
        private async Task TestTableList()
        {
            Console.WriteLine("\n📋 測試資料表列表...");
            
            try
            {
                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("apikey", _apiKey);
                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");

                // 嘗試獲取 OpenAPI 規格來查看可用的資料表
                var response = await _httpClient.GetAsync($"{_apiUrl}/rest/v1/");
                
                Console.WriteLine($"   狀態碼: {response.StatusCode}");
                
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"   ✅ API 端點可用");
                    
                    // 嘗試解析 OpenAPI 規格
                    if (content.Contains("openapi") || content.Contains("swagger"))
                    {
                        Console.WriteLine($"   📄 找到 OpenAPI 規格文件");
                    }
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"   ❌ 無法獲取資料表列表: {errorContent}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"   ❌ 測試失敗: {ex.Message}");
            }
        }

        /// <summary>
        /// 測試 food 資料表
        /// </summary>
        private async Task TestFoodTable()
        {
            Console.WriteLine("\n🍎 測試 food 資料表...");
            
            try
            {
                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("apikey", _apiKey);
                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");

                var response = await _httpClient.GetAsync($"{_apiUrl}/rest/v1/food");
                
                Console.WriteLine($"   狀態碼: {response.StatusCode}");
                Console.WriteLine($"   成功: {response.IsSuccessStatusCode}");
                
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"   ✅ food 資料表存在");
                    Console.WriteLine($"   回應內容: {content}");
                    
                    try
                    {
                        var data = JsonSerializer.Deserialize<JsonElement[]>(content);
                        Console.WriteLine($"   📊 資料筆數: {data.Length}");
                    }
                    catch (Exception parseEx)
                    {
                        Console.WriteLine($"   ⚠️ JSON 解析失敗: {parseEx.Message}");
                    }
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"   ❌ food 資料表錯誤: {errorContent}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"   ❌ 測試失敗: {ex.Message}");
            }
        }

        /// <summary>
        /// 測試 subscriptions 資料表
        /// </summary>
        private async Task TestSubscriptionsTable()
        {
            Console.WriteLine("\n📋 測試 subscriptions 資料表...");
            
            try
            {
                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("apikey", _apiKey);
                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");

                var response = await _httpClient.GetAsync($"{_apiUrl}/rest/v1/subscriptions");
                
                Console.WriteLine($"   狀態碼: {response.StatusCode}");
                Console.WriteLine($"   成功: {response.IsSuccessStatusCode}");
                
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"   ✅ subscriptions 資料表存在");
                    Console.WriteLine($"   回應內容: {content}");
                    
                    try
                    {
                        var data = JsonSerializer.Deserialize<JsonElement[]>(content);
                        Console.WriteLine($"   📊 資料筆數: {data.Length}");
                    }
                    catch (Exception parseEx)
                    {
                        Console.WriteLine($"   ⚠️ JSON 解析失敗: {parseEx.Message}");
                    }
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"   ❌ subscriptions 資料表錯誤: {errorContent}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"   ❌ 測試失敗: {ex.Message}");
            }
        }

        /// <summary>
        /// 測試特定 API 端點
        /// </summary>
        public async Task TestSpecificEndpoint(string endpoint)
        {
            Console.WriteLine($"\n🔍 測試端點: {endpoint}");
            
            try
            {
                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("apikey", _apiKey);
                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");

                var fullUrl = $"{_apiUrl}{endpoint}";
                Console.WriteLine($"   完整 URL: {fullUrl}");
                
                var response = await _httpClient.GetAsync(fullUrl);
                
                Console.WriteLine($"   狀態碼: {response.StatusCode}");
                Console.WriteLine($"   成功: {response.IsSuccessStatusCode}");
                
                var content = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"   回應內容: {content}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"   ❌ 測試失敗: {ex.Message}");
            }
        }

        /// <summary>
        /// 釋放資源
        /// </summary>
        public void Dispose()
        {
            _httpClient?.Dispose();
        }
    }

    /// <summary>
    /// 診斷測試程式進入點
    /// </summary>
    public class SupabaseDebugProgram
    {
        public static async Task RunDiagnostics()
        {
            var debugTest = new SupabaseDebugTest();
            
            try
            {
                await debugTest.RunDiagnosticTests();
                
                // 額外測試一些可能的端點
                Console.WriteLine("\n🔍 測試其他可能的端點...");
                await debugTest.TestSpecificEndpoint("/rest/v1/foods"); // 複數形式
                await debugTest.TestSpecificEndpoint("/rest/v1/Food"); // 大寫
                await debugTest.TestSpecificEndpoint("/rest/v1/"); // 根路徑
            }
            finally
            {
                debugTest.Dispose();
            }
        }
    }
}