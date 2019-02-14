using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using MathNet.Numerics.Statistics;
using Microsoft.AspNetCore.Http;
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

        public IActionResult Solving(StatSolvingViewModel model)
        {
            if (model.Hours == 0) model.Hours = 8760; // 365 days
            if (model.BackSample == null) model.BackSample = "^[1-9ab]";
            if (model.ForeSample == null) model.ForeSample = "^1";

            // имена пользователей по шаблону backSample
            Regex regex = new Regex(model.BackSample);
            var backUserNames = _db.Users
                .Select(u => u.UserName)
                .Where(n => regex.IsMatch(n))
                .ToArray();

            // имена пользователей по шаблону foreSample (foreSample - подшаблон backSample)
            regex = new Regex(model.ForeSample);
            var foreUserNames = backUserNames
                .Where(n => regex.IsMatch(n))
                .ToArray();

            // даты самого раннего правильного решения каждой задачи каждым back-юзером 
            var baskUserName_taskId_when_s = _db.Solvings
                .Where(s => s.Success == 2 && backUserNames.Contains(s.UserName))
                .GroupBy(s => new { s.TaskId, s.UserName })
                .Select(g => new { g.Key.TaskId, g.Key.UserName, When = g.Min(x => x.When) })
                .ToArray();

            // даты самого раннего правильного решения каждой задачи
            var taskId_when_s = baskUserName_taskId_when_s
                .GroupBy(s => s.TaskId)
                .Select(g => new { TaskId = g.Key, When = g.Min(x => x.When) })
                .ToArray();

            // даты самого раннего правильного решения каждой задачи каждым fore-юзером 
            var foreUserName_taskId_when_s = baskUserName_taskId_when_s
                .Where(s => foreUserNames.Contains(s.UserName));

            // те из решений, которые укладываются во временные рамки 
            var restrictedWhens = from s in foreUserName_taskId_when_s
                                 join tw in taskId_when_s
                                 on s.TaskId equals tw.TaskId
                                 where (s.When - tw.When).TotalHours < model.Hours
                                 select s.When;

            if (restrictedWhens.Count() == 0)
                return Content("Решений нет.");

            // разбить по неделям
            model.SolvByWeeks = new List<int>();
            var dt = TimeSpan.FromDays(7);
            for (var t = restrictedWhens.Min() + dt; t < restrictedWhens.Max() + dt; t += dt )
            {
                model.SolvByWeeks.Add(restrictedWhens.Count(w => w < t));
            }

            return View(model);
        }

        public IActionResult Visiting()
        {
            ViewBag.Period = 365;
            ViewBag.Sample = "^[1-9ab]";
            ViewBag.Data = GetVisitingData(ViewBag.Period, ViewBag.Sample);
            return View();
        }

        [HttpPost]
        public IActionResult Visiting(int period = 365, string sample = "")
        {
            if (sample == null)
                sample = "";
            ViewBag.Data = GetVisitingData(period, sample);
            ViewBag.Period = period;
            ViewBag.Sample = sample;
            return View();
        }

        [HttpPost]
        public IActionResult Data(IFormFile file)
        {
            if (file == null)
                return Content("error with file");

            // Clear old records
            var marksToDelete = _db.Marks.Where(m => m.Owner == "opr");    //todo: User.Identity.Name
            _db.Marks.RemoveRange(marksToDelete);
            _db.SaveChanges();

            // Add new records
            using (TextReader reader = new StreamReader(file.OpenReadStream()))
            {
                var head = reader.ReadLine();
                var heads = head.Split("\t", StringSplitOptions.None);
                while (true)
                {
                    var line = reader.ReadLine();
                    if (line == null) break;
                    var lines = line.Split("\t", StringSplitOptions.None);
                    var mark = new Mark(heads, lines, "opr");      //todo: User.Identity.Name
                    _db.Marks.Add(mark);
                }

            }
            _db.SaveChanges();
            return RedirectToAction("Marks");
        }

        public IActionResult Marks()
        {
            var users_by_tutors = from u in _db.Marks
                                  where u.Owner == "opr"           //todo: User.Identity.Name
                                  group u by u.TutorName into g
                                  select new { g.Key, Mark = g.Select(x => Convert.ToDouble((x.Cp1 + x.Cp2 + x.Lab) / 3 + x.Bonus)) };


            Dictionary<string, double> average = new Dictionary<string, double>();

            foreach (var u in users_by_tutors)
            {
                var stat = new DescriptiveStatistics(u.Mark);
                var mean = stat.Mean;
                average.Add(u.Key, mean);

            }

            ViewBag.UsersMark = average;
            return View();
        }



        string GetVisitingData(int period, string sample)
        {
            // ------- это про лекции ----------

            var lecId_title_s = from l in _db.Lectures
                                where l.TutorName == "opr"    ////////////////
                                where l.IsPublic
                                orderby l.Title
                                select new { LecId = l.Id, l.Title };

            var lecId_s = from li in lecId_title_s
                          select li.LecId;

            var lecId_begin_userName_s = (from r in _db.Readings
                                          where lecId_s.Contains(r.LecId)
                                          select new { r.LecId, r.Begin, r.UserName })
                                  .ToArray();

            Regex regex = new Regex(sample);

            var lecId_begin_s1 = from r in lecId_begin_userName_s
                                 where regex.IsMatch(r.UserName)
                                 select new { r.LecId, r.Begin };



            var lecId_begin_s = from r in lecId_begin_s1
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
                where r.Begin < lb.Begin + TimeSpan.FromDays(period)
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

            return JsonConvert.SerializeObject(new { titles, students, visits });
        }


        //public IActionResult Privacy()
        //{
        //    return View();
        //}

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


    }
}
