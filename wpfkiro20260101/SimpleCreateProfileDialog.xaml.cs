using System;
using System.Windows;

namespace wpfkiro20260101
{
    public partial class SimpleCreateProfileDialog : Window
    {
        public string ProfileName => ProfileNameTextBox.Text.Trim();
        public string Description => DescriptionTextBox.Text.Trim();

        public SimpleCreateProfileDialog()
        {
            InitializeComponent();
            ProfileNameTextBox.Focus();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            var profileName = ProfileNameTextBox.Text.Trim();
            
            if (string.IsNullOrWhiteSpace(profileName))
            {
                NameValidationText.Text = "請輸入設定檔名稱";
                NameValidationText.Visibility = Visibility.Visible;
                ProfileNameTextBox.Focus();
                return;
            }

            if (profileName.Length > 100)
            {
                NameValidationText.Text = "設定檔名稱不能超過100個字元";
                NameValidationText.Visibility = Visibility.Visible;
                ProfileNameTextBox.Focus();
                return;
            }

            DialogResult = true;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}