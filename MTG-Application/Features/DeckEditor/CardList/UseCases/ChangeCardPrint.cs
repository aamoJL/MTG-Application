using CommunityToolkit.Mvvm.Input;
using MTGApplication.Features.DeckEditor.CardList.Services;
using MTGApplication.Features.DeckEditor.Editor.Models;
using MTGApplication.Features.DeckEditor.ViewModels;
using MTGApplication.General.Models;
using MTGApplication.General.Services.ReversibleCommandService;
using MTGApplication.General.ViewModels;
using System.Linq;
using System.Threading.Tasks;
using static MTGApplication.Features.DeckEditor.CardList.UseCases.ReversibleActions.CardListViewModelReversibleActions;

namespace MTGApplication.Features.DeckEditor.CardList.UseCases;

public partial class CardListViewModelCommands
{
  public IAsyncRelayCommand<DeckEditorMTGCard> ChangeCardPrintCommand { get; } = new ChangeCardPrint(viewmodel).Command;

  private class ChangeCardPrint(CardListViewModel viewmodel) : ViewModelAsyncCommand<CardListViewModel, DeckEditorMTGCard>(viewmodel)
  {
    protected override async Task Execute(DeckEditorMTGCard card)
    {
      if (Viewmodel.Cards.FirstOrDefault(x => x.Info.Name == card.Info.Name) is DeckEditorMTGCard existingCard)
      {
        var prints = (await Viewmodel.Worker.DoWork(Viewmodel.Importer.ImportFromUri(pageUri: existingCard.Info.PrintSearchUri, paperOnly: true, fetchAll: true))).Found.Select(x => x.Info);

        if (await Viewmodel.Confirmers.ChangeCardPrintConfirmer.Confirm(CardListConfirmers.GetChangeCardPrintConfirmation(prints.Select(x => new MTGCard(x)))) is MTGCard selection)
        {
          if (selection.Info.ScryfallId == existingCard.Info.ScryfallId)
            return; // Same print

          Viewmodel.UndoStack.PushAndExecute(
            new ReversiblePropertyChangeCommand<DeckEditorMTGCard, MTGCardInfo>(existingCard, existingCard.Info, selection.Info)
            {
              ReversibleAction = new ReversibleCardPrintChangeAction(Viewmodel)
            });
        }
      }
    }
  }
}