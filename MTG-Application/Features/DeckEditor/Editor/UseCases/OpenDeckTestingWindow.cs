using MTGApplication.Features.AppWindows.DeckTestingWindow;
using MTGApplication.General.ViewModels;

namespace MTGApplication.Features.DeckEditor.Editor.UseCases;

public class OpenDeckTestingWindow : UseCase
{
  public override void Execute() => new CardTestingWindow().Activate();
}