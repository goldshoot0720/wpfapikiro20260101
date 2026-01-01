using System;

namespace wpfkiro20260101.Models
{
    public class Subscription
    {
        public string Id { get; set; } = string.Empty;
        public string SubscriptionName { get; set; } = string.Empty;
        public DateTime NextDate { get; set; }
        public int Price { get; set; }
        public string Site { get; set; } = string.Empty;
        public string Account { get; set; } = string.Empty;
        public string Note { get; set; } = string.Empty;
        
        // Legacy properties - keeping for backward compatibility
        public string StringToDate { get; set; } = string.Empty;
        public DateTime DateTime { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        
        // Foreign key to Food if this subscription is related to a food item
        public string? FoodId { get; set; }
    }
}