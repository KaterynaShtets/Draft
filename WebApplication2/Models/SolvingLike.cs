using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;


namespace Tut20.Models
{
    [Table("SolvingLike")]
    public class SolvingLike
    {
        public int SolvingId { get; set; }
        public string UserName { get; set; }
        public int Value { get; set; }  // +1, -1
        public DateTime When { get; set; }
        //
        public Solving Solving { set; get; }
    }
}

