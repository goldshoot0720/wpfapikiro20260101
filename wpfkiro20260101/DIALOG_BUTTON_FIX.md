# 對話框按鈕修復說明

## 問題描述
在「新增設定檔」對話框中，確定按鈕沒有正確顯示或無法點擊。

## 修復內容

### 1. 按鈕可見性修復
- 明確設定按鈕的 `Visibility="Visible"`
- 增加按鈕的固定寬度和高度 (`Width="80" Height="35"`)
- 調整按鈕區域的邊距，確保有足夠空間顯示

### 2. 按鈕啟用狀態修復
- 修改初始狀態為 `IsEnabled="True"`
- 簡化驗證邏輯，只要有輸入名稱就啟用按鈕
- 在對話框初始化時確保按鈕狀態正確

### 3. 按鈕文字修復
- 將「儲存」改為「確定」，更符合對話框慣例
- 編輯模式下顯示「更新」

### 4. 窗口大小調整
- 將對話框高度從 350 增加到 400，確保所有內容都能顯示
- 增加按鈕區域的邊距

## 修復的檔案

### CreateProfileDialog.xaml
```xml
<!-- 按鈕區域 -->
<StackPanel Orientation="Horizontal" 
            HorizontalAlignment="Right"
            Margin="0,30,0,10">
    <Button x:Name="CancelButton" 
            Content="取消" 
            Style="{StaticResource CancelButtonStyle}"
            Width="80"
            Height="35"
            Margin="0,0,15,0"
            Visibility="Visible"
            Click="Cancel_Click"/>
    <Button x:Name="SaveButton" 
            Content="確定" 
            Style="{StaticResource ModernButtonStyle}"
            Width="80"
            Height="35"
            Visibility="Visible"
            IsEnabled="True"
            Click="Save_Click"/>
</StackPanel>
```

### CreateProfileDialog.xaml.cs
- 修改 `ValidateInput()` 方法，簡化按鈕啟用邏輯
- 修改 `InitializeDialog()` 方法，確保按鈕初始狀態正確
- 修改 `OnSourceInitialized()` 方法，確保按鈕可見性

## 測試方法

### 手動測試
1. 開啟應用程式
2. 進入「系統設定」→「設定檔管理」
3. 點擊「💾 儲存當前設定」
4. 確認對話框顯示正確，包含「取消」和「確定」按鈕
5. 輸入設定檔名稱，確認「確定」按鈕可以點擊

### 程式測試
使用 `TestCreateProfileDialog.cs` 中的測試方法：
```csharp
// 測試新增對話框
TestCreateProfileDialog.ShowTestDialog();

// 測試編輯對話框
TestCreateProfileDialog.ShowEditTestDialog();
```

## 預期結果
- 對話框正確顯示「取消」和「確定」按鈕
- 輸入設定檔名稱後，「確定」按鈕變為可點擊狀態
- 點擊「確定」後正確儲存設定檔
- 點擊「取消」後關閉對話框而不儲存

## 注意事項
- 確保按鈕樣式 `ModernButtonStyle` 和 `CancelButtonStyle` 正確定義
- 如果仍有問題，檢查父窗口的設定和 Z-Index
- 確保對話框的 Owner 屬性正確設定