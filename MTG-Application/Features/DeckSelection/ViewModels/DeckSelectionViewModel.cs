using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MTGApplication.Features.DeckSelection.Models;
using MTGApplication.Features.DeckSelection.UseCases;
using MTGApplication.General.Services.Databases.Repositories;
using MTGApplication.General.Services.Databases.Repositories.DeckRepository.Models;
using MTGApplication.General.Services.Importers.CardImporter;
using MTGApplication.General.ViewModels;
using System.Collections.ObjectModel;

namespace MTGApplication.Features.DeckSelection;
public partial class DeckSelectionViewModel(IRepository<MTGCardDeckDTO> repository, MTGCardImporter importer) : ViewModelBase, IWorker
{
  public ObservableCollection<DeckSelectionDeck> DeckItems { get; } = [];
  public IRepository<MTGCardDeckDTO> Repository { get; } = repository;
  public MTGCardImporter Importer { get; } = importer;
  public IWorker Worker => this;

  [ObservableProperty] private bool isBusy;

  public IAsyncRelayCommand LoadDecksCommand => (loadDecks ??= (loadDecks = new DeckSelectorViewModelCommands.LoadDecks(this))).Command;

  private DeckSelectorViewModelCommands.LoadDecks loadDecks;
}
