using System;
using System.Threading.Tasks;
using System.Windows;
using wpfkiro20260101.Services;

namespace wpfkiro20260101
{
    /// <summary>
    /// 測試設定檔熱重載功能（不需重新啟動）
    /// </summary>
    public static class TestHotReloadSettings
    {
        public static async Task TestHotReloadFunctionality()
        {
            Console.WriteLine("=== 測試設定檔熱重載功能 ===");
            
            try
            {
                var profileService = SettingsProfileService.Instance;
                var currentSettings = AppSettings.Instance;
                
                Console.WriteLine($"1. 當前設定狀態:");
                Console.WriteLine($"   後端服務: {currentSettings.BackendService}");
                Console.WriteLine($"   API URL: {currentSettings.ApiUrl}");
                Console.WriteLine($"   Project ID: {currentSettings.ProjectId}");
                
                // 創建測試設定檔
                Console.WriteLine("\n2. 創建測試設定檔...");
                var testProfileName = $"測試熱重載_{DateTime.Now:HHmmss}";
                var createResult = await profileService.CreateFromCurrentSettingsAsync(
                    testProfileName, 
                    "測試設定檔熱重載功能"
                );
                
                if (!createResult.Success)
                {
                    Console.WriteLine($"   創建設定檔失敗: {createResult.ErrorMessage}");
                    return;
                }
                
                var testProfile = createResult.Data;
                Console.WriteLine($"   設定檔已創建: {testProfile?.ProfileName}");
                
                // 修改當前設定
                Console.WriteLine("\n3. 暫時修改當前設定...");
                var originalService = currentSettings.BackendService;
                var originalUrl = currentSettings.ApiUrl;
                
                // 切換到不同的服務進行測試
                var newService = originalService == BackendServiceType.Appwrite 
                    ? BackendServiceType.Supabase 
                    : BackendServiceType.Appwrite;
                
                currentSettings.BackendService = newService;
                currentSettings.Save();
                
                Console.WriteLine($"   已切換到: {newService}");
                Console.WriteLine($"   新 API URL: {currentSettings.ApiUrl}");
                
                // 等待一下讓變更生效
                await Task.Delay(500);
                
                // 載入原始設定檔
                Console.WriteLine("\n4. 載入原始設定檔（測試熱重載）...");
                var loadResult = await profileService.LoadProfileAsync(testProfile!.Id);
                
                if (loadResult.Success)
                {
                    Console.WriteLine("   ✅ 設定檔載入成功");
                    
                    // 等待事件處理
                    await Task.Delay(1000);
                    
                    // 驗證設定是否已更新
                    var updatedSettings = AppSettings.Instance;
                    Console.WriteLine($"   載入後的後端服務: {updatedSettings.BackendService}");
                    Console.WriteLine($"   載入後的 API URL: {updatedSettings.ApiUrl}");
                    
                    if (updatedSettings.BackendService == originalService && 
                        updatedSettings.ApiUrl == originalUrl)
                    {
                        Console.WriteLine("   ✅ 設定已成功恢復，熱重載功能正常");
                    }
                    else
                    {
                        Console.WriteLine("   ❌ 設定未正確恢復");
                    }
                }
                else
                {
                    Console.WriteLine($"   ❌ 載入設定檔失敗: {loadResult.ErrorMessage}");
                }
                
                // 清理測試設定檔
                Console.WriteLine("\n5. 清理測試設定檔...");
                var deleteResult = await profileService.DeleteProfileAsync(testProfile.Id);
                if (deleteResult.Success)
                {
                    Console.WriteLine("   測試設定檔已刪除");
                }
                
                Console.WriteLine("\n=== 熱重載功能測試完成 ===");
                
            }
            catch (Exception ex)
            {
                Console.WriteLine($"測試過程中發生錯誤: {ex.Message}");
            }
        }
        
        public static void ShowHotReloadGuide()
        {
            var message = @"🔥 設定檔熱重載功能說明

✨ 新功能特點：
• 載入設定檔後無需重新啟動應用程式
• 所有頁面會自動更新為新的設定
• 後端服務連線會即時切換
• UI 界面會即時反映新設定

🎯 使用方式：
1. 在設定檔管理視窗選擇要載入的設定檔
2. 點擊「載入設定檔」按鈕
3. 確認載入後，所有設定立即生效
4. 無需重新啟動應用程式

📋 自動更新的組件：
• 設定頁面 - 立即顯示新的連線設定
• 食品頁面 - 重新載入對應後端的資料
• 訂閱頁面 - 重新載入對應後端的資料
• 所有使用後端服務的功能

⚡ 技術實現：
• 使用事件驅動架構
• AppSettings 變更時觸發全域事件
• 各頁面訂閱事件並自動更新
• 確保 UI 線程安全

這個功能大幅提升了開發和使用體驗！";

            MessageBox.Show(message, "設定檔熱重載功能", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}