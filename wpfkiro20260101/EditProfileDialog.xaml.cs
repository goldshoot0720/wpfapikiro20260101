using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using wpfkiro20260101.Models;

namespace wpfkiro20260101
{
    public partial class EditProfileDialog : Window
    {
        private SettingsProfile _profile;
        private bool _isLoading = false;

        public string ProfileName => ProfileNameTextBox.Text.Trim();
        public string Description => DescriptionTextBox.Text.Trim();

        public EditProfileDialog(SettingsProfile profile)
        {
            InitializeComponent();
            _profile = profile ?? throw new ArgumentNullException(nameof(profile));
            
            InitializeBackendServiceComboBox();
            LoadProfileData();
            
            this.Loaded += (s, e) => ProfileNameTextBox.Focus();
        }

        private void InitializeBackendServiceComboBox()
        {
            var services = Enum.GetValues(typeof(BackendServiceType))
                              .Cast<BackendServiceType>()
                              .Select(s => new { Value = s, Display = GetServiceDisplayName(s) })
                              .ToList();
            
            BackendServiceComboBox.ItemsSource = services;
            BackendServiceComboBox.DisplayMemberPath = "Display";
            BackendServiceComboBox.SelectedValuePath = "Value";
        }

        private string GetServiceDisplayName(BackendServiceType service)
        {
            return service switch
            {
                BackendServiceType.Appwrite => "Appwrite",
                BackendServiceType.Supabase => "Supabase",
                BackendServiceType.NHost => "NHost",
                BackendServiceType.Contentful => "Contentful",
                BackendServiceType.Back4App => "Back4App",
                BackendServiceType.MySQL => "MySQL",
                BackendServiceType.Strapi => "Strapi",
                BackendServiceType.Sanity => "Sanity",
                _ => service.ToString()
            };
        }

        private void LoadProfileData()
        {
            _isLoading = true;
            
            try
            {
                // 載入基本資訊
                ProfileNameTextBox.Text = _profile.ProfileName;
                DescriptionTextBox.Text = _profile.Description;
                BackendServiceComboBox.SelectedValue = _profile.BackendService;
                
                // 載入連線設定
                LoadConnectionSettings();
                
                // 更新欄位顯示
                UpdateFieldsForService(_profile.BackendService);
                
                UpdateStatusText("設定檔資料載入完成");
            }
            finally
            {
                _isLoading = false;
            }
        }

        private void LoadConnectionSettings()
        {
            ApiUrlTextBox.Text = _profile.ApiUrl;
            ProjectIdTextBox.Text = _profile.ProjectId;
            ApiKeyPasswordBox.Password = _profile.ApiKey;
            DatabaseIdTextBox.Text = _profile.DatabaseId;
            BucketIdTextBox.Text = _profile.BucketId;
            FoodCollectionIdTextBox.Text = _profile.FoodCollectionId;
            SubscriptionCollectionIdTextBox.Text = _profile.SubscriptionCollectionId;
        }

        private void BackendServiceComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_isLoading || BackendServiceComboBox.SelectedValue == null)
                return;

            var selectedService = (BackendServiceType)BackendServiceComboBox.SelectedValue;
            UpdateFieldsForService(selectedService);
            
