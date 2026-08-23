using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.G02.Shared.Dtos.Auth
{
    public class LoginRequest
    {
        [Required]
        [EmailAddress]
        [MaxLength(100)]
        [MinLength(8)]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email Address")]
        [RegularExpression(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", ErrorMessage = "Invalid email format.")]
        public string Email { get; set; }


        [Required]
        [MinLength(6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d@$!%*#?&]{6,}$", ErrorMessage = "Password must be at least 6 characters long and contain at least one letter and one number.")]
        public string Password { get; set; }
    }
}
