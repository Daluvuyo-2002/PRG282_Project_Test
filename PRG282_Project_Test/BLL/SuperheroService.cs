using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PRG282_Project_Test.DAL;
using PRG282_Project_Test.Models;

namespace PRG282_Project_Test.BLL
{
    public class SuperheroService
    {
        private readonly SuperheroRepository _repo = new SuperheroRepository();

        public List<Superhero> GetAllHeroes() => _repo.LoadAll();

        public void AddHero(Superhero hero)
        {
            var heroes = _repo.LoadAll();
            if (heroes.Any(h => h.HeroID.Equals(hero.HeroID, StringComparison.OrdinalIgnoreCase)))
                throw new Exception("A hero with that ID already exists.");
            _repo.Append(hero);
        }

        public void UpdateHero(Superhero updatedHero)
        {
            var list = _repo.LoadAll();
            var hero = list.FirstOrDefault(h => h.HeroID.Equals(updatedHero.HeroID, StringComparison.OrdinalIgnoreCase));
            if (hero == null) throw new Exception("Hero not found.");

            hero.Name = updatedHero.Name;
            hero.Age = updatedHero.Age;
            hero.Superpower = updatedHero.Superpower;
            hero.ExamScore = updatedHero.ExamScore;
            hero.CalculateRankAndThreat();

            _repo.SaveAll(list);
        }

        public void DeleteHero(string heroId)
        {
            var list = _repo.LoadAll();
            var toRemove = list.FirstOrDefault(h => h.HeroID.Equals(heroId, StringComparison.OrdinalIgnoreCase));
            if (toRemove != null)
            {
                list.Remove(toRemove);
                _repo.SaveAll(list);
            }
        }

        public string GenerateSummary()
        {
            var list = _repo.LoadAll();
            int total = list.Count;
            double avgAge = total > 0 ? list.Average(h => h.Age) : 0;
            double avgScore = total > 0 ? list.Average(h => h.ExamScore) : 0;
            int s = list.Count(h => h.Rank == "S");
            int a = list.Count(h => h.Rank == "A");
            int b = list.Count(h => h.Rank == "B");
            int c = list.Count(h => h.Rank == "C");

            var sb = new StringBuilder();
            sb.AppendLine("One Kick Heroes Academy - Summary Report");
            sb.AppendLine("Generated: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            sb.AppendLine($"Total Heroes: {total}");
            sb.AppendLine($"Average Age: {Math.Round(avgAge, 2)}");
            sb.AppendLine($"Average Exam Score: {Math.Round(avgScore, 2)}");
            sb.AppendLine($"Rank Counts: S={s}, A={a}, B={b}, C={c}");

            _repo.SaveSummary(sb.ToString());
            return sb.ToString();
        }
    }
}
