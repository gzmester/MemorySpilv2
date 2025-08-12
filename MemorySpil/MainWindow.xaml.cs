using System.Windows;
using MemorySpil.ViewModels;

namespace MemorySpil
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            // Kobl GameViewModel til XAML
            DataContext = new GameViewModel();
        }
    }
}