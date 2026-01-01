using System;
using System.Linq;

namespace wpfkiro20260101
{
    public class TestYourGoogleImage
    {
        public static void TestGoogleImageUrl()
        {
            Console.WriteLine("=== 測試您提供的 Google 圖片 URL ===");
            
            var yourUrl = "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcTl6UCwjaicHkv5XB5Zus8QkIoFrEz0EIiNCw&s";
            
            Console.WriteLine($"URL: {yourUrl}");
            Console.WriteLine($"長度: {yourUrl.Length} 字元");
            Console.WriteLine($"包含 gstatic.com: {yourUrl.Contains("gstatic.com")}");
            Console.WriteLine($"包含 images: {yourUrl.Contains("images")}");
            Console.WriteLine($"是 HTTPS: {yourUrl.StartsWith("https://")}");
            
            // 測試我們的驗證邏輯
            var isValid = IsValidImageUrl(yourUrl);
            Console.WriteLine($"通過驗證: {isValid}");
            
            if (isValid)
            {
                Console.WriteLine("✅ 這個 Google 圖片 URL 會被系統接受！");
                Console.WriteLine("您可以在添加食品時使用這個 URL。");
            }
            else
            {
                Console.WriteLine("❌ 這個 URL 沒有通過驗證。");
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
    }
}