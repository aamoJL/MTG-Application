using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MTGApplication.Models;
using System;
using System.Collections.ObjectModel;

namespace MTGApplication.ViewModels;

public partial class MTGDeckTestingViewModel : ViewModelBase
{
  // TODO: commander out of the library

  public MTGDeckTestingViewModel() { }
  public MTGDeckTestingViewModel(MTGCard[] deckCards) => DeckCards = deckCards;

  private MTGCard[] deckCards;
  public ObservableCollection<MTGCardViewModel> Library { get; set; } = new();
  public ObservableCollection<MTGCardViewModel> Graveyard { get; set; } = new();
  public ObservableCollection<MTGCardViewModel> Exile { get; set; } = new();
  public ObservableCollection<MTGCardViewModel> Hand { get; set; } = new();

  public event Action NewGameStarted;

  [ObservableProperty]
  private int playerHP = 40;
  [ObservableProperty]
  private int enemyHP = 40;

  public MTGCard[] DeckCards
  {
    get => deckCards;
    set
    {
      deckCards = value;
      NewGame();
    }
  }

  [RelayCommand]
  public void NewGame()
  {
    Library.Clear();
    Graveyard.Clear();
    Exile.Clear();
    Hand.Clear();
    
    PlayerHP = 40;
    EnemyHP = 40;

    // Reset library
    foreach (var item in DeckCards)
    {
      for (var i = 0; i < item.Count; i++)
      {
        Library.Add(new(item)); // Add as many of the same card as the cards Count property
      }
    }

    Shuffle();

    for (var i = 0; i < 7; i++)
    {
      Draw(); // Draw 7 cards from library to hand
    }

    NewGameStarted?.Invoke(); // Clear battlefield
  }

  [RelayCommand]
  public void Draw()
  {
    if (Library.Count == 0) { return; }

    var card = Library[0];
    Library.RemoveAt(0);
    Hand.Add(card);
  }

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

  public void LibraryAddBottom(MTGCardViewModel card) => Library.Add(card);

  public void LibraryAddTop(MTGCardViewModel card) => Library.Insert(0, card);
}
