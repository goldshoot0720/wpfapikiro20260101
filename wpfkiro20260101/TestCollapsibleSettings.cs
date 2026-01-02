using System;
using System.Threading.Tasks;
using System.Windows;

namespace wpfkiro20260101
{
    /// <summary>
    /// 測試可摺疊設定功能
    /// </summary>
    public static class TestCollapsibleSettings
    {
        public static async Task TestCollapsibleFunctionality()
        {
            Console.WriteLine("=== 測試可摺疊設定功能 ===");
            
            try
            {
                // 測試 AppSettings 中的 Table ID 欄位
                var settings = AppSettings.Instance;
                
                Console.WriteLine($"當前後端服務: {settings.BackendService}");
                Console.WriteLine($"Food Collection ID: {settings.FoodCollectionId}");
                Console.WriteLine($"Subscription Collection ID: {settings.SubscriptionCollectionId}");
                
                // 測試設定 Appwrite Table ID
                if (settings.BackendService == BackendServiceType.Appwrite)
                {
                    Console.WriteLine("✅ Appwrite 後端服務已選擇");
                    
                    // 檢查預設的 Table ID 設定
                    if (string.IsNullOrEmpty(settings.FoodCollectionId))
                    {
                        settings.FoodCollectionId = "food";
                        Console.WriteLine("✅ 設定預設 Food Table ID: food");
                    }
                    
                    if (string.IsNullOrEmpty(settings.SubscriptionCollectionId))
                    {
                        settings.SubscriptionCollectionId = "subscription";
                        Console.WriteLine("✅ 設定預設 Subscription Table ID: subscription");
                    }
                    
                    // 保存設定
                    settings.Save();
                    Console.WriteLine("✅ Table ID 設定已保存");
                }
                else
                {
                    Console.WriteLine($"ℹ️ 當前使用 {settings.GetServiceDisplayName()}，Table ID 設定不適用");
                }
                
                // 測試設定檔載入
                Console.WriteLine("\n--- 測試設定檔重新載入 ---");
                AppSettings.ReloadSettings();
                var reloadedSettings = AppSettings.Instance;
                
                Console.WriteLine($"重新載入後的後端服務: {reloadedSettings.BackendService}");
                Console.WriteLine($"重新載入後的 Food Collection ID: {reloadedSettings.FoodCollectionId}");
                Console.WriteLine($"重新載入後的 Subscription Collection ID: {reloadedSettings.SubscriptionCollectionId}");
                
                // 驗證資料一致性
                if (settings.BackendService == reloadedSettings.BackendService &&
                    settings.FoodCollectionId == reloadedSettings.FoodCollectionId &&
                    settings.SubscriptionCollectionId == reloadedSettings.SubscriptionCollectionId)
                {
                    Console.WriteLine("✅ 設定檔載入和保存功能正常");
                }
                else
                {
                    Console.WriteLine("❌ 設定檔載入和保存功能異常");
                }
                
                Console.WriteLine("\n=== 可摺疊設定功能測試完成 ===");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ 測試過程中發生錯誤: {ex.Message}");
                Console.WriteLine($"詳細錯誤: {ex}");
            }
        }
        
        public static void ShowCollapsibleGuide()
        {
            var message = @"
🎯 可摺疊設定功能使用指南

📋 功能說明:
• 後端服務設定區塊可以摺疊/展開
• 連線設定區塊可以摺疊/展開
• Appwrite 服務包含 Table ID 設定

🖱️ 操作方式:
1. 點擊區塊標題來摺疊/展開內容
2. 箭頭圖示顯示當前狀態（▼ 展開 / ▶ 收合）
3. 選擇 Appwrite 時會顯示額外的 Table ID 欄位

⚙️ Appwrite Table ID 設定:
• Food Table ID: 指定食品資料的 Collection ID
• Subscription Table ID: 指定訂閱資料的 Collection ID
• 預設值: 'food' 和 'subscription'

💡 使用建議:
• 收合不常用的設定區塊以保持介面整潔
• 確保 Appwrite Table ID 與實際的 Collection 名稱一致
• 設定變更後記得點擊「儲存設定」按鈕

🔧 故障排除:
• 如果摺疊功能無法正常工作，請重新載入頁面
• 如果 Table ID 設定未顯示，請確認已選擇 Appwrite 服務
• 如果設定未保存，請檢查是否有權限寫入設定檔
";
            
            MessageBox.Show(message, "可摺疊設定功能指南", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        
        public static async Task TestAppwriteTableIdConfiguration()
        {
            Console.WriteLine("=== 測試 Appwrite Table ID 設定 ===");
            
            try
            {
                var settings = AppSettings.Instance;
                
                // 暫時切換到 Appwrite 進行測試
                var originalService = settings.BackendService;
                settings.BackendService = BackendServiceType.Appwrite;
                
                Console.WriteLine("✅ 切換到 Appwrite 後端服務");
                
                // 測試不同的 Table ID 設定
                var testConfigs = new[]
                {
                    new { Food = "food", Subscription = "subscription" },
                    new { Food = "foods", Subscription = "subscriptions" },
                    new { Food = "food_items", Subscription = "subscription_items" }
                };
                
                foreach (var config in testConfigs)
                {
                    settings.FoodCollectionId = config.Food;
                    settings.SubscriptionCollectionId = config.Subscription;
                    
                    Console.WriteLine($"測試設定 - Food: {config.Food}, Subscription: {config.Subscription}");
                    
                    // 驗證設定是否正確應用
                    if (settings.FoodCollectionId == config.Food && 
                        settings.SubscriptionCollectionId == config.Subscription)
                    {
                        Console.WriteLine("✅ Table ID 設定應用成功");
                    }
                    else
                    {
                        Console.WriteLine("❌ Table ID 設定應用失敗");
                    }
                    
                    await Task.Delay(100); // 短暫延遲
                }
                
                // 恢復原始設定
                settings.BackendService = originalService;
                settings.FoodCollectionId = "food";
                settings.SubscriptionCollectionId = "subscription";
                settings.Save();
                
                Console.WriteLine($"✅ 恢復原始後端服務: {originalService}");
                Console.WriteLine("=== Appwrite Table ID 設定測試完成 ===");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Appwrite Table ID 測試失敗: {ex.Message}");
            }
        }
    }
}