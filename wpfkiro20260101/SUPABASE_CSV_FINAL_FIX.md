# Supabase CSV 導入修正方案

## 問題分析

用戶在嘗試將 CSV 文件導入 Supabase 時遇到 "DATA INCOMPATIBLE" 錯誤。經過分析發現以下問題：

### 1. 表名不一致問題
- **應用程式使用**: `subscription` (單數)
- **SUPABASE_SETUP.sql 創建**: `subscriptions` (複數)
- **結果**: 應用程式無法找到正確的表

### 2. CSV 欄位順序問題
- **原 CSV 格式**: `id,name,nextdate,price,site,note,account,created_at,updated_at`
- **正確順序**: `id,created_at,updated_at,name,nextdate,price,site,account,note`

## 修正方案

### 步驟 1: 創建正確的 subscription 表

在 Supabase SQL Editor 中執行 `CREATE_SUBSCRIPTION_TABLE.sql`:

```sql
-- 創建 subscription 資料表 (單數，與應用程式一致)
CREATE TABLE IF NOT EXISTS subscription (
    id UUID DEFAULT gen_random_uuid() PRIMARY KEY,
    created_at TIMESTAMPTZ DEFAULT NOW(),
    updated_at TIMESTAMPTZ DEFAULT NOW(),
    name TEXT,
    nextdate TEXT,
    price BIGINT DEFAULT 0,
    site TEXT,
    account TEXT,
    note TEXT
);

-- 啟用 RLS 和政策
ALTER TABLE subscription ENABLE ROW LEVEL SECURITY;
CREATE POLICY "Allow all operations on subscription" 
ON subscription FOR ALL 
USING (true);
```

### 步驟 2: 驗證表結構

確認兩個表都存在且可訪問：
- `food` 表 (已存在)
- `subscription` 表 (新創建)

### 步驟 3: 重新導出 CSV

使用修正後的 CSV 生成器，現在會產生正確的欄位順序：

**Food CSV 格式**:
```
id,created_at,updated_at,name,price,photo,shop,todate,account
```

**Subscription CSV 格式**:
```
id,created_at,updated_at,name,nextdate,price,site,account,note
```

## 修正的程式碼變更

### SettingsPage.xaml.cs 修正

1. **修正 Subscription CSV 標題行順序**:
   ```csharp
   // 修正前
   csv.AppendLine("id,name,nextdate,price,site,note,account,created_at,updated_at");
   
   // 修正後
   csv.AppendLine("id,created_at,updated_at,name,nextdate,price,site,account,note");
   ```

2. **修正 CSV 資料行順序**:
   ```csharp
   // 修正後的格式
   csv.AppendLine($"\"{EscapeCsvField(id)}\",\"{createdAtFormatted}\",\"{updatedAtFormatted}\",\"{EscapeCsvField(name)}\",\"{nextdate}\",\"{price}\",\"{EscapeCsvField(site)}\",\"{EscapeCsvField(account)}\",\"{EscapeCsvField(note)}\"");
   ```

## 測試步驟

1. **執行表結構診斷**:
   ```csharp
   await SupabaseTableStructureFix.RunDiagnosis();
   ```

2. **重新導出 CSV**:
   - 在設定頁面點擊 "📥 下載 food.csv"
   - 在設定頁面點擊 "📥 下載 subscription.csv"

3. **導入 Supabase**:
   - 在 Supabase Dashboard 中選擇對應的表
   - 使用 Import data 功能
   - 上傳新生成的 CSV 文件

## 預期結果

修正後應該能夠成功：
- ✅ 生成與 Supabase 表結構完全匹配的 CSV 文件
- ✅ 成功導入 CSV 到 Supabase 而不出現 "DATA INCOMPATIBLE" 錯誤
- ✅ 應用程式能正常讀取和操作 Supabase 中的資料

## 驗證方法

1. 檢查 Supabase 中是否有 `food` 和 `subscription` 兩個表
2. 確認表結構與 CSV 標題行完全匹配
3. 測試 CSV 導入功能
4. 在應用程式中測試資料讀取功能

## 注意事項

- 確保 API Key 有足夠的權限
- 確認 RLS 政策允許所有操作（開發環境）
- 如果仍有問題，檢查 Supabase 的錯誤日誌