using System;

namespace wpfkiro20260101.Models
{
    public class FoodSubscription
    {
        public string Id { get; set; } = string.Empty;
        public string SubscriptionName { get; set; } = string.Empty;
        public DateTime NextDate { get; set; }
        public int Price { get; set; }
        public string Site { get; set; } = string.Empty;
        public string Note { get; set; } = string.Empty;
        public string Account { get; set; } = string.Empty;
        
        // Legacy properties - keeping for backward compatibility
        public string FoodName { get; set; } = string.Empty;
        public string StringToDate { get; set; } = string.Empty;
        public DateTime DateTime { get; set; }
        public string Photo { get; set; } = string.Empty;
        public string Shop { get; set; } = string.Empty;
        public string PhotoHash { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}