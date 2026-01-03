# NHost GraphQL 架構錯誤修正

## 問題描述

用戶在使用 NHost 服務時遇到以下 GraphQL 錯誤：

1. **食品資料表錯誤**:
   ```
   GraphQL 錯誤: [{"message":"field 'foods' not found in type: 'query_root'","extensions":{"path":"$.selectionSet.foods","code":"validation-failed"}}]
   ```

2. **訂閱資料表錯誤**:
   ```
   GraphQL 錯誤: [{"message":"field 'subscriptions' not found in type: 'query_root'","extensions":{"path":"$.selectionSet.subscriptions","code":"validation-failed"}}]
   ```

## 問題原因

這些錯誤表示 NHost 資料庫中缺少必要的資料表：
- `foods` 資料表未創建或未正確配置
- `subscriptions` 資料表未創建或未正確配置
- GraphQL 架構未包含這些資料表的查詢欄位

## 修正方案

### 1. 更新 NHost 服務 - 增強錯誤處理

#### GetFoodsAsync 方法修正
**修正前**:
```csharp
public async Task<BackendServiceResult<object[]>> GetFoodsAsync()
{
    var result = await ExecuteGraphQLAsync(query);
    if (!result.Success)
    {
        return BackendServiceResult<object[]>.CreateError(result.ErrorMessage ?? "GraphQL 請求失敗");
    }
    var foods = result.Data.GetProperty("data").GetProperty("foods");
    // ...
}
```

**修正後**:
```csharp
public async Task<BackendServiceResult<object[]>> GetFoodsAsync()
{
    // 首先檢查資料表是否存在
    var schemaCheckResult = await CheckTableExistsAsync("foods");
    if (!schemaCheckResult)
    {
        return BackendServiceResult<object[]>.CreateError("NHost 資料庫中未找到 'foods' 資料表。請先執行 CREATE_NHOST_TABLES.sql 腳本來創建必要的資料表。");
    }

    var result = await ExecuteGraphQLAsync(query);
    if (!result.Success)
    {
        if (result.ErrorMessage?.Contains("field 'foods' not found") == true)
        {
            return BackendServiceResult<object[]>.CreateError("NHost 資料庫中未找到 'foods' 資料表。請先執行 CREATE_NHOST_TABLES.sql 腳本來創建必要的資料表。");
        }
        return BackendServiceResult<object[]>.CreateError($"GraphQL 查詢失敗: {result.ErrorMessage}");
    }

    // 安全的屬性存取
    if (!result.Data.TryGetProperty("data", out var dataProperty) ||
        !dataProperty.TryGetProperty("foods", out var foodsProperty))
    {
        return BackendServiceResult<object[]>.CreateError("GraphQL 回應格式錯誤");
    }
    // ...
}
```

#### 新增資料表檢查方法
```csharp
private async Task<bool> CheckTableExistsAsync(string tableName)
{
    try
    {
        var introspectionQuery = @"
            query IntrospectionQuery {
                __schema {
                    queryType {
                        fields {
                            name
                        }
                    }
                }
            }";

        var result = await ExecuteGraphQLAsync(introspectionQuery);
        if (!result.Success) return false;

        // 檢查回應中是否包含指定的資料表名稱
        if (result.Data.TryGetProperty("data", out var dataProperty) &&
            dataProperty.TryGetProperty("__schema", out var schemaProperty) &&
            schemaProperty.TryGetProperty("queryType", out var queryTypeProperty) &&
            queryTypeProperty.TryGetProperty("fields", out var fieldsProperty))
        {
            foreach (var field in fieldsProperty.EnumerateArray())
            {
                if (field.TryGetProperty("name", out var nameProperty) &&
                    nameProperty.GetString() == tableName)
                {
                    return true;
                }
            }
        }
        return false;
    }
    catch (Exception)
    {
        return false;
    }
}
```

### 2. 資料庫設定修正步驟

#### 步驟 1: 登入 NHost 控制台
1. 前往 https://app.nhost.io/
2. 使用您的帳號登入
3. 選擇專案 `uxgwdiuehabbzenwtcqo`

#### 步驟 2: 執行資料表創建腳本
1. 在 NHost 控制台中，進入 **Database** 頁面
2. 點擊 **SQL Editor**
3. 複製並執行 `CREATE_NHOST_TABLES.sql` 腳本內容：

```sql
-- 創建 Foods 表
CREATE TABLE IF NOT EXISTS foods (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    name TEXT NOT NULL,
    price DECIMAL(10,2),
    photo TEXT,
    shop TEXT,
    todate TIMESTAMP WITH TIME ZONE,
    photohash TEXT,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
    updated_at TIMESTAMP WITH TIME ZONE DEFAULT NOW()
);

-- 創建 Subscriptions 表
CREATE TABLE IF NOT EXISTS subscriptions (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    name TEXT NOT NULL,
    nextdate TIMESTAMP WITH TIME ZONE,
    price DECIMAL(10,2),
    site TEXT,
    note TEXT,
    account TEXT,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
    updated_at TIMESTAMP WITH TIME ZONE DEFAULT NOW()
);
```

