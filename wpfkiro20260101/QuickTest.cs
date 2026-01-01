using System;
using System.Threading.Tasks;
using wpfkiro20260101.Models;
using wpfkiro20260101.Services;

namespace wpfkiro20260101
{
    public static class QuickTest
    {
        public static async Task TestAppwriteFoodCreation()
        {
            Console.WriteLine("=== 快速测试 Appwrite 食品创建 ===");
            
            try
            {
                // 创建 Appwrite 服务
                var appwriteService = new AppwriteService();
                
                // 测试连接
                Console.WriteLine("测试 Appwrite 连接...");
                var connectionTest = await appwriteService.TestConnectionAsync();
                Console.WriteLine($"连接测试结果: {(connectionTest ? "✅ 成功" : "❌ 失败")}");
                
                if (!connectionTest)
                {
                    Console.WriteLine("❌ 无法连接到 Appwrite，请检查设置");
                    return;
                }
                
                // 创建测试食品
                var testFood = new Food
                {
                    FoodName = "测试食品_" + DateTime.Now.ToString("HHmmss"),
                    Price = 50,
                    Photo = "",
                    PhotoHash = "",
                    Shop = "测试商店",
                    ToDate = "2024-12-31",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                Console.WriteLine($"准备创建食品: {testFood.FoodName}");
                Console.WriteLine("字段映射:");
                Console.WriteLine($"  FoodName: '{testFood.FoodName}' → name");
                Console.WriteLine($"  Price: {testFood.Price} → price");
                Console.WriteLine($"  Shop: '{testFood.Shop}' → shop");
                Console.WriteLine($"  ToDate: '{testFood.ToDate}' → todate");
                
                // 调用创建方法
                var result = await appwriteService.CreateFoodAsync(testFood);
                
                if (result.Success)
                {
                    Console.WriteLine("✅ 食品创建成功！");
                    Console.WriteLine($"结果: {result.Data}");
                }
                else
                {
                    Console.WriteLine($"❌ 食品创建失败: {result.ErrorMessage}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ 测试过程中发生错误: {ex.Message}");
                Console.WriteLine($"堆栈跟踪: {ex.StackTrace}");
            }
        }
    }
}