using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication2.Models
{
    public class LectureVisit
    {
        public int Idlec { get; set; }
        public string Title { get; set; }
        public int Count { get; set; }
        public List<string> Students { get; set; }
    }
}
