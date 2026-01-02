using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using wpfkiro20260101.Services;

namespace wpfkiro20260101
{
    /// <summary>
    /// Supabase 快速診斷工具
    /// </summary>
    public static class QuickSupabaseDiagnosis
    {
        public static async Task RunQuickDiagnosis()
        {
            Console.WriteLine("=== Supabase 快速診斷 ===");
            
            try
            {
                var settings = AppSettings.Instance;
                
                // 1. 檢查設定
                Console.WriteLine("🔧 檢查 Supabase 設定...");
                Console.WriteLine($"API URL: {settings.Supabase.ApiUrl}");
                Console.WriteLine($"Project ID: {settings.Supabase.ProjectId}");
                Console.WriteLine($"API Key: {(string.IsNullOrEmpty(settings.Supabase.ApiKey) ? "❌ 未設定" : "✅ 已設定")}");
                
                if (string.IsNullOrEmpty(settings.Supabase.ApiKey))
                {
                    Console.WriteLine("❌ API Key 未設定，無法進行測試");
                    return;
                }
                
                // 2. 測試基本連線
                Console.WriteLine("\n🌐 測試基本連線...");
                var connectionResult = await TestBasicConnection(settings.Supabase);
                
                if (!connectionResult)
                {
                    Console.WriteLine("❌ 基本連線失敗，請檢查網路和設定");
                    return;
                }
                
                // 3. 測試資料表存在性
                Console.WriteLine("\n📋 檢查資料表...");
                await CheckTables(settings.Supabase);
                
                // 4. 測試 CRUD 操作
                Console.WriteLine("\n🔄 測試 CRUD 操作...");
                await TestCrudOperations();
                
                // 5. 測試頁面載入
                Console.WriteLine("\n📄 測試頁面資料載入...");
                await TestPageDataLoading();
                
                Console.WriteLine("\n=== Supabase 快速診斷完成 ===");
                
                // 顯示診斷結果摘要
                ShowDiagnosisResult();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ 診斷過程中發生錯誤: {ex.Message}");
                Console.WriteLine($"詳細錯誤: {ex}");
            }
        }
        
        private static async Task<bool> TestBasicConnection(SupabaseSettings settings)
        {
            try
            {
                using var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("apikey", settings.ApiKey);
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {settings.ApiKey}");
                
                var response = await httpClient.GetAsync($"{settings.ApiUrl}/rest/v1/");
                
                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("✅ 基本連線成功");
                    return true;
                }
                else
                {
                    Console.WriteLine($"❌ 基本連線失敗: {response.StatusCode}");
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"錯誤詳情: {errorContent}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ 連線測試異常: {ex.Message}");
                return false;
            }
        }
        
