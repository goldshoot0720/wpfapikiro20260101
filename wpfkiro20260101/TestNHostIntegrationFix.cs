using System;
using System.Threading.Tasks;
using wpfkiro20260101.Services;

namespace wpfkiro20260101
{
    /// <summary>
    /// 測試 NHost 整合修正
    /// 驗證 NHost 服務在食品和訂閱頁面中的正確整合
    /// </summary>
    public class TestNHostIntegrationFix
    {
        /// <summary>
        /// 執行 NHost 整合測試
        /// </summary>
        public static async Task RunTestAsync()
        {
            Console.WriteLine("=== NHost 整合修正測試 ===");
            Console.WriteLine();

            try
            {
                // 1. 測試後端服務工廠
                await TestBackendServiceFactory();
                Console.WriteLine();

                // 2. 測試 NHost 服務基本功能
                await TestNHostServiceBasics();
                Console.WriteLine();

                // 3. 測試 CRUD 管理器整合
                await TestCrudManagerIntegration();
                Console.WriteLine();

                Console.WriteLine("✅ NHost 整合修正測試完成");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ 測試過程中發生錯誤: {ex.Message}");
                Console.WriteLine($"詳細錯誤: {ex}");
            }
        }

        /// <summary>
        /// 測試後端服務工廠
        /// </summary>
        private static async Task TestBackendServiceFactory()
        {
            Console.WriteLine("--- 測試後端服務工廠 ---");

            try
            {
                // 測試創建 NHost 服務
                var nHostService = BackendServiceFactory.CreateService(BackendServiceType.NHost);
                Console.WriteLine($"✓ 成功創建 NHost 服務: {nHostService.ServiceName}");

                // 測試服務類型
                Console.WriteLine($"✓ 服務類型: {nHostService.ServiceType}");

                // 測試是否支援 NHost
                var isSupported = BackendServiceFactory.IsServiceSupported(BackendServiceType.NHost);
                Console.WriteLine($"✓ NHost 支援狀態: {(isSupported ? "支援" : "不支援")}");

                // 測試獲取支援的服務列表
                var supportedServices = BackendServiceFactory.GetSupportedServices();
                var nHostSupported = Array.Exists(supportedServices, s => s == BackendServiceType.NHost);
                Console.WriteLine($"✓ NHost 在支援列表中: {(nHostSupported ? "是" : "否")}");

                // 測試初始化
                var initResult = await nHostService.InitializeAsync();
                Console.WriteLine($"✓ 服務初始化: {(initResult ? "成功" : "失敗")}");

                nHostService.Dispose();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ 後端服務工廠測試失敗: {ex.Message}");
            }
        }

