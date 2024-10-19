using System.ComponentModel.DataAnnotations;

namespace HFFitnessApp.Models
{
    public class SignInModel
    {
        [Required]
        public string? Username { get; set; } // Nullable string

        [Required]
        [DataType(DataType.Password)]
        public string? Password { get; set; } // Nullable string
    }

}
