using MTGApplication.General.ViewModels;
using System.Linq;

namespace MTGApplication.Features.CardCollection.UseCases;

public partial class CardCollectionViewModelCommands
{
  public class SwitchCardOwnership(CardCollectionViewModel viewmodel) : ViewModelCommand<CardCollectionViewModel, CardCollectionMTGCard>(viewmodel)
  {
    protected override bool CanExecute(CardCollectionMTGCard card) => card != null && Viewmodel.SelectedList != null;

    protected override void Execute(CardCollectionMTGCard card)
    {
      if (!CanExecute(card)) return;

      if (Viewmodel.SelectedList.Cards.FirstOrDefault(x => x.Info.ScryfallId == card.Info.ScryfallId)
        is CardCollectionMTGCard existingCard)
        Viewmodel.SelectedList.Cards.Remove(existingCard);
      else
        Viewmodel.SelectedList.Cards.Add(card);

      Viewmodel.HasUnsavedChanges = true;

      // TODO: move to viewmodel
      //Viewmodel.OnPropertyChanged(nameof(SelectedListCardCount));
    }
  }
}