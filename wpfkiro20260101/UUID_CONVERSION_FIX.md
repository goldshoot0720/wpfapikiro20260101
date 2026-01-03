# UUID 轉換修正 - Appwrite CSV 轉 Supabase CSV

## 問題描述
用戶在使用 Appwrite CSV 轉換成 Supabase CSV 功能時遇到錯誤：
```
Failed to import data: Failed to run sql query: ERROR: 22P02: invalid input syntax for type uuid: "6957e395b806a8a1af04"
```

## 根本原因
- **Appwrite ID 格式**: 使用較短的字符串 ID（如 `6957e395b806a8a1af04`）
- **Supabase UUID 要求**: 需要標準的 UUID 格式（如 `12345678-1234-1234-1234-123456789012`）
- **轉換問題**: 原始代碼直接複製 Appwrite ID 到 Supabase CSV，沒有進行格式轉換

## 解決方案

### 1. 新增 UUID 轉換函數
```csharp
private string ConvertToUuid(string appwriteId)
{
    try
    {
        // 移除可能的引號和空白
        appwriteId = appwriteId.Trim().Trim('"');
        
        // 如果已經是 UUID 格式，直接返回
        if (Guid.TryParse(appwriteId, out _))
        {
            return appwriteId;
        }
        
        // 如果 Appwrite ID 長度不足，用零填充到 32 個字符
        if (appwriteId.Length < 32)
        {
            appwriteId = appwriteId.PadRight(32, '0');
        }
        else if (appwriteId.Length > 32)
        {
            // 如果太長，截取前 32 個字符
            appwriteId = appwriteId.Substring(0, 32);
        }
        
        // 將 32 個字符的字符串轉換為 UUID 格式 (8-4-4-4-12)
        var uuid = $"{appwriteId.Substring(0, 8)}-{appwriteId.Substring(8, 4)}-{appwriteId.Substring(12, 4)}-{appwriteId.Substring(16, 4)}-{appwriteId.Substring(20, 12)}";
        
        // 驗證生成的 UUID 是否有效
        if (Guid.TryParse(uuid, out _))
        {
            return uuid;
        }
        else
        {
            // 如果轉換失敗，生成一個新的 UUID
            return Guid.NewGuid().ToString();
        }
    }
    catch
    {
        // 如果任何步驟失敗，生成一個新的 UUID
        return Guid.NewGuid().ToString();
    }
}
```

### 2. 更新轉換邏輯
修改了以下方法以使用 UUID 轉換：

#### ConvertDataLine 方法
```csharp
// 修改前
var id = CleanField(fields[0]);

// 修改後
var appwriteId = CleanField(fields[0]);
var id = ConvertToUuid(appwriteId); // 轉換為 UUID 格式
```

#### GenerateFoodCsv 方法
```csharp
// 修改前
csv.AppendLine($"{EscapeCsvField(id)},{createdAtFormatted},...");

// 修改後
var supabaseId = ConvertToUuid(id); // 轉換為 UUID 格式
csv.AppendLine($"{EscapeCsvField(supabaseId)},{createdAtFormatted},...");
```

#### GenerateSubscriptionCsv 方法
```csharp
// 修改前
csv.AppendLine($"{EscapeCsvField(id)},{createdAtFormatted},...");

// 修改後
var supabaseId = ConvertToUuid(id); // 轉換為 UUID 格式
csv.AppendLine($"{EscapeCsvField(supabaseId)},{createdAtFormatted},...");
```

## 轉換邏輯說明

### UUID 轉換規則
1. **已是 UUID 格式**: 直接返回原值
2. **短於 32 字符**: 用 '0' 填充到 32 字符
3. **長於 32 字符**: 截取前 32 字符
4. **格式化**: 轉換為標準 UUID 格式 (8-4-4-4-12)
5. **驗證**: 確保生成的 UUID 有效
6. **備用方案**: 如果轉換失敗，生成新的 UUID

### 轉換示例
```
原始 Appwrite ID: "6957e395b806a8a1af04"
填充後 32 字符: "6957e395b806a8a1af04000000000000"
UUID 格式: "6957e395-b806-a8a1-af04-000000000000"
```

## 測試驗證
創建了 `TestUuidConversion.cs` 測試文件，包含：
- 各種 ID 格式的轉換測試
- CSV 轉換完整流程測試
- UUID 有效性驗證

## 影響範圍
- ✅ 單檔案轉換功能
- ✅ 批次資料夾轉換功能
- ✅ 直接 CSV 生成功能（Food 和 Subscription）
- ✅ 測試轉換功能

## 使用方式
1. 在系統設定頁面選擇「資料轉換」功能
2. 選擇 Appwrite CSV 檔案進行轉換
3. 轉換後的 CSV 將包含有效的 UUID 格式 ID
4. 可直接匯入到 Supabase 而不會出現 UUID 格式錯誤

## 注意事項
- 轉換是單向的（Appwrite → Supabase）
- 轉換後的 UUID 是基於原始 ID 生成的，具有一定的一致性
- 如果原始 ID 無法轉換，會生成新的隨機 UUID
- 建議在轉換前備份原始資料

## 狀態
✅ **已修正** - UUID 轉換功能已實現並測試通過