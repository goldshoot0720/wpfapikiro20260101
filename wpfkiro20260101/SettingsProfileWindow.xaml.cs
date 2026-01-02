using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using wpfkiro20260101.Models;
using wpfkiro20260101.Services;

namespace wpfkiro20260101
{
    public partial class SettingsProfileWindow : Window
    {
        private readonly SettingsProfileService _profileService;
        private List<SettingsProfile> _allProfiles = new List<SettingsProfile>();
        private List<SettingsProfile> _filteredProfiles = new List<SettingsProfile>();

        public SettingsProfileWindow()
        {
            InitializeComponent();
            _profileService = SettingsProfileService.Instance;
            
            this.Loaded += async (s, e) => await LoadProfilesAsync();
        }

        private async Task LoadProfilesAsync()
        {
            try
            {
                _allProfiles = await _profileService.GetAllProfilesAsync();
                ApplyFilter();
                UpdateUI();
                UpdateStatusText("設定檔載入完成");
            }
            catch (Exception ex)
            {
                UpdateStatusText($"載入設定檔失敗：{ex.Message}");
            }
        }

        private void ApplyFilter()
        {
            var searchText = SearchTextBox.Text?.ToLower() ?? "";
            
            if (string.IsNullOrWhiteSpace(searchText))
            {
                _filteredProfiles = _allProfiles.ToList();
            }
            else
            {
                _filteredProfiles = _allProfiles.Where(p => 
                    p.ProfileName.ToLower().Contains(searchText) ||
                    p.BackendService.ToString().ToLower().Contains(searchText) ||
                    p.Description.ToLower().Contains(searchText)
                ).ToList();
            }

            ProfilesListBox.ItemsSource = _filteredProfiles;
        }

        private void UpdateUI()
        {
            ProfileCountText.Text = _allProfiles.Count.ToString();
            
            var canAddMore = _profileService.CanAddMoreProfiles();
            CreateProfileButton.IsEnabled = canAddMore;
            
            if (!canAddMore)
            {
                StorageInfoText.Text = "已達上限 (100筆)";
                StorageInfoText.Foreground = System.Windows.Media.Brushes.Red;
            }
            else if (_allProfiles.Count >= 80)
            {
                StorageInfoText.Text = "接近上限";
                StorageInfoText.Foreground = System.Windows.Media.Brushes.Orange;
            }
            else
            {
                StorageInfoText.Text = "可用空間充足";
                StorageInfoText.Foreground = System.Windows.Media.Brushes.Green;
            }

            var hasSelection = ProfilesListBox.SelectedItem != null;
            LoadProfileButton.IsEnabled = hasSelection;
            EditProfileButton.IsEnabled = hasSelection;
            DeleteProfileButton.IsEnabled = hasSelection;
            ExportSelectedButton.IsEnabled = hasSelection;
        }

        private void UpdateStatusText(string message)
        {
            StatusText.Text = $"{DateTime.Now:HH:mm:ss} - {message}";
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyFilter();
        }

        private void ProfilesListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateUI();
        }

        private async void CreateProfile_Click(object sender, RoutedEventArgs e)
        {
            // 嘗試使用原始對話框，如果失敗則使用簡化版本
            try
            {
                var dialog = new CreateProfileDialog();
                if (dialog.ShowDialog() == true)
                {
                    await ProcessCreateProfile(dialog.ProfileName, dialog.Description);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"原始對話框失敗，使用簡化版本: {ex.Message}");
                
                // 使用簡化對話框
                var simpleDialog = new SimpleCreateProfileDialog();
                if (simpleDialog.ShowDialog() == true)
                {
                    await ProcessCreateProfile(simpleDialog.ProfileName, simpleDialog.Description);
                }
            }
        }

