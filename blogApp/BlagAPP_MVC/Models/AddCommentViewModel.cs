using System.ComponentModel.DataAnnotations;

namespace BlagAPP_MVC.Models
{
    public class AddCommentViewModel
    {
        [Required]
        public string ArticleId { get; set; } = string.Empty;

        [Required(ErrorMessage = "Введите текст комментария")]
        [StringLength(1000, ErrorMessage = "Комментарий не должен превышать 1000 символов")]
        public string Content { get; set; } = string.Empty;

        public string? SearchTitle { get; set; }

        public string? SearchTag { get; set; }
    }
}
