using System;
using System.Linq;
using System.Threading.Tasks;
using wpfkiro20260101.Models;
using wpfkiro20260101.Services;

namespace wpfkiro20260101
{
    public class TestNetworkImages
    {
        public static async Task TestImageUrls()
        {
            Console.WriteLine("=== 測試網路圖片功能 ===");

            // 測試用的圖片 URL（這些是常見的測試圖片）
            var testImageUrls = new[]
            {
                "https://picsum.photos/300/200?random=1", // Lorem Picsum 隨機圖片
                "https://via.placeholder.com/300x200/FF0000/FFFFFF?text=Food1", // Placeholder 圖片
                "https://images.unsplash.com/photo-1567620905732-2d1ec7ab7445?w=300&h=200&fit=crop", // Unsplash 食物圖片
                "https://httpbin.org/image/jpeg", // HTTPBin 測試圖片
                "invalid-url", // 無效 URL 測試
                "" // 空 URL 測試
            };

            foreach (var imageUrl in testImageUrls)
            {
                Console.WriteLine($"\n測試圖片 URL: {(string.IsNullOrEmpty(imageUrl) ? "(空)" : imageUrl)}");
                
                try
                {
                    // 創建測試食品
                    var testFood = new Food
                    {
                        Id = Guid.NewGuid().ToString(),
                        FoodName = $"測試食品 - {DateTime.Now:HH:mm:ss}",
                        Photo = imageUrl,
                        Price = 100,
                        Quantity = 1,
                        Shop = "測試商店",
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };

                    Console.WriteLine($"  - 食品名稱: {testFood.FoodName}");
                    Console.WriteLine($"  - 圖片 URL: {testFood.Photo}");
                    Console.WriteLine($"  - URL 有效性: {IsValidImageUrl(testFood.Photo)}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"  - 錯誤: {ex.Message}");
                }
            }

            Console.WriteLine("\n=== 測試完成 ===");
        }

        private static bool IsValidImageUrl(string url)
        {
            if (string.IsNullOrEmpty(url)) return false;
            
            try
            {
                var uri = new Uri(url);
                if (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps)
                    return false;

                var imageExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp" };
                var lowerUrl = url.ToLower();
                return imageExtensions.Any(ext => lowerUrl.Contains(ext)) || 
                       url.Contains("picsum.photos") || 
                       url.Contains("placeholder.com") || 
                       url.Contains("unsplash.com") ||
                       url.Contains("httpbin.org/image");
            }
            catch
            {
                return false;
            }
        }
    }
}