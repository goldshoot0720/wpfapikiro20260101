# 設定檔熱重載功能

## 功能概述

實現了設定檔載入時無需重新啟動應用程式的熱重載功能。當用戶載入不同的設定檔時，所有相關的 UI 組件和後端服務會自動更新，提供無縫的使用體驗。

## 主要特點

### ✨ 即時生效
- 載入設定檔後立即生效，無需重新啟動
- 所有頁面自動更新為新的設定
- 後端服務連線即時切換
- UI 界面即時反映新設定

### 🎯 自動更新組件
- **設定頁面** - 立即顯示新的連線設定
- **食品頁面** - 重新載入對應後端的資料
- **訂閱頁面** - 重新載入對應後端的資料
- **所有使用後端服務的功能**

### ⚡ 技術實現

#### 事件驅動架構
```csharp
// 在 SettingsProfileService 中觸發事件
public static event Action? OnSettingsUpdated;

// 在 AppSettings 中轉發事件
public static event Action? SettingsChanged;

// 各頁面訂閱事件
AppSettings.SettingsChanged += OnSettingsChanged;
```

#### 線程安全更新
```csharp
private async void OnSettingsChanged()
{
    // 在 UI 線程上重新載入設定
    await Dispatcher.InvokeAsync(async () =>
    {
        await LoadData();
        ShowMessage("設定已更新");
    });
}
```

## 使用方式

### 1. 載入設定檔
1. 開啟設定檔管理視窗
2. 選擇要載入的設定檔
3. 點擊「載入設定檔」按鈕
4. 確認載入後，所有設定立即生效

### 2. 測試功能
1. 在設定頁面點擊「🔥 測試熱重載」按鈕
2. 系統會自動執行完整的測試流程
3. 查看控制台輸出了解測試結果

## 代碼修改

### 1. SettingsProfileService.cs
```csharp
// 添加設定更新事件
public static event Action? OnSettingsUpdated;

// 在 LoadProfileAsync 方法中觸發事件
OnSettingsUpdated?.Invoke();
```

### 2. AppSettings.cs
```csharp
// 添加設定變更事件
public static event Action? SettingsChanged;

// 在建構函式中訂閱事件
SettingsProfileService.OnSettingsUpdated += OnSettingsChanged;
```

### 3. 各頁面 (SettingsPage, FoodPage, SubscriptionPage)
```csharp
// 在建構函式中訂閱事件
AppSettings.SettingsChanged += OnSettingsChanged;

// 實現事件處理方法
private async void OnSettingsChanged()
{
    await Dispatcher.InvokeAsync(async () =>
    {
        await LoadData();
        ShowMessage("設定已更新");
    });
}
```

### 4. SettingsProfileWindow.xaml.cs
```csharp
// 更新載入成功訊息
MessageBox.Show("設定檔載入成功！所有設定已即時更新，無需重新啟動應用程式。", 
    "成功", MessageBoxButton.OK, MessageBoxImage.Information);
```

## 測試驗證

### TestHotReloadSettings.cs
提供完整的測試功能：
1. 創建測試設定檔
2. 修改當前設定
3. 載入原始設定檔
4. 驗證設定是否正確恢復
5. 清理測試資料

### 測試步驟
1. 點擊設定頁面的「🔥 測試熱重載」按鈕
2. 觀察控制台輸出
3. 確認所有測試步驟都成功執行
4. 驗證 UI 是否正確更新

## 優勢

### 🚀 提升用戶體驗
- 無需重新啟動應用程式
- 設定切換更加流暢
- 減少等待時間

### 🔧 開發友好
- 便於測試不同後端服務
- 快速切換開發環境
- 提高開發效率

### 🛡️ 穩定可靠
- 事件驅動確保一致性
- 線程安全的 UI 更新
- 完整的錯誤處理

## 注意事項

1. **事件訂閱** - 確保所有需要更新的組件都正確訂閱了事件
2. **線程安全** - UI 更新必須在 UI 線程上執行
3. **資源清理** - 適當時候取消事件訂閱以避免記憶體洩漏
4. **錯誤處理** - 載入設定檔時的異常處理

## 未來擴展

1. **更多組件支援** - 可以為更多頁面添加熱重載支援
2. **設定驗證** - 載入前驗證設定檔的有效性
3. **回滾機制** - 載入失敗時自動回滾到之前的設定
4. **設定比較** - 顯示設定檔之間的差異

這個功能大幅提升了應用程式的使用體驗，讓設定管理變得更加便捷和高效！