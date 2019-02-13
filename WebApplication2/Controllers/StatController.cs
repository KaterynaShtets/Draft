using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Tut20.Models;
using WebApplication2.Data;
using WebApplication2.Models;

namespace WebApplication2.Controllers
{
    public class StatController : Controller
    {

        readonly ApplicationDbContext _db;

        public StatController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Solving(int days=1, string backSample = "^[1-9ab]", string foreSample = "^1S")
        {
            // все имена пользователей, соотв. шаблону sample
            Regex regex = new Regex(backSample);
            var backUserNames = _db.Users
                .Select(u => u.UserName)
                .Where(n => regex.IsMatch(n))
                .ToArray();

            regex = new Regex(foreSample);
            var foreUserNames = backUserNames
                .Where(n => regex.IsMatch(n))
                .ToArray();

            // даты первого правильного решения каждой задачи каждым back-юзером 
            var baskUserName_taskId_when_s = _db.Solvings
                .Where(s => s.Success == 2 && backUserNames.Contains(s.UserName))
                .GroupBy(s => new { s.TaskId, s.UserName })
                .Select(g => new { g.Key.TaskId, g.Key.UserName, When = g.Min(x => x.When) })
                .ToArray();

            // даты первого правильного решения каждой задачи
            var taskId_when_s = baskUserName_taskId_when_s
                .GroupBy(s => s.TaskId)
                .Select(g => new { TaskId = g.Key, When = g.Min(x => x.When) })
                .ToArray();

            // даты первого правильного решения каждой задачи каждым fore-юзером 
            var foreUserName_taskId_when_s = baskUserName_taskId_when_s
                .Where(s => foreUserNames.Contains(s.UserName))
                .ToArray();

            // решения, которые укладываются во временные рамки
            var restritedSolvs = from s in foreUserName_taskId_when_s
                        join tw in taskId_when_s
                        on s.TaskId equals tw.TaskId
                        where s.When - tw.When < TimeSpan.FromDays(days)
                        select s;

            // разбить по неделям

            return Content(
                //taskId_when_s.Count() + "\n\n" +
                //JsonConvert.SerializeObject(taskId_when_s) + "\n\n" +
                restritedSolvs.Count() + "\n\n" +
                JsonConvert.SerializeObject(restritedSolvs)
                );
            
        }


        public IActionResult Visiting()
        {
            // ------- это про лекции ----------

            var lecId_title_s = from l in _db.Lectures
                                where l.TutorName == "opr"
                                where l.IsPublic
                                orderby l.Title
                                select new { LecId = l.Id, l.Title };

            var lecId_s = from li in lecId_title_s
                          select li.LecId;

            var lecId_begin_s = from r in _db.Readings
                                where lecId_s.Contains(r.LecId)
                                select new { r.LecId, r.Begin } into item
                                group item by item.LecId into g
                                select new { LecId = g.Key, Begin = g.Min(x => x.Begin) };

            var titles = (from l in lecId_title_s
                          join lb in lecId_begin_s on l.LecId equals lb.LecId
                          select l.Title)
                          .OrderBy(q => q);

            // ------- это про юзеров ----------

            // все посещения в заданное время. из них нужно выбрать по одному
            var userName_lecId_s = (
                from r in _db.Readings
                join lb in lecId_begin_s on r.LecId equals lb.LecId
                where r.Begin < lb.Begin + TimeSpan.FromDays(1000)
                select new { r.UserName, r.LecId })
                .Distinct()
                .ToArray();

            // имена студентов, посетивших хоть что-то в заданное время
            var userNames = userName_lecId_s.Select(r => r.UserName)
                .Distinct()
                .OrderBy(n => n)
                .ToArray();


            // [["1Barkalov","90"],["1Berezina","75"],...] - who has any mark
            var userName_rait_s =
                (from t in _db.Tickets
                 join e in _db.Exams on t.ExamId equals e.Id
                 where userNames.Contains(t.UserName)
                 select new { t.UserName, e.Rait } into item
                 group item by item.UserName into g
                 select new string[] { g.Key, g.Max(x => x.Rait).ToString() })
                .ToArray();

            // [["0Golda","0"],["0Roy","0"],...] - who has no mark
            var userName_0_s = userNames
                .Except(userName_rait_s.Select(m => m[0]))
                .Select(n => new string[] { n, "0" })
                .ToArray();

            // all of them
            var students = userName_rait_s
                .Union(userName_0_s)
                .OrderBy(m => m[0]);

            // ------- это про посещения ----------
            var lecIds = lecId_title_s.Select(x => x.LecId).ToArray();

            // convert every object like { UserName = "1Tukalo", LecId = 492 } to array like [1, 19] 
            // and group arrays by first element
            var groupedVisits = from v in userName_lecId_s
                         select new int[] {
                               Array.IndexOf(lecIds, v.LecId),
                               Array.IndexOf(userNames, v.UserName),
                          } into item
                         group item by item[0];

            var visits = groupedVisits
                .OrderBy(g => g.Key)
                .Select(g => g.Select(m => m[1]));

            ViewBag.Titles = JsonConvert.SerializeObject(titles);
            ViewBag.Students = JsonConvert.SerializeObject(students);
            ViewBag.Visits = JsonConvert.SerializeObject(visits);

            return View();

            //return Content(
            //    titles.Count() + "\n\n" +
            //    JsonConvert.SerializeObject(titles) + "\n\n" +
            //    students.Count() + "\n\n" +
            //    JsonConvert.SerializeObject(students) + "\n\n" +
            //    visits.Count() + "\n\n" +
            //    JsonConvert.SerializeObject(visits)
            //    );

        }



