using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tut20.Models
{
    public class StatSolvingViewModel
    {
        public string BackSample { set; get; }
        public int Hours { set; get; }
        public string ForeSample { set; get; }
        public List<int> SolvByWeeks { set; get; }
    }
}
