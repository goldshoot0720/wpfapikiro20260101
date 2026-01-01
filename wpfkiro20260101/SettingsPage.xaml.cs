using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using wpfkiro20260101.Services;

namespace wpfkiro20260101
{
    public partial class SettingsPage : Page
    {
        public SettingsPage()
        {
            InitializeComponent();
            
            // 調試：測試 AppSettings 單例
            var settings1 = AppSettings.Instance;
            var settings2 = AppSettings.Instance;
            System.Diagnostics.Debug.WriteLine($"單例測試 - 是否為同一實例: {ReferenceEquals(settings1, settings2)}");
            System.Diagnostics.Debug.WriteLine($"建構函式 - 當前後端服務: {settings1.BackendService}");
            
            LoadSettings();
            
            // 確保在頁面載入後正確顯示欄位
            this.Loaded += (s, e) => 
            {
                var settings = AppSettings.Instance;
                System.Diagnostics.Debug.WriteLine($"頁面載入事件 - 當前後端服務: {settings.BackendService}");
                UpdateFieldsForService(settings.BackendService);
            };
        }

        private void LoadSettings()
        {
            var settings = AppSettings.Instance;
            
            // 調試：顯示載入的後端服務
            System.Diagnostics.Debug.WriteLine($"LoadSettings - 載入後端服務: {settings.BackendService}");
            
            // 暫時移除事件處理器，避免在載入時觸發保存
            AppwriteOption.Checked -= BackendOption_Checked;
            SupabaseOption.Checked -= BackendOption_Checked;
            NHostOption.Checked -= BackendOption_Checked;
            ContentfulOption.Checked -= BackendOption_Checked;
            Back4AppOption.Checked -= BackendOption_Checked;
            MySQLOption.Checked -= BackendOption_Checked;
            
            // 先清除所有選項
            AppwriteOption.IsChecked = false;
            SupabaseOption.IsChecked = false;
            NHostOption.IsChecked = false;
            ContentfulOption.IsChecked = false;
            Back4AppOption.IsChecked = false;
            MySQLOption.IsChecked = false;
            
            // 載入後端服務選擇
            switch (settings.BackendService)
            {
                case BackendServiceType.Appwrite:
                    AppwriteOption.IsChecked = true;
                    System.Diagnostics.Debug.WriteLine("設定 Appwrite 為選中");
                    break;
                case BackendServiceType.Supabase:
                    SupabaseOption.IsChecked = true;
                    System.Diagnostics.Debug.WriteLine("設定 Supabase 為選中");
                    break;
                case BackendServiceType.NHost:
                    NHostOption.IsChecked = true;
                    System.Diagnostics.Debug.WriteLine("設定 NHost 為選中");
                    break;
                case BackendServiceType.Contentful:
                    ContentfulOption.IsChecked = true;
                    System.Diagnostics.Debug.WriteLine("設定 Contentful 為選中");
                    break;
                case BackendServiceType.Back4App:
                    Back4AppOption.IsChecked = true;
                    System.Diagnostics.Debug.WriteLine("設定 Back4App 為選中");
                    break;
                case BackendServiceType.MySQL:
                    MySQLOption.IsChecked = true;
                    System.Diagnostics.Debug.WriteLine("設定 MySQL 為選中");
                    break;
                default:
                    System.Diagnostics.Debug.WriteLine($"未知的後端服務類型: {settings.BackendService}");
                    // 如果是未知類型，預設選擇 Appwrite
                    AppwriteOption.IsChecked = true;
                    break;
            }
            
            // 重新添加事件處理器
            AppwriteOption.Checked += BackendOption_Checked;
            SupabaseOption.Checked += BackendOption_Checked;
            NHostOption.Checked += BackendOption_Checked;
            ContentfulOption.Checked += BackendOption_Checked;
            Back4AppOption.Checked += BackendOption_Checked;
            MySQLOption.Checked += BackendOption_Checked;
            
            // 驗證選擇狀態
            System.Diagnostics.Debug.WriteLine($"RadioButton 狀態驗證:");
            System.Diagnostics.Debug.WriteLine($"Appwrite: {AppwriteOption.IsChecked}");
            System.Diagnostics.Debug.WriteLine($"Supabase: {SupabaseOption.IsChecked}");
            System.Diagnostics.Debug.WriteLine($"NHost: {NHostOption.IsChecked}");
            System.Diagnostics.Debug.WriteLine($"Contentful: {ContentfulOption.IsChecked}");
            System.Diagnostics.Debug.WriteLine($"Back4App: {Back4AppOption.IsChecked}");
            System.Diagnostics.Debug.WriteLine($"MySQL: {MySQLOption.IsChecked}");

            // 載入連線設定
            ApiUrlTextBox.Text = settings.ApiUrl;
            ProjectIdTextBox.Text = settings.ProjectId;
            ApiKeyPasswordBox.Password = settings.ApiKey;
            DatabaseIdTextBox.Text = settings.DatabaseId;
            BucketIdTextBox.Text = settings.BucketId;

            // 根據選擇的服務更新欄位顯示和預設值
            UpdateFieldsForService(settings.BackendService);
            
            // 顯示載入的後端服務（用於調試）
            ShowStatusMessage($"已載入設定 - 後端服務: {settings.BackendService}", Brushes.Blue);
        }

