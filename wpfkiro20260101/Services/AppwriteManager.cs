using System;
using System.Threading.Tasks;
using wpfkiro20260101.Services;

namespace wpfkiro20260101.Services
{
    public class AppwriteManager
    {
        private static AppwriteManager? _instance;
        private static readonly object _lock = new object();
        private AppwriteService? _appwriteService;

        public static AppwriteManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new AppwriteManager();
                        }
                    }
                }
                return _instance;
            }
        }

        private AppwriteManager() { }

        public AppwriteService GetService()
        {
            if (_appwriteService == null)
            {
                _appwriteService = new AppwriteService();
            }
            return _appwriteService;
        }

        public async Task<bool> IsConfiguredAsync()
        {
            var settings = AppSettings.Instance;
            return !string.IsNullOrWhiteSpace(settings.ApiUrl) && 
                   !string.IsNullOrWhiteSpace(settings.ProjectId) &&
                   settings.BackendService == BackendServiceType.Appwrite;
        }

        public async Task<bool> InitializeAsync()
        {
            if (!await IsConfiguredAsync())
            {
                return false;
            }

            var service = GetService();
            return await service.InitializeAsync();
        }

        public async Task<bool> TestConnectionAsync()
        {
            if (!await IsConfiguredAsync())
            {
                return false;
            }

            var service = GetService();
            return await service.TestConnectionAsync();
        }

        // 便捷方法
        public async Task<BackendServiceResult<string>> CreateUserAsync(string email, string password, string name)
        {
            var service = GetService();
            return await service.CreateUserAsync(email, password, name);
        }

        public async Task<BackendServiceResult<object>> GetCurrentUserAsync()
        {
            var service = GetService();
            return await service.GetCurrentUserAsync();
        }

        public async Task<BackendServiceResult<object[]>> GetDatabasesAsync()
        {
            var service = GetService();
            return await service.GetDatabasesAsync();
        }

        public async Task<BackendServiceResult<object[]>> GetCollectionsAsync(string databaseId)
        {
            var service = GetService();
            return await service.GetCollectionsAsync(databaseId);
        }

        public async Task<BackendServiceResult<object[]>> GetDocumentsAsync(string databaseId, string collectionId)
        {
            var service = GetService();
            return await service.GetDocumentsAsync(databaseId, collectionId);
        }

        public async Task<BackendServiceResult<object>> CreateDocumentAsync(string databaseId, string collectionId, object data)
        {
            var service = GetService();
            return await service.CreateDocumentAsync(databaseId, collectionId, data);
        }

        public async Task<BackendServiceResult<object[]>> GetBucketsAsync()
        {
            var service = GetService();
            return await service.GetBucketsAsync();
        }

        // Food methods
        public async Task<BackendServiceResult<object[]>> GetFoodsAsync()
        {
            var service = GetService();
            return await service.GetFoodsAsync();
        }

        public async Task<BackendServiceResult<object>> CreateFoodAsync(object foodData)
        {
            var service = GetService();
            return await service.CreateFoodAsync(foodData);
        }

        public async Task<BackendServiceResult<object>> UpdateFoodAsync(string id, object foodData)
        {
            var service = GetService();
            return await service.UpdateFoodAsync(id, foodData);
        }

        public async Task<BackendServiceResult<bool>> DeleteFoodAsync(string id)
        {
            var service = GetService();
            return await service.DeleteFoodAsync(id);
        }

        // Subscription methods
        public async Task<BackendServiceResult<object[]>> GetSubscriptionsAsync()
        {
            var service = GetService();
            return await service.GetSubscriptionsAsync();
        }

        public async Task<BackendServiceResult<object>> CreateSubscriptionAsync(object subscriptionData)
        {
            var service = GetService();
            return await service.CreateSubscriptionAsync(subscriptionData);
        }

        public async Task<BackendServiceResult<object>> UpdateSubscriptionAsync(string id, object subscriptionData)
        {
            var service = GetService();
            return await service.UpdateSubscriptionAsync(id, subscriptionData);
        }

        public async Task<BackendServiceResult<bool>> DeleteSubscriptionAsync(string id)
        {
            var service = GetService();
            return await service.DeleteSubscriptionAsync(id);
        }
    }
}