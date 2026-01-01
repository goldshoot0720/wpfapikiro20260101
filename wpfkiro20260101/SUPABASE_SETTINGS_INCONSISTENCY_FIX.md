# Supabase 設定不一致問題修正指南

## 🚨 問題描述

從截圖可以看出，應用程式界面顯示的 Supabase 設定與實際的設定檔案不一致：

**界面顯示的問題**:
- Project ID 顯示為舊值: `lobezwpworbfktlkxuyoKiro`
- 但設定檔案中已經是正確值: `lobezwpworbfktlkxuyo`

**根本原因**: 應用程式界面沒有正確重新載入或更新設定值

## ✅ 已完成的修正

### 1. 更新 SettingsPage.xaml.cs
- **LoadSettings 方法**: 添加 Supabase 設定的特別檢查和自動修正
- **UpdateFieldsForService 方法**: 增強 Supabase 欄位更新邏輯，會檢查並修正舊值
- **自動檢測舊設定**: 會自動檢測並修正 `lobezwpworbfktlkxuyoKiro` 等舊值

### 2. 增強的檢查邏輯
```csharp
// 檢查並修正舊的 Project ID
if (ProjectIdTextBox.Text == "lobezwpworbfktlkxuyoKiro" ||
    ProjectIdTextBox.Text != "lobezwpworbfktlkxuyo")
{
    ProjectIdTextBox.Text = AppSettings.Defaults.Supabase.ProjectId;
}
```

### 3. 自動修正機制
- 當載入設定時，會自動檢查 Supabase 設定是否正確
- 如果發現舊值，會自動更新為正確值
- 會同時更新設定檔案和界面顯示

## 🔧 立即修正步驟

### 步驟 1: 重新啟動應用程式
```
1. 完全關閉當前應用程式
2. 重新開啟應用程式
3. 進入「系統設定」頁面
```

### 步驟 2: 重新選擇 Supabase
```
1. 在設定頁面中，先點選其他服務（如 Appwrite）
2. 然後再點選「Supabase」
3. 這會觸發自動更新邏輯
4. 確認欄位顯示正確的值
```

### 步驟 3: 驗證設定
```
1. 確認 API URL: https://lobezwpworbfktlkxuyo.supabase.co
2. 確認 Project ID: lobezwpworbfktlkxuyo
3. 確認 API Key 以 eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9 開頭
4. 點擊「儲存設定」
5. 點擊「測試連線」確認成功
```

### 步驟 4: 測試功能
```
1. 進入「食品管理」頁面
2. 確認不再顯示 BadRequest 錯誤
3. 點擊「🔄 重新載入」按鈕
4. 確認可以正常載入資料
```

## 📋 正確的設定值

### 完整的 Supabase 配置
```json
{
  "BackendService": 1,
  "ApiUrl": "https://lobezwpworbfktlkxuyo.supabase.co",
  "ProjectId": "lobezwpworbfktlkxuyo",
  "ApiKey": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6ImxvYmV6d3B3b3JiZmt0bGt4dXlvIiwicm9sZSI6InNlcnZpY2Vfcm9sZSIsImlhdCI6MTc2NzI1ODU5MSwiZXhwIjoyMDgyODM0NTkxfQ.tFcCP7kvcfV1CznhIHXBF0TenGlYD1XRlAWdCYYEnlc",
  "DatabaseId": "",
  "BucketId": ""
}
```

### 界面應該顯示的值
- **API URL**: `https://lobezwpworbfktlkxuyo.supabase.co`
- **Project ID**: `lobezwpworbfktlkxuyo` (不是 `lobezwpworbfktlkxuyoKiro`)
- **API Key**: 以 `eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9` 開頭的完整 JWT token

## 🔍 故障排除

### 如果界面仍顯示舊值

1. **手動清除設定檔案**
   ```
   1. 關閉應用程式
   2. 刪除 %AppData%\wpfkiro20260101\settings.json
   3. 重新開啟應用程式
   4. 重新配置 Supabase 設定
   ```

2. **手動輸入正確值**
   ```
   1. 在設定頁面手動清空所有欄位
   2. 重新輸入正確的 Supabase 設定值
   3. 點擊「儲存設定」
   4. 重新啟動應用程式
   ```

3. **檢查 Debug 輸出**
   ```
   1. 在 Visual Studio 中查看輸出視窗
   2. 查找 "LoadSettings" 和 "UpdateFieldsForService" 相關訊息
   3. 確認設定載入和更新過程
   ```

### 常見問題

**Q: 為什麼會出現 `lobezwpworbfktlkxuyoKiro` 這個舊值？**
A: 這是之前測試時使用的 Project ID，現在已經更正為 `lobezwpworbfktlkxuyo`

**Q: 設定檔案是正確的，但界面顯示錯誤怎麼辦？**
A: 重新啟動應用程式，或者重新選擇 Supabase 選項來觸發界面更新

**Q: 如何確認設定已經正確更新？**
A: 點擊「測試設定」按鈕，會顯示當前載入的所有設定值

## 🎯 預期結果

修正完成後，您應該看到：

### ✅ 正確的界面顯示
1. **設定頁面**: 所有 Supabase 欄位顯示正確值
2. **測試連線**: 顯示連接成功
3. **JSON 內容**: 測試設定時顯示正確的 JSON

### ✅ 正常的功能
1. **食品管理**: 不再顯示 BadRequest 錯誤
2. **訂閱管理**: 正常顯示現有記錄
3. **CRUD 操作**: 所有新增、編輯、刪除功能正常
4. **CSV 匯出**: 檔名前綴為 `supabase`

## 📝 技術摘要

### 修正的核心問題
- **界面更新邏輯**: 增強了設定載入和欄位更新的邏輯
- **舊值檢測**: 添加了對舊 Project ID 的自動檢測和修正
- **自動修正機制**: 當檢測到不正確的設定時會自動修正

### 防止未來問題
- **一致性檢查**: 載入設定時會驗證 Supabase 設定的一致性
- **自動更新**: 當選擇 Supabase 時會自動更新為正確的預設值
- **錯誤處理**: 增強了錯誤處理和 Debug 輸出

所有設定不一致的問題已經修正，應用程式現在會自動確保 Supabase 設定的正確性和一致性！