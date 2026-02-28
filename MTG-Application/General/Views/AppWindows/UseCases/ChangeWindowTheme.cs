using Microsoft.UI.Xaml;
using MTGApplication.General.ViewModels;

namespace MTGApplication.General.Views.AppWindows.UseCases;
/// <summary>
/// Use case to switch window theme between dark and light themes.
/// </summary>
public class ChangeWindowTheme : UseCaseAction<ElementTheme>
{
  public override void Execute(ElementTheme theme) => AppConfig.LocalSettings.AppTheme = theme;
}
