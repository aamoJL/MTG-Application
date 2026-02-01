using MTGApplication.Features.CardCollectionEditor.CardCollectionList.Models;
using MTGApplication.Features.CardCollectionEditor.CardCollectionList.ViewModels;
using MTGApplication.General.ViewModels;
using System.Linq;

namespace MTGApplication.Features.CardCollectionEditor.CardCollectionList.UseCases;

public partial class CardCollectionEditorViewModelCommands
{
  public class SwitchCardOwnership(CardCollectionListViewModel viewmodel) : SyncCommand<CardCollectionMTGCard>
  {
    protected override bool CanExecute(CardCollectionMTGCard? card) => card != null;

    protected override void Execute(CardCollectionMTGCard? card)
    {
      if (!CanExecute(card))
        return;

      if (viewmodel.Cards.FirstOrDefault(x => x.Info.ScryfallId == card!.Info.ScryfallId) is CardCollectionMTGCard existingCard)
        viewmodel.Cards.Remove(existingCard);
      else
        viewmodel.Cards.Add(card!);
    }
  }
}