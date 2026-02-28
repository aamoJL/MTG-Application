using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MTGApplication.Features.DeckSelection.Models;
using MTGApplication.Features.DeckSelection.UseCases;
using MTGApplication.General.Services.Databases.Repositories;
using MTGApplication.General.Services.Databases.Repositories.DeckRepository;
using MTGApplication.General.Services.Databases.Repositories.DeckRepository.Models;
using MTGApplication.General.Services.Importers.CardImporter;
using MTGApplication.General.Services.NotificationService.UseCases;
using MTGApplication.General.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using static MTGApplication.General.Services.NotificationService.NotificationService;

namespace MTGApplication.Features.DeckSelection.ViewModels.SelectionPage;

public partial class DeckSelectionPageViewModel : ObservableObject
{
  public Worker Worker { get; init; } = new();
  public IRepository<MTGCardDeckDTO> Repository { private get; init; } = new DeckDTORepository();
  public IMTGCardImporter Importer { private get; init; } = App.MTGCardImporter;
  public Notifier Notifier { private get; init; } = new();

  public ObservableCollection<DeckSelectionDeck> DeckItems { get; } = [];

  public Action<DeckSelectionDeck> OnSelected { private get => field ?? throw new NotImplementedException(); init; }

  [RelayCommand]
  private void NewDeck()
  {
    try
    {
      OnSelected(new());
    }
    catch (Exception e)
    {
      new ShowNotification(Notifier).Execute(new(NotificationType.Error, $"Error: {e.Message}"));
    }
  }

  [RelayCommand]
  private void SelectDeck(DeckSelectionDeck deck)
  {
    try
    {
      OnSelected(deck);
    }
    catch (Exception e)
    {
      new ShowNotification(Notifier).Execute(new(NotificationType.Error, $"Error: {e.Message}"));
    }
  }

  [RelayCommand]
  private async Task RefreshDecks()
  {
    try
    {
      var decks = await Worker.DoWork(new FetchDecks(Repository, Importer).Execute());

      DeckItems.Clear();

      foreach (var deck in decks)
        DeckItems.Add(deck);
    }
    catch (Exception e)
    {
      new ShowNotification(Notifier).Execute(new(NotificationType.Error, $"Error: {e.Message}"));
    }
  }
}