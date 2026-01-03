using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using wpfkiro20260101.Models;
using wpfkiro20260101.Services;
using MessageBox = System.Windows.MessageBox;

namespace wpfkiro20260101
{
    public partial class CreateProfileDialog : Window
    {
        private readonly SettingsProfile? _editingProfile;
        private readonly bool _isEditing;

        public string ProfileName => ProfileNameTextBox.Text.Trim();
        public string Description => DescriptionTextBox.Text.Trim();

        public CreateProfileDialog(SettingsProfile? editingProfile = null)
        {
            InitializeComponent();
            
            _editingProfile = editingProfile;
            _isEditing = editingProfile != null;

            InitializeDialog();
            LoadCurrentSettings();
        }

        private void InitializeDialog()
        {
            try
            {
                if (_isEditing && _editingProfile != null)
                {
                    DialogTitle.Text = "編輯設定檔";
                    ProfileNameTextBox.Text = _editingProfile.ProfileName;
                    DescriptionTextBox.Text = _editingProfile.Description;
                    SaveButton.Content = "更新";
                }
                else
                {
                    DialogTitle.Text = "新增設定檔";
                    SaveButton.Content = "確定";
                }

                // 確保按鈕可見和可用
                SaveButton.Visibility = Visibility.Visible;
                CancelButton.Visibility = Visibility.Visible;
                SaveButton.IsEnabled = !string.IsNullOrWhiteSpace(ProfileNameTextBox.Text);

                // 設置焦點
                ProfileNameTextBox.Focus();
                
                System.Diagnostics.Debug.WriteLine($"對話框初始化完成 - 編輯模式: {_isEditing}, 按鈕可見: {SaveButton.Visibility}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"對話框初始化失敗: {ex.Message}");
                // 確保基本功能可用
                SaveButton.Visibility = Visibility.Visible;
                CancelButton.Visibility = Visibility.Visible;
                SaveButton.IsEnabled = true;
            }
        }

        private void LoadCurrentSettings()
        {
            try
            {
                var settings = AppSettings.Instance;
                CurrentServiceText.Text = $"後端服務：{settings.GetServiceDisplayName()}";
                CurrentApiUrlText.Text = $"API URL：{settings.ApiUrl}";
                CurrentProjectIdText.Text = $"Project ID：{settings.ProjectId}";
            }
            catch (Exception ex)
            {
                CurrentServiceText.Text = "無法載入當前設定";
                CurrentApiUrlText.Text = $"錯誤：{ex.Message}";
                CurrentProjectIdText.Text = "";
            }
        }

        private void ProfileNameTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            ValidateInput();
        }

        private async void ValidateInput()
        {
            try
            {
                var profileName = ProfileNameTextBox.Text.Trim();
                var isValid = true;
                var validationMessage = "";

                // 檢查名稱是否為空
                if (string.IsNullOrWhiteSpace(profileName))
                {
                    isValid = false;
                    validationMessage = "請輸入設定檔名稱";
                }
                // 檢查名稱長度
                else if (profileName.Length > 100)
                {
                    isValid = false;
                    validationMessage = "設定檔名稱不能超過100個字元";
                }
                // 檢查名稱是否重複
                else
                {
                    try
                    {
                        var profileService = SettingsProfileService.Instance;
                        var existingProfile = await profileService.GetProfileByNameAsync(profileName);
                        
                        if (existingProfile != null && 
                            (!_isEditing || existingProfile.Id != _editingProfile?.Id))
                        {
                            isValid = false;
                            validationMessage = "設定檔名稱已存在";
                        }
                    }
                    catch (Exception ex)
                    {
                        // 如果檢查失敗，允許用戶繼續，但顯示警告
                        System.Diagnostics.Debug.WriteLine($"驗證名稱時發生錯誤: {ex.Message}");
                    }
                }

                // 更新驗證訊息
                if (!isValid && !string.IsNullOrEmpty(validationMessage))
                {
                    NameValidationText.Text = validationMessage;
                    NameValidationText.Visibility = Visibility.Visible;
                }
                else
                {
                    NameValidationText.Visibility = Visibility.Collapsed;
                }

                // 更新儲存按鈕狀態 - 只要有名稱就啟用
                SaveButton.IsEnabled = !string.IsNullOrWhiteSpace(profileName);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"驗證輸入時發生錯誤: {ex.Message}");
                // 如果驗證過程出錯，至少確保基本功能可用
                SaveButton.IsEnabled = !string.IsNullOrWhiteSpace(ProfileNameTextBox.Text);
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            var profileName = ProfileNameTextBox.Text.Trim();
            
            if (string.IsNullOrWhiteSpace(profileName))
            {
                MessageBox.Show("請輸入設定檔名稱", "驗證錯誤", MessageBoxButton.OK, MessageBoxImage.Warning);
                ProfileNameTextBox.Focus();
                return;
            }

            if (profileName.Length > 100)
            {
                MessageBox.Show("設定檔名稱不能超過100個字元", "驗證錯誤", MessageBoxButton.OK, MessageBoxImage.Warning);
                ProfileNameTextBox.Focus();
                return;
            }

            // 檢查特殊字元
            var invalidChars = System.IO.Path.GetInvalidFileNameChars().Concat(new char[] { '<', '>', ':', '"', '|', '?', '*' });
            if (profileName.Any(c => invalidChars.Contains(c)))
            {
                MessageBox.Show("設定檔名稱包含無效字元", "驗證錯誤", MessageBoxButton.OK, MessageBoxImage.Warning);
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

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            
            // 確保按鈕可見
            SaveButton.Visibility = Visibility.Visible;
            CancelButton.Visibility = Visibility.Visible;
            
            // 初始驗證 - 如果有預設名稱就啟用按鈕
            var hasName = !string.IsNullOrWhiteSpace(ProfileNameTextBox.Text);
            SaveButton.IsEnabled = hasName;
            
            // 延遲驗證以避免初始化時的異步問題
            Dispatcher.BeginInvoke(new Action(async () =>
            {
                try
                {
                    await Task.Delay(100); // 短暫延遲
                    ValidateInput();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"延遲驗證失敗: {ex.Message}");
                    // 如果驗證失敗，至少確保有名稱時按鈕可用
                    SaveButton.IsEnabled = !string.IsNullOrWhiteSpace(ProfileNameTextBox.Text);
                }
            }));
        }
    }
}