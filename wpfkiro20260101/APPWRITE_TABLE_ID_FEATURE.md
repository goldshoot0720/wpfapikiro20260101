# Appwrite Table ID 欄位功能

## 🎯 功能概述

為 Appwrite 後端服務添加了 Food Table ID 和 Subscription Table ID 設定欄位，讓用戶可以自定義資料表的 Collection ID。

## ✅ 已完成的功能

### 1. 新增界面欄位
在系統設定頁面的 Appwrite 設定區域新增了兩個欄位：
- **Food Table ID**: 用於設定食品資料的 Collection ID
- **Subscription Table ID**: 用於設定訂閱資料的 Collection ID

### 2. 界面設計特點
- **僅在選擇 Appwrite 時顯示**: 這些欄位只有在選擇 Appwrite 後端服務時才會顯示
- **自動隱藏**: 選擇其他後端服務時會自動隱藏這些欄位
- **預設值**: 自動填入預設的 Collection ID (`food` 和 `subscription`)

### 3. 設定管理
- **載入設定**: 應用程式啟動時會載入已儲存的 Table ID 設定
- **儲存設定**: 點擊「儲存設定」時會將 Table ID 一併儲存
- **預設值填入**: 選擇 Appwrite 時會自動填入預設的 Table ID

## 🔧 使用方法

### 步驟 1: 選擇 Appwrite 服務
```
1. 進入「系統設定」頁面
2. 在「後端服務設定」區域選擇「Appwrite」
3. 界面會顯示 Appwrite 專用的設定欄位
```

### 步驟 2: 設定 Table ID
```
1. 在「Food Table ID」欄位中輸入食品資料表的 Collection ID
   - 預設值: food
   - 可自定義為其他名稱 (如: foods, food_items 等)

2. 在「Subscription Table ID」欄位中輸入訂閱資料表的 Collection ID
   - 預設值: subscription
   - 可自定義為其他名稱 (如: subscriptions, user_subscriptions 等)
```

### 步驟 3: 儲存設定
```
1. 填入所有必要的 Appwrite 設定
2. 點擊「儲存設定」按鈕
3. 設定會儲存到本地設定檔案
```

## 📋 設定欄位說明

### Appwrite 完整設定欄位
當選擇 Appwrite 時，會顯示以下欄位：

1. **API Endpoint**: Appwrite API 端點 URL
2. **Project ID**: Appwrite 專案 ID
3. **Database ID**: Appwrite 資料庫 ID
4. **Bucket ID**: Appwrite 儲存桶 ID
5. **Food Table ID**: 食品資料表 Collection ID ⭐ 新增
6. **Subscription Table ID**: 訂閱資料表 Collection ID ⭐ 新增
7. **API Key**: Appwrite API 金鑰

### 預設值
```json
{
  "FoodCollectionId": "food",
  "SubscriptionCollectionId": "subscription"
}
```

## 🔄 自動化功能

### 1. 智能顯示/隱藏
- **選擇 Appwrite**: 顯示所有 Appwrite 專用欄位（包含新的 Table ID 欄位）
- **選擇其他服務**: 隱藏所有 Appwrite 專用欄位

### 2. 預設值自動填入
- 首次選擇 Appwrite 時，會自動填入預設的 Table ID
- 如果欄位為空，會自動填入預設值

### 3. 設定持久化
- 所有 Table ID 設定會儲存到 `settings.json` 檔案
- 應用程式重新啟動時會自動載入已儲存的設定

## 💡 使用場景

### 1. 預設使用
```
大多數用戶可以使用預設的 Table ID：
- Food Table ID: food
- Subscription Table ID: subscription
```

### 2. 自定義資料表名稱
```
進階用戶可以自定義 Collection 名稱：
- Food Table ID: my_foods
- Subscription Table ID: my_subscriptions
```

### 3. 多環境部署
```
不同環境使用不同的 Collection 名稱：
- 開發環境: dev_food, dev_subscription
- 生產環境: prod_food, prod_subscription
```

## 🔍 技術實作細節

### 1. XAML 界面
```xml
<!-- Food Collection ID (Appwrite 專用) -->
<TextBlock x:Name="FoodCollectionIdLabel"
           Text="Food Table ID:" 
           Visibility="Collapsed"/>
<TextBox x:Name="FoodCollectionIdTextBox" 
         Visibility="Collapsed"/>

<!-- Subscription Collection ID (Appwrite 專用) -->
<TextBlock x:Name="SubscriptionCollectionIdLabel"
           Text="Subscription Table ID:" 
           Visibility="Collapsed"/>
<TextBox x:Name="SubscriptionCollectionIdTextBox" 
         Visibility="Collapsed"/>
```

### 2. C# 程式邏輯
```csharp
// 載入設定
FoodCollectionIdTextBox.Text = settings.FoodCollectionId;
SubscriptionCollectionIdTextBox.Text = settings.SubscriptionCollectionId;

// 儲存設定
settings.FoodCollectionId = FoodCollectionIdTextBox.Text;
settings.SubscriptionCollectionId = SubscriptionCollectionIdTextBox.Text;

// 顯示/隱藏欄位
FoodCollectionIdLabel.Visibility = System.Windows.Visibility.Visible;
FoodCollectionIdTextBox.Visibility = System.Windows.Visibility.Visible;
```

### 3. 設定檔案格式
```json
{
  "BackendService": 0,
  "ApiUrl": "https://fra.cloud.appwrite.io/v1",
  "ProjectId": "69565017002c03b93af8",
  "DatabaseId": "69565a2800074e1d96c5",
  "BucketId": "6956530b0018bc91e180",
  "FoodCollectionId": "food",
  "SubscriptionCollectionId": "subscription",
  "ApiKey": "standard_bb04794eeaa3f7b9a993866f231d1b2146b595f3..."
}
```

## 🎉 功能優勢

### 1. 靈活性
- 用戶可以根據需求自定義資料表名稱
- 支援不同的命名慣例

### 2. 易用性
- 預設值讓大多數用戶無需修改即可使用
- 界面自動顯示/隱藏相關欄位

### 3. 一致性
- 與現有的 Appwrite 設定欄位保持一致的設計風格
- 遵循相同的顯示/隱藏邏輯

### 4. 可維護性
- 設定集中管理在 AppSettings 類別中
- 易於擴展和修改

這個功能讓用戶在使用 Appwrite 時有更大的彈性來組織和命名他們的資料表！