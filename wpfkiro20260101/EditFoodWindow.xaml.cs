using System;
using System.Windows;
using wpfkiro20260101.Models;

namespace wpfkiro20260101
{
    public partial class EditFoodWindow : Window
    {
        public Food? UpdatedFood { get; private set; }
        private Food _originalFood;

        public EditFoodWindow(Food food)
        {
            InitializeComponent();
            _originalFood = food;
            LoadFoodData();
        }

        private void LoadFoodData()
        {
            try
            {
                // 載入現有食品資料到表單
                FoodNameTextBox.Text = _originalFood.FoodName;
                ShopTextBox.Text = _originalFood.Shop;
                PriceTextBox.Text = _originalFood.Price.ToString();
                PhotoTextBox.Text = _originalFood.Photo;
                NotesTextBox.Text = _originalFood.Note;

                // 處理到期日期
                if (DateTime.TryParse(_originalFood.ToDate, out DateTime expiryDate))
                {
                    ExpiryDatePicker.SelectedDate = expiryDate;
                }

                System.Diagnostics.Debug.WriteLine($"載入編輯資料 - 食品: {_originalFood.FoodName}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"載入食品資料錯誤: {ex.Message}");
                MessageBox.Show($"載入食品資料時發生錯誤：{ex.Message}", "錯誤", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
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

                System.Diagnostics.Debug.WriteLine($"驗證通過 - 食品名稱: {FoodNameTextBox.Text}, 價格: {price}");

                // 創建更新後的食品物件
                UpdatedFood = new Food
                {
                    Id = _originalFood.Id, // 保持原有ID
                    FoodName = FoodNameTextBox.Text.Trim(),
                    Shop = ShopTextBox.Text.Trim(),
                    Price = (int)Math.Round(price),
                    Photo = PhotoTextBox.Text.Trim(),
                    PhotoHash = _originalFood.PhotoHash, // 保持原有PhotoHash
                    Note = NotesTextBox.Text.Trim(),
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