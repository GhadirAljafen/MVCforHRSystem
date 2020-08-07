using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HR.Website.Models
{
    public class UserView
    {
        [Display(Name = "Id")]
        public int UserID { get; set; }
        [Display(Name = "First Name")]
        public string Name { get; set; }
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        [Display(Name = "Job Title")]
        public string JobTitle { get; set; }
        [Display(Name = "Phone Number")]
        public string Mobile { get; set; }
        [Display(Name = "Email Address")]
        public string Email { get; set; }

    }
}