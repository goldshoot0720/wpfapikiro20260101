# 連線設定未儲存問題修復

## 問題描述

用戶反映連線設定無法正確儲存，可能的原因包括：
1. 儲存順序問題：在更新後端服務類型之前就呼叫了 SaveCurrentServiceSettings
2. 設定載入/儲存邏輯不一致
3. 檔案權限或路徑問題

## 已修復的問題

### 1. SaveSettings_Click 方法中的儲存順序問題

**問題**：原本的程式碼先呼叫 `SaveCurrentServiceSettings()`，然後才更新 `settings.BackendService`，導致設定被儲存到錯誤的服務中。

**修復前**：
```csharp
// 儲存當前選擇服務的獨立設定
SaveCurrentServiceSettings();

// 儲存後端服務選擇
if (AppwriteOption.IsChecked == true)
    settings.BackendService = BackendServiceType.Appwrite;
// ...
```

**修復後**：
```csharp
// 先確定要儲存到哪個後端服務
BackendServiceType selectedService = BackendServiceType.Appwrite;

if (AppwriteOption.IsChecked == true)
    selectedService = BackendServiceType.Appwrite;
// ...

// 更新後端服務選擇
settings.BackendService = selectedService;

// 儲存當前選擇服務的獨立設定（現在會儲存到正確的服務）
SaveCurrentServiceSettings();
```

## 診斷工具

創建了 `DiagnoseConnectionSettingsSave.cs` 來幫助診斷設定儲存問題：

### 主要功能
1. **檢查設定檔案**：驗證檔案路徑、存在性、大小和修改時間
2. **顯示當前設定**：展示所有服務的獨立設定狀態
3. **測試儲存功能**：實際測試設定的儲存和載入
4. **服務切換測試**：測試在不同服務間切換時的設定保存

### 使用方式
```csharp
// 執行完整診斷
DiagnoseConnectionSettingsSave.RunDiagnosis();

// 測試服務切換
DiagnoseConnectionSettingsSave.TestServiceSwitching();
```

## 驗證步驟

1. **編譯項目**：確保修復的程式碼能正確編譯
2. **執行診斷**：使用診斷工具檢查當前狀態
3. **測試儲存**：
   - 選擇不同的後端服務
   - 修改連線設定
   - 點擊「儲存設定」
   - 重新載入頁面驗證設定是否保存
4. **測試服務切換**：
   - 在不同服務間切換
   - 驗證每個服務的獨立設定是否正確保存和載入

## 相關檔案

- `SettingsPage.xaml.cs` - 主要修復檔案
- `AppSettings.cs` - 設定管理類別
- `DiagnoseConnectionSettingsSave.cs` - 診斷工具

## 預期結果

修復後應該能夠：
1. ✅ 正確儲存連線設定到選擇的後端服務
2. ✅ 在服務切換時保持各服務的獨立設定
3. ✅ 重新載入頁面後設定仍然存在
4. ✅ 設定檔案正確更新並持久化

## 後續改進

可以考慮的進一步改進：
1. 添加設定驗證邏輯
2. 實現設定備份和恢復功能
3. 添加設定匯入/匯出功能
4. 改善錯誤處理和用戶反饋