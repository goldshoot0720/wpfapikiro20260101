using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using MessageBox = System.Windows.MessageBox;

namespace wpfkiro20260101
{
    public class TestCsvConverter
    {
        public static async Task RunTest()
        {
            try
            {
                Console.WriteLine("=== CSV 轉換功能測試 ===");
                
                // 創建測試用的 Appwrite CSV 資料
                await CreateTestAppwriteCsv();
                
                // 測試轉換功能
                await TestFoodCsvConversion();
                await TestSubscriptionCsvConversion();
                
                Console.WriteLine("\n=== 測試完成 ===");
                
                MessageBox.Show(
                    "CSV 轉換功能測試完成！\n\n" +
                    "已創建測試檔案並進行轉換測試。\n" +
                    "請查看控制台輸出以獲取詳細資訊。\n\n" +
                    "測試檔案位置：桌面\\CsvConverterTest 資料夾",
                    "CSV 轉換測試",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ 測試失敗: {ex.Message}");
                MessageBox.Show($"測試失敗: {ex.Message}", "錯誤", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        
        private static async Task CreateTestAppwriteCsv()
        {
            var testFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "CsvConverterTest");
            Directory.CreateDirectory(testFolder);
            
            // 創建測試 Food CSV (Appwrite 格式)
            var foodCsvPath = Path.Combine(testFolder, "Appwrite_Food_Test.csv");
            var foodCsvContent = @"""$id"",""name"",""price"",""photo"",""shop"",""todate"",""photohash"",""$createdAt"",""$updatedAt""
""dfdef1b4-e091-40ec-904e-58709cdc4909"",""測試蘋果"",""50"",""https://example.com/apple.jpg"",""測試商店"",""2026-01-15T00:00:00.000Z"",""hash123"",""2026-01-02T17:09:09.823Z"",""2026-01-02T17:09:09.823Z""
""12345678-1234-1234-1234-123456789012"",""測試香蕉"",""30"",""https://example.com/banana.jpg"",""水果店"",""2026-01-20T00:00:00.000Z"",""hash456"",""2026-01-02T17:10:00.000Z"",""2026-01-02T17:10:00.000Z""";
            
            await File.WriteAllTextAsync(foodCsvPath, foodCsvContent, System.Text.Encoding.UTF8);
            Console.WriteLine($"✅ 創建測試 Food CSV: {foodCsvPath}");
            
            // 創建測試 Subscription CSV (Appwrite 格式)
            var subscriptionCsvPath = Path.Combine(testFolder, "Appwrite_Subscription_Test.csv");
            var subscriptionCsvContent = @"""$id"",""name"",""nextdate"",""price"",""site"",""note"",""account"",""$createdAt"",""$updatedAt""
""96f5cf96-c82b-4003-a5d2-d7e0e07f8084"",""Netflix"",""2026-02-01"",""390"",""https://netflix.com"",""影音串流服務"",""test@example.com"",""2026-01-02T17:09:03.210Z"",""2026-01-02T17:09:03.210Z""
""87654321-4321-4321-4321-210987654321"",""Spotify"",""2026-01-15"",""149"",""https://spotify.com"",""音樂串流服務"",""test@example.com"",""2026-01-02T17:10:00.000Z"",""2026-01-02T17:10:00.000Z""";
            
            await File.WriteAllTextAsync(subscriptionCsvPath, subscriptionCsvContent, System.Text.Encoding.UTF8);
            Console.WriteLine($"✅ 創建測試 Subscription CSV: {subscriptionCsvPath}");
        }
        
        private static async Task TestFoodCsvConversion()
        {
            Console.WriteLine("\n--- 測試 Food CSV 轉換 ---");
            
            var testFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "CsvConverterTest");
            var inputFile = Path.Combine(testFolder, "Appwrite_Food_Test.csv");
            var outputFile = Path.Combine(testFolder, "Supabase_Food_Test.csv");
            
            if (File.Exists(inputFile))
            {
                // 模擬轉換過程
                var lines = await File.ReadAllLinesAsync(inputFile);
                Console.WriteLine($"📄 輸入檔案行數: {lines.Length}");
                Console.WriteLine($"📄 原始標題行: {lines[0]}");
                
                // 轉換標題行
                var supabaseHeader = "id,created_at,name,todate,amount,photo,price,shop,photohash";
                Console.WriteLine($"📄 轉換後標題行: {supabaseHeader}");
                
                // 轉換第一行資料作為範例
                if (lines.Length > 1)
                {
                    Console.WriteLine($"📄 原始資料行: {lines[1]}");
                    
                    // 簡單轉換示例
                    var convertedLine = ConvertFoodDataLine(lines[1]);
                    Console.WriteLine($"📄 轉換後資料行: {convertedLine}");
                }
                
                Console.WriteLine("✅ Food CSV 轉換測試完成");
            }
            else
            {
                Console.WriteLine("❌ 測試檔案不存在");
            }
        }
        
        private static async Task TestSubscriptionCsvConversion()
        {
            Console.WriteLine("\n--- 測試 Subscription CSV 轉換 ---");
            
            var testFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "CsvConverterTest");
            var inputFile = Path.Combine(testFolder, "Appwrite_Subscription_Test.csv");
            var outputFile = Path.Combine(testFolder, "Supabase_Subscription_Test.csv");
            
            if (File.Exists(inputFile))
            {
                var lines = await File.ReadAllLinesAsync(inputFile);
                Console.WriteLine($"📄 輸入檔案行數: {lines.Length}");
                Console.WriteLine($"📄 原始標題行: {lines[0]}");
                
                // 轉換標題行
                var supabaseHeader = "id,created_at,name,nextdate,price,site,note,account";
                Console.WriteLine($"📄 轉換後標題行: {supabaseHeader}");
                
                // 轉換第一行資料作為範例
                if (lines.Length > 1)
                {
                    Console.WriteLine($"📄 原始資料行: {lines[1]}");
                    
                    var convertedLine = ConvertSubscriptionDataLine(lines[1]);
                    Console.WriteLine($"📄 轉換後資料行: {convertedLine}");
                }
                
                Console.WriteLine("✅ Subscription CSV 轉換測試完成");
            }
            else
            {
                Console.WriteLine("❌ 測試檔案不存在");
            }
        }
        
        private static string ConvertFoodDataLine(string line)
        {
            // 簡單的轉換示例
            // Appwrite: "$id","name","price","photo","shop","todate","photohash","$createdAt","$updatedAt"
            // Supabase: id,created_at,name,todate,amount,photo,price,shop,photohash
            
            var fields = ParseSimpleCsv(line);
            if (fields.Length >= 8)
            {
                var id = CleanField(fields[0]);
                var name = CleanField(fields[1]);
                var price = CleanField(fields[2]);
                var photo = CleanField(fields[3]);
                var shop = CleanField(fields[4]);
                var todate = ConvertDateFormat(CleanField(fields[5]));
                var photohash = CleanField(fields[6]);
                var createdAt = ConvertDateFormat(CleanField(fields[7]));
                var amount = "1"; // 預設數量
                
                return $"{id},{createdAt},{name},{todate},{amount},{photo},{price},{shop},{photohash}";
            }
            
            return line;
        }
        
        private static string ConvertSubscriptionDataLine(string line)
        {
            // Appwrite: "$id","name","nextdate","price","site","note","account","$createdAt","$updatedAt"
            // Supabase: id,created_at,name,nextdate,price,site,note,account
            
            var fields = ParseSimpleCsv(line);
            if (fields.Length >= 8)
            {
                var id = CleanField(fields[0]);
                var name = CleanField(fields[1]);
                var nextdate = ConvertDateFormat(CleanField(fields[2]));
                var price = CleanField(fields[3]);
                var site = CleanField(fields[4]);
                var note = CleanField(fields[5]);
                var account = CleanField(fields[6]);
                var createdAt = ConvertDateFormat(CleanField(fields[7]));
                
                return $"{id},{createdAt},{name},{nextdate},{price},{site},{note},{account}";
            }
            
            return line;
        }
        
        private static string[] ParseSimpleCsv(string line)
        {
            return line.Split(',');
        }
        
        private static string CleanField(string field)
        {
            if (string.IsNullOrEmpty(field)) return "";
            
            field = field.Trim();
            if (field.StartsWith("\"") && field.EndsWith("\""))
            {
                field = field.Substring(1, field.Length - 2);
            }
            
            return field;
        }
        
        private static string ConvertDateFormat(string dateValue)
        {
            if (string.IsNullOrEmpty(dateValue)) return "";

            try
            {
                if (DateTime.TryParse(dateValue, out DateTime parsedDate))
                {
                    return parsedDate.ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss.ffffff+00", System.Globalization.CultureInfo.InvariantCulture);
                }
                
                return dateValue;
            }
            catch
            {
                return dateValue;
            }
        }
    }
}