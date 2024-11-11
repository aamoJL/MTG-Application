using CommunityToolkit.Mvvm.Input;
using MTGApplication.Features.DeckEditor.Editor.Models;
using MTGApplication.Features.DeckEditor.ViewModels;
using MTGApplication.General.Services.ReversibleCommandService;
using MTGApplication.General.ViewModels;
using static MTGApplication.Features.DeckEditor.CardList.UseCases.ReversibleActions.CardListViewModelReversibleActions;

namespace MTGApplication.Features.DeckEditor.CardList.UseCases;

public partial class CardListViewModelCommands
{
  public IRelayCommand<CardCountChangeArgs> ChangeCardCountCommand { get; } = new ChangeCardCount(viewmodel).Command;

  public record CardCountChangeArgs(DeckEditorMTGCard Card, int Value);

  private class ChangeCardCount(CardListViewModel viewmodel) : ViewModelCommand<CardListViewModel, CardCountChangeArgs>(viewmodel)
  {
    protected override bool CanExecute(CardCountChangeArgs args) => args.Card.Count != args.Value && args.Value > 0;

    protected override void Execute(CardCountChangeArgs args)
    {
      if (!CanExecute(args)) return;

      var (card, newValue) = args;

      Viewmodel.UndoStack.PushAndExecute(
        new ReversiblePropertyChangeCommand<DeckEditorMTGCard, int>(card, card.Count, newValue, Viewmodel.CardCopier)
        {
          ReversibleAction = new ReversibleCardCountChangeAction(Viewmodel)
        });
    }
  }
}