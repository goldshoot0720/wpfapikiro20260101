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
using wpfkiro20260101.Services;

namespace wpfkiro20260101
{
    /// <summary>
    /// SubscriptionPage.xaml 的互動邏輯
    /// </summary>
    public partial class SubscriptionPage : Page
    {
        private IBackendService? _currentBackendService;

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
                        nextPayment = DateTime.Now.AddDays(20),
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
                        nextPayment = DateTime.Now.AddDays(25),
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
                        nextPayment = DateTime.Now.AddDays(10),
                        category = "資料庫"
                    },
                    new { 
                        id = "mysql_2", 
                        name = "MySQL 備份服務",
                        website = "https://www.mysql.com/backup",
                        price = 15.0,
                        currency = "USD",
                        nextPayment = DateTime.Now.AddDays(5),
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
                        nextPayment = DateTime.Now.AddDays(30),
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
                    // 顯示無資料訊息
                    if (NoDataMessage != null)
                    {
                        NoDataMessage.Visibility = Visibility.Visible;
                        SubscriptionItemsContainer.Children.Add(NoDataMessage);
                    }
                }
                else
                {
                    // 隱藏無資料訊息
                    if (NoDataMessage != null)
                    {
                        NoDataMessage.Visibility = Visibility.Collapsed;
                    }
                    
                    // 動態創建訂閱項目
                    foreach (var item in subscriptionData)
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

        private Border CreateSubscriptionCard(object subscriptionItem)
        {
            // 創建訂閱卡片的UI元素
            var card = new Border
            {
                Style = (Style)FindResource("SubscriptionCardStyle")
            };

            var grid = new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

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
                if (subscriptionItem.GetType().GetProperty("name")?.GetValue(subscriptionItem) is string itemName)
                    name = itemName;
                if (subscriptionItem.GetType().GetProperty("site")?.GetValue(subscriptionItem) is string itemSite)
                    site = itemSite;
                if (subscriptionItem.GetType().GetProperty("price")?.GetValue(subscriptionItem) is int itemPrice)
                    price = $"NT$ {itemPrice}";
                if (subscriptionItem.GetType().GetProperty("nextDate")?.GetValue(subscriptionItem) is string itemNextDate)
                    nextDate = itemNextDate;
                if (subscriptionItem.GetType().GetProperty("note")?.GetValue(subscriptionItem) is string itemNote)
                    note = itemNote;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"解析訂閱資料時發生錯誤: {ex.Message}");
            }

            // 服務類型標籤
            var categoryBorder = new Border
            {
                Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#10B981")),
                CornerRadius = new CornerRadius(6),
                Padding = new Thickness(8, 4, 8, 4),
                Margin = new Thickness(0, 0, 15, 0)
            };
            Grid.SetColumn(categoryBorder, 0);

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
            Grid.SetColumn(infoPanel, 1);

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
                var siteText = new TextBlock
                {
                    Text = $"網站: {site}",
                    FontSize = 12,
                    Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#6B7280")),
                    Margin = new Thickness(0, 2, 0, 0)
                };
                infoPanel.Children.Add(siteText);
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
                Cursor = Cursors.Hand
            };
            Grid.SetColumn(editButton, 2);

            var deleteButton = new Button
            {
                Content = "刪除",
                Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#EF4444")),
                Foreground = Brushes.White,
                BorderThickness = new Thickness(0),
                Padding = new Thickness(12, 6, 12, 6),
                Cursor = Cursors.Hand
            };
            Grid.SetColumn(deleteButton, 3);

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
    }
}