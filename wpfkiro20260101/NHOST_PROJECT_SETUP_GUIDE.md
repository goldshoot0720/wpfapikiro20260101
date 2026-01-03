# NHost 專案實作設定指南

## 專案配置信息
- **Region**: eu-central-1
- **Subdomain**: uxgwdiuehabbzenwtcqo
- **Admin Secret**: cu#34&yjF3Cr%fgxB#WA,4r4^c=Igcwr
- **GraphQL 端點**: https://uxgwdiuehabbzenwtcqo.graphql.eu-central-1.nhost.run/v1

## 設定步驟

### 1. 資料庫設定
在 NHost 控制台中執行以下 SQL 腳本來創建必要的資料表：

```sql
-- 執行 CREATE_NHOST_TABLES.sql 中的所有腳本
-- 或者手動執行以下命令：

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

### 2. Hasura 權限設定
由於使用 Admin Secret，所有操作都有完整權限。如需細粒度控制，可在 Hasura 控制台設定：

1. 開啟 Hasura 控制台
2. 進入 Data 頁面
3. 確認 `foods` 和 `subscriptions` 表已正確創建
4. 設定適當的權限規則（可選）

### 3. 應用程式配置
NHost 服務已在應用程式中完全配置：

```csharp
// 服務已自動配置以下設定：
// - Region: eu-central-1
// - Subdomain: uxgwdiuehabbzenwtcqo
// - Admin Secret: cu#34&yjF3Cr%fgxB#WA,4r4^c=Igcwr
// - 所有 GraphQL 請求自動包含認證標頭

// 使用方式：
var nhostService = BackendServiceFactory.CreateService(BackendServiceType.NHost);
var foods = await nhostService.GetFoodsAsync();
```

## 測試步驟

### 1. 執行專案實作測試
```csharp
await NHostProjectImplementation.RunImplementationTest();
```

### 2. 執行 Admin Secret 測試
```csharp
await TestNHostWithAdminSecret.RunAdminSecretTest();
```

### 3. 執行整合測試
```csharp
await TestNHostIntegration.RunIntegrationTest();
```

## 功能驗證

### 資料查詢測試
- ✅ 查詢所有食品資料
- ✅ 查詢所有訂閱資料
- ✅ GraphQL 錯誤處理
- ✅ Admin Secret 認證

### CRUD 操作測試
- ✅ 創建新食品
- ✅ 創建新訂閱
- ✅ 更新現有資料
- ✅ 刪除資料

### 整合測試
- ✅ 工廠模式整合
- ✅ CRUD 管理器整合
- ✅ 錯誤處理機制

## GraphQL 查詢範例

### 查詢所有食品
```graphql
query GetFoods {
    foods {
        id
        name
        price
        photo
        shop
        todate
        photohash
        created_at
        updated_at
    }
}
```

### 創建新食品
```graphql
mutation CreateFood($object: foods_insert_input!) {
    insert_foods_one(object: $object) {
        id
        name
        price
        photo
        shop
        todate
        photohash
        created_at
    }
}
```

### 查詢所有訂閱
```graphql
query GetSubscriptions {
    subscriptions {
        id
        name
        nextdate
        price
        site
        note
        account
        created_at
        updated_at
    }
}
```

### 創建新訂閱
```graphql
mutation CreateSubscription($object: subscriptions_insert_input!) {
    insert_subscriptions_one(object: $object) {
        id
        name
        nextdate
        price
        site
        note
        account
        created_at
    }
}
```

## 端點配置

### GraphQL API
```
URL: https://uxgwdiuehabbzenwtcqo.graphql.eu-central-1.nhost.run/v1
Headers:
  Content-Type: application/json
  x-hasura-admin-secret: cu#34&yjF3Cr%fgxB#WA,4r4^c=Igcwr
```

### 認證 API
```
URL: https://uxgwdiuehabbzenwtcqo.auth.eu-central-1.nhost.run/v1
```

### 函數 API
```
URL: https://uxgwdiuehabbzenwtcqo.functions.eu-central-1.nhost.run/v1
```

### 儲存 API
```
URL: https://uxgwdiuehabbzenwtcqo.storage.eu-central-1.nhost.run/v1
```

## 故障排除

### 常見問題

1. **GraphQL 查詢失敗**
   - 檢查 Admin Secret 是否正確
   - 確認資料表是否已創建
   - 驗證查詢語法

2. **連線超時**
   - 檢查網路連線
   - 確認 NHost 專案狀態
   - 驗證端點 URL

3. **權限錯誤**
   - 確認 Admin Secret 已正確設定
   - 檢查 Hasura 權限規則

### 除錯步驟

1. 執行連線測試
2. 檢查 GraphQL 端點回應
3. 驗證資料表結構
4. 測試簡單查詢
5. 檢查錯誤訊息

## 安全性考量

### Admin Secret 使用
- ✅ 適合開發和測試環境
- ⚠️ 生產環境建議使用 JWT 認證
- ⚠️ Admin Secret 提供完整資料庫存取權限

### 建議的生產環境設定
1. 使用 JWT 認證替代 Admin Secret
2. 設定細粒度權限規則
3. 啟用 CORS 限制
4. 使用環境變數管理敏感資訊

## 相關檔案
- `Services/NHostService.cs` - NHost 服務實現
- `NHostProjectImplementation.cs` - 專案實作測試
- `CREATE_NHOST_TABLES.sql` - 資料表創建腳本
- `TestNHostWithAdminSecret.cs` - Admin Secret 測試
- `README_NHost.md` - 詳細使用指南

---

**NHost 專案實作完成！** 🎉

按照此指南設定後，即可開始使用 NHost 作為應用程式的後端服務。