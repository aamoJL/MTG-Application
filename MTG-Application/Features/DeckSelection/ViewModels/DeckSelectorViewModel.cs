using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MTGApplication.Features.DeckSelector.Models;
using MTGApplication.Features.DeckSelector.UseCases;
using MTGApplication.General.Services.API.CardAPI;
using MTGApplication.General.Services.Databases.Repositories;
using MTGApplication.General.Services.Databases.Repositories.DeckRepository.Models;
using MTGApplication.General.ViewModels;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace MTGApplication.Features.DeckSelector;
public partial class DeckSelectionViewModel(IRepository<MTGCardDeckDTO> repository, MTGCardImporter importer) : ViewModelBase, IWorker
{
  public ObservableCollection<DeckSelectionDeck> DeckItems { get; } = [];

  [ObservableProperty] private bool isBusy;

  [RelayCommand] private async Task LoadDecks() => await new LoadDeckSelectionDecks(repository, importer) { Worker = this }.Execute(DeckItems);
}
