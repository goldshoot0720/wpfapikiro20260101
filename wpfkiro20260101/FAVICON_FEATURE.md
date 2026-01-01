# 訂閱項目 Favicon 顯示功能

## 功能概述

為訂閱管理系統添加了 favicon 圖示顯示功能，當訂閱項目有網站且網站有可用的 favicon 時，會在項目欄位中顯示對應的 favicon 圖示。

## 實現細節

### 1. Favicon 獲取邏輯

- 自動從網站 URL 提取 favicon
- 支持多種常見的 favicon 路徑：
  - `/favicon.ico`
  - `/favicon.png`
  - `/apple-touch-icon.png`
  - `/apple-touch-icon-precomposed.png`

### 2. UI 改進

- 在每個訂閱卡片左側添加了 favicon 顯示區域
- 當無法載入 favicon 時顯示預設的地球圖示 🌐
- Favicon 尺寸設置為 16x16 像素，適合卡片布局

### 3. 異步載入

- 使用異步方式載入 favicon，不會阻塞 UI
- 設置了 10 秒的超時時間，避免長時間等待
- 載入失敗時會保持預設圖示

### 4. 記憶體優化

- 將 favicon 圖像設置為 16x16 像素以節省記憶體
- 使用 `Freeze()` 方法使圖像可以跨線程使用
- 設置適當的緩存選項

## 使用方式

1. 在訂閱項目中填入網站 URL（如：https://www.netflix.com）
2. 系統會自動嘗試載入該網站的 favicon
3. 成功載入後，favicon 會顯示在訂閱卡片的左側
4. 如果載入失敗，會顯示預設的地球圖示

## 支持的網站格式

- 完整 URL：`https://www.example.com`
- 不含協議的 URL：`www.example.com`（會自動添加 https://）
- 域名：`example.com`（會自動添加 https://）

## 技術實現

### 核心方法

1. `GetFaviconAsync(string websiteUrl)` - 異步獲取 favicon
2. `LoadFaviconForCard(Border faviconContainer, string websiteUrl)` - 為卡片載入 favicon
3. 修改了 `CreateSubscriptionCard()` 方法以包含 favicon 顯示

### 錯誤處理

- 網路連接失敗時的優雅降級
- 無效 URL 的處理
- 圖像格式不支持時的處理
- 超時處理

## 測試建議

1. 測試知名網站的 favicon 載入（如 Netflix、YouTube、GitHub）
2. 測試無 favicon 的網站
3. 測試無效的 URL
4. 測試網路連接問題的情況

## 未來改進

1. 可以添加 favicon 緩存機制以提高性能
2. 支持更多 favicon 格式和路徑
3. 添加 favicon 載入狀態指示器
4. 支持自定義 favicon 上傳