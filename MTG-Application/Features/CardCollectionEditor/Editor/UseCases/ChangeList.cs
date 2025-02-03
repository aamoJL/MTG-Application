using MTGApplication.Features.CardCollection.Editor.ViewModels;
using MTGApplication.Features.CardCollectionEditor.CardCollectionList.Models;
using MTGApplication.General.ViewModels;

namespace MTGApplication.Features.CardCollection.Editor.UseCases;

public partial class CardCollectionEditorViewModelCommands
{
  public class ChangeList(CardCollectionEditorViewModel viewmodel) : ViewModelCommand<CardCollectionEditorViewModel, MTGCardCollectionList>(viewmodel)
  {
    protected override bool CanExecute(MTGCardCollectionList? list)
      => list != null && Viewmodel.SelectedCardCollectionListViewModel.CollectionList != list && Viewmodel.Collection.CollectionLists.Contains(list);

    protected override void Execute(MTGCardCollectionList? list)
    {
      if (!CanExecute(list))
        return;

      Viewmodel.SelectedCardCollectionListViewModel.CollectionList = list!;
    }
  }
}