using HR.Website.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HR.Website.Models
{
    public class VacationView
    {
        [Display(Name = "Id")]
        public int VacationID { set; get; }
        public int EmployeeID { set; get; }
        [Display(Name = "First Name")]
        public string Name { get; set; }
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        [Display(Name = "Vacation Type")]
        public Types Type { set; get; }
        [Display(Name = "Start")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        [DataType(DataType.Date)]
        public DateTime StartDate { set; get; }
        [Display(Name = "End")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        [DataType(DataType.Date)]
        public DateTime EndDate { set; get; }
        public Statuses Status { set; get; }
        public string Description { set; get; }
        public string Attatchment { set; get; }
        public string RejectionReason { set; get; }
    }
}