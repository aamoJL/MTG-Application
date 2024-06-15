using MTGApplication.General.ViewModels;
using System.Linq;

namespace MTGApplication.Features.CardCollection.UseCases;

public partial class CardCollectionPageViewModelCommands
{
  public class SwitchCardOwnership(CardCollectionListViewModel viewmodel) : ViewModelCommand<CardCollectionListViewModel, CardCollectionMTGCard>(viewmodel)
  {
    protected override bool CanExecute(CardCollectionMTGCard card) => card != null;

    protected override void Execute(CardCollectionMTGCard card)
    {
      if (!CanExecute(card)) return;

      if (Viewmodel.OwnedCards.FirstOrDefault(x => x.Info.ScryfallId == card.Info.ScryfallId)
        is CardCollectionMTGCard existingCard)
        Viewmodel.OwnedCards.Remove(existingCard);
      else
        Viewmodel.OwnedCards.Add(card);

      Viewmodel.HasUnsavedChanges = true;
    }
  }
}