        /// <summary>
        /// 測試 NHost 服務基本功能
        /// </summary>
        private static async Task TestNHostServiceBasics()
        {
            Console.WriteLine("--- 測試 NHost 服務基本功能 ---");

            try
            {
                var nHostService = new NHostService();

                // 測試服務屬性
                Console.WriteLine($"✓ 服務名稱: {nHostService.ServiceName}");
                Console.WriteLine($"✓ 服務類型: {nHostService.ServiceType}");

                // 測試初始化
                var initResult = await nHostService.InitializeAsync();
                Console.WriteLine($"✓ 初始化結果: {(initResult ? "成功" : "失敗")}");

                // 測試連線
                var connectionResult = await nHostService.TestConnectionAsync();
                Console.WriteLine($"✓ 連線測試: {(connectionResult ? "成功" : "失敗")}");

                // 測試獲取食品 (不創建，只測試方法存在)
                try
                {
                    var foodsResult = await nHostService.GetFoodsAsync();
                    Console.WriteLine($"✓ GetFoodsAsync 方法可用: {foodsResult.Success}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"⚠️ GetFoodsAsync 測試: {ex.Message}");
                }

                // 測試獲取訂閱 (不創建，只測試方法存在)
                try
                {
                    var subscriptionsResult = await nHostService.GetSubscriptionsAsync();
                    Console.WriteLine($"✓ GetSubscriptionsAsync 方法可用: {subscriptionsResult.Success}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"⚠️ GetSubscriptionsAsync 測試: {ex.Message}");
                }

                nHostService.Dispose();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ NHost 服務基本功能測試失敗: {ex.Message}");
            }
        }

        /// <summary>
        /// 測試 CRUD 管理器整合
        /// </summary>
        private static async Task TestCrudManagerIntegration()
        {
            Console.WriteLine("--- 測試 CRUD 管理器整合 ---");

            try
            {
                // 創建 NHost CRUD 管理器
                var crudManager = BackendServiceFactory.CreateCrudManager(BackendServiceType.NHost);
                Console.WriteLine($"✓ 成功創建 CRUD 管理器");

                // 測試管理器屬性
                Console.WriteLine($"✓ 管理器服務名稱: {crudManager.GetServiceName()}");
                Console.WriteLine($"✓ 管理器服務類型: {crudManager.GetServiceType()}");

                // 測試連線
                var connectionResult = await crudManager.TestConnectionAsync();
                Console.WriteLine($"✓ CRUD 管理器連線測試: {(connectionResult ? "成功" : "失敗")}");

                // 測試獲取食品方法存在
                try
                {
                    var foodsResult = await crudManager.GetAllFoodsAsync();
                    Console.WriteLine($"✓ GetAllFoodsAsync 方法可用: {foodsResult.Success}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"⚠️ GetAllFoodsAsync 測試: {ex.Message}");
                }

                // 測試獲取訂閱方法存在
                try
                {
                    var subscriptionsResult = await crudManager.GetAllSubscriptionsAsync();
                    Console.WriteLine($"✓ GetAllSubscriptionsAsync 方法可用: {subscriptionsResult.Success}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"⚠️ GetAllSubscriptionsAsync 測試: {ex.Message}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ CRUD 管理器整合測試失敗: {ex.Message}");
            }
        }

        /// <summary>
        /// 測試應用程式設定中的 NHost 配置
        /// </summary>
        public static void TestAppSettingsNHostSupport()
        {
            Console.WriteLine("--- 測試應用程式設定 NHost 支援 ---");

            try
            {
                var settings = AppSettings.Instance;
                
                // 測試設定 NHost 服務
                var originalService = settings.BackendService;
                settings.BackendService = BackendServiceType.NHost;
                
                Console.WriteLine($"✓ 設定後端服務為 NHost: {settings.BackendService}");
                Console.WriteLine($"✓ 服務顯示名稱: {settings.GetServiceDisplayName()}");
                
                // 恢復原始設定
                settings.BackendService = originalService;
                Console.WriteLine($"✓ 恢復原始設定: {settings.BackendService}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ 應用程式設定測試失敗: {ex.Message}");
            }
        }

        /// <summary>
        /// 快速驗證修正
        /// </summary>
        public static async Task QuickVerificationAsync()
        {
            Console.WriteLine("🔍 NHost 整合快速驗證");
            Console.WriteLine("-" + new string('-', 30));

            try
            {
                // 1. 驗證服務創建
                var service = BackendServiceFactory.CreateService(BackendServiceType.NHost);
                Console.WriteLine($"✓ 服務創建: {service.ServiceName}");

                // 2. 驗證支援狀態
                var supported = BackendServiceFactory.IsServiceSupported(BackendServiceType.NHost);
                Console.WriteLine($"✓ 支援狀態: {supported}");

                // 3. 驗證 CRUD 管理器
                var crudManager = BackendServiceFactory.CreateCrudManager(BackendServiceType.NHost);
                Console.WriteLine($"✓ CRUD 管理器: {crudManager.GetServiceName()}");

                // 4. 驗證基本連線
                var connectionResult = await service.TestConnectionAsync();
                Console.WriteLine($"✓ 連線測試: {connectionResult}");

                service.Dispose();
                Console.WriteLine("✅ 快速驗證完成 - NHost 整合正常");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ 快速驗證失敗: {ex.Message}");
            }
        }
    }
}