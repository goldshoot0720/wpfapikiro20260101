using System;
using System.Threading.Tasks;
using wpfkiro20260101.Services;

namespace wpfkiro20260101
{
    public class QuickSettingsTest
    {
        public static async Task RunQuickTest()
        {
            Console.WriteLine("=== 快速設定測試 ===");
            
            try
            {
                // 1. 檢查當前設定
                var settings = AppSettings.Instance;
                Console.WriteLine($"當前設定:");
                Console.WriteLine($"  後端服務: {settings.BackendService}");
                Console.WriteLine($"  API URL: {settings.ApiUrl}");
                Console.WriteLine($"  Project ID: {settings.ProjectId}");
                Console.WriteLine($"  API Key: {settings.ApiKey.Substring(0, Math.Min(30, settings.ApiKey.Length))}...");
                Console.WriteLine($"  Database ID: {settings.DatabaseId}");
                Console.WriteLine($"  Bucket ID: {settings.BucketId}");
                
                // 2. 創建服務並測試
                Console.WriteLine($"\n創建 {settings.BackendService} 服務...");
                var service = BackendServiceFactory.CreateCurrentService();
                Console.WriteLine($"服務名稱: {service.ServiceName}");
                
                // 3. 測試連線
                Console.WriteLine($"測試連線...");
                bool connectionResult = await service.TestConnectionAsync();
                Console.WriteLine($"連線結果: {(connectionResult ? "成功" : "失敗")}");
                
                // 4. 如果是 Appwrite，顯示詳細資訊
                if (service is AppwriteService)
                {
                    Console.WriteLine($"\nAppwrite 詳細資訊:");
                    Console.WriteLine($"  Endpoint: {settings.ApiUrl}");
                    Console.WriteLine($"  Project: {settings.ProjectId}");
                    Console.WriteLine($"  Database: {settings.DatabaseId}");
                    Console.WriteLine($"  Bucket: {settings.BucketId}");
                    Console.WriteLine($"  Food Collection: {settings.FoodCollectionId}");
                    Console.WriteLine($"  Subscription Collection: {settings.SubscriptionCollectionId}");
                }
                
                // 5. 嘗試初始化服務
                Console.WriteLine($"\n初始化服務...");
                bool initResult = await service.InitializeAsync();
                Console.WriteLine($"初始化結果: {(initResult ? "成功" : "失敗")}");
                
                Console.WriteLine($"\n=== 測試完成 ===");
                
                if (connectionResult && initResult)
                {
                    Console.WriteLine("✓ 設定正常，服務可以正常工作");
                }
                else
                {
                    Console.WriteLine("⚠ 設定可能有問題，請檢查連線資訊");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"測試過程中發生錯誤: {ex.Message}");
                Console.WriteLine($"錯誤詳情: {ex.StackTrace}");
            }
        }
        
        public static void ShowCurrentSettings()
        {
            var settings = AppSettings.Instance;
            Console.WriteLine("=== 當前設定摘要 ===");
            Console.WriteLine($"後端服務: {settings.BackendService}");
            Console.WriteLine($"API URL: {settings.ApiUrl}");
            Console.WriteLine($"Project ID: {settings.ProjectId}");
            Console.WriteLine($"設定是否完整: {settings.IsConfigured()}");
            Console.WriteLine($"服務顯示名稱: {settings.GetServiceDisplayName()}");
        }
    }
}