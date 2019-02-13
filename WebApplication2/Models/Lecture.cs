using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.RegularExpressions;


namespace Tut20.Models
{ 
    [Table("Lecture")]
    public class Lecture
    {
        public int Id { get; set; }
        public string TutorName { get; set; }
        public string Title { get; set; }
        public bool IsPublic { get; set; }
        [Required]
        public string Content { get; set; }

        public Lecture(string content, string tutor)
        {
            Content = content;
            TutorName = tutor;
            Title = TitleFromContent();
        }

        public Lecture() { }

        public string TitleFromContent()
        {
            Regex reg = new Regex(@"@1\s*(.*)");
            Match match = reg.Match(Content);

            string title = "_without_name_";

            if (match != null && !string.IsNullOrEmpty(match.Groups[1].Value))
            {
                title = match.Groups[1].Value.Trim();
                // get rid off the invalid characters in the title
                var invalid = System.IO.Path.GetInvalidFileNameChars();
                title = new string(title.Where(c => !invalid.Contains(c)).ToArray());
            }
            return title;
        }

        //public List<TssTask> EmbeddedTasks()
        //{
        //    Regex reg = new Regex(@"^@6(.*)\|\s*(\d*)", RegexOptions.Multiline);
        //    var matches = reg.Matches(Content).Cast<Match>();

        //    return matches.Select(m => new TssTask
        //    {
        //        Id = Convert.ToInt32(m.Groups[2].Value),
        //        Title = m.Groups[1].Value
        //    }).ToList();
        //}

        /// <summary>
        /// Find list of picture names (with extentions .png, .jpg)
        /// </summary>
        public List<string> EmbeddedPictureNames()
        {
            Regex reg = new Regex(@"\[\[(.*)\.(png|jpg)\]\]");
            var matches = reg.Matches(Content).Cast<Match>();
            return matches.Select(m =>m.Groups[1].Value + "." + m.Groups[2].Value).ToList();
        }

        [NotMapped]
        public int SelectionStart { get; set; }
    }
}