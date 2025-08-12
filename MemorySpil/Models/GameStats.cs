using System;


namespace MemorySpil.Model
{
    public class Cards
    {
        public string PlayerName { get; set; }
        public int Moves { get; set; }
        public TimeSpan GameTime { get; set; }
        public DateTime CompletedAt { get; set; }

    }
}