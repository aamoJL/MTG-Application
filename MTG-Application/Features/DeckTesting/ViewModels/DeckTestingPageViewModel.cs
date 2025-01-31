using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MTGApplication.Features.DeckTesting.Models;
using MTGApplication.Features.DeckTesting.UseCases;
using MTGApplication.General.Models;
using MTGApplication.General.Services.Importers.CardImporter;
using MTGApplication.General.Services.Importers.CardImporter.UseCases;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace MTGApplication.Features.DeckTesting.ViewModels;
public partial class DeckTestingPageViewModel : ObservableObject
{
  public DeckTestingPageViewModel(DeckTestingDeck deck, IMTGCardImporter importer)
  {
    Deck = deck;
    Importer = importer;

    _ = UpdateTokens();
  }

  public DeckTestingDeck Deck { get; }
  public IMTGCardImporter Importer { get; }

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

  [NotNull] public IRelayCommand? StartNewGameCommand => field ??= new StartNewGame(this).Command;
  [NotNull] public IRelayCommand? DrawCardCommand => field ??= new DrawCard(this).Command;
  [NotNull] public IRelayCommand? StartNewTurnCommand => field ??= new StartNewTurn(this).Command;
  [NotNull] public IRelayCommand? ShuffleDeckCommand => field ??= new ShuffleDeck(this).Command;

  public void RaiseNewGameStarted() => NewGameStarted?.Invoke();

  public void RaiseNewTurnStarted() => NewTurnStarted?.Invoke();

  /// <exception cref="InvalidOperationException"></exception>
  /// <exception cref="System.Net.Http.HttpRequestException"></exception>
  /// <exception cref="UriFormatException"></exception>
  private async Task UpdateTokens()
  {
    var cards = new List<MTGCard?>(
      [.. Deck.DeckCards,
      Deck.Commander,
      Deck.Partner]).OfType<MTGCard>();

    try
    {
      var tokens = (await new FetchTokenCards(Importer).Execute(cards)).Found
        .Select(x => new DeckTestingMTGCard(x.Info));

      foreach (var item in tokens)
        Tokens.Add(item);
    }
    catch { }
  }
}

