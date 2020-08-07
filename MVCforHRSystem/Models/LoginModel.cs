using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HR.Website.Models
{
    public class LoginModel
    {
        public int UserID { get; set; }
        [Required(ErrorMessage = "Please enter the username.")]
        public string Username { set; get; }
        [Required(ErrorMessage = "Please enter the password.")]
        [DataType(DataType.Password)]
        public string Password { set; get; }
        public int Role { set; get; }
        
    }
}