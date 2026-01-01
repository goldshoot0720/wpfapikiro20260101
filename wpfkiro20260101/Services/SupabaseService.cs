using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace wpfkiro20260101.Services
{
    public class SupabaseService : IBackendService
    {
        private readonly HttpClient _httpClient;
        private readonly AppSettings _settings;

        public string ServiceName => "Supabase";
        public BackendServiceType ServiceType => BackendServiceType.Supabase;

        public SupabaseService()
        {
            _httpClient = new HttpClient();
            _settings = AppSettings.Instance;
        }

        public async Task<bool> TestConnectionAsync()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(_settings.ApiUrl) || 
                    string.IsNullOrWhiteSpace(_settings.ApiKey))
                {
                    return false;
                }

                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("apikey", _settings.ApiKey);
                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_settings.ApiKey}");

                var response = await _httpClient.GetAsync($"{_settings.ApiUrl}/rest/v1/");
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
                await Task.Delay(500);
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
                    string.IsNullOrWhiteSpace(_settings.ApiKey))
                {
                    return BackendServiceResult<object[]>.CreateError("Supabase 設定不完整");
                }

                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("apikey", _settings.ApiKey);
                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_settings.ApiKey}");

                var response = await _httpClient.GetAsync($"{_settings.ApiUrl}/rest/v1/foods");
                
                if (response.IsSuccessStatusCode)
                {
                    var jsonContent = await response.Content.ReadAsStringAsync();
                    var data = JsonSerializer.Deserialize<JsonElement[]>(jsonContent);
                    
                    var foods = new List<object>();
                    foreach (var item in data)
                    {
                        foods.Add(new
                        {
                            id = item.TryGetProperty("id", out var id) ? id.GetString() : "",
                            foodName = item.TryGetProperty("food_name", out var foodName) ? foodName.GetString() : "",
                            price = item.TryGetProperty("price", out var price) ? (price.ValueKind == JsonValueKind.Number ? price.GetInt32() : 0) : 0,
                            photo = item.TryGetProperty("photo", out var photo) ? photo.GetString() : "",
                            photoHash = item.TryGetProperty("photo_hash", out var photoHash) ? photoHash.GetString() : "",
                            shop = item.TryGetProperty("shop", out var shop) ? shop.GetString() : "",
                            toDate = item.TryGetProperty("to_date", out var toDate) ? toDate.GetString() : "",
                            description = item.TryGetProperty("description", out var description) ? description.GetString() : "",
                            category = item.TryGetProperty("category", out var category) ? category.GetString() : "",
                            storageLocation = item.TryGetProperty("storage_location", out var storageLocation) ? storageLocation.GetString() : "",
                            note = item.TryGetProperty("note", out var note) ? note.GetString() : "",
                            createdAt = item.TryGetProperty("created_at", out var createdAt) ? createdAt.GetString() : "",
                            updatedAt = item.TryGetProperty("updated_at", out var updatedAt) ? updatedAt.GetString() : ""
                        });
                    }
                    
                    return BackendServiceResult<object[]>.CreateSuccess(foods.ToArray());
                }
                else
                {
                    return BackendServiceResult<object[]>.CreateError($"Supabase API 錯誤：{response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                return BackendServiceResult<object[]>.CreateError($"載入 Supabase 食品資料失敗：{ex.Message}");
            }
        }

        public async Task<BackendServiceResult<object>> CreateFoodAsync(object foodData)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(_settings.ApiUrl) || 
                    string.IsNullOrWhiteSpace(_settings.ApiKey))
                {
                    return BackendServiceResult<object>.CreateError("Supabase 設定不完整");
                }

                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("apikey", _settings.ApiKey);
                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_settings.ApiKey}");

                var data = new Dictionary<string, object>();
                
                if (foodData is Models.Food food)
                {
                    data["food_name"] = food.FoodName;
                    data["price"] = food.Price;
                    data["quantity"] = food.Quantity;
                    data["photo"] = food.Photo;
                    data["photo_hash"] = food.PhotoHash;
                    data["shop"] = food.Shop;
                    data["to_date"] = food.ToDate;
                    data["description"] = food.Description;
                    data["category"] = food.Category;
                    data["storage_location"] = food.StorageLocation;
                    data["note"] = food.Note;
                    data["created_at"] = food.CreatedAt.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
                    data["updated_at"] = food.UpdatedAt.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
                }

                var json = JsonSerializer.Serialize(data);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync($"{_settings.ApiUrl}/rest/v1/foods", content);
                
                if (response.IsSuccessStatusCode)
                {
                    return BackendServiceResult<object>.CreateSuccess(new
                    {
                        data = foodData
                    });
                }
                else
                {
                    return BackendServiceResult<object>.CreateError($"Supabase 創建失敗：{response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                return BackendServiceResult<object>.CreateError($"創建 Supabase 食品失敗：{ex.Message}");
            }
        }

        public async Task<BackendServiceResult<object>> UpdateFoodAsync(string id, object foodData)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(_settings.ApiUrl) || 
                    string.IsNullOrWhiteSpace(_settings.ApiKey))
                {
                    return BackendServiceResult<object>.CreateError("Supabase 設定不完整");
                }

                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("apikey", _settings.ApiKey);
                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_settings.ApiKey}");

                var data = new Dictionary<string, object>();
                
                if (foodData is Models.Food food)
                {
                    data["food_name"] = food.FoodName;
                    data["price"] = food.Price;
                    data["quantity"] = food.Quantity;
                    data["photo"] = food.Photo;
                    data["photo_hash"] = food.PhotoHash;
                    data["shop"] = food.Shop;
                    data["to_date"] = food.ToDate;
                    data["description"] = food.Description;
                    data["category"] = food.Category;
                    data["storage_location"] = food.StorageLocation;
                    data["note"] = food.Note;
                    data["updated_at"] = food.UpdatedAt.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
                }

                var json = JsonSerializer.Serialize(data);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PatchAsync($"{_settings.ApiUrl}/rest/v1/foods?id=eq.{id}", content);
                
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
                    return BackendServiceResult<object>.CreateError($"Supabase 更新失敗：{response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                return BackendServiceResult<object>.CreateError($"更新 Supabase 食品失敗：{ex.Message}");
            }
        }

        public async Task<BackendServiceResult<bool>> DeleteFoodAsync(string id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(_settings.ApiUrl) || 
                    string.IsNullOrWhiteSpace(_settings.ApiKey))
                {
                    return BackendServiceResult<bool>.CreateError("Supabase 設定不完整");
                }

                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("apikey", _settings.ApiKey);
                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_settings.ApiKey}");

                var response = await _httpClient.DeleteAsync($"{_settings.ApiUrl}/rest/v1/foods?id=eq.{id}");
                
                if (response.IsSuccessStatusCode)
                {
                    return BackendServiceResult<bool>.CreateSuccess(true);
                }
                else
                {
                    return BackendServiceResult<bool>.CreateError($"Supabase 刪除失敗：{response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                return BackendServiceResult<bool>.CreateError($"刪除 Supabase 食品失敗：{ex.Message}");
            }
        }

        // Subscription CRUD operations
        public async Task<BackendServiceResult<object[]>> GetSubscriptionsAsync()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(_settings.ApiUrl) || 
                    string.IsNullOrWhiteSpace(_settings.ApiKey))
                {
                    return BackendServiceResult<object[]>.CreateError("Supabase 設定不完整");
                }

                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("apikey", _settings.ApiKey);
                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_settings.ApiKey}");

                var response = await _httpClient.GetAsync($"{_settings.ApiUrl}/rest/v1/subscriptions");
                
                if (response.IsSuccessStatusCode)
                {
                    var jsonContent = await response.Content.ReadAsStringAsync();
                    var data = JsonSerializer.Deserialize<JsonElement[]>(jsonContent);
                    
                    var subscriptions = new List<object>();
                    foreach (var item in data)
                    {
                        subscriptions.Add(new
                        {
                            id = item.TryGetProperty("id", out var id) ? id.GetString() : "",
                            subscriptionName = item.TryGetProperty("subscription_name", out var subscriptionName) ? subscriptionName.GetString() : "",
                            nextDate = item.TryGetProperty("next_date", out var nextDate) ? nextDate.GetString() : "",
                            price = item.TryGetProperty("price", out var price) ? price.GetInt32() : 0,
                            site = item.TryGetProperty("site", out var site) ? site.GetString() : "",
                            account = item.TryGetProperty("account", out var account) ? account.GetString() : "",
                            note = item.TryGetProperty("note", out var note) ? note.GetString() : "",
                            stringToDate = item.TryGetProperty("string_to_date", out var stringToDate) ? stringToDate.GetString() : "",
                            dateTime = item.TryGetProperty("date_time", out var dateTime) ? dateTime.GetString() : "",
                            foodId = item.TryGetProperty("food_id", out var foodId) ? foodId.GetString() : null,
                            createdAt = item.TryGetProperty("created_at", out var createdAt) ? createdAt.GetString() : "",
                            updatedAt = item.TryGetProperty("updated_at", out var updatedAt) ? updatedAt.GetString() : ""
                        });
                    }
                    
                    return BackendServiceResult<object[]>.CreateSuccess(subscriptions.ToArray());
                }
                else
                {
                    return BackendServiceResult<object[]>.CreateError($"Supabase API 錯誤：{response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                return BackendServiceResult<object[]>.CreateError($"載入 Supabase 訂閱資料失敗：{ex.Message}");
            }
        }

        public async Task<BackendServiceResult<object>> CreateSubscriptionAsync(object subscriptionData)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(_settings.ApiUrl) || 
                    string.IsNullOrWhiteSpace(_settings.ApiKey))
                {
                    return BackendServiceResult<object>.CreateError("Supabase 設定不完整");
                }

                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("apikey", _settings.ApiKey);
                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_settings.ApiKey}");

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
                    data["date_time"] = subscription.DateTime.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
                    if (!string.IsNullOrEmpty(subscription.FoodId))
                    {
                        data["food_id"] = subscription.FoodId;
                    }
                }

                var json = JsonSerializer.Serialize(data);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync($"{_settings.ApiUrl}/rest/v1/subscriptions", content);
                
                if (response.IsSuccessStatusCode)
                {
                    return BackendServiceResult<object>.CreateSuccess(new
                    {
                        data = subscriptionData
                    });
                }
                else
                {
                    return BackendServiceResult<object>.CreateError($"Supabase 創建失敗：{response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                return BackendServiceResult<object>.CreateError($"創建 Supabase 訂閱失敗：{ex.Message}");
            }
        }

        public async Task<BackendServiceResult<object>> UpdateSubscriptionAsync(string id, object subscriptionData)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(_settings.ApiUrl) || 
                    string.IsNullOrWhiteSpace(_settings.ApiKey))
                {
                    return BackendServiceResult<object>.CreateError("Supabase 設定不完整");
                }

                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("apikey", _settings.ApiKey);
                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_settings.ApiKey}");

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
                    data["date_time"] = subscription.DateTime.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
                    if (!string.IsNullOrEmpty(subscription.FoodId))
                    {
                        data["food_id"] = subscription.FoodId;
                    }
                }

                var json = JsonSerializer.Serialize(data);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PatchAsync($"{_settings.ApiUrl}/rest/v1/subscriptions?id=eq.{id}", content);
                
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
                    return BackendServiceResult<object>.CreateError($"Supabase 更新失敗：{response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                return BackendServiceResult<object>.CreateError($"更新 Supabase 訂閱失敗：{ex.Message}");
            }
        }

        public async Task<BackendServiceResult<bool>> DeleteSubscriptionAsync(string id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(_settings.ApiUrl) || 
                    string.IsNullOrWhiteSpace(_settings.ApiKey))
                {
                    return BackendServiceResult<bool>.CreateError("Supabase 設定不完整");
                }

                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("apikey", _settings.ApiKey);
                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_settings.ApiKey}");

                var response = await _httpClient.DeleteAsync($"{_settings.ApiUrl}/rest/v1/subscriptions?id=eq.{id}");
                
                if (response.IsSuccessStatusCode)
                {
                    return BackendServiceResult<bool>.CreateSuccess(true);
                }
                else
                {
                    return BackendServiceResult<bool>.CreateError($"Supabase 刪除失敗：{response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                return BackendServiceResult<bool>.CreateError($"刪除 Supabase 訂閱失敗：{ex.Message}");
            }
        }

        public void Dispose()
        {
            _httpClient?.Dispose();
        }
    }
}