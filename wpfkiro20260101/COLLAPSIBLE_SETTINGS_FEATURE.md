# 可摺疊設定功能實現

## 功能描述
為設定頁面添加可摺疊的區塊功能，讓使用者可以收合不常用的設定區域，提升介面的整潔度和使用體驗。

## 實現的功能

### 1. 後端服務設定摺疊
- **摺疊區塊**: 後端服務選擇區域
- **觸發方式**: 點擊標題區域
- **視覺指示**: 箭頭圖示（▼ 展開 / ▶ 收合）
- **預設狀態**: 展開

### 2. 連線設定摺疊
- **摺疊區塊**: 連線參數設定區域
- **觸發方式**: 點擊標題區域
- **視覺指示**: 箭頭圖示（▼ 展開 / ▶ 收合）
- **預設狀態**: 展開

### 3. Appwrite Table ID 設定
- **新增欄位**: Food Table ID 和 Subscription Table ID
- **顯示條件**: 僅在選擇 Appwrite 後端服務時顯示
- **用途**: 指定 Appwrite 中食品和訂閱資料的 Collection ID

## 技術實現

### XAML 結構

#### 可摺疊標題區域
```xml
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
```

#### 可摺疊內容區域
```xml
<StackPanel x:Name="BackendServiceContent" Margin="0,10,0,0">
    <!-- 後端服務選項內容 -->
</StackPanel>
```

### C# 摺疊邏輯

#### 後端服務設定摺疊
```csharp
private void BackendServiceHeader_Click(object sender, RoutedEventArgs e)
{
    if (BackendServiceContent != null && BackendExpandIcon != null)
    {
        if (BackendServiceContent.Visibility == Visibility.Visible)
        {
            BackendServiceContent.Visibility = Visibility.Collapsed;
            BackendExpandIcon.Text = "▶";
        }
        else
        {
            BackendServiceContent.Visibility = Visibility.Visible;
            BackendExpandIcon.Text = "▼";
        }
    }
}
```

#### 連線設定摺疊
```csharp
private void ConnectionSettingsHeader_Click(object sender, RoutedEventArgs e)
{
    if (ConnectionSettingsContent != null && ConnectionExpandIcon != null)
    {
        if (ConnectionSettingsContent.Visibility == Visibility.Visible)
        {
            ConnectionSettingsContent.Visibility = Visibility.Collapsed;
            ConnectionExpandIcon.Text = "▶";
        }
        else
        {
            ConnectionSettingsContent.Visibility = Visibility.Visible;
            ConnectionExpandIcon.Text = "▼";
        }
    }
}
```

### Appwrite Table ID 設定

#### 新增的 XAML 欄位
```xml
<!-- Food Collection ID (Appwrite 專用) -->
<TextBlock x:Name="FoodCollectionIdLabel"
           Text="Food Table ID:" 
           FontWeight="Bold" 
           Visibility="Collapsed"/>
<TextBox x:Name="FoodCollectionIdTextBox" 
         Visibility="Collapsed"/>

<!-- Subscription Collection ID (Appwrite 專用) -->
<TextBlock x:Name="SubscriptionCollectionIdLabel"
           Text="Subscription Table ID:" 
           FontWeight="Bold" 
           Visibility="Collapsed"/>
<TextBox x:Name="SubscriptionCollectionIdTextBox" 
         Visibility="Collapsed"/>
```

#### 動態顯示邏輯
```csharp
case BackendServiceType.Appwrite:
    // 顯示 Appwrite 專用欄位
    FoodCollectionIdLabel.Visibility = System.Windows.Visibility.Visible;
    FoodCollectionIdTextBox.Visibility = System.Windows.Visibility.Visible;
    SubscriptionCollectionIdLabel.Visibility = System.Windows.Visibility.Visible;
    SubscriptionCollectionIdTextBox.Visibility = System.Windows.Visibility.Visible;
    break;
default:
    // 隱藏 Appwrite 專用欄位
    FoodCollectionIdLabel.Visibility = System.Windows.Visibility.Collapsed;
    FoodCollectionIdTextBox.Visibility = System.Windows.Visibility.Collapsed;
    SubscriptionCollectionIdLabel.Visibility = System.Windows.Visibility.Collapsed;
    SubscriptionCollectionIdTextBox.Visibility = System.Windows.Visibility.Collapsed;
    break;
```

#### 資料載入和保存
```csharp
// LoadSettings 方法中
FoodCollectionIdTextBox.Text = settings.FoodCollectionId;
SubscriptionCollectionIdTextBox.Text = settings.SubscriptionCollectionId;

// SaveSettings_Click 方法中
settings.FoodCollectionId = FoodCollectionIdTextBox.Text;
settings.SubscriptionCollectionId = SubscriptionCollectionIdTextBox.Text;
```

## 使用者體驗改善

### 視覺設計
- **一致的摺疊指示**: 使用統一的箭頭圖示
- **滑鼠游標變化**: 標題區域顯示手型游標
- **平滑的視覺回饋**: 即時的展開/收合狀態變化

### 操作便利性
- **點擊區域**: 整個標題區域都可點擊
- **狀態記憶**: 摺疊狀態在使用期間保持
- **邏輯分組**: 相關設定項目分組顯示

### 空間利用
- **減少視覺雜亂**: 不常用的設定可以收合
- **聚焦重點**: 使用者可專注於當前需要的設定
- **響應式佈局**: 適應不同的螢幕尺寸

## 相容性

### 後端服務支援
- ✅ Appwrite（包含 Table ID 設定）
- ✅ Supabase
- ✅ Back4App
- ✅ MySQL
- ✅ Contentful
- ✅ NHost
- ✅ Strapi
- ✅ Sanity

### 設定項目
- **通用設定**: API URL、Project ID、API Key
- **Appwrite 專用**: Database ID、Bucket ID、Food Table ID、Subscription Table ID
- **動態顯示**: 根據選擇的後端服務自動調整顯示的欄位

## 錯誤處理

### 摺疊功能異常
```csharp
try
{
    // 摺疊邏輯
}
catch (Exception ex)
{
    ShowStatusMessage($"切換顯示狀態時發生錯誤：{ex.Message}", Brushes.Red);
}
```

### 欄位顯示異常
- 如果控制項不存在，不會影響其他功能
- 使用 null 檢查確保程式穩定性
- 提供適當的錯誤訊息

## 測試建議

### 功能測試
1. **摺疊操作**: 測試點擊標題區域的摺疊/展開功能
2. **狀態保持**: 確認摺疊狀態在操作期間保持
3. **視覺指示**: 驗證箭頭圖示正確變化
4. **欄位顯示**: 測試不同後端服務的欄位顯示邏輯

### 相容性測試
1. **後端服務切換**: 測試切換不同後端服務時的欄位變化
2. **資料保存**: 確認 Table ID 欄位正確保存和載入
3. **設定檔相容**: 測試與現有設定檔的相容性

## 未來改進

### 可能的增強功能
1. **動畫效果**: 添加平滑的展開/收合動畫
2. **狀態記憶**: 記住使用者的摺疊偏好設定
3. **鍵盤操作**: 支援鍵盤快捷鍵操作
4. **更多摺疊區塊**: 為其他設定區域添加摺疊功能