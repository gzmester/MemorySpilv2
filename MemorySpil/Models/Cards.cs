using System;
using System.Collections.Generic;
using System.Linq;


namespace MemorySpil.Model
{
    public class Cards
    {
        public int Id { get; set; }
        public string Symbol { get; set; } = string.Empty;
        public bool IsFlipped { get; set; }
        public bool IsMatched { get; set; }

        // Constructor 
        public Cards(int id, string symbol)
        {
            Id = id;
            Symbol = symbol;
            IsFlipped = false;
            IsMatched = false;
        }

        // Parameterløs constructor for serialisering/deserialisering
        public Cards() { }

        // Metode til at generere et sæt kort til et 4x4 memory spil
        public static List<Cards> GenerateCards()
        {
            // Symboler til 8 par 16 kort 4x4
            string[] symbols = { "🐶", "🐱", "🐭", "🐹", "🐰", "🦊", "🐻", "🐼" };
            
            // Liste til at holde kortene
            var cards = new List<Cards>();
            int cardId = 1; // start fra 1, og ++ når de filøjes, så de får hver deres ID

            //Vi skal bruge 2 kort for hvert symbol, altså et par. 
            foreach (string symbol in symbols)
            {
                cards.Add(new Cards(cardId++, symbol));
                cards.Add(new Cards(cardId++, symbol));
            }

            
            return ShuffleCards(cards);
        }

        // Randomiserer kortene
        private static List<Cards> ShuffleCards(List<Cards> cards)
        {
            var random = new Random();
            return cards.OrderBy(x => random.Next()).ToList();
        }

        // Metode til at vende et kort
        public void Flip()
        {
            if (!IsMatched)
            {
                IsFlipped = !IsFlipped;
            }
        }

        // Metode til at markere kort som matchet
        public void SetMatched()
        {
            IsMatched = true;
            IsFlipped = true; // Matchede kort forbliver vendte, så spilleren kan se dem
        }

        // Metode til at nulstille kort
        public void Reset()
        {
            if (!IsMatched)
            {
                IsFlipped = false;
            }
        }

        // Debug
        public override string ToString()
        {
            string status = IsMatched ? "Matched" : (IsFlipped ? "Flipped" : "Hidden");
            return $"Card {Id}: {Symbol} ({status})";
        }
    }
}
