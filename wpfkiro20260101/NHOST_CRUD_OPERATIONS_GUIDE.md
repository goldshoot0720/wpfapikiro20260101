# NHost CRUD 操作完整指南

## 概述

本指南詳細說明如何使用 NHost 服務進行食品和訂閱的 CRUD (Create, Read, Update, Delete) 操作。

## NHost 配置資訊

- **Region**: eu-central-1
- **Subdomain**: uxgwdiuehabbzenwtcqo
- **GraphQL URL**: https://uxgwdiuehabbzenwtcqo.hasura.eu-central-1.nhost.run/v1/graphql
- **Admin Secret**: cu#34&yjF3Cr%fgxB#WA,4r4^c=Igcwr

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

## 食品 CRUD 操作

### 1. 獲取所有食品 (Read)

```csharp
var nHostService = new NHostService();
var result = await nHostService.GetFoodsAsync();

if (result.Success)
{
    var foods = result.Data;
    Console.WriteLine($"獲取到 {foods.Length} 筆食品資料");
}
else
{
    Console.WriteLine($"錯誤: {result.ErrorMessage}");
}
```

**GraphQL 查詢**:
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

### 2. 創建食品 (Create)

```csharp
var newFood = new
{
    name = "蘋果",
    price = 50.00,
    photo = "apple.jpg",
    shop = "水果店",
    todate = DateTime.Now.AddDays(7).ToString("yyyy-MM-ddTHH:mm:ssZ"),
    photohash = "hash_apple_001"
};

var result = await nHostService.CreateFoodAsync(newFood);

if (result.Success)
{
    Console.WriteLine("食品創建成功");
    // result.Data 包含創建的食品資料
}
```

**GraphQL Mutation**:
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

### 3. 更新食品 (Update)

```csharp
var foodId = "your-food-uuid-here";
var updateData = new
{
    name = "更新後的蘋果",
    price = 60.00,
    shop = "新水果店"
};

var result = await nHostService.UpdateFoodAsync(foodId, updateData);

if (result.Success)
{
    Console.WriteLine("食品更新成功");
}
```

**GraphQL Mutation**:
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

### 4. 刪除食品 (Delete)

```csharp
var foodId = "your-food-uuid-here";
var result = await nHostService.DeleteFoodAsync(foodId);

if (result.Success && result.Data)
{
    Console.WriteLine("食品刪除成功");
}
```

**GraphQL Mutation**:
```graphql
mutation DeleteFood($id: uuid!) {
    delete_foods_by_pk(id: $id) {
        id
    }
}
```

## 訂閱 CRUD 操作

### 1. 獲取所有訂閱 (Read)

```csharp
var result = await nHostService.GetSubscriptionsAsync();

if (result.Success)
{
    var subscriptions = result.Data;
    Console.WriteLine($"獲取到 {subscriptions.Length} 筆訂閱資料");
}
```

**GraphQL 查詢**:
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

### 2. 創建訂閱 (Create)

```csharp
var newSubscription = new
{
    name = "Netflix",
    nextdate = DateTime.Now.AddDays(30).ToString("yyyy-MM-ddTHH:mm:ssZ"),
    price = 390.00,
    site = "netflix.com",
    note = "影音串流服務",
    account = "user@example.com"
};

var result = await nHostService.CreateSubscriptionAsync(newSubscription);

if (result.Success)
{
    Console.WriteLine("訂閱創建成功");
}
```

**GraphQL Mutation**:
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

### 3. 更新訂閱 (Update)

```csharp
var subscriptionId = "your-subscription-uuid-here";
var updateData = new
{
    name = "Netflix Premium",
    price = 490.00,
    note = "升級為高級方案"
};

var result = await nHostService.UpdateSubscriptionAsync(subscriptionId, updateData);

if (result.Success)
{
    Console.WriteLine("訂閱更新成功");
}
```

**GraphQL Mutation**:
```graphql
mutation UpdateSubscription($id: uuid!, $changes: subscriptions_set_input!) {
    update_subscriptions_by_pk(pk_columns: {id: $id}, _set: $changes) {
        id
        name
        nextdate
        price
        site
        note
        account
        updated_at
    }
}
```

### 4. 刪除訂閱 (Delete)

```csharp
var subscriptionId = "your-subscription-uuid-here";
var result = await nHostService.DeleteSubscriptionAsync(subscriptionId);

if (result.Success && result.Data)
{
    Console.WriteLine("訂閱刪除成功");
}
```

**GraphQL Mutation**:
```graphql
mutation DeleteSubscription($id: uuid!) {
    delete_subscriptions_by_pk(id: $id) {
        id
    }
}
```

## 認證功能

### 用戶註冊

```csharp
var email = "user@example.com";
var password = "SecurePassword123!";

var result = await nHostService.RegisterAsync(email, password);

if (result.Success)
{
    var userId = result.Data;
    Console.WriteLine($"註冊成功，用戶 ID: {userId}");
}
```

### 用戶登入

```csharp
var result = await nHostService.LoginAsync(email, password);

if (result.Success)
{
    var accessToken = result.Data;
    Console.WriteLine("登入成功");
    // accessToken 會自動設定到 HttpClient 的 Authorization 標頭
}
```

## 測試方法

### 完整測試

```csharp
var tester = new TestNHostCrudOperations();
await tester.RunAllTestsAsync();
```

### 快速測試

```csharp
var tester = new TestNHostCrudOperations();
await tester.QuickTestAsync();
```

### 認證測試

```csharp
var tester = new TestNHostCrudOperations();
await tester.TestAuthenticationAsync();
```

## 錯誤處理

所有 CRUD 操作都返回 `BackendServiceResult<T>` 類型，包含：

- `Success`: 操作是否成功
- `Data`: 返回的資料 (成功時)
- `ErrorMessage`: 錯誤訊息 (失敗時)

```csharp
var result = await nHostService.GetFoodsAsync();

if (result.Success)
{
    // 處理成功結果
    var foods = result.Data;
}
else
{
    // 處理錯誤
    Console.WriteLine($"操作失敗: {result.ErrorMessage}");
}
```

## 注意事項

1. **UUID 格式**: 所有 ID 都使用 UUID 格式
2. **日期格式**: 使用 ISO 8601 格式 (`yyyy-MM-ddTHH:mm:ssZ`)
3. **Admin Secret**: 所有請求都需要正確的 Admin Secret
4. **GraphQL 端點**: 使用 Hasura GraphQL 端點進行所有操作
5. **自動時間戳**: `created_at` 和 `updated_at` 由資料庫自動管理

## 相關檔案

- `Services/NHostService.cs`: NHost 服務實作
- `TestNHostCrudOperations.cs`: CRUD 操作測試
- `CREATE_NHOST_TABLES.sql`: 資料表創建腳本
- `Models/Food.cs`: 食品模型
- `Models/Subscription.cs`: 訂閱模型

## 快速開始

1. 確保 NHost 專案已正確設定
2. 執行 `CREATE_NHOST_TABLES.sql` 創建資料表
3. 使用 `TestNHostCrudOperations` 測試功能
4. 在應用程式中使用 `NHostService` 進行 CRUD 操作