            // 如果啟用同步，自動載入當前設定
            if (SyncWithCurrentCheckBox.IsChecked == true)
            {
                LoadCurrentSettingsForService(selectedService);
            }
        }

        private void UpdateFieldsForService(BackendServiceType serviceType)
        {
            // 更新標籤文字
            switch (serviceType)
            {
                case BackendServiceType.Appwrite:
                    ApiUrlLabel.Text = "API Endpoint：";
                    ProjectIdLabel.Text = "Project ID：";
                    ShowAppwriteFields(true);
                    break;
                case BackendServiceType.Contentful:
                    ApiUrlLabel.Text = "API URL：";
                    ProjectIdLabel.Text = "Space ID：";
                    ShowAppwriteFields(false);
                    break;
                case BackendServiceType.Back4App:
                    ApiUrlLabel.Text = "API URL：";
                    ProjectIdLabel.Text = "App ID：";
                    ShowAppwriteFields(false);
                    break;
                case BackendServiceType.MySQL:
                    ApiUrlLabel.Text = "Host：";
                    ProjectIdLabel.Text = "Database：";
                    ShowAppwriteFields(false);
                    break;
                default:
                    ApiUrlLabel.Text = "API URL：";
                    ProjectIdLabel.Text = "Project ID：";
                    ShowAppwriteFields(false);
                    break;
            }
        }

        private void ShowAppwriteFields(bool show)
        {
            var visibility = show ? Visibility.Visible : Visibility.Collapsed;
            
            DatabaseIdLabel.Visibility = visibility;
            DatabaseIdTextBox.Visibility = visibility;
            BucketIdLabel.Visibility = visibility;
            BucketIdTextBox.Visibility = visibility;
            FoodCollectionIdLabel.Visibility = visibility;
            FoodCollectionIdTextBox.Visibility = visibility;
            SubscriptionCollectionIdLabel.Visibility = visibility;
            SubscriptionCollectionIdTextBox.Visibility = visibility;
        }

        private void SyncWithCurrentCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (BackendServiceComboBox.SelectedValue != null)
            {
                var selectedService = (BackendServiceType)BackendServiceComboBox.SelectedValue;
                LoadCurrentSettingsForService(selectedService);
                UpdateStatusText("已同步當前連線設定");
            }
        }

        private void SyncWithCurrentCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            UpdateStatusText("已停用自動同步");
        }

        private void LoadCurrentSettings_Click(object sender, RoutedEventArgs e)
        {
            if (BackendServiceComboBox.SelectedValue != null)
            {
                var selectedService = (BackendServiceType)BackendServiceComboBox.SelectedValue;
                LoadCurrentSettingsForService(selectedService);
                UpdateStatusText("已載入當前連線設定");
            }
        }

        private void LoadCurrentSettingsForService(BackendServiceType serviceType)
        {
            var currentSettings = AppSettings.Instance;
            
            // 如果當前服務與選擇的服務相同，直接載入
            if (currentSettings.BackendService == serviceType)
            {
                ApiUrlTextBox.Text = currentSettings.ApiUrl;
                ProjectIdTextBox.Text = currentSettings.ProjectId;
                ApiKeyPasswordBox.Password = currentSettings.ApiKey;
                
                if (serviceType == BackendServiceType.Appwrite)
                {
                    DatabaseIdTextBox.Text = currentSettings.DatabaseId;
                    BucketIdTextBox.Text = currentSettings.BucketId;
                    FoodCollectionIdTextBox.Text = currentSettings.FoodCollectionId;
                    SubscriptionCollectionIdTextBox.Text = currentSettings.SubscriptionCollectionId;
                }
            }
            else
            {
                // 載入指定服務的獨立設定
                var serviceSettings = GetServiceSettings(currentSettings, serviceType);
                ApiUrlTextBox.Text = serviceSettings.ApiUrl;
                ProjectIdTextBox.Text = serviceSettings.ProjectId;
                ApiKeyPasswordBox.Password = serviceSettings.ApiKey;
                
                if (serviceType == BackendServiceType.Appwrite)
                {
                    DatabaseIdTextBox.Text = currentSettings.Appwrite.DatabaseId;
                    BucketIdTextBox.Text = currentSettings.Appwrite.BucketId;
                    FoodCollectionIdTextBox.Text = currentSettings.Appwrite.FoodCollectionId;
                    SubscriptionCollectionIdTextBox.Text = currentSettings.Appwrite.SubscriptionCollectionId;
                }
            }
        }

        private IServiceSettings GetServiceSettings(AppSettings settings, BackendServiceType serviceType)
        {
            return serviceType switch
            {
                BackendServiceType.Appwrite => settings.Appwrite,
                BackendServiceType.Supabase => settings.Supabase,
                BackendServiceType.NHost => settings.NHost,
                BackendServiceType.Contentful => settings.Contentful,
                BackendServiceType.Back4App => settings.Back4App,
                BackendServiceType.MySQL => settings.MySQL,
                BackendServiceType.Strapi => settings.Strapi,
                BackendServiceType.Sanity => settings.Sanity,
                _ => settings.Appwrite
            };
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateInput())
                return;

            try
            {
                // 更新設定檔資料
                _profile.ProfileName = ProfileName;
                _profile.Description = Description;
                _profile.BackendService = (BackendServiceType)BackendServiceComboBox.SelectedValue;
                _profile.ApiUrl = ApiUrlTextBox.Text.Trim();
                _profile.ProjectId = ProjectIdTextBox.Text.Trim();
                _profile.ApiKey = ApiKeyPasswordBox.Password;
                _profile.DatabaseId = DatabaseIdTextBox.Text.Trim();
                _profile.BucketId = BucketIdTextBox.Text.Trim();
                _profile.FoodCollectionId = FoodCollectionIdTextBox.Text.Trim();
                _profile.SubscriptionCollectionId = SubscriptionCollectionIdTextBox.Text.Trim();
                _profile.UpdatedAt = DateTime.UtcNow;

                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                UpdateStatusText($"儲存時發生錯誤：{ex.Message}", System.Windows.Media.Brushes.Red);
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(ProfileName))
            {
                UpdateStatusText("請輸入設定檔名稱", System.Windows.Media.Brushes.Red);
                ProfileNameTextBox.Focus();
                return false;
            }

            if (ProfileName.Length > 100)
            {
                UpdateStatusText("設定檔名稱不能超過100個字元", System.Windows.Media.Brushes.Red);
                ProfileNameTextBox.Focus();
                return false;
            }

            if (BackendServiceComboBox.SelectedValue == null)
            {
                UpdateStatusText("請選擇後端服務", System.Windows.Media.Brushes.Red);
                BackendServiceComboBox.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(ApiUrlTextBox.Text))
            {
                UpdateStatusText("請輸入 API URL", System.Windows.Media.Brushes.Red);
                ApiUrlTextBox.Focus();
                return false;
            }

            var selectedService = (BackendServiceType)BackendServiceComboBox.SelectedValue;
            if (RequiresProjectId(selectedService) && string.IsNullOrWhiteSpace(ProjectIdTextBox.Text))
            {
                var fieldName = GetProjectIdFieldName(selectedService);
                UpdateStatusText($"請輸入 {fieldName}", System.Windows.Media.Brushes.Red);
                ProjectIdTextBox.Focus();
                return false;
            }

            return true;
        }

        private bool RequiresProjectId(BackendServiceType serviceType)
        {
            return serviceType != BackendServiceType.Strapi && serviceType != BackendServiceType.Sanity;
        }

        private string GetProjectIdFieldName(BackendServiceType serviceType)
        {
            return serviceType switch
            {
                BackendServiceType.Contentful => "Space ID",
                BackendServiceType.Back4App => "App ID",
                BackendServiceType.MySQL => "Database Name",
                _ => "Project ID"
            };
        }

        private void UpdateStatusText(string message, System.Windows.Media.Brush? color = null)
        {
            StatusText.Text = message;
            StatusText.Foreground = color ?? System.Windows.Media.Brushes.Green;
            
            // 3秒後清除訊息（除非是錯誤訊息）
            if (color != System.Windows.Media.Brushes.Red)
            {
                var timer = new System.Windows.Threading.DispatcherTimer();
                timer.Interval = TimeSpan.FromSeconds(3);
                timer.Tick += (s, e) =>
                {
                    StatusText.Text = "";
                    timer.Stop();
                };
                timer.Start();
            }
        }
    }
}