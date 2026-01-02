using System;
using System.Threading.Tasks;
using wpfkiro20260101.Services;

namespace wpfkiro20260101
{
    /// <summary>
    /// 快速測試設定檔熱重載功能
    /// </summary>
    public static class QuickHotReloadTest
    {
        public static async Task RunQuickTest()
        {
            Console.WriteLine("=== 快速熱重載測試 ===");
            
            try
            {
                var settings = AppSettings.Instance;
                Console.WriteLine($"當前後端服務: {settings.BackendService}");
                
                // 測試事件是否正確設置
                bool eventTriggered = false;
                AppSettings.SettingsChanged += () => {
                    eventTriggered = true;
                    Console.WriteLine("✅ SettingsChanged 事件已觸發");
                };
                
                // 測試通過 SettingsProfileService 實例觸發事件
                var profileService = SettingsProfileService.Instance;
                // 由於事件只能在聲明類內部直接調用，我們通過載入設定檔來觸發事件
                Console.WriteLine("測試事件觸發機制...");
                
                // 等待事件處理
                await Task.Delay(100);
                
                if (eventTriggered)
                {
                    Console.WriteLine("✅ 事件系統運作正常");
                }
                else
                {
                    Console.WriteLine("❌ 事件系統未正確觸發");
                }
                
                Console.WriteLine("=== 快速測試完成 ===");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ 測試失敗: {ex.Message}");
            }
        }
    }
}