using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Net.Http;
using System.IO;
using wpfkiro20260101.Services;
using wpfkiro20260101.Models;
using MessageBox = System.Windows.MessageBox;
using Button = System.Windows.Controls.Button;
using Image = System.Windows.Controls.Image;
using Color = System.Windows.Media.Color;
using ColorConverter = System.Windows.Media.ColorConverter;
using Brushes = System.Windows.Media.Brushes;
using Cursors = System.Windows.Input.Cursors;
using Orientation = System.Windows.Controls.Orientation;

namespace wpfkiro20260101
{
    /// <summary>
    /// FoodPage.xaml 的互動邏輯
    /// </summary>
    public partial class FoodPage : Page
    {
        private IBackendService? _currentBackendService;
        private static readonly HttpClient _httpClient = new HttpClient();

        static FoodPage()
        {
            // 設置 HttpClient 的 User-Agent 以避免被某些網站阻擋
            _httpClient.DefaultRequestHeaders.Add("User-Agent", 
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36");
            _httpClient.Timeout = TimeSpan.FromSeconds(10);
        }

        public FoodPage()
        {
            InitializeComponent();
            
            // 訂閱設定變更事件
            AppSettings.SettingsChanged += OnSettingsChanged;
            
            Loaded += async (s, e) => await LoadFoodData();
        }

        private async void OnSettingsChanged()
        {
            // 在 UI 線程上重新載入資料
            await Dispatcher.InvokeAsync(async () =>
            {
                await LoadFoodData();
                ShowInfoMessage("設定已更新，食品資料已重新載入");
            });
        }

        private async Task LoadFoodData()
        {
            try
            {
                // 獲取當前選擇的後端服務
                _currentBackendService = BackendServiceFactory.CreateCurrentService();
                var settings = AppSettings.Instance;

                // 顯示當前使用的後端服務
                UpdateServiceInfo(settings.GetServiceDisplayName());

                // 根據後端服務類型載入資料
                await LoadDataFromBackend();
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"載入食品資料時發生錯誤：{ex.Message}");
            }
        }

        private async Task LoadDataFromBackend()
        {
            try
            {
                var settings = AppSettings.Instance;

                switch (settings.BackendService)
                {
                    case BackendServiceType.Appwrite:
                        await LoadAppwriteFoodData();
                        break;
                    case BackendServiceType.Supabase:
                        await LoadSupabaseFoodData();
                        break;
                    case BackendServiceType.Back4App:
                        await LoadBack4AppFoodData();
                        break;
                    case BackendServiceType.MySQL:
                        await LoadMySQLFoodData();
                        break;
                    case BackendServiceType.NHost:
                        await LoadNHostFoodData();
                        break;
                    default:
                        ShowInfoMessage($"後端服務 {settings.GetServiceDisplayName()} 暫不支援食品管理功能");
                        break;
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"從後端載入資料時發生錯誤：{ex.Message}");
            }
        }

        private async Task LoadAppwriteFoodData()
        {
            try
            {
                // 使用 Appwrite 服務載入食品資料
                if (_currentBackendService is AppwriteService appwriteService)
                {
                    var result = await appwriteService.GetFoodsAsync();
                    if (result.Success && result.Data != null)
                    {
                        UpdateFoodList(result.Data, "Appwrite");
                    }
                    else
                    {
                        ShowErrorMessage($"Appwrite 載入失敗：{result.ErrorMessage}");
                        // 如果載入失敗，顯示空資料
                        UpdateFoodList(new object[0], "Appwrite (無資料)");
                    }
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"Appwrite 食品資料載入錯誤：{ex.Message}");
                UpdateFoodList(new object[0], "Appwrite (錯誤)");
            }
        }

        private async Task LoadSupabaseFoodData()
        {
            try
            {
                // 使用 Supabase 服務載入食品資料
                if (_currentBackendService is SupabaseService supabaseService)
                {
                    var result = await supabaseService.GetFoodsAsync();
                    if (result.Success && result.Data != null)
                    {
                        UpdateFoodList(result.Data, "Supabase");
                    }
                    else
                    {
                        ShowErrorMessage($"Supabase 載入失敗：{result.ErrorMessage}");
                        UpdateFoodList(new object[0], "Supabase (無資料)");
                    }
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"Supabase 食品資料載入錯誤：{ex.Message}");
                UpdateFoodList(new object[0], "Supabase (錯誤)");
            }
        }

        private async Task LoadBack4AppFoodData()
        {
            try
            {
                // 使用 Back4App 服務載入食品資料
                if (_currentBackendService is Back4AppService back4AppService)
                {
                    var result = await back4AppService.GetFoodsAsync();
                    if (result.Success && result.Data != null)
                    {
                        UpdateFoodList(result.Data, "Back4App");
                    }
                    else
                    {
                        ShowErrorMessage($"Back4App 載入失敗：{result.ErrorMessage}");
                        UpdateFoodList(new object[0], "Back4App (無資料)");
                    }
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"Back4App 食品資料載入錯誤：{ex.Message}");
                UpdateFoodList(new object[0], "Back4App (錯誤)");
            }
        }

