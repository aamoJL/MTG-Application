using CommunityToolkit.Mvvm.Input;
using MTGApplication.Features.DeckEditor.ViewModels;
using MTGApplication.General.ViewModels;
using System.Threading.Tasks;

namespace MTGApplication.Features.DeckEditor.Editor.UseCases;

public partial class DeckEditorViewModelCommands
{
  public IAsyncRelayCommand NewDeckCommand { get; } = new NewDeck(viewmodel).Command;

  private class NewDeck(DeckEditorViewModel viewmodel) : ViewModelAsyncCommand<DeckEditorViewModel>(viewmodel)
  {
    protected override async Task Execute()
    {
      var unsavedArgs = new ISavable.ConfirmArgs();

      await new ConfirmUnsavedChanges(Viewmodel).Command.ExecuteAsync(unsavedArgs);

      if (unsavedArgs.Cancelled)
        return;

      Viewmodel.SetDeck(new());
    }
  }
}