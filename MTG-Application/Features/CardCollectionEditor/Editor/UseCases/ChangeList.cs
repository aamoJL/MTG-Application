using MTGApplication.Features.CardCollection.Editor.ViewModels;
using MTGApplication.Features.CardCollectionEditor.CardCollectionList.Models;
using MTGApplication.General.ViewModels;
using System.Threading.Tasks;

namespace MTGApplication.Features.CardCollection.Editor.UseCases;

public partial class CardCollectionEditorViewModelCommands
{
  public class ChangeList(CardCollectionEditorViewModel viewmodel) : AsyncCommand<MTGCardCollectionList>
  {
    protected override bool CanExecute(MTGCardCollectionList? list)
      => list != null && viewmodel.SelectedCardCollectionListViewModel.CollectionList != list && viewmodel.Collection.CollectionLists.Contains(list);

    protected override async Task Execute(MTGCardCollectionList? list)
    {
      if (!CanExecute(list)) return;
      if (list == null) return;

      await viewmodel.SelectedCardCollectionListViewModel.ChangeCollectionList(list);
    }
  }
}