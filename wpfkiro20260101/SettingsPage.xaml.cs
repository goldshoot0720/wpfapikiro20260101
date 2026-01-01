using System;
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
            LoadSettings();
        }

        private void LoadSettings()
        {
            var settings = AppSettings.Instance;
            
            // 載入後端服務選擇
            switch (settings.BackendService)
            {
                case BackendServiceType.Appwrite:
                    AppwriteOption.IsChecked = true;
                    break;
                case BackendServiceType.Supabase:
                    SupabaseOption.IsChecked = true;
                    break;
                case BackendServiceType.NHost:
                    NHostOption.IsChecked = true;
                    break;
                case BackendServiceType.Contentful:
                    ContentfulOption.IsChecked = true;
                    break;
                case BackendServiceType.MySQL:
                    MySQLOption.IsChecked = true;
                    break;
            }

            // 載入連線設定
            ApiUrlTextBox.Text = settings.ApiUrl;
            ProjectIdTextBox.Text = settings.ProjectId;
            ApiKeyPasswordBox.Password = settings.ApiKey;

            // 根據選擇的服務更新預設值
            UpdateDefaultValues();
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
                else if (MySQLOption.IsChecked == true)
                    settings.BackendService = BackendServiceType.MySQL;

                // 儲存連線設定
                settings.ApiUrl = ApiUrlTextBox.Text;
                settings.ProjectId = ProjectIdTextBox.Text;
                settings.ApiKey = ApiKeyPasswordBox.Password;

                // 儲存到檔案
                settings.Save();

                ShowStatusMessage("設定已成功儲存！", Brushes.Green);
            }
            catch (Exception ex)
            {
                ShowStatusMessage($"儲存設定時發生錯誤：{ex.Message}", Brushes.Red);
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
                    else if (MySQLOption.IsChecked == true)
                        tempSettings.BackendService = BackendServiceType.MySQL;

                    tempSettings.ApiUrl = ApiUrlTextBox.Text;
                    tempSettings.ProjectId = ProjectIdTextBox.Text;
                    tempSettings.ApiKey = ApiKeyPasswordBox.Password;

                    if (string.IsNullOrWhiteSpace(tempSettings.ApiUrl))
                    {
                        ShowStatusMessage("請填寫 API URL", Brushes.Orange);
                        return;
                    }

                    // 根據服務類型檢查必要欄位
                    if ((tempSettings.BackendService == BackendServiceType.Appwrite || 
                         tempSettings.BackendService == BackendServiceType.NHost ||
                         tempSettings.BackendService == BackendServiceType.Contentful ||
                         tempSettings.BackendService == BackendServiceType.MySQL) && 
                        string.IsNullOrWhiteSpace(tempSettings.ProjectId))
                    {
                        var fieldName = tempSettings.BackendService == BackendServiceType.Contentful ? "Space ID" :
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

        private void UpdateDefaultValues()
        {
            var settings = AppSettings.Instance;
            
            // 如果欄位為空，填入預設值
            if (string.IsNullOrWhiteSpace(ApiUrlTextBox.Text))
            {
                ApiUrlTextBox.Text = settings.GetDefaultApiUrl();
            }
            
            if (string.IsNullOrWhiteSpace(ProjectIdTextBox.Text))
            {
                ProjectIdTextBox.Text = settings.GetDefaultProjectId();
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
            if (sender is RadioButton radioButton)
            {
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
                else if (radioButton == MySQLOption)
                    selectedService = BackendServiceType.MySQL;

                UpdateFieldsForService(selectedService);
            }
        }

        private void UpdateFieldsForService(BackendServiceType serviceType)
        {
            // 更新標籤文字
            switch (serviceType)
            {
                case BackendServiceType.Contentful:
                    ProjectIdLabel.Text = "Space ID:";
                    break;
                case BackendServiceType.MySQL:
                    ProjectIdLabel.Text = "Database:";
                    break;
                case BackendServiceType.Supabase:
                    ProjectIdLabel.Text = "Project ID:";
                    break;
                default:
                    ProjectIdLabel.Text = "Project ID:";
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
                   url == AppSettings.Defaults.MySQL.ApiUrl;
        }

        private bool IsDefaultProjectId(string projectId)
        {
            return projectId == AppSettings.Defaults.Appwrite.ProjectId ||
                   projectId == AppSettings.Defaults.NHost.ProjectId ||
                   projectId == AppSettings.Defaults.Contentful.ProjectId ||
                   projectId == AppSettings.Defaults.MySQL.ProjectId;
        }
    }
}