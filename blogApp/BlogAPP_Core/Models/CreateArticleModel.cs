using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BlogAPP_Core.Models
{
    public class CreateArticleModel
    {

        [Required(ErrorMessage = "Введите заголовок")]
        [MinLength(3, ErrorMessage = "Заголовок должен содержать минимум 3 символа")]
        public string Title { get; set; }

        public List<string> Tag { get; set; } = new();

        [Url(ErrorMessage = "Укажите корректный URL обложки")]
        public string CoverImage { get; set; }


        [Required(ErrorMessage = "Введите описание")]
        [StringLength(500, ErrorMessage = "Описание не должно превышать 500 символов")]
        public string Description { get; set; }


        [Required(ErrorMessage = "Введите текст статьи")]
        [MinLength(20, ErrorMessage = "Текст статьи должен содержать минимум 20 символов")]
        public string Text { get; set; }

        [Range(1, 1000, ErrorMessage = "Время чтения должно быть от 1 до 1000 минут")]
        public int ReadTime { get; set; }

    }
}