        private async Task LoadMySQLFoodData()
        {
            try
            {
                // 使用 MySQL 服務載入食品資料
                if (_currentBackendService is MySQLService mySQLService)
                {
                    var result = await mySQLService.GetFoodsAsync();
                    if (result.Success && result.Data != null)
                    {
                        UpdateFoodList(result.Data, "MySQL");
                    }
                    else
                    {
                        ShowErrorMessage($"MySQL 載入失敗：{result.ErrorMessage}");
                        UpdateFoodList(new object[0], "MySQL (無資料)");
                    }
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"MySQL 食品資料載入錯誤：{ex.Message}");
                UpdateFoodList(new object[0], "MySQL (錯誤)");
            }
        }

        private async Task LoadNHostFoodData()
        {
            try
            {
                // 使用 NHost 服務載入食品資料
                if (_currentBackendService is NHostService nHostService)
                {
                    var result = await nHostService.GetFoodsAsync();
                    if (result.Success && result.Data != null)
                    {
                        UpdateFoodList(result.Data, "NHost");
                    }
                    else
                    {
                        ShowErrorMessage($"NHost 載入失敗：{result.ErrorMessage}");
                        UpdateFoodList(new object[0], "NHost (無資料)");
                    }
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"NHost 食品資料載入錯誤：{ex.Message}");
                UpdateFoodList(new object[0], "NHost (錯誤)");
            }
        }

        private void UpdateServiceInfo(string serviceName)
        {
            // 更新UI顯示當前使用的後端服務
            if (BackendServiceLabel != null)
            {
                BackendServiceLabel.Text = $"[{serviceName}]";
            }
            System.Diagnostics.Debug.WriteLine($"食品管理 - 當前後端服務: {serviceName}");
        }

        private void UpdateFoodList(object[] foodData, string source)
        {
            // 更新資料來源資訊
            if (DataSourceInfo != null)
            {
                DataSourceInfo.Text = $"從 {source} 載入了 {foodData.Length} 項食品資料";
            }
            
            // 清除現有的食品項目
            if (FoodItemsContainer != null)
            {
                FoodItemsContainer.Children.Clear();
                
                if (foodData.Length == 0)
                {
                    // 創建新的無資料訊息元素
                    var noDataCard = new Border
                    {
                        Style = (Style)FindResource("FoodCardStyle"),
                        HorizontalAlignment = System.Windows.HorizontalAlignment.Center,
                        MaxWidth = 400
                    };
                    
                    var noDataPanel = new StackPanel
                    {
                        HorizontalAlignment = System.Windows.HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center,
                        Margin = new Thickness(20)
                    };
                    
                    var iconText = new TextBlock
                    {
                        Text = "🍎",
                        FontSize = 48,
                        HorizontalAlignment = System.Windows.HorizontalAlignment.Center,
                        Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#9CA3AF")),
                        Margin = new Thickness(0, 0, 0, 10)
                    };
                    
                    var titleText = new TextBlock
                    {
                        Text = "目前沒有食品資料",
                        FontSize = 16,
                        HorizontalAlignment = System.Windows.HorizontalAlignment.Center,
                        Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#6B7280")),
                        FontWeight = FontWeights.Bold
                    };
                    
                    var hintText = new TextBlock
                    {
                        Text = "點擊上方的「添加食品」按鈕來新增食品項目",
                        FontSize = 12,
                        HorizontalAlignment = System.Windows.HorizontalAlignment.Center,
                        Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#9CA3AF")),
                        Margin = new Thickness(0, 5, 0, 0),
                        TextWrapping = TextWrapping.Wrap
                    };
                    
                    noDataPanel.Children.Add(iconText);
                    noDataPanel.Children.Add(titleText);
                    noDataPanel.Children.Add(hintText);
                    noDataCard.Child = noDataPanel;
                    
                    FoodItemsContainer.Children.Add(noDataCard);
                }
                else
                {
                    // 按日期排序 - 由近到遠（最新的在前面）
                    var sortedData = SortFoodsByDate(foodData);
                    
                    // 動態創建食品項目
                    foreach (var item in sortedData)
                    {
                        var foodCard = CreateFoodCard(item);
                        FoodItemsContainer.Children.Add(foodCard);
                    }
                }
            }
            
            System.Diagnostics.Debug.WriteLine($"從 {source} 載入了 {foodData.Length} 項食品資料");
            
            foreach (var item in foodData)
            {
                System.Diagnostics.Debug.WriteLine($"食品項目: {item}");
            }
        }

