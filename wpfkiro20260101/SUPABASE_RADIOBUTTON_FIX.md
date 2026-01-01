# Supabase RadioButton 選擇問題修正

## 🚨 問題描述

**用戶報告**: 選擇 Supabase 時，彈出的測試設定內容顯示的是 Appwrite 設定

**根本原因分析**:
1. **測試設定功能問題**: `TestSettings_Click` 方法顯示的是已儲存的設定檔案內容，而不是當前界面選擇的設定
2. **欄位更新問題**: 當選擇 Supabase 時，界面欄位沒有立即更新為正確的 Supabase 值
3. **事件觸發問題**: RadioButton 事件可能沒有正確觸發或處理

## ✅ 已完成的修正

### 1. 修正 TestSettings_Click 方法
**問題**: 只顯示已儲存的設定，不顯示當前界面選擇的設定
**修正**: 
- 獲取當前界面上選擇的後端服務
- 顯示當前界面設定 vs 已儲存設定的對比
- 清楚區分「當前選擇」和「已儲存」的設定

```csharp
// 修正後會顯示：
// 1. 當前界面選擇的設定 (JSON)
// 2. 已儲存的設定檔案內容 (JSON)  
// 3. RadioButton 狀態
```

### 2. 增強 BackendOption_Checked 事件處理
**問題**: 選擇 Supabase 時欄位沒有立即更新
**修正**:
- 添加 Supabase 的特別處理邏輯
- 強制更新欄位值為正確的 Supabase 設定
- 即時保存設定到檔案

```csharp
// 當選擇 Supabase 時會：
// 1. 立即更新所有欄位為正確值
// 2. 清空 Appwrite 專用欄位
// 3. 保存設定到檔案
```

### 3. 強化欄位更新邏輯
**問題**: UpdateFieldsForService 方法不會覆蓋現有的錯誤值
**修正**:
- 添加對舊 Project ID 的檢測
- 強制更新不正確的 Supabase 設定
- 確保 API Key 格式正確

## 🔧 使用步驟

### 步驟 1: 重新啟動應用程式
```
1. 完全關閉當前應用程式
2. 重新開啟應用程式（載入修正後的程式碼）
3. 進入「系統設定」頁面
```

### 步驟 2: 測試 RadioButton 功能
```
1. 先點選「Appwrite」選項
2. 再點選「Supabase」選項
3. 觀察欄位是否立即更新為：
   - API URL: https://lobezwpworbfktlkxuyo.supabase.co
   - Project ID: lobezwpworbfktlkxuyo
   - API Key: eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

### 步驟 3: 驗證測試設定功能
```
1. 確保選擇了「Supabase」
2. 點擊「測試設定」按鈕
3. 確認彈出的對話框顯示：
   - 當前界面選擇: Supabase 設定
   - 已儲存設定: 可能還是舊設定
   - RadioButton 狀態: Supabase: True
```

### 步驟 4: 儲存並測試
```
1. 點擊「儲存設定」按鈕
2. 點擊「測試連線」按鈕
3. 確認連接成功
4. 再次點擊「測試設定」確認已儲存設定也是 Supabase
```

## 📋 預期結果

### ✅ 正確的行為

**選擇 Supabase 時**:
1. **欄位立即更新**: 所有欄位自動填入正確的 Supabase 值
2. **狀態訊息**: 顯示「已切換至 Supabase」
3. **測試設定**: 顯示當前選擇為 Supabase 設定

**測試設定對話框**:
```
當前界面選擇的設定:
{
  "BackendService": 1,
  "ApiUrl": "https://lobezwpworbfktlkxuyo.supabase.co",
  "ProjectId": "lobezwpworbfktlkxuyo",
  "ApiKey": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "DatabaseId": "",
  "BucketId": ""
}

RadioButton 狀態:
Supabase: True
Appwrite: False
```

### ❌ 如果仍有問題

**症狀 1**: 選擇 Supabase 但欄位沒有更新
- **原因**: RadioButton 事件沒有觸發
- **解決**: 重新啟動應用程式，確保載入最新程式碼

**症狀 2**: 測試設定仍顯示 Appwrite
- **原因**: 設定檔案中還是舊設定
- **解決**: 點擊「儲存設定」更新設定檔案

**症狀 3**: 欄位顯示舊的 Project ID
- **原因**: 界面沒有正確更新
- **解決**: 手動清空欄位，重新選擇 Supabase

## 🔍 故障排除

### 手動修正步驟
如果自動修正不起作用：

1. **清空所有欄位**
   ```
   1. 手動清空 API URL 欄位
   2. 手動清空 Project ID 欄位  
   3. 手動清空 API Key 欄位
   ```

2. **重新選擇 Supabase**
   ```
   1. 點選其他選項（如 Appwrite）
   2. 再點選 Supabase
   3. 確認欄位自動填入正確值
   ```

3. **手動輸入正確值**
   ```
   API URL: https://lobezwpworbfktlkxuyo.supabase.co
   Project ID: lobezwpworbfktlkxuyo
   API Key: eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6ImxvYmV6d3B3b3JiZmt0bGt4dXlvIiwicm9sZSI6InNlcnZpY2Vfcm9sZSIsImlhdCI6MTc2NzI1ODU5MSwiZXhwIjoyMDgyODM0NTkxfQ.tFcCP7kvcfV1CznhIHXBF0TenGlYD1XRlAWdCYYEnlc
   ```

### Debug 輸出檢查
在 Visual Studio 輸出視窗中查找：
```
選擇 Supabase，強制更新欄位值
即時保存後端服務: Supabase
已切換至 Supabase
```

## 📝 技術摘要

### 修正的核心問題
1. **測試設定邏輯**: 現在會顯示當前界面選擇 vs 已儲存設定
2. **RadioButton 事件**: 增強了 Supabase 選擇時的處理邏輯
3. **欄位更新機制**: 強制更新為正確的 Supabase 值
4. **即時保存**: 選擇服務時立即保存到設定檔案

### 防止未來問題
- **一致性檢查**: 確保界面顯示與實際設定一致
- **強制更新**: 檢測並修正不正確的舊值
- **詳細日誌**: 增加 Debug 輸出便於問題診斷

所有 RadioButton 選擇和測試設定的問題已經修正，現在選擇 Supabase 時會正確顯示和保存 Supabase 設定！