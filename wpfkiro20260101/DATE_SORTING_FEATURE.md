# 日期排序功能實現

## 功能描述
為食品管理和訂閱管理頁面添加日期排序功能，將資料按日期由近到遠排序（最近的日期在前面）。

## 實現的功能

### 1. 訂閱頁面排序 (SubscriptionPage.xaml.cs)
- **排序欄位**: `nextdate` (下次付款日期)
- **排序方式**: 升序排列 (OrderBy)
- **顯示效果**: 最近要付款的訂閱顯示在最前面

### 2. 食品頁面排序 (FoodPage.xaml.cs)
- **主要排序欄位**: `todate` (到期日期)
- **備用排序欄位**: `createdAt` (創建時間)
- **排序方式**: 升序排列 (OrderBy)
- **顯示效果**: 最近到期或最新添加的食品顯示在最前面

## 技術實現

### 新增方法

#### SubscriptionPage.xaml.cs
```csharp
private object[] SortSubscriptionsByDate(object[] subscriptionData)
{
    return subscriptionData.OrderBy(item =>
    {
        var nextDate = GetPropertyValue(item, "nextdate", "nextDate", "NextDate") ?? "";
        if (DateTime.TryParse(nextDate, out DateTime parsedDate))
        {
            return parsedDate;
        }
        return DateTime.MaxValue; // 無效日期排在最後
    }).ToArray();
}
```

#### FoodPage.xaml.cs
```csharp
private object[] SortFoodsByDate(object[] foodData)
{
    return foodData.OrderBy(item =>
    {
        // 優先使用到期日期
        var toDate = GetPropertyValue(item, "todate", "toDate", "ToDate") ?? "";
        if (DateTime.TryParse(toDate, out DateTime parsedDate))
        {
            return parsedDate;
        }
        
        // 備用：使用創建時間
        var createdAt = GetPropertyValue(item, "$createdAt", "createdAt", "CreatedAt") ?? "";
        if (DateTime.TryParse(createdAt, out DateTime createdDate))
        {
            return createdDate;
        }
        
        return DateTime.MaxValue; // 無效日期排在最後
    }).ToArray();
}

private string GetPropertyValue(object obj, params string[] propertyNames)
{
    // 通用屬性值獲取方法，支援多種屬性名稱格式
}
```

### 修改的方法

#### UpdateSubscriptionList()
```csharp
// 原本：直接遍歷原始資料
foreach (var item in subscriptionData)

// 修改後：先排序再遍歷
var sortedData = SortSubscriptionsByDate(subscriptionData);
foreach (var item in sortedData)
```

#### UpdateFoodList()
```csharp
// 原本：直接遍歷原始資料
foreach (var item in foodData)

// 修改後：先排序再遍歷
var sortedData = SortFoodsByDate(foodData);
foreach (var item in sortedData)
```

## 排序邏輯

### 日期解析策略
1. **多欄位支援**: 支援不同的屬性名稱格式 (如 "nextdate", "nextDate", "NextDate")
2. **容錯處理**: 如果日期解析失敗，該項目會排在列表最後
3. **備用欄位**: 食品頁面在主要日期欄位無效時，會使用創建時間作為備用排序依據

### 排序順序
- **升序排列**: 使用 `OrderBy()` 確保最近日期在前
- **無效處理**: 無法解析的日期使用 `DateTime.MaxValue`，自動排在最後

## 使用者體驗改善

### 訂閱管理
- 即將到期的訂閱顯示在最前面
- 幫助使用者優先處理緊急的付款事項
- 提高訂閱管理效率

### 食品管理
- 即將過期的食品顯示在最前面
- 幫助使用者優先消費快過期的食品
- 減少食品浪費

## 相容性

### 後端服務支援
- ✅ Appwrite
- ✅ Supabase  
- ✅ Back4App
- ✅ MySQL
- ✅ Contentful

### 資料格式支援
- 支援多種日期格式的自動解析
- 支援不同的屬性命名慣例
- 向後相容現有資料結構

## 錯誤處理

### 排序失敗處理
```csharp
catch (Exception ex)
{
    System.Diagnostics.Debug.WriteLine($"排序資料時發生錯誤：{ex.Message}");
    return originalData; // 返回原始未排序資料
}
```

### 日期解析失敗
- 無效日期自動排在列表最後
- 不會影響其他有效資料的顯示
- 保持應用程式穩定運行

## 測試建議

### 功能測試
1. **正常排序**: 添加多個不同日期的項目，確認排序正確
2. **無效日期**: 測試包含無效日期的資料處理
3. **空資料**: 確認空資料集不會造成錯誤
4. **混合資料**: 測試部分有效、部分無效日期的混合情況

### 效能測試
1. **大量資料**: 測試包含大量項目時的排序效能
2. **頻繁更新**: 測試頻繁重新載入時的響應速度

## 未來改進

### 可能的增強功能
1. **排序選項**: 提供使用者選擇排序方式的選項
2. **多欄位排序**: 支援多個欄位的組合排序
3. **排序指示器**: 在 UI 中顯示當前的排序狀態
4. **記憶排序**: 記住使用者的排序偏好設定