        private void SaveSettings_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var settings = AppSettings.Instance;

                // 儲存後端服務選擇
                if (AppwriteOption.IsChecked == true)
                    settings.BackendService = BackendServiceType.Appwrite;
                else if (SupabaseOption.IsChecked == true)
                    settings.BackendService = BackendServiceType.Supabase;
                else if (NHostOption.IsChecked == true)
                    settings.BackendService = BackendServiceType.NHost;
                else if (ContentfulOption.IsChecked == true)
                    settings.BackendService = BackendServiceType.Contentful;
                else if (Back4AppOption.IsChecked == true)
                    settings.BackendService = BackendServiceType.Back4App;
                else if (MySQLOption.IsChecked == true)
                    settings.BackendService = BackendServiceType.MySQL;

                // 調試：顯示將要儲存的後端服務
                System.Diagnostics.Debug.WriteLine($"儲存後端服務: {settings.BackendService}");

                // 儲存連線設定
                settings.ApiUrl = ApiUrlTextBox.Text;
                settings.ProjectId = ProjectIdTextBox.Text;
                settings.ApiKey = ApiKeyPasswordBox.Password;
                settings.DatabaseId = DatabaseIdTextBox.Text;
                settings.BucketId = BucketIdTextBox.Text;

                // 儲存到檔案
                settings.Save();

