using System.ComponentModel.DataAnnotations;

namespace BlagAPP_MVC.Models
{

    public class UpdateProfileViewModel
    {
        [Required]
        [MinLength(2)]
        [Display(Name = "Ваше имя")]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Url]
        [Display(Name = "URL аватара")]
        public string? AvatarUrl { get; set; }

        [Display(Name = "О себе")]
        public string? Bio { get; set; }
    }
}
