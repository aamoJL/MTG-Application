using CommunityToolkit.Mvvm.Input;
using MTGApplication.Features.DeckEditor.Editor.Models;
using MTGApplication.Features.DeckEditor.ViewModels;
using MTGApplication.General.ViewModels;

namespace MTGApplication.Features.DeckEditor.CardList.UseCases;

public partial class CardGroupViewModelCommands
{
  public IRelayCommand<DeckEditorMTGCard> BeginMoveFromCommand { get; } = new MoveGroupCard.BeginMoveFrom(groupViewmodel).Command;

  private class MoveGroupCard
  {
    public class BeginMoveFrom(CardGroupViewModel viewmodel) : ViewModelCommand<CardGroupViewModel, DeckEditorMTGCard>(viewmodel)
    {
      protected override void Execute(DeckEditorMTGCard param)
      {

      }
    }
  }
}