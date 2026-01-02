using System;
using System.Windows;

namespace wpfkiro20260101
{
    public class TestCreateProfileDialog
    {
        public static void ShowTestDialog()
        {
            try
            {
                Console.WriteLine("=== 測試新增設定檔對話框 ===");
                
                // 創建並顯示對話框
                var dialog = new CreateProfileDialog();
                
                // 設定擁有者窗口（如果有的話）
                if (Application.Current.MainWindow != null)
                {
                    dialog.Owner = Application.Current.MainWindow;
                }
                
                Console.WriteLine("顯示對話框...");
                var result = dialog.ShowDialog();
                
                if (result == true)
                {
                    Console.WriteLine($"✓ 用戶點擊確定");
                    Console.WriteLine($"  設定檔名稱: {dialog.ProfileName}");
                    Console.WriteLine($"  描述: {dialog.Description}");
                }
                else
                {
                    Console.WriteLine("✗ 用戶取消操作");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ 測試對話框時發生錯誤: {ex.Message}");
                Console.WriteLine($"詳細錯誤: {ex}");
            }
        }
        
        public static void ShowEditTestDialog()
        {
            try
            {
                Console.WriteLine("=== 測試編輯設定檔對話框 ===");
                
                // 創建測試設定檔
                var testProfile = new Models.SettingsProfile
                {
                    Id = "test-id",
                    ProfileName = "測試設定檔",
                    Description = "這是一個測試設定檔",
                    BackendService = BackendServiceType.Appwrite
                };
                
                // 創建並顯示編輯對話框
                var dialog = new CreateProfileDialog(testProfile);
                
                // 設定擁有者窗口（如果有的話）
                if (Application.Current.MainWindow != null)
                {
                    dialog.Owner = Application.Current.MainWindow;
                }
                
                Console.WriteLine("顯示編輯對話框...");
                var result = dialog.ShowDialog();
                
                if (result == true)
                {
                    Console.WriteLine($"✓ 用戶點擊更新");
                    Console.WriteLine($"  設定檔名稱: {dialog.ProfileName}");
                    Console.WriteLine($"  描述: {dialog.Description}");
                }
                else
                {
                    Console.WriteLine("✗ 用戶取消編輯");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ 測試編輯對話框時發生錯誤: {ex.Message}");
                Console.WriteLine($"詳細錯誤: {ex}");
            }
        }
    }
}