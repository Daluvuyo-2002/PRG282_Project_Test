using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRG282_Project_Test.Models
{
    public class Superhero
    {
        public string HeroID { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public string Superpower { get; set; }
        public int ExamScore { get; set; }
        public string Rank { get; set; }
        public string ThreatLevel { get; set; }

        public Superhero() { }

        public Superhero(string heroID, string name, int age, string superpower, int examScore)
        {
            HeroID = heroID;
            Name = name;
            Age = age;
            Superpower = superpower;
            ExamScore = examScore;
            CalculateRankAndThreat();
        }

        public void CalculateRankAndThreat()
        {
            if (ExamScore >= 81 && ExamScore <= 100)
            {
                Rank = "S";
                ThreatLevel = "Finals Week (threat to the entire academy)";
            }
            else if (ExamScore >= 61)
            {
                Rank = "A";
                ThreatLevel = "Midterm Madness (threat to a department)";
            }
            else if (ExamScore >= 41)
            {
                Rank = "B";
                ThreatLevel = "Group Project Gone Wrong (threat to a study group)";
            }
            else
            {
                Rank = "C";
                ThreatLevel = "Pop Quiz (potential threat to an individual student)";
            }
        }

        // Save as pipe-separated record, escape pipes in text fields
        public string ToRecord()
        {
            string esc(string s) => s?.Replace("|", "\\|") ?? "";
            return $"{esc(HeroID)}|{esc(Name)}|{Age}|{esc(Superpower)}|{ExamScore}|{Rank}|{esc(ThreatLevel)}";
        }

        public static Superhero FromRecord(string record)
        {
            if (string.IsNullOrWhiteSpace(record)) return null;
            // split on '|' but take into account escaped pipes
            var parts = new List<string>();
            var sb = new StringBuilder();
            bool escape = false;
            foreach (char c in record)
            {
                if (escape)
                {
                    sb.Append(c);
                    escape = false;
                }
                else
                {
                    if (c == '\\') { escape = true; }
                    else if (c == '|') { parts.Add(sb.ToString()); sb.Clear(); }
                    else sb.Append(c);
                }
            }
            parts.Add(sb.ToString());

            if (parts.Count < 7) return null;
            try
            {
                var sh = new Superhero
                {
                    HeroID = parts[0],
                    Name = parts[1],
                    Age = int.Parse(parts[2]),
                    Superpower = parts[3],
                    ExamScore = int.Parse(parts[4]),
                    Rank = parts[5],
                    ThreatLevel = parts[6]
                };
                return sh;
            }
            catch
            {
                return null;
            }
        }
    }
}
