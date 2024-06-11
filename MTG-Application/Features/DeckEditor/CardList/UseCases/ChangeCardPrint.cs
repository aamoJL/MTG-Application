using MTGApplication.Features.DeckEditor.Services.Cardlist;
using MTGApplication.General.Models.Card;
using MTGApplication.General.Services.ReversibleCommandService;
using MTGApplication.General.ViewModels;
using System.Linq;
using System.Threading.Tasks;

namespace MTGApplication.Features.DeckEditor;

public partial class CardListViewModelCommands
{
  public class ChangeCardPrint(CardListViewModel viewmodel) : ViewModelAsyncCommand<CardListViewModel, DeckEditorMTGCard>(viewmodel)
  {
    protected override async Task Execute(DeckEditorMTGCard card)
    {
      if (Viewmodel.Cards.FirstOrDefault(x => x.Info.Name == card.Info.Name) is DeckEditorMTGCard existingCard)
      {
        var prints = (await Viewmodel.Worker.DoWork(Viewmodel.Importer.ImportFromUri(pageUri: existingCard.Info.PrintSearchUri, paperOnly: true, fetchAll: true))).Found.Select(x => x.Info);

        if (await Viewmodel.Confirmers.ChangeCardPrintConfirmer.Confirm(CardListConfirmers.GetChangeCardPrintConfirmation(prints)) is MTGCardInfo selection)
        {
          if (selection.ScryfallId == existingCard.Info.ScryfallId)
            return; // Same print

          Viewmodel.UndoStack.PushAndExecute(
            new ReversiblePropertyChangeCommand<DeckEditorMTGCard, MTGCardInfo>(existingCard, existingCard.Info, selection, Viewmodel.CardCopier)
            {
              ReversibleAction = new CardListViewModelReversibleActions.ReversibleCardPrintChangeAction(Viewmodel)
            });
        }
      }
    }
  }
}