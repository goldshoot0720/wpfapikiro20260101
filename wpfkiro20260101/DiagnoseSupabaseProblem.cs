using System;
using System.IO;

namespace wpfkiro20260101
{
    public class DiagnoseSupabaseProblem
    {
        public static void RunDiagnosis()
        {
            Console.WriteLine("=== Supabase 問題診斷工具 ===");
            
            try
            {
                // 1. 檢查設定檔案
                var settingsPath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "wpfkiro20260101",
                    "settings.json"
                );
                
                Console.WriteLine($"1. 設定檔案路徑: {settingsPath}");
                
                if (File.Exists(settingsPath))
                {
                    var fileContent = File.ReadAllText(settingsPath);
                    Console.WriteLine("2. 設定檔案內容:");
                    Console.WriteLine(fileContent);
                    
                    // 檢查是否包含正確的 Supabase 設定
                    bool hasSupabaseService = fileContent.Contains("\"BackendService\": 1");
                    bool hasSupabaseUrl = fileContent.Contains("lobezwpworbfktlkxuyo.supabase.co");
                    bool hasSupabaseProjectId = fileContent.Contains("\"ProjectId\": \"lobezwpworbfktlkxuyo\"");
                    
                    Console.WriteLine("\n3. 設定檔案檢查結果:");
                    Console.WriteLine($"   ✅ BackendService = 1 (Supabase): {hasSupabaseService}");
                    Console.WriteLine($"   ✅ Supabase API URL: {hasSupabaseUrl}");
                    Console.WriteLine($"   ✅ 正確的 Project ID: {hasSupabaseProjectId}");
                    
                    if (hasSupabaseService && hasSupabaseUrl && hasSupabaseProjectId)
                    {
                        Console.WriteLine("\n🎉 設定檔案完全正確！");
                        Console.WriteLine("問題是應用程式還在運行舊的程式碼。");
                    }
                    else
                    {
                        Console.WriteLine("\n⚠️ 設定檔案有問題，需要修正。");
                    }
                }
                else
                {
                    Console.WriteLine("2. ❌ 設定檔案不存在！");
                }
                
                // 2. 檢查 AppSettings 實例
                Console.WriteLine("\n4. 檢查 AppSettings 實例:");
                var settings = AppSettings.Instance;
                Console.WriteLine($"   當前後端服務: {settings.BackendService}");
                Console.WriteLine($"   API URL: {settings.ApiUrl}");
                Console.WriteLine($"   Project ID: {settings.ProjectId}");
                
                bool appSettingsCorrect = 
                    settings.BackendService == BackendServiceType.Supabase &&
                    settings.ApiUrl == "https://lobezwpworbfktlkxuyo.supabase.co" &&
                    settings.ProjectId == "lobezwpworbfktlkxuyo";
                
                Console.WriteLine($"\n   ✅ AppSettings 正確: {appSettingsCorrect}");
                
                // 3. 檢查 BackendServiceFactory
                Console.WriteLine("\n5. 檢查 BackendServiceFactory:");
                try
                {
                    var service = Services.BackendServiceFactory.CreateCurrentService();
                    Console.WriteLine($"   創建的服務類型: {service.ServiceName}");
                    Console.WriteLine($"   服務類型正確: {service.ServiceName == "Supabase"}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"   ❌ 創建服務失敗: {ex.Message}");
                }
                
                // 4. 提供解決方案
                Console.WriteLine("\n=== 解決方案 ===");
                
                if (File.Exists(settingsPath))
                {
                    var fileContent = File.ReadAllText(settingsPath);
                    if (fileContent.Contains("\"BackendService\": 1") && 
                        fileContent.Contains("lobezwpworbfktlkxuyo.supabase.co"))
                    {
                        Console.WriteLine("✅ 設定檔案正確");
                        Console.WriteLine("🔄 問題：應用程式需要重新啟動");
                        Console.WriteLine("\n立即解決步驟:");
                        Console.WriteLine("1. 完全關閉當前應用程式");
                        Console.WriteLine("2. 等待 5 秒鐘");
                        Console.WriteLine("3. 重新開啟應用程式");
                        Console.WriteLine("4. 進入系統設定，選擇 Supabase");
                        Console.WriteLine("5. 點擊「測試設定」確認顯示 Supabase 內容");
                    }
                    else
                    {
                        Console.WriteLine("❌ 設定檔案不正確");
                        Console.WriteLine("\n修正步驟:");
                        Console.WriteLine("1. 關閉應用程式");
                        Console.WriteLine("2. 刪除設定檔案");
                        Console.WriteLine("3. 重新開啟應用程式");
                        Console.WriteLine("4. 重新配置 Supabase 設定");
                    }
                }
                else
                {
                    Console.WriteLine("❌ 設定檔案不存在");
                    Console.WriteLine("\n解決步驟:");
                    Console.WriteLine("1. 進入應用程式系統設定");
                    Console.WriteLine("2. 選擇 Supabase");
                    Console.WriteLine("3. 輸入正確的設定值");
                    Console.WriteLine("4. 點擊儲存設定");
                }
                
            }
            catch (Exception ex)
            {
                Console.WriteLine($"診斷過程中發生錯誤: {ex.Message}");
                Console.WriteLine($"詳細錯誤: {ex}");
            }
            
            Console.WriteLine("\n=== 診斷完成 ===");
        }
    }
}