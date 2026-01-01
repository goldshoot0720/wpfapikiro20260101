# Supabase 連接成功配置

## ✅ 已完成的配置

### 1. 正確的連接資訊
- **Project ID**: `lobezwpworbfktlkxuyo`
- **API URL**: `https://lobezwpworbfktlkxuyo.supabase.co`
- **API Key**: `sb_secret_B2gtQik_DZEKevBc82viAw_mbvPA8F1` (使用 Secret Key)

### 2. 確認的資料表結構
- ✅ `food` 資料表存在且可存取
- ✅ `subscription` 資料表存在且可存取
- ❌ `foods` (複數) 不存在
- ❌ `subscriptions` (複數) 不存在

### 3. 更新的服務配置
- 更新 `SupabaseService.cs` 使用正確的資料表名稱
- 更新 `AppSettings.cs` 包含正確的 Supabase 預設值
- 更新所有文件使用正確的 API 端點

### 4. API 端點確認
```
✅ GET  /rest/v1/food        - 回應 200 (空陣列)
✅ GET  /rest/v1/subscription - 回應 200 (空陣列)
❌ GET  /rest/v1/foods       - 回應 404
❌ GET  /rest/v1/subscriptions - 回應 404
```

## 🔧 重要發現

### API Key 使用
- **Secret Key** (`sb_secret_...`) 用於桌面應用程式，具有完整權限
- **Publishable Key** (`sb_publishable_...`) 用於前端應用程式，權限受限
- 桌面應用程式應使用 Secret Key 進行 API 呼叫

### 資料表命名
- Supabase 中的資料表使用單數形式：`food`, `subscription`
- 不是複數形式：`foods`, `subscriptions`

## 📋 下一步行動

### 1. 測試 CRUD 操作
現在可以測試完整的 CRUD 操作：
- 創建食品和訂閱記錄
- 讀取資料列表
- 更新現有記錄
- 刪除記錄

### 2. 在應用程式中配置
在應用程式設定頁面中：
1. 選擇 "Supabase" 作為後端服務
2. 填入：
   - **API URL**: `https://lobezwpworbfktlkxuyo.supabase.co`
   - **API Key**: `sb_secret_B2gtQik_DZEKevBc82viAw_mbvPA8F1`
   - **Project ID**: `lobezwpworbfktlkxuyo`

### 3. 測試連接
使用內建的測試工具驗證連接：
```csharp
// 快速測試
await QuickSupabaseTest.TestConnection();

// 完整診斷
await SupabaseDebugTest.RunDiagnosticTests();
```

## 🎯 成功指標

- [x] API 連接成功 (200 回應)
- [x] 資料表可存取
- [x] 正確的 API Key 配置
- [x] 服務程式碼更新完成
- [ ] CRUD 操作測試
- [ ] 應用程式整合測試

## 📝 配置摘要

```json
{
  "BackendService": "Supabase",
  "ApiUrl": "https://lobezwpworbfktlkxuyo.supabase.co",
  "ApiKey": "sb_secret_B2gtQik_DZEKevBc82viAw_mbvPA8F1",
  "ProjectId": "lobezwpworbfktlkxuyo"
}
```

Supabase 連接問題已成功解決！現在可以進行完整的資料操作測試。