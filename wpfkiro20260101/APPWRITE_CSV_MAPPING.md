# Appwrite CSV 匯出欄位對照

## 📊 根據實際 Appwrite 資料庫結構

基於您提供的 Appwrite 控制台截圖，CSV 匯出功能已完全對照實際的資料庫欄位結構。

## 🗂️ Food 表 CSV 格式

### 檔案名稱
`appwritefood.csv`

### CSV 標題行
```csv
$id,name,price,photo,shop,todate,photohash,$createdAt,$updatedAt
```

### 欄位對照表
| CSV 欄位 | Appwrite 欄位 | 資料類型 | 必填 | 說明 |
|---------|-------------|---------|------|------|
| `$id` | `$id` | string | ✅ | Appwrite 文檔 ID |
| `name` | `name` | string | ✅ | 食品名稱 (Size: 1000) |
| `price` | `price` | integer | ❌ | 價格 (預設: NULL) |
| `photo` | `photo` | string | ❌ | 照片 URL (Size: 1000, 預設: NULL) |
| `shop` | `shop` | string | ❌ | 商店名稱 (Size: 1000, 預設: NULL) |
| `todate` | `todate` | datetime | ❌ | 到期日期 (預設: NULL) |
| `photohash` | `photohash` | string | ❌ | 照片雜湊值 (Size: 10000, 預設: NULL) |
| `$createdAt` | `$createdAt` | datetime | ✅ | 系統創建時間 |
| `$updatedAt` | `$updatedAt` | datetime | ✅ | 系統更新時間 |

### 範例資料
```csv
$id,name,price,photo,shop,todate,photohash,$createdAt,$updatedAt
"67890abc","蘋果",50,"https://example.com/apple.jpg","全聯","2024-12-31","abc123","2024-01-01T10:00:00.000Z","2024-01-01T10:00:00.000Z"
"12345def","香蕉",30,"","家樂福","2024-12-25","","2024-01-01T11:00:00.000Z","2024-01-01T11:00:00.000Z"
```

## 🗂️ Subscription 表 CSV 格式

### 檔案名稱
`appwritesubscription.csv`

### CSV 標題行
```csv
$id,name,nextdate,price,site,note,account,$createdAt,$updatedAt
```

### 欄位對照表
| CSV 欄位 | Appwrite 欄位 | 資料類型 | 必填 | 說明 |
|---------|-------------|---------|------|------|
| `$id` | `$id` | string | ✅ | Appwrite 文檔 ID |
| `name` | `name` | string | ✅ | 訂閱名稱 (Size: 1000) |
| `nextdate` | `nextdate` | datetime | ❌ | 下次付款日期 (預設: NULL) |
| `price` | `price` | integer | ❌ | 價格 (預設: NULL) |
| `site` | `site` | string | ❌ | 網站 URL (Size: 1000, 預設: NULL) |
| `note` | `note` | string | ❌ | 備註 (Size: 1000, 預設: NULL) |
| `account` | `account` | string | ❌ | 帳戶資訊 (Size: 1000, 預設: NULL) |
| `$createdAt` | `$createdAt` | datetime | ✅ | 系統創建時間 |
| `$updatedAt` | `$updatedAt` | datetime | ✅ | 系統更新時間 |

### 範例資料
```csv
$id,name,nextdate,price,site,note,account,$createdAt,$updatedAt
"sub001","Netflix","2024-02-15T00:00:00.000Z",390,"https://netflix.com","家庭方案","user@example.com","2024-01-01T10:00:00.000Z","2024-01-01T10:00:00.000Z"
"sub002","Spotify","2024-02-10T00:00:00.000Z",149,"https://spotify.com","個人方案","music@example.com","2024-01-01T11:00:00.000Z","2024-01-01T11:00:00.000Z"
```

## 🔧 技術實現細節

### 欄位值獲取邏輯
```csharp
// 支援多種可能的屬性名稱，確保相容性
var id = GetPropertyValue(item, "$id", "id", "Id") ?? "";
var name = GetPropertyValue(item, "name", "foodName", "FoodName") ?? "";
var price = GetPropertyValue(item, "price", "Price") ?? "0";
// ... 其他欄位
```

### CSV 格式處理
- **引號處理**: 所有字串欄位都用雙引號包圍
- **逗號轉義**: 內容中的逗號會被正確處理
- **空值處理**: NULL 值會顯示為空字串
- **編碼格式**: UTF-8 with BOM，確保中文正確顯示

## 📋 與其他後端服務的差異

### Appwrite 特色
- 使用 `$id` 而非 `id`
- 系統欄位有 `$` 前綴
- 日期時間格式為 ISO 8601
- 字串欄位有大小限制 (Size: 1000/10000)

### 與 Supabase 的差異
| 欄位 | Appwrite | Supabase |
|------|----------|----------|
| ID | `$id` | `id` |
| 創建時間 | `$createdAt` | `created_at` |
| 更新時間 | `$updatedAt` | `updated_at` |
| 食品名稱 | `name` | `food_name` |
| 到期日期 | `todate` | `to_date` |

## ✅ 驗證方式

### 1. 檢查 CSV 標題
確認匯出的 CSV 文件第一行包含正確的欄位名稱：
```
$id,name,price,photo,shop,todate,photohash,$createdAt,$updatedAt
```

### 2. 檢查資料格式
- ID 欄位應該是 Appwrite 的文檔 ID 格式
- 日期時間應該是 ISO 8601 格式
- 價格應該是整數

### 3. 測試匯入
可以將匯出的 CSV 重新匯入到新的 Appwrite 專案中進行驗證。

## 🚀 使用建議

1. **定期備份**: 建議定期匯出 CSV 作為資料備份
2. **資料遷移**: 可用於在不同 Appwrite 專案間遷移資料
3. **資料分析**: 在 Excel 或其他工具中分析資料
4. **除錯用途**: 檢查資料完整性和格式正確性

這個 CSV 匯出功能完全對照您的 Appwrite 資料庫結構，確保資料的完整性和正確性！