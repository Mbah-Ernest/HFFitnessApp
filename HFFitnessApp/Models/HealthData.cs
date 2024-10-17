using System.ComponentModel.DataAnnotations;

public class HealthData
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string UserId { get; set; } // Foreign key linking to IdentityUser's Id

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
    public string Gender { get; set; }

    [Required]
    public string FitnessGoal { get; set; }

    [Required]
    public string DietaryPreferences { get; set; }

    [Required]
    public string FoodPreferences { get; set; }

    public string HealthConditions { get; set; }

    public string Allergies { get; set; }

    [Required]
    public string ActivityLevel { get; set; }

    [Required]
    public string Occupation { get; set; }

    public string SleepPatterns { get; set; }

    public string InjuryHistory { get; set; }
}
