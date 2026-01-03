# Supabase CSV 導入指南

## 🎯 問題解決

**問題**：`The data that you are trying to import is incompatible with your table structure`

**原因**：CSV 文件的列名與 Supabase 表結構不匹配

**解決狀態**：✅ **已修正**

## 🔧 修正內容

### 1. CSV 導出功能已更新

現在 CSV 導出功能會根據當前使用的後端服務生成正確的列名：

**Supabase 服務時的 CSV 格式**：
- Food: `id,name,price,photo,shop,todate,account,created_at,updated_at`
- Subscription: `id,name,nextdate,price,site,note,account,created_at,updated_at`

**Appwrite 服務時的 CSV 格式**：
- Food: `$id,name,price,photo,shop,todate,photohash,$createdAt,$updatedAt`
- Subscription: `$id,name,nextdate,price,site,note,account,$createdAt,$updatedAt`

## 📋 Supabase 表結構對應

### Food 表
| CSV 列名 | Supabase 列名 | 類型 | 說明 |
|---------|-------------|------|------|
| id | id | UUID | 主鍵 |
| name | name | TEXT | 食品名稱 |
| price | price | BIGINT | 價格 |
| photo | photo | TEXT | 照片 URL |
| shop | shop | TEXT | 商店名稱 |
| todate | todate | TEXT | 到期日期 |
| account | account | TEXT | 帳戶資訊 |
| created_at | created_at | TIMESTAMPTZ | 創建時間 |
| updated_at | updated_at | TIMESTAMPTZ | 更新時間 |

### Subscription 表
| CSV 列名 | Supabase 列名 | 類型 | 說明 |
|---------|-------------|------|------|
| id | id | UUID | 主鍵 |
| name | name | TEXT | 訂閱名稱 |
| nextdate | nextdate | TEXT | 下次付款日期 |
| price | price | BIGINT | 價格 |
| site | site | TEXT | 網站 URL |
| note | note | TEXT | 備註 |
| account | account | TEXT | 帳戶資訊 |
| created_at | created_at | TIMESTAMPTZ | 創建時間 |
| updated_at | updated_at | TIMESTAMPTZ | 更新時間 |

## 🚀 使用步驟

### 1. 重新導出 CSV 文件

1. **重新啟動應用程式** - 確保載入最新的修正程式碼
2. **確認使用 Supabase** - 在設定中選擇 Supabase 服務
3. **重新導出 CSV** - 點擊「📥 下載 food.csv」或「📥 下載 subscription.csv」
4. **檢查文件名** - 應該是 `supabasefood.csv` 或 `supabasesubscription.csv`

### 2. 驗證 CSV 格式

打開導出的 CSV 文件，確認標題行是：

**Food CSV**：
```csv
id,name,price,photo,shop,todate,account,created_at,updated_at
```

**Subscription CSV**：
```csv
id,name,nextdate,price,site,note,account,created_at,updated_at
```

### 3. 導入到 Supabase

1. **登入 Supabase Dashboard**
2. **選擇你的項目**
3. **進入 Table Editor**
4. **選擇要導入的表** (food 或 subscription)
5. **點擊 Import data**
6. **上傳 CSV 文件**
7. **確認列映射正確**
8. **執行導入**

## 📊 CSV 範例

### Food CSV 範例
```csv
id,name,price,photo,shop,todate,account,created_at,updated_at
"550e8400-e29b-41d4-a716-446655440000","蘋果","50","https://example.com/apple.jpg","水果店","2026-02-01","user@example.com","2026-01-03T10:00:00Z","2026-01-03T10:00:00Z"
"550e8400-e29b-41d4-a716-446655440001","香蕉","30","https://example.com/banana.jpg","水果店","2026-01-25","user@example.com","2026-01-03T10:00:00Z","2026-01-03T10:00:00Z"
```

### Subscription CSV 範例
```csv
id,name,nextdate,price,site,note,account,created_at,updated_at
"550e8400-e29b-41d4-a716-446655440000","Netflix","2026-02-01","390","netflix.com","家庭方案","user@example.com","2026-01-03T10:00:00Z","2026-01-03T10:00:00Z"
"550e8400-e29b-41d4-a716-446655440001","Spotify","2026-01-15","149","spotify.com","個人方案","user@example.com","2026-01-03T10:00:00Z","2026-01-03T10:00:00Z"
```

## 🔍 導入注意事項

### 1. ID 欄位處理
- **新資料**：可以留空，Supabase 會自動生成 UUID
- **現有資料**：保持原有的 UUID 格式

### 2. 日期格式
- **建議格式**：`YYYY-MM-DD` (如：2026-01-03)
- **時間戳格式**：`YYYY-MM-DDTHH:MM:SSZ` (如：2026-01-03T10:00:00Z)

### 3. 數值欄位
- **價格**：使用整數，不包含貨幣符號
- **空值**：使用空字串 `""` 或 `NULL`

### 4. 文字欄位
- **包含逗號**：用雙引號包圍
- **包含雙引號**：使用兩個雙引號轉義 `""`

## 🛠️ 疑難排解

### 導入仍然失敗？

1. **檢查列名**：確保 CSV 標題行與表結構完全匹配
2. **檢查資料類型**：確保數值欄位不包含非數字字符
3. **檢查日期格式**：使用標準的 ISO 日期格式
4. **檢查 UUID 格式**：確保 ID 欄位是有效的 UUID 或留空

### 手動修正 CSV

如果你有舊的 CSV 文件，可以手動修正標題行：

**修正前（Appwrite 格式）**：
```csv
$id,name,price,photo,shop,todate,photohash,$createdAt,$updatedAt
```

**修正後（Supabase 格式）**：
```csv
id,name,price,photo,shop,todate,account,created_at,updated_at
```

### 批量替換

在文字編輯器中使用查找替換功能：
- `$id` → `id`
- `$createdAt` → `created_at`
- `$updatedAt` → `updated_at`
- `photohash` → `account` (如果需要)

## 🎉 修正完成

現在你的 CSV 導出功能會根據當前使用的後端服務生成正確的格式：

- ✅ **Supabase 服務** - 生成 Supabase 兼容的 CSV
- ✅ **Appwrite 服務** - 生成 Appwrite 兼容的 CSV
- ✅ **自動列名映射** - 無需手動修改
- ✅ **正確的資料格式** - 符合各服務的要求

重新導出 CSV 文件後，應該可以成功導入到 Supabase 了！