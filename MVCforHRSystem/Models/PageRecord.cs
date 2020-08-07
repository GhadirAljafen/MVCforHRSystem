using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HR.Website.Models
{
    public class PageRecord<T>
    {
            public List<T> Data { get; set; }
            public int TotalRecord { get; set; }
            public int TotalFilteredRecord { get; set; }
    }
}