# 網路圖片功能使用指南

## 功能概述

食品管理系統現在支援使用網路圖片 URL 來顯示食品圖片，無需將圖片檔案儲存在本地。

## 支援的功能

### 1. 添加食品時使用網路圖片
- 在「添加食品」窗口中，輸入有效的圖片 URL
- 點擊「預覽」按鈕可以預先查看圖片
- 系統會驗證 URL 的有效性

### 2. 編輯食品時更新圖片
- 在「編輯食品」窗口中，可以修改圖片 URL
- 支援圖片預覽功能
- 如果原本有圖片，會自動載入預覽

### 3. 食品列表顯示網路圖片
- 食品卡片會自動顯示網路圖片
- 如果圖片載入失敗，會顯示錯誤圖示
- 沒有圖片時顯示預設的食品圖示

## 支援的圖片格式

系統支援以下圖片格式的 URL：
- `.jpg` / `.jpeg`
- `.png`
- `.gif`
- `.bmp`
- `.webp`

## 支援的圖片服務

系統特別支援以下常見的圖片服務：
- **Google 圖片** (`gstatic.com/images`, `googleusercontent.com`)
- **Lorem Picsum** (`picsum.photos`) - 隨機佔位圖片
- **Placeholder.com** (`placeholder.com`) - 自訂佔位圖片
- **Unsplash** (`unsplash.com`) - 高品質免費圖片
- **Imgur** (`imgur.com`) - 圖片分享平台
- **Flickr** (`flickr.com`) - 圖片社群平台
- **Pixabay** (`pixabay.com`) - 免費圖片資源
- **Pexels** (`pexels.com`) - 免費攝影圖片
- **HTTPBin 測試圖片** (`httpbin.org/image`) - 測試用途

## 使用範例

### 有效的圖片 URL 範例：
```
https://picsum.photos/300/200?random=1
https://via.placeholder.com/300x200/FF0000/FFFFFF?text=Food
https://images.unsplash.com/photo-1567620905732-2d1ec7ab7445?w=300&h=200&fit=crop
https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcTl6UCwjaicHkv5XB5Zus8QkIoFrEz0EIiNCw&s
https://i.imgur.com/example.jpg
https://example.com/food-image.jpg
```

### 無效的 URL 範例：
```
http://example.com/document.pdf  (不是圖片格式)
ftp://example.com/image.jpg      (不支援 FTP 協議)
/local/path/image.jpg            (本地路徑)
```

## 使用步驟

### 添加新食品：
1. 點擊「添加食品」按鈕
2. 填寫食品基本資訊
3. 在「照片 URL」欄位輸入圖片網址
4. 點擊「預覽」按鈕查看圖片
5. 確認無誤後點擊「確定新增」

### 編輯現有食品：
1. 在食品卡片上點擊「編輯」按鈕
2. 修改「照片 URL」欄位
3. 點擊「預覽」按鈕查看新圖片
4. 點擊「儲存變更」完成更新

## 注意事項

1. **網路連線**：需要穩定的網路連線才能載入圖片
2. **圖片大小**：建議使用適中大小的圖片以提升載入速度
3. **URL 有效性**：請確保圖片 URL 可以公開存取
4. **快取機制**：系統會快取已載入的圖片以提升效能
5. **錯誤處理**：如果圖片載入失敗，會顯示錯誤提示

## 疑難排解

### 圖片無法顯示？
1. 檢查網路連線是否正常
2. 確認圖片 URL 是否正確
3. 檢查圖片是否為支援的格式
4. 確認圖片 URL 可以在瀏覽器中正常開啟

### 預覽按鈕無法點擊？
1. 確認已輸入 URL
2. 檢查 URL 格式是否正確（需包含 http:// 或 https://）
3. 確認 URL 包含支援的圖片副檔名

## 技術實現

- 使用 WPF 的 `BitmapImage` 類別載入網路圖片
- 實現了 URL 驗證和圖片格式檢查
- 支援圖片快取以提升效能
- 包含錯誤處理和回退機制