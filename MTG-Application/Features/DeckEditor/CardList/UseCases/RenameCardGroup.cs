using CommunityToolkit.Mvvm.Input;
using MTGApplication.Features.DeckEditor.CardList.Services;
using MTGApplication.Features.DeckEditor.ViewModels;
using MTGApplication.General.Services.ReversibleCommandService;
using MTGApplication.General.ViewModels;
using System.Threading.Tasks;
using static MTGApplication.Features.DeckEditor.CardList.UseCases.ReversibleActions.CardListViewModelReversibleActions;

namespace MTGApplication.Features.DeckEditor.CardList.UseCases;

public partial class CardGroupViewModelCommands
{
  public IAsyncRelayCommand RenameGroupCommand { get; } = new RenameCardGroup(groupViewmodel, listViewmodel).Command;

  private class RenameCardGroup(CardGroupViewModel viewmodel, GroupedCardListViewModel listViewmodel) : ViewModelAsyncCommand<CardGroupViewModel>(viewmodel)
  {
    protected override bool CanExecute() => !string.IsNullOrEmpty(Viewmodel.Key);

    protected override async Task Execute()
    {
      if (!CanExecute())
        return;

      if (await listViewmodel.Confirmers.RenameCardGroupConfirmer.Confirm(GroupedCardListConfirmers.GetRenameCardGroupConfirmation(Viewmodel.Key)) is string rename
        && !string.IsNullOrEmpty(rename)
        && rename != Viewmodel.Key)
      {
        listViewmodel.UndoStack.PushAndExecute(
          new ReversiblePropertyChangeCommand<string>(Viewmodel.Key, rename)
          {
            ReversibleAction = new ReversibleRenameGroupAction(Viewmodel)
          });
      }
    }
  }
}