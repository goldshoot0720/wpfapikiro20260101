using System;

namespace wpfkiro20260101.Models
{
    public class Food
    {
        public string Id { get; set; } = string.Empty;
        public string FoodName { get; set; } = string.Empty;
        public string Photo { get; set; } = string.Empty;
        public string PhotoHash { get; set; } = string.Empty;
        public string Shop { get; set; } = string.Empty;
        public string Note { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}