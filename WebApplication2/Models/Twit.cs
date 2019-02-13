using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tut20.Models
{
    [Table("Twit")]
    public class Twit
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public string TutorName { get; set; }
        public int? TaskId { get; set; }
        public string UserName { get; set; }
        public DateTime When { get; set; }
        public int? ParentId { get; set; }
    }
}
