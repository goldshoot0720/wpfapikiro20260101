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

                // 先測試基本連接
                var response = await _httpClient.GetAsync($"{_settings.ApiUrl}/rest/v1/");
                
                if (!response.IsSuccessStatusCode)
                {
                    System.Diagnostics.Debug.WriteLine($"Supabase 基本連接失敗: {response.StatusCode}");
                    return false;
                }

                // 嘗試測試不同的資料表名稱
                var tableNames = new[] { "food", "foods", "Food", "Foods" };
                
                foreach (var tableName in tableNames)
                {
                    try
                    {
                        var testResponse = await _httpClient.GetAsync($"{_settings.ApiUrl}/rest/v1/{tableName}");
                        if (testResponse.IsSuccessStatusCode)
                        {
                            System.Diagnostics.Debug.WriteLine($"找到可用的資料表: {tableName}");
                            return true;
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine($"資料表 {tableName} 不可用: {testResponse.StatusCode}");
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"測試資料表 {tableName} 時發生錯誤: {ex.Message}");
                    }
                }

                return false;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Supabase 連接測試異常: {ex.Message}");
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
                _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
                _httpClient.DefaultRequestHeaders.Add("Content-Type", "application/json");

                var apiUrl = $"{_settings.ApiUrl}/rest/v1/food";
                System.Diagnostics.Debug.WriteLine($"嘗試連接 Supabase Food API: {apiUrl}");
                
                var response = await _httpClient.GetAsync(apiUrl);
                
                System.Diagnostics.Debug.WriteLine($"Food API 回應狀態: {response.StatusCode}");
                
                if (response.IsSuccessStatusCode)
                {
                    var jsonContent = await response.Content.ReadAsStringAsync();
                    System.Diagnostics.Debug.WriteLine($"Food API 成功，回應內容: {jsonContent}");
                    
                    var data = JsonSerializer.Deserialize<JsonElement[]>(jsonContent);
                    
                    var foods = new List<object>();
                    foreach (var item in data)
                    {
                        foods.Add(new
                        {
                            id = item.TryGetProperty("id", out var id) ? id.GetString() : "",
                            foodName = item.TryGetProperty("name", out var name) ? name.GetString() : "",
                            price = item.TryGetProperty("price", out var price) ? (price.ValueKind == JsonValueKind.Number ? price.GetInt32() : 0) : 0,
                            photo = item.TryGetProperty("photo", out var photo) ? photo.GetString() : "",
                            photoHash = "", // 不存在於實際資料表中
                            shop = item.TryGetProperty("shop", out var shop) ? shop.GetString() : "",
                            toDate = item.TryGetProperty("todate", out var toDate) ? toDate.GetString() : "",
                            description = "", // 不存在於實際資料表中
                            category = "", // 不存在於實際資料表中
                            storageLocation = "", // 不存在於實際資料表中
                            note = "", // 不存在於實際資料表中
                            account = item.TryGetProperty("account", out var account) ? account.GetString() : "",
                            createdAt = item.TryGetProperty("created_at", out var createdAt) ? createdAt.GetString() : "",
                            updatedAt = item.TryGetProperty("updated_at", out var updatedAt) ? updatedAt.GetString() : ""
                        });
                    }
                    
                    return BackendServiceResult<object[]>.CreateSuccess(foods.ToArray());
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    System.Diagnostics.Debug.WriteLine($"Food API 錯誤: {response.StatusCode} - {errorContent}");
                    return BackendServiceResult<object[]>.CreateError($"Supabase 拒絕要求：{response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Supabase Food API 連接異常: {ex.Message}");
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
                    data["name"] = food.FoodName;
                    data["price"] = food.Price;
                    data["photo"] = food.Photo;
                    data["shop"] = food.Shop;
                    data["todate"] = food.ToDate;
                    data["account"] = ""; // 新欄位，暫時留空
                }

                var json = JsonSerializer.Serialize(data);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync($"{_settings.ApiUrl}/rest/v1/food", content);
                
                if (response.IsSuccessStatusCode)
                {
                    return BackendServiceResult<object>.CreateSuccess(new
                    {
                        data = foodData
                    });
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    System.Diagnostics.Debug.WriteLine($"Create Food API 錯誤: {response.StatusCode} - {errorContent}");
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
                    data["name"] = food.FoodName;
                    data["price"] = food.Price;
                    data["photo"] = food.Photo;
                    data["shop"] = food.Shop;
                    data["todate"] = food.ToDate;
                    data["account"] = ""; // 新欄位，暫時留空
                }

                var json = JsonSerializer.Serialize(data);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PatchAsync($"{_settings.ApiUrl}/rest/v1/food?id=eq.{id}", content);
                
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
                    var errorContent = await response.Content.ReadAsStringAsync();
                    System.Diagnostics.Debug.WriteLine($"Update Food API 錯誤: {response.StatusCode} - {errorContent}");
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

                var response = await _httpClient.DeleteAsync($"{_settings.ApiUrl}/rest/v1/food?id=eq.{id}");
                
                if (response.IsSuccessStatusCode)
                {
                    return BackendServiceResult<bool>.CreateSuccess(true);
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    System.Diagnostics.Debug.WriteLine($"Delete Food API 錯誤: {response.StatusCode} - {errorContent}");
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
                _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
                _httpClient.DefaultRequestHeaders.Add("Content-Type", "application/json");

                var apiUrl = $"{_settings.ApiUrl}/rest/v1/subscription";
                System.Diagnostics.Debug.WriteLine($"嘗試連接 Supabase Subscription API: {apiUrl}");
                
                var response = await _httpClient.GetAsync(apiUrl);
                
                System.Diagnostics.Debug.WriteLine($"Subscription API 回應狀態: {response.StatusCode}");
                
                if (response.IsSuccessStatusCode)
                {
                    var jsonContent = await response.Content.ReadAsStringAsync();
                    System.Diagnostics.Debug.WriteLine($"Subscription API 成功，回應內容: {jsonContent}");
                    
                    var data = JsonSerializer.Deserialize<JsonElement[]>(jsonContent);
                    
                    var subscriptions = new List<object>();
                    foreach (var item in data)
                    {
                        subscriptions.Add(new
                        {
                            id = item.TryGetProperty("id", out var id) ? id.GetString() : "",
                            subscriptionName = item.TryGetProperty("name", out var name) ? name.GetString() : "",
                            nextDate = item.TryGetProperty("nextdate", out var nextDate) ? nextDate.GetString() : "",
                            price = item.TryGetProperty("price", out var price) ? price.GetInt32() : 0,
                            site = item.TryGetProperty("site", out var site) ? site.GetString() : "",
                            account = item.TryGetProperty("account", out var account) ? account.GetString() : "",
                            note = item.TryGetProperty("note", out var note) ? note.GetString() : "",
                            stringToDate = "", // 不存在於實際資料表中
                            dateTime = "", // 不存在於實際資料表中
                            foodId = (string?)null, // 不存在於實際資料表中
                            createdAt = item.TryGetProperty("created_at", out var createdAt) ? createdAt.GetString() : "",
                            updatedAt = item.TryGetProperty("updated_at", out var updatedAt) ? updatedAt.GetString() : ""
                        });
                    }
                    
                    return BackendServiceResult<object[]>.CreateSuccess(subscriptions.ToArray());
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    System.Diagnostics.Debug.WriteLine($"Subscription API 錯誤: {response.StatusCode} - {errorContent}");
                    return BackendServiceResult<object[]>.CreateError($"Supabase 拒絕要求：{response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Supabase Subscription API 連接異常: {ex.Message}");
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
                    data["name"] = subscription.SubscriptionName;
                    data["nextdate"] = subscription.NextDate.ToString("yyyy-MM-dd");
                    data["price"] = subscription.Price;
                    data["site"] = subscription.Site;
                    data["account"] = subscription.Account;
                    data["note"] = subscription.Note;
                }

                var json = JsonSerializer.Serialize(data);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync($"{_settings.ApiUrl}/rest/v1/subscription", content);
                
                if (response.IsSuccessStatusCode)
                {
                    return BackendServiceResult<object>.CreateSuccess(new
                    {
                        data = subscriptionData
                    });
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    System.Diagnostics.Debug.WriteLine($"Create Subscription API 錯誤: {response.StatusCode} - {errorContent}");
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
                    data["name"] = subscription.SubscriptionName;
                    data["nextdate"] = subscription.NextDate.ToString("yyyy-MM-dd");
                    data["price"] = subscription.Price;
                    data["site"] = subscription.Site;
                    data["account"] = subscription.Account;
                    data["note"] = subscription.Note;
                }

                var json = JsonSerializer.Serialize(data);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PatchAsync($"{_settings.ApiUrl}/rest/v1/subscription?id=eq.{id}", content);
                
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
                    var errorContent = await response.Content.ReadAsStringAsync();
                    System.Diagnostics.Debug.WriteLine($"Update Subscription API 錯誤: {response.StatusCode} - {errorContent}");
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

                var response = await _httpClient.DeleteAsync($"{_settings.ApiUrl}/rest/v1/subscription?id=eq.{id}");
                
                if (response.IsSuccessStatusCode)
                {
                    return BackendServiceResult<bool>.CreateSuccess(true);
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    System.Diagnostics.Debug.WriteLine($"Delete Subscription API 錯誤: {response.StatusCode} - {errorContent}");
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