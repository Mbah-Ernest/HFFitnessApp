using System;
using System.ComponentModel.DataAnnotations;

namespace HFFitnessApp.Models
{
    public class SavedPlan
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }

        public string WorkoutPlan { get; set; }

        public string MealPlan { get; set; }

        public string Notes { get; set; }

        public DateTime DateSaved { get; set; } = DateTime.Now;
    }
}
