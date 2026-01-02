using System;
using System.IO;
using System.Windows;
using Microsoft.Win32;

namespace wpfkiro20260101
{
    public static class FolderSelectDialog
    {
        public static string? SelectFolder(string title = "選擇資料夾", string initialPath = "")
        {
            try
            {
                // 使用 SaveFileDialog 的技巧來選擇資料夾
                var saveFileDialog = new SaveFileDialog
                {
                    Title = title,
                    Filter = "資料夾|*.folder",
                    FileName = "選擇此資料夾",
                    InitialDirectory = string.IsNullOrEmpty(initialPath) 
                        ? Environment.GetFolderPath(Environment.SpecialFolder.Desktop) 
                        : initialPath
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    return Path.GetDirectoryName(saveFileDialog.FileName);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"選擇資料夾時發生錯誤：{ex.Message}", "錯誤", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            return null;
        }

        public static string? SelectFolderWithMessage(string title = "選擇匯出資料夾")
        {
            var result = MessageBox.Show(
                "請在接下來的對話框中選擇要匯出設定檔的資料夾。\n\n" +
                "提示：在檔案對話框中，您只需要選擇想要的資料夾位置，" +
                "然後點擊「儲存」即可選擇該資料夾。",
                title,
                MessageBoxButton.OKCancel,
                MessageBoxImage.Information);

            if (result == MessageBoxResult.OK)
            {
                return SelectFolder(title);
            }

            return null;
        }
    }
}