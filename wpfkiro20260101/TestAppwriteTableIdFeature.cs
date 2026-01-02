using System;

namespace wpfkiro20260101
{
    public class TestAppwriteTableIdFeature
    {
        public static void TestFeature()
        {
            Console.WriteLine("=== Appwrite Table ID 功能測試 ===");
            
            try
            {
                // 1. 檢查 AppSettings 是否包含新欄位
                var settings = AppSettings.Instance;
                
                Console.WriteLine("1. 檢查 AppSettings 新欄位:");
                Console.WriteLine($"   FoodCollectionId: {settings.FoodCollectionId}");
                Console.WriteLine($"   SubscriptionCollectionId: {settings.SubscriptionCollectionId}");
                
                // 2. 檢查預設值
                Console.WriteLine("\n2. 檢查預設值:");
                Console.WriteLine($"   預設 Food Collection ID: {AppSettings.Defaults.Appwrite.FoodCollectionId}");
                Console.WriteLine($"   預設 Subscription Collection ID: {AppSettings.Defaults.Appwrite.SubscriptionCollectionId}");
                
                // 3. 測試設定更新
                Console.WriteLine("\n3. 測試設定更新:");
                var originalFoodId = settings.FoodCollectionId;
                var originalSubId = settings.SubscriptionCollectionId;
                
                // 更新為測試值
                settings.FoodCollectionId = "test_food";
                settings.SubscriptionCollectionId = "test_subscription";
                settings.Save();
                
                Console.WriteLine("   已更新設定為測試值");
                Console.WriteLine($"   新 Food Collection ID: {settings.FoodCollectionId}");
                Console.WriteLine($"   新 Subscription Collection ID: {settings.SubscriptionCollectionId}");
                
                // 4. 測試設定重新載入
                Console.WriteLine("\n4. 測試設定重新載入:");
                AppSettings.ReloadSettings();
                var reloadedSettings = AppSettings.Instance;
                
                Console.WriteLine($"   重新載入後 Food Collection ID: {reloadedSettings.FoodCollectionId}");
                Console.WriteLine($"   重新載入後 Subscription Collection ID: {reloadedSettings.SubscriptionCollectionId}");
                
                bool reloadSuccess = 
                    reloadedSettings.FoodCollectionId == "test_food" &&
                    reloadedSettings.SubscriptionCollectionId == "test_subscription";
                
                Console.WriteLine($"   重新載入測試: {(reloadSuccess ? "✅ 成功" : "❌ 失敗")}");
                
                // 5. 恢復原始設定
                Console.WriteLine("\n5. 恢復原始設定:");
                settings.FoodCollectionId = originalFoodId;
                settings.SubscriptionCollectionId = originalSubId;
                settings.Save();
                
                Console.WriteLine($"   已恢復 Food Collection ID: {settings.FoodCollectionId}");
                Console.WriteLine($"   已恢復 Subscription Collection ID: {settings.SubscriptionCollectionId}");
                
                // 6. 檢查 Appwrite 服務是否能正確使用新設定
                Console.WriteLine("\n6. 檢查 Appwrite 服務整合:");
                
                if (settings.BackendService == BackendServiceType.Appwrite)
                {
                    Console.WriteLine("   當前使用 Appwrite 服務");
                    Console.WriteLine($"   Food Collection ID: {settings.FoodCollectionId}");
                    Console.WriteLine($"   Subscription Collection ID: {settings.SubscriptionCollectionId}");
                }
                else
                {
                    Console.WriteLine($"   當前使用 {settings.BackendService} 服務");
                    Console.WriteLine("   切換到 Appwrite 以測試 Table ID 功能");
                }
                
                // 7. 功能使用指南
                Console.WriteLine("\n=== 功能使用指南 ===");
                Console.WriteLine("1. 進入「系統設定」頁面");
                Console.WriteLine("2. 選擇「Appwrite」選項");
                Console.WriteLine("3. 確認顯示以下欄位:");
                Console.WriteLine("   - Food Table ID (預設: food)");
                Console.WriteLine("   - Subscription Table ID (預設: subscription)");
                Console.WriteLine("4. 可自定義 Table ID 名稱");
                Console.WriteLine("5. 點擊「儲存設定」保存");
                
                Console.WriteLine("\n✅ 所有測試完成！");
                
            }
            catch (Exception ex)
            {
                Console.WriteLine($"測試過程中發生錯誤: {ex.Message}");
                Console.WriteLine($"詳細錯誤: {ex}");
            }
            
            Console.WriteLine("\n=== 測試結束 ===");
        }
        
        public static void TestUIBehavior()
        {
            Console.WriteLine("=== UI 行為測試指南 ===");
            
            Console.WriteLine("\n📋 測試步驟:");
            Console.WriteLine("1. 啟動應用程式");
            Console.WriteLine("2. 進入「系統設定」頁面");
            Console.WriteLine("3. 測試以下場景:");
            
            Console.WriteLine("\n🔍 場景 1: 選擇 Appwrite");
            Console.WriteLine("   - 點擊 Appwrite 選項");
            Console.WriteLine("   - 確認顯示以下欄位:");
            Console.WriteLine("     ✅ API Endpoint");
            Console.WriteLine("     ✅ Project ID");
            Console.WriteLine("     ✅ Database ID");
            Console.WriteLine("     ✅ Bucket ID");
            Console.WriteLine("     ✅ Food Table ID (新增)");
            Console.WriteLine("     ✅ Subscription Table ID (新增)");
            Console.WriteLine("     ✅ API Key");
            
            Console.WriteLine("\n🔍 場景 2: 選擇其他服務");
            Console.WriteLine("   - 點擊 Supabase 選項");
            Console.WriteLine("   - 確認隱藏 Appwrite 專用欄位:");
            Console.WriteLine("     ❌ Database ID (隱藏)");
            Console.WriteLine("     ❌ Bucket ID (隱藏)");
            Console.WriteLine("     ❌ Food Table ID (隱藏)");
            Console.WriteLine("     ❌ Subscription Table ID (隱藏)");
            
            Console.WriteLine("\n🔍 場景 3: 預設值測試");
            Console.WriteLine("   - 選擇 Appwrite");
            Console.WriteLine("   - 確認 Food Table ID 顯示: food");
            Console.WriteLine("   - 確認 Subscription Table ID 顯示: subscription");
            
            Console.WriteLine("\n🔍 場景 4: 自定義值測試");
            Console.WriteLine("   - 修改 Food Table ID 為: my_foods");
            Console.WriteLine("   - 修改 Subscription Table ID 為: my_subscriptions");
            Console.WriteLine("   - 點擊「儲存設定」");
            Console.WriteLine("   - 重新啟動應用程式");
            Console.WriteLine("   - 確認設定已保存");
            
            Console.WriteLine("\n✅ 預期結果:");
            Console.WriteLine("- Appwrite 專用欄位只在選擇 Appwrite 時顯示");
            Console.WriteLine("- 新的 Table ID 欄位正確顯示和隱藏");
            Console.WriteLine("- 預設值自動填入");
            Console.WriteLine("- 自定義值能正確儲存和載入");
            
            Console.WriteLine("\n=== UI 測試指南結束 ===");
        }
    }
}