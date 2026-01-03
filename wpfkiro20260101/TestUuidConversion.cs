using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using MessageBox = System.Windows.MessageBox;

namespace wpfkiro20260101
{
    public class TestUuidConversion
    {
        public static async Task RunTest()
        {
            try
            {
                MessageBox.Show("開始測試 UUID 轉換功能...", "測試", MessageBoxButton.OK, MessageBoxImage.Information);

                // 測試 Appwrite ID 轉換為 UUID
                var testCases = new[]
                {
                    "6957e395b806a8a1af04",           // 原始錯誤的 ID
                    "6957e395b806a8a1af04abcd",       // 較長的 ID
                    "short",                          // 短 ID
                    "verylongappwriteidthatexceeds32characters", // 超長 ID
                    "12345678-1234-1234-1234-123456789012"      // 已經是 UUID 格式
                };

                var results = new System.Text.StringBuilder();
                results.AppendLine("UUID 轉換測試結果：");
                results.AppendLine("==================");

                foreach (var testId in testCases)
                {
                    var convertedUuid = ConvertToUuid(testId);
                    var isValidUuid = Guid.TryParse(convertedUuid, out _);
                    
                    results.AppendLine($"原始 ID: {testId}");
                    results.AppendLine($"轉換後: {convertedUuid}");
                    results.AppendLine($"有效 UUID: {(isValidUuid ? "是" : "否")}");
                    results.AppendLine("---");
                }

                // 測試 CSV 轉換
                results.AppendLine("\n測試 CSV 轉換：");
                results.AppendLine("================");

                // 創建測試 CSV 內容
                var testCsvContent = @"$id,name,price,photo,shop,todate,photohash,$createdAt,$updatedAt
6957e395b806a8a1af04,測試食品,100,https://example.com/photo.jpg,測試商店,2026-01-10,hash123,2026-01-02T17:09:09.823Z,2026-01-02T17:09:09.823Z";

                var tempInputFile = Path.GetTempFileName();
                var tempOutputFile = Path.GetTempFileName();

                try
                {
                    // 寫入測試 CSV
                    File.WriteAllText(tempInputFile, testCsvContent);

                    // 轉換 CSV
                    await ConvertAppwriteToSupabaseCsv(tempInputFile, tempOutputFile, "food");

                    // 讀取轉換結果
                    var convertedContent = File.ReadAllText(tempOutputFile);
                    results.AppendLine("轉換後的 CSV 內容：");
                    results.AppendLine(convertedContent);

                    // 檢查是否包含有效的 UUID
                    var lines = convertedContent.Split('\n');
                    if (lines.Length > 1)
                    {
                        var dataLine = lines[1];
                        var fields = dataLine.Split(',');
                        if (fields.Length > 0)
                        {
                            var uuid = fields[0];
                            var isValidUuid = Guid.TryParse(uuid, out _);
                            results.AppendLine($"轉換後的 UUID: {uuid}");
                            results.AppendLine($"UUID 有效性: {(isValidUuid ? "有效" : "無效")}");
                        }
                    }
                }
                finally
                {
                    // 清理臨時檔案
                    if (File.Exists(tempInputFile)) File.Delete(tempInputFile);
                    if (File.Exists(tempOutputFile)) File.Delete(tempOutputFile);
                }

                MessageBox.Show(results.ToString(), "UUID 轉換測試結果", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"測試過程中發生錯誤：{ex.Message}", "錯誤", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private static string ConvertToUuid(string appwriteId)
        {
            try
            {
                // 移除可能的引號和空白
                appwriteId = appwriteId.Trim().Trim('"');
                
                // 如果已經是 UUID 格式，直接返回
                if (Guid.TryParse(appwriteId, out _))
                {
                    return appwriteId;
                }
                
                // 如果 Appwrite ID 長度不足，用零填充到 32 個字符
                if (appwriteId.Length < 32)
                {
                    appwriteId = appwriteId.PadRight(32, '0');
                }
                else if (appwriteId.Length > 32)
                {
                    // 如果太長，截取前 32 個字符
                    appwriteId = appwriteId.Substring(0, 32);
                }
                
                // 將 32 個字符的字符串轉換為 UUID 格式 (8-4-4-4-12)
                var uuid = $"{appwriteId.Substring(0, 8)}-{appwriteId.Substring(8, 4)}-{appwriteId.Substring(12, 4)}-{appwriteId.Substring(16, 4)}-{appwriteId.Substring(20, 12)}";
                
                // 驗證生成的 UUID 是否有效
                if (Guid.TryParse(uuid, out _))
                {
                    return uuid;
                }
                else
                {
                    // 如果轉換失敗，生成一個新的 UUID
                    return Guid.NewGuid().ToString();
                }
            }
            catch
            {
                // 如果任何步驟失敗，生成一個新的 UUID
                return Guid.NewGuid().ToString();
            }
        }

        private static async Task ConvertAppwriteToSupabaseCsv(string inputFile, string outputFile, string tableType)
        {
            await Task.Run(() =>
            {
                var lines = File.ReadAllLines(inputFile);
                if (lines.Length == 0) return;

                var convertedLines = new System.Collections.Generic.List<string>();
                
                // 處理標題行
                if (tableType == "food")
                {
                    convertedLines.Add("id,created_at,name,todate,amount,photo,price,shop,photohash");
                }

                // 處理資料行
                for (int i = 1; i < lines.Length; i++)
                {
                    var line = lines[i];
                    if (string.IsNullOrWhiteSpace(line)) continue;

                    try
                    {
                        var fields = line.Split(',');
                        if (fields.Length >= 7)
                        {
                            var appwriteId = fields[0].Trim().Trim('"');
                            var id = ConvertToUuid(appwriteId);
                            var name = fields[1];
                            var price = fields[2];
                            var photo = fields[3];
                            var shop = fields[4];
                            var todate = fields[5];
                            var photohash = fields.Length > 6 ? fields[6] : "";
                            var createdAt = fields.Length > 7 ? fields[7] : DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffffff+00");
                            var amount = "1";

                            convertedLines.Add($"{id},{createdAt},{name},{todate},{amount},{photo},{price},{shop},{photohash}");
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"轉換第 {i + 1} 行時發生錯誤: {ex.Message}");
                    }
                }

                // 寫入輸出檔案
                var utf8WithBom = new System.Text.UTF8Encoding(true);
                File.WriteAllLines(outputFile, convertedLines, utf8WithBom);
            });
        }
    }
}