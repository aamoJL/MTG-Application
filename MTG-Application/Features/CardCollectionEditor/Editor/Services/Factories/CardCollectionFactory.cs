using MTGApplication.Features.CardCollection.Editor.ViewModels;
using MTGApplication.Features.CardCollectionEditor.CardCollectionList.Models;
using MTGApplication.Features.CardCollectionEditor.CardCollectionList.ViewModels;

namespace MTGApplication.Features.CardCollectionEditor.Editor.Services.Factories;

public class CardCollectionFactory(CardCollectionEditorViewModel viewmodel)
{
  public CardCollectionListViewModel CreateCardCollectionListViewModel(MTGCardCollectionList? model = null)
  {
    return new(importer: viewmodel.Importer)
    {
      CollectionList = model ?? new(),
      Confirmers = viewmodel.Confirmers.CardCollectionListConfirmers,
      Notifier = viewmodel.Notifier,
      ClipboardService = viewmodel.ClipboardService,
      Worker = viewmodel,
    };
  }
}
