using MTGApplication.Features.AppWindows.DeckBuilderWindow.Controls;
using MTGApplication.General.ViewModels;

namespace MTGApplication.Features.AppWindows.DeckBuilderWindow.UseCases;
public class CreateNewDeckViewTabItem : UseCase<DeckSelectorAndEditorTabViewItem>
{
  public override DeckSelectorAndEditorTabViewItem Execute() => new();
}
