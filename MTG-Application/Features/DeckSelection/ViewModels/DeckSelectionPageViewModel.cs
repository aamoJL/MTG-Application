using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MTGApplication.Features.DeckSelection.Models;
using MTGApplication.Features.DeckSelection.UseCases;
using MTGApplication.General.Services.Databases.Repositories;
using MTGApplication.General.Services.Databases.Repositories.DeckRepository.Models;
using MTGApplication.General.Services.Importers.CardImporter;
using MTGApplication.General.Services.NotificationService.UseCases;
using MTGApplication.General.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using static MTGApplication.General.Services.NotificationService.NotificationService;

namespace MTGApplication.Features.DeckSelection.ViewModels;

public partial class DeckSelectionPageViewModel(IRepository<MTGCardDeckDTO> repository, IMTGCardImporter importer) : ObservableObject
{
  public IRepository<MTGCardDeckDTO> Repository { get; } = repository;
  public IMTGCardImporter Importer { get; } = importer;
  public Notifier Notifier { get; init; } = new();
  public Worker Worker { get; set; } = new();
  public ObservableCollection<DeckSelectionDeck> DeckItems { get; } = [];

  public Action<string> SelectDeck_UC { private get => field ??= (_) => { }; set; }
  public Func<Task<IEnumerable<DeckSelectionDeck>>> FetchDecks_UC { private get => field ??= new GetDeckSelectionDecks(Repository, Importer).Execute; set; }

  [RelayCommand]
  private void SelectDeck(DeckSelectionDeck item) => SelectDeck_UC(item?.Title ?? string.Empty);

  [RelayCommand]
  private async Task RefreshDecks()
  {
    try
    {
      var decks = await Worker.DoWork(FetchDecks_UC());

      DeckItems.Clear();

      foreach (var deck in decks)
        DeckItems.Add(deck);
    }
    catch (Exception e)
    {
      new SendNotification(Notifier).Execute(new(NotificationType.Error, $"Error: {e.Message}"));
    }
  }
}
