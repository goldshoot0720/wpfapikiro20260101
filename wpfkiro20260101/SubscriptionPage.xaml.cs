using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

namespace wpfkiro20260101
{
    /// <summary>
    /// SubscriptionPage.xaml 的互動邏輯
    /// </summary>
    public partial class SubscriptionPage : Page
    {
        private IBackendService? _currentBackendService;
        private static readonly HttpClient _httpClient = new HttpClient();

        static SubscriptionPage()
        {
            // 設置 HttpClient 的 User-Agent 以避免被某些網站阻擋
            _httpClient.DefaultRequestHeaders.Add("User-Agent", 
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36");
            _httpClient.Timeout = TimeSpan.FromSeconds(10);
        }

        public SubscriptionPage()
        {
            InitializeComponent();
            Loaded += async (s, e) => await LoadSubscriptionData();
        }

        private async Task LoadSubscriptionData()
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
                ShowErrorMessage($"載入訂閱資料時發生錯誤：{ex.Message}");
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
                        await LoadAppwriteSubscriptionData();
                        break;
                    case BackendServiceType.Supabase:
                        await LoadSupabaseSubscriptionData();
                        break;
                    case BackendServiceType.Back4App:
                        await LoadBack4AppSubscriptionData();
                        break;
                    case BackendServiceType.MySQL:
                        await LoadMySQLSubscriptionData();
                        break;
                    case BackendServiceType.Contentful:
                        await LoadContentfulSubscriptionData();
                        break;
                    default:
                        ShowInfoMessage($"後端服務 {settings.GetServiceDisplayName()} 暫不支援訂閱管理功能");
                        break;
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"從後端載入資料時發生錯誤：{ex.Message}");
            }
        }

        private async Task LoadAppwriteSubscriptionData()
        {
            try
            {
                // 使用 Appwrite 服務載入訂閱資料
                if (_currentBackendService is AppwriteService appwriteService)
                {
                    var result = await appwriteService.GetSubscriptionsAsync();
                    if (result.Success && result.Data != null)
                    {
                        UpdateSubscriptionList(result.Data, "Appwrite");
                    }
                    else
                    {
                        ShowErrorMessage($"Appwrite 載入失敗：{result.ErrorMessage}");
                        UpdateSubscriptionList(new object[0], "Appwrite (無資料)");
                    }
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"Appwrite 訂閱資料載入錯誤：{ex.Message}");
                UpdateSubscriptionList(new object[0], "Appwrite (錯誤)");
            }
        }

        private async Task LoadSupabaseSubscriptionData()
        {
            try
            {
                await Task.Delay(500);
                var mockData = new object[]
                {
                    new { 
                        id = "supabase_1", 
                        name = "Supabase Pro 方案",
                        website = "https://supabase.com",
                        price = 25.0,
                        currency = "USD",
                        nextPayment = DateTime.Now.AddDays(20).Date,
                        category = "資料庫服務"
                    }
                };
                UpdateSubscriptionList(mockData, "Supabase");
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"Supabase 訂閱資料載入錯誤：{ex.Message}");
            }
        }

        private async Task LoadBack4AppSubscriptionData()
        {
            try
            {
                await Task.Delay(500);
                var mockData = new object[]
                {
                    new { 
                        id = "back4app_1", 
                        name = "Back4App 企業版",
                        website = "https://www.back4app.com",
                        price = 50.0,
                        currency = "USD",
                        nextPayment = DateTime.Now.AddDays(25).Date,
                        category = "後端服務"
                    }
                };
                UpdateSubscriptionList(mockData, "Back4App");
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"Back4App 訂閱資料載入錯誤：{ex.Message}");
            }
        }

        private async Task LoadMySQLSubscriptionData()
        {
            try
            {
                await Task.Delay(500);
                var mockData = new object[]
                {
                    new { 
                        id = "mysql_1", 
                        name = "MySQL 雲端資料庫",
                        website = "https://www.mysql.com",
                        price = 35.0,
                        currency = "USD",
                        nextPayment = DateTime.Now.AddDays(10).Date,
                        category = "資料庫"
                    },
                    new { 
                        id = "mysql_2", 
                        name = "MySQL 備份服務",
                        website = "https://www.mysql.com/backup",
                        price = 15.0,
                        currency = "USD",
                        nextPayment = DateTime.Now.AddDays(5).Date,
                        category = "備份服務"
                    }
                };
                UpdateSubscriptionList(mockData, "MySQL");
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"MySQL 訂閱資料載入錯誤：{ex.Message}");
            }
        }

        private async Task LoadContentfulSubscriptionData()
        {
            try
            {
                await Task.Delay(500);
                var mockData = new object[]
                {
                    new { 
                        id = "contentful_1", 
                        name = "Contentful CMS 專業版",
                        website = "https://www.contentful.com",
                        price = 489.0,
                        currency = "USD",
                        nextPayment = DateTime.Now.AddDays(30).Date,
                        category = "內容管理"
                    }
                };
                UpdateSubscriptionList(mockData, "Contentful");
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"Contentful 訂閱資料載入錯誤：{ex.Message}");
            }
        }

        private void UpdateServiceInfo(string serviceName)
        {
            // 更新UI顯示當前使用的後端服務
            if (BackendServiceLabel != null)
            {
                BackendServiceLabel.Text = $"[{serviceName}]";
            }
            System.Diagnostics.Debug.WriteLine($"訂閱管理 - 當前後端服務: {serviceName}");
        }

        private void UpdateSubscriptionList(object[] subscriptionData, string source)
        {
            // 更新資料來源資訊
            if (DataSourceInfo != null)
            {
                DataSourceInfo.Text = $"從 {source} 載入了 {subscriptionData.Length} 項訂閱資料";
            }
            
            // 清除現有的訂閱項目
            if (SubscriptionItemsContainer != null)
            {
                SubscriptionItemsContainer.Children.Clear();
                
                if (subscriptionData.Length == 0)
                {
                    // 創建新的無資料訊息元素，而不是重用現有的
                    var noDataCard = new Border
                    {
                        Style = (Style)FindResource("SubscriptionCardStyle")
                    };
                    
                    var noDataPanel = new StackPanel
                    {
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center,
                        Margin = new Thickness(20)
                    };
                    
                    var iconText = new TextBlock
                    {
                        Text = "📋",
                        FontSize = 48,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#9CA3AF")),
                        Margin = new Thickness(0, 0, 0, 10)
                    };
                    
                    var titleText = new TextBlock
                    {
                        Text = "目前沒有訂閱資料",
                        FontSize = 16,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#6B7280")),
                        FontWeight = FontWeights.Bold
                    };
                    
                    var hintText = new TextBlock
                    {
                        Text = "點擊上方的「添加訂閱」按鈕來新增訂閱項目",
                        FontSize = 12,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#9CA3AF")),
                        Margin = new Thickness(0, 5, 0, 0)
                    };
                    
                    noDataPanel.Children.Add(iconText);
                    noDataPanel.Children.Add(titleText);
                    noDataPanel.Children.Add(hintText);
                    noDataCard.Child = noDataPanel;
                    
                    SubscriptionItemsContainer.Children.Add(noDataCard);
                }
                else
                {
                    // 按日期排序 - 由近到遠（最新的在前面）
                    var sortedData = SortSubscriptionsByDate(subscriptionData);
                    
                    // 動態創建訂閱項目
                    foreach (var item in sortedData)
                    {
                        var subscriptionCard = CreateSubscriptionCard(item);
                        SubscriptionItemsContainer.Children.Add(subscriptionCard);
                    }
                }
            }
            
            System.Diagnostics.Debug.WriteLine($"從 {source} 載入了 {subscriptionData.Length} 項訂閱資料");
            
            foreach (var item in subscriptionData)
            {
                System.Diagnostics.Debug.WriteLine($"訂閱項目: {item}");
            }
        }

        private object[] SortSubscriptionsByDate(object[] subscriptionData)
        {
            try
            {
                return subscriptionData.OrderByDescending(item =>
                {
                    try
                    {
                        // 嘗試獲取 nextDate 或相關的日期欄位
                        var nextDate = GetPropertyValue(item, "nextdate", "nextDate", "NextDate") ?? "";
                        
                        if (DateTime.TryParse(nextDate, out DateTime parsedDate))
                        {
                            return parsedDate;
                        }
                        
                        // 如果無法解析日期，返回最小值（會排在最後）
                        return DateTime.MinValue;
                    }
                    catch
                    {
                        return DateTime.MinValue;
                    }
                }).ToArray();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"排序訂閱資料時發生錯誤：{ex.Message}");
                // 如果排序失敗，返回原始資料
                return subscriptionData;
            }
        }

        private Border CreateSubscriptionCard(object subscriptionItem)
        {
            // 創建訂閱卡片的UI元素
            var card = new Border
            {
                Style = (Style)FindResource("SubscriptionCardStyle")
            };

            var grid = new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto }); // Favicon
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto }); // Category
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }); // Info
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto }); // Edit button
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto }); // Delete button

            // 解析訂閱項目資料
            var itemData = subscriptionItem.ToString();
            var name = "未知訂閱";
            var site = "";
            var price = "0";
            var nextDate = "";
            var note = "";

            // 簡單的資料解析（實際應用中應該使用更好的方法）
            try
            {
                // 嘗試解析不同的屬性名稱
                if (subscriptionItem.GetType().GetProperty("subscriptionName")?.GetValue(subscriptionItem) is string itemName)
                    name = itemName;
                else if (subscriptionItem.GetType().GetProperty("name")?.GetValue(subscriptionItem) is string itemName2)
                    name = itemName2;
                else if (subscriptionItem.GetType().GetProperty("SubscriptionName")?.GetValue(subscriptionItem) is string subName)
                    name = subName;
                
                if (subscriptionItem.GetType().GetProperty("site")?.GetValue(subscriptionItem) is string itemSite)
                    site = itemSite;
                else if (subscriptionItem.GetType().GetProperty("website")?.GetValue(subscriptionItem) is string itemWebsite)
                    site = itemWebsite;
                else if (subscriptionItem.GetType().GetProperty("Site")?.GetValue(subscriptionItem) is string itemSiteCapital)
                    site = itemSiteCapital;
                
                if (subscriptionItem.GetType().GetProperty("price")?.GetValue(subscriptionItem) is int itemPrice)
                    price = $"NT$ {itemPrice}";
                else if (subscriptionItem.GetType().GetProperty("price")?.GetValue(subscriptionItem) is double itemPriceDouble)
                    price = $"${itemPriceDouble:F2}";
                else if (subscriptionItem.GetType().GetProperty("Price")?.GetValue(subscriptionItem) is int itemPriceCapital)
                    price = $"NT$ {itemPriceCapital}";
                
                if (subscriptionItem.GetType().GetProperty("nextDate")?.GetValue(subscriptionItem) is string itemNextDate)
                    nextDate = itemNextDate;
                else if (subscriptionItem.GetType().GetProperty("nextPayment")?.GetValue(subscriptionItem) is DateTime itemNextPayment)
                    nextDate = itemNextPayment.ToString("yyyy-MM-dd");
                else if (subscriptionItem.GetType().GetProperty("NextDate")?.GetValue(subscriptionItem) is DateTime itemNextDateCapital)
                    nextDate = itemNextDateCapital.ToString("yyyy-MM-dd");
                
                if (subscriptionItem.GetType().GetProperty("note")?.GetValue(subscriptionItem) is string itemNote)
                    note = itemNote;
                else if (subscriptionItem.GetType().GetProperty("Note")?.GetValue(subscriptionItem) is string itemNoteCapital)
                    note = itemNoteCapital;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"解析訂閱資料時發生錯誤: {ex.Message}");
            }

            // Favicon 圖示容器
            var faviconContainer = new Border
            {
                Width = 48,
                Height = 48,
                Margin = new Thickness(0, 0, 10, 0),
                CornerRadius = new CornerRadius(6),
                Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F3F4F6")),
                VerticalAlignment = VerticalAlignment.Center
            };
            Grid.SetColumn(faviconContainer, 0);

            // 預設圖示（當沒有 favicon 時顯示）
            var defaultIcon = new TextBlock
            {
                Text = "🌐",
                FontSize = 24,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#6B7280"))
            };
            faviconContainer.Child = defaultIcon;

            // 異步載入 favicon
            if (!string.IsNullOrEmpty(site))
            {
                _ = LoadFaviconForCard(faviconContainer, site);
            }

            // 服務類型標籤
            var categoryBorder = new Border
            {
                Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#10B981")),
                CornerRadius = new CornerRadius(6),
                Padding = new Thickness(8, 4, 8, 4),
                Margin = new Thickness(0, 0, 15, 0)
            };
            Grid.SetColumn(categoryBorder, 1);

            var categoryText = new TextBlock
            {
                Text = "訂閱",
                FontSize = 12,
                FontWeight = FontWeights.Bold,
                Foreground = Brushes.White
            };
            categoryBorder.Child = categoryText;

            // 訂閱資訊
            var infoPanel = new StackPanel();
            Grid.SetColumn(infoPanel, 2);

            var nameText = new TextBlock
            {
                Text = name,
                FontSize = 16,
                FontWeight = FontWeights.Bold,
                Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#374151"))
            };
            infoPanel.Children.Add(nameText);

            if (!string.IsNullOrEmpty(site))
            {
                // 創建可點擊的網站連結
                var sitePanel = new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    Margin = new Thickness(0, 2, 0, 0)
                };

                var siteLabel = new TextBlock
                {
                    Text = "網站: ",
                    FontSize = 12,
                    Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#6B7280"))
                };

                var siteLink = new TextBlock
                {
                    Text = site,
                    FontSize = 12,
                    Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#3B82F6")),
                    TextDecorations = TextDecorations.Underline,
                    Cursor = Cursors.Hand,
                    ToolTip = $"點擊開啟 {site}"
                };

                // 添加點擊事件
                siteLink.MouseLeftButtonUp += (sender, e) =>
                {
                    try
                    {
                        var url = site;
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
                siteLink.MouseEnter += (sender, e) =>
                {
                    siteLink.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1D4ED8"));
                };

                siteLink.MouseLeave += (sender, e) =>
                {
                    siteLink.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#3B82F6"));
                };

                sitePanel.Children.Add(siteLabel);
                sitePanel.Children.Add(siteLink);
                infoPanel.Children.Add(sitePanel);
            }

            var detailsPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Margin = new Thickness(0, 5, 0, 0)
            };

            var priceText = new TextBlock
            {
                Text = $"價格: {price}",
                FontSize = 12,
                Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#374151")),
                Margin = new Thickness(0, 0, 15, 0)
            };
            detailsPanel.Children.Add(priceText);

            if (!string.IsNullOrEmpty(nextDate))
            {
                var nextDateText = new TextBlock
                {
                    Text = $"下次付款: {nextDate}",
                    FontSize = 12,
                    Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#374151")),
                    Margin = new Thickness(0, 0, 15, 0)
                };
                detailsPanel.Children.Add(nextDateText);
            }

            infoPanel.Children.Add(detailsPanel);

            // 操作按鈕
            var editButton = new Button
            {
                Content = "編輯",
                Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#6366F1")),
                Foreground = Brushes.White,
                BorderThickness = new Thickness(0),
                Padding = new Thickness(12, 6, 12, 6),
                Margin = new Thickness(10, 0, 0, 0),
                Cursor = Cursors.Hand,
                Tag = subscriptionItem  // 將訂閱項目資料存儲在 Tag 中
            };
            editButton.Click += EditSubscription_Click;  // 添加點擊事件
            Grid.SetColumn(editButton, 3);

            var deleteButton = new Button
            {
                Content = "刪除",
                Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#EF4444")),
                Foreground = Brushes.White,
                BorderThickness = new Thickness(0),
                Padding = new Thickness(12, 6, 12, 6),
                Cursor = Cursors.Hand,
                Tag = subscriptionItem  // 將訂閱項目資料存儲在 Tag 中
            };
            deleteButton.Click += DeleteSubscription_Click;  // 添加點擊事件
            Grid.SetColumn(deleteButton, 4);

            grid.Children.Add(faviconContainer);
            grid.Children.Add(categoryBorder);
            grid.Children.Add(infoPanel);
            grid.Children.Add(editButton);
            grid.Children.Add(deleteButton);

            card.Child = grid;
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
                            HorizontalAlignment = HorizontalAlignment.Center,
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

        // 重新載入資料的公開方法
        public async Task RefreshDataAsync()
        {
            await LoadSubscriptionData();
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

                await LoadSubscriptionData();
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

        // 添加訂閱按鈕點擊事件
        private async void AddSubscription_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("開始添加訂閱流程...");
                
                // 打開添加訂閱對話框
                var addWindow = new AddSubscriptionWindow
                {
                    Owner = Window.GetWindow(this)
                };

                System.Diagnostics.Debug.WriteLine("顯示添加訂閱對話框...");
                
                if (addWindow.ShowDialog() == true && addWindow.NewSubscription != null)
                {
                    System.Diagnostics.Debug.WriteLine($"用戶確認添加訂閱: {addWindow.NewSubscription.SubscriptionName}");
                    
                    // 使用 CrudManager 創建訂閱
                    var crudManager = BackendServiceFactory.CreateCrudManager();
                    System.Diagnostics.Debug.WriteLine("創建 CrudManager 成功");
                    
                    var createResult = await crudManager.CreateSubscriptionAsync(addWindow.NewSubscription);
                    System.Diagnostics.Debug.WriteLine($"CreateSubscriptionAsync 結果: Success={createResult.Success}, Error={createResult.ErrorMessage}");

                    if (createResult.Success)
                    {
                        MessageBox.Show(
                            $"訂閱「{addWindow.NewSubscription.SubscriptionName}」已成功添加！",
                            "成功",
                            MessageBoxButton.OK,
                            MessageBoxImage.Information
                        );

                        // 重新載入資料以顯示新添加的訂閱
                        System.Diagnostics.Debug.WriteLine("重新載入訂閱資料...");
                        await LoadSubscriptionData();
                    }
                    else
                    {
                        ShowErrorMessage($"添加訂閱失敗：{createResult.ErrorMessage}");
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("用戶取消添加訂閱或資料為空");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"AddSubscription_Click 錯誤: {ex.Message}");
                ShowErrorMessage($"添加訂閱時發生錯誤：{ex.Message}");
            }
        }

        // 編輯訂閱按鈕點擊事件
        private async void EditSubscription_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender is Button button && button.Tag != null)
                {
                    var subscriptionItem = button.Tag;
                    System.Diagnostics.Debug.WriteLine($"編輯訂閱: {subscriptionItem}");
                    
                    // 解析訂閱資料
                    var subscription = ParseSubscriptionFromItem(subscriptionItem);
                    if (subscription == null)
                    {
                        ShowErrorMessage("無法解析訂閱資料");
                        return;
                    }

                    // 打開編輯訂閱對話框
                    var editWindow = new EditSubscriptionWindow(subscription)
                    {
                        Owner = Window.GetWindow(this)
                    };

                    System.Diagnostics.Debug.WriteLine("顯示編輯訂閱對話框...");
                    
                    if (editWindow.ShowDialog() == true && editWindow.UpdatedSubscription != null)
                    {
                        System.Diagnostics.Debug.WriteLine($"用戶確認編輯訂閱: {editWindow.UpdatedSubscription.SubscriptionName}");
                        
                        // 使用 CrudManager 更新訂閱
                        var crudManager = BackendServiceFactory.CreateCrudManager();
                        var updateResult = await crudManager.UpdateSubscriptionAsync(subscription.Id, editWindow.UpdatedSubscription);

                        if (updateResult.Success)
                        {
                            MessageBox.Show(
                                $"訂閱「{editWindow.UpdatedSubscription.SubscriptionName}」已成功更新！",
                                "成功",
                                MessageBoxButton.OK,
                                MessageBoxImage.Information
                            );

                            // 重新載入資料以顯示更新後的訂閱
                            await LoadSubscriptionData();
                        }
                        else
                        {
                            ShowErrorMessage($"更新訂閱失敗：{updateResult.ErrorMessage}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"編輯訂閱時發生錯誤：{ex.Message}");
            }
        }

        private Subscription? ParseSubscriptionFromItem(object subscriptionItem)
        {
            try
            {
                var subscription = new Subscription();
                
                if (subscriptionItem.GetType().GetProperty("id")?.GetValue(subscriptionItem) is string id)
                    subscription.Id = id;
                
                // 嘗試解析不同的名稱屬性
                if (subscriptionItem.GetType().GetProperty("subscriptionName")?.GetValue(subscriptionItem) is string subscriptionName)
                    subscription.SubscriptionName = subscriptionName;
                else if (subscriptionItem.GetType().GetProperty("name")?.GetValue(subscriptionItem) is string name)
                    subscription.SubscriptionName = name;
                else if (subscriptionItem.GetType().GetProperty("SubscriptionName")?.GetValue(subscriptionItem) is string subName)
                    subscription.SubscriptionName = subName;
                
                if (subscriptionItem.GetType().GetProperty("site")?.GetValue(subscriptionItem) is string site)
                    subscription.Site = site;
                if (subscriptionItem.GetType().GetProperty("price")?.GetValue(subscriptionItem) is int price)
                    subscription.Price = price;
                if (subscriptionItem.GetType().GetProperty("account")?.GetValue(subscriptionItem) is string account)
                    subscription.Account = account;
                if (subscriptionItem.GetType().GetProperty("note")?.GetValue(subscriptionItem) is string note)
                    subscription.Note = note;
                
                // 處理日期
                if (subscriptionItem.GetType().GetProperty("nextDate")?.GetValue(subscriptionItem) is string nextDateStr)
                {
                    if (DateTime.TryParse(nextDateStr, out DateTime nextDate))
                    {
                        subscription.NextDate = nextDate;
                        subscription.StringToDate = nextDate.ToString("yyyy-MM-dd");
                        subscription.DateTime = nextDate;
                    }
                }

                subscription.CreatedAt = DateTime.UtcNow;
                subscription.UpdatedAt = DateTime.UtcNow;

                return subscription;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"解析訂閱資料錯誤: {ex.Message}");
                return null;
            }
        }

        // 刪除訂閱按鈕點擊事件
        private async void DeleteSubscription_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender is Button button && button.Tag != null)
                {
                    var subscriptionItem = button.Tag;
                    System.Diagnostics.Debug.WriteLine($"刪除訂閱: {subscriptionItem}");
                    
                    // 獲取訂閱ID
                    string subscriptionId = "";
                    string subscriptionName = "未知訂閱";
                    
                    try
                    {
                        if (subscriptionItem.GetType().GetProperty("id")?.GetValue(subscriptionItem) is string id)
                            subscriptionId = id;
                        
                        // 嘗試解析不同的名稱屬性
                        if (subscriptionItem.GetType().GetProperty("subscriptionName")?.GetValue(subscriptionItem) is string subName1)
                            subscriptionName = subName1;
                        else if (subscriptionItem.GetType().GetProperty("name")?.GetValue(subscriptionItem) is string subName2)
                            subscriptionName = subName2;
                        else if (subscriptionItem.GetType().GetProperty("SubscriptionName")?.GetValue(subscriptionItem) is string subName3)
                            subscriptionName = subName3;
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"解析訂閱資料時發生錯誤: {ex.Message}");
                    }

                    if (string.IsNullOrEmpty(subscriptionId))
                    {
                        ShowErrorMessage("無法獲取訂閱ID");
                        return;
                    }

                    // 確認刪除
                    var result = MessageBox.Show(
                        $"確定要刪除訂閱「{subscriptionName}」嗎？\n此操作無法復原。",
                        "確認刪除",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Question
                    );

                    if (result == MessageBoxResult.Yes)
                    {
                        // 使用 CrudManager 刪除訂閱
                        var crudManager = BackendServiceFactory.CreateCrudManager();
                        var deleteResult = await crudManager.DeleteSubscriptionAsync(subscriptionId);

                        if (deleteResult.Success)
                        {
                            MessageBox.Show(
                                $"訂閱「{subscriptionName}」已成功刪除！",
                                "成功",
                                MessageBoxButton.OK,
                                MessageBoxImage.Information
                            );

                            // 重新載入資料以更新顯示
                            await LoadSubscriptionData();
                        }
                        else
                        {
                            ShowErrorMessage($"刪除訂閱失敗：{deleteResult.ErrorMessage}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"刪除訂閱時發生錯誤：{ex.Message}");
            }
        }

        private string GetPropertyValue(object obj, params string[] propertyNames)
        {
            if (obj == null) return null;

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