        //public IActionResult Visiting()
        //{
        //    // ------- это про лекции ----------

        //    var lecId_title_s = from l in _db.Lectures
        //                        where l.TutorName == "opr"
        //                        where l.IsPublic
        //                        orderby l.Title
        //                        select new { LecId = l.Id, l.Title };

        //    var lecId_s = from li in lecId_title_s
        //                  select li.LecId;

        //    var lecId_begin_s = from r in _db.Readings
        //                        where lecId_s.Contains(r.LecId)
        //                        select new { r.LecId, r.Begin } into item
        //                        group item by item.LecId into g
        //                        select new { LecId = g.Key, Begin = g.Min(x => x.Begin) };

        //    var titles = (from l in lecId_title_s
        //                  join lb in lecId_begin_s on l.LecId equals lb.LecId
        //                  select l.Title)
        //                  .OrderBy(q => q);

        //    // ------- это про юзеров ----------

        //    // все посещения в заданное время. из них нужно выбрать по одному
        //    var userName_lecId_s = (
        //        from r in _db.Readings
        //        join lb in lecId_begin_s on r.LecId equals lb.LecId
        //        where r.Begin < lb.Begin + TimeSpan.FromDays(1000)
        //        select new { r.UserName, r.LecId })
        //        .Distinct()                
        //        .ToArray();

        //    // имена студентов, посетивших хоть что-то в заданное время
        //    var userNames = userName_lecId_s.Select(r => r.UserName)
        //        .Distinct()
        //        .OrderBy(n => n)
        //        .ToArray();


        //    // [["1Barkalov","90"],["1Berezina","75"],...] - who has any mark
        //    var userName_rait_s =
        //        (from t in _db.Tickets
        //        join e in _db.Exams on t.ExamId equals e.Id
        //        where userNames.Contains(t.UserName)
        //        select new { t.UserName, e.Rait } into item
        //        group item by item.UserName into g
        //        select new string[] { g.Key, g.Max(x => x.Rait).ToString() })
        //        .ToArray();

        //    // [["0Golda","0"],["0Roy","0"],...] - who has no mark
        //    var userName_0_s = userNames
        //        .Except(userName_rait_s.Select(m => m[0]))
        //        .Select(n => new string[] { n, "0" })
        //        .ToArray();

        //    // all of them
        //    var students = userName_rait_s
        //        .Union(userName_0_s)
        //        .OrderBy(m => m[0]);

        //    // ------- это про посещения ----------
        //    var lecIds = lecId_title_s.Select(x => x.LecId).ToArray();

        //    var visits = (from v in userName_lecId_s
        //                  select new int[] {
        //                       Array.IndexOf(lecIds, v.LecId),
        //                      Array.IndexOf(userNames, v.UserName),                             
        //                  })
        //                  .ToArray();


        //    return Content(
        //        titles.Count() + "\n\n" +
        //        JsonConvert.SerializeObject(titles) + "\n\n" +
        //        students.Count() + "\n\n" +
        //        JsonConvert.SerializeObject(students) + "\n\n" +
        //        visits.Count() + "\n\n" +
        //        JsonConvert.SerializeObject(visits)
        //        );


        //}


        public IActionResult Visiting111()
        {
            var lecId_title_s = from l in _db.Lectures
                           where l.TutorName == "opr"
                           where l.IsPublic
                           select new{ LecId = l.Id,l.Title };

            var lecId_s = from li in lecId_title_s
                          select li.LecId;

            var lecId_begin_s = from r in _db.Readings
                    where lecId_s.Contains(r.LecId)
                    select new { r.LecId, r.Begin } into item
                    group item by item.LecId into g
                    select new {  LecId=g.Key, Begin = g.Min(x => x.Begin) };

            var v = (from l in lecId_title_s
                     join b in lecId_begin_s on l.LecId equals b.LecId
                     select l.Title).OrderBy(q=>q);



            var students_rait_s = from e in _db.Exams
                                  join t in _db.Tickets on e.Id equals t.ExamId
                                  select new { Mark = e.Rait, Students = t.UserName};

            var std = from s in students_rait_s
                      group s by s.Students into g
                      select new { Name = g.Key, Mark = (from m in g select m.Mark).Max() };

            string json = JsonConvert.SerializeObject(std);
            return Content(json);
             




           // var people = _db.Readings.ToList().Distinct(new ReadingComparer()).OrderBy(o => o.LecId);

      //      var AllVisits = (from lecid in _db.Lectures
      //                             join r in people on lecid.Id equals r.LecId
      //                             where lecid.TutorName == "opr"
      //                             group r by r.LecId into res
      //                             select new LectureVisit{ Idlec = res.Key, Title = (from t in _db.Lectures where t.Id == res.Key select t.Title).First(),
      //                             Count = res.Distinct().Count(), Students = (from s in people where s.LecId == res.Key select s.UserName).OrderBy(s => s).ToList()}).OrderBy(t => t.Title);

      //      var PeopleName = from p in _db.Readings.ToList().Distinct(new ReadingComparerName()).OrderBy(p=>p.UserName)
      //                         select p.UserName;

      //      var ListLectures = from l in _db.Lectures
      //                         where l.TutorName == "opr"
      //                         orderby l.Title
      //                         select l.Title;
            
      //      ViewBag.Lectures = ListLectures;
      //      ViewBag.All_visit = AllVisits;
      //      // ViewBag.SecondHalf = visitsSecondHalf;
      //      ViewBag.StudentsName = PeopleName;
      //      return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


    }
}
