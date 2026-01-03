using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using wpfkiro20260101.Services;
using Brushes = System.Windows.Media.Brushes;
using MessageBox = System.Windows.MessageBox;
using RadioButton = System.Windows.Controls.RadioButton;

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
            
            // 訂閱設定變更事件
            AppSettings.SettingsChanged += OnSettingsChanged;
            
            // 確保在頁面載入後正確顯示欄位
            this.Loaded += (s, e) => 
            {
                var settings = AppSettings.Instance;
                System.Diagnostics.Debug.WriteLine($"頁面載入事件 - 當前後端服務: {settings.BackendService}");
                UpdateFieldsForService(settings.BackendService);
            };
        }

        private void OnSettingsChanged()
        {
            // 在 UI 線程上重新載入設定
            Dispatcher.Invoke(() =>
            {
                LoadSettings();
                ShowStatusMessage("設定檔已載入，界面已更新", System.Windows.Media.Brushes.Green);
            });
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
            StrapiOption.Checked -= BackendOption_Checked;
            SanityOption.Checked -= BackendOption_Checked;
            
            // 先清除所有選項
            AppwriteOption.IsChecked = false;
            SupabaseOption.IsChecked = false;
            NHostOption.IsChecked = false;
            ContentfulOption.IsChecked = false;
            Back4AppOption.IsChecked = false;
            MySQLOption.IsChecked = false;
            StrapiOption.IsChecked = false;
            SanityOption.IsChecked = false;
            
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
                case BackendServiceType.Strapi:
                    StrapiOption.IsChecked = true;
                    System.Diagnostics.Debug.WriteLine("設定 Strapi 為選中");
                    break;
                case BackendServiceType.Sanity:
                    SanityOption.IsChecked = true;
                    System.Diagnostics.Debug.WriteLine("設定 Sanity 為選中");
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
            StrapiOption.Checked += BackendOption_Checked;
            SanityOption.Checked += BackendOption_Checked;
            
            // 驗證選擇狀態
            System.Diagnostics.Debug.WriteLine($"RadioButton 狀態驗證:");
            System.Diagnostics.Debug.WriteLine($"Appwrite: {AppwriteOption.IsChecked}");
            System.Diagnostics.Debug.WriteLine($"Supabase: {SupabaseOption.IsChecked}");
            System.Diagnostics.Debug.WriteLine($"NHost: {NHostOption.IsChecked}");
            System.Diagnostics.Debug.WriteLine($"Contentful: {ContentfulOption.IsChecked}");
            System.Diagnostics.Debug.WriteLine($"Back4App: {Back4AppOption.IsChecked}");
            System.Diagnostics.Debug.WriteLine($"MySQL: {MySQLOption.IsChecked}");
            System.Diagnostics.Debug.WriteLine($"Strapi: {StrapiOption.IsChecked}");
            System.Diagnostics.Debug.WriteLine($"Sanity: {SanityOption.IsChecked}");

            // 載入連線設定
            ApiUrlTextBox.Text = settings.ApiUrl;
            ProjectIdTextBox.Text = settings.ProjectId;
            ApiKeyPasswordBox.Password = settings.ApiKey;
            DatabaseIdTextBox.Text = settings.DatabaseId;
            BucketIdTextBox.Text = settings.BucketId;
            FoodCollectionIdTextBox.Text = settings.FoodCollectionId;
            SubscriptionCollectionIdTextBox.Text = settings.SubscriptionCollectionId;

            // 特別處理 Supabase 設定，確保使用正確的值
            if (settings.BackendService == BackendServiceType.Supabase)
            {
                // 強制使用正確的 Supabase 設定
                if (settings.ProjectId != AppSettings.Defaults.Supabase.ProjectId ||
                    settings.ApiUrl != AppSettings.Defaults.Supabase.ApiUrl)
                {
                    System.Diagnostics.Debug.WriteLine("檢測到舊的 Supabase 設定，正在更新...");
                    settings.ApiUrl = AppSettings.Defaults.Supabase.ApiUrl;
                    settings.ProjectId = AppSettings.Defaults.Supabase.ProjectId;
                    settings.ApiKey = AppSettings.Defaults.Supabase.ApiKey;
                    settings.Save();
                    
                    // 更新界面顯示
                    ApiUrlTextBox.Text = settings.ApiUrl;
                    ProjectIdTextBox.Text = settings.ProjectId;
                    ApiKeyPasswordBox.Password = settings.ApiKey;
                }
            }

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
                else if (StrapiOption.IsChecked == true)
                    settings.BackendService = BackendServiceType.Strapi;
                else if (SanityOption.IsChecked == true)
                    settings.BackendService = BackendServiceType.Sanity;

                // 調試：顯示將要儲存的後端服務
                System.Diagnostics.Debug.WriteLine($"儲存後端服務: {settings.BackendService}");

                // 儲存連線設定
                settings.ApiUrl = ApiUrlTextBox.Text;
                settings.ProjectId = ProjectIdTextBox.Text;
                settings.ApiKey = ApiKeyPasswordBox.Password;
                settings.DatabaseId = DatabaseIdTextBox.Text;
                settings.BucketId = BucketIdTextBox.Text;
                settings.FoodCollectionId = FoodCollectionIdTextBox.Text;
                settings.SubscriptionCollectionId = SubscriptionCollectionIdTextBox.Text;

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
                // 獲取當前界面上選擇的後端服務
                BackendServiceType currentSelectedService = BackendServiceType.Appwrite;
                
                if (AppwriteOption.IsChecked == true)
                    currentSelectedService = BackendServiceType.Appwrite;
                else if (SupabaseOption.IsChecked == true)
                    currentSelectedService = BackendServiceType.Supabase;
                else if (NHostOption.IsChecked == true)
                    currentSelectedService = BackendServiceType.NHost;
                else if (ContentfulOption.IsChecked == true)
                    currentSelectedService = BackendServiceType.Contentful;
                else if (Back4AppOption.IsChecked == true)
                    currentSelectedService = BackendServiceType.Back4App;
                else if (MySQLOption.IsChecked == true)
                    currentSelectedService = BackendServiceType.MySQL;
                else if (StrapiOption.IsChecked == true)
                    currentSelectedService = BackendServiceType.Strapi;
                else if (SanityOption.IsChecked == true)
                    currentSelectedService = BackendServiceType.Sanity;

                // 獲取當前界面上的設定值
                var currentSettings = new
                {
                    BackendService = (int)currentSelectedService,
                    ApiUrl = ApiUrlTextBox.Text,
                    ProjectId = ProjectIdTextBox.Text,
                    ApiKey = ApiKeyPasswordBox.Password,
                    DatabaseId = DatabaseIdTextBox.Text,
                    BucketId = BucketIdTextBox.Text
                };

                // 序列化當前設定為 JSON
                var currentJson = System.Text.Json.JsonSerializer.Serialize(currentSettings, new System.Text.Json.JsonSerializerOptions
                {
                    WriteIndented = true
                });

                // 讀取已儲存的設定檔案內容
                var settingsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "wpfkiro20260101", "settings.json");
                var savedJsonContent = File.Exists(settingsPath) ? File.ReadAllText(settingsPath) : "檔案不存在";
                
                // 載入已儲存的設定
                AppSettings.ReloadSettings();
                var savedSettings = AppSettings.Instance;
                
                // 顯示當前設定狀態
                var message = $"當前界面選擇的設定:\n{currentJson}\n\n" +
                             $"已儲存的設定檔案內容:\n{savedJsonContent}\n\n" +
                             $"載入後的設定狀態:\n" +
                             $"後端服務: {savedSettings.BackendService}\n" +
                             $"API URL: {savedSettings.ApiUrl}\n" +
                             $"Project ID: {savedSettings.ProjectId}\n\n" +
                             $"RadioButton 狀態:\n" +
                             $"Appwrite: {AppwriteOption.IsChecked}\n" +
                             $"Supabase: {SupabaseOption.IsChecked}\n" +
                             $"NHost: {NHostOption.IsChecked}\n" +
                             $"Contentful: {ContentfulOption.IsChecked}\n" +
                             $"Back4App: {Back4AppOption.IsChecked}\n" +
                             $"MySQL: {MySQLOption.IsChecked}\n" +
                             $"Strapi: {StrapiOption.IsChecked}\n" +
                             $"Sanity: {SanityOption.IsChecked}";
                
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
                        
                        // 如果是 Supabase，執行額外的診斷
                        if (tempSettings.BackendService == BackendServiceType.Supabase)
                        {
                            ShowStatusMessage("Supabase 連線成功，正在執行修正後的測試...", Brushes.Blue);
                            await TestSupabaseHeaderFix.RunHeaderFixTest();
                            ShowStatusMessage("Supabase 測試完成，請查看控制台輸出", Brushes.Green);
                        }
                    }
                    else
                    {
                        ShowStatusMessage($"連線測試失敗，請檢查 {service.ServiceName} 設定", Brushes.Red);
                        
                        // 如果是 Supabase 連線失敗，提供診斷建議
                        if (tempSettings.BackendService == BackendServiceType.Supabase)
                        {
                            ShowStatusMessage("Supabase 連線失敗，正在執行修正後的診斷...", Brushes.Orange);
                            await TestSupabaseHeaderFix.RunHeaderFixTest();
                        }
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

        private void ShowStatusMessage(string message, System.Windows.Media.Brush color)
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
                    StrapiOption.IsChecked = false;
                    SanityOption.IsChecked = false;
                }
                else if (radioButton == SupabaseOption)
                {
                    AppwriteOption.IsChecked = false;
                    NHostOption.IsChecked = false;
                    ContentfulOption.IsChecked = false;
                    Back4AppOption.IsChecked = false;
                    MySQLOption.IsChecked = false;
                    StrapiOption.IsChecked = false;
                    SanityOption.IsChecked = false;
                }
                else if (radioButton == NHostOption)
                {
                    AppwriteOption.IsChecked = false;
                    SupabaseOption.IsChecked = false;
                    ContentfulOption.IsChecked = false;
                    Back4AppOption.IsChecked = false;
                    MySQLOption.IsChecked = false;
                    StrapiOption.IsChecked = false;
                    SanityOption.IsChecked = false;
                }
                else if (radioButton == ContentfulOption)
                {
                    AppwriteOption.IsChecked = false;
                    SupabaseOption.IsChecked = false;
                    NHostOption.IsChecked = false;
                    Back4AppOption.IsChecked = false;
                    MySQLOption.IsChecked = false;
                    StrapiOption.IsChecked = false;
                    SanityOption.IsChecked = false;
                }
                else if (radioButton == Back4AppOption)
                {
                    AppwriteOption.IsChecked = false;
                    SupabaseOption.IsChecked = false;
                    NHostOption.IsChecked = false;
                    ContentfulOption.IsChecked = false;
                    MySQLOption.IsChecked = false;
                    StrapiOption.IsChecked = false;
                    SanityOption.IsChecked = false;
                }
                else if (radioButton == MySQLOption)
                {
                    AppwriteOption.IsChecked = false;
                    SupabaseOption.IsChecked = false;
                    NHostOption.IsChecked = false;
                    ContentfulOption.IsChecked = false;
                    Back4AppOption.IsChecked = false;
                    StrapiOption.IsChecked = false;
                    SanityOption.IsChecked = false;
                }
                else if (radioButton == StrapiOption)
                {
                    AppwriteOption.IsChecked = false;
                    SupabaseOption.IsChecked = false;
                    NHostOption.IsChecked = false;
                    ContentfulOption.IsChecked = false;
                    Back4AppOption.IsChecked = false;
                    MySQLOption.IsChecked = false;
                    SanityOption.IsChecked = false;
                }
                else if (radioButton == SanityOption)
                {
                    AppwriteOption.IsChecked = false;
                    SupabaseOption.IsChecked = false;
                    NHostOption.IsChecked = false;
                    ContentfulOption.IsChecked = false;
                    Back4AppOption.IsChecked = false;
                    MySQLOption.IsChecked = false;
                    StrapiOption.IsChecked = false;
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
                else if (radioButton == StrapiOption)
                    selectedService = BackendServiceType.Strapi;
                else if (radioButton == SanityOption)
                    selectedService = BackendServiceType.Sanity;

                // 特別處理 Supabase：強制更新為正確的值
                if (selectedService == BackendServiceType.Supabase)
                {
                    System.Diagnostics.Debug.WriteLine("選擇 Supabase，強制更新欄位值");
                    ApiUrlTextBox.Text = AppSettings.Defaults.Supabase.ApiUrl;
                    ProjectIdTextBox.Text = AppSettings.Defaults.Supabase.ProjectId;
                    ApiKeyPasswordBox.Password = AppSettings.Defaults.Supabase.ApiKey;
                    DatabaseIdTextBox.Text = "";
                    BucketIdTextBox.Text = "";
                }

                // 即時保存後端服務選擇
                try
                {
                    var settings = AppSettings.Instance;
                    settings.BackendService = selectedService;
                    
                    // 如果是 Supabase，也更新相關設定
                    if (selectedService == BackendServiceType.Supabase)
                    {
                        settings.ApiUrl = AppSettings.Defaults.Supabase.ApiUrl;
                        settings.ProjectId = AppSettings.Defaults.Supabase.ProjectId;
                        settings.ApiKey = AppSettings.Defaults.Supabase.ApiKey;
                        settings.DatabaseId = "";
                        settings.BucketId = "";
                    }
                    
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
                    FoodCollectionIdLabel.Visibility = System.Windows.Visibility.Visible;
                    FoodCollectionIdTextBox.Visibility = System.Windows.Visibility.Visible;
                    SubscriptionCollectionIdLabel.Visibility = System.Windows.Visibility.Visible;
                    SubscriptionCollectionIdTextBox.Visibility = System.Windows.Visibility.Visible;
                    // 顯示 API Key 欄位
                    ApiKeyLabel.Visibility = System.Windows.Visibility.Visible;
                    ApiKeyPasswordBox.Visibility = System.Windows.Visibility.Visible;
                    break;
                case BackendServiceType.Contentful:
                    ApiUrlLabel.Text = "API URL:";
                    ProjectIdLabel.Text = "Space ID:";
                    // 隱藏 Appwrite 專用欄位
                    DatabaseIdLabel.Visibility = System.Windows.Visibility.Collapsed;
                    DatabaseIdTextBox.Visibility = System.Windows.Visibility.Collapsed;
                    BucketIdLabel.Visibility = System.Windows.Visibility.Collapsed;
                    BucketIdTextBox.Visibility = System.Windows.Visibility.Collapsed;
                    FoodCollectionIdLabel.Visibility = System.Windows.Visibility.Collapsed;
                    FoodCollectionIdTextBox.Visibility = System.Windows.Visibility.Collapsed;
                    SubscriptionCollectionIdLabel.Visibility = System.Windows.Visibility.Collapsed;
                    SubscriptionCollectionIdTextBox.Visibility = System.Windows.Visibility.Collapsed;
                    // 顯示 API Key 欄位
                    ApiKeyLabel.Visibility = System.Windows.Visibility.Visible;
                    ApiKeyPasswordBox.Visibility = System.Windows.Visibility.Visible;
                    break;
                case BackendServiceType.Back4App:
                    ApiUrlLabel.Text = "API URL:";
                    ProjectIdLabel.Text = "App ID:";
                    // 隱藏 Appwrite 專用欄位
                    DatabaseIdLabel.Visibility = System.Windows.Visibility.Collapsed;
                    DatabaseIdTextBox.Visibility = System.Windows.Visibility.Collapsed;
                    BucketIdLabel.Visibility = System.Windows.Visibility.Collapsed;
                    BucketIdTextBox.Visibility = System.Windows.Visibility.Collapsed;
                    FoodCollectionIdLabel.Visibility = System.Windows.Visibility.Collapsed;
                    FoodCollectionIdTextBox.Visibility = System.Windows.Visibility.Collapsed;
                    SubscriptionCollectionIdLabel.Visibility = System.Windows.Visibility.Collapsed;
                    SubscriptionCollectionIdTextBox.Visibility = System.Windows.Visibility.Collapsed;
                    // 顯示 API Key 欄位
                    ApiKeyLabel.Visibility = System.Windows.Visibility.Visible;
                    ApiKeyPasswordBox.Visibility = System.Windows.Visibility.Visible;
                    break;
                case BackendServiceType.MySQL:
                    ApiUrlLabel.Text = "API URL:";
                    ProjectIdLabel.Text = "Database:";
                    // 隱藏 Appwrite 專用欄位
                    DatabaseIdLabel.Visibility = System.Windows.Visibility.Collapsed;
                    DatabaseIdTextBox.Visibility = System.Windows.Visibility.Collapsed;
                    BucketIdLabel.Visibility = System.Windows.Visibility.Collapsed;
                    BucketIdTextBox.Visibility = System.Windows.Visibility.Collapsed;
                    FoodCollectionIdLabel.Visibility = System.Windows.Visibility.Collapsed;
                    FoodCollectionIdTextBox.Visibility = System.Windows.Visibility.Collapsed;
                    SubscriptionCollectionIdLabel.Visibility = System.Windows.Visibility.Collapsed;
                    SubscriptionCollectionIdTextBox.Visibility = System.Windows.Visibility.Collapsed;
                    // 顯示 API Key 欄位
                    ApiKeyLabel.Visibility = System.Windows.Visibility.Visible;
                    ApiKeyPasswordBox.Visibility = System.Windows.Visibility.Visible;
                    break;
                case BackendServiceType.Supabase:
                    ApiUrlLabel.Text = "API URL:";
                    ProjectIdLabel.Text = "Project ID:";
                    // 隱藏 Appwrite 專用欄位
                    DatabaseIdLabel.Visibility = System.Windows.Visibility.Collapsed;
                    DatabaseIdTextBox.Visibility = System.Windows.Visibility.Collapsed;
                    BucketIdLabel.Visibility = System.Windows.Visibility.Collapsed;
                    BucketIdTextBox.Visibility = System.Windows.Visibility.Collapsed;
                    FoodCollectionIdLabel.Visibility = System.Windows.Visibility.Collapsed;
                    FoodCollectionIdTextBox.Visibility = System.Windows.Visibility.Collapsed;
                    SubscriptionCollectionIdLabel.Visibility = System.Windows.Visibility.Collapsed;
                    SubscriptionCollectionIdTextBox.Visibility = System.Windows.Visibility.Collapsed;
                    // 顯示 API Key 欄位
                    ApiKeyLabel.Visibility = System.Windows.Visibility.Visible;
                    ApiKeyPasswordBox.Visibility = System.Windows.Visibility.Visible;
                    break;
                case BackendServiceType.NHost:
                    ApiUrlLabel.Text = "NHOST_GRAPHQL_URL:";
                    ProjectIdLabel.Text = "NHOST_ADMIN_SECRET:";
                    // 隱藏 Appwrite 專用欄位和 API Key 欄位（NHost 只需要兩個欄位）
                    DatabaseIdLabel.Visibility = System.Windows.Visibility.Collapsed;
                    DatabaseIdTextBox.Visibility = System.Windows.Visibility.Collapsed;
                    BucketIdLabel.Visibility = System.Windows.Visibility.Collapsed;
                    BucketIdTextBox.Visibility = System.Windows.Visibility.Collapsed;
                    FoodCollectionIdLabel.Visibility = System.Windows.Visibility.Collapsed;
                    FoodCollectionIdTextBox.Visibility = System.Windows.Visibility.Collapsed;
                    SubscriptionCollectionIdLabel.Visibility = System.Windows.Visibility.Collapsed;
                    SubscriptionCollectionIdTextBox.Visibility = System.Windows.Visibility.Collapsed;
                    ApiKeyLabel.Visibility = System.Windows.Visibility.Collapsed;
                    ApiKeyPasswordBox.Visibility = System.Windows.Visibility.Collapsed;
                    break;
                case BackendServiceType.Strapi:
                    ApiUrlLabel.Text = "API URL:";
                    ProjectIdLabel.Text = "Project ID:";
                    // 隱藏 Appwrite 專用欄位
                    DatabaseIdLabel.Visibility = System.Windows.Visibility.Collapsed;
                    DatabaseIdTextBox.Visibility = System.Windows.Visibility.Collapsed;
                    BucketIdLabel.Visibility = System.Windows.Visibility.Collapsed;
                    BucketIdTextBox.Visibility = System.Windows.Visibility.Collapsed;
                    FoodCollectionIdLabel.Visibility = System.Windows.Visibility.Collapsed;
                    FoodCollectionIdTextBox.Visibility = System.Windows.Visibility.Collapsed;
                    SubscriptionCollectionIdLabel.Visibility = System.Windows.Visibility.Collapsed;
                    SubscriptionCollectionIdTextBox.Visibility = System.Windows.Visibility.Collapsed;
                    // 顯示 API Key 欄位
                    ApiKeyLabel.Visibility = System.Windows.Visibility.Visible;
                    ApiKeyPasswordBox.Visibility = System.Windows.Visibility.Visible;
                    break;
                case BackendServiceType.Sanity:
                    ApiUrlLabel.Text = "API URL:";
                    ProjectIdLabel.Text = "Project ID:";
                    // 隱藏 Appwrite 專用欄位
                    DatabaseIdLabel.Visibility = System.Windows.Visibility.Collapsed;
                    DatabaseIdTextBox.Visibility = System.Windows.Visibility.Collapsed;
                    BucketIdLabel.Visibility = System.Windows.Visibility.Collapsed;
                    BucketIdTextBox.Visibility = System.Windows.Visibility.Collapsed;
                    FoodCollectionIdLabel.Visibility = System.Windows.Visibility.Collapsed;
                    FoodCollectionIdTextBox.Visibility = System.Windows.Visibility.Collapsed;
                    SubscriptionCollectionIdLabel.Visibility = System.Windows.Visibility.Collapsed;
                    SubscriptionCollectionIdTextBox.Visibility = System.Windows.Visibility.Collapsed;
                    // 顯示 API Key 欄位
                    ApiKeyLabel.Visibility = System.Windows.Visibility.Visible;
                    ApiKeyPasswordBox.Visibility = System.Windows.Visibility.Visible;
                    break;
                default:
                    ApiUrlLabel.Text = "API URL:";
                    ProjectIdLabel.Text = "Project ID:";
                    // 隱藏 Appwrite 專用欄位
                    DatabaseIdLabel.Visibility = System.Windows.Visibility.Collapsed;
                    DatabaseIdTextBox.Visibility = System.Windows.Visibility.Collapsed;
                    BucketIdLabel.Visibility = System.Windows.Visibility.Collapsed;
                    BucketIdTextBox.Visibility = System.Windows.Visibility.Collapsed;
                    FoodCollectionIdLabel.Visibility = System.Windows.Visibility.Collapsed;
                    FoodCollectionIdTextBox.Visibility = System.Windows.Visibility.Collapsed;
                    SubscriptionCollectionIdLabel.Visibility = System.Windows.Visibility.Collapsed;
                    SubscriptionCollectionIdTextBox.Visibility = System.Windows.Visibility.Collapsed;
                    // 顯示 API Key 欄位
                    ApiKeyLabel.Visibility = System.Windows.Visibility.Visible;
                    ApiKeyPasswordBox.Visibility = System.Windows.Visibility.Visible;
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
                        IsDefaultUrl(ApiUrlTextBox.Text) ||
                        ApiUrlTextBox.Text.Contains("lobezwpworbfktlkxuyoKiro") ||
                        !ApiUrlTextBox.Text.Contains("lobezwpworbfktlkxuyo.supabase.co"))
                    {
                        ApiUrlTextBox.Text = AppSettings.Defaults.Supabase.ApiUrl;
                    }
                    if (string.IsNullOrWhiteSpace(ProjectIdTextBox.Text) ||
                        IsDefaultProjectId(ProjectIdTextBox.Text) ||
                        ProjectIdTextBox.Text == "lobezwpworbfktlkxuyoKiro" ||
                        ProjectIdTextBox.Text != "lobezwpworbfktlkxuyo")
                    {
                        ProjectIdTextBox.Text = AppSettings.Defaults.Supabase.ProjectId;
                    }
                    if (string.IsNullOrWhiteSpace(ApiKeyPasswordBox.Password) ||
                        !ApiKeyPasswordBox.Password.StartsWith("eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9"))
                    {
                        ApiKeyPasswordBox.Password = AppSettings.Defaults.Supabase.ApiKey;
                    }
                    break;

                case BackendServiceType.NHost:
                    // NHost 強制使用正確的預設值
                    if (string.IsNullOrWhiteSpace(ApiUrlTextBox.Text) || 
                        IsDefaultUrl(ApiUrlTextBox.Text) ||
                        ApiUrlTextBox.Text.Contains("your-project") ||
                        !ApiUrlTextBox.Text.Contains("uxgwdiuehabbzenwtcqo"))
                    {
                        ApiUrlTextBox.Text = AppSettings.Defaults.NHost.ApiUrl;
                    }
                    if (string.IsNullOrWhiteSpace(ProjectIdTextBox.Text) ||
                        IsDefaultProjectId(ProjectIdTextBox.Text) ||
                        ProjectIdTextBox.Text.Contains("your-project") ||
                        ProjectIdTextBox.Text != "cu#34&yjF3Cr%fgxB#WA,4r4^c=Igcwr")
                    {
                        ProjectIdTextBox.Text = AppSettings.Defaults.NHost.ProjectId;
                    }
                    // NHost 只需要兩個欄位，不需要 API Key
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

                case BackendServiceType.Strapi:
                    if (string.IsNullOrWhiteSpace(ApiUrlTextBox.Text) || 
                        IsDefaultUrl(ApiUrlTextBox.Text))
                    {
                        ApiUrlTextBox.Text = AppSettings.Defaults.Strapi.ApiUrl;
                    }
                    if (string.IsNullOrWhiteSpace(ProjectIdTextBox.Text) ||
                        IsDefaultProjectId(ProjectIdTextBox.Text))
                    {
                        ProjectIdTextBox.Text = AppSettings.Defaults.Strapi.ProjectId;
                    }
                    if (string.IsNullOrWhiteSpace(ApiKeyPasswordBox.Password))
                    {
                        ApiKeyPasswordBox.Password = AppSettings.Defaults.Strapi.ApiKey;
                    }
                    break;

                case BackendServiceType.Sanity:
                    if (string.IsNullOrWhiteSpace(ApiUrlTextBox.Text) || 
                        IsDefaultUrl(ApiUrlTextBox.Text))
                    {
                        ApiUrlTextBox.Text = AppSettings.Defaults.Sanity.ApiUrl;
                    }
                    if (string.IsNullOrWhiteSpace(ProjectIdTextBox.Text) ||
                        IsDefaultProjectId(ProjectIdTextBox.Text))
                    {
                        ProjectIdTextBox.Text = AppSettings.Defaults.Sanity.ProjectId;
                    }
                    if (string.IsNullOrWhiteSpace(ApiKeyPasswordBox.Password))
                    {
                        ApiKeyPasswordBox.Password = AppSettings.Defaults.Sanity.ApiKey;
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
                   url == AppSettings.Defaults.MySQL.ApiUrl ||
                   url == AppSettings.Defaults.Strapi.ApiUrl ||
                   url == AppSettings.Defaults.Sanity.ApiUrl;
        }

        private bool IsDefaultProjectId(string projectId)
        {
            return projectId == AppSettings.Defaults.Appwrite.ProjectId ||
                   projectId == AppSettings.Defaults.Supabase.ProjectId ||
                   projectId == AppSettings.Defaults.NHost.ProjectId ||
                   projectId == AppSettings.Defaults.Contentful.ProjectId ||
                   projectId == AppSettings.Defaults.Back4App.ProjectId ||
                   projectId == AppSettings.Defaults.MySQL.ProjectId ||
                   projectId == AppSettings.Defaults.Strapi.ProjectId ||
                   projectId == AppSettings.Defaults.Sanity.ProjectId;
        }

        private async void DownloadFoodCsv_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DownloadFoodCsvButton.IsEnabled = false;
                DownloadFoodCsvButton.Content = "下載中...";

                var service = BackendServiceFactory.CreateCurrentService();
                var result = await service.GetFoodsAsync();

                if (result.Success && result.Data != null)
                {
                    var serviceName = service.ServiceName.ToLower();
                    var fileName = $"{serviceName}food.csv";
                    
                    var csvContent = GenerateFoodCsv(result.Data);
                    await SaveCsvFile(csvContent, fileName);
                    
                    ShowStatusMessage($"成功下載 {fileName}！", Brushes.Green);
                }
                else
                {
                    ShowStatusMessage($"下載失敗：{result.ErrorMessage}", Brushes.Red);
                }
            }
            catch (Exception ex)
            {
                ShowStatusMessage($"下載食品資料時發生錯誤：{ex.Message}", Brushes.Red);
            }
            finally
            {
                DownloadFoodCsvButton.IsEnabled = true;
                DownloadFoodCsvButton.Content = "📥 下載 food.csv";
            }
        }

        private async void DownloadSubscriptionCsv_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DownloadSubscriptionCsvButton.IsEnabled = false;
                DownloadSubscriptionCsvButton.Content = "下載中...";

                var service = BackendServiceFactory.CreateCurrentService();
                var result = await service.GetSubscriptionsAsync();

                if (result.Success && result.Data != null)
                {
                    var serviceName = service.ServiceName.ToLower();
                    var fileName = $"{serviceName}subscription.csv";
                    
                    var csvContent = GenerateSubscriptionCsv(result.Data);
                    await SaveCsvFile(csvContent, fileName);
                    
                    ShowStatusMessage($"成功下載 {fileName}！", Brushes.Green);
                }
                else
                {
                    ShowStatusMessage($"下載失敗：{result.ErrorMessage}", Brushes.Red);
                }
            }
            catch (Exception ex)
            {
                ShowStatusMessage($"下載訂閱資料時發生錯誤：{ex.Message}", Brushes.Red);
            }
            finally
            {
                DownloadSubscriptionCsvButton.IsEnabled = true;
                DownloadSubscriptionCsvButton.Content = "📥 下載 subscription.csv";
            }
        }

        private string GenerateFoodCsv(object[] foods)
        {
            var csv = new System.Text.StringBuilder();
            var settings = AppSettings.Instance;
            
            // 根據當前後端服務生成正確的 CSV 標題行
            if (settings.BackendService == BackendServiceType.Supabase)
            {
                // Supabase 實際表結構
                csv.AppendLine("id,created_at,name,todate,amount,photo,price,shop,photohash");
            }
            else
            {
                // Appwrite 和其他服務的表結構
                csv.AppendLine("$id,name,price,photo,shop,todate,photohash,$createdAt,$updatedAt");
            }

            foreach (var item in foods)
            {
                try
                {
                    // 根據 Appwrite 的實際欄位名稱獲取資料
                    var id = GetPropertyValue(item, "$id", "id", "Id") ?? "";
                    var name = GetPropertyValue(item, "name", "foodName", "FoodName") ?? "";
                    var price = GetPropertyValue(item, "price", "Price") ?? "0";
                    var photo = GetPropertyValue(item, "photo", "Photo") ?? "";
                    var shop = GetPropertyValue(item, "shop", "Shop") ?? "";
                    var todateRaw = GetPropertyValue(item, "todate", "toDate", "ToDate") ?? "";
                    var photohash = GetPropertyValue(item, "photohash", "photoHash", "PhotoHash") ?? "";
                    var account = GetPropertyValue(item, "account", "Account") ?? "";
                    var createdAt = GetPropertyValue(item, "$createdAt", "createdAt", "CreatedAt", "created_at") ?? "";
                    var updatedAt = GetPropertyValue(item, "$updatedAt", "updatedAt", "UpdatedAt", "updated_at") ?? "";

                    // 處理日期格式 - 確保使用英文格式
                    var todate = FormatDateForCsv(todateRaw);
                    var createdAtFormatted = FormatDateForCsv(createdAt);
                    var updatedAtFormatted = FormatDateForCsv(updatedAt);

                    // 根據後端服務生成不同的 CSV 行
                    if (settings.BackendService == BackendServiceType.Supabase)
                    {
                        // Supabase 格式：id,created_at,name,todate,amount,photo,price,shop,photohash
                        var amount = GetPropertyValue(item, "amount", "quantity", "Quantity") ?? "1"; // 預設數量為1
                        var supabaseId = ConvertToUuid(id); // 轉換為 UUID 格式
                        
                        csv.AppendLine($"{EscapeCsvField(supabaseId)},{createdAtFormatted},{EscapeCsvField(name)},{todate},{amount},{EscapeCsvField(photo)},{price},{EscapeCsvField(shop)},{EscapeCsvField(photohash)}");
                    }
                    else
                    {
                        // Appwrite 格式：$id,name,price,photo,shop,todate,photohash,$createdAt,$updatedAt
                        csv.AppendLine($"\"{EscapeCsvField(id)}\",\"{EscapeCsvField(name)}\",{price},\"{EscapeCsvField(photo)}\",\"{EscapeCsvField(shop)}\",\"{todate}\",\"{EscapeCsvField(photohash)}\",\"{createdAtFormatted}\",\"{updatedAtFormatted}\"");
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"處理食品項目時發生錯誤：{ex.Message}");
                }
            }

            return csv.ToString();
        }

        private string GenerateSubscriptionCsv(object[] subscriptions)
        {
            var csv = new System.Text.StringBuilder();
            var settings = AppSettings.Instance;
            
            // 根據當前後端服務生成正確的 CSV 標題行
            if (settings.BackendService == BackendServiceType.Supabase)
            {
                // Supabase 實際表結構
                csv.AppendLine("id,created_at,name,nextdate,price,site,note,account");
            }
            else
            {
                // Appwrite 和其他服務的表結構
                csv.AppendLine("$id,name,nextdate,price,site,note,account,$createdAt,$updatedAt");
            }

            foreach (var item in subscriptions)
            {
                try
                {
                    // 根據 Appwrite 的實際欄位名稱獲取資料
                    var id = GetPropertyValue(item, "$id", "id", "Id") ?? "";
                    var name = GetPropertyValue(item, "name", "subscriptionName", "SubscriptionName") ?? "";
                    var nextdateRaw = GetPropertyValue(item, "nextdate", "nextDate", "NextDate") ?? "";
                    var price = GetPropertyValue(item, "price", "Price") ?? "0";
                    var site = GetPropertyValue(item, "site", "Site") ?? "";
                    var note = GetPropertyValue(item, "note", "Note") ?? "";
                    var account = GetPropertyValue(item, "account", "Account") ?? "";
                    var createdAt = GetPropertyValue(item, "$createdAt", "createdAt", "CreatedAt", "created_at") ?? "";
                    var updatedAt = GetPropertyValue(item, "$updatedAt", "updatedAt", "UpdatedAt", "updated_at") ?? "";

                    // 處理日期格式 - 確保使用英文格式
                    var nextdate = FormatDateForCsv(nextdateRaw);
                    var createdAtFormatted = FormatDateForCsv(createdAt);
                    var updatedAtFormatted = FormatDateForCsv(updatedAt);

                    // 根據後端服務生成不同的 CSV 行
                    if (settings.BackendService == BackendServiceType.Supabase)
                    {
                        // Supabase 格式：id,created_at,name,nextdate,price,site,note,account
                        var supabaseId = ConvertToUuid(id); // 轉換為 UUID 格式
                        csv.AppendLine($"{EscapeCsvField(supabaseId)},{createdAtFormatted},{EscapeCsvField(name)},{nextdate},{price},{EscapeCsvField(site)},{EscapeCsvField(note)},{EscapeCsvField(account)}");
                    }
                    else
                    {
                        // Appwrite 格式：$id,name,nextdate,price,site,note,account,$createdAt,$updatedAt
                        csv.AppendLine($"\"{EscapeCsvField(id)}\",\"{EscapeCsvField(name)}\",\"{nextdate}\",{price},\"{EscapeCsvField(site)}\",\"{EscapeCsvField(note)}\",\"{EscapeCsvField(account)}\",\"{createdAtFormatted}\",\"{updatedAtFormatted}\"");
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"處理訂閱項目時發生錯誤：{ex.Message}");
                }
            }

            return csv.ToString();
        }

        private string GetPropertyValue(object obj, params string[] propertyNames)
        {
            foreach (var propertyName in propertyNames)
            {
                var property = obj.GetType().GetProperty(propertyName);
                if (property != null)
                {
                    var value = property.GetValue(obj);
                    return value?.ToString() ?? "";
                }
            }
            return "";
        }

        private string FormatDateForCsv(string dateValue)
        {
            if (string.IsNullOrEmpty(dateValue))
                return "";

            try
            {
                var settings = AppSettings.Instance;
                
                // 嘗試解析日期時間
                if (DateTime.TryParse(dateValue, out DateTime parsedDate))
                {
                    if (settings.BackendService == BackendServiceType.Supabase)
                    {
                        // Supabase 格式：2026-01-02 17:09:09.823688+00
                        return parsedDate.ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss.ffffff+00", System.Globalization.CultureInfo.InvariantCulture);
                    }
                    else
                    {
                        // Appwrite 和其他服務格式：ISO 8601
                        return parsedDate.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffZ", System.Globalization.CultureInfo.InvariantCulture);
                    }
                }
                
                // 如果無法解析，返回原始值
                return dateValue;
            }
            catch
            {
                // 如果發生錯誤，返回原始值
                return dateValue;
            }
        }

        private string EscapeCsvField(string field)
        {
            if (string.IsNullOrEmpty(field))
                return "";
            
            // 將雙引號轉換為兩個雙引號（CSV 標準）
            return field.Replace("\"", "\"\"");
        }

        private async Task SaveCsvFile(string csvContent, string fileName)
        {
            try
            {
                // 使用 SaveFileDialog 讓用戶選擇保存位置
                var saveFileDialog = new Microsoft.Win32.SaveFileDialog
                {
                    FileName = fileName,
                    DefaultExt = ".csv",
                    Filter = "CSV 文件 (*.csv)|*.csv|所有文件 (*.*)|*.*",
                    Title = "保存 CSV 文件"
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    // 使用 UTF-8 編碼保存，包含 BOM 以確保 Excel 正確顯示中文
                    var utf8WithBom = new System.Text.UTF8Encoding(true);
                    await File.WriteAllTextAsync(saveFileDialog.FileName, csvContent, utf8WithBom);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"保存文件時發生錯誤：{ex.Message}");
            }
        }

        // 設定檔匯出功能
        private async void QuickExportProfiles_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                QuickExportProfilesButton.IsEnabled = false;
                QuickExportProfilesButton.Content = "匯出中...";

                var profileService = SettingsProfileService.Instance;
                var profileCount = profileService.GetProfileCount();

                if (profileCount == 0)
                {
                    ShowStatusMessage("沒有設定檔可以匯出", Brushes.Orange);
                    return;
                }

                // 讓用戶選擇保存位置
                var saveFileDialog = new Microsoft.Win32.SaveFileDialog
                {
                    Title = "選擇設定檔匯出位置",
                    Filter = "JSON 檔案 (*.json)|*.json|所有檔案 (*.*)|*.*",
                    DefaultExt = "json",
                    FileName = $"設定檔備份_{DateTime.Now:yyyyMMdd_HHmmss}.json",
                    InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    var result = await profileService.ExportProfilesAsync();
                    if (result.Success)
                    {
                        await File.WriteAllTextAsync(saveFileDialog.FileName, result.Data);
                        var fileInfo = new FileInfo(saveFileDialog.FileName);
                        ShowStatusMessage($"成功匯出 {profileCount} 筆設定檔", Brushes.Green);
                        
                        var result2 = MessageBox.Show(
                            $"成功匯出 {profileCount} 筆設定檔到：\n{fileInfo.DirectoryName}\n檔案：{fileInfo.Name}\n\n是否要開啟檔案位置？",
                            "匯出成功",
                            MessageBoxButton.YesNo,
                            MessageBoxImage.Information);

                        if (result2 == MessageBoxResult.Yes)
                        {
                            System.Diagnostics.Process.Start("explorer.exe", $"/select,\"{saveFileDialog.FileName}\"");
                        }
                    }
                    else
                    {
                        ShowStatusMessage($"匯出失敗：{result.ErrorMessage}", Brushes.Red);
                    }
                }
                else
                {
                    ShowStatusMessage("已取消匯出", Brushes.Gray);
                }
            }
            catch (Exception ex)
            {
                ShowStatusMessage($"匯出時發生錯誤：{ex.Message}", Brushes.Red);
            }
            finally
            {
                QuickExportProfilesButton.IsEnabled = true;
                QuickExportProfilesButton.Content = "📤 快速匯出";
            }
        }

        private async void ExportToFolder_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ExportToFolderButton.IsEnabled = false;
                ExportToFolderButton.Content = "選擇中...";

                var profileService = SettingsProfileService.Instance;
                var profileCount = profileService.GetProfileCount();

                if (profileCount == 0)
                {
                    ShowStatusMessage("沒有設定檔可以匯出", Brushes.Orange);
                    return;
                }

                var selectedFolder = FolderSelectDialog.SelectFolderWithMessage("選擇設定檔匯出資料夾");

                if (!string.IsNullOrEmpty(selectedFolder))
                {
                    ExportToFolderButton.Content = "匯出中...";
                    ShowStatusMessage("正在匯出設定檔...", Brushes.Blue);
                    
                    var result = await profileService.ExportProfilesAsync();
                    if (result.Success)
                    {
                        var fileName = $"設定檔備份_{DateTime.Now:yyyyMMdd_HHmmss}.json";
                        var filePath = Path.Combine(selectedFolder, fileName);
                        
                        await File.WriteAllTextAsync(filePath, result.Data);
                        ShowStatusMessage($"成功匯出 {profileCount} 筆設定檔到指定資料夾", Brushes.Green);
                        
                        var result2 = MessageBox.Show(
                            $"成功匯出 {profileCount} 筆設定檔到：\n{selectedFolder}\n檔案：{fileName}\n\n是否要開啟檔案位置？",
                            "匯出成功",
                            MessageBoxButton.YesNo,
                            MessageBoxImage.Information);

                        if (result2 == MessageBoxResult.Yes)
                        {
                            System.Diagnostics.Process.Start("explorer.exe", $"/select,\"{filePath}\"");
                        }
                    }
                    else
                    {
                        ShowStatusMessage($"匯出失敗：{result.ErrorMessage}", Brushes.Red);
                    }
                }
                else
                {
                    ShowStatusMessage("已取消匯出", Brushes.Gray);
                }
            }
            catch (Exception ex)
            {
                ShowStatusMessage($"匯出到資料夾時發生錯誤：{ex.Message}", Brushes.Red);
            }
            finally
            {
                ExportToFolderButton.IsEnabled = true;
                ExportToFolderButton.Content = "📁 選擇資料夾";
            }
        }

        private async void TestExport_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                TestExportButton.IsEnabled = false;
                TestExportButton.Content = "測試中...";
                ShowStatusMessage("正在執行匯出功能測試...", Brushes.Blue);

                await TestProfileExport.TestExportFunctionality();
                
                ShowStatusMessage("匯出功能測試完成，請查看調試輸出", Brushes.Green);
                TestProfileExport.ShowExportGuide();
            }
            catch (Exception ex)
            {
                ShowStatusMessage($"測試時發生錯誤：{ex.Message}", Brushes.Red);
            }
            finally
            {
                TestExportButton.IsEnabled = true;
                TestExportButton.Content = "🧪 測試匯出";
            }
        }

        private async void TestHotReload_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                TestHotReloadButton.IsEnabled = false;
                TestHotReloadButton.Content = "測試中...";
                ShowStatusMessage("正在測試設定檔熱重載功能...", Brushes.Blue);

                await TestHotReloadSettings.TestHotReloadFunctionality();
                
                ShowStatusMessage("熱重載功能測試完成，請查看調試輸出", Brushes.Green);
                TestHotReloadSettings.ShowHotReloadGuide();
            }
            catch (Exception ex)
            {
                ShowStatusMessage($"測試熱重載功能時發生錯誤：{ex.Message}", Brushes.Red);
            }
            finally
            {
                TestHotReloadButton.IsEnabled = true;
                TestHotReloadButton.Content = "🔥 測試熱重載";
            }
        }

        private async void QuickTest_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                QuickTestButton.IsEnabled = false;
                QuickTestButton.Content = "測試中...";
                ShowStatusMessage("正在執行快速測試...", Brushes.Blue);

                // 檢查當前後端服務
                var settings = AppSettings.Instance;
                Console.WriteLine($"當前後端服務: {settings.BackendService}");
                
                if (settings.BackendService == BackendServiceType.Supabase)
                {
                    // 如果是 Supabase，執行修正後的測試
                    Console.WriteLine("執行修正後的 Supabase 測試...");
                    await TestSupabaseFixed.RunFixedTest();
                    TestSupabaseFixed.ShowFixedIssues();
                }
                else
                {
                    // 執行一般測試
                    Console.WriteLine("執行一般功能測試...");
                    
                    // 測試摺疊功能
                    await TestCollapsibleSettings.TestCollapsibleFunctionality();
                    
                    // 測試 Appwrite Table ID 設定
                    await TestCollapsibleSettings.TestAppwriteTableIdConfiguration();
                    
                    // 顯示使用指南
                    TestCollapsibleSettings.ShowCollapsibleGuide();
                }
                
                ShowStatusMessage("快速測試完成，請查看控制台輸出", Brushes.Green);
            }
            catch (Exception ex)
            {
                ShowStatusMessage($"快速測試時發生錯誤：{ex.Message}", Brushes.Red);
            }
            finally
            {
                QuickTestButton.IsEnabled = true;
                QuickTestButton.Content = "⚡ 快速測試";
            }
        }

        // 缺少的事件處理方法
        private void BackendServiceHeader_Click(object sender, RoutedEventArgs e)
        {
            // 切換後端服務設定的顯示/隱藏
            try
            {
                if (BackendServiceContent != null && BackendExpandIcon != null)
                {
                    if (BackendServiceContent.Visibility == Visibility.Visible)
                    {
                        BackendServiceContent.Visibility = Visibility.Collapsed;
                        BackendExpandIcon.Text = "▶";
                    }
                    else
                    {
                        BackendServiceContent.Visibility = Visibility.Visible;
                        BackendExpandIcon.Text = "▼";
                    }
                }
            }
            catch (Exception ex)
            {
                ShowStatusMessage($"切換後端服務設定顯示狀態時發生錯誤：{ex.Message}", Brushes.Red);
            }
        }

        private void ConnectionSettingsHeader_Click(object sender, RoutedEventArgs e)
        {
            // 切換連線設定的顯示/隱藏
            try
            {
                if (ConnectionSettingsContent != null && ConnectionExpandIcon != null)
                {
                    if (ConnectionSettingsContent.Visibility == Visibility.Visible)
                    {
                        ConnectionSettingsContent.Visibility = Visibility.Collapsed;
                        ConnectionExpandIcon.Text = "▶";
                    }
                    else
                    {
                        ConnectionSettingsContent.Visibility = Visibility.Visible;
                        ConnectionExpandIcon.Text = "▼";
                    }
                }
            }
            catch (Exception ex)
            {
                ShowStatusMessage($"切換連線設定顯示狀態時發生錯誤：{ex.Message}", Brushes.Red);
            }
        }

        private void ManageProfiles_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var profileWindow = new SettingsProfileWindow
                {
                    Owner = Window.GetWindow(this)
                };
                
                profileWindow.ShowDialog();
            }
            catch (Exception ex)
            {
                ShowStatusMessage($"開啟設定檔管理視窗時發生錯誤：{ex.Message}", Brushes.Red);
            }
        }

        private void ShowSettingsFile_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var settingsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "wpfkiro20260101", "settings.json");
                if (File.Exists(settingsPath))
                {
                    System.Diagnostics.Process.Start("explorer.exe", $"/select,\"{settingsPath}\"");
                }
                else
                {
                    ShowStatusMessage("設定檔案不存在", Brushes.Orange);
                }
            }
            catch (Exception ex)
            {
                ShowStatusMessage($"開啟設定檔案位置時發生錯誤：{ex.Message}", Brushes.Red);
            }
        }

        private void RefreshSettings_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // 強制重新載入設定
                AppSettings.ReloadSettings();
                
                // 重新載入界面
                LoadSettings();
                
                ShowStatusMessage("設定已刷新！", Brushes.Green);
            }
            catch (Exception ex)
            {
                ShowStatusMessage($"刷新設定時發生錯誤：{ex.Message}", Brushes.Red);
            }
        }

        // 資料轉換功能
        private async void ConvertFoodCsv_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ConvertFoodCsvButton.IsEnabled = false;
                ConvertFoodCsvButton.Content = "轉換中...";
                ShowStatusMessage("正在轉換 Food CSV...", Brushes.Blue);

                var openFileDialog = new Microsoft.Win32.OpenFileDialog
                {
                    Title = "選擇 Appwrite Food CSV 檔案",
                    Filter = "CSV 文件 (*.csv)|*.csv|所有文件 (*.*)|*.*",
                    DefaultExt = ".csv"
                };

                if (openFileDialog.ShowDialog() == true)
                {
                    var inputFile = openFileDialog.FileName;
                    var outputFile = Path.Combine(Path.GetDirectoryName(inputFile), 
                        $"Supabase_{Path.GetFileNameWithoutExtension(inputFile)}.csv");

                    await ConvertAppwriteToSupabaseCsv(inputFile, outputFile, "food");
                    
                    ShowStatusMessage($"Food CSV 轉換完成！輸出檔案：{Path.GetFileName(outputFile)}", Brushes.Green);
                }
                else
                {
                    ShowStatusMessage("未選擇檔案", Brushes.Orange);
                }
            }
            catch (Exception ex)
            {
                ShowStatusMessage($"轉換 Food CSV 時發生錯誤：{ex.Message}", Brushes.Red);
            }
            finally
            {
                ConvertFoodCsvButton.IsEnabled = true;
                ConvertFoodCsvButton.Content = "🔄 轉換 Food CSV";
            }
        }

        private async void ConvertSubscriptionCsv_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ConvertSubscriptionCsvButton.IsEnabled = false;
                ConvertSubscriptionCsvButton.Content = "轉換中...";
                ShowStatusMessage("正在轉換 Subscription CSV...", Brushes.Blue);

                var openFileDialog = new Microsoft.Win32.OpenFileDialog
                {
                    Title = "選擇 Appwrite Subscription CSV 檔案",
                    Filter = "CSV 文件 (*.csv)|*.csv|所有文件 (*.*)|*.*",
                    DefaultExt = ".csv"
                };

                if (openFileDialog.ShowDialog() == true)
                {
                    var inputFile = openFileDialog.FileName;
                    var outputFile = Path.Combine(Path.GetDirectoryName(inputFile), 
                        $"Supabase_{Path.GetFileNameWithoutExtension(inputFile)}.csv");

                    await ConvertAppwriteToSupabaseCsv(inputFile, outputFile, "subscription");
                    
                    ShowStatusMessage($"Subscription CSV 轉換完成！輸出檔案：{Path.GetFileName(outputFile)}", Brushes.Green);
                }
                else
                {
                    ShowStatusMessage("未選擇檔案", Brushes.Orange);
                }
            }
            catch (Exception ex)
            {
                ShowStatusMessage($"轉換 Subscription CSV 時發生錯誤：{ex.Message}", Brushes.Red);
            }
            finally
            {
                ConvertSubscriptionCsvButton.IsEnabled = true;
                ConvertSubscriptionCsvButton.Content = "🔄 轉換 Subscription CSV";
            }
        }

        private async void BatchConvert_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                BatchConvertButton.IsEnabled = false;
                BatchConvertButton.Content = "批次轉換中...";
                ShowStatusMessage("正在進行批次轉換...", Brushes.Blue);

                var folderDialog = new System.Windows.Forms.FolderBrowserDialog
                {
                    Description = "選擇包含 Appwrite CSV 檔案的資料夾",
                    ShowNewFolderButton = false
                };

                if (folderDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    var inputFolder = folderDialog.SelectedPath;
                    var csvFiles = Directory.GetFiles(inputFolder, "*.csv");
                    
                    if (csvFiles.Length == 0)
                    {
                        ShowStatusMessage("選擇的資料夾中沒有找到 CSV 檔案", Brushes.Orange);
                        return;
                    }

                    int convertedCount = 0;
                    var outputFolder = Path.Combine(inputFolder, "Supabase_Converted");
                    Directory.CreateDirectory(outputFolder);

                    foreach (var csvFile in csvFiles)
                    {
                        try
                        {
                            var fileName = Path.GetFileNameWithoutExtension(csvFile);
                            var outputFile = Path.Combine(outputFolder, $"Supabase_{fileName}.csv");
                            
                            // 根據檔案名稱判斷類型
                            var tableType = fileName.ToLower().Contains("food") ? "food" : 
                                          fileName.ToLower().Contains("subscription") ? "subscription" : "food";
                            
                            await ConvertAppwriteToSupabaseCsv(csvFile, outputFile, tableType);
                            convertedCount++;
                            
                            ShowStatusMessage($"已轉換 {convertedCount}/{csvFiles.Length} 個檔案...", Brushes.Blue);
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine($"轉換檔案 {csvFile} 時發生錯誤: {ex.Message}");
                        }
                    }
                    
                    ShowStatusMessage($"批次轉換完成！共轉換 {convertedCount} 個檔案，輸出至：{outputFolder}", Brushes.Green);
                }
                else
                {
                    ShowStatusMessage("未選擇資料夾", Brushes.Orange);
                }
            }
            catch (Exception ex)
            {
                ShowStatusMessage($"批次轉換時發生錯誤：{ex.Message}", Brushes.Red);
            }
            finally
            {
                BatchConvertButton.IsEnabled = true;
                BatchConvertButton.Content = "📂 批次轉換資料夾";
            }
        }

        private async Task ConvertAppwriteToSupabaseCsv(string inputFile, string outputFile, string tableType)
        {
            await Task.Run(() =>
            {
                var lines = File.ReadAllLines(inputFile);
                if (lines.Length == 0) return;

                var convertedLines = new List<string>();
                
                // 處理標題行
                var headerLine = lines[0];
                if (tableType == "food")
                {
                    // Appwrite Food: $id,name,price,photo,shop,todate,photohash,$createdAt,$updatedAt
                    // Supabase Food: id,created_at,name,todate,amount,photo,price,shop,photohash
                    convertedLines.Add("id,created_at,name,todate,amount,photo,price,shop,photohash");
                }
                else if (tableType == "subscription")
                {
                    // Appwrite Subscription: $id,name,nextdate,price,site,note,account,$createdAt,$updatedAt
                    // Supabase Subscription: id,created_at,name,nextdate,price,site,note,account
                    convertedLines.Add("id,created_at,name,nextdate,price,site,note,account");
                }

                // 處理資料行
                for (int i = 1; i < lines.Length; i++)
                {
                    var line = lines[i];
                    if (string.IsNullOrWhiteSpace(line)) continue;

                    try
                    {
                        var convertedLine = ConvertDataLine(line, tableType);
                        if (!string.IsNullOrEmpty(convertedLine))
                        {
                            convertedLines.Add(convertedLine);
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"轉換第 {i + 1} 行時發生錯誤: {ex.Message}");
                    }
                }

                // 寫入輸出檔案
                var utf8WithBom = new System.Text.UTF8Encoding(true);
                File.WriteAllLines(outputFile, convertedLines, utf8WithBom);
            });
        }

        private string ConvertDataLine(string line, string tableType)
        {
            // 簡單的 CSV 解析（假設沒有複雜的引號處理）
            var fields = ParseCsvLine(line);
            
            if (tableType == "food")
            {
                // Appwrite Food 欄位順序: $id,name,price,photo,shop,todate,photohash,$createdAt,$updatedAt
                // Supabase Food 欄位順序: id,created_at,name,todate,amount,photo,price,shop,photohash
                if (fields.Length >= 7)
                {
                    var appwriteId = CleanField(fields[0]);
                    var id = ConvertToUuid(appwriteId); // 轉換為 UUID 格式
                    var name = CleanField(fields[1]);
                    var price = CleanField(fields[2]);
                    var photo = CleanField(fields[3]);
                    var shop = CleanField(fields[4]);
                    var todate = ConvertDateFormat(CleanField(fields[5]));
                    var photohash = fields.Length > 6 ? CleanField(fields[6]) : "";
                    var createdAt = fields.Length > 7 ? ConvertDateFormat(CleanField(fields[7])) : DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffffff+00");
                    var amount = "1"; // 預設數量

                    return $"{id},{createdAt},{name},{todate},{amount},{photo},{price},{shop},{photohash}";
                }
            }
            else if (tableType == "subscription")
            {
                // Appwrite Subscription 欄位順序: $id,name,nextdate,price,site,note,account,$createdAt,$updatedAt
                // Supabase Subscription 欄位順序: id,created_at,name,nextdate,price,site,note,account
                if (fields.Length >= 7)
                {
                    var appwriteId = CleanField(fields[0]);
                    var id = ConvertToUuid(appwriteId); // 轉換為 UUID 格式
                    var name = CleanField(fields[1]);
                    var nextdate = ConvertDateFormat(CleanField(fields[2]));
                    var price = CleanField(fields[3]);
                    var site = CleanField(fields[4]);
                    var note = CleanField(fields[5]);
                    var account = CleanField(fields[6]);
                    var createdAt = fields.Length > 7 ? ConvertDateFormat(CleanField(fields[7])) : DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffffff+00");

                    return $"{id},{createdAt},{name},{nextdate},{price},{site},{note},{account}";
                }
            }

            return "";
        }

        private string[] ParseCsvLine(string line)
        {
            // 簡單的 CSV 解析
            var fields = new List<string>();
            var current = new System.Text.StringBuilder();
            bool inQuotes = false;

            for (int i = 0; i < line.Length; i++)
            {
                char c = line[i];
                
                if (c == '"')
                {
                    inQuotes = !inQuotes;
                }
                else if (c == ',' && !inQuotes)
                {
                    fields.Add(current.ToString());
                    current.Clear();
                }
                else
                {
                    current.Append(c);
                }
            }
            
            fields.Add(current.ToString());
            return fields.ToArray();
        }

        private string CleanField(string field)
        {
            if (string.IsNullOrEmpty(field)) return "";
            
            // 移除前後的引號
            field = field.Trim();
            if (field.StartsWith("\"") && field.EndsWith("\""))
            {
                field = field.Substring(1, field.Length - 2);
            }
            
            return field;
        }

        private string ConvertDateFormat(string dateValue)
        {
            if (string.IsNullOrEmpty(dateValue)) return "";

            try
            {
                if (DateTime.TryParse(dateValue, out DateTime parsedDate))
                {
                    // 轉換為 Supabase 格式：yyyy-MM-dd HH:mm:ss.ffffff+00
                    return parsedDate.ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss.ffffff+00", System.Globalization.CultureInfo.InvariantCulture);
                }
                
                return dateValue;
            }
            catch
            {
                return dateValue;
            }
        }

        private string ConvertToUuid(string appwriteId)
        {
            try
            {
                // 移除可能的引號和空白
                appwriteId = appwriteId.Trim().Trim('"');
                
                // 如果已經是 UUID 格式，直接返回
                if (Guid.TryParse(appwriteId, out _))
                {
                    return appwriteId;
                }
                
                // 如果 Appwrite ID 長度不足，用零填充到 32 個字符
                if (appwriteId.Length < 32)
                {
                    appwriteId = appwriteId.PadRight(32, '0');
                }
                else if (appwriteId.Length > 32)
                {
                    // 如果太長，截取前 32 個字符
                    appwriteId = appwriteId.Substring(0, 32);
                }
                
                // 將 32 個字符的字符串轉換為 UUID 格式 (8-4-4-4-12)
                var uuid = $"{appwriteId.Substring(0, 8)}-{appwriteId.Substring(8, 4)}-{appwriteId.Substring(12, 4)}-{appwriteId.Substring(16, 4)}-{appwriteId.Substring(20, 12)}";
                
                // 驗證生成的 UUID 是否有效
                if (Guid.TryParse(uuid, out _))
                {
                    return uuid;
                }
                else
                {
                    // 如果轉換失敗，生成一個新的 UUID
                    return Guid.NewGuid().ToString();
                }
            }
            catch
            {
                // 如果任何步驟失敗，生成一個新的 UUID
                return Guid.NewGuid().ToString();
            }
        }

        private async void TestConverter_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                TestConverterButton.IsEnabled = false;
                TestConverterButton.Content = "測試中...";
                ShowStatusMessage("正在測試 CSV 轉換功能...", Brushes.Blue);

                await TestCsvConverter.RunTest();
                
                ShowStatusMessage("CSV 轉換功能測試完成！", Brushes.Green);
            }
            catch (Exception ex)
            {
                ShowStatusMessage($"測試 CSV 轉換功能時發生錯誤：{ex.Message}", Brushes.Red);
            }
            finally
            {
                TestConverterButton.IsEnabled = true;
                TestConverterButton.Content = "🧪 測試轉換功能";
            }
        }
    }
}