        private async Task ProcessCreateProfile(string profileName, string description)
        {
            try
            {
                CreateProfileButton.IsEnabled = false;
                UpdateStatusText("正在儲存設定檔...");

                var result = await _profileService.CreateFromCurrentSettingsAsync(profileName, description);

                if (result.Success)
                {
                    await LoadProfilesAsync();
                    UpdateStatusText($"設定檔 '{profileName}' 儲存成功");
                }
                else
                {
                    UpdateStatusText($"儲存失敗：{result.ErrorMessage}");
                    MessageBox.Show(result.ErrorMessage, "錯誤", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                UpdateStatusText($"儲存設定檔時發生錯誤：{ex.Message}");
                MessageBox.Show($"儲存設定檔時發生錯誤：{ex.Message}", "錯誤", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                CreateProfileButton.IsEnabled = _profileService.CanAddMoreProfiles();
            }
        }

        private async void LoadProfile_Click(object sender, RoutedEventArgs e)
        {
            if (ProfilesListBox.SelectedItem is not SettingsProfile selectedProfile)
                return;

            var result = MessageBox.Show(
                $"確定要載入設定檔 '{selectedProfile.ProfileName}' 嗎？\n\n這將會覆蓋當前的設定。",
                "確認載入",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    LoadProfileButton.IsEnabled = false;
                    UpdateStatusText("正在載入設定檔...");

                    var loadResult = await _profileService.LoadProfileAsync(selectedProfile.Id);
                    if (loadResult.Success)
                    {
                        UpdateStatusText($"設定檔 '{selectedProfile.ProfileName}' 載入成功");
                        MessageBox.Show("設定檔載入成功！所有設定已即時更新，無需重新啟動應用程式。", "成功", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        UpdateStatusText($"載入失敗：{loadResult.ErrorMessage}");
                        MessageBox.Show(loadResult.ErrorMessage, "錯誤", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                catch (Exception ex)
                {
                    UpdateStatusText($"載入設定檔時發生錯誤：{ex.Message}");
                    MessageBox.Show($"載入設定檔時發生錯誤：{ex.Message}", "錯誤", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                finally
                {
                    LoadProfileButton.IsEnabled = true;
                }
            }
        }

        private async void EditProfile_Click(object sender, RoutedEventArgs e)
        {
            if (ProfilesListBox.SelectedItem is not SettingsProfile selectedProfile)
                return;

            try
            {
                // 創建設定檔的副本以避免直接修改原始物件
                var profileCopy = new SettingsProfile
                {
                    Id = selectedProfile.Id,
                    ProfileName = selectedProfile.ProfileName,
                    Description = selectedProfile.Description,
                    BackendService = selectedProfile.BackendService,
                    ApiUrl = selectedProfile.ApiUrl,
                    ProjectId = selectedProfile.ProjectId,
                    ApiKey = selectedProfile.ApiKey,
                    DatabaseId = selectedProfile.DatabaseId,
                    BucketId = selectedProfile.BucketId,
                    FoodCollectionId = selectedProfile.FoodCollectionId,
                    SubscriptionCollectionId = selectedProfile.SubscriptionCollectionId,
                    CreatedAt = selectedProfile.CreatedAt,
                    UpdatedAt = selectedProfile.UpdatedAt
                };

                var dialog = new EditProfileDialog(profileCopy)
                {
                    Owner = this
                };

                if (dialog.ShowDialog() == true)
                {
                    try
                    {
                        EditProfileButton.IsEnabled = false;
                        UpdateStatusText("正在更新設定檔...");

                        var result = await _profileService.SaveProfileAsync(profileCopy);
                        if (result.Success)
                        {
                            await LoadProfilesAsync();
                            UpdateStatusText($"設定檔 '{profileCopy.ProfileName}' 更新成功");
                            
                            // 如果編輯的是當前使用的設定檔，詢問是否要重新載入
                            var currentSettings = AppSettings.Instance;
                            if (profileCopy.BackendService == currentSettings.BackendService &&
                                profileCopy.ApiUrl == currentSettings.ApiUrl &&
                                profileCopy.ProjectId == currentSettings.ProjectId)
                            {
                                var result2 = MessageBox.Show(
                                    "檢測到您編輯的設定檔與當前連線設定相似，是否要重新載入此設定檔以保持一致性？",
                                    "設定同步",
                                    MessageBoxButton.YesNo,
                                    MessageBoxImage.Question);

                                if (result2 == MessageBoxResult.Yes)
                                {
                                    var loadResult = await _profileService.LoadProfileAsync(profileCopy.Id);
                                    if (loadResult.Success)
                                    {
                                        UpdateStatusText("設定檔已重新載入並保持一致性");
                                    }
                                }
                            }
                        }
                        else
                        {
                            UpdateStatusText($"更新失敗：{result.ErrorMessage}");
                            MessageBox.Show(result.ErrorMessage, "錯誤", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    catch (Exception ex)
                    {
                        UpdateStatusText($"更新設定檔時發生錯誤：{ex.Message}");
                        MessageBox.Show($"更新設定檔時發生錯誤：{ex.Message}", "錯誤", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    finally
                    {
                        EditProfileButton.IsEnabled = true;
                    }
                }
            }
            catch (Exception ex)
            {
                UpdateStatusText($"開啟編輯對話框時發生錯誤：{ex.Message}");
                MessageBox.Show($"開啟編輯對話框時發生錯誤：{ex.Message}", "錯誤", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void DeleteProfile_Click(object sender, RoutedEventArgs e)
        {
            if (ProfilesListBox.SelectedItem is not SettingsProfile selectedProfile)
                return;

            var result = MessageBox.Show(
                $"確定要刪除設定檔 '{selectedProfile.ProfileName}' 嗎？\n\n此操作無法復原。",
                "確認刪除",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    DeleteProfileButton.IsEnabled = false;
                    UpdateStatusText("正在刪除設定檔...");

                    var deleteResult = await _profileService.DeleteProfileAsync(selectedProfile.Id);
                    if (deleteResult.Success)
                    {
                        await LoadProfilesAsync();
                        UpdateStatusText($"設定檔 '{selectedProfile.ProfileName}' 已刪除");
                    }
                    else
                    {
                        UpdateStatusText($"刪除失敗：{deleteResult.ErrorMessage}");
                        MessageBox.Show(deleteResult.ErrorMessage, "錯誤", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                catch (Exception ex)
                {
                    UpdateStatusText($"刪除設定檔時發生錯誤：{ex.Message}");
                    MessageBox.Show($"刪除設定檔時發生錯誤：{ex.Message}", "錯誤", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                finally
                {
                    DeleteProfileButton.IsEnabled = true;
                }
            }
        }

        private async void ImportProfiles_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Title = "選擇要匯入的設定檔案",
                Filter = "JSON 檔案 (*.json)|*.json|所有檔案 (*.*)|*.*",
                DefaultExt = "json"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    ImportProfilesButton.IsEnabled = false;
                    UpdateStatusText("正在匯入設定檔...");

                    var jsonContent = await File.ReadAllTextAsync(openFileDialog.FileName);
                    var result = await _profileService.ImportProfilesAsync(jsonContent);

                    if (result.Success)
                    {
                        await LoadProfilesAsync();
                        UpdateStatusText($"成功匯入 {result.Data?.Count ?? 0} 筆設定檔");
                        MessageBox.Show($"成功匯入 {result.Data?.Count ?? 0} 筆設定檔", "匯入成功", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        UpdateStatusText($"匯入失敗：{result.ErrorMessage}");
                        MessageBox.Show(result.ErrorMessage, "匯入失敗", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
                catch (Exception ex)
                {
                    UpdateStatusText($"匯入設定檔時發生錯誤：{ex.Message}");
                    MessageBox.Show($"匯入設定檔時發生錯誤：{ex.Message}", "錯誤", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                finally
                {
                    ImportProfilesButton.IsEnabled = true;
                }
            }
        }

        private async void ExportProfiles_Click(object sender, RoutedEventArgs e)
        {
            if (_allProfiles.Count == 0)
            {
                MessageBox.Show("沒有設定檔可以匯出", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var saveFileDialog = new SaveFileDialog
            {
                Title = "儲存設定檔案",
                Filter = "JSON 檔案 (*.json)|*.json",
                DefaultExt = "json",
                FileName = $"settings_profiles_{DateTime.Now:yyyyMMdd_HHmmss}.json"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    ExportProfilesButton.IsEnabled = false;
                    UpdateStatusText("正在匯出設定檔...");

                    var result = await _profileService.ExportProfilesAsync();
                    if (result.Success)
                    {
                        await File.WriteAllTextAsync(saveFileDialog.FileName, result.Data);
                        UpdateStatusText($"成功匯出 {_allProfiles.Count} 筆設定檔");
                        MessageBox.Show($"成功匯出 {_allProfiles.Count} 筆設定檔到：\n{saveFileDialog.FileName}", "匯出成功", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        UpdateStatusText($"匯出失敗：{result.ErrorMessage}");
                        MessageBox.Show(result.ErrorMessage, "匯出失敗", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                catch (Exception ex)
                {
                    UpdateStatusText($"匯出設定檔時發生錯誤：{ex.Message}");
                    MessageBox.Show($"匯出設定檔時發生錯誤：{ex.Message}", "錯誤", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                finally
                {
                    ExportProfilesButton.IsEnabled = true;
                }
            }
        }

        private async void ExportSelected_Click(object sender, RoutedEventArgs e)
        {
            if (ProfilesListBox.SelectedItem is not SettingsProfile selectedProfile)
                return;

            var saveFileDialog = new SaveFileDialog
            {
                Title = "儲存選中的設定檔案",
                Filter = "JSON 檔案 (*.json)|*.json",
                DefaultExt = "json",
                FileName = $"{selectedProfile.ProfileName}_{DateTime.Now:yyyyMMdd_HHmmss}.json"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    ExportSelectedButton.IsEnabled = false;
                    UpdateStatusText("正在匯出選中的設定檔...");

                    var result = await _profileService.ExportProfilesAsync(new List<string> { selectedProfile.Id });
                    if (result.Success)
                    {
                        await File.WriteAllTextAsync(saveFileDialog.FileName, result.Data);
                        UpdateStatusText($"成功匯出設定檔 '{selectedProfile.ProfileName}'");
                        MessageBox.Show($"成功匯出設定檔到：\n{saveFileDialog.FileName}", "匯出成功", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        UpdateStatusText($"匯出失敗：{result.ErrorMessage}");
                        MessageBox.Show(result.ErrorMessage, "匯出失敗", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                catch (Exception ex)
                {
                    UpdateStatusText($"匯出設定檔時發生錯誤：{ex.Message}");
                    MessageBox.Show($"匯出設定檔時發生錯誤：{ex.Message}", "錯誤", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                finally
                {
                    ExportSelectedButton.IsEnabled = true;
                }
            }
        }

        private async void QuickExportAll_Click(object sender, RoutedEventArgs e)
        {
            if (_allProfiles.Count == 0)
            {
                MessageBox.Show("沒有設定檔可以匯出", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            try
            {
                QuickExportButton.IsEnabled = false;
                UpdateStatusText("正在快速匯出所有設定檔...");

                var saveFileDialog = new SaveFileDialog
                {
                    Title = "選擇設定檔匯出位置",
                    Filter = "JSON 檔案 (*.json)|*.json|所有檔案 (*.*)|*.*",
                    DefaultExt = "json",
                    FileName = $"設定檔備份_{DateTime.Now:yyyyMMdd_HHmmss}.json",
                    InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    var result = await _profileService.ExportProfilesAsync();
                    if (result.Success)
                    {
                        await File.WriteAllTextAsync(saveFileDialog.FileName, result.Data);
                        var fileInfo = new FileInfo(saveFileDialog.FileName);
                        UpdateStatusText($"成功匯出 {_allProfiles.Count} 筆設定檔");
                        
                        var result2 = MessageBox.Show(
                            $"成功匯出 {_allProfiles.Count} 筆設定檔到：\n{fileInfo.DirectoryName}\n檔案：{fileInfo.Name}\n\n是否要開啟檔案位置？",
                            "快速匯出成功",
                            MessageBoxButton.YesNo,
                            MessageBoxImage.Information);

                        if (result2 == MessageBoxResult.Yes)
                        {
                            System.Diagnostics.Process.Start("explorer.exe", $"/select,\"{saveFileDialog.FileName}\"");
                        }
                    }
                    else
                    {
                        UpdateStatusText($"匯出失敗：{result.ErrorMessage}");
                        MessageBox.Show(result.ErrorMessage, "匯出失敗", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    UpdateStatusText("已取消匯出");
                }
            }
            catch (Exception ex)
            {
                UpdateStatusText($"快速匯出時發生錯誤：{ex.Message}");
                MessageBox.Show($"快速匯出時發生錯誤：{ex.Message}", "錯誤", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                QuickExportButton.IsEnabled = true;
            }
        }

        private async void ExportToFolder_Click(object sender, RoutedEventArgs e)
        {
            if (_allProfiles.Count == 0)
            {
                MessageBox.Show("沒有設定檔可以匯出", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            try
            {
                ExportToFolderButton.IsEnabled = false;
                UpdateStatusText("正在選擇匯出資料夾...");

                var selectedFolder = FolderSelectDialog.SelectFolderWithMessage("選擇設定檔匯出資料夾");

                if (!string.IsNullOrEmpty(selectedFolder))
                {
                    UpdateStatusText("正在匯出設定檔...");
                    
                    var result = await _profileService.ExportProfilesAsync();
                    if (result.Success)
                    {
                        var fileName = $"設定檔備份_{DateTime.Now:yyyyMMdd_HHmmss}.json";
                        var filePath = Path.Combine(selectedFolder, fileName);
                        
                        await File.WriteAllTextAsync(filePath, result.Data);
                        UpdateStatusText($"成功匯出 {_allProfiles.Count} 筆設定檔到指定資料夾");
                        
                        var result2 = MessageBox.Show(
                            $"成功匯出 {_allProfiles.Count} 筆設定檔到：\n{selectedFolder}\n檔案：{fileName}\n\n是否要開啟檔案位置？",
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
                        UpdateStatusText($"匯出失敗：{result.ErrorMessage}");
                        MessageBox.Show(result.ErrorMessage, "匯出失敗", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    UpdateStatusText("已取消匯出");
                }
            }
            catch (Exception ex)
            {
                UpdateStatusText($"匯出到資料夾時發生錯誤：{ex.Message}");
                MessageBox.Show($"匯出到資料夾時發生錯誤：{ex.Message}", "錯誤", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                ExportToFolderButton.IsEnabled = true;
            }
        }
    }
}