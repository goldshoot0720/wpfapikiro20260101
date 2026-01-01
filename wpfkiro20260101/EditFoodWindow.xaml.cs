using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using wpfkiro20260101.Models;

namespace wpfkiro20260101
{
    public partial class EditFoodWindow : Window
    {
        public Food? UpdatedFood { get; private set; }
        private Food _originalFood;
        private readonly HttpClient _httpClient;
        private string _validatedImageUrl = string.Empty;
        private System.Windows.Threading.DispatcherTimer _previewTimer;

        public EditFoodWindow(Food food)
        {
            InitializeComponent();
            _httpClient = new HttpClient();
            _originalFood = food;
            
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
            
            LoadFoodData();
        }

        private async void LoadFoodData()
        {
            try
            {
                // 載入現有食品資料到表單
                FoodNameTextBox.Text = _originalFood.FoodName;
                ShopTextBox.Text = _originalFood.Shop;
                PriceTextBox.Text = _originalFood.Price.ToString();
                QuantityTextBox.Text = _originalFood.Quantity.ToString();
                PhotoUrlTextBox.Text = _originalFood.Photo;
                _validatedImageUrl = _originalFood.Photo;

                // 處理到期日期
                if (DateTime.TryParse(_originalFood.ToDate, out DateTime expiryDate))
                {
                    ExpiryDatePicker.SelectedDate = expiryDate;
                }

                // 如果有現有圖片 URL，自動載入預覽
                if (!string.IsNullOrEmpty(_originalFood.Photo) && IsValidUrl(_originalFood.Photo))
                {
                    await LoadImagePreview(_originalFood.Photo);
                }

                System.Diagnostics.Debug.WriteLine($"載入編輯資料 - 食品: {_originalFood.FoodName}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"載入食品資料錯誤: {ex.Message}");
                MessageBox.Show($"載入食品資料時發生錯誤：{ex.Message}", "錯誤", MessageBoxButton.OK, MessageBoxImage.Error);
            }
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

        private void SaveFood_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("開始儲存食品變更...");
                
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

                // 創建更新後的食品物件
                UpdatedFood = new Food
                {
                    Id = _originalFood.Id, // 保持原有ID
                    FoodName = FoodNameTextBox.Text.Trim(),
                    Shop = ShopTextBox.Text.Trim(),
                    Price = (int)Math.Round(price),
                    Quantity = quantity,
                    Photo = _validatedImageUrl, // 使用驗證過的圖片 URL
                    PhotoHash = _originalFood.PhotoHash, // 保持原有PhotoHash
                    ToDate = ExpiryDatePicker.SelectedDate?.ToString("yyyy-MM-dd") ?? "",
                    CreatedAt = _originalFood.CreatedAt, // 保持原有創建時間
                    UpdatedAt = DateTime.UtcNow // 更新修改時間
                };

                System.Diagnostics.Debug.WriteLine($"更新食品對象成功 - ID: {UpdatedFood.Id}");

                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"SaveFood_Click 錯誤: {ex.Message}");
                MessageBox.Show($"儲存食品時發生錯誤：{ex.Message}", "錯誤", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}