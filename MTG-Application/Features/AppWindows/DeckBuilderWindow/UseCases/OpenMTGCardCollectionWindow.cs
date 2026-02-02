using MTGApplication.General.ViewModels;

namespace MTGApplication.Features.AppWindows.DeckBuilderWindow.UseCases;

public class OpenMTGCardCollectionWindow : UseCaseAction
{
  public override void Execute() => new CardCollectionWindow.CardCollectionWindow().Activate();
}