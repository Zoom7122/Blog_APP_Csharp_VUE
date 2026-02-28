using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BlogAPP_Core.Models
{
    public class UpdateUserDto
    {
        [Required]
        [MinLength(2)]
        public string FirstName { get; set; }

        public string Avatar_url { get; set; }

        public string Bio { get; set; }
    }
}
