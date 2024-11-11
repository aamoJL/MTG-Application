using CommunityToolkit.Mvvm.Input;
using MTGApplication.Features.DeckEditor.Editor.Models;
using MTGApplication.Features.DeckEditor.ViewModels;
using MTGApplication.General.Services.ReversibleCommandService;
using MTGApplication.General.ViewModels;
using System.Linq;
using static MTGApplication.Features.DeckEditor.CardList.UseCases.ReversibleActions.CardListViewModelReversibleActions;

namespace MTGApplication.Features.DeckEditor.CardList.UseCases;

public partial class CardListViewModelCommands
{
  public IRelayCommand ClearCommand { get; } = new Clear(viewmodel).Command;

  private class Clear(CardListViewModel viewmodel) : ViewModelCommand<CardListViewModel>(viewmodel)
  {
    protected override bool CanExecute() => Viewmodel.Cards.Any();

    protected override void Execute()
    {
      if (!CanExecute()) return;

      Viewmodel.UndoStack.PushAndExecute(
        new ReversibleCollectionCommand<DeckEditorMTGCard>(Viewmodel.Cards, Viewmodel.CardCopier)
        {
          ReversibleAction = new ReversibleRemoveCardAction(Viewmodel)
        });
    }
  }
}