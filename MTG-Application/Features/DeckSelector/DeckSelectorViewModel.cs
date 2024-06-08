using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MTGApplication.Features.DeckSelector.Models;
using MTGApplication.General.Models.Card;
using MTGApplication.General.Services.API.CardAPI;
using MTGApplication.General.Services.Databases.Repositories;
using MTGApplication.General.Services.Databases.Repositories.DeckRepository.Models;
using MTGApplication.General.ViewModels;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace MTGApplication.Features.DeckSelector;
public partial class DeckSelectorViewModel : ViewModelBase, IWorker
{
  public DeckSelectorViewModel(IRepository<MTGCardDeckDTO> repository, ICardAPI<DeckEditorMTGCard> cardAPI)
  {
    Repository = repository;
    CardAPI = cardAPI;
  }

  public ObservableCollection<DeckSelectionListItem> DeckItems { get; } = new();
  public IRepository<MTGCardDeckDTO> Repository { get; }
  public ICardAPI<DeckEditorMTGCard> CardAPI { get; }

  [ObservableProperty] private bool isBusy;

  [RelayCommand]
  private async Task LoadDecks()
  {
    var deckNameImageTuples = await ((IWorker)this).DoWork(new GetDeckSelectorListItems(Repository, CardAPI).Execute());

    DeckItems.Clear();

    foreach (var (Name, ImageUri) in deckNameImageTuples) 
      DeckItems.Add(new(Name, ImageUri));
  }
}
