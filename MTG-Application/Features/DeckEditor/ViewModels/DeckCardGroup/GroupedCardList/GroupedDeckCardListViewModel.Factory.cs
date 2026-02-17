using MTGApplication.Features.DeckEditor.Models;
using MTGApplication.Features.DeckEditor.ViewModels.DeckCardList;
using System.Collections.ObjectModel;

namespace MTGApplication.Features.DeckEditor.ViewModels.DeckCardGroup.GroupedCardList;

public partial class GroupedDeckCardListViewModel
{
  public new class Factory : DeckCardListViewModel.Factory
  {
    public required GroupedCardListConfirmers GroupConfirmers { protected get; init; }

    public new GroupedDeckCardListViewModel Build(ObservableCollection<DeckEditorMTGCard> list)
    {
      return new(list)
      {
        Worker = Worker,
        Importer = Importer,
        UndoStack = UndoStack,
        Notifier = Notifier,
        NetworkService = NetworkService,
        Confirmers = ListConfirmers,
        EdhrecImporter = EdhrecImporter,
        Exporter = Exporter,
        GroupedListConfirmers = GroupConfirmers,
      };
    }
  }
}
