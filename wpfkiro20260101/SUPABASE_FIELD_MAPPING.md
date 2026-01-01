# Supabase 實際欄位對照表

## 基於實際資料表結構的欄位對照

根據 Supabase Dashboard 截圖，以下是實際的資料表結構和欄位對照：

## 📊 Food 資料表 (實際表名: `food`)

### 實際欄位結構
| 欄位名稱 | 資料類型 | 說明 |
|---------|---------|------|
| `id` | UUID | 主鍵，自動生成 |
| `created_at` | timestamp | 創建時間，自動生成 |
| `name` | text | 食品名稱 |
| `todate` | text | 到期日期 |
| `account` | text | 帳戶資訊 |
| `photo` | text | 照片 URL |
| `price` | int8 | 價格 |
| `shop` | text | 商店名稱 |

### 應用程式欄位對照
| 應用程式屬性 | Supabase 欄位 | 對照說明 |
|-------------|---------------|----------|
| `Id` | `id` | UUID 主鍵 |
| `FoodName` | `name` | 食品名稱 |
| `Price` | `price` | 價格（整數） |
| `Photo` | `photo` | 照片 URL |
| `Shop` | `shop` | 商店名稱 |
| `ToDate` | `todate` | 到期日期（文字格式） |
| `CreatedAt` | `created_at` | 創建時間 |
| `PhotoHash` | - | 不存在於 Supabase 表中 |
| `Description` | - | 不存在於 Supabase 表中 |
| `Category` | - | 不存在於 Supabase 表中 |
| `StorageLocation` | - | 不存在於 Supabase 表中 |
| `Note` | - | 不存在於 Supabase 表中 |
| `Quantity` | - | 不存在於 Supabase 表中 |
| `UpdatedAt` | - | 不存在於 Supabase 表中 |

## 📋 Subscriptions 資料表 (實際表名: `subscriptions`)

### 實際欄位結構
| 欄位名稱 | 資料類型 | 說明 |
|---------|---------|------|
| `id` | UUID | 主鍵，自動生成 |
| `created_at` | timestamp | 創建時間，自動生成 |
| `name` | text | 訂閱名稱 |
| `nextdate` | date | 下次付款日期 |
| `price` | int8 | 價格 |
| `site` | text | 網站 URL |
| `note` | text | 備註 |
| `account` | text | 帳戶資訊 |

### 應用程式欄位對照
| 應用程式屬性 | Supabase 欄位 | 對照說明 |
|-------------|---------------|----------|
| `Id` | `id` | UUID 主鍵 |
| `SubscriptionName` | `name` | 訂閱名稱 |
| `NextDate` | `nextdate` | 下次付款日期 |
| `Price` | `price` | 價格（整數） |
| `Site` | `site` | 網站 URL |
| `Account` | `account` | 帳戶資訊 |
| `Note` | `note` | 備註 |
| `CreatedAt` | `created_at` | 創建時間 |
| `StringToDate` | - | 不存在於 Supabase 表中 |
| `DateTime` | - | 不存在於 Supabase 表中 |
| `FoodId` | - | 不存在於 Supabase 表中 |
| `UpdatedAt` | - | 不存在於 Supabase 表中 |

## 🔧 SupabaseService 修正內容

### API 端點修正
- **Food 表**: `/rest/v1/food` (不是 `/rest/v1/foods`)
- **Subscriptions 表**: `/rest/v1/subscriptions` (正確)

### 欄位名稱修正

#### Food 相關方法
```csharp
// 修正前
data["food_name"] = food.FoodName;
data["to_date"] = food.ToDate;

// 修正後
data["name"] = food.FoodName;
data["todate"] = food.ToDate;
```

#### Subscription 相關方法
```csharp
// 修正前
data["subscription_name"] = subscription.SubscriptionName;
data["next_date"] = subscription.NextDate.ToString("yyyy-MM-dd");

// 修正後
data["name"] = subscription.SubscriptionName;
data["nextdate"] = subscription.NextDate.ToString("yyyy-MM-dd");
```

## 📝 資料處理注意事項

### 日期格式
- **Food.todate**: 文字格式，可以是任何日期字串
- **Subscriptions.nextdate**: DATE 類型，需要 `yyyy-MM-dd` 格式

### 價格處理
- 兩個表的 `price` 欄位都是 `int8` 類型
- 應用程式中使用整數處理價格

### 缺失欄位處理
- 應用程式中的某些屬性在 Supabase 表中不存在
- 在讀取時設為空值或預設值
- 在寫入時忽略這些欄位

## 🧪 測試建議

### 連接測試
1. 使用提供的 API 金鑰測試基本連接
2. 確認能夠讀取空的資料表
3. 測試創建、讀取、更新、刪除操作

### 資料驗證
1. 確認日期格式正確處理
2. 驗證價格欄位的整數轉換
3. 測試中文字元的正確儲存和讀取

### API 端點驗證
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

## 🔄 CSV 匯出對照

### Food CSV 格式
```csv
id,name,price,photo,shop,todate,account,created_at
```

### Subscriptions CSV 格式
```csv
id,name,nextdate,price,site,note,account,created_at
```

## ⚠️ 重要提醒

1. **表名差異**: Food 表名為 `food`（單數），不是 `foods`
2. **欄位名稱**: 使用實際的欄位名稱，不是應用程式的屬性名稱
3. **資料類型**: 注意 `price` 是整數，`nextdate` 是日期類型
4. **缺失欄位**: 某些應用程式屬性在資料庫中不存在，需要適當處理

## 📋 後續工作

1. ✅ 更新 SupabaseService.cs 以匹配實際欄位
2. 🔄 測試所有 CRUD 操作
3. 🔄 驗證 CSV 匯出功能
4. 🔄 確認日期排序功能正常運作