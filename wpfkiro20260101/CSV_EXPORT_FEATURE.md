# CSV 匯出功能說明

## 功能概述

在系統設定頁面新增了 CSV 匯出功能，可以將食品和訂閱資料匯出為 CSV 格式，方便備份或在其他應用程式中使用。

## 主要特色

### 🏷️ 智能檔名前綴
- 檔案名稱會自動包含當前使用的後端服務名稱作為前綴
- 例如：`appwritefood.csv`、`supabasefood.csv`、`appwritesubscription.csv`
- 讓用戶清楚知道資料來源

### 📊 完整資料匯出
**食品資料 (food.csv)：**
- ID、食品名稱、價格、數量
- 照片 URL、商店、到期日期
- 描述、分類、儲存位置、備註
- 創建時間、更新時間

**訂閱資料 (subscription.csv)：**
- ID、訂閱名稱、下次付款日期、價格
- 網站、帳戶、備註
- 創建時間、更新時間

### 🌐 中文支援
- 使用 UTF-8 編碼並包含 BOM
- 確保 Excel 和其他應用程式正確顯示中文
- 標題行使用中文，便於理解

### 💾 用戶友善的保存體驗
- 使用 Windows 標準的檔案保存對話框
- 預設檔名包含服務前綴
- 支援自訂保存位置和檔名

## 使用方式

### 1. 進入設定頁面
- 點擊側邊欄的「系統設定」

### 2. 找到資料匯出區域
- 在「後端服務設定」和「連線設定」下方
- 可以看到「資料匯出」區域

### 3. 選擇要匯出的資料類型
- **📥 下載 food.csv** - 匯出所有食品資料
- **📥 下載 subscription.csv** - 匯出所有訂閱資料

### 4. 選擇保存位置
- 點擊按鈕後會開啟檔案保存對話框
- 預設檔名格式：`[服務名稱][資料類型].csv`
- 可以修改檔名或選擇不同的保存位置

## 檔名範例

根據當前使用的後端服務，檔名會自動調整：

### Appwrite 服務
- `appwritefood.csv`
- `appwritesubscription.csv`

### Supabase 服務
- `supabasefood.csv`
- `supabasesubscription.csv`

### Back4App 服務
- `back4appfood.csv`
- `back4appsubscription.csv`

### MySQL 服務
- `mysqlfood.csv`
- `mysqlsubscription.csv`

## 技術實現

### CSV 格式處理
- 自動處理包含逗號的欄位（用雙引號包圍）
- 正確轉義雙引號字元（轉換為兩個雙引號）
- 符合 RFC 4180 CSV 標準

### 錯誤處理
- 網路連線失敗時顯示錯誤訊息
- 檔案保存失敗時提供詳細錯誤資訊
- 資料載入失敗時的適當回饋

### 用戶體驗
- 下載過程中按鈕顯示「下載中...」狀態
- 成功下載後顯示綠色成功訊息
- 失敗時顯示紅色錯誤訊息
- 3秒後自動清除狀態訊息

## 資料欄位對應

### 食品資料欄位 (appwritefood.csv)
```csv
$id,name,price,photo,shop,todate,photohash,$createdAt,$updatedAt
```

### 訂閱資料欄位 (appwritesubscription.csv)
```csv
$id,name,nextdate,price,site,note,account,$createdAt,$updatedAt
```

## Appwrite 欄位對照表

### Food 表欄位
| CSV 欄位 | Appwrite 欄位 | 類型 | 說明 |
|---------|-------------|------|------|
| $id | $id | string | 文檔 ID |
| name | name | string | 食品名稱 (必填) |
| price | price | integer | 價格 |
| photo | photo | string | 照片 URL |
| shop | shop | string | 商店名稱 |
| todate | todate | datetime | 到期日期 |
| photohash | photohash | string | 照片雜湊值 |
| $createdAt | $createdAt | datetime | 創建時間 |
| $updatedAt | $updatedAt | datetime | 更新時間 |

### Subscription 表欄位
| CSV 欄位 | Appwrite 欄位 | 類型 | 說明 |
|---------|-------------|------|------|
| $id | $id | string | 文檔 ID |
| name | name | string | 訂閱名稱 (必填) |
| nextdate | nextdate | datetime | 下次付款日期 |
| price | price | integer | 價格 |
| site | site | string | 網站 URL |
| note | note | string | 備註 |
| account | account | string | 帳戶資訊 |
| $createdAt | $createdAt | datetime | 創建時間 |
| $updatedAt | $updatedAt | datetime | 更新時間 |

## 使用場景

### 📋 資料備份
- 定期匯出資料作為備份
- 在更換後端服務前保存資料

### 📊 資料分析
- 在 Excel 中分析食品消費模式
- 計算訂閱服務的總支出

### 🔄 資料遷移
- 將資料匯出後匯入其他系統
- 在不同後端服務間轉移資料

### 📈 報表製作
- 製作月度/年度消費報表
- 分析食品到期趨勢

## 注意事項

1. **網路連線**：需要穩定的網路連線才能從後端服務載入資料
2. **權限設定**：確保後端服務的 API 金鑰有讀取權限
3. **資料量**：大量資料可能需要較長的載入時間
4. **檔案格式**：匯出的 CSV 檔案使用 UTF-8 編碼，建議使用支援此編碼的應用程式開啟

## 疑難排解

### 下載失敗？
1. 檢查網路連線是否正常
2. 確認後端服務設定是否正確
3. 檢查 API 金鑰是否有效

### Excel 顯示亂碼？
1. 確認使用的是較新版本的 Excel
2. 嘗試使用「資料」→「從文字」功能匯入
3. 選擇 UTF-8 編碼

### 檔案無法保存？
1. 檢查目標資料夾是否有寫入權限
2. 確認磁碟空間是否足夠
3. 檢查檔名是否包含非法字元

這個功能讓您可以輕鬆地備份和分析您的食品管理資料，提升資料的可攜性和實用性！