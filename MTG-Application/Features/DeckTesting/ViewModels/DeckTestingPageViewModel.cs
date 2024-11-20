using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MTGApplication.Features.DeckTesting.Models;
using MTGApplication.Features.DeckTesting.UseCases;
using System;
using System.Collections.ObjectModel;

namespace MTGApplication.Features.DeckTesting.ViewModels;
public partial class DeckTestingPageViewModel(DeckTestingDeck deck) : ObservableObject
{
  public DeckTestingDeck Deck { get; } = deck;

  public ObservableCollection<DeckTestingMTGCard> Library { get; } = [];
  public ObservableCollection<DeckTestingMTGCard> Graveyard { get; } = [];
  public ObservableCollection<DeckTestingMTGCard> Exile { get; } = [];
  public ObservableCollection<DeckTestingMTGCard> Hand { get; } = [];
  public ObservableCollection<DeckTestingMTGCard> CommandZone { get; } = [];
  public ObservableCollection<DeckTestingMTGCard> Tokens { get; } = [];

  public int PlayerHP { get; set { field = value; OnPropertyChanged(); } } = 40;
  public int EnemyHP { get; set { field = value; OnPropertyChanged(); } } = 40;
  public int TurnCount { get; set { field = value; OnPropertyChanged(); } } = 0;

  public event Action NewGameStarted;
  public event Action NewTurnStarted;

  public IRelayCommand StartNewGameCommand => field ??= new StartNewGame(this).Command;
  public IRelayCommand DrawCardCommand => field ??= new DrawCard(this).Command;
  public IRelayCommand StartNewTurnCommand => field ??= new StartNewTurn(this).Command;
  public IRelayCommand ShuffleDeckCommand => field ??= new ShuffleDeck(this).Command;

  public void RaiseNewGameStarted() => NewGameStarted?.Invoke();

  public void RaiseNewTurnStarted() => NewTurnStarted?.Invoke();
}

