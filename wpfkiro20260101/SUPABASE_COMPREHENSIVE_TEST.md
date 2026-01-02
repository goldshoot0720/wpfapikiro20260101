# Supabase 綜合測試功能

## 功能概述
為 Supabase 後端服務創建了完整的測試和診斷工具，確保食品和訂閱功能正常運作。

## 實現的測試工具

### 1. TestSupabaseComprehensive.cs
**綜合功能測試**
- ✅ 連線測試
- ✅ 食品 CRUD 操作測試
- ✅ 訂閱 CRUD 操作測試
- ✅ CrudManager 整合測試
- ✅ 資料表結構檢查

### 2. QuickSupabaseDiagnosis.cs
**快速診斷工具**
- ✅ 基本連線診斷
- ✅ 資料表存在性檢查
- ✅ API 權限驗證
- ✅ 錯誤分析和建議
- ✅ 診斷結果摘要

### 3. TestSupabaseQuick.cs
**快速測試工具**
- ✅ 服務創建測試
- ✅ 基本功能驗證
- ✅ 資料載入測試
- ✅ 使用指南顯示

## Supabase 設定資訊

### 預設設定
```csharp
public class SupabaseSettings : IServiceSettings
{
    public string ApiUrl { get; set; } = "https://lobezwpworbfktlkxuyo.supabase.co";
    public string ProjectId { get; set; } = "lobezwpworbfktlkxuyo";
    public string ApiKey { get; set; } = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...";
}
```

### 資料表結構

#### Food 資料表 (food)
| 欄位名稱 | 類型 | 說明 |
|---------|------|------|
| id | 主鍵 | 唯一識別碼 |
| name | 文字 | 食品名稱 |
| price | 數字 | 價格 |
| photo | 文字 | 圖片 URL |
| shop | 文字 | 商店名稱 |
| todate | 文字 | 到期日期 |
| account | 文字 | 帳戶資訊 |
| created_at | 時間戳 | 創建時間 |
| updated_at | 時間戳 | 更新時間 |

#### Subscription 資料表 (subscription)
| 欄位名稱 | 類型 | 說明 |
|---------|------|------|
| id | 主鍵 | 唯一識別碼 |
| name | 文字 | 訂閱名稱 |
| nextdate | 文字 | 下次付款日期 |
| price | 數字 | 價格 |
| site | 文字 | 網站 URL |
| account | 文字 | 帳戶資訊 |
| note | 文字 | 備註 |
| created_at | 時間戳 | 創建時間 |
| updated_at | 時間戳 | 更新時間 |

## 測試使用方式

### 1. 自動診斷測試
```csharp
// 在設定頁面點擊「測試連線」按鈕
// 系統會自動執行 Supabase 診斷
```

### 2. 快速測試
```csharp
// 在設定頁面點擊「⚡ 快速測試」按鈕
// 如果當前是 Supabase 服務，會執行專用測試
```

### 3. 手動測試
```csharp
// 在控制台或程式中直接調用
await TestSupabaseComprehensive.RunComprehensiveTest();
await QuickSupabaseDiagnosis.RunQuickDiagnosis();
await TestSupabaseQuick.RunQuickTest();
```

## 測試項目檢查清單

### 連線測試
- [ ] 基本 API 連線
- [ ] API Key 驗證
- [ ] 權限檢查
- [ ] 網路連線狀態

### 資料表測試
- [ ] food 資料表存在性
- [ ] subscription 資料表存在性
- [ ] 資料表結構正確性
- [ ] 資料讀取權限

### CRUD 操作測試
- [ ] 食品資料讀取 (GET)
- [ ] 食品資料創建 (POST)
- [ ] 食品資料更新 (PATCH)
- [ ] 食品資料刪除 (DELETE)
- [ ] 訂閱資料讀取 (GET)
- [ ] 訂閱資料創建 (POST)
- [ ] 訂閱資料更新 (PATCH)
- [ ] 訂閱資料刪除 (DELETE)

### 整合測試
- [ ] BackendServiceFactory 正確創建 SupabaseService
- [ ] CrudManager 正確路由到 SupabaseService
- [ ] 食品頁面資料載入
- [ ] 訂閱頁面資料載入
- [ ] 日期排序功能正常

## 常見問題和解決方案

### 連線問題
**問題**: 401 Unauthorized
**解決**: 檢查 API Key 是否正確且有效

**問題**: 404 Not Found
**解決**: 確認資料表名稱正確，檢查 Supabase 控制台

**問題**: 403 Forbidden
**解決**: 檢查 Row Level Security (RLS) 政策設定

### 資料問題
**問題**: 資料格式錯誤
**解決**: 檢查欄位名稱和資料類型是否匹配

**問題**: 日期格式問題
**解決**: 確保日期格式為 "yyyy-MM-dd"

### 權限問題
**問題**: RLS 阻止訪問
**解決**: 在 Supabase 控制台調整 RLS 政策或暫時禁用

## 診斷輸出範例

### 成功案例
```
=== Supabase 快速診斷 ===
🔧 檢查 Supabase 設定...
API URL: https://lobezwpworbfktlkxuyo.supabase.co
Project ID: lobezwpworbfktlkxuyo
API Key: ✅ 已設定

🌐 測試基本連線...
✅ 基本連線成功

📋 檢查資料表...
✅ 資料表 'food' 存在，包含 多筆 記錄
✅ 資料表 'subscription' 存在，包含 多筆 記錄

🔄 測試 CRUD 操作...
📖 測試讀取食品資料...
✅ 成功讀取 5 項食品
📖 測試讀取訂閱資料...
✅ 成功讀取 3 項訂閱

📄 測試頁面資料載入...
✅ BackendServiceFactory 正確創建 SupabaseService
✅ CrudManager 成功載入 5 項食品資料

=== Supabase 快速診斷完成 ===
```

### 錯誤案例
```
=== Supabase 快速診斷 ===
🔧 檢查 Supabase 設定...
API URL: https://lobezwpworbfktlkxuyo.supabase.co
Project ID: lobezwpworbfktlkxuyo
API Key: ✅ 已設定

🌐 測試基本連線...
❌ 基本連線失敗: Unauthorized
錯誤詳情: {"message":"Invalid API key"}

建議: 請檢查 API Key 是否正確
```

## 後續改進建議

### 功能增強
1. **批量測試**: 支援批量創建測試資料
2. **效能測試**: 測試大量資料的載入效能
3. **並發測試**: 測試多個同時請求的處理
4. **資料驗證**: 更嚴格的資料格式驗證

### 使用者體驗
1. **進度指示**: 顯示測試進度條
2. **詳細報告**: 生成測試報告文件
3. **自動修復**: 自動修復常見的設定問題
4. **測試排程**: 定期自動執行測試

### 監控和日誌
1. **錯誤追蹤**: 詳細的錯誤日誌記錄
2. **效能監控**: API 回應時間監控
3. **使用統計**: 功能使用情況統計
4. **健康檢查**: 定期健康狀態檢查

## 總結

Supabase 綜合測試功能提供了完整的診斷和測試工具，確保：

✅ **連線穩定性**: 全面的連線測試和診斷
✅ **功能完整性**: 涵蓋所有 CRUD 操作
✅ **整合正確性**: 驗證與其他組件的整合
✅ **使用者友好**: 提供清晰的診斷結果和建議
✅ **問題解決**: 快速定位和解決常見問題

透過這些測試工具，可以快速確認 Supabase 的食品和訂閱功能是否正常運作，並在出現問題時提供具體的解決建議。