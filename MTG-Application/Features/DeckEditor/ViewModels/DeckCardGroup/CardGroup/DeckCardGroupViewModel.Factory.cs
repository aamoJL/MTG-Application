using MTGApplication.Features.DeckEditor.Models;
using MTGApplication.Features.DeckEditor.Services;
using MTGApplication.Features.DeckEditor.ViewModels.DeckCard;
using MTGApplication.Features.DeckEditor.ViewModels.DeckCardList;
using MTGApplication.General.Services.Importers.CardImporter;
using MTGApplication.General.Services.ReversibleCommandService;
using MTGApplication.General.ViewModels;
using System;
using static MTGApplication.General.Services.NotificationService.NotificationService;

namespace MTGApplication.Features.DeckEditor.ViewModels.DeckCardGroup.CardGroup;

public partial class DeckCardGroupViewModel
{
  public class Factory
  {
    public required Worker Worker { private get; init; }
    public required ReversibleCommandStack UndoStack { private get; init; }
    public required IMTGCardImporter Importer { private get; init; }
    public required IEdhrecImporter EdhrecImporter { private get; init; }
    public required Notifier Notifier { private get; init; }
    public required CardFilters CardFilter { private get; init; } = new();
    public required CardSorter CardSorter { private get; init; } = new();
    public required GroupConfirmers GroupConfirmers { private get; init; }
    public required DeckCardListViewModel.CardListConfirmers ListConfirmers { private get; init; }
    public required DeckCardViewModel.Factory CardFactory { private get; init; }
    public required Action<DeckEditorCardGroup> OnGroupDelete { private get; init; }
    public required Action<DeckEditorCardGroup, string> OnGroupRename { private get; init; }

    public DeckCardGroupViewModel Build(DeckEditorCardGroup group)
    {
      return new(group)
      {
        Worker = Worker,
        Importer = Importer,
        EdhrecImporter = EdhrecImporter,
        UndoStack = UndoStack,
        Notifier = Notifier,
        Confirmers = GroupConfirmers,
        ListConfirmers = ListConfirmers,
        CardViewModelFactory = CardFactory,
        CardFilter = CardFilter,
        CardSorter = CardSorter,
        OnDelete = OnGroupDelete,
        OnRename = OnGroupRename,
      };
    }
  }
}
