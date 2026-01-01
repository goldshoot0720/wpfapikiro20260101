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

        // 食品訂閱相關方法
        public async Task<BackendServiceResult<object[]>> GetFoodSubscriptionsAsync()
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

                // 假設 Back4App 中有一個名為 FoodSubscription 的類別
                var response = await _httpClient.GetAsync($"{_settings.ApiUrl}/classes/FoodSubscription");
                
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
                                foodName = item.TryGetProperty("foodName", out var foodName) ? foodName.GetString() : "",
                                stringToDate = item.TryGetProperty("stringToDate", out var stringToDate) ? stringToDate.GetString() : "",
                                dateTime = item.TryGetProperty("dateTime", out var dateTime) && DateTime.TryParse(dateTime.GetString(), out var dt) ? dt : DateTime.Now,
                                photo = item.TryGetProperty("photo", out var photo) ? photo.GetString() : "",
                                price = item.TryGetProperty("price", out var price) ? price.GetInt32() : 0,
                                shop = item.TryGetProperty("shop", out var shop) ? shop.GetString() : "",
                                photoHash = item.TryGetProperty("photoHash", out var photoHash) ? photoHash.GetString() : "",
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
                return BackendServiceResult<object[]>.CreateError($"載入 Back4App 食品訂閱資料失敗：{ex.Message}");
            }
        }

        public async Task<BackendServiceResult<object>> CreateFoodSubscriptionAsync(object subscriptionData)
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
                
                if (subscriptionData is Models.FoodSubscription foodSub)
                {
                    data["foodName"] = foodSub.FoodName;
                    data["stringToDate"] = foodSub.StringToDate;
                    data["dateTime"] = new { __type = "Date", iso = foodSub.DateTime.ToString("yyyy-MM-ddTHH:mm:ss.fffZ") };
                    data["photo"] = foodSub.Photo;
                    data["price"] = foodSub.Price;
                    data["shop"] = foodSub.Shop;
                    data["photoHash"] = foodSub.PhotoHash;
                }

                var json = JsonSerializer.Serialize(data);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync($"{_settings.ApiUrl}/classes/FoodSubscription", content);
                
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
                return BackendServiceResult<object>.CreateError($"創建 Back4App 食品訂閱失敗：{ex.Message}");
            }
        }

        public void Dispose()
        {
            _httpClient?.Dispose();
        }
    }
}