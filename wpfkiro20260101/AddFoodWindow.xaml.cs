using System;
using System.Windows;
using wpfkiro20260101.Models;

namespace wpfkiro20260101
{
    public partial class AddFoodWindow : Window
    {
        public Food? NewFood { get; private set; }

        public AddFoodWindow()
        {
            InitializeComponent();
            
            // 設定預設值
            ExpiryDatePicker.SelectedDate = DateTime.Now.AddDays(7).Date; // 預設7天後到期
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
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
                    Photo = PhotoTextBox.Text.Trim(),
                    PhotoHash = "", // 暫時留空
                    Note = NotesTextBox.Text.Trim(),
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