using System;


namespace MemorySpilV2.Model
{
    public class Cards
    {
        public string PlayerName { get; set; }
        public int Moves { get; set; }
        public TimeSpan GameTime { get; set; }
        public DateTime CompletedAt { get; set; }

    }
}