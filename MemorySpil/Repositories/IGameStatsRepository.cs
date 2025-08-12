using System;
using MemorySpil.Model;

public interface IGameStatsRepository
    {
        public void SaveGame(GameStats stats);
        List<GameStats> GetTop10Scores();
        List<GameStats> GetGamesByPlayer(string name);
    }