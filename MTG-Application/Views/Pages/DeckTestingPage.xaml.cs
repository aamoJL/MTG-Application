using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using MTGApplication.Models;
using System;
using System.Collections.ObjectModel;

namespace MTGApplication.Views.Pages;

public sealed partial class DeckTestingPage : Page
{
  public DeckTestingPage() => InitializeComponent();

  public MTGCard[] DeckCards
  {
    get => (MTGCard[])GetValue(DeckCardsProperty);
    set
    {
      SetValue(DeckCardsProperty, value);
      NewGame();
    }
  }

  public static readonly DependencyProperty DeckCardsProperty =
      DependencyProperty.Register(nameof(DeckCards), typeof(MTGCard[]), typeof(DeckTestingPage), new PropertyMetadata(Array.Empty<MTGCard>()));

  private ObservableCollection<MTGCard> Library { get; set; } = new();
  private ObservableCollection<MTGCard> Graveyard { get; set; } = new();
  private ObservableCollection<MTGCard> Exile { get; set; } = new();
  private ObservableCollection<MTGCard> Hand { get; set; } = new();

  public void NewGame()
  {
    Library.Clear();
    Graveyard.Clear();
    Exile.Clear();
    Hand.Clear();

    foreach (var item in DeckCards)
    {
      Library.Add(item);
    }
  }
}
