# 立即修正 Supabase 切換問題

## 🚨 當前問題

**症狀**: 選擇 Supabase 後，測試設定仍顯示 Appwrite 內容
**根本原因**: 應用程式還在運行舊的程式碼，修正還沒有生效

## 🔧 立即解決方案

### 步驟 1: 重新啟動應用程式 (必須)
```
1. 完全關閉當前的應用程式
2. 等待 5 秒鐘
3. 重新開啟應用程式
4. 這會載入修正後的程式碼
```

### 步驟 2: 手動強制設定 Supabase
如果重新啟動後仍有問題，請手動設定：

```
1. 進入「系統設定」頁面
2. 選擇「Supabase」選項
3. 手動清空並重新輸入以下值：

API URL: https://lobezwpworbfktlkxuyo.supabase.co
Project ID: lobezwpworbfktlkxuyo
API Key: eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6ImxvYmV6d3B3b3JiZmt0bGt4dXlvIiwicm9sZSI6InNlcnZpY2Vfcm9sZSIsImlhdCI6MTc2NzI1ODU5MSwiZXhwIjoyMDgyODM0NTkxfQ.tFcCP7kvcfV1CznhIHXBF0TenGlYD1XRlAWdCYYEnlc

4. 點擊「儲存設定」
5. 點擊「測試設定」確認顯示 Supabase 內容
```

### 步驟 3: 驗證修正結果
```
1. 點擊「測試設定」按鈕
2. 確認彈出對話框顯示：
   - BackendService: 1 (Supabase)
   - ApiUrl: https://lobezwpworbfktlkxuyo.supabase.co
   - ProjectId: lobezwpworbfktlkxuyo

3. 點擊「測試連線」按鈕
4. 確認連接成功

5. 進入「食品管理」頁面
6. 確認不再顯示 BadRequest 錯誤
```

## 🔍 如果仍有問題

### 方法 1: 刪除設定檔案重新開始
```
1. 關閉應用程式
2. 刪除設定檔案：
   路徑: %AppData%\wpfkiro20260101\settings.json
3. 重新開啟應用程式
4. 重新配置 Supabase 設定
```

### 方法 2: 檢查設定檔案內容
```
1. 開啟檔案總管
2. 輸入路徑: %AppData%\wpfkiro20260101\
3. 開啟 settings.json 檔案
4. 確認內容為：
{
  "BackendService": 1,
  "ApiUrl": "https://lobezwpworbfktlkxuyo.supabase.co",
  "ProjectId": "lobezwpworbfktlkxuyo",
  "ApiKey": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "DatabaseId": "",
  "BucketId": ""
}
```

### 方法 3: 手動編輯設定檔案
如果應用程式無法正確保存設定：
```
1. 關閉應用程式
2. 用記事本開啟 %AppData%\wpfkiro20260101\settings.json
3. 替換為以下內容：
{
  "BackendService": 1,
  "ApiUrl": "https://lobezwpworbfktlkxuyo.supabase.co",
  "ProjectId": "lobezwpworbfktlkxuyo",
  "ApiKey": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6ImxvYmV6d3B3b3JiZmt0bGt4dXlvIiwicm9sZSI6InNlcnZpY2Vfcm9sZSIsImlhdCI6MTc2NzI1ODU5MSwiZXhwIjoyMDgyODM0NTkxfQ.tFcCP7kvcfV1CznhIHXBF0TenGlYD1XRlAWdCYYEnlc",
  "DatabaseId": "",
  "BucketId": "",
  "FoodCollectionId": "food",
  "SubscriptionCollectionId": "subscription"
}
4. 儲存檔案
5. 重新開啟應用程式
```

## ✅ 成功指標

修正成功後，您應該看到：

### 測試設定對話框
```
當前界面選擇的設定:
{
  "BackendService": 1,
  "ApiUrl": "https://lobezwpworbfktlkxuyo.supabase.co",
  "ProjectId": "lobezwpworbfktlkxuyo",
  "ApiKey": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "DatabaseId": "",
  "BucketId": ""
}

RadioButton 狀態:
Supabase: True
Appwrite: False
```

### 應用程式功能
1. **連接測試**: 顯示「連線測試成功！(Supabase)」
2. **食品管理**: 不再顯示 BadRequest 錯誤
3. **頁面標題**: 顯示 [Supabase] 而不是 [Appwrite]
4. **CSV 匯出**: 檔名前綴為 supabase

## 🚨 重要提醒

**最關鍵的步驟是重新啟動應用程式！**

我們修正了程式碼，但是：
- 應用程式還在記憶體中運行舊的程式碼
- 必須重新啟動才能載入修正後的程式碼
- 這就是為什麼選擇 Supabase 後仍顯示 Appwrite 的原因

請務必：
1. **完全關閉應用程式**
2. **重新開啟應用程式**
3. **然後再測試 Supabase 功能**

這樣修正就會生效了！