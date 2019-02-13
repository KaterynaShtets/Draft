using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using WebApplication2.Data;

namespace Tut20.Models
{  
    public enum EventKind { Unknown = 0, Register = 1, Login = 2, LoginFailed = 3, See = 4, PasswordChanged = 5};

    [Table("Event")]
    public class Event
    {
        public int Id { get; set; }
        public EventKind Kind { get; set; }
        public string TutorName { get; set; }
        public DateTime Date { get; set; }
        public string Text { get; set; }
        public string Param { get; set; }
        public string RemoteAddress { get; set; }


        //public static void Log(ApplicationDbContext db, EventKind kind, string tutorName="", string text="")
        //{
        //    db.Events.Add(new Event
        //    {
        //        Kind = kind,
        //        TutorName = tutorName,
        //        Date = DateTime.Now,
        //        Text = text,
        //    });
        //    db.SaveChanges();
        //}
    }


}
