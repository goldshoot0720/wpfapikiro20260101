using System;
using System.Threading.Tasks;

namespace wpfkiro20260101.Services
{
    public interface IBackendService : IDisposable
    {
        Task<bool> TestConnectionAsync();
        Task<bool> InitializeAsync();
        string ServiceName { get; }
        BackendServiceType ServiceType { get; }
        
        // CRUD operations for Food
        Task<BackendServiceResult<object[]>> GetFoodsAsync();
        Task<BackendServiceResult<object>> CreateFoodAsync(object foodData);
        Task<BackendServiceResult<object>> UpdateFoodAsync(string id, object foodData);
        Task<BackendServiceResult<bool>> DeleteFoodAsync(string id);
        
        // CRUD operations for Subscriptions
        Task<BackendServiceResult<object[]>> GetSubscriptionsAsync();
        Task<BackendServiceResult<object>> CreateSubscriptionAsync(object subscriptionData);
        Task<BackendServiceResult<object>> UpdateSubscriptionAsync(string id, object subscriptionData);
        Task<BackendServiceResult<bool>> DeleteSubscriptionAsync(string id);
    }

    public class BackendServiceResult<T>
    {
        public bool Success { get; set; }
        public T? Data { get; set; }
        public string? ErrorMessage { get; set; }

        public static BackendServiceResult<T> CreateSuccess(T data)
        {
            return new BackendServiceResult<T>
            {
                Success = true,
                Data = data
            };
        }

        public static BackendServiceResult<T> CreateError(string errorMessage)
        {
            return new BackendServiceResult<T>
            {
                Success = false,
                ErrorMessage = errorMessage
            };
        }
    }
}