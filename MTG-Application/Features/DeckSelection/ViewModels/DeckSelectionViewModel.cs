using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MTGApplication.Features.DeckSelector.Models;
using MTGApplication.Features.DeckSelector.UseCases;
using MTGApplication.General.Services.API.CardAPI;
using MTGApplication.General.Services.Databases.Repositories;
using MTGApplication.General.Services.Databases.Repositories.DeckRepository.Models;
using MTGApplication.General.ViewModels;
using System.Collections.ObjectModel;

namespace MTGApplication.Features.DeckSelector;
public partial class DeckSelectionViewModel(IRepository<MTGCardDeckDTO> repository, MTGCardImporter importer) : ViewModelBase, IWorker
{
  public ObservableCollection<DeckSelectionDeck> DeckItems { get; } = [];
  public IRepository<MTGCardDeckDTO> Repository { get; } = repository;
  public MTGCardImporter Importer { get; } = importer;
  public IWorker Worker => this;

  [ObservableProperty] private bool isBusy;

  public IAsyncRelayCommand LoadDecksCommand => new DeckSelectorViewModelCommands.LoadDecks(this).Command;

}
