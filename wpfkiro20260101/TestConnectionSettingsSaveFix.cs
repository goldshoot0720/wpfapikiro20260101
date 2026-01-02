using System;
using System.Windows;

namespace wpfkiro20260101
{
    /// <summary>
    /// 測試連線設定儲存修復
    /// </summary>
    public class TestConnectionSettingsSaveFix
    {
        public static void RunTest()
        {
            try
            {
                Console.WriteLine("=== 測試連線設定儲存修復 ===\n");

                // 1. 測試基本設定儲存
                TestBasicSettingsSave();

                // 2. 測試服務切換時的設定保存
                TestServiceSwitchingSettings();

                // 3. 測試設定持久化
                TestSettingsPersistence();

                Console.WriteLine("\n=== 所有測試完成 ===");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"測試過程中發生錯誤: {ex.Message}");
            }
        }

        private static void TestBasicSettingsSave()
        {
            Console.WriteLine("--- 測試基本設定儲存 ---");

            try
            {
                var settings = AppSettings.Instance;
                var originalService = settings.BackendService;
                var originalApiUrl = settings.ApiUrl;

                // 測試修改當前服務的設定
                var testApiUrl = $"https://test-basic-{DateTime.Now.Ticks}.example.com";
                settings.ApiUrl = testApiUrl;

                Console.WriteLine($"設定 API URL: {testApiUrl}");

                // 儲存設定
                settings.Save();
                Console.WriteLine("設定已儲存");

                // 重新載入驗證
                AppSettings.ReloadSettings();
                var reloadedSettings = AppSettings.Instance;

                var success = reloadedSettings.ApiUrl == testApiUrl;
                Console.WriteLine($"設定儲存測試: {(success ? "✅ 成功" : "❌ 失敗")}");

                if (!success)
                {
                    Console.WriteLine($"預期: {testApiUrl}");
                    Console.WriteLine($"實際: {reloadedSettings.ApiUrl}");
                }

                // 恢復原始設定
                reloadedSettings.ApiUrl = originalApiUrl;
                reloadedSettings.Save();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"基本設定儲存測試失敗: {ex.Message}");
            }

