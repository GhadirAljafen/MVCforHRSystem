using Foolproof;
using HR.Website.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HR.Website.Models
{
    public class VacationInsertAndEdit
    {
        //hidden
        public int VacationID { set; get; }
        //from session
        public int EmployeeID { set; get; }
        [Required]
        public Types Type { set; get; }
        public string Attatchment { set; get; }
        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-ddThh:mm:ss}")]
        public DateTime StartDate { set; get; }
        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-ddThh:mm:ss}")]
        public DateTime EndDate { set; get; }
        public string Description { set; get; }
        [Required]
        // hiddien
        public Statuses Status { set; get; } = Statuses.Pending; // pending
        [Display(Name ="Reason")]
        [RequiredIf("Status", "Approved")]
        public string RejectionReason { set; get; }

    }
}