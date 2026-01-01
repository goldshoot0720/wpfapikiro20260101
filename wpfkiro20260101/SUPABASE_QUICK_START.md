# Supabase 快速開始指南

## 🚀 快速配置步驟

### 1. 在應用程式中配置 Supabase

1. **開啟應用程式**
2. **進入設定頁面**
3. **選擇 Supabase 作為後端服務**
4. **填入連接資訊**：
   ```
   API URL: https://lobezwpworbfktlkxuyo.supabase.co
   API Key: sb_secret_B2gtQik_DZEKevBc82viAw_mbvPA8F1
   Project ID: lobezwpworbfktlkxuyo
   ```

### 2. 驗證連接

1. **進入食品管理頁面**
2. **點擊「🔄 重新載入」按鈕**
3. **檢查是否顯示「從 Supabase 載入了 X 項資料」**

### 3. 測試基本功能

#### 測試食品管理
1. 點擊「➕ 添加食品」
2. 填入測試資料：
   - 食品名稱：測試蘋果
   - 價格：50
   - 商店：全聯
   - 到期日期：2026-02-01
3. 點擊「添加」
4. 確認食品出現在列表中

#### 測試訂閱管理
1. 進入訂閱管理頁面
2. 點擊「➕ 添加訂閱」
3. 填入測試資料：
   - 訂閱名稱：Netflix
   - 網站：https://netflix.com
   - 月費：390
   - 下次付款：2026-02-01
4. 點擊「添加」
5. 確認訂閱出現在列表中

## 📊 資料表結構確認

您的 Supabase 資料庫已包含以下資料表：

### Food 資料表
- ✅ `id` (UUID)
- ✅ `created_at` (timestamp)
- ✅ `name` (text)
- ✅ `todate` (text)
- ✅ `account` (text)
- ✅ `photo` (text)
- ✅ `price` (int8)
- ✅ `shop` (text)

### Subscriptions 資料表
- ✅ `id` (UUID)
- ✅ `created_at` (timestamp)
- ✅ `name` (text)
- ✅ `nextdate` (date)
- ✅ `price` (int8)
- ✅ `site` (text)
- ✅ `note` (text)
- ✅ `account` (text)

## 🔧 已完成的修正

### SupabaseService 更新
- ✅ 修正 API 端點：`/rest/v1/food` (不是 foods)
- ✅ 更新欄位對照：`name` 而不是 `food_name`
- ✅ 修正日期欄位：`nextdate` 和 `todate`
- ✅ 移除不存在的欄位處理

### 功能支援
- ✅ 食品 CRUD 操作
- ✅ 訂閱 CRUD 操作
- ✅ 日期排序功能
- ✅ CSV 匯出功能
- ✅ 網路圖片支援
- ✅ 可點擊 URL

## 🧪 測試連接

如果需要測試連接，可以使用以下 curl 命令：

```bash
# 測試 Food API
curl -H "apikey: sb_secret_B2gtQik_DZEKevBc82viAw_mbvPA8F1" \
     -H "Authorization: Bearer sb_secret_B2gtQik_DZEKevBc82viAw_mbvPA8F1" \
     https://lobezwpworbfktlkxuyo.supabase.co/rest/v1/food

# 測試 Subscriptions API
curl -H "apikey: sb_secret_B2gtQik_DZEKevBc82viAw_mbvPA8F1" \
     -H "Authorization: Bearer sb_secret_B2gtQik_DZEKevBc82viAw_mbvPA8F1" \
     https://lobezwpworbfktlkxuyo.supabase.co/rest/v1/subscriptions
```

## ❓ 常見問題

### Q: 連接失敗怎麼辦？
A: 檢查以下項目：
1. API URL 是否正確
2. API Key 是否完整
3. 網路連接是否正常
4. Supabase 專案是否啟用

### Q: 資料無法儲存？
A: 可能原因：
1. RLS (Row Level Security) 政策問題
2. 欄位類型不匹配
3. 必填欄位缺失

### Q: 中文字元顯示異常？
A: Supabase 預設支援 UTF-8，應該不會有問題。如果有問題，檢查：
1. 應用程式的字元編碼設定
2. HTTP 請求的 Content-Type

## 📞 支援資源

- [Supabase 官方文件](https://supabase.com/docs)
- [REST API 文件](https://supabase.com/docs/guides/api/rest/introduction)
- [專案 Dashboard](https://supabase.com/dashboard/project/lobezwpworbfktlkxuyo)

## 🎉 完成！

現在您的應用程式已經可以與 Supabase 正常連接和操作了！您可以：

1. ✅ 管理食品資料
2. ✅ 管理訂閱資料
3. ✅ 匯出 CSV 檔案
4. ✅ 使用網路圖片
5. ✅ 點擊訂閱網址
6. ✅ 按日期排序顯示