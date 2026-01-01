using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace wpfkiro20260101.Services
{
    public class MySQLService : IBackendService
    {
        private readonly AppSettings _settings;
        private readonly HttpClient _httpClient;

        public string ServiceName => "MySQL";
        public BackendServiceType ServiceType => BackendServiceType.MySQL;

        public MySQLService()
        {
            _settings = AppSettings.Instance;
            _httpClient = new HttpClient();
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

                // 假設有一個 MySQL REST API 端點
                _httpClient.DefaultRequestHeaders.Clear();
                if (!string.IsNullOrWhiteSpace(_settings.ApiKey))
                {
                    _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_settings.ApiKey}");
                }

                var response = await _httpClient.GetAsync($"{_settings.ApiUrl}/api/health");
                return response.IsSuccessStatusCode;
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
                await Task.Delay(800);
                return await TestConnectionAsync();
            }
            catch (Exception)
            {
                return false;
            }
        }

        // Food CRUD operations
        public async Task<BackendServiceResult<object[]>> GetFoodsAsync()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(_settings.ApiUrl) || 
                    string.IsNullOrWhiteSpace(_settings.ProjectId))
                {
                    return BackendServiceResult<object[]>.CreateError("MySQL 設定不完整");
                }

                _httpClient.DefaultRequestHeaders.Clear();
                if (!string.IsNullOrWhiteSpace(_settings.ApiKey))
                {
                    _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_settings.ApiKey}");
                }

                var response = await _httpClient.GetAsync($"{_settings.ApiUrl}/api/databases/{_settings.ProjectId}/tables/foods/rows");
                
                if (response.IsSuccessStatusCode)
                {
                    var jsonContent = await response.Content.ReadAsStringAsync();
                    var data = JsonSerializer.Deserialize<JsonElement>(jsonContent);
                    
                    var foods = new List<object>();
                    
                    if (data.TryGetProperty("rows", out var rows))
                    {
                        foreach (var row in rows.EnumerateArray())
                        {
                            foods.Add(new
                            {
                                id = row.TryGetProperty("id", out var id) ? id.GetString() : "",
                                foodName = row.TryGetProperty("food_name", out var foodName) ? foodName.GetString() : "",
                                price = row.TryGetProperty("price", out var price) ? (price.ValueKind == JsonValueKind.Number ? price.GetInt32() : 0) : 0,
                                photo = row.TryGetProperty("photo", out var photo) ? photo.GetString() : "",
                                photoHash = row.TryGetProperty("photo_hash", out var photoHash) ? photoHash.GetString() : "",
                                shop = row.TryGetProperty("shop", out var shop) ? shop.GetString() : "",
                                toDate = row.TryGetProperty("to_date", out var toDate) ? toDate.GetString() : "",
                                description = row.TryGetProperty("description", out var description) ? description.GetString() : "",
                                category = row.TryGetProperty("category", out var category) ? category.GetString() : "",
                                storageLocation = row.TryGetProperty("storage_location", out var storageLocation) ? storageLocation.GetString() : "",
                                note = row.TryGetProperty("note", out var note) ? note.GetString() : "",
                                createdAt = row.TryGetProperty("created_at", out var createdAt) ? createdAt.GetString() : "",
                                updatedAt = row.TryGetProperty("updated_at", out var updatedAt) ? updatedAt.GetString() : ""
                            });
                        }
                    }
                    
                    return BackendServiceResult<object[]>.CreateSuccess(foods.ToArray());
                }
                else
                {
                    return BackendServiceResult<object[]>.CreateError($"MySQL API 錯誤：{response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                return BackendServiceResult<object[]>.CreateError($"載入 MySQL 食品資料失敗：{ex.Message}");
            }
        }

        public async Task<BackendServiceResult<object>> CreateFoodAsync(object foodData)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(_settings.ApiUrl) || 
                    string.IsNullOrWhiteSpace(_settings.ProjectId))
                {
                    return BackendServiceResult<object>.CreateError("MySQL 設定不完整");
                }

                _httpClient.DefaultRequestHeaders.Clear();
                if (!string.IsNullOrWhiteSpace(_settings.ApiKey))
                {
                    _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_settings.ApiKey}");
                }

                var data = new Dictionary<string, object>();
                
                if (foodData is Models.Food food)
                {
                    data["food_name"] = food.FoodName;
                    data["price"] = food.Price;
                    data["photo"] = food.Photo;
                    data["photo_hash"] = food.PhotoHash;
                    data["shop"] = food.Shop;
                    data["to_date"] = food.ToDate;
                    data["description"] = food.Description;
                    data["category"] = food.Category;
                    data["storage_location"] = food.StorageLocation;
                    data["note"] = food.Note;
                    data["created_at"] = food.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss");
                    data["updated_at"] = food.UpdatedAt.ToString("yyyy-MM-dd HH:mm:ss");
                }

                var json = JsonSerializer.Serialize(data);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync($"{_settings.ApiUrl}/api/databases/{_settings.ProjectId}/tables/foods/rows", content);
                
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<JsonElement>(responseContent);
                    
                    return BackendServiceResult<object>.CreateSuccess(new
                    {
                        id = result.TryGetProperty("id", out var id) ? id.GetString() : "",
                        data = foodData
                    });
                }
                else
                {
                    return BackendServiceResult<object>.CreateError($"MySQL 創建失敗：{response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                return BackendServiceResult<object>.CreateError($"創建 MySQL 食品失敗：{ex.Message}");
            }
        }

        public async Task<BackendServiceResult<object>> UpdateFoodAsync(string id, object foodData)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(_settings.ApiUrl) || 
                    string.IsNullOrWhiteSpace(_settings.ProjectId))
                {
                    return BackendServiceResult<object>.CreateError("MySQL 設定不完整");
                }

                _httpClient.DefaultRequestHeaders.Clear();
                if (!string.IsNullOrWhiteSpace(_settings.ApiKey))
                {
                    _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_settings.ApiKey}");
                }

                var data = new Dictionary<string, object>();
                
                if (foodData is Models.Food food)
                {
                    data["food_name"] = food.FoodName;
                    data["price"] = food.Price;
                    data["photo"] = food.Photo;
                    data["photo_hash"] = food.PhotoHash;
                    data["shop"] = food.Shop;
                    data["to_date"] = food.ToDate;
                    data["description"] = food.Description;
                    data["category"] = food.Category;
                    data["storage_location"] = food.StorageLocation;
                    data["note"] = food.Note;
                    data["updated_at"] = food.UpdatedAt.ToString("yyyy-MM-dd HH:mm:ss");
                }

                var json = JsonSerializer.Serialize(data);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync($"{_settings.ApiUrl}/api/databases/{_settings.ProjectId}/tables/foods/rows/{id}", content);
                
                if (response.IsSuccessStatusCode)
                {
                    return BackendServiceResult<object>.CreateSuccess(new
                    {
                        id = id,
                        data = foodData
                    });
                }
                else
                {
                    return BackendServiceResult<object>.CreateError($"MySQL 更新失敗：{response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                return BackendServiceResult<object>.CreateError($"更新 MySQL 食品失敗：{ex.Message}");
            }
        }

        public async Task<BackendServiceResult<bool>> DeleteFoodAsync(string id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(_settings.ApiUrl) || 
                    string.IsNullOrWhiteSpace(_settings.ProjectId))
                {
                    return BackendServiceResult<bool>.CreateError("MySQL 設定不完整");
                }

                _httpClient.DefaultRequestHeaders.Clear();
                if (!string.IsNullOrWhiteSpace(_settings.ApiKey))
                {
                    _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_settings.ApiKey}");
                }

                var response = await _httpClient.DeleteAsync($"{_settings.ApiUrl}/api/databases/{_settings.ProjectId}/tables/foods/rows/{id}");
                
                if (response.IsSuccessStatusCode)
                {
                    return BackendServiceResult<bool>.CreateSuccess(true);
                }
                else
                {
                    return BackendServiceResult<bool>.CreateError($"MySQL 刪除失敗：{response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                return BackendServiceResult<bool>.CreateError($"刪除 MySQL 食品失敗：{ex.Message}");
            }
        }

        // Subscription CRUD operations
        public async Task<BackendServiceResult<object[]>> GetSubscriptionsAsync()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(_settings.ApiUrl) || 
                    string.IsNullOrWhiteSpace(_settings.ProjectId))
                {
                    return BackendServiceResult<object[]>.CreateError("MySQL 設定不完整");
                }

                _httpClient.DefaultRequestHeaders.Clear();
                if (!string.IsNullOrWhiteSpace(_settings.ApiKey))
                {
                    _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_settings.ApiKey}");
                }

                var response = await _httpClient.GetAsync($"{_settings.ApiUrl}/api/databases/{_settings.ProjectId}/tables/subscriptions/rows");
                
                if (response.IsSuccessStatusCode)
                {
                    var jsonContent = await response.Content.ReadAsStringAsync();
                    var data = JsonSerializer.Deserialize<JsonElement>(jsonContent);
                    
                    var subscriptions = new List<object>();
                    
                    if (data.TryGetProperty("rows", out var rows))
                    {
                        foreach (var row in rows.EnumerateArray())
                        {
                            subscriptions.Add(new
                            {
                                id = row.TryGetProperty("id", out var id) ? id.GetString() : "",
                                subscriptionName = row.TryGetProperty("subscription_name", out var subscriptionName) ? subscriptionName.GetString() : "",
                                nextDate = row.TryGetProperty("next_date", out var nextDate) && DateTime.TryParse(nextDate.GetString(), out var nd) ? nd : DateTime.Now,
                                price = row.TryGetProperty("price", out var price) ? price.GetInt32() : 0,
                                site = row.TryGetProperty("site", out var site) ? site.GetString() : "",
                                account = row.TryGetProperty("account", out var account) ? account.GetString() : "",
                                note = row.TryGetProperty("note", out var note) ? note.GetString() : "",
                                stringToDate = row.TryGetProperty("string_to_date", out var stringToDate) ? stringToDate.GetString() : "",
                                dateTime = row.TryGetProperty("date_time", out var dateTime) && DateTime.TryParse(dateTime.GetString(), out var dt) ? dt : DateTime.Now,
                                foodId = row.TryGetProperty("food_id", out var foodId) ? foodId.GetString() : null,
                                createdAt = row.TryGetProperty("created_at", out var createdAt) ? createdAt.GetString() : "",
                                updatedAt = row.TryGetProperty("updated_at", out var updatedAt) ? updatedAt.GetString() : ""
                            });
                        }
                    }
                    
                    return BackendServiceResult<object[]>.CreateSuccess(subscriptions.ToArray());
                }
                else
                {
                    return BackendServiceResult<object[]>.CreateError($"MySQL API 錯誤：{response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                return BackendServiceResult<object[]>.CreateError($"載入 MySQL 訂閱資料失敗：{ex.Message}");
            }
        }

        public async Task<BackendServiceResult<object>> CreateSubscriptionAsync(object subscriptionData)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(_settings.ApiUrl) || 
                    string.IsNullOrWhiteSpace(_settings.ProjectId))
                {
                    return BackendServiceResult<object>.CreateError("MySQL 設定不完整");
                }

                _httpClient.DefaultRequestHeaders.Clear();
                if (!string.IsNullOrWhiteSpace(_settings.ApiKey))
                {
                    _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_settings.ApiKey}");
                }

                var data = new Dictionary<string, object>();
                
                if (subscriptionData is Models.Subscription subscription)
                {
                    data["subscription_name"] = subscription.SubscriptionName;
                    data["next_date"] = subscription.NextDate.ToString("yyyy-MM-dd");
                    data["price"] = subscription.Price;
                    data["site"] = subscription.Site;
                    data["account"] = subscription.Account;
                    data["note"] = subscription.Note;
                    data["string_to_date"] = subscription.StringToDate;
                    data["date_time"] = subscription.DateTime.ToString("yyyy-MM-dd HH:mm:ss");
                    data["food_id"] = subscription.FoodId;
                    data["created_at"] = subscription.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss");
                    data["updated_at"] = subscription.UpdatedAt.ToString("yyyy-MM-dd HH:mm:ss");
                }

                var json = JsonSerializer.Serialize(data);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync($"{_settings.ApiUrl}/api/databases/{_settings.ProjectId}/tables/subscriptions/rows", content);
                
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<JsonElement>(responseContent);
                    
                    return BackendServiceResult<object>.CreateSuccess(new
                    {
                        id = result.TryGetProperty("id", out var id) ? id.GetString() : "",
                        data = subscriptionData
                    });
                }
                else
                {
                    return BackendServiceResult<object>.CreateError($"MySQL 創建失敗：{response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                return BackendServiceResult<object>.CreateError($"創建 MySQL 訂閱失敗：{ex.Message}");
            }
        }

        public async Task<BackendServiceResult<object>> UpdateSubscriptionAsync(string id, object subscriptionData)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(_settings.ApiUrl) || 
                    string.IsNullOrWhiteSpace(_settings.ProjectId))
                {
                    return BackendServiceResult<object>.CreateError("MySQL 設定不完整");
                }

                _httpClient.DefaultRequestHeaders.Clear();
                if (!string.IsNullOrWhiteSpace(_settings.ApiKey))
                {
                    _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_settings.ApiKey}");
                }

                var data = new Dictionary<string, object>();
                
                if (subscriptionData is Models.Subscription subscription)
                {
                    data["subscription_name"] = subscription.SubscriptionName;
                    data["next_date"] = subscription.NextDate.ToString("yyyy-MM-dd");
                    data["price"] = subscription.Price;
                    data["site"] = subscription.Site;
                    data["account"] = subscription.Account;
                    data["note"] = subscription.Note;
                    data["string_to_date"] = subscription.StringToDate;
                    data["date_time"] = subscription.DateTime.ToString("yyyy-MM-dd HH:mm:ss");
                    data["food_id"] = subscription.FoodId;
                    data["updated_at"] = subscription.UpdatedAt.ToString("yyyy-MM-dd HH:mm:ss");
                }

                var json = JsonSerializer.Serialize(data);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync($"{_settings.ApiUrl}/api/databases/{_settings.ProjectId}/tables/subscriptions/rows/{id}", content);
                
                if (response.IsSuccessStatusCode)
                {
                    return BackendServiceResult<object>.CreateSuccess(new
                    {
                        id = id,
                        data = subscriptionData
                    });
                }
                else
                {
                    return BackendServiceResult<object>.CreateError($"MySQL 更新失敗：{response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                return BackendServiceResult<object>.CreateError($"更新 MySQL 訂閱失敗：{ex.Message}");
            }
        }

        public async Task<BackendServiceResult<bool>> DeleteSubscriptionAsync(string id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(_settings.ApiUrl) || 
                    string.IsNullOrWhiteSpace(_settings.ProjectId))
                {
                    return BackendServiceResult<bool>.CreateError("MySQL 設定不完整");
                }

                _httpClient.DefaultRequestHeaders.Clear();
                if (!string.IsNullOrWhiteSpace(_settings.ApiKey))
                {
                    _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_settings.ApiKey}");
                }

                var response = await _httpClient.DeleteAsync($"{_settings.ApiUrl}/api/databases/{_settings.ProjectId}/tables/subscriptions/rows/{id}");
                
                if (response.IsSuccessStatusCode)
                {
                    return BackendServiceResult<bool>.CreateSuccess(true);
                }
                else
                {
                    return BackendServiceResult<bool>.CreateError($"MySQL 刪除失敗：{response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                return BackendServiceResult<bool>.CreateError($"刪除 MySQL 訂閱失敗：{ex.Message}");
            }
        }

        // MySQL 特定的方法
        public async Task<BackendServiceResult<string[]>> GetTablesAsync()
        {
            try
            {
                await Task.Delay(1000);
                var tables = new[] { "users", "products", "orders", "categories", "food_subscriptions" };
                return BackendServiceResult<string[]>.CreateSuccess(tables);
            }
            catch (Exception ex)
            {
                return BackendServiceResult<string[]>.CreateError(ex.Message);
            }
        }

        public async Task<BackendServiceResult<object[]>> ExecuteQueryAsync(string query)
        {
            try
            {
                await Task.Delay(1200);
                var results = new object[] { new { id = 1, name = "Sample Data" } };
                return BackendServiceResult<object[]>.CreateSuccess(results);
            }
            catch (Exception ex)
            {
                return BackendServiceResult<object[]>.CreateError(ex.Message);
            }
        }

        public void Dispose()
        {
            _httpClient?.Dispose();
        }
    }
}