                ShowStatusMessage($"設定已成功儲存！後端服務: {settings.BackendService}", Brushes.Green);
            }
            catch (Exception ex)
            {
                ShowStatusMessage($"儲存設定時發生錯誤：{ex.Message}", Brushes.Red);
            }
        }

        private void TestSettings_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // 強制重新載入設定
                AppSettings.ReloadSettings();
                var settings = AppSettings.Instance;
                
                // 讀取原始 JSON 檔案內容
                var settingsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "wpfkiro20260101", "settings.json");
                var jsonContent = File.Exists(settingsPath) ? File.ReadAllText(settingsPath) : "檔案不存在";
                
                // 顯示當前設定狀態
                var message = $"原始 JSON 內容:\n{jsonContent}\n\n" +
                             $"載入後的設定狀態:\n" +
                             $"後端服務: {settings.BackendService}\n" +
                             $"API URL: {settings.ApiUrl}\n" +
                             $"Project ID: {settings.ProjectId}\n\n" +
                             $"RadioButton 狀態:\n" +
                             $"Appwrite: {AppwriteOption.IsChecked}\n" +
                             $"Supabase: {SupabaseOption.IsChecked}\n" +
                             $"NHost: {NHostOption.IsChecked}\n" +
                             $"Contentful: {ContentfulOption.IsChecked}\n" +
                             $"Back4App: {Back4AppOption.IsChecked}\n" +
                             $"MySQL: {MySQLOption.IsChecked}";
                
                MessageBox.Show(message, "設定測試", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"測試設定時發生錯誤：{ex.Message}", "錯誤", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void TestConnection_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                TestConnectionButton.IsEnabled = false;
                TestConnectionButton.Content = "測試中...";
                ShowStatusMessage("正在測試連線...", Brushes.Blue);

                // 暫時更新設定以進行測試
                var tempSettings = AppSettings.Instance;
                var originalApiUrl = tempSettings.ApiUrl;
                var originalProjectId = tempSettings.ProjectId;
                var originalApiKey = tempSettings.ApiKey;
                var originalService = tempSettings.BackendService;

                try
                {
                    // 使用當前表單的值進行測試
                    if (AppwriteOption.IsChecked == true)
                        tempSettings.BackendService = BackendServiceType.Appwrite;
                    else if (SupabaseOption.IsChecked == true)
                        tempSettings.BackendService = BackendServiceType.Supabase;
                    else if (NHostOption.IsChecked == true)
                        tempSettings.BackendService = BackendServiceType.NHost;
                    else if (ContentfulOption.IsChecked == true)
                        tempSettings.BackendService = BackendServiceType.Contentful;
                    else if (Back4AppOption.IsChecked == true)
                        tempSettings.BackendService = BackendServiceType.Back4App;
                    else if (MySQLOption.IsChecked == true)
                        tempSettings.BackendService = BackendServiceType.MySQL;

                    tempSettings.ApiUrl = ApiUrlTextBox.Text;
                    tempSettings.ProjectId = ProjectIdTextBox.Text;
                    tempSettings.ApiKey = ApiKeyPasswordBox.Password;
                    tempSettings.DatabaseId = DatabaseIdTextBox.Text;
                    tempSettings.BucketId = BucketIdTextBox.Text;

                    if (string.IsNullOrWhiteSpace(tempSettings.ApiUrl))
                    {
                        ShowStatusMessage("請填寫 API URL", Brushes.Orange);
                        return;
                    }

                    // 根據服務類型檢查必要欄位
                    if ((tempSettings.BackendService == BackendServiceType.Appwrite || 
                         tempSettings.BackendService == BackendServiceType.NHost ||
                         tempSettings.BackendService == BackendServiceType.Contentful ||
                         tempSettings.BackendService == BackendServiceType.Back4App ||
                         tempSettings.BackendService == BackendServiceType.MySQL) && 
                        string.IsNullOrWhiteSpace(tempSettings.ProjectId))
                    {
                        var fieldName = tempSettings.BackendService == BackendServiceType.Contentful ? "Space ID" :
                                       tempSettings.BackendService == BackendServiceType.Back4App ? "App ID" :
                                       tempSettings.BackendService == BackendServiceType.MySQL ? "Database Name" : "Project ID";
                        ShowStatusMessage($"請填寫 {fieldName}", Brushes.Orange);
                        return;
                    }

                    // 創建服務並測試連線
                    var service = BackendServiceFactory.CreateCurrentService();
                    bool connectionSuccess = await service.TestConnectionAsync();

                    if (connectionSuccess)
                    {
                        ShowStatusMessage($"連線測試成功！({service.ServiceName})", Brushes.Green);
                    }
                    else
                    {
                        ShowStatusMessage($"連線測試失敗，請檢查 {service.ServiceName} 設定", Brushes.Red);
                    }
                }
                finally
                {
                    // 恢復原始設定
                    tempSettings.ApiUrl = originalApiUrl;
                    tempSettings.ProjectId = originalProjectId;
                    tempSettings.ApiKey = originalApiKey;
                    tempSettings.BackendService = originalService;
                }
            }
            catch (Exception ex)
            {
                ShowStatusMessage($"連線測試時發生錯誤：{ex.Message}", Brushes.Red);
            }
            finally
            {
                TestConnectionButton.IsEnabled = true;
                TestConnectionButton.Content = "測試連線";
            }
        }

        private void ShowStatusMessage(string message, Brush color)
        {
            StatusMessage.Text = message;
            StatusMessage.Foreground = color;
            
            // 3秒後清除訊息
            var timer = new System.Windows.Threading.DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(3);
            timer.Tick += (s, e) =>
            {
                StatusMessage.Text = "";
                timer.Stop();
            };
            timer.Start();
        }

        private void BackendOption_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is RadioButton radioButton && radioButton.IsChecked == true)
            {
                // 手動處理互斥選擇
                if (radioButton == AppwriteOption)
                {
                    SupabaseOption.IsChecked = false;
                    NHostOption.IsChecked = false;
                    ContentfulOption.IsChecked = false;
                    Back4AppOption.IsChecked = false;
                    MySQLOption.IsChecked = false;
                }
                else if (radioButton == SupabaseOption)
                {
                    AppwriteOption.IsChecked = false;
                    NHostOption.IsChecked = false;
                    ContentfulOption.IsChecked = false;
                    Back4AppOption.IsChecked = false;
                    MySQLOption.IsChecked = false;
                }
                else if (radioButton == NHostOption)
                {
                    AppwriteOption.IsChecked = false;
                    SupabaseOption.IsChecked = false;
                    ContentfulOption.IsChecked = false;
                    Back4AppOption.IsChecked = false;
                    MySQLOption.IsChecked = false;
                }
                else if (radioButton == ContentfulOption)
                {
                    AppwriteOption.IsChecked = false;
                    SupabaseOption.IsChecked = false;
                    NHostOption.IsChecked = false;
                    Back4AppOption.IsChecked = false;
                    MySQLOption.IsChecked = false;
                }
                else if (radioButton == Back4AppOption)
                {
                    AppwriteOption.IsChecked = false;
                    SupabaseOption.IsChecked = false;
                    NHostOption.IsChecked = false;
                    ContentfulOption.IsChecked = false;
                    MySQLOption.IsChecked = false;
                }
                else if (radioButton == MySQLOption)
                {
                    AppwriteOption.IsChecked = false;
                    SupabaseOption.IsChecked = false;
                    NHostOption.IsChecked = false;
                    ContentfulOption.IsChecked = false;
                    Back4AppOption.IsChecked = false;
                }

                // 根據選擇的服務更新預設值
                BackendServiceType selectedService = BackendServiceType.Appwrite;
                
                if (radioButton == AppwriteOption)
                    selectedService = BackendServiceType.Appwrite;
                else if (radioButton == SupabaseOption)
                    selectedService = BackendServiceType.Supabase;
                else if (radioButton == NHostOption)
                    selectedService = BackendServiceType.NHost;
                else if (radioButton == ContentfulOption)
                    selectedService = BackendServiceType.Contentful;
                else if (radioButton == Back4AppOption)
                    selectedService = BackendServiceType.Back4App;
                else if (radioButton == MySQLOption)
                    selectedService = BackendServiceType.MySQL;

                // 即時保存後端服務選擇
                try
                {
                    var settings = AppSettings.Instance;
                    settings.BackendService = selectedService;
                    settings.Save();
                    
                    System.Diagnostics.Debug.WriteLine($"即時保存後端服務: {selectedService}");
                    ShowStatusMessage($"已切換至 {settings.GetServiceDisplayName()}", Brushes.Green);
                }
                catch (Exception ex)
                {
                    ShowStatusMessage($"保存設定時發生錯誤：{ex.Message}", Brushes.Red);
                    System.Diagnostics.Debug.WriteLine($"即時保存失敗: {ex.Message}");
                }

                UpdateFieldsForService(selectedService);
            }
        }

        private void UpdateFieldsForService(BackendServiceType serviceType)
        {
            // 更新標籤文字
            switch (serviceType)
            {
                case BackendServiceType.Appwrite:
                    ApiUrlLabel.Text = "API Endpoint:";
                    ProjectIdLabel.Text = "Project ID:";
                    // 顯示 Appwrite 專用欄位
                    DatabaseIdLabel.Visibility = System.Windows.Visibility.Visible;
                    DatabaseIdTextBox.Visibility = System.Windows.Visibility.Visible;
                    BucketIdLabel.Visibility = System.Windows.Visibility.Visible;
                    BucketIdTextBox.Visibility = System.Windows.Visibility.Visible;
                    break;
                case BackendServiceType.Contentful:
                    ApiUrlLabel.Text = "API URL:";
                    ProjectIdLabel.Text = "Space ID:";
                    // 隱藏 Appwrite 專用欄位
                    DatabaseIdLabel.Visibility = System.Windows.Visibility.Collapsed;
                    DatabaseIdTextBox.Visibility = System.Windows.Visibility.Collapsed;
                    BucketIdLabel.Visibility = System.Windows.Visibility.Collapsed;
                    BucketIdTextBox.Visibility = System.Windows.Visibility.Collapsed;
                    break;
                case BackendServiceType.Back4App:
                    ApiUrlLabel.Text = "API URL:";
                    ProjectIdLabel.Text = "App ID:";
                    // 隱藏 Appwrite 專用欄位
                    DatabaseIdLabel.Visibility = System.Windows.Visibility.Collapsed;
                    DatabaseIdTextBox.Visibility = System.Windows.Visibility.Collapsed;
                    BucketIdLabel.Visibility = System.Windows.Visibility.Collapsed;
                    BucketIdTextBox.Visibility = System.Windows.Visibility.Collapsed;
                    break;
                case BackendServiceType.MySQL:
                    ApiUrlLabel.Text = "API URL:";
                    ProjectIdLabel.Text = "Database:";
                    // 隱藏 Appwrite 專用欄位
                    DatabaseIdLabel.Visibility = System.Windows.Visibility.Collapsed;
                    DatabaseIdTextBox.Visibility = System.Windows.Visibility.Collapsed;
                    BucketIdLabel.Visibility = System.Windows.Visibility.Collapsed;
                    BucketIdTextBox.Visibility = System.Windows.Visibility.Collapsed;
                    break;
                case BackendServiceType.Supabase:
                    ApiUrlLabel.Text = "API URL:";
                    ProjectIdLabel.Text = "Project ID:";
                    // 隱藏 Appwrite 專用欄位
                    DatabaseIdLabel.Visibility = System.Windows.Visibility.Collapsed;
                    DatabaseIdTextBox.Visibility = System.Windows.Visibility.Collapsed;
                    BucketIdLabel.Visibility = System.Windows.Visibility.Collapsed;
                    BucketIdTextBox.Visibility = System.Windows.Visibility.Collapsed;
                    break;
                default:
                    ApiUrlLabel.Text = "API URL:";
                    ProjectIdLabel.Text = "Project ID:";
                    // 隱藏 Appwrite 專用欄位
                    DatabaseIdLabel.Visibility = System.Windows.Visibility.Collapsed;
                    DatabaseIdTextBox.Visibility = System.Windows.Visibility.Collapsed;
                    BucketIdLabel.Visibility = System.Windows.Visibility.Collapsed;
                    BucketIdTextBox.Visibility = System.Windows.Visibility.Collapsed;
                    break;
            }

            // 如果欄位為空或為預設值，則更新為新服務的預設值
            switch (serviceType)
            {
                case BackendServiceType.Appwrite:
                    if (string.IsNullOrWhiteSpace(ApiUrlTextBox.Text) || 
                        IsDefaultUrl(ApiUrlTextBox.Text))
                    {
                        ApiUrlTextBox.Text = AppSettings.Defaults.Appwrite.ApiUrl;
                    }
                    if (string.IsNullOrWhiteSpace(ProjectIdTextBox.Text) ||
                        IsDefaultProjectId(ProjectIdTextBox.Text))
                    {
                        ProjectIdTextBox.Text = AppSettings.Defaults.Appwrite.ProjectId;
                    }
                    if (string.IsNullOrWhiteSpace(DatabaseIdTextBox.Text))
                    {
                        DatabaseIdTextBox.Text = AppSettings.Defaults.Appwrite.DatabaseId;
                    }
                    if (string.IsNullOrWhiteSpace(BucketIdTextBox.Text))
                    {
                        BucketIdTextBox.Text = AppSettings.Defaults.Appwrite.BucketId;
                    }
                    if (string.IsNullOrWhiteSpace(ApiKeyPasswordBox.Password))
                    {
                        ApiKeyPasswordBox.Password = AppSettings.Defaults.Appwrite.ApiKey;
                    }
                    break;

                case BackendServiceType.Supabase:
                    if (string.IsNullOrWhiteSpace(ApiUrlTextBox.Text) || 
                        IsDefaultUrl(ApiUrlTextBox.Text))
                    {
                        ApiUrlTextBox.Text = AppSettings.Defaults.Supabase.ApiUrl;
                    }
                    // Supabase 不需要 Project ID，清空該欄位
                    if (IsDefaultProjectId(ProjectIdTextBox.Text))
                    {
                        ProjectIdTextBox.Text = "";
                    }
                    break;

                case BackendServiceType.NHost:
                    if (string.IsNullOrWhiteSpace(ApiUrlTextBox.Text) || 
                        IsDefaultUrl(ApiUrlTextBox.Text))
                    {
                        ApiUrlTextBox.Text = AppSettings.Defaults.NHost.ApiUrl;
                    }
                    if (string.IsNullOrWhiteSpace(ProjectIdTextBox.Text) ||
                        IsDefaultProjectId(ProjectIdTextBox.Text))
                    {
                        ProjectIdTextBox.Text = AppSettings.Defaults.NHost.ProjectId;
                    }
                    break;

                case BackendServiceType.Contentful:
                    if (string.IsNullOrWhiteSpace(ApiUrlTextBox.Text) || 
                        IsDefaultUrl(ApiUrlTextBox.Text))
                    {
                        ApiUrlTextBox.Text = AppSettings.Defaults.Contentful.ApiUrl;
                    }
                    if (string.IsNullOrWhiteSpace(ProjectIdTextBox.Text) ||
                        IsDefaultProjectId(ProjectIdTextBox.Text))
                    {
                        ProjectIdTextBox.Text = AppSettings.Defaults.Contentful.ProjectId;
                    }
                    break;

                case BackendServiceType.Back4App:
                    if (string.IsNullOrWhiteSpace(ApiUrlTextBox.Text) || 
                        IsDefaultUrl(ApiUrlTextBox.Text))
                    {
                        ApiUrlTextBox.Text = AppSettings.Defaults.Back4App.ApiUrl;
                    }
                    if (string.IsNullOrWhiteSpace(ProjectIdTextBox.Text) ||
                        IsDefaultProjectId(ProjectIdTextBox.Text))
                    {
                        ProjectIdTextBox.Text = AppSettings.Defaults.Back4App.ProjectId;
                    }
                    break;

                case BackendServiceType.MySQL:
                    if (string.IsNullOrWhiteSpace(ApiUrlTextBox.Text) || 
                        IsDefaultUrl(ApiUrlTextBox.Text))
                    {
                        ApiUrlTextBox.Text = AppSettings.Defaults.MySQL.ApiUrl;
                    }
                    if (string.IsNullOrWhiteSpace(ProjectIdTextBox.Text) ||
                        IsDefaultProjectId(ProjectIdTextBox.Text))
                    {
                        ProjectIdTextBox.Text = AppSettings.Defaults.MySQL.ProjectId;
                    }
                    break;
            }
        }

        private bool IsDefaultUrl(string url)
        {
            return url == AppSettings.Defaults.Appwrite.ApiUrl ||
                   url == AppSettings.Defaults.Supabase.ApiUrl ||
                   url == AppSettings.Defaults.NHost.ApiUrl ||
                   url == AppSettings.Defaults.Contentful.ApiUrl ||
                   url == AppSettings.Defaults.Back4App.ApiUrl ||
                   url == AppSettings.Defaults.MySQL.ApiUrl;
        }

        private bool IsDefaultProjectId(string projectId)
        {
            return projectId == AppSettings.Defaults.Appwrite.ProjectId ||
                   projectId == AppSettings.Defaults.NHost.ProjectId ||
                   projectId == AppSettings.Defaults.Contentful.ProjectId ||
                   projectId == AppSettings.Defaults.Back4App.ProjectId ||
                   projectId == AppSettings.Defaults.MySQL.ProjectId;
        }
    }
}