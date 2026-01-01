using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace wpfkiro20260101
{
    public class TestGoogleImages
    {
        public static async Task TestImageUrlValidation()
        {
            Console.WriteLine("=== 測試圖片 URL 驗證功能 ===");

            var testUrls = new[]
            {
                // Google 圖片 URL（您提供的）
                "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcTl6UCwjaicHkv5XB5Zus8QkIoFrEz0EIiNCw&s",
                
                // 其他常見的圖片服務
                "https://picsum.photos/300/200",
                "https://via.placeholder.com/300x200",
                "https://images.unsplash.com/photo-1567620905732-2d1ec7ab7445",
                "https://i.imgur.com/example.jpg",
                
                // 傳統的圖片 URL
                "https://example.com/image.jpg",
                "https://example.com/photo.png",
                
                // 無效的 URL
                "https://example.com/document.pdf",
                "ftp://example.com/image.jpg",
                "not-a-url"
            };

            foreach (var url in testUrls)
            {
                Console.WriteLine($"\n測試 URL: {url}");
                Console.WriteLine($"  驗證結果: {IsValidImageUrl(url)}");
                
                if (IsValidImageUrl(url))
                {
                    Console.WriteLine($"  ✅ 此 URL 被識別為有效的圖片 URL");
                    
                    // 嘗試實際載入圖片（僅測試連線）
                    try
                    {
                        using var httpClient = new HttpClient();
                        httpClient.Timeout = TimeSpan.FromSeconds(5);
                        var response = await httpClient.GetAsync(url);
                        Console.WriteLine($"  HTTP 狀態: {response.StatusCode}");
                        Console.WriteLine($"  內容類型: {response.Content.Headers.ContentType?.MediaType ?? "未知"}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"  ❌ 載入失敗: {ex.Message}");
                    }
                }
                else
                {
                    Console.WriteLine($"  ❌ 此 URL 不被識別為圖片 URL");
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
                
                // 檢查常見的圖片副檔名
                if (imageExtensions.Any(ext => lowerUrl.Contains(ext)))
                    return true;
                
                // 檢查特殊的圖片服務
                var imageServices = new[]
                {
                    "picsum.photos",
                    "placeholder.com", 
                    "unsplash.com",
                    "httpbin.org/image",
                    "gstatic.com/images", // Google 圖片
                    "googleusercontent.com",
                    "imgur.com",
                    "flickr.com",
                    "pixabay.com",
                    "pexels.com"
                };
                
                return imageServices.Any(service => lowerUrl.Contains(service));
            }
            catch
            {
                return false;
            }
        }

        // 測試 Google 圖片 URL 的特殊處理
        public static void TestGoogleImageUrlRecognition()
        {
            Console.WriteLine("=== 測試 Google 圖片 URL 識別 ===");
            
            var googleUrls = new[]
            {
                "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcTl6UCwjaicHkv5XB5Zus8QkIoFrEz0EIiNCw&s",
                "https://lh3.googleusercontent.com/example-image",
                "https://images.gstatic.com/example.jpg",
                "https://www.gstatic.com/images/branding/product/1x/photos_48dp.png"
            };

            foreach (var url in googleUrls)
            {
                Console.WriteLine($"URL: {url}");
                Console.WriteLine($"識別為圖片: {IsValidImageUrl(url)}");
                Console.WriteLine();
            }
        }
    }
}