        private static async Task CheckTables(SupabaseSettings settings)
        {
            var tables = new[] { "food", "subscription" };
            
            foreach (var table in tables)
            {
                try
                {
                    using var httpClient = new HttpClient();
                    httpClient.DefaultRequestHeaders.Add("apikey", settings.ApiKey);
                    httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {settings.ApiKey}");
                    
                    var response = await httpClient.GetAsync($"{settings.ApiUrl}/rest/v1/{table}");
                    
                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        var recordCount = content == "[]" ? "0" : "多筆"; // 統一為字串類型
                        Console.WriteLine($"✅ 資料表 '{table}' 存在，包含 {recordCount} 記錄");
                    }
                    else
                    {
                        Console.WriteLine($"❌ 資料表 '{table}' 不可訪問: {response.StatusCode}");
                        
                        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                        {
                            Console.WriteLine($"   建議: 請在 Supabase 控制台創建 '{table}' 資料表");
                        }
                        else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                        {
                            Console.WriteLine($"   建議: 請檢查 API Key 權限設定");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ 檢查資料表 '{table}' 時發生異常: {ex.Message}");
                }
            }
        }
        
        private static async Task TestCrudOperations()
        {
            try
            {
                var supabaseService = new SupabaseService();
                
                // 測試讀取食品
                Console.WriteLine("📖 測試讀取食品資料...");
                var foodsResult = await supabaseService.GetFoodsAsync();
                if (foodsResult.Success)
                {
                    Console.WriteLine($"✅ 成功讀取 {foodsResult.Data?.Length ?? 0} 項食品");
                }
                else
                {
                    Console.WriteLine($"❌ 讀取食品失敗: {foodsResult.ErrorMessage}");
                }
                
                // 測試讀取訂閱
                Console.WriteLine("📖 測試讀取訂閱資料...");
                var subscriptionsResult = await supabaseService.GetSubscriptionsAsync();
                if (subscriptionsResult.Success)
                {
                    Console.WriteLine($"✅ 成功讀取 {subscriptionsResult.Data?.Length ?? 0} 項訂閱");
                }
                else
                {
                    Console.WriteLine($"❌ 讀取訂閱失敗: {subscriptionsResult.ErrorMessage}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ CRUD 操作測試異常: {ex.Message}");
            }
        }
        
        private static async Task TestPageDataLoading()
        {
            try
            {
                // 測試透過 BackendServiceFactory 載入資料
                var currentService = BackendServiceFactory.CreateCurrentService();
                
                if (currentService is SupabaseService)
                {
                    Console.WriteLine("✅ BackendServiceFactory 正確創建 SupabaseService");
                }
                else
                {
                    Console.WriteLine($"⚠️ BackendServiceFactory 創建了 {currentService?.GetType().Name}，不是 SupabaseService");
                }
                
                // 測試 CrudManager
                var crudManager = BackendServiceFactory.CreateCrudManager();
                var testResult = await crudManager.GetAllFoodsAsync();
                
                if (testResult.Success)
                {
                    Console.WriteLine($"✅ CrudManager 成功載入 {testResult.Data?.Length ?? 0} 項食品資料");
                }
                else
                {
                    Console.WriteLine($"❌ CrudManager 載入失敗: {testResult.ErrorMessage}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ 頁面資料載入測試異常: {ex.Message}");
            }
        }
        
        private static void ShowDiagnosisResult()
        {
            var settings = AppSettings.Instance;
            
            var message = $@"
🔍 Supabase 診斷結果摘要

📊 設定狀態:
• API URL: {settings.Supabase.ApiUrl}
• Project ID: {settings.Supabase.ProjectId}
• API Key: {(string.IsNullOrEmpty(settings.Supabase.ApiKey) ? "❌ 未設定" : "✅ 已設定")}

🎯 建議檢查項目:
1. 確認 Supabase 專案是否正常運行
2. 檢查 API Key 是否有效且具備正確權限
3. 確認 'food' 和 'subscription' 資料表是否存在
4. 檢查 Row Level Security (RLS) 政策設定
5. 確認網路連線是否正常

💡 常見問題解決:
• 401 錯誤: API Key 無效或權限不足
• 404 錯誤: 資料表不存在
• 403 錯誤: RLS 政策阻止訪問
• 網路錯誤: 檢查防火牆或代理設定

🔧 下一步操作:
1. 如果連線失敗，請檢查 Supabase 控制台
2. 如果資料表不存在，請創建對應的資料表
3. 如果權限問題，請調整 RLS 政策
4. 測試完成後，可以嘗試在食品或訂閱頁面載入資料

詳細診斷結果請查看控制台輸出。
";
            
            MessageBox.Show(message, "Supabase 診斷結果", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        
        public static async Task TestSupabaseWithCurrentSettings()
        {
            Console.WriteLine("=== 使用當前設定測試 Supabase ===");
            
            try
            {
                var settings = AppSettings.Instance;
                
                // 暫時切換到 Supabase
                var originalService = settings.BackendService;
                settings.BackendService = BackendServiceType.Supabase;
                
                Console.WriteLine($"✅ 切換到 Supabase 服務進行測試");
                Console.WriteLine($"使用設定: {settings.Supabase.ApiUrl}");
                
                // 執行綜合測試
                await TestSupabaseComprehensive.RunComprehensiveTest();
                
                // 恢復原始設定
                settings.BackendService = originalService;
                Console.WriteLine($"✅ 恢復原始服務: {originalService}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ 測試失敗: {ex.Message}");
            }
        }
    }
}