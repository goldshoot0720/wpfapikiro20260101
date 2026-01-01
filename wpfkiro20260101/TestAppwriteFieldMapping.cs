using System;
using System.Threading.Tasks;
using wpfkiro20260101.Models;
using wpfkiro20260101.Services;

namespace wpfkiro20260101
{
    public class TestAppwriteFieldMapping
    {
        public static async Task TestCreateFood()
        {
            try
            {
                Console.WriteLine("=== 測試 Appwrite 食品創建 ===");
                
                var appwriteService = new AppwriteService();
                
                // 創建測試食品
                var testFood = new Food
                {
                    FoodName = "測試食品",
                    Price = 100,
                    Photo = "test.jpg",
                    PhotoHash = "testhash123",
                    Shop = "測試商店",
                    ToDate = "2024-12-31",
                    Description = "測試描述",
                    Category = "測試分類",
                    StorageLocation = "冰箱",
                    Note = "測試備註",
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };

                Console.WriteLine($"準備創建食品: {testFood.FoodName}");
                
                var result = await appwriteService.CreateFoodAsync(testFood);
                
                if (result.Success)
                {
                    Console.WriteLine("✅ 食品創建成功！");
                    Console.WriteLine($"結果: {result.Data}");
                }
                else
                {
                    Console.WriteLine($"❌ 食品創建失敗: {result.ErrorMessage}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ 測試過程中發生錯誤: {ex.Message}");
            }
        }

        public static async Task TestGetFoods()
        {
            try
            {
                Console.WriteLine("\n=== 測試 Appwrite 食品載入 ===");
                
                var appwriteService = new AppwriteService();
                
                var result = await appwriteService.GetFoodsAsync();
                
                if (result.Success)
                {
                    Console.WriteLine($"✅ 食品載入成功！共 {result.Data?.Length ?? 0} 項");
                    
                    if (result.Data != null && result.Data.Length > 0)
                    {
                        Console.WriteLine("前幾項食品:");
                        for (int i = 0; i < Math.Min(3, result.Data.Length); i++)
                        {
                            Console.WriteLine($"  - {result.Data[i]}");
                        }
                    }
                }
                else
                {
                    Console.WriteLine($"❌ 食品載入失敗: {result.ErrorMessage}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ 測試過程中發生錯誤: {ex.Message}");
            }
        }
    }
}