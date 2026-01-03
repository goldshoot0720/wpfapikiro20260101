# NHost 整合指南

## 配置信息
- **Region**: eu-central-1
- **Subdomain**: uxgwdiuehabbzenwtcqo
- **Admin Secret**: cu#34&yjF3Cr%fgxB#WA,4r4^c=Igcwr
- **服務類型**: NHost (GraphQL + PostgreSQL)

## 端點 URL
```
GraphQL:   https://uxgwdiuehabbzenwtcqo.graphql.eu-central-1.nhost.run/v1
Auth:      https://uxgwdiuehabbzenwtcqo.auth.eu-central-1.nhost.run/v1
Functions: https://uxgwdiuehabbzenwtcqo.functions.eu-central-1.nhost.run/v1
Storage:   https://uxgwdiuehabbzenwtcqo.storage.eu-central-1.nhost.run/v1
```

## 認證配置
NHost 服務已配置 Hasura Admin Secret，所有 GraphQL 請求都會自動包含認證標頭：
```
x-hasura-admin-secret: cu#34&yjF3Cr%fgxB#WA,4r4^c=Igcwr
```

## 功能特色
- ✅ GraphQL API 整合
- ✅ 用戶認證 (註冊/登入)
- ✅ 即時資料庫操作
- ✅ 檔案儲存支援
- ✅ 無伺服器函數

## 資料表結構

### Foods 表
```sql
CREATE TABLE foods (
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
```

### Subscriptions 表
```sql
CREATE TABLE subscriptions (
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

## GraphQL 查詢範例

### 獲取所有食品
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

### 更新食品
```graphql
mutation UpdateFood($id: uuid!, $changes: foods_set_input!) {
    update_foods_by_pk(pk_columns: {id: $id}, _set: $changes) {
        id
        name
        price
        photo
        shop
        todate
        photohash
        updated_at
    }
}
```

### 刪除食品
```graphql
mutation DeleteFood($id: uuid!) {
    delete_foods_by_pk(id: $id) {
        id
    }
}
```

## 認證流程

### 用戶註冊
```http
POST https://uxgwdiuehabbzenwtcqo.auth.eu-central-1.nhost.run/v1/signup/email-password
Content-Type: application/json

{
    "email": "user@example.com",
    "password": "password123"
}
```

### 用戶登入
```http
POST https://uxgwdiuehabbzenwtcqo.auth.eu-central-1.nhost.run/v1/signin/email-password
Content-Type: application/json

{
    "email": "user@example.com",
    "password": "password123"
}
```

### 使用 Admin Secret 進行 GraphQL 查詢
```http
POST https://uxgwdiuehabbzenwtcqo.graphql.eu-central-1.nhost.run/v1
Content-Type: application/json
x-hasura-admin-secret: cu#34&yjF3Cr%fgxB#WA,4r4^c=Igcwr

{
    "query": "query GetFoods { foods { id name price } }"
}
```

## 使用方式

### 1. 在應用程式中選擇 NHost
在系統設定中選擇 "NHost" 作為後端服務。

### 2. 測試連線
```csharp
// 基本連線測試
await TestNHostConnection.RunTest();

// 快速測試
await TestNHostQuick.RunQuickTest();

// 完整整合測試
await TestNHostIntegration.RunIntegrationTest();

// Admin Secret 測試 (推薦)
await TestNHostWithAdminSecret.RunAdminSecretTest();
```

### 3. 進行 CRUD 操作
```csharp
var nhostService = new NHostService();

// 獲取資料
var foods = await nhostService.GetFoodsAsync();

// 創建資料
var newFood = new {
    name = "測試食品",
    price = 100,
    shop = "測試商店"
};
var result = await nhostService.CreateFoodAsync(newFood);
```

## 權限設定
在 NHost 控制台中設定適當的權限規則：

```javascript
// foods 表權限
{
  "select": {
    "filter": {}
  },
  "insert": {
    "check": {}
  },
  "update": {
    "filter": {},
    "check": {}
  },
  "delete": {
    "filter": {}
  }
}
```

## 注意事項
1. 確保 NHost 專案已正確設定資料表結構
2. 檢查權限設定是否允許匿名存取或需要認證
3. GraphQL 查詢需要符合實際的資料表結構
4. 使用 UUID 作為主鍵格式

## 故障排除

### 連線失敗
- 檢查 subdomain 和 region 是否正確
- 確認網路連線正常
- 檢查 NHost 專案狀態

### GraphQL 錯誤
- 檢查查詢語法是否正確
- 確認資料表結構與查詢匹配
- 檢查權限設定

### 認證問題
- 確認用戶已註冊
- 檢查 JWT token 是否有效
- 驗證權限設定

## 相關檔案
- `Services/NHostService.cs` - NHost 服務實現
- `TestNHostConnection.cs` - 連線測試
- `README_NHost.md` - 本文檔