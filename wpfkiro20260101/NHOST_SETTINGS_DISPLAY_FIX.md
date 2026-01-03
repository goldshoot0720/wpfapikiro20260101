# NHost 連線設定顯示修正

## 問題描述

用戶反映 NHost 連線設定在系統設定頁面中顯示不正確，選擇 NHost 時沒有顯示適當的連線設定欄位。

## 問題原因

1. **UpdateFieldsForService 方法缺少 NHost 處理**: 在 `SettingsPage.xaml.cs` 中的 `UpdateFieldsForService` 方法沒有為 `BackendServiceType.NHost` 添加專門的 case 處理。

2. **AppSettings 中 NHost 預設值不正確**: `AppSettings.cs` 中的 NHost 預設值使用了通用的佔位符，而不是實際的連線資訊。

3. **欄位標籤不適合 NHost**: NHost 使用 GraphQL URL 和 Admin Secret，但標籤顯示為通用的 "API URL" 和 "Project ID"。

## 修正內容

### 1. 更新 SettingsPage.xaml.cs

在 `UpdateFieldsForService` 方法中添加 NHost 的專門處理：

```csharp
case BackendServiceType.NHost:
    ApiUrlLabel.Text = "GraphQL URL:";
    ProjectIdLabel.Text = "Admin Secret:";
    // 隱藏 Appwrite 專用欄位
    DatabaseIdLabel.Visibility = System.Windows.Visibility.Collapsed;
    DatabaseIdTextBox.Visibility = System.Windows.Visibility.Collapsed;
    BucketIdLabel.Visibility = System.Windows.Visibility.Collapsed;
    BucketIdTextBox.Visibility = System.Windows.Visibility.Collapsed;
    FoodCollectionIdLabel.Visibility = System.Windows.Visibility.Collapsed;
    FoodCollectionIdTextBox.Visibility = System.Windows.Visibility.Collapsed;
    SubscriptionCollectionIdLabel.Visibility = System.Windows.Visibility.Collapsed;
    SubscriptionCollectionIdTextBox.Visibility = System.Windows.Visibility.Collapsed;
    break;
```

並在預設值設定部分添加：

```csharp
case BackendServiceType.NHost:
    if (string.IsNullOrWhiteSpace(ApiUrlTextBox.Text) || 
        IsDefaultUrl(ApiUrlTextBox.Text))
    {
        ApiUrlTextBox.Text = AppSettings.Defaults.NHost.ApiUrl;
    }
    if (string.IsNullOrWhiteSpace(ProjectIdTextBox.Text) ||
        IsDefaultProjectId(ProjectIdTextBox.Text))
    {
        ProjectIdTextBox.Text = AppSettings.Defaults.NHost.ProjectId;
    }
    if (string.IsNullOrWhiteSpace(ApiKeyPasswordBox.Password))
    {
        ApiKeyPasswordBox.Password = AppSettings.Defaults.NHost.ApiKey;
    }
    break;
```

### 2. 更新 AppSettings.cs

更新 NHost 的預設值為實際的連線資訊：

```csharp
public class NHostSettings : IServiceSettings
{
    public string ApiUrl { get; set; } = "https://uxgwdiuehabbzenwtcqo.hasura.eu-central-1.nhost.run/v1/graphql";
    public string ProjectId { get; set; } = "cu#34&yjF3Cr%fgxB#WA,4r4^c=Igcwr";
    public string ApiKey { get; set; } = "cu#34&yjF3Cr%fgxB#WA,4r4^c=Igcwr";
}
```

```csharp
public static class NHost
{
    public const string ApiUrl = "https://uxgwdiuehabbzenwtcqo.hasura.eu-central-1.nhost.run/v1/graphql";
    public const string ProjectId = "cu#34&yjF3Cr%fgxB#WA,4r4^c=Igcwr";
    public const string ApiKey = "cu#34&yjF3Cr%fgxB#WA,4r4^c=Igcwr";
}
```

## 修正後的顯示效果

當用戶在系統設定頁面選擇 NHost 時，現在會正確顯示：

### 欄位標籤
- **第一個欄位**: "GraphQL URL:" (而不是 "API URL:")
- **第二個欄位**: "Admin Secret:" (而不是 "Project ID:")
- **第三個欄位**: "API Key:" (保持不變)

### 預設值
- **GraphQL URL**: `https://uxgwdiuehabbzenwtcqo.hasura.eu-central-1.nhost.run/v1/graphql`
- **Admin Secret**: `cu#34&yjF3Cr%fgxB#WA,4r4^c=Igcwr`
- **API Key**: `cu#34&yjF3Cr%fgxB#WA,4r4^c=Igcwr`

### 隱藏欄位
- Database ID (Appwrite 專用)
- Bucket ID (Appwrite 專用)
- Food Table ID (Appwrite 專用)
- Subscription Table ID (Appwrite 專用)

## 測試驗證

創建了 `TestNHostSettingsDisplay.cs` 來驗證修正效果：

```csharp
// 執行完整測試
await TestNHostSettingsDisplay.RunTest();

// 快速檢查
TestNHostSettingsDisplay.QuickCheck();
```

## 與簡化設定的一致性

這個修正與之前的 NHost 連線設定簡化完全一致：

1. **GraphQL URL**: 直接使用完整的 Hasura GraphQL 端點
2. **Admin Secret**: 使用實際的管理員密鑰
3. **簡化欄位**: 只顯示必要的連線設定欄位

## 用戶體驗改善

修正後，用戶在選擇 NHost 時會看到：

1. **正確的欄位標籤**: 清楚標示這是 GraphQL URL 和 Admin Secret
2. **自動填入的正確值**: 無需手動輸入複雜的連線資訊
3. **簡潔的界面**: 隱藏不相關的 Appwrite 專用欄位
4. **一致的體驗**: 與其他後端服務的設定方式保持一致

## 總結

這個修正解決了 NHost 連線設定顯示的問題，確保：

- ✅ 欄位標籤正確顯示
- ✅ 預設值自動填入
- ✅ 不相關欄位被隱藏
- ✅ 與簡化設定保持一致
- ✅ 提供良好的用戶體驗

用戶現在可以正確地在系統設定中配置和使用 NHost 服務。