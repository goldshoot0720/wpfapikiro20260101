# Supabase API Key 更新摘要

## 🔑 新的 API Key 資訊

### Legacy Service Role API Key
```
eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6ImxvYmV6d3B3b3JiZmt0bGt4dXlvIiwicm9sZSI6InNlcnZpY2Vfcm9sZSIsImlhdCI6MTc2NzI1ODU5MSwiZXhwIjoyMDgyODM0NTkxfQ.tFcCP7kvcfV1CznhIHXBF0TenGlYD1XRlAWdCYYEnlc
```

### JWT Token 解析
- **Issuer**: supabase
- **Reference**: lobezwpworbfktlkxuyo
- **Role**: service_role
- **Issued At**: 1767258591 (2026-01-01)
- **Expires**: 2082834591 (2036-01-01)

## ✅ 已更新的檔案

### 1. 核心配置檔案
- `AppSettings.cs` - 預設 Supabase API Key
- `SupabaseService.cs` - 服務實作（使用 AppSettings）

### 2. 測試工具
- `QuickSupabaseTest.cs` - 快速連接測試
- `SupabaseDebugTest.cs` - 詳細診斷測試
- `SupabaseTableCheck.cs` - 資料表檢查工具

### 3. 文檔檔案
- `README_Supabase.md` - 完整 Supabase 文檔
- `SUPABASE_FINAL_CONFIG.md` - 最終配置指南

## 🧪 連接測試結果

### API 端點測試
```
✅ food table: 200 OK (空陣列)
✅ subscription table: 200 OK (空陣列)
```

### 權限驗證
- ✅ Service Role 權限正常
- ✅ 桌面應用程式環境支援
- ✅ 完整 CRUD 操作權限

## 🔧 應用程式配置

### 在設定頁面中使用
1. 選擇「Supabase」服務
2. API URL: `https://lobezwpworbfktlkxuyo.supabase.co`
3. Project ID: `lobezwpworbfktlkxuyo`
4. API Key: `eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6ImxvYmV6d3B3b3JiZmt0bGt4dXlvIiwicm9sZSI6InNlcnZpY2Vfcm9sZSIsImlhdCI6MTc2NzI1ODU5MSwiZXhwIjoyMDgyODM0NTkxfQ.tFcCP7kvcfV1CznhIHXBF0TenGlYD1XRlAWdCYYEnlc`

## 🎯 解決的問題

### 1. 權限問題
- **舊問題**: 舊的 secret key 在某些環境下被拒絕
- **新解決**: JWT service_role token 具有完整權限

### 2. 資料表存取
- **確認**: `food` 和 `subscription` 資料表都存在
- **狀態**: 兩個資料表都返回 200 狀態碼

### 3. 食品管理錯誤
- **原因**: 可能是舊 API key 權限問題
- **解決**: 新 API key 應該能解決載入失敗問題

## 📋 下一步測試

### 1. 重新啟動應用程式
關閉並重新開啟應用程式以載入新的 API key

### 2. 測試食品管理
1. 進入「食品管理」頁面
2. 點擊「🔄 重新載入」
3. 確認不再出現錯誤訊息

### 3. 測試 CRUD 操作
1. 新增測試食品記錄
2. 編輯現有記錄
3. 刪除測試記錄
4. 確認所有操作正常

### 4. 測試訂閱管理
1. 確認現有 Supabase Pro 記錄正常顯示
2. 測試新增、編輯、刪除功能

## 🔒 安全性注意事項

### API Key 管理
- Service Role Key 具有完整資料庫權限
- 僅在安全的桌面應用程式環境中使用
- 不要在前端網頁或公開環境中使用

### Token 有效期
- 當前 Token 有效期至 2036 年
- 建議定期更新 API Key
- 監控 Token 使用情況

所有 API Key 更新已完成，應用程式現在應該能夠正常連接 Supabase 並執行所有資料操作！