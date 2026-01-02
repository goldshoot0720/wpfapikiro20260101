using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using wpfkiro20260101.Models;

namespace wpfkiro20260101.Services
{
    public class SettingsProfileService
    {
        private static readonly string ProfilesFilePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "wpfkiro20260101",
            "profiles.json"
        );

        private List<SettingsProfile> _profiles = new List<SettingsProfile>();
        private static SettingsProfileService? _instance;
        private static readonly object _lock = new object();

        public static SettingsProfileService Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new SettingsProfileService();
                            _instance.LoadProfiles();
                        }
                    }
                }
                return _instance;
            }
        }

        private SettingsProfileService() { }

        public async Task<List<SettingsProfile>> GetAllProfilesAsync()
        {
            return _profiles.OrderBy(p => p.ProfileName).ToList();
        }

        public async Task<SettingsProfile?> GetProfileByIdAsync(string id)
        {
            return _profiles.FirstOrDefault(p => p.Id == id);
        }

        public async Task<SettingsProfile?> GetProfileByNameAsync(string name)
        {
            return _profiles.FirstOrDefault(p => p.ProfileName.Equals(name, StringComparison.OrdinalIgnoreCase));
        }

        public async Task<BackendServiceResult<SettingsProfile>> SaveProfileAsync(SettingsProfile profile)
        {
            try
            {
                // 檢查名稱是否重複
                var existingProfile = _profiles.FirstOrDefault(p => 
                    p.ProfileName.Equals(profile.ProfileName, StringComparison.OrdinalIgnoreCase) && 
                    p.Id != profile.Id);
                
                if (existingProfile != null)
                {
                    return BackendServiceResult<SettingsProfile>.CreateError("設定檔名稱已存在");
                }

                // 檢查是否超過100筆限制
                if (string.IsNullOrEmpty(profile.Id) && _profiles.Count >= 100)
                {
                    return BackendServiceResult<SettingsProfile>.CreateError("設定檔數量已達上限（100筆）");
                }

                profile.UpdatedAt = DateTime.UtcNow;

                // 新增或更新
                var existingIndex = _profiles.FindIndex(p => p.Id == profile.Id);
                if (existingIndex >= 0)
                {
                    _profiles[existingIndex] = profile;
                }
                else
                {
                    if (string.IsNullOrEmpty(profile.Id))
                    {
                        profile.Id = Guid.NewGuid().ToString();
                        profile.CreatedAt = DateTime.UtcNow;
                    }
                    _profiles.Add(profile);
                }

                await SaveProfilesToFileAsync();
                return BackendServiceResult<SettingsProfile>.CreateSuccess(profile);
            }
            catch (Exception ex)
            {
                return BackendServiceResult<SettingsProfile>.CreateError($"儲存設定檔失敗：{ex.Message}");
            }
        }

        public async Task<BackendServiceResult<bool>> DeleteProfileAsync(string id)
        {
            try
            {
                var profile = _profiles.FirstOrDefault(p => p.Id == id);
                if (profile == null)
                {
                    return BackendServiceResult<bool>.CreateError("找不到指定的設定檔");
                }

                _profiles.Remove(profile);
                await SaveProfilesToFileAsync();
                return BackendServiceResult<bool>.CreateSuccess(true);
            }
            catch (Exception ex)
            {
                return BackendServiceResult<bool>.CreateError($"刪除設定檔失敗：{ex.Message}");
            }
        }

        public async Task<BackendServiceResult<SettingsProfile>> CreateFromCurrentSettingsAsync(string profileName, string description = "")
        {
            try
            {
                var currentSettings = AppSettings.Instance;
                var profile = SettingsProfile.FromAppSettings(currentSettings, profileName, description);
                
                return await SaveProfileAsync(profile);
            }
            catch (Exception ex)
            {
                return BackendServiceResult<SettingsProfile>.CreateError($"從當前設定創建設定檔失敗：{ex.Message}");
            }
        }

        public async Task<BackendServiceResult<bool>> LoadProfileAsync(string id)
        {
            try
            {
                var profile = await GetProfileByIdAsync(id);
                if (profile == null)
                {
                    return BackendServiceResult<bool>.CreateError("找不到指定的設定檔");
                }

                var settings = AppSettings.Instance;
                profile.ApplyToAppSettings(settings);
                settings.Save();

                return BackendServiceResult<bool>.CreateSuccess(true);
            }
            catch (Exception ex)
            {
                return BackendServiceResult<bool>.CreateError($"載入設定檔失敗：{ex.Message}");
            }
        }

        public async Task<BackendServiceResult<List<SettingsProfile>>> ImportProfilesAsync(string jsonContent)
        {
            try
            {
                var importedProfiles = JsonSerializer.Deserialize<List<SettingsProfile>>(jsonContent);
                if (importedProfiles == null)
                {
                    return BackendServiceResult<List<SettingsProfile>>.CreateError("無效的JSON格式");
                }

                var successfulImports = new List<SettingsProfile>();
                var errors = new List<string>();

                foreach (var profile in importedProfiles)
                {
                    // 檢查總數限制
                    if (_profiles.Count + successfulImports.Count >= 100)
                    {
                        errors.Add($"已達設定檔數量上限，無法匯入 '{profile.ProfileName}'");
                        continue;
                    }

                    // 重新生成ID避免衝突
                    profile.Id = Guid.NewGuid().ToString();
                    profile.CreatedAt = DateTime.UtcNow;
                    profile.UpdatedAt = DateTime.UtcNow;

                    // 檢查名稱衝突
                    var originalName = profile.ProfileName;
                    var counter = 1;
                    while (_profiles.Any(p => p.ProfileName.Equals(profile.ProfileName, StringComparison.OrdinalIgnoreCase)) ||
                           successfulImports.Any(p => p.ProfileName.Equals(profile.ProfileName, StringComparison.OrdinalIgnoreCase)))
                    {
                        profile.ProfileName = $"{originalName} ({counter})";
                        counter++;
                    }

                    successfulImports.Add(profile);
                }

                _profiles.AddRange(successfulImports);
                await SaveProfilesToFileAsync();

                if (errors.Any())
                {
                    var errorMessage = $"成功匯入 {successfulImports.Count} 筆設定檔。錯誤：{string.Join("; ", errors)}";
                    return BackendServiceResult<List<SettingsProfile>>.CreateError(errorMessage);
                }

                return BackendServiceResult<List<SettingsProfile>>.CreateSuccess(successfulImports);
            }
            catch (Exception ex)
            {
                return BackendServiceResult<List<SettingsProfile>>.CreateError($"匯入設定檔失敗：{ex.Message}");
            }
        }

        public async Task<BackendServiceResult<string>> ExportProfilesAsync(List<string>? profileIds = null)
        {
            try
            {
                var profilesToExport = profileIds == null 
                    ? _profiles 
                    : _profiles.Where(p => profileIds.Contains(p.Id)).ToList();

                var json = JsonSerializer.Serialize(profilesToExport, new JsonSerializerOptions
                {
                    WriteIndented = true
                });

                return BackendServiceResult<string>.CreateSuccess(json);
            }
            catch (Exception ex)
            {
                return BackendServiceResult<string>.CreateError($"匯出設定檔失敗：{ex.Message}");
            }
        }

        private void LoadProfiles()
        {
            try
            {
                if (File.Exists(ProfilesFilePath))
                {
                    var json = File.ReadAllText(ProfilesFilePath);
                    var profiles = JsonSerializer.Deserialize<List<SettingsProfile>>(json);
                    _profiles = profiles ?? new List<SettingsProfile>();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"載入設定檔失敗：{ex.Message}");
                _profiles = new List<SettingsProfile>();
            }
        }

        private async Task SaveProfilesToFileAsync()
        {
            try
            {
                var directory = Path.GetDirectoryName(ProfilesFilePath);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory!);
                }

                var json = JsonSerializer.Serialize(_profiles, new JsonSerializerOptions
                {
                    WriteIndented = true
                });

                await File.WriteAllTextAsync(ProfilesFilePath, json);
            }
            catch (Exception ex)
            {
                throw new Exception($"儲存設定檔失敗：{ex.Message}", ex);
            }
        }

        public int GetProfileCount()
        {
            return _profiles.Count;
        }

        public bool CanAddMoreProfiles()
        {
            return _profiles.Count < 100;
        }
    }
}