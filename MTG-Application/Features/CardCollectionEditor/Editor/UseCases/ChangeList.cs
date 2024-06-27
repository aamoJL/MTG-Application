using MTGApplication.Features.CardCollection.Editor.ViewModels;
using MTGApplication.Features.CardCollectionEditor.CardCollectionList.Models;
using MTGApplication.General.ViewModels;
using System.Threading.Tasks;

namespace MTGApplication.Features.CardCollection.Editor.UseCases;

public partial class CardCollectionEditorViewModelCommands
{
  public class ChangeList(CardCollectionEditorViewModel viewmodel) : ViewModelAsyncCommand<CardCollectionEditorViewModel, MTGCardCollectionList>(viewmodel)
  {
    protected override bool CanExecute(MTGCardCollectionList list)
      => list == null || (Viewmodel.SelectedCardCollectionList != list && Viewmodel.CardCollectionViewModel.CollectionLists.Contains(list));

    protected override async Task Execute(MTGCardCollectionList list)
    {
      if (!CanExecute(list)) return;

      await Viewmodel.ChangeCollectionList(list);
    }
  }
}