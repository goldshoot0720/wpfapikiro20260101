# Back4App 設定指南

Back4App 是一個基於 Parse 的後端即服務平台，提供完整的資料庫和 API 功能。

## 免費方案包含

- 25,000 API 請求/月
- 1GB 資料庫儲存空間
- 1GB 檔案儲存空間
- 10GB 資料傳輸

## 設定步驟

### 1. 註冊 Back4App 帳號
1. 前往 [Back4App 官網](https://www.back4app.com/)
2. 點擊 "Sign Up" 註冊新帳號
3. 驗證您的電子郵件地址

### 2. 建立新應用程式
1. 登入 Back4App 控制台
2. 點擊 "Build new app"
3. 選擇 "Backend as a Service"
4. 輸入應用程式名稱
5. 選擇資料中心位置（建議選擇離您最近的位置）

### 3. 取得 API 金鑰
1. 在應用程式控制台中，點擊左側選單的 "App Settings"
2. 選擇 "Security & Keys"
3. 複製以下資訊：
   - **Application ID**：這是您的 App ID
   - **REST API Key**：這是您的 API Key

### 4. 在應用程式中設定
1. 開啟系統設定頁面
2. 選擇 "Back4App" 作為後端服務
3. 填入以下資訊：
   - **API URL**: `https://parseapi.back4app.com`（預設值）
   - **App ID**: 您的 Application ID
   - **API Key**: 您的 REST API Key
4. 點擊 "測試連線" 確認設定正確
5. 點擊 "儲存設定"

## API 端點

Back4App 使用 Parse REST API，主要端點包括：

- **基礎 URL**: `https://parseapi.back4app.com`
- **物件操作**: `/classes/{className}`
- **用戶管理**: `/users`
- **檔案上傳**: `/files`
- **雲端函數**: `/functions/{functionName}`

## 認證標頭

所有 API 請求都需要包含以下標頭：

```
X-Parse-Application-Id: {您的 Application ID}
X-Parse-REST-API-Key: {您的 REST API Key}
Content-Type: application/json
```

## 資料庫結構

Back4App 使用 NoSQL 資料庫（基於 MongoDB），支援：

- 動態結構定義
- JSON 格式資料
- 關聯式查詢
- 即時查詢
- 地理位置查詢

## 常見問題

### Q: 如何增加 API 請求限制？
A: 可以升級到付費方案，或者優化您的 API 使用方式。

### Q: 支援哪些資料類型？
A: 支援字串、數字、布林值、陣列、物件、日期、檔案、地理位置等。

### Q: 如何備份資料？
A: 在控制台的 "Database" 頁面可以匯出資料。

## 相關連結

- [Back4App 官方文件](https://www.back4app.com/docs/)
- [Parse REST API 指南](https://docs.parseplatform.org/rest/guide/)
- [Back4App 控制台](https://dashboard.back4app.com/)