        private object[] SortFoodsByDate(object[] foodData)
        {
            try
            {
                return foodData.OrderBy(item =>
                {
                    try
                    {
                        string toDate = "";
                        
                        // 檢查是否為 JsonElement（NHost 返回的格式）
                        if (item is JsonElement jsonElement)
                        {
                            // 嘗試使用可用的日期欄位進行排序
                            string dateString = "";
                            
                            // 按優先順序嘗試不同的日期欄位
                            if (jsonElement.TryGetProperty("todate", out var todateElement))
                            {
                                dateString = todateElement.GetString() ?? "";
                            }
                            else if (jsonElement.TryGetProperty("toDate", out var toDateElement))
                            {
                                dateString = toDateElement.GetString() ?? "";
                            }
                            else if (jsonElement.TryGetProperty("ToDate", out var ToDateElement))
                            {
                                dateString = ToDateElement.GetString() ?? "";
                            }
                            else if (jsonElement.TryGetProperty("created_at", out var createdAtElement))
                            {
                                dateString = createdAtElement.GetString() ?? "";
                            }
                            else if (jsonElement.TryGetProperty("$createdAt", out var dollarCreatedAtElement))
                            {
                                dateString = dollarCreatedAtElement.GetString() ?? "";
                            }
                            
                            if (!string.IsNullOrEmpty(dateString) && DateTime.TryParse(dateString, out DateTime jsonParsedDate))
                            {
                                return jsonParsedDate;
                            }
                            
                            // 如果沒有找到有效的日期，使用名稱進行排序
                            if (jsonElement.TryGetProperty("name", out var nameElement))
                            {
                                var name = nameElement.GetString() ?? "";
                                // 使用名稱的字母順序作為排序依據，無日期的項目排在最後
                                return DateTime.MaxValue.AddDays(-name.Length);
                            }
                            else if (jsonElement.TryGetProperty("foodName", out var foodNameElement))
                            {
                                var name = foodNameElement.GetString() ?? "";
                                return DateTime.MaxValue.AddDays(-name.Length);
                            }
                        }
                        else
                        {
                            // 原有的反射解析邏輯（用於其他後端服務）
                            toDate = GetPropertyValue(item, "todate", "toDate", "ToDate", "nextdate") ?? "";
                        }
                        
                        if (DateTime.TryParse(toDate, out DateTime parsedDate))
                        {
                            return parsedDate;
                        }
                        
                        // 如果無法解析日期，嘗試使用 createdAt 或 updatedAt
                        var createdAt = GetPropertyValue(item, "$createdAt", "createdAt", "CreatedAt", "created_at") ?? "";
                        if (DateTime.TryParse(createdAt, out DateTime createdDate))
                        {
                            return createdDate;
                        }
                        
                        // 如果都無法解析，返回最大值（會排在最後）
                        return DateTime.MaxValue;
                    }
                    catch
                    {
                        return DateTime.MaxValue;
                    }
                }).ToArray();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"排序食品資料時發生錯誤：{ex.Message}");
                // 如果排序失敗，返回原始資料
                return foodData;
            }
        }

