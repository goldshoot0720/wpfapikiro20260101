using System;

namespace wpfkiro20260101.Services
{
    public static class BackendServiceFactory
    {
        public static IBackendService CreateService(BackendServiceType serviceType)
        {
            return serviceType switch
            {
                BackendServiceType.Appwrite => new AppwriteService(),
                BackendServiceType.Supabase => new SupabaseService(),
                BackendServiceType.NHost => new NHostService(),
                BackendServiceType.Contentful => new ContentfulService(),
                BackendServiceType.Back4App => new Back4AppService(),
                BackendServiceType.MySQL => new MySQLService(),
                _ => throw new ArgumentException($"不支援的後端服務類型: {serviceType}")
            };
        }

        public static IBackendService CreateCurrentService()
        {
            var settings = AppSettings.Instance;
            return CreateService(settings.BackendService);
        }
    }
}