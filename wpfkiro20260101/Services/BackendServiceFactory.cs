using System;

namespace wpfkiro20260101.Services
{
    public static class BackendServiceFactory
    {
        /// <summary>
        /// 根據設定創建對應的後端服務
        /// </summary>
        /// <param name="serviceType">後端服務類型</param>
        /// <returns>IBackendService 實例</returns>
        public static IBackendService CreateService(BackendServiceType serviceType)
        {
            return serviceType switch
            {
                BackendServiceType.Appwrite => new AppwriteService(),
                BackendServiceType.Supabase => new SupabaseService(),
                BackendServiceType.Back4App => new Back4AppService(),
                BackendServiceType.MySQL => new MySQLService(),
                BackendServiceType.NHost => new NHostService(),
                BackendServiceType.Contentful => throw new NotImplementedException("Contentful 服務尚未實作"),
                _ => throw new ArgumentException($"不支援的後端服務類型：{serviceType}")
            };
        }

        /// <summary>
        /// 根據當前應用程式設定創建後端服務
        /// </summary>
        /// <returns>IBackendService 實例</returns>
        public static IBackendService CreateCurrentService()
        {
            var settings = AppSettings.Instance;
            return CreateService(settings.BackendService);
        }

        /// <summary>
        /// 創建 CRUD 管理器，使用當前設定的後端服務
        /// </summary>
        /// <returns>CrudManager 實例</returns>
        public static CrudManager CreateCrudManager()
        {
            var service = CreateCurrentService();
            return new CrudManager(service);
        }

        /// <summary>
        /// 創建 CRUD 管理器，使用指定的後端服務類型
        /// </summary>
        /// <param name="serviceType">後端服務類型</param>
        /// <returns>CrudManager 實例</returns>
        public static CrudManager CreateCrudManager(BackendServiceType serviceType)
        {
            var service = CreateService(serviceType);
            return new CrudManager(service);
        }

        /// <summary>
        /// 取得所有支援的後端服務類型
        /// </summary>
        /// <returns>支援的服務類型陣列</returns>
        public static BackendServiceType[] GetSupportedServices()
        {
            return new[]
            {
                BackendServiceType.Appwrite,
                BackendServiceType.Supabase,
                BackendServiceType.Back4App,
                BackendServiceType.MySQL,
                BackendServiceType.NHost
            };
        }

        /// <summary>
        /// 檢查指定的服務類型是否受支援
        /// </summary>
        /// <param name="serviceType">要檢查的服務類型</param>
        /// <returns>是否受支援</returns>
        public static bool IsServiceSupported(BackendServiceType serviceType)
        {
            return serviceType switch
            {
                BackendServiceType.Appwrite or 
                BackendServiceType.Supabase or 
                BackendServiceType.Back4App or 
                BackendServiceType.MySQL or
                BackendServiceType.NHost => true,
                _ => false
            };
        }
    }
}