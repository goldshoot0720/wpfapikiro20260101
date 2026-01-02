# 設定頁面折疊功能

## 功能概述

為了提升用戶體驗和節省頁面空間，我們為設定頁面的主要設定區域添加了折疊/展開功能。現在支援兩個主要區域的折疊：

1. **後端服務設定** - 選擇後端服務提供商
2. **連線設定** - 配置API連線參數

## 功能特點

### 🎯 主要特性
- **可點擊標題**：點擊區域標題即可折疊或展開設定內容
- **視覺指示器**：使用 ▼ 和 ▶ 圖示清楚顯示當前狀態
- **滑鼠互動**：標題區域滑鼠游標變為手型，提示可點擊
- **預設展開**：首次載入時所有設定區域預設為展開狀態
- **獨立控制**：每個區域可以獨立折疊/展開

### 🎨 用戶界面改進
- **節省空間**：折疊後可以節省大量垂直空間
- **更好的組織**：讓用戶可以專注於需要的設定區域
- **直觀操作**：簡單的點擊操作，符合用戶習慣
- **統一設計**：所有折疊區域使用相同的視覺設計

## 支援的折疊區域

### 1. 後端服務設定
包含所有8個後端服務選項：
- **Appwrite** (2GB 容量, 5GB 流量)
- **Supabase** (1GB 容量, 5GB 流量)
- **NHost**
- **Contentful**
- **Back4App**
- **MySQL**
- **Strapi**
- **Sanity**

### 2. 連線設定
包含所有連線配置欄位：
- API Endpoint
- Project ID
- Database ID (Appwrite 專用)
- Bucket ID (Appwrite 專用)
- Food Table ID (Appwrite 專用)
- Subscription Table ID (Appwrite 專用)
- API Key

## 技術實現

### XAML 結構範例
```xml
<!-- 後端服務設定 -->
<Border Style="{StaticResource SettingsCardStyle}">
    <StackPanel>
        <!-- 可點擊的標題區域 -->
        <Border Background="Transparent" 
                Cursor="Hand" 
                MouseLeftButtonDown="BackendServiceHeader_Click">
            <StackPanel Orientation="Horizontal">
                <TextBlock x:Name="BackendExpandIcon" 
                           Text="▼" 
                           FontSize="14" 
                           Foreground="#6B7280" 
                           Margin="0,0,10,0" 
                           VerticalAlignment="Center"/>
                <TextBlock Text="後端服務設定" Style="{StaticResource SectionTitleStyle}" Margin="0"/>
            </StackPanel>
        </Border>
        
        <!-- 可折疊的後端服務設定內容 -->
        <StackPanel x:Name="BackendServiceContent" Margin="0,10,0,0">
            <!-- 所有後端服務選項 -->
        </StackPanel>
    </StackPanel>
</Border>

<!-- 連線設定 -->
<Border Style="{StaticResource SettingsCardStyle}">
    <StackPanel>
        <!-- 可點擊的標題區域 -->
        <Border Background="Transparent" 
                Cursor="Hand" 
                MouseLeftButtonDown="ConnectionSettingsHeader_Click">
            <StackPanel Orientation="Horizontal">
                <TextBlock x:Name="ConnectionExpandIcon" 
                           Text="▼" 
                           FontSize="14" 
                           Foreground="#6B7280" 
                           Margin="0,0,10,0" 
                           VerticalAlignment="Center"/>
                <TextBlock Text="連線設定" Style="{StaticResource SectionTitleStyle}" Margin="0"/>
            </StackPanel>
        </Border>
        
        <!-- 可折疊的連線設定內容 -->
        <Grid x:Name="ConnectionSettingsContent" Margin="0,10,0,0">
            <!-- 所有連線設定欄位 -->
        </Grid>
    </StackPanel>
</Border>
```

### C# 事件處理
```csharp
// 後端服務設定折疊/展開功能
private void BackendServiceHeader_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
{
    try
    {
        if (BackendServiceContent.Visibility == System.Windows.Visibility.Visible)
        {
            // 折疊
            BackendServiceContent.Visibility = System.Windows.Visibility.Collapsed;
            BackendExpandIcon.Text = "▶";
        }
        else
        {
            // 展開
            BackendServiceContent.Visibility = System.Windows.Visibility.Visible;
            BackendExpandIcon.Text = "▼";
        }
    }
    catch (Exception ex)
    {
        ShowStatusMessage($"切換後端服務設定顯示狀態時發生錯誤：{ex.Message}", Brushes.Red);
    }
}

// 連線設定折疊/展開功能
private void ConnectionSettingsHeader_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
{
    try
    {
        if (ConnectionSettingsContent.Visibility == System.Windows.Visibility.Visible)
        {
            // 折疊
            ConnectionSettingsContent.Visibility = System.Windows.Visibility.Collapsed;
            ConnectionExpandIcon.Text = "▶";
        }
        else
        {
            // 展開
            ConnectionSettingsContent.Visibility = System.Windows.Visibility.Visible;
            ConnectionExpandIcon.Text = "▼";
        }
    }
    catch (Exception ex)
    {
        ShowStatusMessage($"切換連線設定顯示狀態時發生錯誤：{ex.Message}", Brushes.Red);
    }
}
```

## 使用方式

### 後端服務設定
1. **展開設定**：點擊「後端服務設定」標題，看到 ▼ 圖示時表示內容已展開
2. **折疊設定**：再次點擊標題，看到 ▶ 圖示時表示內容已折疊
3. **選擇服務**：展開後可以選擇任一後端服務並查看其特性

### 連線設定
1. **展開設定**：點擊「連線設定」標題，看到 ▼ 圖示時表示內容已展開
2. **折疊設定**：再次點擊標題，看到 ▶ 圖示時表示內容已折疊
3. **配置連線**：展開後可以配置所選後端服務的連線參數

### 狀態保持
- 在同一個會話中，每個區域的折疊狀態會獨立保持
- 重新載入頁面時，所有區域預設為展開狀態

## 相容性

- ✅ 與現有的所有後端服務設定相容
- ✅ 不影響設定的儲存和載入功能
- ✅ 保持所有現有的驗證和測試功能
- ✅ 支援所有 Windows 版本
- ✅ 與設定檔管理功能完全相容

## 未來改進

可以考慮的增強功能：
- 記住用戶的折疊偏好設定並持久化
- 添加平滑的動畫效果
- 為其他設定區域（資料匯出、設定檔管理）也添加折疊功能
- 鍵盤快捷鍵支援（如 Space 或 Enter）
- 全部展開/折疊的快捷按鈕

## 測試

使用 `TestCollapsibleSettings.cs` 文件來測試所有折疊功能：

```csharp
// 測試所有折疊功能
TestCollapsibleSettings.RunAllTests();

// 或單獨測試
TestCollapsibleSettings.TestCollapsibleConnectionSettings();
TestCollapsibleSettings.TestCollapsibleBackendServiceSettings();
```

## 效益總結

這個功能讓設定頁面更加整潔和用戶友好：

1. **空間效率**：用戶可以折疊不需要的區域，專注於當前任務
2. **更好的組織**：清晰的區域劃分，便於快速定位所需設定
3. **提升體驗**：減少滾動需求，提高操作效率
4. **視覺清晰**：統一的折疊指示器，直觀易懂

特別適合以下使用場景：
- 已經配置好後端服務，只需要調整連線參數
- 需要在多個後端服務之間快速切換
- 在小螢幕或低解析度環境下使用