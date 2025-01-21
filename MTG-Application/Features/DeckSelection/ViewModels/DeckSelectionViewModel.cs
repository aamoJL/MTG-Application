using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MTGApplication.Features.DeckSelection.Models;
using MTGApplication.Features.DeckSelection.UseCases;
using MTGApplication.General.Services.Databases.Repositories;
using MTGApplication.General.Services.Databases.Repositories.DeckRepository.Models;
using MTGApplication.General.Services.Importers.CardImporter;
using MTGApplication.General.ViewModels;
using System.Collections.ObjectModel;
using static MTGApplication.General.Services.NotificationService.NotificationService;

namespace MTGApplication.Features.DeckSelection.ViewModels;
public partial class DeckSelectionViewModel(IRepository<MTGCardDeckDTO> repository, IMTGCardImporter importer) : ObservableObject, IWorker
{
  public ObservableCollection<DeckSelectionDeck> DeckItems { get; } = [];
  public IRepository<MTGCardDeckDTO> Repository { get; } = repository;
  public IMTGCardImporter Importer { get; } = importer;
  public Notifier Notifier { get; } = new();
  public IWorker Worker => this;

  [ObservableProperty] public partial bool IsBusy { get; set; }

  public IAsyncRelayCommand? LoadDecksCommand => field ??= new DeckSelectorViewModelCommands.LoadDecks(this).Command;
}
