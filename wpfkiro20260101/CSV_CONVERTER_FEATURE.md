# CSV 資料轉換功能

## 功能概述

新增了 Appwrite CSV 轉換為 Supabase CSV 的功能，讓用戶可以輕鬆地在不同後端服務之間遷移資料。

## 主要功能

### 1. 單檔轉換
- **Food CSV 轉換**: 將 Appwrite 格式的 Food CSV 轉換為 Supabase 格式
- **Subscription CSV 轉換**: 將 Appwrite 格式的 Subscription CSV 轉換為 Supabase 格式

### 2. 批次轉換
- 選擇包含多個 CSV 檔案的資料夾
- 自動識別檔案類型（根據檔案名稱）
- 批次轉換所有 CSV 檔案
- 輸出到專用的 "Supabase_Converted" 資料夾

### 3. 測試功能
- 內建測試工具，自動創建測試資料
- 驗證轉換邏輯的正確性
- 提供詳細的轉換過程資訊

## 欄位映射

### Food 表
**Appwrite 格式**:
```
$id,name,price,photo,shop,todate,photohash,$createdAt,$updatedAt
```

**Supabase 格式**:
```
id,created_at,name,todate,amount,photo,price,shop,photohash
```

**映射規則**:
- `$id` → `id`
- `$createdAt` → `created_at`
- `name` → `name`
- `todate` → `todate`
- 新增 `amount` 欄位（預設值：1）
- `photo` → `photo`
- `price` → `price`
- `shop` → `shop`
- `photohash` → `photohash`
- 移除 `$updatedAt`（Supabase 沒有此欄位）

### Subscription 表
**Appwrite 格式**:
```
$id,name,nextdate,price,site,note,account,$createdAt,$updatedAt
```

**Supabase 格式**:
```
id,created_at,name,nextdate,price,site,note,account
```

**映射規則**:
- `$id` → `id`
- `$createdAt` → `created_at`
- `name` → `name`
- `nextdate` → `nextdate`
- `price` → `price`
- `site` → `site`
- `note` → `note`
- `account` → `account`
- 移除 `$updatedAt`

## 日期格式轉換

### Appwrite 格式
```
2026-01-02T17:09:09.823Z (ISO 8601)
```

### Supabase 格式
```
2026-01-02 17:09:09.823688+00 (PostgreSQL TIMESTAMPTZ)
```

## 使用方式

### 1. 單檔轉換
1. 在系統設定頁面找到「資料轉換」區域
2. 點擊「🔄 轉換 Food CSV」或「🔄 轉換 Subscription CSV」
3. 選擇要轉換的 Appwrite CSV 檔案
4. 轉換後的檔案會保存在同一資料夾，檔名前綴為 "Supabase_"

### 2. 批次轉換
1. 點擊「📂 批次轉換資料夾」
2. 選擇包含多個 CSV 檔案的資料夾
3. 系統會自動轉換所有 CSV 檔案
4. 轉換結果保存在 "Supabase_Converted" 子資料夾

### 3. 測試功能
1. 點擊「🧪 測試轉換功能」
2. 系統會在桌面創建 "CsvConverterTest" 資料夾
3. 自動生成測試資料並進行轉換測試
4. 查看控制台輸出以獲取詳細資訊

## 技術特點

### 1. 智能 CSV 解析
- 支援引號包圍的欄位
- 處理欄位中的逗號
- 自動清理多餘的引號和空白

### 2. 錯誤處理
- 檔案不存在檢查
- 格式錯誤容錯處理
- 詳細的錯誤訊息

### 3. 編碼支援
- 使用 UTF-8 with BOM 編碼
- 確保中文字元正確顯示
- 相容 Excel 和其他 CSV 工具

### 4. 效能優化
- 異步處理，不阻塞 UI
- 批次處理大量檔案
- 記憶體效率優化

## 注意事項

### 1. 檔案格式要求
- 輸入檔案必須是有效的 CSV 格式
- 第一行必須是標題行
- 欄位數量必須與預期格式匹配

### 2. 資料完整性
- 轉換過程中會保留所有原始資料
- 日期格式會自動轉換
- 空值會保持為空值

### 3. 檔案命名
- 輸出檔案會自動加上 "Supabase_" 前綴
- 批次轉換會創建專用的輸出資料夾
- 避免覆蓋原始檔案

## 故障排除

### 常見問題

1. **轉換失敗**
   - 檢查輸入檔案格式是否正確
   - 確認檔案沒有被其他程式佔用
   - 檢查磁碟空間是否足夠

2. **欄位數量不匹配**
   - 確認 CSV 檔案的欄位數量
   - 檢查是否有缺失的欄位
   - 驗證標題行格式

3. **日期格式錯誤**
   - 確認原始日期格式符合 ISO 8601
   - 檢查時區設定
   - 驗證日期值的有效性

### 調試工具

使用內建的測試功能可以：
- 驗證轉換邏輯
- 檢查欄位映射
- 測試日期格式轉換
- 確認檔案編碼

## 未來改進

1. **支援更多格式**
   - Excel 檔案轉換
   - JSON 格式轉換
   - 自定義欄位映射

2. **進階功能**
   - 資料驗證規則
   - 轉換預覽功能
   - 轉換歷史記錄

3. **效能提升**
   - 多執行緒處理
   - 進度條顯示
   - 大檔案分塊處理