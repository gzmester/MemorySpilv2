using System;
using System.Globalization;


namespace MemorySpil.Model
{
    public class GameStats
    {
        public string PlayerName { get; }
        public int Moves { get; }
        public TimeSpan GameTime { get; }
        public DateTime CompletedAt { get; }

        public GameStats(string playerName, int moves, TimeSpan gameTime, DateTime completedAt)
        {
            if (string.IsNullOrWhiteSpace(playerName))
                throw new ArgumentException("PlayerName is required.", nameof(playerName));
            if (moves < 0)
                throw new ArgumentOutOfRangeException(nameof(moves));
            if (gameTime < TimeSpan.Zero)
                throw new ArgumentOutOfRangeException(nameof(gameTime));

            PlayerName = playerName.Trim();
            Moves = moves;
            GameTime = gameTime;
            CompletedAt = completedAt;
        }

        public string ToCsv()
        {
            return string.Join(",",
                PlayerName,
                Moves.ToString(),
                GameTime.ToString(@"hh\:mm\:ss"),
                CompletedAt.ToString("yyyy-MM-dd HH:mm:ss"));
        }

        public static bool TryParseCsv(string line, out GameStats? stats)
        {
            stats = null;
            if (string.IsNullOrWhiteSpace(line)) return false;
            if (line.StartsWith("PlayerName", StringComparison.OrdinalIgnoreCase)) return false;

            var parts = line.Split(',', 4);
            if (parts.Length != 4) return false;

            var name = parts[0].Trim();

            if (!int.TryParse(parts[1].Trim(), out var moves))
                return false;

            // Tid: accepter både "hh:mm:ss" og standard "c"
            if (!TimeSpan.TryParseExact(parts[2].Trim(),
                                        new[] { @"hh\:mm\:ss", "c" },
                                        null,
                                        out var gameTime))
                return false;

            // Dato: præcis "yyyy-MM-dd HH:mm:ss"
            if (!DateTime.TryParseExact(parts[3].Trim(),
                                        "yyyy-MM-dd HH:mm:ss",
                                        null,
                                        DateTimeStyles.None,
                                        out var completedAt))
                return false;

            stats = new GameStats(name, moves, gameTime, completedAt);
            return true;
        }
    }
}