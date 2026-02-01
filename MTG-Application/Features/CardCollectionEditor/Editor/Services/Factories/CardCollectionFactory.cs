using MTGApplication.Features.CardCollection.Editor.ViewModels;
using MTGApplication.Features.CardCollectionEditor.CardCollectionList.ViewModels;
using System.Linq;

namespace MTGApplication.Features.CardCollectionEditor.Editor.Services.Factories;

public class CardCollectionFactory(CardCollectionEditorViewModel viewmodel)
{
  public CardCollectionListViewModel CreateCardCollectionListViewModel()
  {
    return new CardCollectionListViewModel(importer: viewmodel.Importer)
    {
      Confirmers = viewmodel.Confirmers.CardCollectionListConfirmers,
      Notifier = viewmodel.Notifier,
      ClipboardService = viewmodel.ClipboardService,
      Worker = viewmodel.Worker,
      NameValidator = (name) => { return viewmodel.CollectionLists.FirstOrDefault(x => x.Name == name) is null; },
    };
  }
}
