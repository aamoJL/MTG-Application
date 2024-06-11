using Microsoft.UI.Xaml;
using MTGApplication.General.ViewModels;

namespace MTGApplication.Features.AppWindows.DeckBuilderWindow.UseCases;
/// <summary>
/// Use case to switch window theme between dark and light themes.
/// </summary>
public class ChangeWindowTheme(ElementTheme theme) : UseCase()
{
  public override void Execute() => AppConfig.LocalSettings.AppTheme = theme;
}
