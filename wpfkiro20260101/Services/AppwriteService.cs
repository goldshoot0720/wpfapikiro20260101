using System;
using System.Threading.Tasks;
using Appwrite;
using Appwrite.Services;

namespace wpfkiro20260101.Services
{
    public class AppwriteService : IBackendService
    {
        private readonly AppSettings _settings;
        private Client? _client;
        private Health? _health;

        public string ServiceName => "Appwrite";
        public BackendServiceType ServiceType => BackendServiceType.Appwrite;

        public AppwriteService()
        {
            _settings = AppSettings.Instance;
        }

        public async Task<bool> TestConnectionAsync()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(_settings.ApiUrl) || 
                    string.IsNullOrWhiteSpace(_settings.ProjectId))
                {
                    return false;
                }

                // 創建 Appwrite 客戶端
                var client = new Client()
                    .SetEndpoint(_settings.ApiUrl)
                    .SetProject(_settings.ProjectId);

                // 如果有 API Key，設定它（用於伺服器端）
                if (!string.IsNullOrWhiteSpace(_settings.ApiKey))
                {
                    client.SetKey(_settings.ApiKey);
                }

                // 測試健康檢查
                var health = new Health(client);
                var result = await health.Get();
                
                return !string.IsNullOrEmpty(result.Status);
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> InitializeAsync()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(_settings.ApiUrl) || 
                    string.IsNullOrWhiteSpace(_settings.ProjectId))
                {
                    return false;
                }

                // 初始化 Appwrite 客戶端
                _client = new Client()
                    .SetEndpoint(_settings.ApiUrl)
                    .SetProject(_settings.ProjectId);

                // 如果有 API Key，設定它
                if (!string.IsNullOrWhiteSpace(_settings.ApiKey))
                {
                    _client.SetKey(_settings.ApiKey);
                }

                // 初始化健康檢查服務
                _health = new Health(_client);

                return await TestConnectionAsync();
            }
            catch (Exception)
            {
                return false;
            }
        }

        // 基本的健康檢查方法
        public async Task<BackendServiceResult<object>> GetHealthAsync()
        {
            try
            {
                if (_health == null)
                {
                    await InitializeAsync();
                }

                if (_health == null)
                {
                    return BackendServiceResult<object>.CreateError("Appwrite 服務未初始化");
                }

                var health = await _health.Get();
                return BackendServiceResult<object>.CreateSuccess(health);
            }
            catch (Exception ex)
            {
                return BackendServiceResult<object>.CreateError(ex.Message);
            }
        }

        // 簡化的用戶創建方法（模擬）
        public async Task<BackendServiceResult<string>> CreateUserAsync(string email, string password, string name)
        {
            try
            {
                // 這裡先返回模擬結果，實際實作需要根據 Appwrite SDK 版本調整
                await Task.Delay(1000);
                var userId = Guid.NewGuid().ToString();
                return BackendServiceResult<string>.CreateSuccess(userId);
            }
            catch (Exception ex)
            {
                return BackendServiceResult<string>.CreateError(ex.Message);
            }
        }

        // 模擬的其他方法
        public async Task<BackendServiceResult<object>> GetCurrentUserAsync()
        {
            try
            {
                await Task.Delay(500);
                var user = new { id = "user123", email = "test@example.com", name = "Test User" };
                return BackendServiceResult<object>.CreateSuccess(user);
            }
            catch (Exception ex)
            {
                return BackendServiceResult<object>.CreateError(ex.Message);
            }
        }

        public async Task<BackendServiceResult<object[]>> GetDatabasesAsync()
        {
            try
            {
                await Task.Delay(500);
                var databases = new object[] 
                { 
                    new { id = "db1", name = "Main Database" },
                    new { id = "db2", name = "Test Database" }
                };
                return BackendServiceResult<object[]>.CreateSuccess(databases);
            }
            catch (Exception ex)
            {
                return BackendServiceResult<object[]>.CreateError(ex.Message);
            }
        }

        public async Task<BackendServiceResult<object[]>> GetCollectionsAsync(string databaseId)
        {
            try
            {
                await Task.Delay(500);
                var collections = new object[] 
                { 
                    new { id = "col1", name = "Users" },
                    new { id = "col2", name = "Posts" }
                };
                return BackendServiceResult<object[]>.CreateSuccess(collections);
            }
            catch (Exception ex)
            {
                return BackendServiceResult<object[]>.CreateError(ex.Message);
            }
        }

        public async Task<BackendServiceResult<object[]>> GetDocumentsAsync(string databaseId, string collectionId)
        {
            try
            {
                await Task.Delay(500);
                var documents = new object[] 
                { 
                    new { id = "doc1", title = "Sample Document 1" },
                    new { id = "doc2", title = "Sample Document 2" }
                };
                return BackendServiceResult<object[]>.CreateSuccess(documents);
            }
            catch (Exception ex)
            {
                return BackendServiceResult<object[]>.CreateError(ex.Message);
            }
        }

        public async Task<BackendServiceResult<object>> CreateDocumentAsync(string databaseId, string collectionId, object data)
        {
            try
            {
                await Task.Delay(500);
                var document = new { id = Guid.NewGuid().ToString(), data = data };
                return BackendServiceResult<object>.CreateSuccess(document);
            }
            catch (Exception ex)
            {
                return BackendServiceResult<object>.CreateError(ex.Message);
            }
        }

        public async Task<BackendServiceResult<object[]>> GetBucketsAsync()
        {
            try
            {
                await Task.Delay(500);
                var buckets = new object[] 
                { 
                    new { id = "bucket1", name = "Images" },
                    new { id = "bucket2", name = "Documents" }
                };
                return BackendServiceResult<object[]>.CreateSuccess(buckets);
            }
            catch (Exception ex)
            {
                return BackendServiceResult<object[]>.CreateError(ex.Message);
            }
        }

        // Food CRUD operations
        public async Task<BackendServiceResult<object[]>> GetFoodsAsync()
        {
            try
            {
                if (_client == null)
                {
                    var initResult = await InitializeAsync();
                    if (!initResult)
                    {
                        return BackendServiceResult<object[]>.CreateError("Appwrite 客戶端未初始化");
                    }
                }

                var settings = AppSettings.Instance;
                if (string.IsNullOrWhiteSpace(settings.DatabaseId) || 
                    string.IsNullOrWhiteSpace(settings.FoodCollectionId))
                {
                    return BackendServiceResult<object[]>.CreateError("資料庫ID或食品集合ID未設定");
                }

                // 使用 Appwrite Databases 服務
                var databases = new Appwrite.Services.Databases(_client);
                
                // 獲取食品文檔
                var documents = await databases.ListDocuments(
                    databaseId: settings.DatabaseId,
                    collectionId: settings.FoodCollectionId
                );

                // 轉換為我們的資料格式 - 對照實際Appwrite欄位
                var foods = documents.Documents.Select(doc => new
                {
                    id = doc.Id,
                    foodName = doc.Data.TryGetValue("food_name", out var foodName) ? foodName?.ToString() ?? "" : "",
                    photo = doc.Data.TryGetValue("photo", out var photo) ? photo?.ToString() ?? "" : "",
                    photoHash = doc.Data.TryGetValue("photo_hash", out var photoHash) ? photoHash?.ToString() ?? "" : "",
                    shop = doc.Data.TryGetValue("shop", out var shop) ? shop?.ToString() ?? "" : "",
                    note = doc.Data.TryGetValue("note", out var note) ? note?.ToString() ?? "" : "",
                    createdAt = doc.CreatedAt,
                    updatedAt = doc.UpdatedAt
                }).ToArray<object>();

                return BackendServiceResult<object[]>.CreateSuccess(foods);
            }
            catch (Exception ex)
            {
                return BackendServiceResult<object[]>.CreateError($"載入食品資料失敗：{ex.Message}");
            }
        }

        public async Task<BackendServiceResult<object>> CreateFoodAsync(object foodData)
        {
            try
            {
                if (_client == null)
                {
                    var initResult = await InitializeAsync();
                    if (!initResult)
                    {
                        return BackendServiceResult<object>.CreateError("Appwrite 客戶端未初始化");
                    }
                }

                var settings = AppSettings.Instance;
                if (string.IsNullOrWhiteSpace(settings.DatabaseId) || 
                    string.IsNullOrWhiteSpace(settings.FoodCollectionId))
                {
                    return BackendServiceResult<object>.CreateError("資料庫ID或食品集合ID未設定");
                }

                // 使用 Appwrite Databases 服務
                var databases = new Appwrite.Services.Databases(_client);
                
                // 將 foodData 轉換為字典 - 對照實際Appwrite欄位
                var data = new Dictionary<string, object>();
                
                if (foodData is Models.Food food)
                {
                    data["food_name"] = food.FoodName;
                    data["photo"] = food.Photo;
                    data["photo_hash"] = food.PhotoHash;
                    data["shop"] = food.Shop;
                    data["note"] = food.Note;
                }
                else
                {
                    return BackendServiceResult<object>.CreateError("無效的食品資料格式");
                }

                // 創建文檔
                var document = await databases.CreateDocument(
                    databaseId: settings.DatabaseId,
                    collectionId: settings.FoodCollectionId,
                    documentId: Appwrite.ID.Unique(),
                    data: data
                );

                var result = new
                {
                    id = document.Id,
                    data = foodData,
                    createdAt = document.CreatedAt,
                    updatedAt = document.UpdatedAt
                };

                return BackendServiceResult<object>.CreateSuccess(result);
            }
            catch (Exception ex)
            {
                return BackendServiceResult<object>.CreateError($"創建食品失敗：{ex.Message}");
            }
        }

        public async Task<BackendServiceResult<object>> UpdateFoodAsync(string id, object foodData)
        {
            try
            {
                if (_client == null)
                {
                    var initResult = await InitializeAsync();
                    if (!initResult)
                    {
                        return BackendServiceResult<object>.CreateError("Appwrite 客戶端未初始化");
                    }
                }

                var settings = AppSettings.Instance;
                if (string.IsNullOrWhiteSpace(settings.DatabaseId) || 
                    string.IsNullOrWhiteSpace(settings.FoodCollectionId))
                {
                    return BackendServiceResult<object>.CreateError("資料庫ID或食品集合ID未設定");
                }

                // 使用 Appwrite Databases 服務
                var databases = new Appwrite.Services.Databases(_client);
                
                // 將 foodData 轉換為字典 - 對照實際Appwrite欄位
                var data = new Dictionary<string, object>();
                
                if (foodData is Models.Food food)
                {
                    data["food_name"] = food.FoodName;
                    data["photo"] = food.Photo;
                    data["photo_hash"] = food.PhotoHash;
                    data["shop"] = food.Shop;
                    data["note"] = food.Note;
                }
                else
                {
                    return BackendServiceResult<object>.CreateError("無效的食品資料格式");
                }

                // 更新文檔
                var document = await databases.UpdateDocument(
                    databaseId: settings.DatabaseId,
                    collectionId: settings.FoodCollectionId,
                    documentId: id,
                    data: data
                );

                var result = new
                {
                    id = document.Id,
                    data = foodData,
                    updatedAt = document.UpdatedAt
                };

                return BackendServiceResult<object>.CreateSuccess(result);
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
                if (_client == null)
                {
                    var initResult = await InitializeAsync();
                    if (!initResult)
                    {
                        return BackendServiceResult<bool>.CreateError("Appwrite 客戶端未初始化");
                    }
                }

                var settings = AppSettings.Instance;
                if (string.IsNullOrWhiteSpace(settings.DatabaseId) || 
                    string.IsNullOrWhiteSpace(settings.FoodCollectionId))
                {
                    return BackendServiceResult<bool>.CreateError("資料庫ID或食品集合ID未設定");
                }

                // 使用 Appwrite Databases 服務
                var databases = new Appwrite.Services.Databases(_client);
                
                // 刪除文檔
                await databases.DeleteDocument(
                    databaseId: settings.DatabaseId,
                    collectionId: settings.FoodCollectionId,
                    documentId: id
                );

                return BackendServiceResult<bool>.CreateSuccess(true);
            }
            catch (Exception ex)
            {
                return BackendServiceResult<bool>.CreateError($"刪除食品失敗：{ex.Message}");
            }
        }

        // Subscription CRUD operations
        public async Task<BackendServiceResult<object[]>> GetSubscriptionsAsync()
        {
            try
            {
                if (_client == null)
                {
                    var initResult = await InitializeAsync();
                    if (!initResult)
                    {
                        return BackendServiceResult<object[]>.CreateError("Appwrite 客戶端未初始化");
                    }
                }

                var settings = AppSettings.Instance;
                if (string.IsNullOrWhiteSpace(settings.DatabaseId) || 
                    string.IsNullOrWhiteSpace(settings.SubscriptionCollectionId))
                {
                    return BackendServiceResult<object[]>.CreateError("資料庫ID或訂閱集合ID未設定");
                }

                // 使用 Appwrite Databases 服務
                var databases = new Appwrite.Services.Databases(_client);
                
                // 獲取訂閱文檔
                var documents = await databases.ListDocuments(
                    databaseId: settings.DatabaseId,
                    collectionId: settings.SubscriptionCollectionId
                );

                // 轉換為我們的資料格式 - 對照實際Appwrite欄位
                var subscriptions = documents.Documents.Select(doc => new
                {
                    id = doc.Id,
                    subscriptionName = doc.Data.TryGetValue("subscription_name", out var subscriptionName) ? subscriptionName?.ToString() ?? "" : "",
                    nextDate = doc.Data.TryGetValue("next_date", out var nextDate) ? nextDate?.ToString() ?? "" : "",
                    price = doc.Data.TryGetValue("price", out var price) && int.TryParse(price?.ToString(), out var p) ? p : 0,
                    site = doc.Data.TryGetValue("site", out var site) ? site?.ToString() ?? "" : "",
                    account = doc.Data.TryGetValue("account", out var account) ? account?.ToString() ?? "" : "",
                    note = doc.Data.TryGetValue("note", out var note) ? note?.ToString() ?? "" : "",
                    stringToDate = doc.Data.TryGetValue("string_to_date", out var stringToDate) ? stringToDate?.ToString() ?? "" : "",
                    dateTime = doc.Data.TryGetValue("date_time", out var dateTime) ? dateTime?.ToString() ?? "" : "",
                    foodId = doc.Data.TryGetValue("food_id", out var foodId) ? foodId?.ToString() : null,
                    createdAt = doc.CreatedAt,
                    updatedAt = doc.UpdatedAt
                }).ToArray<object>();

                return BackendServiceResult<object[]>.CreateSuccess(subscriptions);
            }
            catch (Exception ex)
            {
                return BackendServiceResult<object[]>.CreateError($"載入訂閱資料失敗：{ex.Message}");
            }
        }

        public async Task<BackendServiceResult<object>> CreateSubscriptionAsync(object subscriptionData)
        {
            try
            {
                if (_client == null)
                {
                    var initResult = await InitializeAsync();
                    if (!initResult)
                    {
                        return BackendServiceResult<object>.CreateError("Appwrite 客戶端未初始化");
                    }
                }

                var settings = AppSettings.Instance;
                if (string.IsNullOrWhiteSpace(settings.DatabaseId) || 
                    string.IsNullOrWhiteSpace(settings.SubscriptionCollectionId))
                {
                    return BackendServiceResult<object>.CreateError("資料庫ID或訂閱集合ID未設定");
                }

                // 使用 Appwrite Databases 服務
                var databases = new Appwrite.Services.Databases(_client);
                
                // 將 subscriptionData 轉換為字典 - 對照實際Appwrite欄位
                var data = new Dictionary<string, object>();
                
                if (subscriptionData is Models.Subscription subscription)
                {
                    data["subscription_name"] = subscription.SubscriptionName;
                    data["next_date"] = subscription.NextDate.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
                    data["price"] = subscription.Price;
                    data["site"] = subscription.Site;
                    data["account"] = subscription.Account;
                    data["note"] = subscription.Note;
                    data["string_to_date"] = subscription.StringToDate;
                    data["date_time"] = subscription.DateTime.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
                    if (!string.IsNullOrEmpty(subscription.FoodId))
                    {
                        data["food_id"] = subscription.FoodId;
                    }
                }
                else
                {
                    return BackendServiceResult<object>.CreateError("無效的訂閱資料格式");
                }

                // 創建文檔
                var document = await databases.CreateDocument(
                    databaseId: settings.DatabaseId,
                    collectionId: settings.SubscriptionCollectionId,
                    documentId: Appwrite.ID.Unique(),
                    data: data
                );

                var result = new
                {
                    id = document.Id,
                    data = subscriptionData,
                    createdAt = document.CreatedAt,
                    updatedAt = document.UpdatedAt
                };

                return BackendServiceResult<object>.CreateSuccess(result);
            }
            catch (Exception ex)
            {
                return BackendServiceResult<object>.CreateError($"創建訂閱失敗：{ex.Message}");
            }
        }

        public async Task<BackendServiceResult<object>> UpdateSubscriptionAsync(string id, object subscriptionData)
        {
            try
            {
                if (_client == null)
                {
                    var initResult = await InitializeAsync();
                    if (!initResult)
                    {
                        return BackendServiceResult<object>.CreateError("Appwrite 客戶端未初始化");
                    }
                }

                var settings = AppSettings.Instance;
                if (string.IsNullOrWhiteSpace(settings.DatabaseId) || 
                    string.IsNullOrWhiteSpace(settings.SubscriptionCollectionId))
                {
                    return BackendServiceResult<object>.CreateError("資料庫ID或訂閱集合ID未設定");
                }

                // 使用 Appwrite Databases 服務
                var databases = new Appwrite.Services.Databases(_client);
                
                // 將 subscriptionData 轉換為字典 - 對照實際Appwrite欄位
                var data = new Dictionary<string, object>();
                
                if (subscriptionData is Models.Subscription subscription)
                {
                    data["subscription_name"] = subscription.SubscriptionName;
                    data["next_date"] = subscription.NextDate.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
                    data["price"] = subscription.Price;
                    data["site"] = subscription.Site;
                    data["account"] = subscription.Account;
                    data["note"] = subscription.Note;
                    data["string_to_date"] = subscription.StringToDate;
                    data["date_time"] = subscription.DateTime.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
                    if (!string.IsNullOrEmpty(subscription.FoodId))
                    {
                        data["food_id"] = subscription.FoodId;
                    }
                }
                else
                {
                    return BackendServiceResult<object>.CreateError("無效的訂閱資料格式");
                }

                // 更新文檔
                var document = await databases.UpdateDocument(
                    databaseId: settings.DatabaseId,
                    collectionId: settings.SubscriptionCollectionId,
                    documentId: id,
                    data: data
                );

                var result = new
                {
                    id = document.Id,
                    data = subscriptionData,
                    updatedAt = document.UpdatedAt
                };

                return BackendServiceResult<object>.CreateSuccess(result);
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
                if (_client == null)
                {
                    var initResult = await InitializeAsync();
                    if (!initResult)
                    {
                        return BackendServiceResult<bool>.CreateError("Appwrite 客戶端未初始化");
                    }
                }

                var settings = AppSettings.Instance;
                if (string.IsNullOrWhiteSpace(settings.DatabaseId) || 
                    string.IsNullOrWhiteSpace(settings.SubscriptionCollectionId))
                {
                    return BackendServiceResult<bool>.CreateError("資料庫ID或訂閱集合ID未設定");
                }

                // 使用 Appwrite Databases 服務
                var databases = new Appwrite.Services.Databases(_client);
                
                // 刪除文檔
                await databases.DeleteDocument(
                    databaseId: settings.DatabaseId,
                    collectionId: settings.SubscriptionCollectionId,
                    documentId: id
                );

                return BackendServiceResult<bool>.CreateSuccess(true);
            }
            catch (Exception ex)
            {
                return BackendServiceResult<bool>.CreateError($"刪除訂閱失敗：{ex.Message}");
            }
        }

        public Client? GetClient() => _client;
        public Health? GetHealth() => _health;
    }
}