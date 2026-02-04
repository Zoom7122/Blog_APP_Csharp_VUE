using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Text;

namespace blogApp_DAL.Model
{

    [Table("User")]
    public class User
    {
        // id (TEXT, Not Null) 
        [Key]
        public string Id { get; set; }

        // firstName (TEXT, Not Null)
        [Required]
        public string FirstName { get; set; }

        // email (TEXT, Not Null)
        [Required]
        [EmailAddress] 
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        public DateTime Created_at { get; set; }

        public string Avatar_url { get; set; }

        public string Bio { get; set; }

        [Required] 
        public bool Is_active { get; set; } = true; 

        [Required]
        public string Role { get; set; }
    }
}
