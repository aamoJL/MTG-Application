using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MTGApplication.General.Models;
using System;
using System.Collections.ObjectModel;

namespace MTGApplication.Features.DeckTesting.ViewModels;
public partial class DeckTestingPageViewModel : ObservableObject
{
  [ObservableProperty] private int playerHP = 40;
  [ObservableProperty] private int enemyHP = 40;
  [ObservableProperty] private int turnCount = 0;

  public ObservableCollection<MTGCard> Library { get; } = [];
  public ObservableCollection<MTGCard> Graveyard { get; } = [];
  public ObservableCollection<MTGCard> Exile { get; } = [];
  public ObservableCollection<MTGCard> Hand { get; } = [];
  public ObservableCollection<MTGCard> CommandZone { get; } = [];
  public ObservableCollection<MTGCard> Tokens { get; } = [];

  public event Action NewGameStarted;
  public event Action NewTurnStarted;

  [RelayCommand]
  private void NewGame()
  {

  }

  [RelayCommand]
  private void Draw()
  {

  }

  [RelayCommand]
  private void NewTurn()
  {

  }

  [RelayCommand]
  private void Shuffle()
  {

  }
}

