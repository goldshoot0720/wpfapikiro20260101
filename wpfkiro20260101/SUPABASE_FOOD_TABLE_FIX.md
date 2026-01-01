# Supabase Food 資料表修正指南

## 🚨 問題診斷

**錯誤訊息**: "Supabase 載入失敗：無法找到有效的 Supabase 食品資料表端點"

**原因分析**:
- `subscription` 資料表存在且正常運作
- `food` 資料表可能不存在或無法存取
- 應用程式嘗試存取 `/rest/v1/food` 端點時失敗

## 🔧 解決方案

### 方案 1: 在 Supabase Dashboard 中創建 food 資料表

1. **登入 Supabase Dashboard**
   - 前往 https://supabase.com/dashboard
   - 選擇專案 `lobezwpworbfktlkxuyo`

2. **開啟 SQL Editor**
   - 點擊左側選單的「SQL Editor」
   - 點擊「New query」

3. **執行 SQL 腳本**
   - 複製 `CREATE_FOOD_TABLE.sql` 中的內容
   - 貼上並執行腳本

### 方案 2: 使用 Table Editor 手動創建

1. **進入 Table Editor**
   - 在 Supabase Dashboard 中點擊「Table Editor」
   - 點擊「Create a new table」

2. **設定資料表**
   - **Table name**: `food`
   - **Enable Row Level Security**: 勾選

3. **新增欄位**
   ```
   id          | uuid     | Primary Key, Default: gen_random_uuid()
   created_at  | timestamptz | Default: now()
   updated_at  | timestamptz | Default: now()
   name        | text     | 食品名稱
   price       | bigint   | 價格
   photo       | text     | 照片 URL
   shop        | text     | 商店名稱
   todate      | text     | 到期日期
   account     | text     | 帳戶資訊
   ```

4. **設定 RLS 政策**
   - 進入「Authentication」→「Policies」
   - 為 `food` 資料表創建政策：
     ```sql
     CREATE POLICY "Allow all operations on food" 
     ON food FOR ALL 
     USING (true);
     ```

## 🧪 測試驗證

### 1. 使用內建測試工具
```csharp
// 在應用程式中執行
await SupabaseTableCheck.CheckTables();
```

### 2. 手動測試
1. 重新啟動應用程式
2. 進入「食品管理」頁面
3. 點擊「🔄 重新載入」按鈕
4. 確認不再出現錯誤訊息

### 3. CRUD 操作測試
1. 點擊「➕ 新增食品」
2. 填入測試資料並儲存
3. 確認資料正確顯示
4. 測試編輯和刪除功能

## 📋 預期的資料表結構

### food 資料表欄位對照
| 應用程式屬性 | Supabase 欄位 | 類型 | 說明 |
|-------------|---------------|------|------|
| Id | id | UUID | 主鍵 |
| FoodName | name | TEXT | 食品名稱 |
| Price | price | BIGINT | 價格 |
| Photo | photo | TEXT | 照片 URL |
| Shop | shop | TEXT | 商店名稱 |
| ToDate | todate | TEXT | 到期日期 |
| Account | account | TEXT | 帳戶資訊 |
| CreatedAt | created_at | TIMESTAMPTZ | 創建時間 |
| UpdatedAt | updated_at | TIMESTAMPTZ | 更新時間 |

## 🔍 故障排除

### 如果問題持續存在

1. **檢查 API 權限**
   - 確認 API Key 有正確的權限
   - 檢查 RLS 政策是否正確設定

2. **檢查資料表名稱**
   - 確認資料表名稱為 `food`（小寫，單數）
   - 不是 `foods`、`Food` 或其他變體

3. **檢查網路連接**
   - 確認能夠連接到 Supabase
   - 檢查防火牆設定

4. **查看詳細錯誤**
   - 在 Visual Studio 的「輸出」視窗中查看詳細的除錯訊息
   - 檢查 Supabase Dashboard 的 Logs

## 📝 完成檢查清單

- [ ] `food` 資料表已創建
- [ ] RLS 已啟用並設定政策
- [ ] 資料表欄位結構正確
- [ ] 應用程式能夠成功載入食品資料
- [ ] CRUD 操作正常運作
- [ ] 錯誤訊息不再出現

完成以上步驟後，食品管理功能應該能夠正常運作。