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

        // 食品訂閱相關方法
        public async Task<BackendServiceResult<object[]>> GetFoodSubscriptionsAsync()
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

                // 假設 MySQL REST API 端點
                var response = await _httpClient.GetAsync($"{_settings.ApiUrl}/api/databases/{_settings.ProjectId}/tables/food_subscriptions/rows");
                
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
                                foodName = row.TryGetProperty("food_name", out var foodName) ? foodName.GetString() : "",
                                stringToDate = row.TryGetProperty("string_to_date", out var stringToDate) ? stringToDate.GetString() : "",
                                dateTime = row.TryGetProperty("date_time", out var dateTime) && DateTime.TryParse(dateTime.GetString(), out var dt) ? dt : DateTime.Now,
                                photo = row.TryGetProperty("photo", out var photo) ? photo.GetString() : "",
                                price = row.TryGetProperty("price", out var price) ? price.GetInt32() : 0,
                                shop = row.TryGetProperty("shop", out var shop) ? shop.GetString() : "",
                                photoHash = row.TryGetProperty("photo_hash", out var photoHash) ? photoHash.GetString() : "",
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
                return BackendServiceResult<object[]>.CreateError($"載入 MySQL 食品訂閱資料失敗：{ex.Message}");
            }
        }

        public async Task<BackendServiceResult<object>> CreateFoodSubscriptionAsync(object subscriptionData)
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
                
                if (subscriptionData is Models.FoodSubscription foodSub)
                {
                    data["food_name"] = foodSub.FoodName;
                    data["string_to_date"] = foodSub.StringToDate;
                    data["date_time"] = foodSub.DateTime.ToString("yyyy-MM-dd HH:mm:ss");
                    data["photo"] = foodSub.Photo;
                    data["price"] = foodSub.Price;
                    data["shop"] = foodSub.Shop;
                    data["photo_hash"] = foodSub.PhotoHash;
                }

                var json = JsonSerializer.Serialize(data);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync($"{_settings.ApiUrl}/api/databases/{_settings.ProjectId}/tables/food_subscriptions/rows", content);
                
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
                return BackendServiceResult<object>.CreateError($"創建 MySQL 食品訂閱失敗：{ex.Message}");
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