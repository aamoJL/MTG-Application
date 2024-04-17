using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MTGApplication.Database.Repositories;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace MTGApplication.ViewModels;

public class DeckItemViewModel
{
  public DeckItemViewModel(string title, string imageUri = "")
  {
    Title = title;
    ImageUri = imageUri;
  }

  public string Title { get; set; }
  public string ImageUri { get; }
}

public partial class DeckBuilderDeckDisplayViewModel : ViewModelBase
{
  public class DeckSelectedEventArgs : EventArgs
  {
    public DeckSelectedEventArgs(string name) => Name = name ?? "";

    public string Name { get; }
  }

  public ObservableCollection<DeckItemViewModel> Decks { get; } = new();
  [ObservableProperty] private bool isBusy;

  public event EventHandler<DeckSelectedEventArgs> DeckSelected;

  public DeckBuilderDeckDisplayViewModel() { }

  [RelayCommand]
  public void SelectDeck(DeckItemViewModel deckItem) => DeckSelected?.Invoke(this, new(deckItem?.Title));
  
  public async Task Init()
  {
    IsBusy = true;

    var repository = new SQLiteMTGDeckRepository(App.MTGCardAPI, new());
    var decks = await repository.GetDecksWithCommanders();

    Decks.Clear();
    foreach (var deck in decks)
    {
      Decks.Add(new DeckItemViewModel(deck.Name, deck.Commander?.Info.FrontFace.ImageUri ?? ""));
    }

    IsBusy = false;
  }
}
