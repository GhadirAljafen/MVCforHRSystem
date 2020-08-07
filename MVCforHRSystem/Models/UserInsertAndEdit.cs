using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HR.Website.Models
{
    public class UserInsertAndEdit
    {
        public int UserID { get; set; }
        [Required]
        [Display(Name = "First Name")]
        public string Name { get; set; }
        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        [Required]
        [Display(Name = "Job Title")]
        public string JobTitle { get; set; }
        [Required]
        [Display(Name = "Phone Number")]
        [RegularExpression(@"^(\d{10})$", ErrorMessage = "Enter a valied number")]
        public string Mobile { get; set; }
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [Required]
        public string Username { set; get; }
        public int Role { set; get; } = 1;
        [Display(Name = "Manager ID")]
        public int ManagerID { set; get; }
        public string Password { set; get; } = "P@ssw0rd";

    }
}