using System;
using System.Threading.Tasks;
using System.Windows;
using wpfkiro20260101.Models;
using wpfkiro20260101.Services;

namespace wpfkiro20260101
{
    public class FixProfileEditDialog
    {
        public static async Task DiagnoseAndFix()
        {
            try
            {
                Console.WriteLine("=== 診斷設定檔編輯對話框問題 ===");

                // 1. 檢查設定檔服務
                var profileService = SettingsProfileService.Instance;
                var profiles = await profileService.GetAllProfilesAsync();
                Console.WriteLine($"✓ 設定檔服務正常，共有 {profiles.Count} 個設定檔");

                // 2. 檢查 AppSettings
                var settings = AppSettings.Instance;
                Console.WriteLine($"✓ AppSettings 正常，當前服務: {settings.GetServiceDisplayName()}");

                // 3. 測試對話框創建
                Console.WriteLine("正在測試對話框創建...");
                
                // 創建測試設定檔
                var testProfile = new SettingsProfile
                {
                    Id = "test-profile-id",
                    ProfileName = "測試設定檔",
                    Description = "用於測試的設定檔",
                    BackendService = BackendServiceType.Appwrite,
                    ApiUrl = "https://test.appwrite.io/v1",
                    ProjectId = "test-project-id",
                    ApiKey = "test-api-key"
                };

                // 測試編輯對話框
                try
                {
                    var editDialog = new CreateProfileDialog(testProfile);
                    Console.WriteLine("✓ 編輯對話框創建成功");
                    
                    // 檢查對話框狀態
                    Console.WriteLine($"  - 標題: {editDialog.DialogTitle.Text}");
                    Console.WriteLine($"  - 設定檔名稱: '{editDialog.ProfileNameTextBox.Text}'");
                    Console.WriteLine($"  - 描述: '{editDialog.DescriptionTextBox.Text}'");
                    Console.WriteLine($"  - 儲存按鈕內容: {editDialog.SaveButton.Content}");
                    Console.WriteLine($"  - 儲存按鈕可見: {editDialog.SaveButton.Visibility}");
                    Console.WriteLine($"  - 儲存按鈕啟用: {editDialog.SaveButton.IsEnabled}");
                    Console.WriteLine($"  - 取消按鈕可見: {editDialog.CancelButton.Visibility}");
                    
                    editDialog.Close();
                    Console.WriteLine("✓ 編輯對話框測試完成");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"✗ 編輯對話框創建失敗: {ex.Message}");
                    Console.WriteLine($"  堆疊追蹤: {ex.StackTrace}");
                    
                    // 嘗試使用簡化對話框
                    try
                    {
                        var simpleDialog = new SimpleCreateProfileDialog();
                        simpleDialog.ProfileNameTextBox.Text = testProfile.ProfileName;
                        simpleDialog.DescriptionTextBox.Text = testProfile.Description;
                        simpleDialog.Title = "編輯設定檔";
                        
                        Console.WriteLine("✓ 簡化對話框創建成功");
                        Console.WriteLine($"  - 標題: {simpleDialog.Title}");
                        Console.WriteLine($"  - 設定檔名稱: '{simpleDialog.ProfileNameTextBox.Text}'");
                        Console.WriteLine($"  - 描述: '{simpleDialog.DescriptionTextBox.Text}'");
                        
                        simpleDialog.Close();
                        Console.WriteLine("✓ 簡化對話框可以作為備用方案");
                    }
                    catch (Exception ex2)
                    {
                        Console.WriteLine($"✗ 簡化對話框也失敗: {ex2.Message}");
                    }
                }

                // 4. 測試新增對話框
                try
                {
                    var newDialog = new CreateProfileDialog();
                    Console.WriteLine("✓ 新增對話框創建成功");
                    Console.WriteLine($"  - 標題: {newDialog.DialogTitle.Text}");
                    Console.WriteLine($"  - 儲存按鈕內容: {newDialog.SaveButton.Content}");
                    newDialog.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"✗ 新增對話框創建失敗: {ex.Message}");
                }

                Console.WriteLine("=== 診斷完成 ===");
                
                // 5. 提供修復建議
                Console.WriteLine("\n=== 修復建議 ===");
                Console.WriteLine("1. 如果編輯對話框無法正常顯示，系統會自動使用簡化版本");
                Console.WriteLine("2. 檢查 XAML 樣式是否有衝突");
                Console.WriteLine("3. 確保所有必要的 using 語句都已包含");
                Console.WriteLine("4. 檢查對話框的初始化順序");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"診斷過程中發生錯誤: {ex.Message}");
                Console.WriteLine($"堆疊追蹤: {ex.StackTrace}");
            }
        }

        public static void ShowEditDialogSafely(SettingsProfile profile, Window owner)
        {
            try
            {
                var dialog = new CreateProfileDialog(profile);
                dialog.Owner = owner;
                dialog.ShowDialog();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"主要編輯對話框失敗，使用備用方案: {ex.Message}");
                
                // 使用簡化對話框作為備用
                var simpleDialog = new SimpleCreateProfileDialog();
                simpleDialog.Owner = owner;
                simpleDialog.ProfileNameTextBox.Text = profile.ProfileName;
                simpleDialog.DescriptionTextBox.Text = profile.Description;
                simpleDialog.Title = "編輯設定檔";
                
                var result = simpleDialog.ShowDialog();
                if (result == true)
                {
                    // 更新設定檔
                    profile.ProfileName = simpleDialog.ProfileName;
                    profile.Description = simpleDialog.Description;
                    
                    // 這裡可以添加儲存邏輯
                    Console.WriteLine($"設定檔已更新: {profile.ProfileName}");
                }
            }
        }
    }
}