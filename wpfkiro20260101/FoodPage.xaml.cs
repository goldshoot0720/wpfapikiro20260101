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
    /// FoodPage.xaml 的互動邏輯
    /// </summary>
    public partial class FoodPage : Page
    {
        private IBackendService? _currentBackendService;

        public FoodPage()
        {
            InitializeComponent();
            Loaded += async (s, e) => await LoadFoodData();
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
                        HorizontalAlignment = HorizontalAlignment.Center,
                        MaxWidth = 400
                    };
                    
                    var noDataPanel = new StackPanel
                    {
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center,
                        Margin = new Thickness(20)
                    };
                    
                    var iconText = new TextBlock
                    {
                        Text = "🍎",
                        FontSize = 48,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#9CA3AF")),
                        Margin = new Thickness(0, 0, 0, 10)
                    };
                    
                    var titleText = new TextBlock
                    {
                        Text = "目前沒有食品資料",
                        FontSize = 16,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#6B7280")),
                        FontWeight = FontWeights.Bold
                    };
                    
                    var hintText = new TextBlock
                    {
                        Text = "點擊上方的「添加食品」按鈕來新增食品項目",
                        FontSize = 12,
                        HorizontalAlignment = HorizontalAlignment.Center,
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
                    // 動態創建食品項目
                    foreach (var item in foodData)
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
            var shop = "";
            var toDate = "";
            var photo = "";

            // 簡單的資料解析
            try
            {
                if (foodItem.GetType().GetProperty("foodName")?.GetValue(foodItem) is string itemName)
                    name = itemName;
                if (foodItem.GetType().GetProperty("price")?.GetValue(foodItem) is int itemPrice)
                    price = $"NT$ {itemPrice}";
                if (foodItem.GetType().GetProperty("shop")?.GetValue(foodItem) is string itemShop)
                    shop = itemShop;
                if (foodItem.GetType().GetProperty("toDate")?.GetValue(foodItem) is string itemToDate)
                    toDate = itemToDate;
                if (foodItem.GetType().GetProperty("photo")?.GetValue(foodItem) is string itemPhoto)
                    photo = itemPhoto;
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

            var imageText = new TextBlock
            {
                Text = string.IsNullOrEmpty(photo) ? "🍎" : "📷",
                FontSize = 48,
                Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#10B981")),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
            imageBorder.Child = imageText;
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

            // 價格
            var priceText = new TextBlock
            {
                Text = $"價格: {price}",
                FontSize = 12,
                Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#6B7280")),
                Margin = new Thickness(0, 0, 0, 5)
            };
            stackPanel.Children.Add(priceText);

            // 商店
            if (!string.IsNullOrEmpty(shop))
            {
                var shopText = new TextBlock
                {
                    Text = $"商店: {shop}",
                    FontSize = 12,
                    Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#374151")),
                    Margin = new Thickness(0, 0, 0, 5)
                };
                stackPanel.Children.Add(shopText);
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
                Cursor = Cursors.Hand
            };
            Grid.SetColumn(editButton, 0);

            var deleteButton = new Button
            {
                Content = "🗑️",
                Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#EF4444")),
                Foreground = Brushes.White,
                BorderThickness = new Thickness(0),
                Padding = new Thickness(8),
                Cursor = Cursors.Hand
            };
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
    }
}