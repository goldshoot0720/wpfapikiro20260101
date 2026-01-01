# Supabase 配置指南

## 連接資訊

### 專案資訊
- **Project ID**: `lobezwpworbfktlkxuyo`
- **URL**: `https://lobezwpworbfktlkxuyo.supabase.co`
- **RESTful Endpoint**: `https://lobezwpworbfktlkxuyo.supabase.co`

### API 金鑰
- **Publishable Key**: `sb_publishable_rRdFecl88xBtuCiokGk8fQ_CUd3Rwt-`
- **Secret Key**: `eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6ImxvYmV6d3B3b3JiZmt0bGt4dXlvIiwicm9sZSI6InNlcnZpY2Vfcm9sZSIsImlhdCI6MTc2NzI1ODU5MSwiZXhwIjoyMDgyODM0NTkxfQ.tFcCP7kvcfV1CznhIHXBF0TenGlYD1XRlAWdCYYEnlc`

## 應用程式設定

### 在應用程式中配置 Supabase
1. 開啟應用程式
2. 進入「設定」頁面
3. 選擇「Supabase」作為後端服務
4. 填入以下資訊：
   - **API URL**: `https://lobezwpworbfktlkxuyo.supabase.co`
   - **API Key**: `eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6ImxvYmV6d3B3b3JiZmt0bGt4dXlvIiwicm9sZSI6InNlcnZpY2Vfcm9sZSIsImlhdCI6MTc2NzI1ODU5MSwiZXhwIjoyMDgyODM0NTkxfQ.tFcCP7kvcfV1CznhIHXBF0TenGlYD1XRlAWdCYYEnlc`
   - **Project ID**: `lobezwpworbfktlkxuyo`

## 資料庫結構

### 需要創建的資料表

#### 1. foods 資料表
```sql
CREATE TABLE foods (
    id UUID DEFAULT gen_random_uuid() PRIMARY KEY,
    food_name VARCHAR(255) NOT NULL,
    price INTEGER DEFAULT 0,
    quantity INTEGER DEFAULT 1,
    photo TEXT,
    photo_hash TEXT,
    shop VARCHAR(255),
    to_date VARCHAR(50),
    description TEXT,
    category VARCHAR(100),
    storage_location VARCHAR(100),
    note TEXT,
    created_at TIMESTAMPTZ DEFAULT NOW(),
    updated_at TIMESTAMPTZ DEFAULT NOW()
);
```

#### 2. subscriptions 資料表
```sql
CREATE TABLE subscriptions (
    id UUID DEFAULT gen_random_uuid() PRIMARY KEY,
    subscription_name VARCHAR(255) NOT NULL,
    next_date DATE,
    price INTEGER DEFAULT 0,
    site TEXT,
    account VARCHAR(255),
    note TEXT,
    string_to_date VARCHAR(50),
    date_time TIMESTAMPTZ,
    food_id UUID REFERENCES foods(id),
    created_at TIMESTAMPTZ DEFAULT NOW(),
    updated_at TIMESTAMPTZ DEFAULT NOW()
);
```

### 建立資料表的步驟

1. **登入 Supabase Dashboard**
   - 前往 https://supabase.com/dashboard
   - 選擇專案 `lobezwpworbfktlkxuyo`

2. **開啟 SQL Editor**
   - 點擊左側選單的「SQL Editor」
   - 點擊「New query」

3. **執行 SQL 指令**
   - 複製上述的 `foods` 資料表 SQL 指令
   - 貼上並執行
   - 複製上述的 `subscriptions` 資料表 SQL 指令
   - 貼上並執行

4. **設定 Row Level Security (RLS)**
   ```sql
   -- 啟用 RLS
   ALTER TABLE foods ENABLE ROW LEVEL SECURITY;
   ALTER TABLE subscriptions ENABLE ROW LEVEL SECURITY;
   
   -- 創建允許所有操作的政策（開發用）
   CREATE POLICY "Allow all operations on foods" ON foods FOR ALL USING (true);
   CREATE POLICY "Allow all operations on subscriptions" ON subscriptions FOR ALL USING (true);
   ```

## 欄位對照表

### Foods 資料表欄位對照
| 應用程式屬性 | Supabase 欄位 | 類型 | 說明 |
|-------------|---------------|------|------|
| Id | id | UUID | 主鍵 |
| FoodName | food_name | VARCHAR(255) | 食品名稱 |
| Price | price | INTEGER | 價格 |
| Quantity | quantity | INTEGER | 數量 |
| Photo | photo | TEXT | 照片 URL |
| PhotoHash | photo_hash | TEXT | 照片雜湊值 |
| Shop | shop | VARCHAR(255) | 商店名稱 |
| ToDate | to_date | VARCHAR(50) | 到期日期 |
| Description | description | TEXT | 描述 |
| Category | category | VARCHAR(100) | 分類 |
| StorageLocation | storage_location | VARCHAR(100) | 儲存位置 |
| Note | note | TEXT | 備註 |
| CreatedAt | created_at | TIMESTAMPTZ | 創建時間 |
| UpdatedAt | updated_at | TIMESTAMPTZ | 更新時間 |

