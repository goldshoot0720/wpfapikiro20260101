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
            NextPaymentDatePicker.SelectedDate = DateTime.Now.AddDays(30).Date;
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
                System.Diagnostics.Debug.WriteLine("開始添加訂閱...");
                
                // 驗證必填欄位
                if (string.IsNullOrWhiteSpace(ServiceNameTextBox.Text))
                {
                    MessageBox.Show("請輸入服務名稱", "驗證錯誤", MessageBoxButton.OK, MessageBoxImage.Warning);
                    ServiceNameTextBox.Focus();
                    return;
                }

                // 驗證月費
                if (!decimal.TryParse(MonthlyFeeTextBox.Text, out decimal monthlyFee) || monthlyFee < 0)
                {
                    MessageBox.Show("請輸入有效的月費（正數）", "驗證錯誤", MessageBoxButton.OK, MessageBoxImage.Warning);
                    MonthlyFeeTextBox.Focus();
                    return;
                }

                // 驗證下次付款日期
                if (!NextPaymentDatePicker.SelectedDate.HasValue)
                {
                    MessageBox.Show("請選擇下次付款日期", "驗證錯誤", MessageBoxButton.OK, MessageBoxImage.Warning);
                    NextPaymentDatePicker.Focus();
                    return;
                }

                System.Diagnostics.Debug.WriteLine($"驗證通過 - 服務名稱: {ServiceNameTextBox.Text}, 月費: {monthlyFee}, 日期: {NextPaymentDatePicker.SelectedDate}");

                // 創建新訂閱
                NewSubscription = new Subscription
                {
                    Id = Guid.NewGuid().ToString(),
                    SubscriptionName = ServiceNameTextBox.Text.Trim(),
                    Site = WebsiteTextBox.Text.Trim(),
                    Price = (int)Math.Round(monthlyFee), // 轉換為整數以符合模型
                    Account = "", // 暫時留空，因為表單中沒有此欄位
                    NextDate = DateTime.SpecifyKind(NextPaymentDatePicker.SelectedDate.Value, DateTimeKind.Utc),
                    Note = NotesTextBox.Text.Trim(),
                    StringToDate = NextPaymentDatePicker.SelectedDate.Value.ToString("yyyy-MM-dd"),
                    DateTime = NextPaymentDatePicker.SelectedDate.Value,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                System.Diagnostics.Debug.WriteLine($"創建訂閱對象成功 - ID: {NewSubscription.Id}");

                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"AddSubscription_Click 錯誤: {ex.Message}");
                MessageBox.Show($"創建訂閱時發生錯誤：{ex.Message}", "錯誤", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}