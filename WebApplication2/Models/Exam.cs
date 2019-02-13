using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tut20.Models
{
    [Table("Exam")]
    public class Exam
    {
        public int Id { get; set; }

        [Display(Name="Tutor")]
        public string TutorName { get; set; }

        private bool hidden;
        public bool Hidden {
            get { return hidden; }
            set
            {   // hidden exam is always passive
                hidden = value;
                if (hidden)
                    isActive = false;
            }
        }

        private bool isActive;
        public bool IsActive
        {
            get { return isActive; }
            set
            {   // hidden exam is always passive
                if (hidden)
                    isActive = false;
                else
                    isActive = value;
            }
        }

        public string Title { get; set; }
        public int Duration { get; set; }
        public int Rait { get; set; }
        public int TaskId { get; set; }
        //
        public virtual List<Ticket> Tickets { get; set; }
    }

}
