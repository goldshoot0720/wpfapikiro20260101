using System;
using System.Threading.Tasks;
using wpfkiro20260101.Models;
using wpfkiro20260101.Services;

namespace wpfkiro20260101
{
    /// <summary>
    /// 測試食品創建功能的修復
    /// </summary>
    public class TestFoodCreation
    {
        public static async Task RunTestAsync()
        {
            Console.WriteLine("=== 測試食品創建功能修復 ===");
            
            try
            {
                // 創建 CrudManager
                var crudManager = BackendServiceFactory.CreateCrudManager();
                Console.WriteLine($"使用後端服務: {crudManager.GetServiceName()}");
                
                // 測試連線
                Console.WriteLine("\n1. 測試連線...");
                var connectionTest = await crudManager.TestConnectionAsync();
                Console.WriteLine($"連線結果: {(connectionTest ? "✅ 成功" : "❌ 失敗")}");
                
                if (!connectionTest)
                {
                    Console.WriteLine("連線失敗，請檢查後端服務設定");
                    return;
                }
                
                // 創建包含所有欄位的測試食品
                Console.WriteLine("\n2. 創建測試食品（包含所有欄位）...");
                var testFood = new Food
                {
                    Id = Guid.NewGuid().ToString(),
                    FoodName = "測試食品 - 完整欄位",
                    Price = 99,
                    Shop = "測試商店",
                    ToDate = "2026-02-01",
                    Description = "這是一個測試食品，包含所有欄位",
                    Category = "測試分類",
                    StorageLocation = "測試儲存位置",
                    Note = "測試備註",
                    Photo = "test_food.jpg",
                    PhotoHash = "test123hash",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                
                Console.WriteLine($"食品資料:");
                Console.WriteLine($"  名稱: {testFood.FoodName}");
                Console.WriteLine($"  價格: {testFood.Price}");
                Console.WriteLine($"  商店: {testFood.Shop}");
                Console.WriteLine($"  到期日: {testFood.ToDate}");
                Console.WriteLine($"  描述: {testFood.Description}");
                Console.WriteLine($"  分類: {testFood.Category}");
                Console.WriteLine($"  儲存位置: {testFood.StorageLocation}");
                Console.WriteLine($"  備註: {testFood.Note}");
                
                // 嘗試創建食品
                var createResult = await crudManager.CreateFoodAsync(testFood);
                
                if (createResult.Success)
                {
                    Console.WriteLine("\n✅ 食品創建成功！");
                    Console.WriteLine($"創建結果: {createResult.Data}");
                    
                    // 讀取所有食品以驗證
                    Console.WriteLine("\n3. 驗證創建結果 - 讀取所有食品...");
                    var readResult = await crudManager.GetAllFoodsAsync();
                    
                    if (readResult.Success && readResult.Data != null)
                    {
                        Console.WriteLine($"✅ 讀取成功！找到 {readResult.Data.Length} 項食品");
                        
                        // 查找我們剛創建的食品
                        bool foundTestFood = false;
                        foreach (var item in readResult.Data)
                        {
                            // 使用反射檢查食品名稱
                            var nameProperty = item.GetType().GetProperty("foodName") ?? item.GetType().GetProperty("FoodName");
                            if (nameProperty?.GetValue(item)?.ToString() == testFood.FoodName)
                            {
                                foundTestFood = true;
                                Console.WriteLine($"✅ 找到測試食品: {item}");
                                break;
                            }
                        }
                        
                        if (!foundTestFood)
                        {
                            Console.WriteLine("⚠️ 未找到剛創建的測試食品");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"❌ 讀取食品失敗: {readResult.ErrorMessage}");
                    }
                }
                else
                {
                    Console.WriteLine($"\n❌ 食品創建失敗: {createResult.ErrorMessage}");
                }
                
                Console.WriteLine("\n=== 測試完成 ===");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n❌ 測試過程中發生錯誤: {ex.Message}");
                Console.WriteLine($"堆疊追蹤: {ex.StackTrace}");
            }
        }
    }
}