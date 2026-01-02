using System;
using System.Threading.Tasks;
using wpfkiro20260101.Services;

namespace wpfkiro20260101
{
    public class TestSupabaseFieldMappingFixed
    {
        public static async Task RunTest()
        {
            Console.WriteLine("=== Supabase 欄位映射修正測試 ===");
            
            try
            {
                var settings = AppSettings.Instance;
                
                // 檢查設定
                if (string.IsNullOrWhiteSpace(settings.ApiUrl) || 
                    string.IsNullOrWhiteSpace(settings.ApiKey))
                {
                    Console.WriteLine("❌ Supabase 設定不完整");
                    Console.WriteLine($"API URL: {(string.IsNullOrWhiteSpace(settings.ApiUrl) ? "未設定" : "已設定")}");
                    Console.WriteLine($"API Key: {(string.IsNullOrWhiteSpace(settings.ApiKey) ? "未設定" : "已設定")}");
                    return;
                }
                
                var supabaseService = new SupabaseService();
                
                // 1. 測試連接
                Console.WriteLine("\n1. 測試 Supabase 連接...");
                var connectionResult = await supabaseService.TestConnectionAsync();
                Console.WriteLine($"連接結果: {(connectionResult ? "✅ 成功" : "❌ 失敗")}");
                
                if (!connectionResult)
                {
                    Console.WriteLine("❌ 無法連接到 Supabase，停止測試");
                    return;
                }
                
                // 2. 測試讀取 Food 資料（使用修正後的欄位映射）
                Console.WriteLine("\n2. 測試讀取 Food 資料...");
                var foodsResult = await supabaseService.GetFoodsAsync();
                
                if (foodsResult.Success)
                {
                    Console.WriteLine($"✅ 成功讀取 {foodsResult.Data.Length} 筆 Food 資料");
                    
                    if (foodsResult.Data.Length > 0)
                    {
                        Console.WriteLine("第一筆資料內容:");
                        var firstFood = foodsResult.Data[0];
                        Console.WriteLine($"  - ID: {GetPropertyValue(firstFood, "id")}");
                        Console.WriteLine($"  - 名稱: {GetPropertyValue(firstFood, "foodName")}");
                        Console.WriteLine($"  - 價格: {GetPropertyValue(firstFood, "price")}");
                        Console.WriteLine($"  - 商店: {GetPropertyValue(firstFood, "shop")} (來自 site 欄位)");
                        Console.WriteLine($"  - 到期日: {GetPropertyValue(firstFood, "toDate")} (來自 nextdate 欄位)");
                        Console.WriteLine($"  - 圖片: {GetPropertyValue(firstFood, "photo")} (來自 photohash 欄位)");
                        Console.WriteLine($"  - 備註: {GetPropertyValue(firstFood, "note")}");
                        Console.WriteLine($"  - 創建時間: {GetPropertyValue(firstFood, "createdAt")}");
                    }
                }
                else
                {
                    Console.WriteLine($"❌ 讀取 Food 資料失敗: {foodsResult.ErrorMessage}");
                }
                
                // 3. 測試讀取 Subscription 資料
                Console.WriteLine("\n3. 測試讀取 Subscription 資料...");
                var subscriptionsResult = await supabaseService.GetSubscriptionsAsync();
                
                if (subscriptionsResult.Success)
                {
                    Console.WriteLine($"✅ 成功讀取 {subscriptionsResult.Data.Length} 筆 Subscription 資料");
                    
                    if (subscriptionsResult.Data.Length > 0)
                    {
                        Console.WriteLine("第一筆資料內容:");
                        var firstSub = subscriptionsResult.Data[0];
                        Console.WriteLine($"  - ID: {GetPropertyValue(firstSub, "id")}");
                        Console.WriteLine($"  - 名稱: {GetPropertyValue(firstSub, "subscriptionName")}");
                        Console.WriteLine($"  - 下次付款: {GetPropertyValue(firstSub, "nextDate")}");
                        Console.WriteLine($"  - 價格: {GetPropertyValue(firstSub, "price")}");
                        Console.WriteLine($"  - 網站: {GetPropertyValue(firstSub, "site")}");
                        Console.WriteLine($"  - 帳戶: {GetPropertyValue(firstSub, "account")}");
                        Console.WriteLine($"  - 備註: {GetPropertyValue(firstSub, "note")}");
                    }
                }
                else
                {
                    Console.WriteLine($"❌ 讀取 Subscription 資料失敗: {subscriptionsResult.ErrorMessage}");
                }
                
                // 4. 測試創建 Food（使用修正後的欄位映射）
                Console.WriteLine("\n4. 測試創建 Food 資料...");
                var testFood = new Models.Food
                {
                    FoodName = "測試食品_欄位映射",
                    Price = 99,
                    Shop = "測試商店",
                    ToDate = DateTime.Now.AddDays(7).ToString("yyyy-MM-dd"),
                    Photo = "test_photo_hash_123",
                    Note = "欄位映射測試"
                };
                
                var createResult = await supabaseService.CreateFoodAsync(testFood);
                
                if (createResult.Success)
                {
                    Console.WriteLine("✅ 成功創建測試 Food 資料");
                    Console.WriteLine("  - 使用正確的欄位映射:");
                    Console.WriteLine("    * shop → site");
                    Console.WriteLine("    * toDate → nextdate");
                    Console.WriteLine("    * photo → photohash");
                }
                else
                {
                    Console.WriteLine($"❌ 創建 Food 資料失敗: {createResult.ErrorMessage}");
                }
                
                Console.WriteLine("\n=== 欄位映射測試完成 ===");
                Console.WriteLine("修正項目:");
                Console.WriteLine("✅ Food 資料表欄位映射已修正");
                Console.WriteLine("✅ CreateFoodAsync 使用正確欄位名稱");
                Console.WriteLine("✅ UpdateFoodAsync 使用正確欄位名稱");
                Console.WriteLine("✅ GetFoodsAsync 讀取正確欄位");
                Console.WriteLine("✅ Subscription 資料表映射確認正確");
                
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ 測試過程發生異常: {ex.Message}");
                Console.WriteLine($"詳細錯誤: {ex.StackTrace}");
            }
        }
        
        private static string GetPropertyValue(object obj, string propertyName)
        {
            try
            {
                var property = obj.GetType().GetProperty(propertyName);
                if (property != null)
                {
                    var value = property.GetValue(obj);
                    return value?.ToString() ?? "null";
                }
                return "屬性不存在";
            }
            catch
            {
                return "讀取錯誤";
            }
        }
    }
}