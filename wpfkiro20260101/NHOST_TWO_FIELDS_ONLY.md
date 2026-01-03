# NHost 兩欄位設定完成

## 用戶需求

用戶明確指出 NHost 只需要兩個欄位：
1. **NHOST_GRAPHQL_URL** → `https://uxgwdiuehabbzenwtcqo.hasura.eu-central-1.nhost.run/v1/graphql`
2. **NHOST_ADMIN_SECRET** → `cu#34&yjF3Cr%fgxB#WA,4r4^c=Igcwr`

## 實作修正

### 1. 更新欄位標籤 (SettingsPage.xaml.cs)

```csharp
case BackendServiceType.NHost:
    ApiUrlLabel.Text = "NHOST_GRAPHQL_URL:";
    ProjectIdLabel.Text = "NHOST_ADMIN_SECRET:";
    // 隱藏 API Key 欄位（NHost 只需要兩個欄位）
    ApiKeyLabel.Visibility = System.Windows.Visibility.Collapsed;
    ApiKeyPasswordBox.Visibility = System.Windows.Visibility.Collapsed;
    break;
```

### 2. 添加 API Key 標籤名稱 (SettingsPage.xaml)

```xml
<TextBlock Grid.Row="6" Grid.Column="0" 
           x:Name="ApiKeyLabel"
           Text="API Key:" 
           FontWeight="Bold" 
           Margin="0,0,15,10" 
           VerticalAlignment="Center"/>
```

### 3. 更新所有其他服務顯示 API Key 欄位

確保除了 NHost 之外的所有服務都正確顯示 API Key 欄位：
- Appwrite ✅
- Supabase ✅
- Contentful ✅
- Back4App ✅
- MySQL ✅
- Strapi ✅
- Sanity ✅

### 4. 移除 NHost 的 API Key 處理

```csharp
case BackendServiceType.NHost:
    // 只處理兩個欄位
    if (string.IsNullOrWhiteSpace(ApiUrlTextBox.Text))
        ApiUrlTextBox.Text = AppSettings.Defaults.NHost.ApiUrl;
    if (string.IsNullOrWhiteSpace(ProjectIdTextBox.Text))
        ProjectIdTextBox.Text = AppSettings.Defaults.NHost.ProjectId;
    // 不處理 API Key
    break;
```

## 顯示效果

當用戶在系統設定頁面選擇 NHost 時：

### 顯示的欄位
1. **NHOST_GRAPHQL_URL:** `https://uxgwdiuehabbzenwtcqo.hasura.eu-central-1.nhost.run/v1/graphql`
2. **NHOST_ADMIN_SECRET:** `cu#34&yjF3Cr%fgxB#WA,4r4^c=Igcwr`

### 隱藏的欄位
- API Key (第三個欄位)
- Database ID (Appwrite 專用)
- Bucket ID (Appwrite 專用)
- Food Table ID (Appwrite 專用)
- Subscription Table ID (Appwrite 專用)

## 優點

### 1. 簡潔性
- 只顯示必要的兩個欄位
- 清楚的欄位命名 (NHOST_GRAPHQL_URL, NHOST_ADMIN_SECRET)
- 隱藏不相關的欄位

### 2. 用戶體驗
- 減少混淆，只顯示需要的欄位
- 自動填入正確的預設值
- 與其他服務保持一致的操作方式

### 3. 維護性
- 清楚的程式碼結構
- 每個服務都有明確的欄位顯示邏輯
- 容易理解和修改

## 測試驗證

### 可用測試檔案
1. **TestNHostSettingsDisplay.cs** - 完整的設定顯示測試
2. **TestNHostSimplified.cs** - 簡化設定測試

### 測試執行
```csharp
// 執行完整測試
await TestNHostSettingsDisplay.RunTest();

// 快速檢查
TestNHostSettingsDisplay.QuickCheck();
```

## 與服務實作的一致性

這個兩欄位設定與 NHostService.cs 的實作完全一致：

```csharp
public class NHostService : IBackendService
{
    // 只需要這兩個值
    private readonly string _graphqlUrl = "https://uxgwdiuehabbzenwtcqo.hasura.eu-central-1.nhost.run/v1/graphql";
    private readonly string _adminSecret = "cu#34&yjF3Cr%fgxB#WA,4r4^c=Igcwr";
    
    // 所有操作都使用這兩個值
}
```

## 總結

NHost 現在完全符合用戶需求：

- ✅ 只顯示兩個必要欄位
- ✅ 使用清楚的欄位命名
- ✅ 自動填入正確的預設值
- ✅ 隱藏不相關的欄位
- ✅ 與服務實作保持一致
- ✅ 提供良好的用戶體驗

用戶在選擇 NHost 時，將看到最簡潔、最清楚的設定界面，只需要關注這兩個核心欄位。