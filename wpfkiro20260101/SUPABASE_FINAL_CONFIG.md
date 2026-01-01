# Supabase 最終配置設定

## ✅ 正確的連接設定

### 連接資訊
- **API URL**: `https://lobezwpworbfktlkxuyo.supabase.co`
- **Project ID**: `lobezwpworbfktlkxuyo`
- **API Key**: `eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6ImxvYmV6d3B3b3JiZmt0bGt4dXlvIiwicm9sZSI6InNlcnZpY2Vfcm9sZSIsImlhdCI6MTc2NzI1ODU5MSwiZXhwIjoyMDgyODM0NTkxfQ.tFcCP7kvcfV1CznhIHXBF0TenGlYD1XRlAWdCYYEnlc`

### 資料表名稱
- ✅ `food` (食品資料表)
- ✅ `subscription` (訂閱資料表)

## 🔧 應用程式設定步驟

### 1. 在應用程式中配置
1. 開啟應用程式
2. 進入「系統設定」頁面
3. 選擇「Supabase」作為後端服務
4. 填入連線設定：
   - **API URL**: `https://lobezwpworbfktlkxuyo.supabase.co`
   - **Project ID**: `lobezwpworbfktlkxuyo`
   - **API Key**: `eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6ImxvYmV6d3B3b3JiZmt0bGt4dXlvIiwicm9sZSI6InNlcnZpY2Vfcm9sZSIsImlhdCI6MTc2NzI1ODU5MSwiZXhwIjoyMDgyODM0NTkxfQ.tFcCP7kvcfV1CznhIHXBF0TenGlYD1XRlAWdCYYEnlc`

### 2. 儲存設定
點擊「儲存」按鈕保存配置

### 3. 測試連接
- 進入「食品管理」頁面，點擊「🔄 重新載入」
- 進入「訂閱管理」頁面，點擊「🔄 重新載入」
- 確認可以成功連接並載入資料

## 📋 API 端點

### 食品管理 API
```
GET    /rest/v1/food           - 獲取所有食品
POST   /rest/v1/food           - 創建新食品
PATCH  /rest/v1/food?id=eq.{id} - 更新食品
DELETE /rest/v1/food?id=eq.{id} - 刪除食品
```

### 訂閱管理 API
```
GET    /rest/v1/subscription           - 獲取所有訂閱
POST   /rest/v1/subscription           - 創建新訂閱
PATCH  /rest/v1/subscription?id=eq.{id} - 更新訂閱
DELETE /rest/v1/subscription?id=eq.{id} - 刪除訂閱
```

## 🎯 功能測試

### 測試項目
- [ ] 連接測試成功
- [ ] 食品資料 CRUD 操作
- [ ] 訂閱資料 CRUD 操作
- [ ] CSV 匯出功能
- [ ] 網路圖片載入
- [ ] 日期排序功能

### 測試方法
1. **新增測試**: 創建新的食品和訂閱記錄
2. **讀取測試**: 確認資料正確顯示
3. **編輯測試**: 修改現有記錄
4. **刪除測試**: 刪除測試記錄
5. **匯出測試**: 下載 CSV 檔案

## 🔍 故障排除

### 常見問題
1. **連接失敗**: 檢查網路連接和 API 設定
2. **權限錯誤**: 確認使用正確的 Secret Key
3. **資料表錯誤**: 確認資料表名稱為單數形式

### 診斷工具
```csharp
// 快速連接測試
await QuickSupabaseTest.TestConnection();

// 完整診斷測試
await SupabaseDebugTest.RunDiagnosticTests();
```

## 📝 重要提醒

### API Key 安全性
- Secret Key 具有完整資料庫權限
- 僅在安全的桌面應用程式環境中使用
- 不要在前端網頁應用程式中使用 Secret Key

### 資料備份
- 定期使用 CSV 匯出功能備份資料
- 建議在進行大量資料操作前先備份

Supabase 配置已完成，可以開始使用完整的資料管理功能！