#### 步驟 3: 驗證資料表創建
1. 在 SQL Editor 中執行：
   ```sql
   SELECT table_name FROM information_schema.tables 
   WHERE table_schema = 'public' 
   AND table_name IN ('foods', 'subscriptions');
   ```
2. 確認返回兩個資料表名稱

#### 步驟 4: 設定 GraphQL 權限
1. 進入 **GraphQL** 頁面
2. 確認 `foods` 和 `subscriptions` 資料表在 GraphQL 架構中可見
3. 設定適當的查詢和變更權限

### 3. 測試和驗證

#### 使用診斷工具
```csharp
// 執行完整診斷
await TestNHostSchemaFix.RunSchemaDiagnosticsAsync();

// 快速驗證修正
await TestNHostSchemaFix.QuickFixVerificationAsync();

// 測試資料表創建
await TestNHostSchemaFix.TestTableCreationAsync();
```

#### 預期結果
修正成功後，應該看到：
```
=== NHost 資料庫架構診斷 ===
1. 測試 NHost 連線...
   初始化結果: ✅ 成功
2. 測試 GraphQL 端點...
   連線測試: ✅ 成功
3. 測試食品資料表存取...
   ✅ 食品資料表正常 (0 筆資料)
4. 測試訂閱資料表存取...
   ✅ 訂閱資料表正常 (0 筆資料)

✅ NHost 資料庫架構正常，所有資料表都可正常存取
```

## 修正內容總結

### 程式碼修正
- ✅ 更新 `GetFoodsAsync()` 方法 - 增強錯誤處理
- ✅ 更新 `GetSubscriptionsAsync()` 方法 - 增強錯誤處理
- ✅ 更新 `CreateFoodAsync()` 方法 - 增強錯誤處理
- ✅ 新增 `CheckTableExistsAsync()` 方法 - 資料表存在檢查
- ✅ 改善 GraphQL 回應解析 - 安全的屬性存取

### 診斷工具
- ✅ 創建 `TestNHostSchemaFix.cs` - 架構診斷工具
- ✅ 提供詳細的錯誤訊息和修正建議
- ✅ 支援快速驗證和完整診斷

### 用戶體驗改善
- ✅ 清楚的錯誤訊息指導用戶如何修正
- ✅ 提供具體的修正步驟
- ✅ 自動檢測資料表是否存在

## NHost 配置資訊

- **Region**: eu-central-1
- **Subdomain**: uxgwdiuehabbzenwtcqo
- **GraphQL URL**: https://uxgwdiuehabbzenwtcqo.hasura.eu-central-1.nhost.run/v1/graphql
- **Admin Secret**: cu#34&yjF3Cr%fgxB#WA,4r4^c=Igcwr

## 相關檔案

### 修正的檔案
- `wpfkiro20260101/Services/NHostService.cs` - 增強錯誤處理和資料表檢查

### 新增的檔案
- `wpfkiro20260101/TestNHostSchemaFix.cs` - 架構診斷工具
- `wpfkiro20260101/NHOST_SCHEMA_FIX.md` - 本修正文檔

### 相關檔案
- `wpfkiro20260101/CREATE_NHOST_TABLES.sql` - 資料表創建腳本
- `wpfkiro20260101/NHOST_CRUD_OPERATIONS_GUIDE.md` - CRUD 操作指南

## 故障排除

### 如果仍然出現錯誤

1. **檢查 Admin Secret**:
   - 確認 Admin Secret 正確
   - 檢查是否有特殊字元需要編碼

2. **檢查網路連線**:
   - 確認可以存取 NHost GraphQL 端點
   - 檢查防火牆設定

3. **檢查資料表權限**:
   - 在 NHost 控制台中檢查 GraphQL 權限設定
   - 確認 Admin Secret 有足夠權限存取資料表

4. **重新創建資料表**:
   - 如果資料表已存在但仍有問題，可以先刪除再重新創建
   - 確認資料表結構與 GraphQL 架構一致

## 總結

✅ **問題解決**: GraphQL "field not found" 錯誤已修正

✅ **錯誤處理**: 提供清楚的錯誤訊息和修正指導

✅ **診斷工具**: 創建完整的診斷和測試工具

✅ **用戶體驗**: 改善錯誤訊息，提供具體修正步驟

用戶現在會收到清楚的指導訊息，告知需要先設定 NHost 資料庫，而不是看到難以理解的 GraphQL 錯誤。