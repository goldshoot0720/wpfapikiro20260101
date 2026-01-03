# Supabase CSV 導入成功指南

## 當前狀況分析

根據您提供的 CSV 資料：
```
id,created_at,updated_at,name,price,photo,shop,todate,account
"dfdef1b4-e091-40ec-904e-58709cdc4909","2026-01-02T17:09:09.823Z","","2","0","https://nextshadcn20250928.vercel.app/images/1761405863-3ca40781-b24f-4c48-9795-7bc061f58ed6.jpeg","","2026-01-09T16:00:00.000Z",""
```

## 發現的問題

### 1. **updated_at 欄位為空**
- 當前值：`""`（空字串）
- 建議：使用 `created_at` 的值或當前時間戳

### 2. **數字欄位格式**
- `price` 欄位：當前為 `"0"`（字串），應該是 `0`（數字）
- Supabase 期望數字欄位不要用引號包圍

### 3. **必填欄位檢查**
- `name` 欄位：當前為 `"2"`，看起來不是有效的食品名稱
- `shop` 欄位：當前為空

## 修正方案

### 步驟 1: 確保 Supabase 表結構正確

在 Supabase SQL Editor 中執行以下查詢來檢查表結構：

```sql
-- 檢查 food 表結構
SELECT column_name, data_type, is_nullable, column_default
FROM information_schema.columns 
WHERE table_name = 'food'
ORDER BY ordinal_position;
```

### 步驟 2: 修正 CSV 資料格式

正確的 CSV 格式應該是：

```csv
id,created_at,updated_at,name,price,photo,shop,todate,account
"dfdef1b4-e091-40ec-904e-58709cdc4909","2026-01-02T17:09:09.823Z","2026-01-02T17:09:09.823Z","測試食品",0,"https://nextshadcn20250928.vercel.app/images/1761405863-3ca40781-b24f-4c48-9795-7bc061f58ed6.jpeg","測試商店","2026-01-09T16:00:00.000Z","test@example.com"
```

### 步驟 3: 檢查資料類型匹配

確保 CSV 中的資料類型與 Supabase 表定義匹配：

| 欄位 | Supabase 類型 | CSV 格式 | 範例 |
|------|---------------|----------|------|
| id | UUID | 字串（加引號） | `"dfdef1b4-e091-40ec-904e-58709cdc4909"` |
| created_at | TIMESTAMPTZ | 字串（加引號） | `"2026-01-02T17:09:09.823Z"` |
| updated_at | TIMESTAMPTZ | 字串（加引號） | `"2026-01-02T17:09:09.823Z"` |
| name | TEXT | 字串（加引號） | `"測試食品"` |
| price | BIGINT | 數字（不加引號） | `100` |
| photo | TEXT | 字串（加引號） | `"https://example.com/photo.jpg"` |
| shop | TEXT | 字串（加引號） | `"測試商店"` |
| todate | TEXT | 字串（加引號） | `"2026-01-09T16:00:00.000Z"` |
| account | TEXT | 字串（加引號） | `"test@example.com"` |

## 導入步驟

### 1. 準備 CSV 文件
- 確保使用 UTF-8 編碼
- 確保每行都有完整的 9 個欄位
- 確保數字欄位不使用引號

### 2. 在 Supabase 中導入
1. 進入 Supabase Dashboard
2. 選擇 `food` 表
3. 點擊 "Import data"
4. 選擇 CSV 文件
5. 確認欄位映射正確
6. 執行導入

### 3. 驗證導入結果
```sql
-- 檢查導入的資料
SELECT * FROM food ORDER BY created_at DESC LIMIT 5;

-- 檢查資料筆數
SELECT COUNT(*) as total_records FROM food;
```

## 常見問題解決

### 問題 1: "Column count mismatch"
**原因**: CSV 欄位數與表欄位數不匹配
**解決**: 確保 CSV 有完整的 9 個欄位

### 問題 2: "Invalid UUID format"
**原因**: ID 欄位格式不正確
**解決**: 確保 ID 是有效的 UUID 格式

### 問題 3: "Invalid timestamp format"
**原因**: 時間戳格式不正確
**解決**: 使用 ISO 8601 格式：`YYYY-MM-DDTHH:mm:ss.sssZ`

### 問題 4: "Data type mismatch"
**原因**: 資料類型不匹配
**解決**: 確保數字欄位不使用引號，文字欄位使用引號

## 測試建議

1. **先測試小量資料**: 只導入 1-2 筆資料進行測試
2. **檢查欄位映射**: 確認 Supabase 正確識別每個欄位
3. **驗證資料完整性**: 導入後檢查資料是否正確顯示

## 成功指標

導入成功後，您應該能夠：
- ✅ 在 Supabase 表中看到新資料
- ✅ 應用程式能正常讀取資料
- ✅ 所有欄位都有正確的值和格式
- ✅ 沒有資料遺失或格式錯誤