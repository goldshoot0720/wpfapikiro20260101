# NHost GraphQL 權限修正指南

## 問題現況

✅ **錯誤訊息已改善**: 現在顯示清楚的中文指導訊息
✅ **資料表已存在**: NHost 資料庫中有 `food` 和 `subscription` 資料表
❌ **權限問題**: GraphQL 查詢仍然失敗，需要設定權限

## 快速修正步驟

### 步驟 1: 登入 NHost 控制台
1. 前往 https://app.nhost.io/
2. 登入您的帳號
3. 選擇專案 `goldshoot0720`

### 步驟 2: 設定 GraphQL 權限
1. 點擊左側選單的 **"GraphQL"**
2. 進入 **"Permissions"** 頁面
3. 為 `foods` 資料表設定權限：
   - **Role**: `public`
   - **Operation**: `select` (查詢)
   - **Permission**: `{}` (允許所有)
4. 為 `subscriptions` 資料表設定相同權限

### 步驟 3: 設定完整 CRUD 權限
為了支援完整的 CRUD 操作，需要設定以下權限：

#### Foods 資料表權限
```json
Role: public
Operations:
- select: {} (允許查詢所有記錄)
- insert: {} (允許插入新記錄)
- update: {} (允許更新記錄)
- delete: {} (允許刪除記錄)
```

#### Subscriptions 資料表權限
```json
Role: public
Operations:
- select: {} (允許查詢所有記錄)
- insert: {} (允許插入新記錄)
- update: {} (允許更新記錄)
- delete: {} (允許刪除記錄)
```

### 步驟 4: 驗證設定
1. 在 GraphQL Playground 中測試查詢：
   ```graphql
   query {
     foods {
       id
       name
       price
       shop
     }
   }
   ```

2. 測試訂閱查詢：
   ```graphql
   query {
     subscriptions {
       id
       name
       price
       site
     }
   }
   ```

### 步驟 5: 重新測試應用程式
1. 重新啟動 WPF 應用程式
2. 選擇 NHost 作為後端服務
3. 測試食品和訂閱功能

## 權限設定詳細說明

### 為什麼需要設定權限？
NHost 使用 Hasura GraphQL 引擎，預設情況下所有資料表都是受保護的。即使使用 Admin Secret，也需要明確設定權限規則。

### 權限規則說明
- `{}`: 允許所有操作，無條件限制
- `{"user_id": {"_eq": "X-Hasura-User-Id"}}`: 僅允許存取自己的資料
- `{"is_public": {"_eq": true}}`: 僅允許存取公開資料

### 安全考量
目前使用 `{}` 允許所有操作是為了快速解決問題。在生產環境中，建議：
1. 創建適當的用戶角色
2. 設定更嚴格的權限規則
3. 實作用戶認證和授權

## 診斷工具

### 快速檢查
```csharp
await FixNHostGraphQLPermissions.QuickPermissionCheckAsync();
```

### 完整診斷
```csharp
await FixNHostGraphQLPermissions.RunPermissionFixAsync();
```

### 顯示設定資訊
```csharp
FixNHostGraphQLPermissions.ShowNHostConfiguration();
```

## 常見問題

### Q: 設定權限後仍然失敗？
A: 
1. 檢查 Admin Secret 是否正確
2. 確認資料表名稱拼寫正確 (`foods`, `subscriptions`)
3. 清除瀏覽器快取並重新載入 GraphQL 控制台

### Q: 如何確認權限設定成功？
A: 
1. 在 GraphQL Playground 中能成功執行查詢
2. 應用程式不再顯示 "field not found" 錯誤
3. 能正常載入食品和訂閱資料

### Q: 權限設定會影響其他功能嗎？
A: 
不會。權限設定只影響 GraphQL 查詢的存取控制，不會影響資料庫本身或其他服務。

## 相關檔案

- `FixNHostGraphQLPermissions.cs` - 權限診斷工具
- `Services/NHostService.cs` - NHost 服務實作
- `CREATE_NHOST_TABLES.sql` - 資料表創建腳本
- `NHOST_SCHEMA_FIX.md` - 架構修正文檔

## 總結

1. ✅ 錯誤訊息已改善，用戶現在知道問題所在
2. ✅ 資料表已存在，資料庫設定正確
3. 🔧 需要設定 GraphQL 權限以允許查詢存取
4. 🎯 設定完成後，NHost 功能將完全正常運作

按照上述步驟設定權限後，應用程式將能正常使用 NHost 進行食品和訂閱管理。