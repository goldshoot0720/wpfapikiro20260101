using System;
using System.Threading.Tasks;
using System.Windows;
using MessageBox = System.Windows.MessageBox;

namespace wpfkiro20260101
{
    public class TestCsvFormatFix
    {
        public static async Task RunTest()
        {
            try
            {
                Console.WriteLine("=== 測試 CSV 格式修正 ===");
                
                // 模擬一個 Food 物件
                var testFood = new
                {
                    id = "dfdef1b4-e091-40ec-904e-58709cdc4909",
                    createdAt = "2026-01-02T17:09:09.823Z",
                    updatedAt = "", // 空值測試
                    foodName = "測試食品",
                    price = 100,
                    photo = "https://example.com/photo.jpg",
                    shop = "測試商店",
                    toDate = "2026-01-09T16:00:00.000Z",
                    account = "test@example.com"
                };
                
                // 測試 CSV 生成
                var csvContent = GenerateTestCsv(new[] { testFood });
                
                Console.WriteLine("生成的 CSV 內容:");
                Console.WriteLine("================");
                Console.WriteLine(csvContent);
                Console.WriteLine("================");
                
                // 驗證格式
                var lines = csvContent.Split('\n', StringSplitOptions.RemoveEmptyEntries);
                Console.WriteLine($"\nCSV 行數: {lines.Length}");
                Console.WriteLine($"標題行: {lines[0]}");
                if (lines.Length > 1)
                {
                    Console.WriteLine($"資料行: {lines[1]}");
                }
                
                // 檢查是否符合 Supabase 格式
                var expectedHeader = "id,created_at,updated_at,name,price,photo,shop,todate,account";
                var actualHeader = lines[0];
                
                var headerMatch = actualHeader == expectedHeader;
                Console.WriteLine($"\n標題行匹配: {(headerMatch ? "✅ 正確" : "❌ 錯誤")}");
                
                if (!headerMatch)
                {
                    Console.WriteLine($"期望: {expectedHeader}");
                    Console.WriteLine($"實際: {actualHeader}");
                }
                
                // 檢查資料行格式
                if (lines.Length > 1)
                {
                    var dataLine = lines[1];
                    var fields = dataLine.Split(',');
                    Console.WriteLine($"\n資料欄位數: {fields.Length}");
                    
                    if (fields.Length == 9)
                    {
                        Console.WriteLine("✅ 欄位數正確");
                        Console.WriteLine($"ID: {fields[0]}");
                        Console.WriteLine($"Created At: {fields[1]}");
                        Console.WriteLine($"Updated At: {fields[2]}");
                        Console.WriteLine($"Name: {fields[3]}");
                        Console.WriteLine($"Price: {fields[4]}");
                    }
                    else
                    {
                        Console.WriteLine("❌ 欄位數不正確");
                    }
                }
                
                MessageBox.Show(
                    $"CSV 格式測試完成！\n\n" +
                    $"標題行匹配: {(headerMatch ? "✅ 正確" : "❌ 錯誤")}\n" +
                    $"資料行數: {lines.Length}\n\n" +
                    "詳細資訊請查看控制台輸出。",
                    "CSV 格式測試",
                    MessageBoxButton.OK,
                    headerMatch ? MessageBoxImage.Information : MessageBoxImage.Warning
                );
                
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ 測試失敗: {ex.Message}");
                MessageBox.Show($"測試失敗: {ex.Message}", "錯誤", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        
        private static string GenerateTestCsv(object[] foods)
        {
            var csv = new System.Text.StringBuilder();
            
            // Supabase 格式標題行
            csv.AppendLine("id,created_at,updated_at,name,price,photo,shop,todate,account");
            
            foreach (var item in foods)
            {
                try
                {
                    var id = GetPropertyValue(item, "id") ?? "";
                    var createdAt = GetPropertyValue(item, "createdAt", "created_at") ?? "";
                    var updatedAt = GetPropertyValue(item, "updatedAt", "updated_at") ?? "";
                    var name = GetPropertyValue(item, "foodName", "name") ?? "";
                    var price = GetPropertyValue(item, "price") ?? "0";
                    var photo = GetPropertyValue(item, "photo") ?? "";
                    var shop = GetPropertyValue(item, "shop") ?? "";
                    var todate = GetPropertyValue(item, "toDate", "todate") ?? "";
                    var account = GetPropertyValue(item, "account") ?? "";
                    
                    // 如果 updated_at 為空，使用 created_at
                    var updatedAtValue = string.IsNullOrEmpty(updatedAt) ? createdAt : updatedAt;
                    
                    // 生成 CSV 行，數字欄位不加引號
                    csv.AppendLine($"\"{EscapeCsvField(id)}\",\"{createdAt}\",\"{updatedAtValue}\",\"{EscapeCsvField(name)}\",{price},\"{EscapeCsvField(photo)}\",\"{EscapeCsvField(shop)}\",\"{todate}\",\"{EscapeCsvField(account)}\"");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"處理項目時發生錯誤：{ex.Message}");
                }
            }
            
            return csv.ToString();
        }
        
        private static string GetPropertyValue(object obj, params string[] propertyNames)
        {
            foreach (var propertyName in propertyNames)
            {
                var property = obj.GetType().GetProperty(propertyName);
                if (property != null)
                {
                    var value = property.GetValue(obj);
                    return value?.ToString() ?? "";
                }
            }
            return "";
        }
        
        private static string EscapeCsvField(string field)
        {
            if (string.IsNullOrEmpty(field))
                return "";
            
            // 將雙引號轉換為兩個雙引號（CSV 標準）
            return field.Replace("\"", "\"\"");
        }
    }
}