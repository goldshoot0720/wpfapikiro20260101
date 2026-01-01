# 系統設定功能說明

## 功能概述

系統設定頁面允許用戶選擇和配置不同的後端服務提供商，包括：

- **Appwrite** - 開源的後端即服務平台
- **Supabase** - 開源的 Firebase 替代方案
- **NHost** - 現代化的後端即服務平台
- **Contentful** - 強大的內容管理系統
- **MySQL** - 世界上最受歡迎的開源關聯式資料庫

## 使用方式

### 1. 進入設定頁面
- 在左側導航欄點擊「系統設定」按鈕
- 或者直接導航到設定頁面

### 2. 選擇後端服務
- 點擊其中一個後端服務選項
- 系統會自動填入該服務的預設 URL 和設定
- 標籤會根據服務類型自動調整

### 3. 填寫連線資訊
- **API URL**: 後端服務的 API 端點
- **Project ID/Space ID/Database**: 根據服務類型顯示不同標籤
- **API Key**: API 金鑰或密碼

### 4. 測試連線
- 點擊「測試連線」按鈕驗證設定是否正確
- 系統會顯示連線結果和服務名稱

### 5. 儲存設定
- 點擊「儲存設定」按鈕保存配置
- 設定會儲存到本地檔案系統

## 預設值

### Appwrite
- API URL: `https://cloud.appwrite.io/v1`
- Project ID: `your-project-id`

### Supabase
- API URL: `https://your-project.supabase.co`
- Project ID: 不需要

### NHost
- API URL: `https://your-project.nhost.run`
- Project ID: `your-project-id`

### Contentful
- API URL: `https://api.contentful.com`
- Space ID: `your-space-id`

### MySQL
- API URL: `localhost:3306`
- Database: `your-database-name`

## 設定檔案位置

設定會儲存在：
```
%APPDATA%\wpfkiro20260101\settings.json
```

## 架構說明

### 核心類別
- `AppSettings`: 單例模式的設定管理類
- `BackendServiceType`: 後端服務類型枚舉（支援 5 種服務）
- `IBackendService`: 後端服務介面
- `BackendServiceFactory`: 服務工廠類

### 服務實作
- `AppwriteService`: Appwrite 服務實作
- `SupabaseService`: Supabase 服務實作
- `NHostService`: NHost 服務實作
- `ContentfulService`: Contentful 服務實作
- `MySQLService`: MySQL 服務實作

## 服務特色

### Contentful 特色
- 內容管理系統 API
- 支援多語言內容
- 豐富的媒體管理功能
- 提供 `GetContentTypesAsync()` 和 `GetEntriesAsync()` 方法

### MySQL 特色
- 傳統關聯式資料庫
- 支援 SQL 查詢
- 連線池管理
- 提供 `GetTablesAsync()` 和 `ExecuteQueryAsync()` 方法

## 擴展性

要添加新的後端服務：

1. 在 `BackendServiceType` 枚舉中添加新類型
2. 創建實作 `IBackendService` 的新服務類
3. 在 `BackendServiceFactory` 中添加對應的創建邏輯
4. 在設定頁面 XAML 中添加新的選項
5. 更新 `AppSettings.Defaults` 類別
6. 在 `UpdateFieldsForService` 方法中添加標籤邏輯

## 注意事項

- 設定會在應用程式重啟後保持
- 連線測試是模擬的，實際應用中需要實作真正的 SDK 呼叫
- API 金鑰會以明文儲存，生產環境中應考慮加密
- 目前支援的是 .NET 10.0，確保相容性
- 不同服務的欄位標籤會自動調整（Project ID/Space ID/Database）