        private Border CreateFoodCard(object foodItem)
        {
            // 創建食品卡片的UI元素
            var card = new Border
            {
                Style = (Style)FindResource("FoodCardStyle")
            };

            var stackPanel = new StackPanel
            {
                Margin = new Thickness(15)
            };

            // 解析食品項目資料
            var name = "未知食品";
            var price = "0";
            var quantity = "1";
            var shop = "";
            var toDate = "";
            var photo = "";
            var category = "";
            var storageLocation = "";
            var description = "";

            // 智能資料解析 - 支援多種後端服務的資料格式
            try
            {
                // 檢查是否為 JsonElement（NHost 返回的格式）
                if (foodItem is JsonElement jsonElement)
                {
                    // 使用 JsonElement 的方法解析資料
                    if (jsonElement.TryGetProperty("name", out var nameElement))
                        name = nameElement.GetString() ?? "未知食品";
                    else if (jsonElement.TryGetProperty("foodName", out var foodNameElement))
                        name = foodNameElement.GetString() ?? "未知食品";
                    else if (jsonElement.TryGetProperty("FoodName", out var FoodNameElement))
                        name = FoodNameElement.GetString() ?? "未知食品";
                    
                    if (jsonElement.TryGetProperty("price", out var priceElement))
                    {
                        if (priceElement.ValueKind == JsonValueKind.Number)
                        {
                            if (priceElement.TryGetInt32(out var intPrice))
                                price = $"NT$ {intPrice}";
                            else if (priceElement.TryGetDouble(out var doublePrice))
                                price = $"NT$ {doublePrice:F2}";
                        }
                        else if (priceElement.ValueKind == JsonValueKind.String)
                        {
                            var priceStr = priceElement.GetString() ?? "0";
                            if (int.TryParse(priceStr, out var parsedPrice))
                                price = $"NT$ {parsedPrice}";
                        }
                    }
                    
                    if (jsonElement.TryGetProperty("quantity", out var quantityElement))
                    {
                        if (quantityElement.ValueKind == JsonValueKind.Number)
                        {
                            if (quantityElement.TryGetInt32(out var intQuantity))
                                quantity = intQuantity.ToString();
                        }
                        else if (quantityElement.ValueKind == JsonValueKind.String)
                        {
                            quantity = quantityElement.GetString() ?? "1";
                        }
                    }
                    
                    if (jsonElement.TryGetProperty("shop", out var shopElement))
                        shop = shopElement.GetString() ?? "";
                    
                    if (jsonElement.TryGetProperty("todate", out var todateElement))
                        toDate = todateElement.GetString() ?? "";
                    else if (jsonElement.TryGetProperty("toDate", out var toDateElement))
                        toDate = toDateElement.GetString() ?? "";
                    else if (jsonElement.TryGetProperty("ToDate", out var ToDateElement))
                        toDate = ToDateElement.GetString() ?? "";
                    
                    if (jsonElement.TryGetProperty("photo", out var photoElement))
                        photo = photoElement.GetString() ?? "";
                    else if (jsonElement.TryGetProperty("photohash", out var photohashElement))
                        photo = photohashElement.GetString() ?? "";
                    
                    if (jsonElement.TryGetProperty("category", out var categoryElement))
                        category = categoryElement.GetString() ?? "";
                    
                    if (jsonElement.TryGetProperty("storageLocation", out var storageLocationElement))
                        storageLocation = storageLocationElement.GetString() ?? "";
                    
                    if (jsonElement.TryGetProperty("description", out var descriptionElement))
                        description = descriptionElement.GetString() ?? "";
                    else if (jsonElement.TryGetProperty("note", out var noteElement))
                        description = noteElement.GetString() ?? "";
                }
                else
                {
                    // 原有的反射解析邏輯（用於其他後端服務）
                    // 使用 GetPropertyValue 方法來處理不同的資料格式（包括 JsonElement）
                    var nameValue = GetPropertyValue(foodItem, "name", "foodName", "FoodName");
                    if (!string.IsNullOrEmpty(nameValue))
                        name = nameValue;

                    var priceValue = GetPropertyValue(foodItem, "price", "Price");
                    if (!string.IsNullOrEmpty(priceValue) && int.TryParse(priceValue, out int parsedPrice))
                        price = $"NT$ {parsedPrice}";

                    var quantityValue = GetPropertyValue(foodItem, "quantity", "Quantity");
                    if (!string.IsNullOrEmpty(quantityValue) && int.TryParse(quantityValue, out int parsedQuantity))
                        quantity = parsedQuantity.ToString();

                    var shopValue = GetPropertyValue(foodItem, "shop", "Shop", "site");
                    if (!string.IsNullOrEmpty(shopValue))
                        shop = shopValue;

                    var toDateValue = GetPropertyValue(foodItem, "toDate", "ToDate", "todate", "nextdate");
                    if (!string.IsNullOrEmpty(toDateValue))
                        toDate = toDateValue;

                    var photoValue = GetPropertyValue(foodItem, "photo", "Photo", "photohash");
                    if (!string.IsNullOrEmpty(photoValue))
                        photo = photoValue;

                    var categoryValue = GetPropertyValue(foodItem, "category", "Category");
                    if (!string.IsNullOrEmpty(categoryValue))
                        category = categoryValue;

                    var storageValue = GetPropertyValue(foodItem, "storageLocation", "StorageLocation");
                    if (!string.IsNullOrEmpty(storageValue))
                        storageLocation = storageValue;

                    var descValue = GetPropertyValue(foodItem, "description", "Description", "note");
                    if (!string.IsNullOrEmpty(descValue))
                        description = descValue;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"解析食品資料時發生錯誤: {ex.Message}");
            }

            // 食品圖片區域
            var imageBorder = new Border
            {
                Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F3F4F6")),
                CornerRadius = new CornerRadius(8),
                Height = 120,
                Margin = new Thickness(0, 0, 0, 15)
            };

            // 如果有有效的圖片 URL，顯示網路圖片；否則顯示預設圖示
            if (!string.IsNullOrEmpty(photo) && IsValidImageUrl(photo))
            {
                var image = new Image
                {
                    Stretch = Stretch.UniformToFill,
                    HorizontalAlignment = System.Windows.HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center
                };

                try
                {
                    var bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri(photo);
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.EndInit();
                    image.Source = bitmap;
                    imageBorder.Child = image;
                }
                catch
                {
                    // 如果載入失敗，顯示預設圖示
                    var fallbackText = new TextBlock
                    {
                        Text = "❌",
                        FontSize = 48,
                        Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#EF4444")),
                        HorizontalAlignment = System.Windows.HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center
                    };
                    imageBorder.Child = fallbackText;
                }
            }
            else
            {
                var imageText = new TextBlock
                {
                    Text = "🍎",
                    FontSize = 48,
                    Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#10B981")),
                    HorizontalAlignment = System.Windows.HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center
                };
                imageBorder.Child = imageText;
            }
            
            stackPanel.Children.Add(imageBorder);

            // 食品名稱
            var nameText = new TextBlock
            {
                Text = name,
                FontSize = 14,
                FontWeight = FontWeights.Bold,
                Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#374151")),
                Margin = new Thickness(0, 0, 0, 10)
            };
            stackPanel.Children.Add(nameText);

            // 價格和數量
            var priceQuantityGrid = new Grid();
            priceQuantityGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            priceQuantityGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

            var priceText = new TextBlock
            {
                Text = $"價格: {price}",
                FontSize = 12,
                Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#6B7280")),
                Margin = new Thickness(0, 0, 5, 5)
            };
            Grid.SetColumn(priceText, 0);

            var quantityText = new TextBlock
            {
                Text = $"數量: {quantity}",
                FontSize = 12,
                Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#6B7280")),
                Margin = new Thickness(5, 0, 0, 5)
            };
            Grid.SetColumn(quantityText, 1);

            priceQuantityGrid.Children.Add(priceText);
            priceQuantityGrid.Children.Add(quantityText);
            stackPanel.Children.Add(priceQuantityGrid);

            // 商店
            if (!string.IsNullOrEmpty(shop))
            {
                // 創建可點擊的商店連結（如果是網址）
                var shopPanel = new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    Margin = new Thickness(0, 0, 0, 5)
                };

                var shopLabel = new TextBlock
                {
                    Text = "商店: ",
                    FontSize = 12,
                    Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#6B7280"))
                };

                if (IsValidUrl(shop))
                {
                    // 如果是有效的網址，創建可點擊的連結
                    var shopLink = new TextBlock
                    {
                        Text = shop,
                        FontSize = 12,
                        Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#3B82F6")),
                        TextDecorations = TextDecorations.Underline,
                        Cursor = Cursors.Hand,
                        ToolTip = $"點擊開啟 {shop}"
                    };

                    // 添加點擊事件
                    shopLink.MouseLeftButtonUp += (sender, e) =>
                    {
                        try
                        {
                            var url = shop;
                            if (!url.StartsWith("http://") && !url.StartsWith("https://"))
                            {
                                url = "https://" + url;
                            }
                            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                            {
                                FileName = url,
                                UseShellExecute = true
                            });
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"無法開啟網站：{ex.Message}", "錯誤", MessageBoxButton.OK, MessageBoxImage.Warning);
                        }
                    };

