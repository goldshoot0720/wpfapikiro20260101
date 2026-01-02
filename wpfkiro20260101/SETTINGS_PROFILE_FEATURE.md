# 設定檔管理功能

## 功能概述

設定檔管理功能允許用戶儲存、載入和管理多個應用程式設定檔，支援最多 100 筆設定檔的儲存。每個設定檔包含完整的後端服務配置資訊，包括 API 端點、認證資訊和服務特定設定。

## 主要功能

### 1. 設定檔儲存
- 從當前應用程式設定創建新的設定檔
- 支援自訂設定檔名稱和描述
- 自動儲存所有後端服務配置
- 最多支援 100 筆設定檔

### 2. 設定檔載入
- 從已儲存的設定檔載入配置
- 一鍵切換不同的後端服務設定
- 自動應用所有相關配置

### 3. 設定檔管理
- 編輯設定檔名稱和描述
- 刪除不需要的設定檔
- 搜尋和篩選設定檔
- 查看設定檔詳細資訊

### 4. 匯入/匯出功能
- 匯出設定檔為 JSON 格式
- 匯入外部設定檔
- 支援批量匯出和選擇性匯出
- 自動處理名稱衝突

## 支援的後端服務

設定檔功能支援所有已實現的後端服務：

- **Appwrite**: 包含 Database ID, Bucket ID, Collection IDs
- **Supabase**: 包含 Project ID 和 API Key
- **NHost**: 基本 API 配置
- **Contentful**: Space ID 和 API Key
- **Back4App**: App ID 和 API Key
- **MySQL**: 資料庫連線資訊
- **Strapi**: API Token 配置
- **Sanity**: Project ID 和 Token

## 資料結構

### SettingsProfile 模型
```csharp
public class SettingsProfile
{
    public string Id { get; set; }
    public string ProfileName { get; set; }        // 設定檔名稱 (必填, 最大100字元)
    public BackendServiceType BackendService { get; set; }
    public string ApiUrl { get; set; }
    public string ProjectId { get; set; }
    public string ApiKey { get; set; }
    public string DatabaseId { get; set; }         // Appwrite 專用
    public string BucketId { get; set; }           // Appwrite 專用
    public string FoodCollectionId { get; set; }   // Appwrite 專用
    public string SubscriptionCollectionId { get; set; } // Appwrite 專用
    public string Description { get; set; }        // 設定檔描述 (選填, 最大500字元)
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
```

### Appwrite 資料庫欄位對應
- `profile_name` → ProfileName
- `backend_service` → BackendService (字串格式)
- `api_url` → ApiUrl
- `project_id` → ProjectId
- `api_key` → ApiKey
- `database_id` → DatabaseId
- `bucket_id` → BucketId
- `food_collection_id` → FoodCollectionId
- `subscription_collection_id` → SubscriptionCollectionId
- `description` → Description

## 使用方式

### 1. 開啟設定檔管理
1. 進入「系統設定」頁面
2. 在「設定檔管理」區域點擊「📁 管理設定檔」按鈕
3. 設定檔管理視窗將會開啟

### 2. 儲存當前設定
1. 在設定檔管理視窗中點擊「💾 儲存當前設定」
2. 輸入設定檔名稱（必填）
3. 輸入設定檔描述（選填）
4. 點擊「儲存」完成

### 3. 載入設定檔
1. 在設定檔列表中選擇要載入的設定檔
2. 點擊「📂 載入設定檔」
3. 確認載入操作
4. 重新啟動應用程式以確保設定生效

### 4. 編輯設定檔
1. 選擇要編輯的設定檔
2. 點擊「✏️ 編輯設定檔」
3. 修改名稱或描述
4. 點擊「更新」儲存變更

### 5. 匯出設定檔
- **匯出全部**: 點擊「📤 匯出設定檔」匯出所有設定檔
- **匯出選中**: 選擇設定檔後點擊「📤 匯出選中」

### 6. 匯入設定檔
1. 點擊「📥 匯入設定檔」
2. 選擇 JSON 格式的設定檔案
3. 系統會自動處理名稱衝突並匯入

## 儲存位置

### 本地儲存
設定檔資料儲存在：
```
%APPDATA%\wpfkiro20260101\profiles.json
```

### Appwrite 雲端儲存
如果使用 Appwrite 作為後端服務，設定檔也會同步儲存到 Appwrite 資料庫的 `settings_profiles` 集合中。

## 限制和注意事項

### 數量限制
- 最多支援 100 筆設定檔
- 設定檔名稱最大 100 字元
- 描述最大 500 字元

### 安全考量
- API Key 等敏感資訊會被儲存，請確保檔案安全
- 匯出的 JSON 檔案包含完整的認證資訊
- 建議定期備份設定檔資料

### 相容性
- 設定檔格式向後相容
- 支援不同版本間的設定檔匯入
- 自動處理缺失欄位的預設值

## 錯誤處理

### 常見錯誤
1. **設定檔名稱重複**: 系統會提示並要求使用不同名稱
2. **儲存空間不足**: 達到 100 筆限制時會顯示警告
3. **匯入格式錯誤**: 無效的 JSON 格式會被拒絕
4. **網路連線問題**: Appwrite 同步失敗時會顯示錯誤訊息

### 故障恢復
- 本地檔案損壞時可從 Appwrite 恢復
- 支援手動編輯 JSON 檔案進行修復
- 提供重置功能清除所有設定檔

## 開發者資訊

### 相關檔案
- `Models/SettingsProfile.cs` - 設定檔資料模型
- `Services/SettingsProfileService.cs` - 設定檔管理服務
- `SettingsProfileWindow.xaml/.cs` - 設定檔管理視窗
- `CreateProfileDialog.xaml/.cs` - 新增/編輯對話框
- `CREATE_SETTINGS_PROFILES_TABLE.sql` - Appwrite 表結構

### API 方法
- `GetAllProfilesAsync()` - 取得所有設定檔
- `SaveProfileAsync()` - 儲存設定檔
- `LoadProfileAsync()` - 載入設定檔
- `DeleteProfileAsync()` - 刪除設定檔
- `ImportProfilesAsync()` - 匯入設定檔
- `ExportProfilesAsync()` - 匯出設定檔

### 擴展性
- 支援新增更多後端服務類型
- 可擴展設定檔欄位
- 支援自訂驗證規則
- 可整合更多雲端儲存服務