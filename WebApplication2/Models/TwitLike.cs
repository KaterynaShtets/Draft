using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Tut20.Models
{
    [Table("TwitLike")]
    public class TwitLike
    {
        public int TwitId { get; set; }
        public string UserName { get; set; }
        public int Value { get; set; }  // +1, -1
        public DateTime When { get; set; }
        //
        public Twit Twit { set; get; }
    }
}
