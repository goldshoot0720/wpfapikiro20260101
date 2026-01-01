# Appwrite 中文日期時間轉換修正

## 問題描述
在中文系統環境下，日期時間格式導致 Appwrite 出現以下錯誤：
```
Invalid document structure: Attribute "nextdate" has invalid type. Value must be valid date between 1000-01-01 00:00:00 and 9999-12-31 23:59:59.
```

## 根本原因
1. **UI 組件問題**: DatePicker 控制項返回的 DateTime 包含本地時區資訊
2. **CSV 匯出問題**: 日期時間可能以中文格式輸出
3. **Appwrite 要求**: 需要標準的 UTC 時間格式

## 修正的檔案

### 1. AppwriteService.cs
**修正內容**: 確保傳送給 Appwrite 的日期時間為 UTC 格式
```csharp
// 修正前
data["nextdate"] = subscription.NextDate;

// 修正後  
data["nextdate"] = subscription.NextDate.ToUniversalTime();
```

### 2. AddSubscriptionWindow.xaml.cs
**修正內容**: 將 DatePicker 的值明確設定為 UTC 時間
```csharp
// 修正前
NextDate = NextPaymentDatePicker.SelectedDate.Value,

// 修正後
NextDate = DateTime.SpecifyKind(NextPaymentDatePicker.SelectedDate.Value, DateTimeKind.Utc),
```

### 3. EditSubscriptionWindow.xaml.cs
**修正內容**: 同樣將編輯時的日期設定為 UTC 時間
```csharp
// 修正前
NextDate = NextPaymentDatePicker.SelectedDate.Value,

// 修正後
NextDate = DateTime.SpecifyKind(NextPaymentDatePicker.SelectedDate.Value, DateTimeKind.Utc),
```

### 4. SettingsPage.xaml.cs
**新增功能**: 添加日期格式化方法，確保 CSV 匯出使用英文格式

**新增方法**:
```csharp
private string FormatDateForCsv(string dateValue)
{
    if (string.IsNullOrEmpty(dateValue))
        return "";

    try
    {
        if (DateTime.TryParse(dateValue, out DateTime parsedDate))
        {
            return parsedDate.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffZ", System.Globalization.CultureInfo.InvariantCulture);
        }
        return dateValue;
    }
    catch
    {
        return dateValue;
    }
}
```

**修正的方法**:
- `GenerateFoodCsv()`: 使用 `FormatDateForCsv()` 處理日期欄位
- `GenerateSubscriptionCsv()`: 使用 `FormatDateForCsv()` 處理日期欄位

## 技術細節

### 時區處理策略
1. **UI 層**: 使用 `DateTime.SpecifyKind()` 明確指定為 UTC
2. **服務層**: 使用 `ToUniversalTime()` 確保轉換為 UTC
3. **CSV 匯出**: 使用 `CultureInfo.InvariantCulture` 確保英文格式

### 日期格式標準化
- **內部處理**: 統一使用 UTC DateTime 物件
- **Appwrite 傳輸**: 自動序列化為 ISO 8601 格式
- **CSV 匯出**: 明確使用 `yyyy-MM-ddTHH:mm:ss.fffZ` 格式

## 測試建議

### 1. 訂閱功能測試
- 在中文系統環境下新增訂閱
- 編輯現有訂閱的日期
- 確認 Appwrite 不再出現日期格式錯誤

### 2. CSV 匯出測試
- 匯出包含中文日期的資料
- 檢查 CSV 檔案中的日期格式是否為英文
- 確認日期時間格式一致性

### 3. 跨時區測試
- 在不同時區設定下測試
- 確認日期時間正確轉換為 UTC

## 相關檔案
- `wpfkiro20260101/Services/AppwriteService.cs`
- `wpfkiro20260101/AddSubscriptionWindow.xaml.cs`
- `wpfkiro20260101/EditSubscriptionWindow.xaml.cs`
- `wpfkiro20260101/SettingsPage.xaml.cs`