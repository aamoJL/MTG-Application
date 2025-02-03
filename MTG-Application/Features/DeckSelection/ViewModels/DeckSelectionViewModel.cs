using CommunityToolkit.Mvvm.ComponentModel;
using MTGApplication.Features.DeckSelection.Models;
using MTGApplication.Features.DeckSelection.UseCases;
using MTGApplication.General.Services.Databases.Repositories;
using MTGApplication.General.Services.Databases.Repositories.DeckRepository.Models;
using MTGApplication.General.Services.Importers.CardImporter;
using MTGApplication.General.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using static MTGApplication.General.Services.NotificationService.NotificationService;

namespace MTGApplication.Features.DeckSelection.ViewModels;

public partial class DeckSelectionViewModel : ObservableObject, IWorker
{
  public DeckSelectionViewModel(IRepository<MTGCardDeckDTO> repository, IMTGCardImporter importer)
  {
    Importer = importer;
    Repository = repository;

    DeckFetchingTask = (this as IWorker).DoWork(UpdateDecks());
  }

  public IRepository<MTGCardDeckDTO> Repository { get; }
  public IMTGCardImporter Importer { get; }
  public Notifier Notifier { get; init; } = new();

  public ObservableCollection<DeckSelectionDeck> DeckItems { get; } = [];

  [ObservableProperty] public partial bool IsBusy { get; set; }

  private Task DeckFetchingTask { get; set; }

  public async Task WaitForDeckUpdate() => await DeckFetchingTask;

  private async Task UpdateDecks()
  {
    try
    {
      var decks = await new GetDeckSelectionDecks(Repository, Importer).Execute();

      foreach (var deck in decks)
        DeckItems.Add(deck);
    }
    catch (Exception e)
    {
      Notifier.Notify(new(NotificationType.Error, $"Error: {e.Message}"));

      await Task.FromException(e);
    }
  }
}
