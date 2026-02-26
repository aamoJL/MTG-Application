using CommunityToolkit.Mvvm.Input;
using MTGApplication.Features.DeckEditor.Models;
using MTGApplication.General.Services.API.CardAPI;
using MTGApplication.General.Services.Importers.CardImporter;
using MTGApplication.General.Services.Importers.CardImporter.ScryfallAPI;
using MTGApplication.General.Services.IOServices;
using MTGApplication.General.Services.NotificationService.UseCases;
using MTGApplication.General.Services.ReversibleCommandService;
using MTGApplication.General.ViewModels;
using System;
using System.ComponentModel;
using System.Threading.Tasks;
using static MTGApplication.Features.DeckEditor.ViewModels.DeckCard.DeckCardViewModel;
using static MTGApplication.General.Services.NotificationService.NotificationService;

namespace MTGApplication.Features.DeckEditor.ViewModels.DeckCommanders;

public partial class DeckCommandersViewModel : ViewModelBase
{
  public DeckCommandersViewModel(DeckEditorMTGDeck deck)
  {
    Model = deck;
    Model.PropertyChanged += Model_PropertyChanged;
  }

  public Worker Worker { get; init; } = new();
  public ReversibleCommandStack UndoStack { get; init; } = new();
  public IMTGCardImporter Importer { private get; init; } = App.MTGCardImporter;
  public IEdhrecImporter EdhrecImporter { private get; init; } = new EdhrecImporter();
  public IScryfallImporter ScryfallImporter { private get; init; } = new ScryfallAPI();
  public Notifier Notifier { private get; init; } = new();
  public INetworkService NetworkService { private get; init; } = new NetworkService();
  public CardConfirmers Confirmers { private get; init; } = new();

  public CommanderViewModel Commander { get => field ??= CommanderViewModelFactory.Build(Model); private set; }
  public PartnerViewModel Partner { get => field ??= PartnerViewModelFactory.Build(Model); private set; }

  private DeckEditorMTGDeck Model { get; }
  private CommanderViewModel.Factory CommanderViewModelFactory => field ??= new()
  {
    Worker = Worker,
    UndoStack = UndoStack,
    Importer = Importer,
    EdhrecImporter = EdhrecImporter,
    ScryfallImporter = ScryfallImporter,
    Notifier = Notifier,
    NetworkService = NetworkService,
    Confirmers = Confirmers,
  };
  private PartnerViewModel.Factory PartnerViewModelFactory => field ??= new()
  {
    Worker = Worker,
    UndoStack = UndoStack,
    Importer = Importer,
    EdhrecImporter = EdhrecImporter,
    ScryfallImporter = ScryfallImporter,
    Notifier = Notifier,
    NetworkService = NetworkService,
    Confirmers = Confirmers,
  };

  [RelayCommand(CanExecute = nameof(CanOpenEdhrecWebsite))]
  private async Task OpenEdhrecWebsite()
  {
    try
    {
      if (Model.Commander == null)
        throw new InvalidOperationException("Deck does not have a commander");

      await NetworkService.OpenUri(
        General.Services.Importers.CardImporter.EdhrecImporter.GetCommanderWebsiteUri(Model.Commander!, Model.CommanderPartner));
    }
    catch (Exception e)
    {
      new ShowNotification(Notifier).Execute(new(NotificationType.Error, $"Error: {e.Message}"));
    }
  }

  private bool CanOpenEdhrecWebsite() => Model.Commander != null;

  private void Model_PropertyChanged(object? sender, PropertyChangedEventArgs e)
  {
    switch (e.PropertyName)
    {
      case nameof(Model.Commander): OpenEdhrecWebsiteCommand.NotifyCanExecuteChanged(); break;
    }
  }
}