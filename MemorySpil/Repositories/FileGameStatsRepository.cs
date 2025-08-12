using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MemorySpil.Model;

namespace MemorySpil.Repository
{
    public class FileGameStatsRepository : IGameStatsRepository
    {
        private readonly string _path;
        private const string Header = "PlayerName,Moves,GameTime,CompletedAt";

        public FileGameStatsRepository(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentException("CSV path is required.", nameof(path));

            _path = path;
            EnsureFile();
        }

        private void EnsureFile()
        {
            var dir = Path.GetDirectoryName(_path);
            if (!string.IsNullOrWhiteSpace(dir) && !Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            if (!File.Exists(_path))
            {
                using var sw = new StreamWriter(_path, append: false);
                sw.WriteLine(Header);
            }
        }

        public void SaveGame(GameStats stats)
        {
            if (stats == null) throw new ArgumentNullException(nameof(stats));
            using var sw = new StreamWriter(_path, append: true);
            sw.WriteLine(stats.ToCsv());
        }

        public List<GameStats> GetTop10Scores()
        {
            return ReadAll()
                .OrderBy(s => s.Moves)
                .ThenBy(s => s.GameTime)
                .ThenBy(s => s.CompletedAt)
                .Take(10)
                .ToList();
        }

        public List<GameStats> GetGamesByPlayer(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return new List<GameStats>();

            return ReadAll()
                .Where(s => string.Equals(s.PlayerName, name, StringComparison.OrdinalIgnoreCase))
                .OrderBy(s => s.Moves)
                .ThenBy(s => s.GameTime)
                .ThenBy(s => s.CompletedAt)
                .ToList();
        }

        private List<GameStats> ReadAll()
        {
            var results = new List<GameStats>();
            if (!File.Exists(_path)) return results;

            foreach (var line in File.ReadLines(_path))
                if (GameStats.TryParseCsv(line, out var s) && s != null)
                    results.Add(s);

            return results;
        }
    }
}