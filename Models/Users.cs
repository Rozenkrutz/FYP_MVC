using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace FYP.Models
{
    public class Users
    {
        
        // public string Nric { get; set; }

        [Required(ErrorMessage = "This field is mandatory!")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "This field is mandatory!")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        public int RoleID { get; set; }

        public int MembershipNo { get; set; }

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "This field is mandatory!")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Password not confirmed!")]
        [Required(ErrorMessage = "This field is mandatory!")]
        public string ConfirmPassword { get; set; }
    }  
}
