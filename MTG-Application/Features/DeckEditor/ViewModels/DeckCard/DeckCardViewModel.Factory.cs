using MTGApplication.Features.DeckEditor.Models;
using MTGApplication.General.Services.Importers.CardImporter;
using MTGApplication.General.Services.IOServices;
using MTGApplication.General.Services.ReversibleCommandService;
using MTGApplication.General.ViewModels;
using System;
using static MTGApplication.General.Services.NotificationService.NotificationService;

namespace MTGApplication.Features.DeckEditor.ViewModels.DeckCard;

public partial class DeckCardViewModel
{
  public class Factory
  {
    public required Worker Worker { get; init; }
    public required IMTGCardImporter Importer { private get; init; }
    public required ReversibleCommandStack UndoStack { private get; init; }
    public required Notifier Notifier { private get; init; }
    public required INetworkService NetworkService { private get; init; }
    public required CardConfirmers Confirmers { private get; init; }
    public required Action<DeckEditorMTGCard> OnCardDelete { private get; init; }

    public DeckCardViewModel Build(DeckEditorMTGCard card)
    {
      return new(card)
      {
        Worker = Worker,
        Importer = Importer,
        UndoStack = UndoStack,
        Notifier = Notifier,
        NetworkService = NetworkService,
        Confirmers = Confirmers,
        OnDelete = OnCardDelete,
      };
    }
  }
}
