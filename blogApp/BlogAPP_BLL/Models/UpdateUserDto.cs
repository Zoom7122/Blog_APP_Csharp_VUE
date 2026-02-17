using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BlogAPP_BLL.Models
{
    public class UpdateUserDto
    {
        [Required]
        [MinLength(2)]
        public string FirstName { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        public string Avatar_url { get; set; }

        public string Bio { get; set; }
    }
}
