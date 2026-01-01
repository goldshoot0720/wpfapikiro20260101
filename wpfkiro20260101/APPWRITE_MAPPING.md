# Appwrite 資料庫對照表

## 📊 實際 Appwrite 資料庫結構

根據提供的截圖，您的 Appwrite 資料庫包含以下表：

### 資料庫資訊
- **資料庫ID**: `69565a2800074e1d96c5`
- **專案ID**: `69565017002c03b93af8`

### 表結構

#### 1. `food` 表
- **表ID**: `food`
- **用途**: 儲存食品相關資料
- **對應功能**: 食品管理頁面
- **實際欄位**:
  - `name` (string) - 食品名稱
  - `todate` (string) - 到期日期
  - `photo` (string) - 照片路徑
  - `price` (integer) - 價格
  - `shop` (string) - 商店名稱
  - `photohash` (string) - 照片雜湊值

#### 2. `subscription` 表  
- **表ID**: `subscription`
- **用途**: 儲存訂閱相關資料
- **對應功能**: 訂閱管理頁面
- **實際欄位**:
  - `name` (string) - 訂閱名稱
  - `nextdate` (string) - 下次付款日期
  - `price` (integer) - 價格
  - `site` (string) - 網站URL
  - `note` (string) - 備註
  - `account` (string) - 帳戶資訊

## 🔧 代碼對照修正

### AppSettings.cs 修正
```csharp
// 修正後設定 (正確)
public const string FoodCollectionId = "food";
public const string SubscriptionCollectionId = "subscription";
```

### AppwriteService.cs 修正

#### 食品資料載入 - 實際欄位對照
```csharp
var foods = documents.Documents.Select(doc => new
{
    id = doc.Id,
    foodName = doc.Data.TryGetValue("name", out var name) ? name?.ToString() ?? "" : "",
    toDate = doc.Data.TryGetValue("todate", out var todate) ? todate?.ToString() ?? "" : "",
    photo = doc.Data.TryGetValue("photo", out var photo) ? photo?.ToString() ?? "" : "",
    price = doc.Data.TryGetValue("price", out var price) && int.TryParse(price?.ToString(), out var p) ? p : 0,
    shop = doc.Data.TryGetValue("shop", out var shop) ? shop?.ToString() ?? "" : "",
    photoHash = doc.Data.TryGetValue("photohash", out var photoHash) ? photoHash?.ToString() ?? "" : ""
});
```

#### 訂閱資料載入 - 實際欄位對照
```csharp
var subscriptions = documents.Documents.Select(doc => new
{
    id = doc.Id,
    name = doc.Data.TryGetValue("name", out var name) ? name?.ToString() ?? "" : "",
    nextDate = doc.Data.TryGetValue("nextdate", out var nextdate) ? nextdate?.ToString() ?? "" : "",
    price = doc.Data.TryGetValue("price", out var price) && int.TryParse(price?.ToString(), out var p) ? p : 0,
    site = doc.Data.TryGetValue("site", out var site) ? site?.ToString() ?? "" : "",
    note = doc.Data.TryGetValue("note", out var note) ? note?.ToString() ?? "" : "",
    account = doc.Data.TryGetValue("account", out var account) ? account?.ToString() ?? "" : ""
});
```

## 📋 欄位對照表

### Food 表欄位對照
| 代碼中的屬性 | Appwrite欄位 | 類型 | 說明 |
|-------------|-------------|------|------|
| foodName | name | string | 食品名稱 |
| toDate | todate | string | 到期日期 |
| photo | photo | string | 照片路徑 |
| price | price | integer | 價格 |
| shop | shop | string | 商店名稱 |
| photoHash | photohash | string | 照片雜湊值 |

### Subscription 表欄位對照
| 代碼中的屬性 | Appwrite欄位 | 類型 | 說明 |
|-------------|-------------|------|------|
| name | name | string | 訂閱名稱 |
| nextDate | nextdate | string | 下次付款日期 |
| price | price | integer | 價格 |
| site | site | string | 網站URL |
| note | note | string | 備註 |
| account | account | string | 帳戶資訊 |

## ✅ 修正完成項目

1. ✅ 更新 AppSettings 中的集合ID常數
2. ✅ 修正 AppwriteService 中的集合ID引用
3. ✅ 更新食品資料載入方法使用實際欄位名稱
4. ✅ 更新訂閱資料載入方法使用實際欄位名稱
5. ✅ 修正創建和更新方法的欄位對照
6. ✅ 所有CRUD操作都使用正確的Appwrite欄位名稱

## 🎯 測試建議

1. **測試食品管理頁面**
   - 進入食品管理頁面
   - 點擊「🔄 重新載入」按鈕
   - 檢查是否正確載入 `food` 表的資料
   - 驗證欄位對照是否正確

2. **測試訂閱管理頁面**
   - 進入訂閱管理頁面
   - 點擊「🔄 重新載入」按鈕
   - 檢查是否正確載入 `subscription` 表的資料
   - 驗證欄位對照是否正確

3. **檢查錯誤處理**
   - 如果表為空，應顯示「從 Appwrite 載入了 0 項資料」
   - 如果連接失敗，應顯示具體錯誤訊息

## 📝 重要變更

- **Food表**: 移除了 `datetime` 欄位，改用 `todate` 字串欄位
- **Subscription表**: 使用 `nextdate`、`site`、`note`、`account` 等實際欄位
- **價格欄位**: 兩個表都使用 `integer` 類型而非 `double`
- **欄位命名**: 完全對照實際Appwrite表結構