using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using wpfkiro20260101.Models;
using MessageBox = System.Windows.MessageBox;

namespace wpfkiro20260101
{
    public partial class AddFoodWindow : Window
    {
        public Food? NewFood { get; private set; }
        private readonly HttpClient _httpClient;
        private string _validatedImageUrl = string.Empty;
        private System.Windows.Threading.DispatcherTimer _previewTimer;

        public AddFoodWindow()
        {
            InitializeComponent();
            _httpClient = new HttpClient();
            
            // 設定預覽延遲計時器
            _previewTimer = new System.Windows.Threading.DispatcherTimer();
            _previewTimer.Interval = TimeSpan.FromMilliseconds(800); // 800ms 延遲
            _previewTimer.Tick += async (s, e) =>
            {
                _previewTimer.Stop();
                var url = PhotoUrlTextBox.Text.Trim();
                if (IsValidUrl(url))
                {
                    await LoadImagePreview(url);
                }
            };
            
            // 設定預設值
            ExpiryDatePicker.SelectedDate = DateTime.Now.AddDays(7).Date; // 預設7天後到期
        }

        private void PhotoUrl_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            var url = PhotoUrlTextBox.Text.Trim();
            PreviewButton.IsEnabled = !string.IsNullOrEmpty(url) && IsValidUrl(url);
            
            // 停止之前的計時器
            _previewTimer.Stop();
            
            if (string.IsNullOrEmpty(url))
            {
                HideImagePreview();
                return;
            }

            // 如果是有效的 URL，啟動延遲預覽
            if (IsValidUrl(url))
            {
                _previewTimer.Start();
            }
            else
            {
                HideImagePreview();
            }
        }

        private bool IsValidUrl(string url)
        {
            return Uri.TryCreate(url, UriKind.Absolute, out Uri? result) 
                   && (result.Scheme == Uri.UriSchemeHttp || result.Scheme == Uri.UriSchemeHttps)
                   && IsImageUrl(url);
        }

        private bool IsImageUrl(string url)
        {
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

        private async void PreviewImage_Click(object sender, RoutedEventArgs e)
        {
            var url = PhotoUrlTextBox.Text.Trim();
            if (string.IsNullOrEmpty(url)) return;

            await LoadImagePreview(url);
        }

        private async Task LoadImagePreview(string url)
        {
            try
            {
                ShowLoadingState();
                
                // 驗證圖片是否可以載入
                var response = await _httpClient.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri(url);
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.EndInit();
                    
                    ImagePreview.Source = bitmap;
                    _validatedImageUrl = url;
                    ShowImagePreview();
                }
                else
                {
                    ShowErrorState();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"載入圖片失敗: {ex.Message}");
                ShowErrorState();
            }
        }

        private void ShowLoadingState()
        {
            ImagePreviewBorder.Visibility = Visibility.Visible;
            LoadingText.Visibility = Visibility.Visible;
            ErrorText.Visibility = Visibility.Collapsed;
            ImagePreview.Visibility = Visibility.Collapsed;
        }

        private void ShowImagePreview()
        {
            ImagePreviewBorder.Visibility = Visibility.Visible;
            LoadingText.Visibility = Visibility.Collapsed;
            ErrorText.Visibility = Visibility.Collapsed;
            ImagePreview.Visibility = Visibility.Visible;
        }

        private void ShowErrorState()
        {
            ImagePreviewBorder.Visibility = Visibility.Visible;
            LoadingText.Visibility = Visibility.Collapsed;
            ErrorText.Visibility = Visibility.Visible;
            ImagePreview.Visibility = Visibility.Collapsed;
        }

        private void HideImagePreview()
        {
            ImagePreviewBorder.Visibility = Visibility.Collapsed;
            _validatedImageUrl = string.Empty;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            _httpClient?.Dispose();
            DialogResult = false;
            Close();
        }

        private void AddFood_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("開始添加食品...");
                
                // 驗證必填欄位
                if (string.IsNullOrWhiteSpace(FoodNameTextBox.Text))
                {
                    MessageBox.Show("請輸入食品名稱", "驗證錯誤", MessageBoxButton.OK, MessageBoxImage.Warning);
                    FoodNameTextBox.Focus();
                    return;
                }

                // 驗證價格（可選）
                decimal price = 0;
                if (!string.IsNullOrWhiteSpace(PriceTextBox.Text))
                {
                    if (!decimal.TryParse(PriceTextBox.Text, out price) || price < 0)
                    {
                        MessageBox.Show("請輸入有效的價格（正數）", "驗證錯誤", MessageBoxButton.OK, MessageBoxImage.Warning);
                        PriceTextBox.Focus();
                        return;
                    }
                }

                // 驗證數量
                int quantity = 1;
                if (!string.IsNullOrWhiteSpace(QuantityTextBox.Text))
                {
                    if (!int.TryParse(QuantityTextBox.Text, out quantity) || quantity <= 0)
                    {
                        MessageBox.Show("請輸入有效的數量（正整數）", "驗證錯誤", MessageBoxButton.OK, MessageBoxImage.Warning);
                        QuantityTextBox.Focus();
                        return;
                    }
                }

                System.Diagnostics.Debug.WriteLine($"驗證通過 - 食品名稱: {FoodNameTextBox.Text}, 價格: {price}, 數量: {quantity}");

                // 創建新食品
                NewFood = new Food
                {
                    Id = Guid.NewGuid().ToString(),
                    FoodName = FoodNameTextBox.Text.Trim(),
                    Shop = ShopTextBox.Text.Trim(),
                    Price = (int)Math.Round(price),
                    Quantity = quantity,
                    Photo = _validatedImageUrl, // 使用驗證過的圖片 URL
                    PhotoHash = "", // 暫時留空
                    ToDate = ExpiryDatePicker.SelectedDate?.ToString("yyyy-MM-dd") ?? "",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                System.Diagnostics.Debug.WriteLine($"創建食品對象成功 - ID: {NewFood.Id}");

                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"AddFood_Click 錯誤: {ex.Message}");
                MessageBox.Show($"創建食品時發生錯誤：{ex.Message}", "錯誤", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}