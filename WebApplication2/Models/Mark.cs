using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tut20.Models
{
    [Table("Mark")]
    public class Mark
    {

        public Mark(string[] heads, string[] lines, string owner)
        {
            Owner = owner;
            for (var i = 0; i < heads.Length; i++)
            {
                if (heads[i] == "UserName")
                {
                    UserName = lines[i];
                }
                else if (heads[i] == "Cp1")
                {
                    if (!string.IsNullOrEmpty(lines[i]))
                        Cp1 = Convert.ToSingle(lines[i]);
                }
                else if (heads[i] == "Cp2")
                {
                    if (!string.IsNullOrEmpty(lines[i]))
                        Cp2 = Convert.ToSingle(lines[i]);
                }
                else if (heads[i] == "Prefix")
                {
                    Prefix = lines[i];
                }
                else if (heads[i] == "TutorName")
                {
                    TutorName = lines[i];
                }
                else if (heads[i] == "Bonus")
                {
                    if (!string.IsNullOrEmpty(lines[i]))
                        Bonus = Convert.ToSingle(lines[i]);
                }
                else if (heads[i] == "Col1")
                {
                    if (!string.IsNullOrEmpty(lines[i]))
                        Col1 = Convert.ToSingle(lines[i]);
                }
                else if (heads[i] == "Col2")
                {
                    if (!string.IsNullOrEmpty(lines[i]))
                        Col2 = Convert.ToSingle(lines[i]);
                }
                else if (heads[i] == "Lab")
                {
                    if (!string.IsNullOrEmpty(lines[i]))
                        Lab = Convert.ToSingle(lines[i]);
                }
            }
        }

        public Mark()
        {
        }

        public int Id { get; set; }
        public string UserName { get; set; }
        public string Prefix { get; set; }
        public float Cp1 { get; set; }
        public float Cp2 { get; set; }
        public float Bonus { get; set; }
        public float Lab { get; set; }
        public string TutorName { get; set; }
        public float Col1 { get; set; }
        public float Col2 { get; set; }
        public string Comment { get; set; }
        public string Owner { get; set; }
    }
}
