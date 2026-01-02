using System;
using System.ComponentModel.DataAnnotations;

namespace wpfkiro20260101.Models
{
    public class SettingsProfile
    {
        public string Id { get; set; } = "";
        
        [Required]
        [StringLength(100)]
        public string ProfileName { get; set; } = "";
        
        public BackendServiceType BackendService { get; set; } = BackendServiceType.Appwrite;
        
        // 通用設定
        public string ApiUrl { get; set; } = "";
        public string ProjectId { get; set; } = "";
        public string ApiKey { get; set; } = "";
        
        // Appwrite 專用設定
        public string DatabaseId { get; set; } = "";
        public string BucketId { get; set; } = "";
        public string FoodCollectionId { get; set; } = "";
        public string SubscriptionCollectionId { get; set; } = "";
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        
        public string Description { get; set; } = "";
        
        // 從 AppSettings 創建設定檔
        public static SettingsProfile FromAppSettings(AppSettings settings, string profileName, string description = "")
        {
            return new SettingsProfile
            {
                Id = Guid.NewGuid().ToString(),
                ProfileName = profileName,
                BackendService = settings.BackendService,
                ApiUrl = settings.ApiUrl,
                ProjectId = settings.ProjectId,
                ApiKey = settings.ApiKey,
                DatabaseId = settings.DatabaseId,
                BucketId = settings.BucketId,
                FoodCollectionId = settings.FoodCollectionId,
                SubscriptionCollectionId = settings.SubscriptionCollectionId,
                Description = description,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
        }
        
        // 應用到 AppSettings
        public void ApplyToAppSettings(AppSettings settings)
        {
            settings.BackendService = this.BackendService;
            
            var serviceSettings = settings.GetCurrentServiceSettings();
            serviceSettings.ApiUrl = this.ApiUrl;
            serviceSettings.ProjectId = this.ProjectId;
            serviceSettings.ApiKey = this.ApiKey;
            
            if (this.BackendService == BackendServiceType.Appwrite)
            {
                settings.Appwrite.DatabaseId = this.DatabaseId;
                settings.Appwrite.BucketId = this.BucketId;
                settings.Appwrite.FoodCollectionId = this.FoodCollectionId;
                settings.Appwrite.SubscriptionCollectionId = this.SubscriptionCollectionId;
            }
        }
        
        public override string ToString()
        {
            return $"{ProfileName} ({BackendService})";
        }
    }
}