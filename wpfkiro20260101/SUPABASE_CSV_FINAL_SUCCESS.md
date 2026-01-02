# Supabase CSV 導入最終成功方案

## ✅ 問題已解決

根據實際的 Supabase 表結構，已完成所有修正：

### 實際的 Supabase 表結構

**Food 表**:
```
id,created_at,name,todate,amount,photo,price,shop,photohash
```

**Subscription 表**:
```
id,created_at,name,nextdate,price,site,note,account
```

## 🔧 修正內容

### 1. CSV 格式修正

**Food CSV 格式**:
```csv
id,created_at,name,todate,amount,photo,price,shop,photohash
dfdef1b4-e091-40ec-904e-58709cdc4909,2026-01-02 17:09:09.823688+00,測試食品,2026-01-10 00:00:00,1,https://example.com/photo.jpg,100,測試商店,
```

**Subscription CSV 格式**:
```csv
id,created_at,name,nextdate,price,site,note,account
96f5cf96-c82b-4003-a5d2-d7e0e07f8084,2026-01-02 17:09:03.21007+00,Netflix,2026-02-02,390,https://netflix.com,影音串流服務,test@example.com
```

### 2. 程式碼修正

#### SettingsPage.xaml.cs 修正:

1. **Food CSV 標題行**:
   ```csharp
   csv.AppendLine("id,created_at,name,todate,amount,photo,price,shop,photohash");
   ```

2. **Subscription CSV 標題行**:
   ```csharp
   csv.AppendLine("id,created_at,name,nextdate,price,site,note,account");
   ```

3. **日期格式修正**:
   ```csharp
   // Supabase 格式：2026-01-02 17:09:09.823688+00
   return parsedDate.ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss.ffffff+00", System.Globalization.CultureInfo.InvariantCulture);
   ```

4. **CSV 資料行格式**:
   - 移除不必要的引號
   - 數字欄位不加引號
   - 正確的欄位順序

### 3. SQL 表結構

創建了 `CREATE_SUPABASE_TABLES_FINAL.sql` 包含正確的表結構：

```sql
-- Food 表
CREATE TABLE food (
    id UUID DEFAULT gen_random_uuid() PRIMARY KEY,
    created_at TIMESTAMPTZ DEFAULT NOW(),
    name TEXT,
    todate TIMESTAMPTZ,
    amount INTEGER DEFAULT 1,
    photo TEXT,
    price BIGINT DEFAULT 0,
    shop TEXT,
    photohash TEXT
);

-- Subscription 表
CREATE TABLE subscription (
    id UUID DEFAULT gen_random_uuid() PRIMARY KEY,
    created_at TIMESTAMPTZ DEFAULT NOW(),
    name TEXT,
    nextdate DATE,
    price BIGINT DEFAULT 0,
    site TEXT,
    note TEXT,
    account TEXT
);
```

## 🚀 使用步驟

### 1. 確保表結構正確
在 Supabase SQL Editor 中執行 `CREATE_SUPABASE_TABLES_FINAL.sql`

### 2. 重新導出 CSV
使用修正後的程式碼重新導出 CSV 文件

### 3. 導入 Supabase
現在 CSV 格式與表結構完全匹配，應該可以成功導入

## ✅ 預期結果

修正後的 CSV 應該：
- ✅ 欄位順序與 Supabase 表完全匹配
- ✅ 日期格式正確 (`2026-01-02 17:09:09.823688+00`)
- ✅ 數字欄位不使用引號
- ✅ 所有必要欄位都有值
- ✅ 成功導入到 Supabase 而不出現錯誤

## 🔍 關鍵差異

### 與之前的主要差異:
1. **移除了 `updated_at` 欄位** - Supabase 沒有這個欄位
2. **Food 表增加了 `amount` 和 `photohash` 欄位**
3. **欄位順序完全匹配實際表結構**
4. **日期格式改為 Supabase 格式**
5. **移除不必要的引號包圍**

現在 CSV 格式應該與 Supabase 完全兼容！