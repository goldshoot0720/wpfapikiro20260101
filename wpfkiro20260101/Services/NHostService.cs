using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace wpfkiro20260101.Services
{
    public class NHostService : IBackendService
    {
        private readonly AppSettings _settings;
        private readonly HttpClient _httpClient;
        
        // 最簡化的連線設定欄位
        private readonly string _graphqlUrl = "https://uxgwdiuehabbzenwtcqo.hasura.eu-central-1.nhost.run/v1/graphql";
        private readonly string _adminSecret = "cu#34&yjF3Cr%fgxB#WA,4r4^c=Igcwr";

        public string ServiceName => "NHost";
        public BackendServiceType ServiceType => BackendServiceType.NHost;

        public NHostService()
        {
            _settings = AppSettings.Instance;
            _httpClient = new HttpClient();
            
            // 設定預設標頭
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "wpfkiro20260101/1.0");
            
            // 設定 Hasura Admin Secret 標頭
            _httpClient.DefaultRequestHeaders.Add("x-hasura-admin-secret", _adminSecret);
        }

        public async Task<bool> TestConnectionAsync()
        {
            try
            {
                // 測試 NHost 健康檢查端點
                var healthCheckUrl = "https://uxgwdiuehabbzenwtcqo.functions.eu-central-1.nhost.run/v1/healthz";
                
                using var request = new HttpRequestMessage(HttpMethod.Get, healthCheckUrl);
                using var response = await _httpClient.SendAsync(request);
                
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"NHost 連線測試失敗: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> InitializeAsync()
        {
            try
            {
                // 初始化 NHost 客戶端配置
                System.Diagnostics.Debug.WriteLine($"初始化 NHost 服務");
                System.Diagnostics.Debug.WriteLine($"GraphQL URL: {_graphqlUrl}");
                System.Diagnostics.Debug.WriteLine($"Admin Secret: 已設定");
                
                // 測試連線
                return await TestConnectionAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"NHost 初始化失敗: {ex.Message}");
                return false;
            }
        }

        // 認證相關方法
        public async Task<BackendServiceResult<string>> RegisterAsync(string email, string password)
        {
            try
            {
                var authUrl = "https://uxgwdiuehabbzenwtcqo.auth.eu-central-1.nhost.run/v1/signup/email-password";
                
                var requestData = new
                {
                    email = email,
                    password = password
                };

                var json = JsonSerializer.Serialize(requestData);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                using var response = await _httpClient.PostAsync(authUrl, content);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var result = JsonSerializer.Deserialize<JsonElement>(responseContent);
                    var userId = result.GetProperty("session").GetProperty("user").GetProperty("id").GetString();
                    return BackendServiceResult<string>.CreateSuccess(userId ?? "");
                }
                else
                {
                    return BackendServiceResult<string>.CreateError($"註冊失敗: {responseContent}");
                }
            }
            catch (Exception ex)
            {
                return BackendServiceResult<string>.CreateError($"註冊錯誤: {ex.Message}");
            }
        }

        public async Task<BackendServiceResult<string>> LoginAsync(string email, string password)
        {
            try
            {
                var authUrl = "https://uxgwdiuehabbzenwtcqo.auth.eu-central-1.nhost.run/v1/signin/email-password";
                
                var requestData = new
                {
                    email = email,
                    password = password
                };

                var json = JsonSerializer.Serialize(requestData);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                using var response = await _httpClient.PostAsync(authUrl, content);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var result = JsonSerializer.Deserialize<JsonElement>(responseContent);
                    var accessToken = result.GetProperty("session").GetProperty("accessToken").GetString();
                    
                    // 設定授權標頭
                    _httpClient.DefaultRequestHeaders.Authorization = 
                        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
                    
                    return BackendServiceResult<string>.CreateSuccess(accessToken ?? "");
                }
                else
                {
                    return BackendServiceResult<string>.CreateError($"登入失敗: {responseContent}");
                }
            }
            catch (Exception ex)
            {
                return BackendServiceResult<string>.CreateError($"登入錯誤: {ex.Message}");
            }
        }

        // Food CRUD operations
        public async Task<BackendServiceResult<object[]>> GetFoodsAsync()
        {
            try
            {
                // 首先檢查資料表是否存在
                var schemaCheckResult = await CheckTableExistsAsync("foods");
                if (!schemaCheckResult)
                {
                    return BackendServiceResult<object[]>.CreateError("NHost 資料庫中未找到 'foods' 資料表。請先執行 CREATE_NHOST_TABLES.sql 腳本來創建必要的資料表。");
                }

                var query = @"
                    query GetFoods {
                        foods {
                            id
                            name
                            price
                            photo
                            shop
                            todate
                            photohash
                            created_at
                            updated_at
                        }
                    }";

                var result = await ExecuteGraphQLAsync(query);
                if (!result.Success)
                {
                    // 檢查是否是資料表不存在的錯誤
                    if (result.ErrorMessage?.Contains("field 'foods' not found") == true)
                    {
                        return BackendServiceResult<object[]>.CreateError("NHost 資料庫中未找到 'foods' 資料表。請先執行 CREATE_NHOST_TABLES.sql 腳本來創建必要的資料表。");
                    }
                    return BackendServiceResult<object[]>.CreateError($"GraphQL 查詢失敗: {result.ErrorMessage}");
                }

                // 檢查回應中是否有資料
                if (!result.Data.TryGetProperty("data", out var dataProperty))
                {
                    return BackendServiceResult<object[]>.CreateError("GraphQL 回應格式錯誤：缺少 data 屬性");
                }

                if (!dataProperty.TryGetProperty("foods", out var foodsProperty))
                {
                    return BackendServiceResult<object[]>.CreateError("GraphQL 回應格式錯誤：缺少 foods 屬性");
                }
                
                var foodList = new List<object>();
                foreach (var food in foodsProperty.EnumerateArray())
                {
                    foodList.Add(food);
                }
                
                return BackendServiceResult<object[]>.CreateSuccess(foodList.ToArray());
            }
            catch (Exception ex)
            {
                return BackendServiceResult<object[]>.CreateError($"獲取食品資料錯誤: {ex.Message}");
            }
        }

        public async Task<BackendServiceResult<object>> CreateFoodAsync(object foodData)
        {
            try
            {
                // 首先檢查資料表是否存在
                var schemaCheckResult = await CheckTableExistsAsync("foods");
                if (!schemaCheckResult)
                {
                    return BackendServiceResult<object>.CreateError("NHost 資料庫中未找到 'foods' 資料表。請先執行 CREATE_NHOST_TABLES.sql 腳本來創建必要的資料表。");
                }

                var mutation = @"
                    mutation CreateFood($object: foods_insert_input!) {
                        insert_foods_one(object: $object) {
                            id
                            name
                            price
                            photo
                            shop
                            todate
                            photohash
                            created_at
                        }
                    }";

                var result = await ExecuteGraphQLAsync(mutation, new { @object = foodData });
                if (!result.Success)
                {
                    if (result.ErrorMessage?.Contains("field 'foods' not found") == true)
                    {
                        return BackendServiceResult<object>.CreateError("NHost 資料庫中未找到 'foods' 資料表。請先執行 CREATE_NHOST_TABLES.sql 腳本來創建必要的資料表。");
                    }
                    return BackendServiceResult<object>.CreateError($"GraphQL 變更失敗: {result.ErrorMessage}");
                }

                if (!result.Data.TryGetProperty("data", out var dataProperty) ||
                    !dataProperty.TryGetProperty("insert_foods_one", out var createdFood))
                {
                    return BackendServiceResult<object>.CreateError("GraphQL 回應格式錯誤");
                }

                return BackendServiceResult<object>.CreateSuccess(createdFood);
            }
            catch (Exception ex)
            {
                return BackendServiceResult<object>.CreateError($"創建食品錯誤: {ex.Message}");
            }
        }

        public async Task<BackendServiceResult<object>> UpdateFoodAsync(string id, object foodData)
        {
            try
            {
                var mutation = @"
                    mutation UpdateFood($id: uuid!, $changes: foods_set_input!) {
                        update_foods_by_pk(pk_columns: {id: $id}, _set: $changes) {
                            id
                            name
                            price
                            photo
                            shop
                            todate
                            photohash
                            updated_at
                        }
                    }";

                var result = await ExecuteGraphQLAsync(mutation, new { id = id, changes = foodData });
                if (!result.Success)
                {
                    return BackendServiceResult<object>.CreateError(result.ErrorMessage ?? "GraphQL 請求失敗");
                }

                var updatedFood = result.Data.GetProperty("data").GetProperty("update_foods_by_pk");
                return BackendServiceResult<object>.CreateSuccess(updatedFood);
            }
            catch (Exception ex)
            {
                return BackendServiceResult<object>.CreateError($"更新食品錯誤: {ex.Message}");
            }
        }

        public async Task<BackendServiceResult<bool>> DeleteFoodAsync(string id)
        {
            try
            {
                var mutation = @"
                    mutation DeleteFood($id: uuid!) {
                        delete_foods_by_pk(id: $id) {
                            id
                        }
                    }";

                var result = await ExecuteGraphQLAsync(mutation, new { id = id });
                if (!result.Success)
                {
                    return BackendServiceResult<bool>.CreateError(result.ErrorMessage ?? "GraphQL 請求失敗");
                }

                var deletedFood = result.Data.GetProperty("data").GetProperty("delete_foods_by_pk");
                return BackendServiceResult<bool>.CreateSuccess(!deletedFood.ValueKind.Equals(JsonValueKind.Null));
            }
            catch (Exception ex)
            {
                return BackendServiceResult<bool>.CreateError($"刪除食品錯誤: {ex.Message}");
            }
        }

        // Subscription CRUD operations
        public async Task<BackendServiceResult<object[]>> GetSubscriptionsAsync()
        {
            try
            {
                // 首先檢查資料表是否存在
                var schemaCheckResult = await CheckTableExistsAsync("subscriptions");
                if (!schemaCheckResult)
                {
                    return BackendServiceResult<object[]>.CreateError("NHost 資料庫中未找到 'subscriptions' 資料表。請先執行 CREATE_NHOST_TABLES.sql 腳本來創建必要的資料表。");
                }

                var query = @"
                    query GetSubscriptions {
                        subscriptions {
                            id
                            name
                            nextdate
                            price
                            site
                            note
                            account
                            created_at
                            updated_at
                        }
                    }";

                var result = await ExecuteGraphQLAsync(query);
                if (!result.Success)
                {
                    // 檢查是否是資料表不存在的錯誤
                    if (result.ErrorMessage?.Contains("field 'subscriptions' not found") == true)
                    {
                        return BackendServiceResult<object[]>.CreateError("NHost 資料庫中未找到 'subscriptions' 資料表。請先執行 CREATE_NHOST_TABLES.sql 腳本來創建必要的資料表。");
                    }
                    return BackendServiceResult<object[]>.CreateError($"GraphQL 查詢失敗: {result.ErrorMessage}");
                }

                // 檢查回應中是否有資料
                if (!result.Data.TryGetProperty("data", out var dataProperty))
                {
                    return BackendServiceResult<object[]>.CreateError("GraphQL 回應格式錯誤：缺少 data 屬性");
                }

                if (!dataProperty.TryGetProperty("subscriptions", out var subscriptionsProperty))
                {
                    return BackendServiceResult<object[]>.CreateError("GraphQL 回應格式錯誤：缺少 subscriptions 屬性");
                }
                
                var subscriptionList = new List<object>();
                foreach (var subscription in subscriptionsProperty.EnumerateArray())
                {
                    subscriptionList.Add(subscription);
                }
                
                return BackendServiceResult<object[]>.CreateSuccess(subscriptionList.ToArray());
            }
            catch (Exception ex)
            {
                return BackendServiceResult<object[]>.CreateError($"獲取訂閱資料錯誤: {ex.Message}");
            }
        }

        public async Task<BackendServiceResult<object>> CreateSubscriptionAsync(object subscriptionData)
        {
            try
            {
                var mutation = @"
                    mutation CreateSubscription($object: subscriptions_insert_input!) {
                        insert_subscriptions_one(object: $object) {
                            id
                            name
                            nextdate
                            price
                            site
                            note
                            account
                            created_at
                        }
                    }";

                var result = await ExecuteGraphQLAsync(mutation, new { @object = subscriptionData });
                if (!result.Success)
                {
                    return BackendServiceResult<object>.CreateError(result.ErrorMessage ?? "GraphQL 請求失敗");
                }

                var createdSubscription = result.Data.GetProperty("data").GetProperty("insert_subscriptions_one");
                return BackendServiceResult<object>.CreateSuccess(createdSubscription);
            }
            catch (Exception ex)
            {
                return BackendServiceResult<object>.CreateError($"創建訂閱錯誤: {ex.Message}");
            }
        }

        public async Task<BackendServiceResult<object>> UpdateSubscriptionAsync(string id, object subscriptionData)
        {
            try
            {
                var mutation = @"
                    mutation UpdateSubscription($id: uuid!, $changes: subscriptions_set_input!) {
                        update_subscriptions_by_pk(pk_columns: {id: $id}, _set: $changes) {
                            id
                            name
                            nextdate
                            price
                            site
                            note
                            account
                            updated_at
                        }
                    }";

                var result = await ExecuteGraphQLAsync(mutation, new { id = id, changes = subscriptionData });
                if (!result.Success)
                {
                    return BackendServiceResult<object>.CreateError(result.ErrorMessage ?? "GraphQL 請求失敗");
                }

                var updatedSubscription = result.Data.GetProperty("data").GetProperty("update_subscriptions_by_pk");
                return BackendServiceResult<object>.CreateSuccess(updatedSubscription);
            }
            catch (Exception ex)
            {
                return BackendServiceResult<object>.CreateError($"更新訂閱錯誤: {ex.Message}");
            }
        }

        public async Task<BackendServiceResult<bool>> DeleteSubscriptionAsync(string id)
        {
            try
            {
                var mutation = @"
                    mutation DeleteSubscription($id: uuid!) {
                        delete_subscriptions_by_pk(id: $id) {
                            id
                        }
                    }";

                var result = await ExecuteGraphQLAsync(mutation, new { id = id });
                if (!result.Success)
                {
                    return BackendServiceResult<bool>.CreateError(result.ErrorMessage ?? "GraphQL 請求失敗");
                }

                var deletedSubscription = result.Data.GetProperty("data").GetProperty("delete_subscriptions_by_pk");
                return BackendServiceResult<bool>.CreateSuccess(!deletedSubscription.ValueKind.Equals(JsonValueKind.Null));
            }
            catch (Exception ex)
            {
                return BackendServiceResult<bool>.CreateError($"刪除訂閱錯誤: {ex.Message}");
            }
        }

        // Helper method for GraphQL requests
        private async Task<BackendServiceResult<JsonElement>> ExecuteGraphQLAsync(string query, object? variables = null)
        {
            try
            {
                object requestData;
                if (variables != null)
                {
                    requestData = new { query = query, variables = variables };
                }
                else
                {
                    requestData = new { query = query };
                }
                
                var json = JsonSerializer.Serialize(requestData);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                using var request = new HttpRequestMessage(HttpMethod.Post, _graphqlUrl);
                request.Content = content;
                request.Headers.Add("x-hasura-admin-secret", _adminSecret);

                using var response = await _httpClient.SendAsync(request);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var result = JsonSerializer.Deserialize<JsonElement>(responseContent);
                    
                    // 檢查是否有 GraphQL 錯誤
                    if (result.TryGetProperty("errors", out var errors))
                    {
                        return BackendServiceResult<JsonElement>.CreateError($"GraphQL 錯誤: {errors}");
                    }
                    
                    return BackendServiceResult<JsonElement>.CreateSuccess(result);
                }
                else
                {
                    return BackendServiceResult<JsonElement>.CreateError($"GraphQL 請求失敗: {response.StatusCode} - {responseContent}");
                }
            }
            catch (Exception ex)
            {
                return BackendServiceResult<JsonElement>.CreateError($"GraphQL 請求錯誤: {ex.Message}");
            }
        }

        // Helper method to check if a table exists in the database
        private async Task<bool> CheckTableExistsAsync(string tableName)
        {
            try
            {
                // 使用簡單的 introspection 查詢來檢查資料表是否存在
                var introspectionQuery = @"
                    query IntrospectionQuery {
                        __schema {
                            queryType {
                                fields {
                                    name
                                }
                            }
                        }
                    }";

                var result = await ExecuteGraphQLAsync(introspectionQuery);
                if (!result.Success)
                {
                    // 如果 introspection 失敗，假設資料表不存在
                    return false;
                }

                // 檢查回應中是否包含指定的資料表名稱
                if (result.Data.TryGetProperty("data", out var dataProperty) &&
                    dataProperty.TryGetProperty("__schema", out var schemaProperty) &&
                    schemaProperty.TryGetProperty("queryType", out var queryTypeProperty) &&
                    queryTypeProperty.TryGetProperty("fields", out var fieldsProperty))
                {
                    foreach (var field in fieldsProperty.EnumerateArray())
                    {
                        if (field.TryGetProperty("name", out var nameProperty) &&
                            nameProperty.GetString() == tableName)
                        {
                            return true;
                        }
                    }
                }

                return false;
            }
            catch (Exception)
            {
                // 如果檢查過程中發生錯誤，假設資料表不存在
                return false;
            }
        }

        public void Dispose()
        {
            _httpClient?.Dispose();
        }
    }
}