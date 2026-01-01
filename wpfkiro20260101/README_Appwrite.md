# Appwrite 整合指南

## 概述

本應用程式以 Appwrite 作為主要後端服務，提供完整的後端即服務 (BaaS) 功能，包括：

- 用戶認證和管理
- 資料庫操作
- 檔案儲存
- 即時功能
- 雲端函數

## 快速開始

### 1. 創建 Appwrite 專案

1. 前往 [Appwrite Cloud](https://cloud.appwrite.io) 或自架 Appwrite 伺服器
2. 創建新專案
3. 記錄以下資訊：
   - **API Endpoint**: `https://cloud.appwrite.io/v1` (或你的自架地址)
   - **Project ID**: 在專案設定中找到
   - **API Key**: 在 API Keys 頁面創建 (可選，用於伺服器端操作)

### 2. 配置應用程式

1. 啟動應用程式
2. 點擊左側導航欄的「系統設定」
3. 選擇 **Appwrite** 作為後端服務
4. 填寫連線資訊：
   - **API URL**: 你的 Appwrite API 端點
   - **Project ID**: 你的專案 ID
   - **API Key**: 你的 API 金鑰 (可選)
5. 點擊「測試連線」驗證設定
6. 點擊「儲存設定」保存配置

### 3. 測試功能

1. 點擊左側導航欄的「Appwrite 測試」
2. 使用各種測試按鈕驗證功能：
   - 測試連線
   - 初始化服務
   - 用戶管理
   - 資料庫操作
   - 儲存管理

## 功能說明

### 🔐 用戶認證
- 創建新用戶帳號
- 用戶登入/登出
- 獲取當前用戶資訊
- 用戶資料管理

### 🗄️ 資料庫操作
- 獲取資料庫列表
- 管理集合 (Collections)
- CRUD 文檔操作
- 查詢和篩選

### 📁 檔案儲存
- 獲取儲存桶列表
- 檔案上傳/下載
- 檔案管理
- 權限控制

### ⚡ 即時功能
- 即時資料同步
- 事件監聽
- 推播通知

## 架構說明

### 核心類別

#### `AppwriteService`
- 實作 `IBackendService` 介面
- 提供 Appwrite SDK 的封裝
- 處理連線、初始化和基本操作

#### `AppwriteManager`
- 單例模式管理 Appwrite 服務
- 提供便捷的 API 方法
- 統一錯誤處理和狀態管理

#### `AppwriteTestPage`
- 提供完整的測試介面
- 展示各種 Appwrite 功能
- 即時顯示操作結果

### 設定管理

設定儲存在 `AppSettings` 類別中，包括：
- `BackendService`: 設定為 `BackendServiceType.Appwrite`
- `ApiUrl`: Appwrite API 端點
- `ProjectId`: 專案識別碼
- `ApiKey`: API 金鑰 (可選)

## 開發指南

### 添加新功能

1. 在 `AppwriteService` 中添加新方法
2. 在 `AppwriteManager` 中添加便捷方法
3. 在 `AppwriteTestPage` 中添加測試介面
4. 更新相關文檔

### 錯誤處理

所有 Appwrite 操作都返回 `BackendServiceResult<T>` 類型：
```csharp
var result = await appwriteManager.GetDatabasesAsync();
if (result.Success)
{
    // 處理成功結果
    var databases = result.Data;
}
else
{
    // 處理錯誤
    var error = result.ErrorMessage;
}
```

### 最佳實踐

1. **連線管理**: 使用 `AppwriteManager` 統一管理連線
2. **錯誤處理**: 總是檢查操作結果的 `Success` 屬性
3. **配置檢查**: 在執行操作前檢查 Appwrite 是否已正確配置
4. **資源清理**: 適當地處理客戶端資源

## 常見問題

### Q: 連線測試失敗怎麼辦？
A: 檢查以下項目：
- API URL 是否正確
- Project ID 是否正確
- 網路連線是否正常
- Appwrite 伺服器是否運行正常

### Q: 需要 API Key 嗎？
A: API Key 是可選的，主要用於：
- 伺服器端操作
- 管理員權限操作
- 跨域請求

### Q: 如何處理用戶認證？
A: 目前實作提供基本的用戶管理功能，完整的認證流程需要根據具體需求實作。

## 相關資源

- [Appwrite 官方文檔](https://appwrite.io/docs)
- [Appwrite .NET SDK](https://github.com/appwrite/sdk-for-dotnet)
- [Appwrite Cloud](https://cloud.appwrite.io)
- [Appwrite 社群](https://appwrite.io/discord)

## 版本資訊

- Appwrite SDK: 0.7.0
- .NET 版本: 10.0
- 支援平台: Windows (WPF)

## 授權

本專案遵循 MIT 授權條款。