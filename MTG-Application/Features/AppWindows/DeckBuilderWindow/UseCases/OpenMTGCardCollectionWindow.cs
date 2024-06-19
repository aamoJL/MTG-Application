using MTGApplication.General.ViewModels;

namespace MTGApplication.Features.AppWindows.DeckBuilderWindow.UseCases;
public class OpenMTGCardCollectionWindow : UseCase
{
  public override void Execute() => new CardCollectionWindow.CardCollectionWindow().Activate();
}
