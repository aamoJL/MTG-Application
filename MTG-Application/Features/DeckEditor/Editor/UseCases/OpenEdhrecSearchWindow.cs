using MTGApplication.Features.DeckEditor.ViewModels;
using MTGApplication.General.ViewModels;

namespace MTGApplication.Features.DeckEditor.Editor.UseCases;

public class OpenEdhrecSearchWindow(DeckEditorViewModel viewmodel) : ViewModelCommand<DeckEditorViewModel>(viewmodel)
{
  protected override bool CanExecute() => Viewmodel.Commander != null;

  protected override void Execute()
  {
    // TODO: commander themes
    // var themes = await CommanderAPI.GetThemes(new Models.Structs.Commanders(
    //  deck.Commander?.Info.Name ?? string.Empty, deck.CommanderPartner?.Info.Name ?? string.Empty));

    new AppWindows.EdhrecSearchWindow.EdhrecSearchWindow().Activate();
  }
}