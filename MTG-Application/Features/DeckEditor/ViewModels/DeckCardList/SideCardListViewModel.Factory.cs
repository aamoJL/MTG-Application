using MTGApplication.Features.DeckEditor.Models;
using System.Collections.ObjectModel;

namespace MTGApplication.Features.DeckEditor.ViewModels.DeckCardList;

public partial class SideCardListViewModel
{
  public new class Factory : Factory<SideCardListViewModel>
  {
    public override SideCardListViewModel Build(ObservableCollection<DeckEditorMTGCard> list)
    {
      return new(list)
      {
        Worker = Worker,
        Importer = Importer,
        EdhrecImporter = EdhrecImporter,
        ScryfallImporter = ScryfallImporter,
        Exporter = Exporter,
        UndoStack = UndoStack,
        Notifier = Notifier,
        NetworkService = NetworkService,
        Confirmers = ListConfirmers,
      };
    }
  }
}
