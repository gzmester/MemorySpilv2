﻿using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MemorySpil;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void PlayerNameTextBox_TextChanged(object sender, TextChangedEventArgs e)
    {

    }

    private void NewGameBtn_Click(object sender, RoutedEventArgs e)
    {
        // TODO: Add logic to start a new game
    }
}