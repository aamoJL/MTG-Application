using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MTGApplication.Features.DeckTesting.Models;
using MTGApplication.General.Models;
using MTGApplication.General.Services.Importers.CardImporter;
using MTGApplication.General.Services.Importers.CardImporter.UseCases;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace MTGApplication.Features.DeckTesting.ViewModels;

public partial class DeckTestingPageViewModel : ObservableObject
{
  public DeckTestingPageViewModel(DeckTestingDeck deck)
  {
    Deck = deck;

    Library.CollectionChanged += Library_CollectionChanged;
  }

  public DeckTestingDeck Deck { get; }
  public IMTGCardImporter Importer { get; init; } = App.MTGCardImporter;

  public ObservableCollection<DeckTestingMTGCard> Library { get; } = [];
  public ObservableCollection<DeckTestingMTGCard> Graveyard { get; } = [];
  public ObservableCollection<DeckTestingMTGCard> Exile { get; } = [];
  public ObservableCollection<DeckTestingMTGCard> Hand { get; } = [];
  public ObservableCollection<DeckTestingMTGCard> CommandZone { get; } = [];
  public ObservableCollection<DeckTestingMTGCard> Tokens { get; } = [];

  [ObservableProperty] public partial int PlayerHP { get; set; } = 40;
  [ObservableProperty] public partial int EnemyHP { get; set; } = 40;
  [ObservableProperty] public partial int TurnCount { get; set; } = 0;

  public event Action? NewGameStarted;
  public event Action? NewTurnStarted;

  [RelayCommand]
  private void StartNewGame()
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
    foreach (var item in Deck.DeckCards)
      Library.Add(new(item.Info));

    ShuffleDeckCommand?.Execute(null);

    // Add commanders to the command zone
    if (Deck.Commander != null)
      CommandZone.Add(new(Deck.Commander.Info));

    if (Deck.Partner != null)
      CommandZone.Add(new(Deck.Partner.Info));

    // Draw 7 cards from library to hand
    for (var i = 0; i < 7; i++)
      DrawCardCommand?.Execute(null);

    RaiseNewGameStarted();
  }

  [RelayCommand(CanExecute = nameof(CanDrawCard))]
  private void DrawCard()
  {
    if (!CanDrawCard()) return;

    var card = Library[0];
    Library.RemoveAt(0);
    Hand.Add(card);
  }

  [RelayCommand]
  private void StartNewTurn()
  {
    TurnCount++;
    DrawCard();
    RaiseNewTurnStarted();
  }

  [RelayCommand]
  private void ShuffleDeck()
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

  [RelayCommand]
  private async Task RefreshTokens()
  {
    var cards = new List<MTGCard?>(
      [.. Deck.DeckCards,
      Deck.Commander,
      Deck.Partner]).OfType<MTGCard>();

    try
    {
      var tokens = (await new FetchTokenCards(Importer).Execute(cards)).Found
        .Select(x => new DeckTestingMTGCard(x.Info) { IsToken = true });

      foreach (var item in tokens)
        Tokens.Add(item);
    }
    catch { }
  }

  public void RaiseNewGameStarted() => NewGameStarted?.Invoke();

  public void RaiseNewTurnStarted() => NewTurnStarted?.Invoke();

  private bool CanDrawCard() => Library.Count > 0;

  private void Library_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    => DrawCardCommand.NotifyCanExecuteChanged();
}