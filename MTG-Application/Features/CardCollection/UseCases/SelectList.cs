using MTGApplication.General.ViewModels;
using System.Threading.Tasks;

namespace MTGApplication.Features.CardCollection.UseCases;

public partial class CardCollectionViewModelCommands
{
  public class SelectList(CardCollectionViewModel viewmodel) : ViewModelAsyncCommand<CardCollectionViewModel, MTGCardCollectionList>(viewmodel)
  {
    protected override bool CanExecute(MTGCardCollectionList list)
      => Viewmodel.SelectedList != list && (list == null || Viewmodel.Collection.CollectionLists.Contains(list));

    protected override async Task Execute(MTGCardCollectionList list)
    {
      if (!CanExecute(list)) return;

      Viewmodel.SelectedList = list;
      Viewmodel.QueryCardsViewModel.OwnedCards = Viewmodel.SelectedList?.Cards ?? [];

      // TODO: move to viewmodel
      //Viewmodel.OnPropertyChanged(nameof(Viewmodel.SelectedListCardCount));
      await Viewmodel.Worker.DoWork(Viewmodel.QueryCardsViewModel.UpdateQueryCards(Viewmodel.SelectedList?.SearchQuery ?? string.Empty));
    }
  }
}