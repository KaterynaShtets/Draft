using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tut20.Models
{
    [Table("Solving")]
    public class Solving
    {
        public int Id { get; set; }
        public int TaskId { get; set; }
        public string Code { get; set; }
        public DateTime When { get; set; }
        public string UserName { get; set; }
        public string TaskName { get; set; }
        public int? Success { get; set; }
        public string Message { get; set; }
        public int? ExamId { get; set; }
    }

}
