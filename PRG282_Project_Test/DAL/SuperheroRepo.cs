using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using PRG282_Project_Test.Models;

namespace PRG282_Project_Test.DAL
{
    public class SuperheroRepository
    {
        private readonly string dataFile;
        private readonly string summaryFile;

        public SuperheroRepository()
        {
            dataFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "superheroes.txt");
            summaryFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "summary.txt");
            EnsureFilesExist();
        }

        private void EnsureFilesExist()
        {
            if (!File.Exists(dataFile)) File.WriteAllText(dataFile, "");
            if (!File.Exists(summaryFile)) File.WriteAllText(summaryFile, "");
        }

        public List<Superhero> LoadAll()
        {
            var list = new List<Superhero>();
            foreach (var line in File.ReadAllLines(dataFile))
            {
                var h = Superhero.FromRecord(line);
                if (h != null) list.Add(h);
            }
            return list;
        }

        public void SaveAll(List<Superhero> list)
        {
            var lines = list.Select(h => h.ToRecord()).ToArray();
            File.WriteAllLines(dataFile, lines);
        }

        public void Append(Superhero hero)
        {
            File.AppendAllLines(dataFile, new[] { hero.ToRecord() });
        }

        public void SaveSummary(string summary)
        {
            File.WriteAllText(summaryFile, summary);
        }
    }
}
