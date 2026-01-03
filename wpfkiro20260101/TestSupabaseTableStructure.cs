using System;
using System.Threading.Tasks;
using System.Windows;
using MessageBox = System.Windows.MessageBox;

namespace wpfkiro20260101
{
    public class TestSupabaseTableStructure
    {
        public static async Task RunTest()
        {
            try
            {
                Console.WriteLine("=== 測試 Supabase 表結構修正 ===");
                
                // 運行診斷
                await SupabaseTableStructureFix.RunDiagnosis();
                
                Console.WriteLine("\n=== 測試完成 ===");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"測試失敗: {ex.Message}");
                MessageBox.Show($"測試失敗: {ex.Message}", "錯誤", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}