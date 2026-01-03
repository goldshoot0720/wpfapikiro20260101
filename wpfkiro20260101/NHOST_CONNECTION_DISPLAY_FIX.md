# NHost 連線設定顯示修正

## 問題描述

從用戶提供的截圖可以看到，NHost 連線設定顯示存在以下問題：

1. **欄位標籤正確**: `NHOST_GRAPHQL_URL` 和 `NHOST_ADMIN_SECRET` ✅
2. **預設值錯誤**: 顯示 `https://your-project.nhost.run` 和 `your-project-id` ❌

## 問題原因

雖然 AppSettings 中的預設值是正確的，但條件判斷不夠強制，導致舊的預設值沒有被更新。

## 修正方案

### 1. 強化預設值更新條件

在 `SettingsPage.xaml.cs` 中修改 NHost 的預設值設定邏輯：

```csharp
case BackendServiceType.NHost:
    // NHost 強制使用正確的預設值
    if (string.IsNullOrWhiteSpace(ApiUrlTextBox.Text) || 
        IsDefaultUrl(ApiUrlTextBox.Text) ||
        ApiUrlTextBox.Text.Contains("your-project") ||
        !ApiUrlTextBox.Text.Contains("uxgwdiuehabbzenwtcqo"))
    {
        ApiUrlTextBox.Text = AppSettings.Defaults.NHost.ApiUrl;
    }
    if (string.IsNullOrWhiteSpace(ProjectIdTextBox.Text) ||
        IsDefaultProjectId(ProjectIdTextBox.Text) ||
        ProjectIdTextBox.Text.Contains("your-project") ||
        ProjectIdTextBox.Text != "cu#34&yjF3Cr%fgxB#WA,4r4^c=Igcwr")
    {
        ProjectIdTextBox.Text = AppSettings.Defaults.NHost.ProjectId;
    }
    break;
```

### 2. 創建強制更新工具

創建 `ForceNHostSettingsUpdate.cs` 來強制更新設定：

```csharp
// 執行完整更新
await ForceNHostSettingsUpdate.RunUpdate();

// 快速修正
ForceNHostSettingsUpdate.QuickFix();
```

## 修正後的顯示效果

當用戶選擇 NHost 時，應該顯示：

### 欄位標籤 ✅
- `NHOST_GRAPHQL_URL:`
- `NHOST_ADMIN_SECRET:`

### 預設值 ✅
- `https://uxgwdiuehabbzenwtcqo.hasura.eu-central-1.nhost.run/v1/graphql`
- `cu#34&yjF3Cr%fgxB#WA,4r4^c=Igcwr`

### 隱藏欄位 ✅
- API Key 欄位被隱藏
- 所有 Appwrite 專用欄位被隱藏

## 使用步驟

### 方法 1: 自動修正
1. 執行 `ForceNHostSettingsUpdate.QuickFix()`
2. 重新開啟系統設定頁面
3. 選擇 NHost 查看結果

### 方法 2: 手動驗證
1. 執行 `ForceNHostSettingsUpdate.RunUpdate()`
2. 查看詳細的更新報告
3. 按照說明重新開啟設定頁面

## 驗證方法

### 檢查 AppSettings
```csharp
var settings = AppSettings.Instance;
Console.WriteLine($"NHost API URL: {settings.NHost.ApiUrl}");
Console.WriteLine($"NHost Project ID: {settings.NHost.ProjectId}");
```

### 檢查 Defaults
```csharp
Console.WriteLine($"Default API URL: {AppSettings.Defaults.NHost.ApiUrl}");
Console.WriteLine($"Default Project ID: {AppSettings.Defaults.NHost.ProjectId}");
```

## 預期結果

修正後，用戶在系統設定頁面選擇 NHost 時將看到：

```
連線設定
▼

NHOST_GRAPHQL_URL:    [https://uxgwdiuehabbzenwtcqo.hasura.eu-central-1.nhost.run/v1/graphql]
NHOST_ADMIN_SECRET:   [cu#34&yjF3Cr%fgxB#WA,4r4^c=Igcwr]
```

## 技術細節

### 更新條件
- 檢查是否包含 `your-project` (舊預設值)
- 檢查是否包含正確的 subdomain `uxgwdiuehabbzenwtcqo`
- 檢查是否完全匹配正確的 Admin Secret

### 強制更新
- 直接修改 AppSettings 實例
- 儲存到檔案
- 重新載入設定
- 驗證更新結果

## 總結

這個修正確保了：

- ✅ NHost 連線設定顯示正確的欄位標籤
- ✅ 自動填入正確的預設值
- ✅ 隱藏不需要的欄位
- ✅ 提供強制更新工具
- ✅ 完整的驗證機制

用戶現在可以正確地看到和使用 NHost 的兩欄位設定。