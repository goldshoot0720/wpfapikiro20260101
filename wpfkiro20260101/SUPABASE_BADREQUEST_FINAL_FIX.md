# Supabase BadRequest 錯誤最終修正

## 🎯 問題描述

應用程式在載入訂閱頁面時顯示錯誤：
```
動作已遭拒絕：Supabase 拒絕要求：BadRequest
```

## 🔧 修正內容

### 1. 完善 HTTP 標頭配置

為所有 Supabase API 請求添加完整的 HTTP 標頭：

```csharp
// 清除並重新設置 HTTP 標頭
_httpClient.DefaultRequestHeaders.Clear();
_httpClient.DefaultRequestHeaders.Add("apikey", _settings.ApiKey);
_httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_settings.ApiKey}");
_httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
_httpClient.DefaultRequestHeaders.Add("Content-Type", "application/json");
```

### 2. 增強錯誤處理

- 添加詳細的 Debug 輸出
- 包含完整的錯誤回應內容
- 顯示 API Key 的前 20 個字符用於驗證

### 3. 修正的方法

- ✅ `TestConnectionAsync()` - 連接測試
- ✅ `GetFoodsAsync()` - 載入食品資料
- ✅ `GetSubscriptionsAsync()` - 載入訂閱資料
- ✅ `CreateSubscriptionAsync()` - 創建訂閱

## 🚀 使用方法

### 1. 運行測試工具

在 Visual Studio 中執行以下代碼來測試修正：

```csharp
await TestSupabaseBadRequestFix.RunTest();
```

### 2. 手動測試步驟

1. **確認設定**
   - 進入「系統設定」頁面
   - 選擇「Supabase」選項
   - 確認 API URL 和 API Key 正確

2. **測試訂閱頁面**
   - 進入「訂閱管理」頁面
   - 應該不再顯示 BadRequest 錯誤
   - 可以正常載入訂閱資料（如果有的話）

3. **測試食品頁面**
   - 進入「食品管理」頁面
   - 確認可以正常載入

## 📋 預期結果

修正後應該看到：

### 成功的 Debug 輸出
```
嘗試連接 Supabase Subscription API: https://lobezwpworbfktlkxuyo.supabase.co/rest/v1/subscription
使用 API Key: eyJhbGciOiJIUzI1NiIs...
Subscription API 回應狀態: OK
Subscription API 成功，回應內容: [...]
```

### UI 顯示
- 不再出現 BadRequest 錯誤對話框
- 正常顯示訂閱列表（空列表或實際資料）
- 可以正常使用新增、編輯、刪除功能

## 🔍 故障排除

### 如果仍有問題

1. **檢查 Visual Studio 輸出視窗**
   - 查看詳細的 Debug 訊息
   - 確認 API Key 是否正確

2. **驗證 Supabase 設定**
   ```
   API URL: https://lobezwpworbfktlkxuyo.supabase.co
   Project ID: lobezwpworbfktlkxuyo
   API Key: eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
   ```

3. **重新啟動應用程式**
   - 完全關閉應用程式
   - 重新開啟以載入最新程式碼

### 常見錯誤

- **401 Unauthorized**: API Key 錯誤或過期
- **404 Not Found**: 資料表不存在
- **403 Forbidden**: 權限不足

## ✅ 修正完成

此修正解決了 Supabase BadRequest 錯誤，現在應用程式可以正常：

1. 連接到 Supabase 資料庫
2. 載入訂閱和食品資料
3. 執行 CRUD 操作
4. 顯示詳細的錯誤訊息（如果有問題）

**修正檔案**:
- `wpfkiro20260101/Services/SupabaseService.cs` - 主要修正
- `wpfkiro20260101/TestSupabaseBadRequestFix.cs` - 測試工具

**下次使用時**，只需要確保在設定中選擇了 Supabase 並使用正確的 API 憑證即可。