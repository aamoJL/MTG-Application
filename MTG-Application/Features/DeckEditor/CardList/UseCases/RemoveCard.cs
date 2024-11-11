using CommunityToolkit.Mvvm.Input;
using MTGApplication.Features.DeckEditor.Editor.Models;
using MTGApplication.Features.DeckEditor.ViewModels;
using MTGApplication.General.Services.ReversibleCommandService;
using MTGApplication.General.ViewModels;
using static MTGApplication.Features.DeckEditor.CardList.UseCases.ReversibleActions.CardListViewModelReversibleActions;

namespace MTGApplication.Features.DeckEditor.CardList.UseCases;

public partial class CardListViewModelCommands
{
  public IRelayCommand<DeckEditorMTGCard> RemoveCardCommand { get; } = new RemoveCard(viewmodel).Command;

  private class RemoveCard(CardListViewModel viewmodel) : ViewModelCommand<CardListViewModel, DeckEditorMTGCard>(viewmodel)
  {
    protected override void Execute(DeckEditorMTGCard card)
      => Viewmodel.UndoStack.PushAndExecute(
        new ReversibleCollectionCommand<DeckEditorMTGCard>(card, Viewmodel.CardCopier)
        {
          ReversibleAction = new ReversibleRemoveCardAction(Viewmodel)
        });
  }
}