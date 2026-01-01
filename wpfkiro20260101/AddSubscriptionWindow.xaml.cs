using System;
using System.Windows;
using wpfkiro20260101.Models;

namespace wpfkiro20260101
{
    public partial class AddSubscriptionWindow : Window
    {
        public Subscription? NewSubscription { get; private set; }

        public AddSubscriptionWindow()
        {
            InitializeComponent();
            
            // 設定預設值
            NextDatePicker.SelectedDate = DateTime.Now.AddDays(30);
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void AddSubscription_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // 驗證必填欄位
                if (string.IsNullOrWhiteSpace(SubscriptionNameTextBox.Text))
                {
                    MessageBox.Show("請輸入訂閱名稱", "驗證錯誤", MessageBoxButton.OK, MessageBoxImage.Warning);
                    SubscriptionNameTextBox.Focus();
                    return;
                }

                // 驗證價格
                if (!int.TryParse(PriceTextBox.Text, out int price) || price < 0)
                {
                    MessageBox.Show("請輸入有效的價格（正整數）", "驗證錯誤", MessageBoxButton.OK, MessageBoxImage.Warning);
                    PriceTextBox.Focus();
                    return;
                }

                // 創建新訂閱
                NewSubscription = new Subscription
                {
                    Id = Guid.NewGuid().ToString(),
                    SubscriptionName = SubscriptionNameTextBox.Text.Trim(),
                    Site = SiteTextBox.Text.Trim(),
                    Price = price,
                    Account = AccountTextBox.Text.Trim(),
                    NextDate = NextDatePicker.SelectedDate ?? DateTime.Now.AddDays(30),
                    Note = NoteTextBox.Text.Trim(),
                    StringToDate = (NextDatePicker.SelectedDate ?? DateTime.Now.AddDays(30)).ToString("yyyy-MM-dd"),
                    DateTime = NextDatePicker.SelectedDate ?? DateTime.Now.AddDays(30),
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"創建訂閱時發生錯誤：{ex.Message}", "錯誤", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}