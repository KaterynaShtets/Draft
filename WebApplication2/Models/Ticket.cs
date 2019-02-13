using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Tut20.Models
{
    public enum TicketState { Open = 0, Fail = 1, Success = 2, Reopen = 3 }

    [Table("Ticket")]
    public class Ticket
    {
        public int Id { get; set; }
        public int ExamId { get; set; }
        public string UserName { get; set; }
        public int TaskId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime FinishTime { get; set; }
        public TicketState State { get; set; }  
        //
        public virtual Exam Exam { get; set; }

        public TimeSpan GetRestTime()
        {
            return StartTime + TimeSpan.FromMinutes(Exam.Duration) - DateTime.Now;
        }
    }

}
