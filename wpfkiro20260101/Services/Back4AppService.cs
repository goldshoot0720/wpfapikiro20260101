using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace wpfkiro20260101.Services
{
    public class Back4AppService : IBackendService
    {
        private readonly HttpClient _httpClient;
        private readonly AppSettings _settings;

        public string ServiceName => "Back4App";
        public BackendServiceType ServiceType => BackendServiceType.Back4App;

        public Back4AppService()
        {
            _httpClient = new HttpClient();
            _settings = AppSettings.Instance;
        }

        public async Task<bool> TestConnectionAsync()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(_settings.ApiUrl) ||
                    string.IsNullOrWhiteSpace(_settings.ProjectId) ||
                    string.IsNullOrWhiteSpace(_settings.ApiKey))
                {
                    return false;
                }

                // 設定 Back4App 的標頭
                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("X-Parse-Application-Id", _settings.ProjectId);
                _httpClient.DefaultRequestHeaders.Add("X-Parse-REST-API-Key", _settings.ApiKey);

                // 測試連線 - 嘗試獲取應用程式資訊
                var response = await _httpClient.GetAsync($"{_settings.ApiUrl}/classes/_User?limit=0");
                
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
                if (string.IsNullOrWhiteSpace(_settings.ApiUrl) ||
                    string.IsNullOrWhiteSpace(_settings.ProjectId) ||
                    string.IsNullOrWhiteSpace(_settings.ApiKey))
                {
                    return false;
                }

                // 設定 HTTP 客戶端
                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("X-Parse-Application-Id", _settings.ProjectId);
                _httpClient.DefaultRequestHeaders.Add("X-Parse-REST-API-Key", _settings.ApiKey);

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
                    string.IsNullOrWhiteSpace(_settings.ProjectId) ||
                    string.IsNullOrWhiteSpace(_settings.ApiKey))
                {
                    return BackendServiceResult<object[]>.CreateError("Back4App 設定不完整");
                }

                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("X-Parse-Application-Id", _settings.ProjectId);
                _httpClient.DefaultRequestHeaders.Add("X-Parse-REST-API-Key", _settings.ApiKey);

                var response = await _httpClient.GetAsync($"{_settings.ApiUrl}/classes/Food");
                
                if (response.IsSuccessStatusCode)
                {
                    var jsonContent = await response.Content.ReadAsStringAsync();
                    var parseResponse = JsonSerializer.Deserialize<JsonElement>(jsonContent);
                    
                    if (parseResponse.TryGetProperty("results", out var results))
                    {
                        var foods = new List<object>();
                        foreach (var item in results.EnumerateArray())
                        {
                            foods.Add(new
                            {
                                id = item.TryGetProperty("objectId", out var id) ? id.GetString() : "",
                                foodName = item.TryGetProperty("foodName", out var foodName) ? foodName.GetString() : "",
                                photo = item.TryGetProperty("photo", out var photo) ? photo.GetString() : "",
                                photoHash = item.TryGetProperty("photoHash", out var photoHash) ? photoHash.GetString() : "",
                                shop = item.TryGetProperty("shop", out var shop) ? shop.GetString() : "",
                                note = item.TryGetProperty("note", out var note) ? note.GetString() : "",
                                createdAt = item.TryGetProperty("createdAt", out var createdAt) ? createdAt.GetString() : "",
                                updatedAt = item.TryGetProperty("updatedAt", out var updatedAt) ? updatedAt.GetString() : ""
                            });
                        }
                        
                        return BackendServiceResult<object[]>.CreateSuccess(foods.ToArray());
                    }
                    else
                    {
                        return BackendServiceResult<object[]>.CreateSuccess(new object[0]);
                    }
                }
                else
                {
                    return BackendServiceResult<object[]>.CreateError($"Back4App API 錯誤：{response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                return BackendServiceResult<object[]>.CreateError($"載入 Back4App 食品資料失敗：{ex.Message}");
            }
        }

        public async Task<BackendServiceResult<object>> CreateFoodAsync(object foodData)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(_settings.ApiUrl) ||
                    string.IsNullOrWhiteSpace(_settings.ProjectId) ||
                    string.IsNullOrWhiteSpace(_settings.ApiKey))
                {
                    return BackendServiceResult<object>.CreateError("Back4App 設定不完整");
                }

                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("X-Parse-Application-Id", _settings.ProjectId);
                _httpClient.DefaultRequestHeaders.Add("X-Parse-REST-API-Key", _settings.ApiKey);

                var data = new Dictionary<string, object>();
                
                if (foodData is Models.Food food)
                {
                    data["foodName"] = food.FoodName;
                    data["photo"] = food.Photo;
                    data["photoHash"] = food.PhotoHash;
                    data["shop"] = food.Shop;
                    data["note"] = food.Note;
                }

                var json = JsonSerializer.Serialize(data);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync($"{_settings.ApiUrl}/classes/Food", content);
                
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<JsonElement>(responseContent);
                    
                    return BackendServiceResult<object>.CreateSuccess(new
                    {
                        id = result.TryGetProperty("objectId", out var id) ? id.GetString() : "",
                        data = foodData,
                        createdAt = result.TryGetProperty("createdAt", out var createdAt) ? createdAt.GetString() : ""
                    });
                }
                else
                {
                    return BackendServiceResult<object>.CreateError($"Back4App 創建失敗：{response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                return BackendServiceResult<object>.CreateError($"創建 Back4App 食品失敗：{ex.Message}");
            }
        }

        public async Task<BackendServiceResult<object>> UpdateFoodAsync(string id, object foodData)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(_settings.ApiUrl) ||
                    string.IsNullOrWhiteSpace(_settings.ProjectId) ||
                    string.IsNullOrWhiteSpace(_settings.ApiKey))
                {
                    return BackendServiceResult<object>.CreateError("Back4App 設定不完整");
                }

                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("X-Parse-Application-Id", _settings.ProjectId);
                _httpClient.DefaultRequestHeaders.Add("X-Parse-REST-API-Key", _settings.ApiKey);

                var data = new Dictionary<string, object>();
                
                if (foodData is Models.Food food)
                {
                    data["foodName"] = food.FoodName;
                    data["photo"] = food.Photo;
                    data["photoHash"] = food.PhotoHash;
                    data["shop"] = food.Shop;
                    data["note"] = food.Note;
                }

                var json = JsonSerializer.Serialize(data);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync($"{_settings.ApiUrl}/classes/Food/{id}", content);
                
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<JsonElement>(responseContent);
                    
                    return BackendServiceResult<object>.CreateSuccess(new
                    {
                        id = id,
                        data = foodData,
                        updatedAt = result.TryGetProperty("updatedAt", out var updatedAt) ? updatedAt.GetString() : ""
                    });
                }
                else
                {
                    return BackendServiceResult<object>.CreateError($"Back4App 更新失敗：{response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                return BackendServiceResult<object>.CreateError($"更新 Back4App 食品失敗：{ex.Message}");
            }
        }

        public async Task<BackendServiceResult<bool>> DeleteFoodAsync(string id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(_settings.ApiUrl) ||
                    string.IsNullOrWhiteSpace(_settings.ProjectId) ||
                    string.IsNullOrWhiteSpace(_settings.ApiKey))
                {
                    return BackendServiceResult<bool>.CreateError("Back4App 設定不完整");
                }

                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("X-Parse-Application-Id", _settings.ProjectId);
                _httpClient.DefaultRequestHeaders.Add("X-Parse-REST-API-Key", _settings.ApiKey);

                var response = await _httpClient.DeleteAsync($"{_settings.ApiUrl}/classes/Food/{id}");
                
                if (response.IsSuccessStatusCode)
                {
                    return BackendServiceResult<bool>.CreateSuccess(true);
                }
                else
                {
                    return BackendServiceResult<bool>.CreateError($"Back4App 刪除失敗：{response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                return BackendServiceResult<bool>.CreateError($"刪除 Back4App 食品失敗：{ex.Message}");
            }
        }

        public void Dispose()
        {
            _httpClient?.Dispose();
        }

        public async Task<BackendServiceResult<object[]>> GetSubscriptionsAsync()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(_settings.ApiUrl) ||
                    string.IsNullOrWhiteSpace(_settings.ProjectId) ||
                    string.IsNullOrWhiteSpace(_settings.ApiKey))
                {
                    return BackendServiceResult<object[]>.CreateError("Back4App 設定不完整");
                }

                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("X-Parse-Application-Id", _settings.ProjectId);
                _httpClient.DefaultRequestHeaders.Add("X-Parse-REST-API-Key", _settings.ApiKey);

                // 假設 Back4App 中有一個名為 Subscription 的類別
                var response = await _httpClient.GetAsync($"{_settings.ApiUrl}/classes/Subscription");
                
                if (response.IsSuccessStatusCode)
                {
                    var jsonContent = await response.Content.ReadAsStringAsync();
                    var parseResponse = JsonSerializer.Deserialize<JsonElement>(jsonContent);
                    
                    if (parseResponse.TryGetProperty("results", out var results))
                    {
                        var subscriptions = new List<object>();
                        foreach (var item in results.EnumerateArray())
                        {
                            subscriptions.Add(new
                            {
                                id = item.TryGetProperty("objectId", out var id) ? id.GetString() : "",
                                name = item.TryGetProperty("name", out var name) ? name.GetString() : "",
                                nextDate = item.TryGetProperty("nextDate", out var nextDate) ? nextDate.GetString() : "",
                                price = item.TryGetProperty("price", out var price) ? price.GetInt32() : 0,
                                site = item.TryGetProperty("site", out var site) ? site.GetString() : "",
                                note = item.TryGetProperty("note", out var note) ? note.GetString() : "",
                                account = item.TryGetProperty("account", out var account) ? account.GetString() : "",
                                createdAt = item.TryGetProperty("createdAt", out var createdAt) ? createdAt.GetString() : "",
                                updatedAt = item.TryGetProperty("updatedAt", out var updatedAt) ? updatedAt.GetString() : ""
                            });
                        }
                        
                        return BackendServiceResult<object[]>.CreateSuccess(subscriptions.ToArray());
                    }
                    else
                    {
                        return BackendServiceResult<object[]>.CreateSuccess(new object[0]);
                    }
                }
                else
                {
                    return BackendServiceResult<object[]>.CreateError($"Back4App API 錯誤：{response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                return BackendServiceResult<object[]>.CreateError($"載入 Back4App 訂閱資料失敗：{ex.Message}");
            }
        }

        public async Task<BackendServiceResult<object>> CreateSubscriptionAsync(object subscriptionData)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(_settings.ApiUrl) ||
                    string.IsNullOrWhiteSpace(_settings.ProjectId) ||
                    string.IsNullOrWhiteSpace(_settings.ApiKey))
                {
                    return BackendServiceResult<object>.CreateError("Back4App 設定不完整");
                }

                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("X-Parse-Application-Id", _settings.ProjectId);
                _httpClient.DefaultRequestHeaders.Add("X-Parse-REST-API-Key", _settings.ApiKey);

                var data = new Dictionary<string, object>();
                
                if (subscriptionData is Models.Subscription subscription)
                {
                    data["subscriptionName"] = subscription.SubscriptionName;
                    data["nextDate"] = new { __type = "Date", iso = subscription.NextDate.ToString("yyyy-MM-ddTHH:mm:ss.fffZ") };
                    data["price"] = subscription.Price;
                    data["site"] = subscription.Site;
                    data["account"] = subscription.Account;
                    data["note"] = subscription.Note;
                    data["stringToDate"] = subscription.StringToDate;
                    data["dateTime"] = new { __type = "Date", iso = subscription.DateTime.ToString("yyyy-MM-ddTHH:mm:ss.fffZ") };
                    if (!string.IsNullOrEmpty(subscription.FoodId))
                    {
                        data["foodId"] = subscription.FoodId;
                    }
                }

                var json = JsonSerializer.Serialize(data);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync($"{_settings.ApiUrl}/classes/Subscription", content);
                
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<JsonElement>(responseContent);
                    
                    return BackendServiceResult<object>.CreateSuccess(new
                    {
                        id = result.TryGetProperty("objectId", out var id) ? id.GetString() : "",
                        data = subscriptionData,
                        createdAt = result.TryGetProperty("createdAt", out var createdAt) ? createdAt.GetString() : ""
                    });
                }
                else
                {
                    return BackendServiceResult<object>.CreateError($"Back4App 創建失敗：{response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                return BackendServiceResult<object>.CreateError($"創建 Back4App 訂閱失敗：{ex.Message}");
            }
        }

        public async Task<BackendServiceResult<object>> UpdateSubscriptionAsync(string id, object subscriptionData)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(_settings.ApiUrl) ||
                    string.IsNullOrWhiteSpace(_settings.ProjectId) ||
                    string.IsNullOrWhiteSpace(_settings.ApiKey))
                {
                    return BackendServiceResult<object>.CreateError("Back4App 設定不完整");
                }

                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("X-Parse-Application-Id", _settings.ProjectId);
                _httpClient.DefaultRequestHeaders.Add("X-Parse-REST-API-Key", _settings.ApiKey);

                var data = new Dictionary<string, object>();
                
                if (subscriptionData is Models.Subscription subscription)
                {
                    data["subscriptionName"] = subscription.SubscriptionName;
                    data["nextDate"] = new { __type = "Date", iso = subscription.NextDate.ToString("yyyy-MM-ddTHH:mm:ss.fffZ") };
                    data["price"] = subscription.Price;
                    data["site"] = subscription.Site;
                    data["account"] = subscription.Account;
                    data["note"] = subscription.Note;
                    data["stringToDate"] = subscription.StringToDate;
                    data["dateTime"] = new { __type = "Date", iso = subscription.DateTime.ToString("yyyy-MM-ddTHH:mm:ss.fffZ") };
                    if (!string.IsNullOrEmpty(subscription.FoodId))
                    {
                        data["foodId"] = subscription.FoodId;
                    }
                }

                var json = JsonSerializer.Serialize(data);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync($"{_settings.ApiUrl}/classes/Subscription/{id}", content);
                
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<JsonElement>(responseContent);
                    
                    return BackendServiceResult<object>.CreateSuccess(new
                    {
                        id = id,
                        data = subscriptionData,
                        updatedAt = result.TryGetProperty("updatedAt", out var updatedAt) ? updatedAt.GetString() : ""
                    });
                }
                else
                {
                    return BackendServiceResult<object>.CreateError($"Back4App 更新失敗：{response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                return BackendServiceResult<object>.CreateError($"更新 Back4App 訂閱失敗：{ex.Message}");
            }
        }

        public async Task<BackendServiceResult<bool>> DeleteSubscriptionAsync(string id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(_settings.ApiUrl) ||
                    string.IsNullOrWhiteSpace(_settings.ProjectId) ||
                    string.IsNullOrWhiteSpace(_settings.ApiKey))
                {
                    return BackendServiceResult<bool>.CreateError("Back4App 設定不完整");
                }

                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("X-Parse-Application-Id", _settings.ProjectId);
                _httpClient.DefaultRequestHeaders.Add("X-Parse-REST-API-Key", _settings.ApiKey);

                var response = await _httpClient.DeleteAsync($"{_settings.ApiUrl}/classes/Subscription/{id}");
                
                if (response.IsSuccessStatusCode)
                {
                    return BackendServiceResult<bool>.CreateSuccess(true);
                }
                else
                {
                    return BackendServiceResult<bool>.CreateError($"Back4App 刪除失敗：{response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                return BackendServiceResult<bool>.CreateError($"刪除 Back4App 訂閱失敗：{ex.Message}");
            }
        }
    }
}