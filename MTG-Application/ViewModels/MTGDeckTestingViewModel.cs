using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MTGApplication.General.ViewModels;
using MTGApplication.Models;
using System;
using System.Collections.ObjectModel;

namespace MTGApplication.ViewModels;

public partial class MTGDeckTestingViewModel : ViewModelBase
{
  public MTGDeckTestingViewModel(MTGCardDeck deck, DeckTestingMTGCardViewModel[] tokens = null)
  {
    CardDeck = deck.GetCopy();
    Tokens = tokens ?? Array.Empty<DeckTestingMTGCardViewModel>();
  }

  #region Properties
  [ObservableProperty] private int playerHP = 40;
  [ObservableProperty] private int enemyHP = 40;
  [ObservableProperty] private int turnCount;

  public ObservableCollection<DeckTestingMTGCardViewModel> Library { get; set; } = new();
  public ObservableCollection<DeckTestingMTGCardViewModel> Graveyard { get; set; } = new();
  public ObservableCollection<DeckTestingMTGCardViewModel> Exile { get; set; } = new();
  public ObservableCollection<DeckTestingMTGCardViewModel> Hand { get; set; } = new();
  public ObservableCollection<DeckTestingMTGCardViewModel> CommandZone { get; set; } = new();
  public DeckTestingMTGCardViewModel[] Tokens { get; set; }
  public MTGCardDeck CardDeck { get; }
  #endregion

  public event Action NewGameStarted;
  public event Action NewTurnStarted;

  #region Relay Commands
  /// <summary>
  /// Resets game state
  /// </summary>
  [RelayCommand]
  public void NewGame()
  {
    Library.Clear();
    Graveyard.Clear();
    Exile.Clear();
    Hand.Clear();
    CommandZone.Clear();

    TurnCount = 0;
    PlayerHP = 40;
    EnemyHP = 40;

    // Reset library
    foreach (var item in CardDeck.DeckCards)
    {
      for (var i = 0; i < item.Count; i++)
        Library.Add(new(item)); // Add as many of the same cards as the card's Count property
    }

    Shuffle();

    // Add commanders to the command zone
    if (CardDeck.Commander != null) { CommandZone.Add(new(CardDeck.Commander)); }
    if (CardDeck.CommanderPartner != null) { CommandZone.Add(new(CardDeck.CommanderPartner)); }

    for (var i = 0; i < 7; i++)
      Draw(); // Draw 7 cards from library to hand

    NewGameStarted?.Invoke(); // Clear battlefield
  }

  /// <summary>
  /// Draws a card from the library to the hand
  /// </summary>
  [RelayCommand]
  public void Draw()
  {
    if (Library.Count == 0) { return; }

    var card = Library[0];
    Library.RemoveAt(0);
    Hand.Add(card);
  }

  /// <summary>
  /// Untaps all battlefield cards and draws a card from the library to the hand
  /// </summary>
  [RelayCommand]
  public void NewTurn()
  {
    TurnCount++;
    NewTurnStarted?.Invoke();
    Draw();
  }

  /// <summary>
  /// Shuffles the library
  /// </summary>
  [RelayCommand]
  public void Shuffle()
  {
    var rng = new Random();
    var list = Library;
    var n = list.Count;
    while (n > 1)
    {
      n--;
      var k = rng.Next(n + 1);
      (list[n], list[k]) = (list[k], list[n]);
    }
  }
  #endregion

  /// <summary>
  /// Adds given card to the bottom of the library
  /// </summary>
  public void LibraryAddBottom(DeckTestingMTGCardViewModel card) => Library.Add(card);

  /// <summary>
  /// Adds given card to the top of the library
  /// </summary>
  public void LibraryAddTop(DeckTestingMTGCardViewModel card) => Library.Insert(0, card);
}
