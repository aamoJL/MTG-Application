using MTGApplication.Features.DeckEditor.Models;
using MTGApplication.General.Services.Importers.CardImporter;
using MTGApplication.General.Services.Importers.CardImporter.ScryfallAPI;
using MTGApplication.General.Services.IOServices;
using MTGApplication.General.Services.ReversibleCommandService;
using MTGApplication.General.ViewModels;
using static MTGApplication.Features.DeckEditor.ViewModels.DeckCard.DeckCardViewModel;
using static MTGApplication.General.Services.NotificationService.NotificationService;

namespace MTGApplication.Features.DeckEditor.ViewModels.DeckCommanders;

public abstract partial class CommanderViewModelBase
{
  public abstract class Factory<T> where T : CommanderViewModelBase
  {
    public required ReversibleCommandStack UndoStack { protected get; init; }
    public required Worker Worker { protected get; init; }
    public required IMTGCardImporter Importer { protected get; init; }
    public required IEdhrecImporter EdhrecImporter { protected get; init; }
    public required IScryfallImporter ScryfallImporter { protected get; init; }
    public required Notifier Notifier { protected get; init; }
    public required INetworkService NetworkService { protected get; init; }
    public required CardConfirmers Confirmers { protected get; init; }

    public abstract T Build(DeckEditorMTGDeck deck);
  }
}