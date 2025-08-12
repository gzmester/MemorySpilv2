using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using MemorySpil.Model;
using MemorySpil.Models.MVVM;
using MemorySpil.Repository;

namespace MemorySpil.ViewModels
{
    public class GameViewModel : ViewModelBase
    {
        private readonly List<Cards> _allCards;
        private readonly IGameStatsRepository _statsRepository;
        private Cards _firstSelectedCard;
        private Cards _secondSelectedCard;
        private bool _isProcessing;
        private int _moves;
        private DateTime _gameStartTime;
        private bool _gameCompleted;
        private string _playerName;

        public GameViewModel()
        {
            // Opret vores csv fil
            _statsRepository = new FileGameStatsRepository("game_stats.csv");
            
            // Opret vores kort // kalder GenerateCards inde i Cards model
            _allCards = Cards.GenerateCards();
            CardsCollection = new ObservableCollection<Cards>(_allCards);
            
            // Start spillet
            StartGame();
        }

        // Properties til binding
        public ObservableCollection<Cards> CardsCollection { get; set; }

        public string PlayerName
        {
            get => _playerName;
            set
            {
                _playerName = value;
                OnPropertyChanged(nameof(PlayerName));
                OnPropertyChanged(nameof(GameStatus));
            }
        }

        public int Moves
        {
            get => _moves;
            set
            {
                _moves = value;
                OnPropertyChanged(nameof(Moves));
                OnPropertyChanged(nameof(GameStatus));
            }
        }

        public TimeSpan GameTime
        {
            get => DateTime.Now - _gameStartTime;
        }

        public bool GameCompleted
        {
            get => _gameCompleted;
            set
            {
                _gameCompleted = value;
                OnPropertyChanged(nameof(GameCompleted));
            }
        }

        public string GameStatus
        {
            get
            {
                if (GameCompleted)
                {
                    var playerDisplay = string.IsNullOrWhiteSpace(PlayerName) ? "Du" : PlayerName;
                    return $"{playerDisplay} vandt på {Moves} træk i {GameTime:mm\\:ss}!";
                }
                
                var currentPlayer = string.IsNullOrWhiteSpace(PlayerName) ? "Spiller" : PlayerName;
                return $"{currentPlayer} - Træk: {Moves} | Tid: {GameTime:mm\\:ss}";
            }
        }

        // Kalder vores MVVM RelayCommand, for at håndtere et kort træk
        public RelayCommand CardClickCommand => new RelayCommand(execute => OnCardClick((Cards)execute));

        // Kalder vores MVVM RelayCommand, for at starte et nyt spil
        public RelayCommand NewGameCommand => new RelayCommand(execute => StartNewGame());

        // Kalder vores MVVM RelayCommand, for at gemme spil resultat
        public RelayCommand SaveGameCommand => new RelayCommand(execute => SaveGameStats());

        private void StartGame()
        {
            _gameStartTime = DateTime.Now;
            Moves = 0;
            GameCompleted = false;
            _firstSelectedCard = null;
            _secondSelectedCard = null;
            _isProcessing = false;

            // Reset alle kort
            foreach (var card in _allCards)
            {
                card.IsFlipped = false;
                card.IsMatched = false;
            }
        }

        private async void OnCardClick(Cards clickedCard)
        {
            // Tjek om kortet allerede er vendt eller matchet
            if (_isProcessing || clickedCard.IsFlipped || clickedCard.IsMatched)
                return;

            // Vend kortet
            clickedCard.Flip();
            OnPropertyChanged(nameof(Cards));

            if (_firstSelectedCard == null)
            {
                // Første kort valgt
                _firstSelectedCard = clickedCard;
            }
            else if (_secondSelectedCard == null)
            {
                // Andet kort valgt
                _secondSelectedCard = clickedCard;
                _isProcessing = true;
                Moves++;

                // Check for match efter kort delay
                // delay 1 sekund 1000 ms
                await Task.Delay(1000);
                await CheckForMatch();
            }
        }

        private async Task CheckForMatch()
        {
            if (_firstSelectedCard.Symbol == _secondSelectedCard.Symbol)
            {
                // Match fundet!
                _firstSelectedCard.SetMatched();
                _secondSelectedCard.SetMatched();
                
                // Check om spillet er færdigt
                if (_allCards.All(c => c.IsMatched))
                {
                    GameCompleted = true;
                    // Automatisk gem statistik når spillet er færdigt
                    SaveGameStats();
                }
            }
            else
            {
                // Ikke et match, vend kortene tilbage
                _firstSelectedCard.Reset();
                _secondSelectedCard.Reset();
            }

            // Set valgte kort tilbage = null
            _firstSelectedCard = null;
            _secondSelectedCard = null;
            _isProcessing = false;

            // Opdater UI
            OnPropertyChanged(nameof(Cards));
            OnPropertyChanged(nameof(GameStatus));
        }

        private void StartNewGame()
        {
            // Kalder GenerateCards for at oprette nye kort
            var newCards = Cards.GenerateCards();
            CardsCollection.Clear();
            foreach (var card in newCards)
            {
                CardsCollection.Add(card);
            }
            
            _allCards.Clear();
            _allCards.AddRange(newCards);
            
            StartGame();
            OnPropertyChanged(nameof(GameStatus));
        }

        private void SaveGameStats()
        {
            // Kun gem hvis spillet er færdigt og spillernavn er angivet
            if (!GameCompleted || string.IsNullOrWhiteSpace(PlayerName))
                return;

            try
            {
                var stats = new GameStats(
                    PlayerName.Trim(),
                    Moves,
                    GameTime,
                    DateTime.Now
                );
                // gem statistik, hvis spillet er færdigt
                _statsRepository.SaveGame(stats);
            }
            catch (Exception ex)
            {
                // try / catch til fejlhåndtering
                System.Diagnostics.Debug.WriteLine($"Der skete en fejl: {ex.Message}");
            }
        }

        //evt en metode til at hente score
        public List<GameStats> GetTopScores()
        {
            return _statsRepository.GetTop10Scores();
        }

        // Metode til at hente spil for specifik spiller
        public List<GameStats> GetPlayerGames(string playerName)
        {
            return _statsRepository.GetGamesByPlayer(playerName);
        }
    }
}