### Subscriptions 資料表欄位對照
| 應用程式屬性 | Supabase 欄位 | 類型 | 說明 |
|-------------|---------------|------|------|
| Id | id | UUID | 主鍵 |
| SubscriptionName | subscription_name | VARCHAR(255) | 訂閱名稱 |
| NextDate | next_date | DATE | 下次付款日期 |
| Price | price | INTEGER | 價格 |
| Site | site | TEXT | 網站 URL |
| Account | account | VARCHAR(255) | 帳戶資訊 |
| Note | note | TEXT | 備註 |
| StringToDate | string_to_date | VARCHAR(50) | 日期字串 |
| DateTime | date_time | TIMESTAMPTZ | 日期時間 |
| FoodId | food_id | UUID | 關聯食品 ID |
| CreatedAt | created_at | TIMESTAMPTZ | 創建時間 |
| UpdatedAt | updated_at | TIMESTAMPTZ | 更新時間 |

## API 端點

### Foods API
- **GET** `/rest/v1/food` - 獲取所有食品
- **POST** `/rest/v1/food` - 創建新食品
- **PATCH** `/rest/v1/food?id=eq.{id}` - 更新食品
- **DELETE** `/rest/v1/food?id=eq.{id}` - 刪除食品

### Subscriptions API
- **GET** `/rest/v1/subscription` - 獲取所有訂閱
- **POST** `/rest/v1/subscription` - 創建新訂閱
- **PATCH** `/rest/v1/subscription?id=eq.{id}` - 更新訂閱
- **DELETE** `/rest/v1/subscription?id=eq.{id}` - 刪除訂閱

## 測試連接

### 使用應用程式測試
1. 配置好 Supabase 設定後
2. 進入「食品管理」或「訂閱管理」頁面
3. 點擊「🔄 重新載入」按鈕
4. 檢查是否能成功連接並載入資料

### 使用 curl 測試
```bash
# 測試 API 連接
curl -H "apikey: sb_secret_B2gtQik_DZEKevBc82viAw_mbvPA8F1" \
     -H "Authorization: Bearer sb_secret_B2gtQik_DZEKevBc82viAw_mbvPA8F1" \
     https://lobezwpworbfktlkxuyo.supabase.co/rest/v1/food

# 測試訂閱 API
curl -H "apikey: sb_secret_B2gtQik_DZEKevBc82viAw_mbvPA8F1" \
     -H "Authorization: Bearer sb_secret_B2gtQik_DZEKevBc82viAw_mbvPA8F1" \
     https://lobezwpworbfktlkxuyo.supabase.co/rest/v1/subscription
```

## 安全性注意事項

### API 金鑰管理
- **Publishable Key**: 可以在前端使用，權限受限
- **Secret Key**: 僅用於後端，具有完整權限
- 本應用程式使用 Secret Key 進行 API 呼叫

### Row Level Security (RLS)
- 已啟用 RLS 保護資料
- 目前設定為允許所有操作（開發環境）
- 生產環境建議設定更嚴格的存取政策

## 故障排除

### 常見問題

1. **連接失敗**
   - 檢查 API URL 是否正確
   - 確認 API Key 是否有效
   - 檢查網路連接

2. **資料表不存在**
   - 確認已在 Supabase Dashboard 中創建資料表
   - 檢查資料表名稱是否正確（foods, subscriptions）

3. **權限錯誤**
   - 確認已啟用 RLS 並設定適當的政策
   - 檢查 API Key 權限

4. **資料格式錯誤**
   - 確認日期格式符合 Supabase 要求
   - 檢查資料類型是否匹配

## 進階功能

### 即時訂閱 (Realtime)
```javascript
// 可以在未來版本中實現即時資料同步
const subscription = supabase
  .from('foods')
  .on('*', payload => {
    console.log('Change received!', payload)
  })
  .subscribe()
```

### 檔案儲存 (Storage)
```javascript
// 可以用於儲存食品照片
const { data, error } = await supabase.storage
  .from('food-images')
  .upload('public/food1.jpg', file)
```

## 相關連結

- [Supabase 官方文件](https://supabase.com/docs)
- [Supabase REST API 文件](https://supabase.com/docs/guides/api/rest/introduction)
- [專案 Dashboard](https://supabase.com/dashboard/project/lobezwpworbfktlkxuyo)