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

  [ObservableProperty] private int playerHP = 40;
  [ObservableProperty] private int enemyHP = 40;
  [ObservableProperty] private int turnCount = 0;

  public event Action NewGameStarted;
  public event Action NewTurnStarted;

  public IRelayCommand StartNewGameCommand => (startNewGame ??= new StartNewGame(this)).Command;
  public IRelayCommand DrawCardCommand => (drawCard ??= new DrawCard(this)).Command;
  public IRelayCommand StartNewTurnCommand => (startNewTurn ??= new StartNewTurn(this)).Command;
  public IRelayCommand ShuffleDeckCommand => (shuffleDeck ??= new ShuffleDeck(this)).Command;

  private StartNewGame startNewGame;
  private DrawCard drawCard;
  private StartNewTurn startNewTurn;
  private ShuffleDeck shuffleDeck;

  public void RaiseNewGameStarted() => NewGameStarted?.Invoke();

  public void RaiseNewTurnStarted() => NewTurnStarted?.Invoke();

  // TODO: fetch tokens here so the window can open immediately
}

