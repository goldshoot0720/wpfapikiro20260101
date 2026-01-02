using System;
using System.Threading.Tasks;
using System.Windows;
using wpfkiro20260101.Models;
using wpfkiro20260101.Services;

namespace wpfkiro20260101
{
    public class TestProfileEditDialog
    {
        public static async Task RunTest()
        {
            try
            {
                Console.WriteLine("=== 測試設定檔編輯對話框 ===");

                // 1. 創建測試設定檔
                var profileService = SettingsProfileService.Instance;
                var testProfile = new SettingsProfile
                {
                    Id = Guid.NewGuid().ToString(),
                    ProfileName = "測試設定檔",
                    Description = "這是一個測試設定檔",
                    BackendService = BackendServiceType.Appwrite,
                    ApiUrl = "https://test.appwrite.io/v1",
                    ProjectId = "test-project",
                    ApiKey = "test-api-key",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                // 2. 儲存測試設定檔
                var saveResult = await profileService.SaveProfileAsync(testProfile);
                if (!saveResult.Success)
                {
                    Console.WriteLine($"儲存測試設定檔失敗: {saveResult.ErrorMessage}");
                    return;
                }
                Console.WriteLine("✓ 測試設定檔已創建");

                // 3. 測試編輯對話框
                Console.WriteLine("正在測試編輯對話框...");
                
                try
                {
                    var dialog = new CreateProfileDialog(testProfile);
                    Console.WriteLine("✓ CreateProfileDialog 創建成功");
                    
                    // 檢查對話框屬性
                    Console.WriteLine($"對話框標題: {dialog.Title}");
                    Console.WriteLine($"是否為編輯模式: {dialog.Title.Contains("編輯")}");
                    
                    // 檢查按鈕狀態
                    Console.WriteLine($"儲存按鈕可見: {dialog.SaveButton.Visibility}");
                    Console.WriteLine($"取消按鈕可見: {dialog.CancelButton.Visibility}");
                    Console.WriteLine($"儲存按鈕啟用: {dialog.SaveButton.IsEnabled}");
                    
                    // 檢查輸入框內容
                    Console.WriteLine($"設定檔名稱: '{dialog.ProfileNameTextBox.Text}'");
                    Console.WriteLine($"描述: '{dialog.DescriptionTextBox.Text}'");
                    
                    dialog.Close();
                    Console.WriteLine("✓ 編輯對話框測試完成");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"✗ CreateProfileDialog 測試失敗: {ex.Message}");
                    
                    // 測試簡化對話框
                    try
                    {
                        var simpleDialog = new SimpleCreateProfileDialog();
                        simpleDialog.ProfileNameTextBox.Text = testProfile.ProfileName;
                        simpleDialog.DescriptionTextBox.Text = testProfile.Description;
                        simpleDialog.Title = "編輯設定檔";
                        
                        Console.WriteLine("✓ SimpleCreateProfileDialog 創建成功");
                        Console.WriteLine($"簡化對話框標題: {simpleDialog.Title}");
                        Console.WriteLine($"設定檔名稱: '{simpleDialog.ProfileNameTextBox.Text}'");
                        
                        simpleDialog.Close();
                        Console.WriteLine("✓ 簡化對話框測試完成");
                    }
                    catch (Exception ex2)
                    {
                        Console.WriteLine($"✗ SimpleCreateProfileDialog 也失敗: {ex2.Message}");
                    }
                }

                // 4. 清理測試資料
                await profileService.DeleteProfileAsync(testProfile.Id);
                Console.WriteLine("✓ 測試資料已清理");

                Console.WriteLine("=== 測試完成 ===");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"測試過程中發生錯誤: {ex.Message}");
                Console.WriteLine($"堆疊追蹤: {ex.StackTrace}");
            }
        }

        public static void TestDialogCreation()
        {
            try
            {
                Console.WriteLine("=== 測試對話框創建 ===");

                // 測試新增對話框
                var newDialog = new CreateProfileDialog();
                Console.WriteLine("✓ 新增對話框創建成功");
                Console.WriteLine($"標題: {newDialog.Title}");
                newDialog.Close();

                // 測試編輯對話框
                var testProfile = new SettingsProfile
                {
                    Id = "test-id",
                    ProfileName = "測試設定檔",
                    Description = "測試描述",
                    BackendService = BackendServiceType.Appwrite
                };

                var editDialog = new CreateProfileDialog(testProfile);
                Console.WriteLine("✓ 編輯對話框創建成功");
                Console.WriteLine($"標題: {editDialog.Title}");
                Console.WriteLine($"設定檔名稱: '{editDialog.ProfileNameTextBox.Text}'");
                editDialog.Close();

                Console.WriteLine("=== 對話框創建測試完成 ===");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"對話框創建測試失敗: {ex.Message}");
                Console.WriteLine($"堆疊追蹤: {ex.StackTrace}");
            }
        }
    }
}