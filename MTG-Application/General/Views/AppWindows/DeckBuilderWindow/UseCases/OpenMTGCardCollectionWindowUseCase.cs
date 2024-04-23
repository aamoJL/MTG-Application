using MTGApplication.General.UseCases;
using MTGApplication.Views.Pages;
using MTGApplication.Views.Windows;

namespace MTGApplication.General.Views;
public class OpenMTGCardCollectionWindowUseCase : UseCase
{
  public override void Execute() => new ThemedWindow
  {
    Content = new MTGCardCollectionPage(),
    Title = "MTG Card Collections"
  }.Activate();
}
