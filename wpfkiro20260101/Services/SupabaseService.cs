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
        private readonly AppSettings _settings;
        private readonly HttpClient _httpClient;

        public string ServiceName => "Supabase";
        public BackendServiceType ServiceType => BackendServiceType.Supabase;

        public SupabaseService()
        {
            _settings = AppSettings.Instance;
            _httpClient = new HttpClient();
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

                // 測試 Supabase REST API
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

        // 食品訂閱相關方法
        public async Task<BackendServiceResult<object[]>> GetFoodSubscriptionsAsync()
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

                // 假設 Supabase 中有一個名為 food_subscriptions 的表
                var response = await _httpClient.GetAsync($"{_settings.ApiUrl}/rest/v1/food_subscriptions");
                
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
                            foodName = item.TryGetProperty("food_name", out var foodName) ? foodName.GetString() : "",
                            stringToDate = item.TryGetProperty("string_to_date", out var stringToDate) ? stringToDate.GetString() : "",
                            dateTime = item.TryGetProperty("date_time", out var dateTime) && DateTime.TryParse(dateTime.GetString(), out var dt) ? dt : DateTime.Now,
                            photo = item.TryGetProperty("photo", out var photo) ? photo.GetString() : "",
                            price = item.TryGetProperty("price", out var price) ? price.GetInt32() : 0,
                            shop = item.TryGetProperty("shop", out var shop) ? shop.GetString() : "",
                            photoHash = item.TryGetProperty("photo_hash", out var photoHash) ? photoHash.GetString() : ""
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
                return BackendServiceResult<object[]>.CreateError($"載入 Supabase 食品訂閱資料失敗：{ex.Message}");
            }
        }

        public async Task<BackendServiceResult<object>> CreateFoodSubscriptionAsync(object subscriptionData)
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
                
                if (subscriptionData is Models.FoodSubscription foodSub)
                {
                    data["food_name"] = foodSub.FoodName;
                    data["string_to_date"] = foodSub.StringToDate;
                    data["date_time"] = foodSub.DateTime.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
                    data["photo"] = foodSub.Photo;
                    data["price"] = foodSub.Price;
                    data["shop"] = foodSub.Shop;
                    data["photo_hash"] = foodSub.PhotoHash;
                }

                var json = JsonSerializer.Serialize(data);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync($"{_settings.ApiUrl}/rest/v1/food_subscriptions", content);
                
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
                    return BackendServiceResult<object>.CreateError($"Supabase 創建失敗：{response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                return BackendServiceResult<object>.CreateError($"創建 Supabase 食品訂閱失敗：{ex.Message}");
            }
        }

        // Supabase 特定的方法
        public async Task<BackendServiceResult<string>> SignUpAsync(string email, string password)
        {
            try
            {
                await Task.Delay(1000);
                return BackendServiceResult<string>.CreateSuccess("user-id-456");
            }
            catch (Exception ex)
            {
                return BackendServiceResult<string>.CreateError(ex.Message);
            }
        }
    }
}