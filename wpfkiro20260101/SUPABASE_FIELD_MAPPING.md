# Supabase 欄位對照表 - 修正完成

## 資料表結構對照

### Food 資料表 (food)

| Supabase 欄位 | 應用程式欄位 | 類型 | 說明 | 狀態 |
|--------------|-------------|------|------|------|
| `id` | `id` | bigint | 主鍵，自動遞增 | ✅ 正確 |
| `created_at` | `createdAt` | timestamptz | 創建時間 | ✅ 正確 |
| `name` | `foodName` | text | 食品名稱 | ✅ 正確 |
| `nextdate` | `toDate` | date | 到期日期 | ✅ 已修正 |
| `price` | `price` | int8 | 價格 | ✅ 正確 |
| `site` | `shop` | text | 商店/網站 | ✅ 已修正 |
| `note` | `note` | text | 備註 | ✅ 正確 |
| `photohash` | `photoHash` | text | 圖片雜湊 | ✅ 已修正 |

### Subscription 資料表 (subscription)

| Supabase 欄位 | 應用程式欄位 | 類型 | 說明 | 狀態 |
|--------------|-------------|------|------|------|
| `id` | `id` | bigint | 主鍵，自動遞增 | ✅ 正確 |
| `created_at` | `createdAt` | timestamptz | 創建時間 | ✅ 正確 |
| `name` | `subscriptionName` | text | 訂閱名稱 | ✅ 正確 |
| `nextdate` | `nextDate` | date | 下次付款日期 | ✅ 正確 |
| `price` | `price` | int8 | 價格 | ✅ 正確 |
| `site` | `site` | text | 網站 | ✅ 正確 |
| `note` | `note` | text | 備註 | ✅ 正確 |
| `account` | `account` | text | 帳戶資訊 | ✅ 正確 |

## 已修正的欄位映射

### ✅ Food 資料表修正完成
- **讀取映射** (GetFoodsAsync): 
  - `shop` ← `site` ✅ 已修正
  - `toDate` ← `nextdate` ✅ 已修正  
  - `photo` ← `photohash` ✅ 已修正

- **寫入映射** (CreateFoodAsync/UpdateFoodAsync):
  - `data["site"]` ← `food.Shop` ✅ 已修正
  - `data["nextdate"]` ← `food.ToDate` ✅ 已修正
  - `data["photohash"]` ← `food.Photo` ✅ 已修正

### ✅ Subscription 資料表確認正確
- 所有欄位映射都正確，無需修正

## 修正前後對照

### Food 資料寫入修正
```csharp
// ❌ 修正前 (錯誤的欄位名稱)
data["shop"] = food.Shop;      // 應該是 site
data["todate"] = food.ToDate;  // 應該是 nextdate  
data["photo"] = food.Photo;    // 應該是 photohash

// ✅ 修正後 (正確的欄位名稱)
data["site"] = food.Shop;      // 正確
data["nextdate"] = food.ToDate; // 正確
data["photohash"] = food.Photo; // 正確
```

### Food 資料讀取修正
```csharp
// ❌ 修正前 (錯誤的欄位名稱)
shop = item.TryGetProperty("shop", out var shop) ? shop.GetString() : "",
toDate = item.TryGetProperty("todate", out var toDate) ? toDate.GetString() : "",
photo = item.TryGetProperty("photo", out var photo) ? photo.GetString() : "",

// ✅ 修正後 (正確的欄位名稱)
shop = item.TryGetProperty("site", out var site) ? site.GetString() : "",
toDate = item.TryGetProperty("nextdate", out var nextdate) ? nextdate.GetString() : "",
photo = item.TryGetProperty("photohash", out var photohash) ? photohash.GetString() : "",
```

## 測試驗證

✅ **已完成修正的項目**:
- Food 資料讀取欄位映射
- Food 資料寫入欄位映射 (CreateFoodAsync)
- Food 資料更新欄位映射 (UpdateFoodAsync)
- Subscription 資料映射確認正確

✅ **測試檔案**: `TestSupabaseFieldMappingFixed.cs`
- 驗證連接功能
- 驗證資料讀取
- 驗證資料寫入
- 驗證欄位映射正確性

## 使用說明

現在 Supabase 服務已經可以正確與實際的資料庫結構配合使用：

1. **Food 資料操作**: 所有 CRUD 操作都使用正確的欄位映射
2. **Subscription 資料操作**: 欄位映射本來就正確
3. **相容性**: 應用程式介面保持不變，只是底層映射修正

## 注意事項

- 應用程式層面的欄位名稱保持不變 (`shop`, `toDate`, `photo`)
- 只有與 Supabase 通訊時才使用正確的欄位名稱 (`site`, `nextdate`, `photohash`)
- 這樣保持了應用程式的相容性，同時修正了資料庫映射問題