using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugTrackerWithLayout.ViewModels
{
    public class BugListFilterViewModel
    {
        public string Search { get; set; }
        public string Status { get; set; }
        public int? CategoryId { get; set; }
    }
}
