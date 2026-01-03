using System;
using System.Threading.Tasks;
using System.Windows;
using wpfkiro20260101.Services;
using MessageBox = System.Windows.MessageBox;

namespace wpfkiro20260101
{
    public class SupabaseTableStructureFix
    {
        public static async Task RunDiagnosis()
        {
            try
            {
                Console.WriteLine("=== Supabase 表結構診斷 ===");
                
                // 確保使用 Supabase 設定
                var settings = AppSettings.Instance;
                if (settings.BackendService != BackendServiceType.Supabase)
                {
                    Console.WriteLine("⚠️ 當前後端服務不是 Supabase，正在切換...");
                    settings.BackendService = BackendServiceType.Supabase;
                    settings.Save();
                }
                
                Console.WriteLine($"✅ 後端服務: {settings.GetServiceDisplayName()}");
                Console.WriteLine($"✅ API URL: {settings.ApiUrl}");
                
                // 創建 Supabase 服務實例
                var supabaseService = new SupabaseService();
                
                Console.WriteLine("\n=== 表結構分析 ===");
                
                // 測試 food 表
                Console.WriteLine("\n--- 測試 food 表 ---");
                var foodResult = await supabaseService.GetFoodsAsync();
                Console.WriteLine($"food 表訪問: {(foodResult.Success ? "✅ 成功" : "❌ 失敗")}");
                if (!foodResult.Success)
                {
                    Console.WriteLine($"錯誤: {foodResult.ErrorMessage}");
                }
                
                // 測試 subscription 表 (單數)
                Console.WriteLine("\n--- 測試 subscription 表 (單數) ---");
                var subscriptionResult = await supabaseService.GetSubscriptionsAsync();
                Console.WriteLine($"subscription 表訪問: {(subscriptionResult.Success ? "✅ 成功" : "❌ 失敗")}");
                if (!subscriptionResult.Success)
                {
                    Console.WriteLine($"錯誤: {subscriptionResult.ErrorMessage}");
                }
                
                Console.WriteLine("\n=== 問題診斷 ===");
                
                if (!foodResult.Success)
                {
                    Console.WriteLine("❌ food 表問題:");
                    Console.WriteLine("   可能原因:");
                    Console.WriteLine("   1. 表不存在 - 需要執行 CREATE_FOOD_TABLE.sql");
                    Console.WriteLine("   2. API 權限問題 - 檢查 API Key 權限");
                    Console.WriteLine("   3. RLS 政策問題 - 檢查 Row Level Security 設定");
                }
                
                if (!subscriptionResult.Success)
                {
                    Console.WriteLine("❌ subscription 表問題:");
                    Console.WriteLine("   可能原因:");
                    Console.WriteLine("   1. 表名不匹配 - 程式使用 'subscription'，但可能創建了 'subscriptions'");
                    Console.WriteLine("   2. 表不存在 - 需要創建 subscription 表");
                    Console.WriteLine("   3. API 權限問題 - 檢查 API Key 權限");
                }
                
                Console.WriteLine("\n=== 建議的修正步驟 ===");
                
                if (!foodResult.Success)
                {
                    Console.WriteLine("\n🔧 修正 food 表:");
                    Console.WriteLine("1. 在 Supabase SQL Editor 中執行:");
                    Console.WriteLine("   CREATE_FOOD_TABLE.sql 中的腳本");
                    Console.WriteLine("2. 確認表名為 'food' (單數)");
                    Console.WriteLine("3. 確認 RLS 政策允許所有操作");
                }
                
                if (!subscriptionResult.Success)
                {
                    Console.WriteLine("\n🔧 修正 subscription 表:");
                    Console.WriteLine("1. 在 Supabase SQL Editor 中執行:");
                    Console.WriteLine("   CREATE TABLE subscription (");
                    Console.WriteLine("     id UUID DEFAULT gen_random_uuid() PRIMARY KEY,");
                    Console.WriteLine("     name TEXT,");
                    Console.WriteLine("     nextdate TEXT,");
                    Console.WriteLine("     price BIGINT DEFAULT 0,");
                    Console.WriteLine("     site TEXT,");
                    Console.WriteLine("     account TEXT,");
                    Console.WriteLine("     note TEXT,");
                    Console.WriteLine("     created_at TIMESTAMPTZ DEFAULT NOW(),");
                    Console.WriteLine("     updated_at TIMESTAMPTZ DEFAULT NOW()");
                    Console.WriteLine("   );");
                    Console.WriteLine("2. 啟用 RLS:");
                    Console.WriteLine("   ALTER TABLE subscription ENABLE ROW LEVEL SECURITY;");
                    Console.WriteLine("3. 創建政策:");
                    Console.WriteLine("   CREATE POLICY \"Allow all\" ON subscription FOR ALL USING (true);");
                }
                
                Console.WriteLine("\n=== CSV 導入建議 ===");
                
                if (foodResult.Success || subscriptionResult.Success)
                {
                    Console.WriteLine("✅ 部分表可用，CSV 格式建議:");
                    Console.WriteLine("Food CSV 標題行: id,created_at,updated_at,name,price,photo,shop,todate,account");
                    Console.WriteLine("Subscription CSV 標題行: id,created_at,updated_at,name,nextdate,price,site,account,note");
                    Console.WriteLine("\n📝 重要提醒:");
                    Console.WriteLine("1. 欄位順序必須與上述完全一致");
                    Console.WriteLine("2. 時間戳記格式: yyyy-MM-ddTHH:mm:ss.fffZ");
                    Console.WriteLine("3. 確保所有文字欄位都用雙引號包圍");
                }
                
                // 顯示結果對話框
                var message = "Supabase 表結構診斷完成！\n\n";
                message += $"Food 表: {(foodResult.Success ? "✅ 正常" : "❌ 有問題")}\n";
                message += $"Subscription 表: {(subscriptionResult.Success ? "✅ 正常" : "❌ 有問題")}\n\n";
                
                if (!foodResult.Success || !subscriptionResult.Success)
                {
                    message += "發現問題，建議:\n";
                    message += "1. 檢查 Supabase 中的表是否存在\n";
                    message += "2. 確認表名正確 (food, subscription)\n";
                    message += "3. 檢查 RLS 政策設定\n";
                    message += "4. 驗證 API Key 權限\n\n";
                    message += "詳細資訊請查看 Visual Studio 輸出視窗。";
                }
                else
                {
                    message += "所有表都正常！\n";
                    message += "現在可以重新導出 CSV 並嘗試導入。";
                }
                
                MessageBox.Show(message, "表結構診斷", MessageBoxButton.OK, 
                    (!foodResult.Success || !subscriptionResult.Success) ? MessageBoxImage.Warning : MessageBoxImage.Information);
                
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ 診斷過程中發生異常: {ex.Message}");
                MessageBox.Show(
                    $"診斷過程中發生錯誤：\n\n{ex.Message}\n\n" +
                    "請檢查:\n" +
                    "1. 網路連接是否正常\n" +
                    "2. Supabase API 設定是否正確\n" +
                    "3. API Key 是否有效",
                    "診斷錯誤",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }
        }
    }
}