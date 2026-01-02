# Supabase updated_at 欄位修正

## 問題發現

用戶正確指出了關鍵問題：
- **Appwrite** 有 `$updatedAt` 欄位（系統自動管理）
- **Supabase** 沒有 `updated_at` 欄位（除非手動創建）

這就是為什麼 CSV 中 `updated_at` 欄位是空的！

## 修正方案

### 1. 更新 CSV 格式

**修正前的 CSV 格式**:
```csv
id,created_at,updated_at,name,price,photo,shop,todate,account
```

**修正後的 CSV 格式**:
```csv
id,created_at,name,price,photo,shop,todate,account
```

### 2. 更新表結構

**Food 表結構** (移除 updated_at):
```sql
CREATE TABLE food (
    id UUID DEFAULT gen_random_uuid() PRIMARY KEY,
    created_at TIMESTAMPTZ DEFAULT NOW(),
    name TEXT,
    price BIGINT DEFAULT 0,
    photo TEXT,
    shop TEXT,
    todate TEXT,
    account TEXT
);
```

**Subscription 表結構** (移除 updated_at):
```sql
CREATE TABLE subscription (
    id UUID DEFAULT gen_random_uuid() PRIMARY KEY,
    created_at TIMESTAMPTZ DEFAULT NOW(),
    name TEXT,
    nextdate TEXT,
    price BIGINT DEFAULT 0,
    site TEXT,
    account TEXT,
    note TEXT
);
```

## 修正的檔案

### 1. SettingsPage.xaml.cs
- 修正 `GenerateFoodCsv()` 方法
- 修正 `GenerateSubscriptionCsv()` 方法
- 移除 `updated_at` 欄位的處理

### 2. SQL 腳本
- `CREATE_FOOD_TABLE_FIXED.sql` - 正確的 food 表結構
- `CREATE_SUBSCRIPTION_TABLE_FIXED.sql` - 正確的 subscription 表結構

## 正確的 CSV 格式

### Food CSV:
```csv
id,created_at,name,price,photo,shop,todate,account
"dfdef1b4-e091-40ec-904e-58709cdc4909","2026-01-02T17:09:09.823Z","測試食品",100,"https://example.com/photo.jpg","測試商店","2026-01-09T16:00:00.000Z","test@example.com"
```

### Subscription CSV:
```csv
id,created_at,name,nextdate,price,site,account,note
"12345678-1234-1234-1234-123456789012","2026-01-02T17:09:09.823Z","Netflix","2026-02-01",390,"https://netflix.com","test@example.com","影音串流服務"
```

## 執行步驟

### 1. 更新 Supabase 表結構
在 Supabase SQL Editor 中執行：
```sql
-- 如果表已存在且有 updated_at 欄位，先刪除
ALTER TABLE food DROP COLUMN IF EXISTS updated_at;
ALTER TABLE subscription DROP COLUMN IF EXISTS updated_at;
```

### 2. 重新導出 CSV
使用修正後的程式碼重新導出 CSV 文件。

### 3. 導入 Supabase
現在 CSV 格式應該與表結構完全匹配，可以成功導入。

## 驗證

導入成功後，CSV 應該：
- ✅ 沒有 `updated_at` 欄位
- ✅ 欄位數量與表結構匹配
- ✅ 所有資料類型正確
- ✅ 成功導入到 Supabase

## 重要提醒

- Appwrite 使用 `$updatedAt`（系統管理）
- Supabase 預設沒有 `updated_at`（需手動創建）
- 不同後端服務的欄位結構不同，CSV 生成需要區分處理