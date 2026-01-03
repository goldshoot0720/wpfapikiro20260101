# Supabase Content-Type 標頭錯誤修正

## 🎯 問題描述

應用程式在載入 Supabase 資料時出現錯誤：
```
Supabase 載入失敗：載入 Supabase 訂閱資料失敗：Misused header name, 'Content-Type'. 
Make sure request headers are used with HttpRequestMessage, response headers with 
HttpResponseMessage, and content headers with HttpContent objects.
```

## 🔍 問題根因

這個錯誤是因為在 GET 請求中錯誤地將 `Content-Type` 標頭添加到了 `HttpClient.DefaultRequestHeaders` 中。

在 .NET HttpClient 中：
- **請求標頭** (如 `Authorization`, `Accept`) 應該添加到 `DefaultRequestHeaders`
- **內容標頭** (如 `Content-Type`) 應該添加到 `HttpContent` 對象中
- **GET 請求** 通常不需要 `Content-Type` 標頭

## 🔧 修正內容

### 1. GET 請求修正

**修正前**：
```csharp
_httpClient.DefaultRequestHeaders.Clear();
_httpClient.DefaultRequestHeaders.Add("apikey", _settings.ApiKey);
_httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_settings.ApiKey}");
_httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
_httpClient.DefaultRequestHeaders.Add("Content-Type", "application/json"); // ❌ 錯誤
```

**修正後**：
```csharp
_httpClient.DefaultRequestHeaders.Clear();
_httpClient.DefaultRequestHeaders.Add("apikey", _settings.ApiKey);
_httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_settings.ApiKey}");
_httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
// ✅ 移除了 Content-Type，因為這是 GET 請求
```

### 2. POST 請求修正

**修正後**：
```csharp
_httpClient.DefaultRequestHeaders.Clear();
_httpClient.DefaultRequestHeaders.Add("apikey", _settings.ApiKey);
_httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_settings.ApiKey}");
_httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
// Content-Type 會在 StringContent 中自動設置

var content = new StringContent(json, Encoding.UTF8, "application/json");
// ✅ Content-Type 在這裡正確設置
```

### 3. 修正的方法

- ✅ `TestConnectionAsync()` - 連接測試
- ✅ `GetFoodsAsync()` - 載入食品資料  
- ✅ `GetSubscriptionsAsync()` - 載入訂閱資料
- ✅ `CreateSubscriptionAsync()` - 創建訂閱

## 🚀 立即解決步驟

### 1. 重新啟動應用程式
```
1. 完全關閉當前應用程式
2. 重新開啟應用程式
3. 確保載入最新的修正程式碼
```

### 2. 測試修正結果
```
1. 進入「訂閱管理」頁面
2. 應該不再看到 Content-Type 錯誤
3. 可以正常載入訂閱資料
4. 測試新增、編輯功能
```

### 3. 運行測試工具（可選）
```csharp
await TestSupabaseBadRequestFix.RunTest();
```

## 📋 預期結果

修正後你應該看到：

### ✅ 正常情況
- 訂閱頁面正常載入，不再出現 Content-Type 錯誤
- 顯示「從 Supabase 載入了 X 項訂閱資料」
- 可以正常使用所有 CRUD 功能

### 📊 Debug 輸出（在 Visual Studio 輸出視窗）
```
嘗試連接 Supabase Subscription API: https://lobezwpworbfktlkxuyo.supabase.co/rest/v1/subscription
使用 API Key: eyJhbGciOiJIUzI1NiIs...
Subscription API 回應狀態: OK
Subscription API 成功，回應內容: [...]
```

## 🔍 技術細節

### HTTP 標頭分類

| 標頭類型 | 添加位置 | 範例 |
|---------|---------|------|
| 請求標頭 | `DefaultRequestHeaders` | `Authorization`, `Accept`, `apikey` |
| 內容標頭 | `HttpContent` | `Content-Type`, `Content-Length` |
| 回應標頭 | `HttpResponseMessage` | `Server`, `Date` |

### Supabase API 要求的標頭

**必要標頭**：
- `apikey`: Supabase API 金鑰
- `Authorization`: Bearer token (通常與 apikey 相同)

**可選標頭**：
- `Accept`: application/json (指定回應格式)

**不需要的標頭**：
- `Content-Type` (在 GET 請求中)

## 🎉 修正完成

此修正解決了 Supabase Content-Type 標頭錯誤，現在應用程式可以正常：

1. ✅ 連接到 Supabase 資料庫
2. ✅ 載入訂閱和食品資料
3. ✅ 執行所有 CRUD 操作
4. ✅ 正確處理 HTTP 標頭

**修正檔案**:
- `wpfkiro20260101/Services/SupabaseService.cs` - 主要修正
- `wpfkiro20260101/TestSupabaseBadRequestFix.cs` - 更新的測試工具

**關鍵改善**:
- 移除了 GET 請求中不當的 Content-Type 標頭
- 保持了 POST 請求中正確的 Content-Type 設置
- 改善了錯誤處理和 Debug 輸出

現在你的應用程式應該可以完全正常地使用 Supabase 功能了！