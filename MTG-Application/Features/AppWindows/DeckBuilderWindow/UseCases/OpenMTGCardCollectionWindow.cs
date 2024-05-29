using MTGApplication.General.ViewModels;

namespace MTGApplication.Features.AppWindows.DeckBuilderWindow;
public class OpenMTGCardCollectionWindow : UseCase
{
  public override void Execute() => new CardCollectionWindow.CardCollectionWindow().Activate();
}