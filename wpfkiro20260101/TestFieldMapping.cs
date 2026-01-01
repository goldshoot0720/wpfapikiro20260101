using System;
using System.Collections.Generic;
using wpfkiro20260101.Models;

namespace wpfkiro20260101
{
    public static class TestFieldMapping
    {
        public static void TestFoodFieldMapping()
        {
            Console.WriteLine("=== 測試 Food 字段映射 ===");
            
            var food = new Food
            {
                FoodName = "測試食品",
                Price = 100,
                Photo = "test.jpg",
                PhotoHash = "hash123",
                Shop = "測試商店",
                ToDate = "2024-12-31"
            };

            // 模擬 Appwrite 字段映射
            var appwriteData = new Dictionary<string, object>
            {
                ["name"] = food.FoodName,        // 實際欄位: "name"
                ["price"] = food.Price,          // 實際欄位: "price"
                ["photo"] = food.Photo,          // 實際欄位: "photo"
                ["photohash"] = food.PhotoHash,  // 實際欄位: "photohash"
                ["shop"] = food.Shop,            // 實際欄位: "shop"
                ["todate"] = food.ToDate         // 實際欄位: "todate"
            };

            Console.WriteLine("Food 模型屬性 -> Appwrite 字段映射:");
            Console.WriteLine($"FoodName: '{food.FoodName}' -> name: '{appwriteData["name"]}'");
            Console.WriteLine($"Price: {food.Price} -> price: {appwriteData["price"]}");
            Console.WriteLine($"Photo: '{food.Photo}' -> photo: '{appwriteData["photo"]}'");
            Console.WriteLine($"PhotoHash: '{food.PhotoHash}' -> photohash: '{appwriteData["photohash"]}'");
            Console.WriteLine($"Shop: '{food.Shop}' -> shop: '{appwriteData["shop"]}'");
            Console.WriteLine($"ToDate: '{food.ToDate}' -> todate: '{appwriteData["todate"]}'");

            // 檢查必需字段
            bool hasRequiredName = appwriteData.ContainsKey("name") && !string.IsNullOrEmpty(appwriteData["name"].ToString());
            Console.WriteLine($"\n必需字段 'name' 檢查: {(hasRequiredName ? "✅ 通過" : "❌ 失敗")}");
            
            if (!hasRequiredName)
            {
                Console.WriteLine("❌ 錯誤: 缺少必需的屬性 'name'");
            }
        }

        public static void TestSubscriptionFieldMapping()
        {
            Console.WriteLine("\n=== 測試 Subscription 字段映射 ===");
            
            var subscription = new Subscription
            {
                SubscriptionName = "測試訂閱",
                NextDate = DateTime.Now.AddDays(30),
                Price = 99,
                Site = "https://example.com",
                Account = "test@example.com",
                Note = "測試備註"
            };

            // 模擬 Appwrite 字段映射
            var appwriteData = new Dictionary<string, object>
            {
                ["name"] = subscription.SubscriptionName,      // 實際欄位: "name"
                ["nextdate"] = subscription.NextDate.ToString("yyyy-MM-dd"),  // 實際欄位: "nextdate"
                ["price"] = subscription.Price,               // 實際欄位: "price"
                ["site"] = subscription.Site,                 // 實際欄位: "site"
                ["account"] = subscription.Account,           // 實際欄位: "account"
                ["note"] = subscription.Note                  // 實際欄位: "note"
            };

            Console.WriteLine("Subscription 模型屬性 -> Appwrite 字段映射:");
            Console.WriteLine($"SubscriptionName: '{subscription.SubscriptionName}' -> name: '{appwriteData["name"]}'");
            Console.WriteLine($"NextDate: {subscription.NextDate:yyyy-MM-dd} -> nextdate: '{appwriteData["nextdate"]}'");
            Console.WriteLine($"Price: {subscription.Price} -> price: {appwriteData["price"]}");
            Console.WriteLine($"Site: '{subscription.Site}' -> site: '{appwriteData["site"]}'");
            Console.WriteLine($"Account: '{subscription.Account}' -> account: '{appwriteData["account"]}'");
            Console.WriteLine($"Note: '{subscription.Note}' -> note: '{appwriteData["note"]}'");

            // 檢查必需字段
            bool hasRequiredName = appwriteData.ContainsKey("name") && !string.IsNullOrEmpty(appwriteData["name"].ToString());
            Console.WriteLine($"\n必需字段 'name' 檢查: {(hasRequiredName ? "✅ 通過" : "❌ 失敗")}");
        }
    }
}