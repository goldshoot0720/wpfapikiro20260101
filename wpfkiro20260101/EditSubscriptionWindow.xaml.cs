using System;
using System.Windows;
using wpfkiro20260101.Models;
using MessageBox = System.Windows.MessageBox;

namespace wpfkiro20260101
{
    public partial class EditSubscriptionWindow : Window
    {
        public Subscription? UpdatedSubscription { get; private set; }
        private Subscription _originalSubscription;

        public EditSubscriptionWindow(Subscription subscription)
        {
            InitializeComponent();
            _originalSubscription = subscription;
            LoadSubscriptionData();
        }

        private void LoadSubscriptionData()
        {
            try
            {
                // 載入現有訂閱資料到表單
                ServiceNameTextBox.Text = _originalSubscription.SubscriptionName;
                WebsiteTextBox.Text = _originalSubscription.Site;
                MonthlyFeeTextBox.Text = _originalSubscription.Price.ToString();
                NextPaymentDatePicker.SelectedDate = _originalSubscription.NextDate;
                AccountTextBox.Text = _originalSubscription.Account;
                NotesTextBox.Text = _originalSubscription.Note;

                System.Diagnostics.Debug.WriteLine($"載入編輯資料 - 訂閱: {_originalSubscription.SubscriptionName}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"載入訂閱資料錯誤: {ex.Message}");
                MessageBox.Show($"載入訂閱資料時發生錯誤：{ex.Message}", "錯誤", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void SaveSubscription_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("開始儲存訂閱變更...");
                
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

                System.Diagnostics.Debug.WriteLine($"驗證通過 - 服務名稱: {ServiceNameTextBox.Text}, 月費: {monthlyFee}");

                // 創建更新後的訂閱物件
                UpdatedSubscription = new Subscription
                {
                    Id = _originalSubscription.Id, // 保持原有ID
                    SubscriptionName = ServiceNameTextBox.Text.Trim(),
                    Site = WebsiteTextBox.Text.Trim(),
                    Price = (int)Math.Round(monthlyFee),
                    Account = AccountTextBox.Text.Trim(),
                    NextDate = DateTime.SpecifyKind(NextPaymentDatePicker.SelectedDate.Value, DateTimeKind.Utc),
                    Note = NotesTextBox.Text.Trim(),
                    StringToDate = NextPaymentDatePicker.SelectedDate.Value.ToString("yyyy-MM-dd"),
                    DateTime = NextPaymentDatePicker.SelectedDate.Value,
                    CreatedAt = _originalSubscription.CreatedAt, // 保持原有創建時間
                    UpdatedAt = DateTime.UtcNow // 更新修改時間
                };

                System.Diagnostics.Debug.WriteLine($"更新訂閱對象成功 - ID: {UpdatedSubscription.Id}");

                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"SaveSubscription_Click 錯誤: {ex.Message}");
                MessageBox.Show($"儲存訂閱時發生錯誤：{ex.Message}", "錯誤", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}