            Console.WriteLine();
        }

        private static void TestServiceSwitchingSettings()
        {
            Console.WriteLine("--- 測試服務切換時的設定保存 ---");

            try
            {
                var settings = AppSettings.Instance;
                var originalService = settings.BackendService;

                // 測試在 Appwrite 和 Supabase 之間切換
                var appwriteTestUrl = $"https://appwrite-test-{DateTime.Now.Ticks}.example.com";
                var supabaseTestUrl = $"https://supabase-test-{DateTime.Now.Ticks}.example.com";

                // 設定 Appwrite
                settings.BackendService = BackendServiceType.Appwrite;
                settings.ApiUrl = appwriteTestUrl;
                settings.Save();
                Console.WriteLine($"設定 Appwrite API URL: {appwriteTestUrl}");

                // 切換到 Supabase
                settings.BackendService = BackendServiceType.Supabase;
                settings.ApiUrl = supabaseTestUrl;
                settings.Save();
                Console.WriteLine($"設定 Supabase API URL: {supabaseTestUrl}");

                // 驗證 Appwrite 設定是否保留
                settings.BackendService = BackendServiceType.Appwrite;
                var appwriteUrlAfterSwitch = settings.ApiUrl;
                var appwriteSuccess = appwriteUrlAfterSwitch == appwriteTestUrl;

                Console.WriteLine($"Appwrite 設定保留測試: {(appwriteSuccess ? "✅ 成功" : "❌ 失敗")}");
                if (!appwriteSuccess)
                {
                    Console.WriteLine($"預期: {appwriteTestUrl}");
                    Console.WriteLine($"實際: {appwriteUrlAfterSwitch}");
                }

                // 驗證 Supabase 設定是否保留
                settings.BackendService = BackendServiceType.Supabase;
                var supabaseUrlAfterSwitch = settings.ApiUrl;
                var supabaseSuccess = supabaseUrlAfterSwitch == supabaseTestUrl;

                Console.WriteLine($"Supabase 設定保留測試: {(supabaseSuccess ? "✅ 成功" : "❌ 失敗")}");
                if (!supabaseSuccess)
                {
                    Console.WriteLine($"預期: {supabaseTestUrl}");
                    Console.WriteLine($"實際: {supabaseUrlAfterSwitch}");
                }

                // 恢復原始服務
                settings.BackendService = originalService;
                settings.Save();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"服務切換設定測試失敗: {ex.Message}");
            }

            Console.WriteLine();
        }

        private static void TestSettingsPersistence()
        {
            Console.WriteLine("--- 測試設定持久化 ---");

            try
            {
                var settings = AppSettings.Instance;
                var originalService = settings.BackendService;

                // 設定測試值
                var testProjectId = $"test-project-{DateTime.Now.Ticks}";
                var testApiKey = $"test-key-{DateTime.Now.Ticks}";

                settings.ProjectId = testProjectId;
                settings.ApiKey = testApiKey;
                settings.Save();

                Console.WriteLine($"設定 Project ID: {testProjectId}");
                Console.WriteLine($"設定 API Key: {testApiKey}");

                // 強制重新載入設定（模擬應用程式重啟）
                AppSettings.ReloadSettings();
                var reloadedSettings = AppSettings.Instance;

                var projectIdSuccess = reloadedSettings.ProjectId == testProjectId;
                var apiKeySuccess = reloadedSettings.ApiKey == testApiKey;

                Console.WriteLine($"Project ID 持久化測試: {(projectIdSuccess ? "✅ 成功" : "❌ 失敗")}");
                Console.WriteLine($"API Key 持久化測試: {(apiKeySuccess ? "✅ 成功" : "❌ 失敗")}");

                if (!projectIdSuccess)
                {
                    Console.WriteLine($"Project ID - 預期: {testProjectId}, 實際: {reloadedSettings.ProjectId}");
                }

                if (!apiKeySuccess)
                {
                    Console.WriteLine($"API Key - 預期: {testApiKey}, 實際: {reloadedSettings.ApiKey}");
                }

                var overallSuccess = projectIdSuccess && apiKeySuccess;
                Console.WriteLine($"整體持久化測試: {(overallSuccess ? "✅ 成功" : "❌ 失敗")}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"設定持久化測試失敗: {ex.Message}");
            }

            Console.WriteLine();
        }

        public static void TestSettingsPageSaveLogic()
        {
            Console.WriteLine("=== 測試設定頁面儲存邏輯 ===\n");

            try
            {
                // 模擬設定頁面的儲存邏輯
                var settings = AppSettings.Instance;
                var originalService = settings.BackendService;

                // 模擬用戶選擇不同的服務並修改設定
                var selectedService = BackendServiceType.Supabase;
                var testApiUrl = $"https://test-page-logic-{DateTime.Now.Ticks}.example.com";
                var testProjectId = $"test-page-project-{DateTime.Now.Ticks}";

                Console.WriteLine($"模擬選擇服務: {selectedService}");
                Console.WriteLine($"模擬設定 API URL: {testApiUrl}");
                Console.WriteLine($"模擬設定 Project ID: {testProjectId}");

                // 使用修復後的邏輯：先更新服務，再儲存設定
                settings.BackendService = selectedService;

                // 模擬 SaveCurrentServiceSettings 的邏輯
                switch (settings.BackendService)
                {
                    case BackendServiceType.Supabase:
                        settings.Supabase.ApiUrl = testApiUrl;
                        settings.Supabase.ProjectId = testProjectId;
                        break;
                }

                settings.Save();
                Console.WriteLine("設定已儲存");

                // 驗證設定是否正確儲存
                AppSettings.ReloadSettings();
                var reloadedSettings = AppSettings.Instance;
                reloadedSettings.BackendService = selectedService;

                var success = reloadedSettings.ApiUrl == testApiUrl && 
                             reloadedSettings.ProjectId == testProjectId;

                Console.WriteLine($"設定頁面邏輯測試: {(success ? "✅ 成功" : "❌ 失敗")}");

                if (!success)
                {
                    Console.WriteLine($"API URL - 預期: {testApiUrl}, 實際: {reloadedSettings.ApiUrl}");
                    Console.WriteLine($"Project ID - 預期: {testProjectId}, 實際: {reloadedSettings.ProjectId}");
                }

                // 恢復原始服務
                settings.BackendService = originalService;
                settings.Save();

                Console.WriteLine("\n=== 設定頁面邏輯測試完成 ===");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"設定頁面邏輯測試失敗: {ex.Message}");
            }
        }
    }
}