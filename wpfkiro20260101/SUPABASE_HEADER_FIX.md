# Supabase HTTP 標頭修正

## 問題描述
在使用 Supabase 服務時出現 HTTP 標頭錯誤：
```
Supabase 載入失敗: Misused header name, 'Content-Type'. Make sure request headers are used with HttpRequestMessage, response headers with HttpResponseMessage, and content headers with HttpContent objects.
```

## 問題原因
在 GET 請求中錯誤地設定了 `Content-Type` 標頭。HTTP GET 請求不應該包含 Content-Type 標頭，因為 GET 請求沒有請求主體 (request body)。

## 修正內容

### 修正前的錯誤代碼
```csharp
// ❌ 錯誤：在 GET 請求中設定 Content-Type
_httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
_httpClient.DefaultRequestHeaders.Add("Content-Type", "application/json");
```

### 修正後的正確代碼
```csharp
// ✅ 正確：GET 請求只設定必要的標頭
_httpClient.DefaultRequestHeaders.Clear();
_httpClient.DefaultRequestHeaders.Add("apikey", _settings.ApiKey);
_httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_settings.ApiKey}");
```

## 修正的方法

### 1. GetFoodsAsync()
- ❌ 移除：`Accept` 標頭 (非必要)
- ❌ 移除：`Content-Type` 標頭 (GET 請求不需要)
- ✅ 保留：`apikey` 標頭 (Supabase 必要)
- ✅ 保留：`Authorization` 標頭 (Supabase 必要)

### 2. GetSubscriptionsAsync()
- ❌ 移除：`Accept` 標頭 (非必要)
- ❌ 移除：`Content-Type` 標頭 (GET 請求不需要)
- ✅ 保留：`apikey` 標頭 (Supabase 必要)
- ✅ 保留：`Authorization` 標頭 (Supabase 必要)

### 3. POST/PATCH/DELETE 方法
- ✅ 保持：正確的標頭設定
- ✅ 保持：`Content-Type` 在 StringContent 中設定

## HTTP 標頭最佳實踐

### GET 請求
```csharp
// 正確的 GET 請求標頭設定
_httpClient.DefaultRequestHeaders.Clear();
_httpClient.DefaultRequestHeaders.Add("apikey", apiKey);
_httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
// 不要設定 Content-Type (GET 請求沒有 body)
```

### POST/PATCH 請求
```csharp
// 正確的 POST/PATCH 請求標頭設定
_httpClient.DefaultRequestHeaders.Clear();
_httpClient.DefaultRequestHeaders.Add("apikey", apiKey);
_httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

// Content-Type 在 StringContent 中設定
var content = new StringContent(json, Encoding.UTF8, "application/json");
```

## 測試驗證

### 新增測試工具
- `TestSupabaseHeaderFix.cs` - 專門測試修正後的標頭設定
- 整合到設定頁面的「測試連線」功能中

### 測試項目
1. ✅ 基本連線測試
2. ✅ 食品資料載入測試
3. ✅ 訂閱資料載入測試
4. ✅ 錯誤處理驗證

### 預期結果
- ✅ 不再出現 "Misused header name" 錯誤
- ✅ 成功載入 Supabase 資料
- ✅ 正常顯示食品和訂閱列表
- ✅ 日期排序功能正常運作

## 使用方式

### 1. 透過設定頁面測試
1. 前往「系統設定」頁面
2. 選擇 Supabase 作為後端服務
3. 點擊「測試連線」按鈕
4. 系統會自動執行修正後的測試

### 2. 查看測試結果
- 控制台會顯示詳細的測試過程
- 成功時會顯示載入的資料數量
- 失敗時會顯示具體的錯誤訊息和建議

### 3. 驗證實際功能
- 前往「食品管理」頁面查看資料載入
- 前往「訂閱管理」頁面查看資料載入
- 確認日期排序功能正常

## 相關文件
- `SUPABASE_COMPREHENSIVE_TEST.md` - 綜合測試說明
- `DATE_SORTING_FEATURE.md` - 日期排序功能
- `COLLAPSIBLE_SETTINGS_FEATURE.md` - 摺疊設定功能

## 故障排除

### 如果仍然出現連線問題
1. **檢查 Supabase 設定**
   - API URL 是否正確
   - API Key 是否有效
   - Project ID 是否正確

2. **檢查 Supabase 專案**
   - 專案是否正常運行
   - 資料表是否存在 (`food`, `subscription`)
   - Row Level Security (RLS) 政策是否正確

3. **檢查網路連線**
   - 防火牆設定
   - 代理伺服器設定
   - DNS 解析問題

### 常見錯誤和解決方案
- **401 Unauthorized**: API Key 無效或過期
- **404 Not Found**: 資料表不存在或名稱錯誤
- **403 Forbidden**: RLS 政策阻止訪問
- **網路錯誤**: 檢查網路連線和防火牆設定

## 總結

這次修正解決了 Supabase 服務中的 HTTP 標頭設定問題，確保：

✅ **正確的標頭使用**: GET 請求不包含 Content-Type
✅ **相容性改善**: 符合 HTTP 標準和 .NET HttpClient 要求
✅ **功能完整性**: 所有 CRUD 操作正常運作
✅ **錯誤處理**: 提供清晰的錯誤訊息和診斷
✅ **測試覆蓋**: 完整的測試工具和驗證機制

現在 Supabase 的食品和訂閱功能應該可以正常運作了！