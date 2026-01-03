using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using wpfkiro20260101.Services;
using wpfkiro20260101.Models;
using MessageBox = System.Windows.MessageBox;

namespace wpfkiro20260101
{
    public class TestProfileExport
    {
        public static async Task TestExportFunctionality()
        {
            try
            {
                var profileService = SettingsProfileService.Instance;
                
                // 測試 1: 檢查設定檔數量
                var profileCount = profileService.GetProfileCount();
                System.Diagnostics.Debug.WriteLine($"當前設定檔數量: {profileCount}");
                
                if (profileCount == 0)
                {
                    // 創建測試設定檔
                    await CreateTestProfile();
                    profileCount = profileService.GetProfileCount();
                    System.Diagnostics.Debug.WriteLine($"創建測試設定檔後數量: {profileCount}");
                }
                
                // 測試 2: 匯出所有設定檔
                var exportResult = await profileService.ExportProfilesAsync();
                if (exportResult.Success)
                {
                    System.Diagnostics.Debug.WriteLine("✅ 匯出功能測試成功");
                    System.Diagnostics.Debug.WriteLine($"匯出的 JSON 長度: {exportResult.Data?.Length ?? 0} 字元");
                    
                    // 測試 3: 驗證 JSON 格式
                    if (!string.IsNullOrEmpty(exportResult.Data))
                    {
                        try
                        {
                            var profiles = System.Text.Json.JsonSerializer.Deserialize<SettingsProfile[]>(exportResult.Data);
                            System.Diagnostics.Debug.WriteLine($"✅ JSON 格式驗證成功，包含 {profiles?.Length ?? 0} 筆設定檔");
                            
                            // 顯示第一筆設定檔的資訊
                            if (profiles != null && profiles.Length > 0)
                            {
                                var firstProfile = profiles[0];
                                System.Diagnostics.Debug.WriteLine($"第一筆設定檔: {firstProfile.ProfileName} ({firstProfile.BackendService})");
                            }
                        }
                        catch (Exception jsonEx)
                        {
                            System.Diagnostics.Debug.WriteLine($"❌ JSON 格式驗證失敗: {jsonEx.Message}");
                        }
                    }
                    
                    // 測試 4: 測試桌面匯出
                    await TestDesktopExport(exportResult.Data);
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"❌ 匯出功能測試失敗: {exportResult.ErrorMessage}");
                }
                
                // 測試 5: 測試單一設定檔匯出
                await TestSingleProfileExport();
                
                System.Diagnostics.Debug.WriteLine("🎯 設定檔匯出功能測試完成");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ 測試過程中發生錯誤: {ex.Message}");
            }
        }
        
        private static async Task CreateTestProfile()
        {
            try
            {
                var profileService = SettingsProfileService.Instance;
                var testProfile = new SettingsProfile
                {
                    ProfileName = $"測試設定檔_{DateTime.Now:HHmmss}",
                    Description = "自動測試創建的設定檔",
                    BackendService = BackendServiceType.Appwrite,
                    ApiUrl = "https://test.appwrite.io/v1",
                    ProjectId = "test-project-id",
                    ApiKey = "test-api-key",
                    DatabaseId = "test-database-id",
                    BucketId = "test-bucket-id",
                    FoodCollectionId = "food",
                    SubscriptionCollectionId = "subscription"
                };
                
                var result = await profileService.SaveProfileAsync(testProfile);
                if (result.Success)
                {
                    System.Diagnostics.Debug.WriteLine("✅ 測試設定檔創建成功");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"❌ 測試設定檔創建失敗: {result.ErrorMessage}");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ 創建測試設定檔時發生錯誤: {ex.Message}");
            }
        }
        
        private static async Task TestDesktopExport(string jsonData)
        {
            try
            {
                var desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                var fileName = $"測試匯出_{DateTime.Now:yyyyMMdd_HHmmss}.json";
                var filePath = Path.Combine(desktopPath, fileName);
                
                await File.WriteAllTextAsync(filePath, jsonData);
                
                if (File.Exists(filePath))
                {
                    var fileInfo = new FileInfo(filePath);
                    System.Diagnostics.Debug.WriteLine($"✅ 桌面匯出測試成功");
                    System.Diagnostics.Debug.WriteLine($"檔案路徑: {filePath}");
                    System.Diagnostics.Debug.WriteLine($"檔案大小: {fileInfo.Length} 位元組");
                    
                    // 清理測試檔案
                    File.Delete(filePath);
                    System.Diagnostics.Debug.WriteLine("🧹 測試檔案已清理");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("❌ 桌面匯出測試失敗：檔案未創建");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ 桌面匯出測試失敗: {ex.Message}");
            }
        }
        
        private static async Task TestSingleProfileExport()
        {
            try
            {
                var profileService = SettingsProfileService.Instance;
                var profiles = await profileService.GetAllProfilesAsync();
                
                if (profiles.Count > 0)
                {
                    var firstProfile = profiles[0];
                    var result = await profileService.ExportProfilesAsync(new List<string> { firstProfile.Id });
                    
                    if (result.Success)
                    {
                        System.Diagnostics.Debug.WriteLine("✅ 單一設定檔匯出測試成功");
                        System.Diagnostics.Debug.WriteLine($"匯出設定檔: {firstProfile.ProfileName}");
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine($"❌ 單一設定檔匯出測試失敗: {result.ErrorMessage}");
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("⚠️ 沒有設定檔可供單一匯出測試");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ 單一設定檔匯出測試失敗: {ex.Message}");
            }
        }
        
        public static void ShowExportGuide()
        {
            var message = @"設定檔匯出功能使用指南：

🎯 功能概述：
• 匯出設定檔：將您的連線設定保存為 JSON 檔案
• 編輯一致性：編輯設定檔時自動與當前連線設定同步
• 多種匯出方式：快速匯出、選擇資料夾匯出、選擇性匯出

📤 匯出方式：
1. 快速匯出（推薦）：
   - 點擊設定頁面的「📤 快速匯出」按鈕
   - 選擇保存位置和檔名
   - 預設位置為桌面，可自由選擇

2. 選擇資料夾匯出：
   - 點擊「📁 選擇資料夾」按鈕
   - 選擇要匯出的資料夾
   - 自動生成帶時間戳記的檔名

3. 管理匯出：
   - 點擊「📁 管理設定檔」開啟管理視窗
   - 「📤 匯出設定檔」：選擇位置匯出全部
   - 「📁 選擇資料夾匯出」：選擇資料夾匯出全部
   - 「📤 匯出選中」：匯出特定設定檔
   - 「⚡ 快速匯出全部」：選擇位置快速匯出

✏️ 編輯一致性：
• 編輯設定檔時可選擇「與當前連線設定保持同步」
• 自動載入當前連線設定到編輯表單
• 編輯完成後提示是否重新載入以保持一致性

📁 檔案格式：
• JSON 格式，包含所有設定檔資訊
• 可用於備份、分享或遷移設定
• 支援重新匯入到其他裝置

🔧 故障排除：
• 如果匯出失敗，請檢查磁碟空間和權限
• 確保選擇的資料夾有寫入權限
• 檔案名稱包含時間戳記避免衝突

💡 使用建議：
• 定期備份重要設定檔
• 重要變更前先匯出備份
• 使用資料夾匯出功能統一管理備份位置";

            MessageBox.Show(message, "設定檔匯出使用指南", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}