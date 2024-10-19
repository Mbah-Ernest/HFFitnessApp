using System.ComponentModel.DataAnnotations;

namespace HFFitnessApp.Models
{
    public class HealthData
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty; // No need to mark this as required for form validation

        [Required]
        [Range(1, 120, ErrorMessage = "Please enter a valid age between 1 and 120.")]
        public int Age { get; set; }

        [Required]
        [Range(100, 250, ErrorMessage = "Please enter a valid height between 100 and 250 cm.")]
        public double Height { get; set; }

        [Required]
        [Range(30, 300, ErrorMessage = "Please enter a valid weight between 30 and 300 kg.")]
        public double Weight { get; set; }

        [Required]
        public string Gender { get; set; } = string.Empty;

        [Required]
        public string FitnessGoal { get; set; } = string.Empty;

        [Required]
        public string DietaryPreferences { get; set; } = string.Empty;

        public string FoodPreferences { get; set; } = string.Empty;
        public string HealthConditions { get; set; } = string.Empty;
        public string Allergies { get; set; } = string.Empty;
        public string ActivityLevel { get; set; } = string.Empty;
        public string Occupation { get; set; } = string.Empty;
        public string SleepPatterns { get; set; } = string.Empty;
        public string InjuryHistory { get; set; } = string.Empty;
    }
}
