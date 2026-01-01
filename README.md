# 鋒兄AI資訊系統 (WPF版本)

一個基於WPF技術構建的現代化資訊管理系統，提供智能化的影片、圖片、訂閱和食品管理功能。

## 🌟 系統特色

- **現代化UI設計**：採用漸變背景和卡片式布局
- **多功能模塊**：涵蓋影片庫、圖片庫、訂閱管理、食品管理等
- **響應式界面**：適配不同屏幕尺寸
- **智能管理**：支援智能分類和快速搜尋

## 📋 功能模塊

### 🏠 首頁 (HomePage)
- 系統介紹和版權信息
- 技術棧展示（前端：SolidJS + Tailwind CSS，後端：Strapi CMS）
- 功能模塊快速導航

### 📊 儀表板 (DashboardPage)
- 系統統計數據實時監控
- 訂閱管理統計（總數、即將到期、已過期）
- 食品管理統計（庫存、保質期提醒）
- 智能提醒系統

### 🎬 影片庫 (VideosPage)
- 影片收藏和管理
- 支援多種格式（MP4等）
- 影片信息展示（時長、大小、格式）
- 搜尋和篩選功能

### 🖼️ 圖片庫 (PhotosPage)
- 圖片收藏管理（支援241張圖片）
- 多格式支援（JPG、PNG、GIF、JPEG）
- 智能分類和標籤系統
- 批量操作和統計信息

### 📋 訂閱管理 (SubscriptionPage)
- 各類服務訂閱追蹤
- 到期提醒和自動計算剩餘天數
- 分類管理（影音娛樂、軟體等）
- 價格和付款周期管理

### 🍎 食品管理 (FoodPage)
- 食品保質期管理
- 智能到期提醒
- 庫存數量追蹤
- 分類和標籤系統

## 🛠️ 技術棧

- **框架**：.NET 10.0 + WPF
- **語言**：C# 12.0
- **UI技術**：XAML + 現代化設計
- **架構**：MVVM模式
- **版本控制**：Git

## 🚀 快速開始

### 環境要求
- .NET 10.0 SDK
- Visual Studio 2022 或 Visual Studio Code
- Windows 10/11

### 安裝步驟

1. **克隆倉庫**
   ```bash
   git clone https://github.com/goldshoot0720/wpfapikiro20260101.git
   cd wpfapikiro20260101
   ```

2. **還原依賴**
   ```bash
   dotnet restore
   ```

3. **編譯項目**
   ```bash
   dotnet build
   ```

4. **運行應用**
   ```bash
   dotnet run --project wpfkiro20260101
   ```

## 📁 項目結構

```
wpfkiro20260101/
├── wpfkiro20260101/
│   ├── MainWindow.xaml          # 主窗口
│   ├── MainWindow.xaml.cs       # 主窗口邏輯
│   ├── HomePage.xaml            # 首頁
│   ├── DashboardPage.xaml       # 儀表板
│   ├── VideosPage.xaml          # 影片庫
│   ├── PhotosPage.xaml          # 圖片庫
│   ├── SubscriptionPage.xaml    # 訂閱管理
│   ├── FoodPage.xaml            # 食品管理
│   ├── App.xaml                 # 應用程序配置
│   └── wpfkiro20260101.csproj   # 項目文件
├── README.md                    # 項目說明
└── wpfkiro20260101.slnx        # 解決方案文件
```

## 🎨 界面預覽

### 主界面特色
- **漸變背景**：藍紫色漸變，現代感十足
- **側邊導航**：清晰的功能模塊導航
- **卡片布局**：信息展示清晰有序
- **響應式設計**：適配不同屏幕尺寸

### 功能亮點
- **智能統計**：實時數據監控和分析
- **到期提醒**：自動計算和提醒功能
- **搜尋篩選**：快速定位所需內容
- **批量操作**：提高管理效率

## 🔧 開發說明

### 添加新功能模塊
1. 創建新的Page文件（.xaml + .xaml.cs）
2. 在MainWindow.xaml中添加導航按鈕
3. 在MainWindow.xaml.cs中添加導航邏輯
4. 實現具體的功能邏輯

### 自定義樣式
- 修改Page.Resources中的樣式定義
- 調整漸變背景色彩
- 自定義卡片和按鈕樣式

## 📄 版權信息

**鋒兄達習公開資訊 © 版權所有 2025 - 2125**

### 技術支持
- **前端技術**：SolidJS (SolidStart)、Netlify、Tailwind CSS
- **後端技術**：Strapi CMS、RESTful API

## 🤝 貢獻指南

歡迎提交Issue和Pull Request來改進這個項目！

1. Fork 這個倉庫
2. 創建你的功能分支 (`git checkout -b feature/AmazingFeature`)
3. 提交你的更改 (`git commit -m 'Add some AmazingFeature'`)
4. 推送到分支 (`git push origin feature/AmazingFeature`)
5. 打開一個Pull Request

## 📞 聯繫方式

如有問題或建議，請通過以下方式聯繫：

- GitHub Issues: [提交問題](https://github.com/goldshoot0720/wpfapikiro20260101/issues)
- 項目維護者: goldshoot0720

---

**感謝使用鋒兄AI資訊系統！** 🎉