using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
namespace HR.Website.Models
{
    public class PagingModel
    {

        public int DisplayLength { get; set; }
        public int DisplayStart { get; set; }
        public string SortCol { get; set; }
        public string SortDir { get; set; }
        public string SearchValue { get; set; } = null;

    }
}