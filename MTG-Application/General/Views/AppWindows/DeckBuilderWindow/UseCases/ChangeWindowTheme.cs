using Microsoft.UI.Xaml;
using MTGApplication.General.ViewModels;

namespace MTGApplication.General.Views.AppWindows;
/// <summary>
/// Use case to switch window theme between dark and light themes.
/// </summary>
public class ChangeWindowTheme : UseCase
{
  public ChangeWindowTheme(ElementTheme theme) : base() => Theme = theme;

  public ElementTheme Theme { get; }

  public override void Execute() => AppConfig.LocalSettings.AppTheme = Theme;
}
