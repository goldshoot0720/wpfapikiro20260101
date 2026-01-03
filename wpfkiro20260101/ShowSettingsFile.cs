using System;
using System.IO;
using System.Windows;
using MessageBox = System.Windows.MessageBox;

namespace wpfkiro20260101
{
    public class ShowSettingsFile
    {
        public static void ShowSettingsFileLocation()
        {
            try
            {
                // 獲取設定檔路徑
                var settingsPath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "wpfkiro20260101",
                    "settings.json"
                );

                var message = $"設定檔位置：\n{settingsPath}\n\n";

                if (File.Exists(settingsPath))
                {
                    message += "設定檔存在！\n\n";
                    
                    // 讀取設定檔內容
                    var content = File.ReadAllText(settingsPath);
                    message += $"設定檔內容：\n{content}";
                }
                else
                {
                    message += "設定檔不存在！";
                }

                // 顯示訊息框
                MessageBox.Show(message, "設定檔資訊", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"顯示設定檔資訊時發生錯誤：{ex.Message}", "錯誤", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public static void OpenSettingsFolder()
        {
            try
            {
                var settingsFolder = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "wpfkiro20260101"
                );

                if (Directory.Exists(settingsFolder))
                {
                    System.Diagnostics.Process.Start("explorer.exe", settingsFolder);
                }
                else
                {
                    MessageBox.Show("設定檔資料夾不存在！", "錯誤", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"開啟設定檔資料夾時發生錯誤：{ex.Message}", "錯誤", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}