using System;
using System.Threading.Tasks;
using wpfkiro20260101.Models;

namespace wpfkiro20260101.Services
{
    public class CrudManager
    {
        private readonly IBackendService _backendService;

        public CrudManager(IBackendService backendService)
        {
            _backendService = backendService ?? throw new ArgumentNullException(nameof(backendService));
        }

        // Food CRUD operations
        public async Task<BackendServiceResult<object>> CreateFoodAsync(Food food)
        {
            try
            {
                if (food == null)
                {
                    return BackendServiceResult<object>.CreateError("食品資料不能為空");
                }

                // 設定創建時間
                food.CreatedAt = DateTime.UtcNow;
                food.UpdatedAt = DateTime.UtcNow;

                return await _backendService.CreateFoodAsync(food);
            }
            catch (Exception ex)
            {
                return BackendServiceResult<object>.CreateError($"創建食品失敗：{ex.Message}");
            }
        }

        public async Task<BackendServiceResult<object[]>> GetAllFoodsAsync()
        {
            try
            {
                return await _backendService.GetFoodsAsync();
            }
            catch (Exception ex)
            {
                return BackendServiceResult<object[]>.CreateError($"讀取食品失敗：{ex.Message}");
            }
        }

        public async Task<BackendServiceResult<object>> UpdateFoodAsync(string id, Food food)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                {
                    return BackendServiceResult<object>.CreateError("ID不能為空");
                }

                if (food == null)
                {
                    return BackendServiceResult<object>.CreateError("食品資料不能為空");
                }

                // 設定更新時間
                food.UpdatedAt = DateTime.UtcNow;
                food.Id = id; // 確保ID正確

                return await _backendService.UpdateFoodAsync(id, food);
            }
            catch (Exception ex)
            {
                return BackendServiceResult<object>.CreateError($"更新食品失敗：{ex.Message}");
            }
        }

        public async Task<BackendServiceResult<bool>> DeleteFoodAsync(string id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                {
                    return BackendServiceResult<bool>.CreateError("ID不能為空");
                }

                return await _backendService.DeleteFoodAsync(id);
            }
            catch (Exception ex)
            {
                return BackendServiceResult<bool>.CreateError($"刪除食品失敗：{ex.Message}");
            }
        }

        // Subscription CRUD operations
        public async Task<BackendServiceResult<object>> CreateSubscriptionAsync(Subscription subscription)
        {
            try
            {
                if (subscription == null)
                {
                    return BackendServiceResult<object>.CreateError("訂閱資料不能為空");
                }

                // 設定創建時間
                subscription.CreatedAt = DateTime.UtcNow;
                subscription.UpdatedAt = DateTime.UtcNow;

                return await _backendService.CreateSubscriptionAsync(subscription);
            }
            catch (Exception ex)
            {
                return BackendServiceResult<object>.CreateError($"創建訂閱失敗：{ex.Message}");
            }
        }

        public async Task<BackendServiceResult<object[]>> GetAllSubscriptionsAsync()
        {
            try
            {
                return await _backendService.GetSubscriptionsAsync();
            }
            catch (Exception ex)
            {
                return BackendServiceResult<object[]>.CreateError($"讀取訂閱失敗：{ex.Message}");
            }
        }

        public async Task<BackendServiceResult<object>> UpdateSubscriptionAsync(string id, Subscription subscription)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                {
                    return BackendServiceResult<object>.CreateError("ID不能為空");
                }

                if (subscription == null)
                {
                    return BackendServiceResult<object>.CreateError("訂閱資料不能為空");
                }

                // 設定更新時間
                subscription.UpdatedAt = DateTime.UtcNow;
                subscription.Id = id; // 確保ID正確

                return await _backendService.UpdateSubscriptionAsync(id, subscription);
            }
            catch (Exception ex)
            {
                return BackendServiceResult<object>.CreateError($"更新訂閱失敗：{ex.Message}");
            }
        }

        public async Task<BackendServiceResult<bool>> DeleteSubscriptionAsync(string id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                {
                    return BackendServiceResult<bool>.CreateError("ID不能為空");
                }

                return await _backendService.DeleteSubscriptionAsync(id);
            }
            catch (Exception ex)
            {
                return BackendServiceResult<bool>.CreateError($"刪除訂閱失敗：{ex.Message}");
            }
        }

        // 批量操作
        public async Task<BackendServiceResult<int>> DeleteMultipleFoodsAsync(string[] ids)
        {
            try
            {
                if (ids == null || ids.Length == 0)
                {
                    return BackendServiceResult<int>.CreateError("ID列表不能為空");
                }

                int successCount = 0;
                int failCount = 0;

                foreach (var id in ids)
                {
                    var result = await DeleteFoodAsync(id);
                    if (result.Success)
                    {
                        successCount++;
                    }
                    else
                    {
                        failCount++;
                    }
                }

                if (failCount > 0)
                {
                    return BackendServiceResult<int>.CreateError($"部分刪除失敗：成功 {successCount}，失敗 {failCount}");
                }

                return BackendServiceResult<int>.CreateSuccess(successCount);
            }
            catch (Exception ex)
            {
                return BackendServiceResult<int>.CreateError($"批量刪除食品失敗：{ex.Message}");
            }
        }

        public async Task<BackendServiceResult<int>> DeleteMultipleSubscriptionsAsync(string[] ids)
        {
            try
            {
                if (ids == null || ids.Length == 0)
                {
                    return BackendServiceResult<int>.CreateError("ID列表不能為空");
                }

                int successCount = 0;
                int failCount = 0;

                foreach (var id in ids)
                {
                    var result = await DeleteSubscriptionAsync(id);
                    if (result.Success)
                    {
                        successCount++;
                    }
                    else
                    {
                        failCount++;
                    }
                }

                if (failCount > 0)
                {
                    return BackendServiceResult<int>.CreateError($"部分刪除失敗：成功 {successCount}，失敗 {failCount}");
                }

                return BackendServiceResult<int>.CreateSuccess(successCount);
            }
            catch (Exception ex)
            {
                return BackendServiceResult<int>.CreateError($"批量刪除訂閱失敗：{ex.Message}");
            }
        }

        // 驗證方法
        public static BackendServiceResult<bool> ValidateFood(Food food)
        {
            if (food == null)
            {
                return BackendServiceResult<bool>.CreateError("食品資料不能為空");
            }

            if (string.IsNullOrWhiteSpace(food.FoodName))
            {
                return BackendServiceResult<bool>.CreateError("食品名稱不能為空");
            }

            return BackendServiceResult<bool>.CreateSuccess(true);
        }

        public static BackendServiceResult<bool> ValidateSubscription(Subscription subscription)
        {
            if (subscription == null)
            {
                return BackendServiceResult<bool>.CreateError("訂閱資料不能為空");
            }

            if (string.IsNullOrWhiteSpace(subscription.SubscriptionName))
            {
                return BackendServiceResult<bool>.CreateError("訂閱名稱不能為空");
            }

            if (subscription.Price < 0)
            {
                return BackendServiceResult<bool>.CreateError("價格不能為負數");
            }

            return BackendServiceResult<bool>.CreateSuccess(true);
        }

        // 取得後端服務資訊
        public string GetServiceName() => _backendService.ServiceName;
        public BackendServiceType GetServiceType() => _backendService.ServiceType;

        // 測試連線
        public async Task<bool> TestConnectionAsync()
        {
            return await _backendService.TestConnectionAsync();
        }
    }
}