                    // 添加滑鼠懸停效果
                    shopLink.MouseEnter += (sender, e) =>
                    {
                        shopLink.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1D4ED8"));
                    };

                    shopLink.MouseLeave += (sender, e) =>
                    {
                        shopLink.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#3B82F6"));
                    };

                    shopPanel.Children.Add(shopLabel);
                    shopPanel.Children.Add(shopLink);
                }
                else
                {
                    // 如果不是網址，顯示普通文字
                    var shopText = new TextBlock
                    {
                        Text = shop,
                        FontSize = 12,
                        Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#374151"))
                    };

                    shopPanel.Children.Add(shopLabel);
                    shopPanel.Children.Add(shopText);
                }

                stackPanel.Children.Add(shopPanel);
            }

            // 分類和儲存位置
            if (!string.IsNullOrEmpty(category) || !string.IsNullOrEmpty(storageLocation))
            {
                var categoryGrid = new Grid();
                categoryGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                categoryGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

                if (!string.IsNullOrEmpty(category))
                {
                    var categoryText = new TextBlock
                    {
                        Text = $"🏷️ {category}",
                        FontSize = 11,
                        Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#8B5CF6")),
                        Margin = new Thickness(0, 0, 5, 5)
                    };
                    Grid.SetColumn(categoryText, 0);
                    categoryGrid.Children.Add(categoryText);
                }

                if (!string.IsNullOrEmpty(storageLocation))
                {
                    var storageText = new TextBlock
                    {
                        Text = $"📦 {storageLocation}",
                        FontSize = 11,
                        Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#059669")),
                        Margin = new Thickness(5, 0, 0, 5)
                    };
                    Grid.SetColumn(storageText, 1);
                    categoryGrid.Children.Add(storageText);
                }

                stackPanel.Children.Add(categoryGrid);
            }

            // 到期日期
            if (!string.IsNullOrEmpty(toDate))
            {
                var dateText = new TextBlock
                {
                    Text = $"到期日期: {toDate}",
                    FontSize = 12,
                    Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#10B981")),
                    FontWeight = FontWeights.Bold,
                    Margin = new Thickness(0, 0, 0, 15)
                };
                stackPanel.Children.Add(dateText);
            }

            // 操作按鈕
            var buttonGrid = new Grid();
            buttonGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            buttonGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

            var editButton = new Button
            {
                Content = "編輯",
                Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#6366F1")),
                Foreground = Brushes.White,
                BorderThickness = new Thickness(0),
                Padding = new Thickness(0, 8, 0, 8),
                Margin = new Thickness(0, 0, 5, 0),
                Cursor = Cursors.Hand,
                Tag = foodItem  // 將食品項目資料存儲在 Tag 中
            };
            editButton.Click += EditFood_Click;  // 添加點擊事件
            Grid.SetColumn(editButton, 0);

            var deleteButton = new Button
            {
                Content = "🗑️",
                Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#EF4444")),
                Foreground = Brushes.White,
                BorderThickness = new Thickness(0),
                Padding = new Thickness(8),
                Cursor = Cursors.Hand,
                Tag = foodItem  // 將食品項目資料存儲在 Tag 中
            };
            deleteButton.Click += DeleteFood_Click;  // 添加點擊事件
            Grid.SetColumn(deleteButton, 1);

            buttonGrid.Children.Add(editButton);
            buttonGrid.Children.Add(deleteButton);
            stackPanel.Children.Add(buttonGrid);

            card.Child = stackPanel;
            return card;
        }

        private void ShowErrorMessage(string message)
        {
            MessageBox.Show(message, "錯誤", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void ShowInfoMessage(string message)
        {
            MessageBox.Show(message, "資訊", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private bool IsValidUrl(string url)
        {
            if (string.IsNullOrEmpty(url)) return false;
            
            try
            {
                // 檢查是否包含常見的網址模式
                var lowerUrl = url.ToLower();
                
                // 如果已經是完整的 URL
                if (lowerUrl.StartsWith("http://") || lowerUrl.StartsWith("https://"))
                {
                    return Uri.TryCreate(url, UriKind.Absolute, out _);
                }
                
                // 檢查是否看起來像域名
                if (lowerUrl.Contains(".") && !lowerUrl.Contains(" "))
                {
                    // 嘗試構建 URL 並驗證
                    return Uri.TryCreate("https://" + url, UriKind.Absolute, out _);
                }
                
                return false;
            }
            catch
            {
                return false;
            }
        }

        // 獲取網站 favicon 的方法
        private async Task<BitmapImage?> GetFaviconAsync(string websiteUrl)
        {
            try
            {
                if (string.IsNullOrEmpty(websiteUrl))
                    return null;

                // 確保 URL 格式正確
                if (!websiteUrl.StartsWith("http://") && !websiteUrl.StartsWith("https://"))
                {
                    websiteUrl = "https://" + websiteUrl;
                }

                var uri = new Uri(websiteUrl);
                var baseUrl = $"{uri.Scheme}://{uri.Host}";
                
                // 嘗試多個常見的 favicon 路徑
                var faviconUrls = new[]
                {
                    $"{baseUrl}/favicon.ico",
                    $"{baseUrl}/favicon.png",
                    $"{baseUrl}/apple-touch-icon.png",
                    $"{baseUrl}/apple-touch-icon-precomposed.png"
                };

                foreach (var faviconUrl in faviconUrls)
                {
                    try
                    {
                        var response = await _httpClient.GetAsync(faviconUrl);
                        if (response.IsSuccessStatusCode)
                        {
                            var imageBytes = await response.Content.ReadAsByteArrayAsync();
                            if (imageBytes.Length > 0)
                            {
                                var bitmap = new BitmapImage();
                                bitmap.BeginInit();
                                bitmap.StreamSource = new MemoryStream(imageBytes);
                                bitmap.DecodePixelWidth = 40; // 設置較大尺寸以獲得更好品質
                                bitmap.DecodePixelHeight = 40;
                                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                                bitmap.EndInit();
                                bitmap.Freeze(); // 使其可以跨線程使用
                                
                                System.Diagnostics.Debug.WriteLine($"成功載入 favicon: {faviconUrl}");
                                return bitmap;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"載入 favicon 失敗 ({faviconUrl}): {ex.Message}");
                        continue;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"獲取 favicon 時發生錯誤: {ex.Message}");
            }

            return null;
        }

        // 異步載入 favicon 並更新 UI
        private async Task LoadFaviconForCard(Border faviconContainer, string websiteUrl)
        {
            try
            {
                var favicon = await GetFaviconAsync(websiteUrl);
                if (favicon != null)
                {
                    // 在 UI 線程上更新圖像
                    Dispatcher.Invoke(() =>
                    {
                        var image = new Image
                        {
                            Source = favicon,
                            Width = 40,
                            Height = 40,
                            HorizontalAlignment = System.Windows.HorizontalAlignment.Center,
                            VerticalAlignment = VerticalAlignment.Center
                        };
                        faviconContainer.Child = image;
                    });
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"載入 favicon 時發生錯誤: {ex.Message}");
                // 保持預設圖示
            }
        }

        private bool IsValidImageUrl(string url)
        {
            if (string.IsNullOrEmpty(url)) return false;
            
            try
            {
                var uri = new Uri(url);
                if (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps)
                    return false;

                var imageExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp" };
                var lowerUrl = url.ToLower();
                
                // 檢查常見的圖片副檔名
                if (imageExtensions.Any(ext => lowerUrl.Contains(ext)))
                    return true;
                
                // 檢查特殊的圖片服務
                var imageServices = new[]
                {
                    "picsum.photos",
                    "placeholder.com", 
                    "unsplash.com",
                    "httpbin.org/image",
                    "gstatic.com/images", // Google 圖片
                    "googleusercontent.com",
                    "imgur.com",
                    "flickr.com",
                    "pixabay.com",
                    "pexels.com"
                };
                
                return imageServices.Any(service => lowerUrl.Contains(service));
            }
            catch
            {
                return false;
            }
        }

        // 重新載入資料的公開方法
        public async Task RefreshDataAsync()
        {
            await LoadFoodData();
        }

        // 重新載入按鈕點擊事件
        private async void RefreshData_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (RefreshDataButton != null)
                {
                    RefreshDataButton.IsEnabled = false;
                    RefreshDataButton.Content = "載入中...";
                }

                if (DataSourceInfo != null)
                {
                    DataSourceInfo.Text = "正在重新載入資料...";
                }

                await LoadFoodData();
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"重新載入資料時發生錯誤：{ex.Message}");
            }
            finally
            {
                if (RefreshDataButton != null)
                {
                    RefreshDataButton.IsEnabled = true;
                    RefreshDataButton.Content = "🔄 重新載入";
                }
            }
        }

        // 編輯食品按鈕點擊事件
        private async void EditFood_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender is Button button && button.Tag != null)
                {
                    var foodItem = button.Tag;
                    System.Diagnostics.Debug.WriteLine($"編輯食品: {foodItem}");
                    
                    // 解析食品資料
                    var food = ParseFoodFromItem(foodItem);
                    if (food == null)
                    {
                        ShowErrorMessage("無法解析食品資料");
                        return;
                    }

                    // 打開編輯食品對話框
                    var editWindow = new EditFoodWindow(food)
                    {
                        Owner = Window.GetWindow(this)
                    };

                    System.Diagnostics.Debug.WriteLine("顯示編輯食品對話框...");
                    
                    if (editWindow.ShowDialog() == true && editWindow.UpdatedFood != null)
                    {
                        System.Diagnostics.Debug.WriteLine($"用戶確認編輯食品: {editWindow.UpdatedFood.FoodName}");
                        
                        // 使用 CrudManager 更新食品
                        var crudManager = BackendServiceFactory.CreateCrudManager();
                        var updateResult = await crudManager.UpdateFoodAsync(food.Id, editWindow.UpdatedFood);

                        if (updateResult.Success)
                        {
                            MessageBox.Show(
                                $"食品「{editWindow.UpdatedFood.FoodName}」已成功更新！",
                                "成功",
                                MessageBoxButton.OK,
                                MessageBoxImage.Information
                            );

                            // 重新載入資料以顯示更新後的食品
                            await LoadFoodData();
                        }
                        else
                        {
                            ShowErrorMessage($"更新食品失敗：{updateResult.ErrorMessage}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"編輯食品時發生錯誤：{ex.Message}");
            }
        }

        private Food? ParseFoodFromItem(object foodItem)
        {
            try
            {
                var food = new Food();
                
                // 檢查是否為 JsonElement（NHost 返回的格式）
                if (foodItem is JsonElement jsonElement)
                {
                    // 使用 JsonElement 的方法解析資料
                    if (jsonElement.TryGetProperty("id", out var idElement))
                        food.Id = idElement.GetString() ?? "";
                    
                    if (jsonElement.TryGetProperty("name", out var nameElement))
                        food.FoodName = nameElement.GetString() ?? "";
                    else if (jsonElement.TryGetProperty("foodName", out var foodNameElement))
                        food.FoodName = foodNameElement.GetString() ?? "";
                    else if (jsonElement.TryGetProperty("FoodName", out var FoodNameElement))
                        food.FoodName = FoodNameElement.GetString() ?? "";
                    
                    if (jsonElement.TryGetProperty("shop", out var shopElement))
                        food.Shop = shopElement.GetString() ?? "";
                    
                    if (jsonElement.TryGetProperty("price", out var priceElement))
                    {
                        if (priceElement.ValueKind == JsonValueKind.Number)
                        {
                            if (priceElement.TryGetInt32(out var intPrice))
                                food.Price = intPrice;
                        }
                        else if (priceElement.ValueKind == JsonValueKind.String)
                        {
                            var priceStr = priceElement.GetString() ?? "0";
                            if (int.TryParse(priceStr, out var parsedPrice))
                                food.Price = parsedPrice;
                        }
                    }
                    
                    if (jsonElement.TryGetProperty("quantity", out var quantityElement))
                    {
                        if (quantityElement.ValueKind == JsonValueKind.Number)
                        {
                            if (quantityElement.TryGetInt32(out var intQuantity))
                                food.Quantity = intQuantity;
                        }
                        else if (quantityElement.ValueKind == JsonValueKind.String)
                        {
                            var quantityStr = quantityElement.GetString() ?? "1";
                            if (int.TryParse(quantityStr, out var parsedQuantity))
                                food.Quantity = parsedQuantity;
                        }
                    }
                    
                    if (jsonElement.TryGetProperty("photo", out var photoElement))
                        food.Photo = photoElement.GetString() ?? "";
                    else if (jsonElement.TryGetProperty("photohash", out var photohashElement))
                        food.PhotoHash = photohashElement.GetString() ?? "";
                    
                    if (jsonElement.TryGetProperty("note", out var noteElement))
                        food.Note = noteElement.GetString() ?? "";
                    
                    if (jsonElement.TryGetProperty("description", out var descriptionElement))
                        food.Description = descriptionElement.GetString() ?? "";
                    
                    if (jsonElement.TryGetProperty("category", out var categoryElement))
                        food.Category = categoryElement.GetString() ?? "";
                    
                    if (jsonElement.TryGetProperty("storageLocation", out var storageLocationElement))
                        food.StorageLocation = storageLocationElement.GetString() ?? "";
                    
                    // 處理到期日期
                    if (jsonElement.TryGetProperty("todate", out var todateElement))
                        food.ToDate = todateElement.GetString() ?? "";
                    else if (jsonElement.TryGetProperty("toDate", out var toDateElement))
                        food.ToDate = toDateElement.GetString() ?? "";
                    else if (jsonElement.TryGetProperty("ToDate", out var ToDateElement))
                        food.ToDate = ToDateElement.GetString() ?? "";
                }
                else
                {
                    // 原有的反射解析邏輯（用於其他後端服務）
                    if (foodItem.GetType().GetProperty("id")?.GetValue(foodItem) is string id)
                        food.Id = id;
                    if (foodItem.GetType().GetProperty("foodName")?.GetValue(foodItem) is string name)
                        food.FoodName = name;
                    if (foodItem.GetType().GetProperty("shop")?.GetValue(foodItem) is string shop)
                        food.Shop = shop;
                    if (foodItem.GetType().GetProperty("price")?.GetValue(foodItem) is int price)
                        food.Price = price;
                    if (foodItem.GetType().GetProperty("quantity")?.GetValue(foodItem) is int quantity)
                        food.Quantity = quantity;
                    if (foodItem.GetType().GetProperty("photo")?.GetValue(foodItem) is string photo)
                        food.Photo = photo;
                    if (foodItem.GetType().GetProperty("photoHash")?.GetValue(foodItem) is string photoHash)
                        food.PhotoHash = photoHash;
                    if (foodItem.GetType().GetProperty("note")?.GetValue(foodItem) is string note)
                        food.Note = note;
                    if (foodItem.GetType().GetProperty("description")?.GetValue(foodItem) is string description)
                        food.Description = description;
                    if (foodItem.GetType().GetProperty("category")?.GetValue(foodItem) is string category)
                        food.Category = category;
                    if (foodItem.GetType().GetProperty("storageLocation")?.GetValue(foodItem) is string storageLocation)
                        food.StorageLocation = storageLocation;
                    
                    // 處理到期日期
                    if (foodItem.GetType().GetProperty("toDate")?.GetValue(foodItem) is string toDateStr)
                    {
                        food.ToDate = toDateStr;
                    }
                }

                food.CreatedAt = DateTime.UtcNow;
                food.UpdatedAt = DateTime.UtcNow;

                return food;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"解析食品資料錯誤: {ex.Message}");
                return null;
            }
        }

        // 刪除食品按鈕點擊事件
        private async void DeleteFood_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender is Button button && button.Tag != null)
                {
                    var foodItem = button.Tag;
                    System.Diagnostics.Debug.WriteLine($"刪除食品: {foodItem}");
                    
                    // 獲取食品ID
                    string foodId = "";
                    string foodName = "未知食品";
                    
                    try
                    {
                        // 檢查是否為 JsonElement（NHost 返回的格式）
                        if (foodItem is JsonElement jsonElement)
                        {
                            if (jsonElement.TryGetProperty("id", out var idElement))
                                foodId = idElement.GetString() ?? "";
                            
                            if (jsonElement.TryGetProperty("name", out var nameElement))
                                foodName = nameElement.GetString() ?? "未知食品";
                            else if (jsonElement.TryGetProperty("foodName", out var foodNameElement))
                                foodName = foodNameElement.GetString() ?? "未知食品";
                            else if (jsonElement.TryGetProperty("FoodName", out var FoodNameElement))
                                foodName = FoodNameElement.GetString() ?? "未知食品";
                        }
                        else
                        {
                            // 原有的反射解析邏輯（用於其他後端服務）
                            if (foodItem.GetType().GetProperty("id")?.GetValue(foodItem) is string id)
                                foodId = id;
                            if (foodItem.GetType().GetProperty("foodName")?.GetValue(foodItem) is string name)
                                foodName = name;
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"解析食品資料時發生錯誤: {ex.Message}");
                    }

                    if (string.IsNullOrEmpty(foodId))
                    {
                        ShowErrorMessage("無法獲取食品ID");
                        return;
                    }

                    // 確認刪除
                    var result = MessageBox.Show(
                        $"確定要刪除食品「{foodName}」嗎？\n此操作無法復原。",
                        "確認刪除",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Question
                    );

                    if (result == MessageBoxResult.Yes)
                    {
                        // 使用 CrudManager 刪除食品
                        var crudManager = BackendServiceFactory.CreateCrudManager();
                        var deleteResult = await crudManager.DeleteFoodAsync(foodId);

                        if (deleteResult.Success)
                        {
                            MessageBox.Show(
                                $"食品「{foodName}」已成功刪除！",
                                "成功",
                                MessageBoxButton.OK,
                                MessageBoxImage.Information
                            );

                            // 重新載入資料以更新顯示
                            await LoadFoodData();
                        }
                        else
                        {
                            ShowErrorMessage($"刪除食品失敗：{deleteResult.ErrorMessage}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"刪除食品時發生錯誤：{ex.Message}");
            }
        }

        // 添加食品按鈕點擊事件
        private async void AddFood_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("開始添加食品流程...");
                
                // 打開添加食品對話框
                var addWindow = new AddFoodWindow
                {
                    Owner = Window.GetWindow(this)
                };

                System.Diagnostics.Debug.WriteLine("顯示添加食品對話框...");
                
                if (addWindow.ShowDialog() == true && addWindow.NewFood != null)
                {
                    System.Diagnostics.Debug.WriteLine($"用戶確認添加食品: {addWindow.NewFood.FoodName}");
                    
                    // 使用 CrudManager 創建食品
                    var crudManager = BackendServiceFactory.CreateCrudManager();
                    System.Diagnostics.Debug.WriteLine("創建 CrudManager 成功");
                    
                    var createResult = await crudManager.CreateFoodAsync(addWindow.NewFood);
                    System.Diagnostics.Debug.WriteLine($"CreateFoodAsync 結果: Success={createResult.Success}, Error={createResult.ErrorMessage}");

                    if (createResult.Success)
                    {
                        MessageBox.Show(
                            $"食品「{addWindow.NewFood.FoodName}」已成功添加！",
                            "成功",
                            MessageBoxButton.OK,
                            MessageBoxImage.Information
                        );

                        // 重新載入資料以顯示新添加的食品
                        System.Diagnostics.Debug.WriteLine("重新載入食品資料...");
                        await LoadFoodData();
                    }
                    else
                    {
                        ShowErrorMessage($"添加食品失敗：{createResult.ErrorMessage}");
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("用戶取消添加食品或資料為空");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"AddFood_Click 錯誤: {ex.Message}");
                ShowErrorMessage($"添加食品時發生錯誤：{ex.Message}");
            }
        }

        private string GetPropertyValue(object obj, params string[] propertyNames)
        {
            if (obj == null) return null;

            // 處理 JsonElement（NHost 和其他 GraphQL 服務返回的格式）
            if (obj is JsonElement jsonElement)
            {
                foreach (var propertyName in propertyNames)
                {
                    try
                    {
                        if (jsonElement.TryGetProperty(propertyName, out var property))
                        {
                            return property.ValueKind switch
                            {
                                JsonValueKind.String => property.GetString(),
                                JsonValueKind.Number => property.GetInt32().ToString(),
                                JsonValueKind.True => "true",
                                JsonValueKind.False => "false",
                                JsonValueKind.Null => null,
                                _ => property.ToString()
                            };
                        }
                    }
                    catch
                    {
                        // 繼續嘗試下一個屬性名稱
                    }
                }
                return null;
            }

            // 處理普通物件（使用反射）
            var objType = obj.GetType();
            
            foreach (var propertyName in propertyNames)
            {
                try
                {
                    var property = objType.GetProperty(propertyName);
                    if (property != null)
                    {
                        var value = property.GetValue(obj);
                        return value?.ToString();
                    }
                }
                catch
                {
                    // 繼續嘗試下一個屬性名稱
                }
            }
            
            return null;
        }
    }
}