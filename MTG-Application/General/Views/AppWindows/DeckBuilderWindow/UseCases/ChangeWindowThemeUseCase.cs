using Microsoft.UI.Xaml;

namespace MTGApplication.General.Views;
/// <summary>
/// Use case to switch window theme between dark and light themes.
/// </summary>
public class ChangeWindowThemeUseCase : UseCase
{
  public ChangeWindowThemeUseCase(ElementTheme theme) : base() => Theme = theme;

  public ElementTheme Theme { get; }

  public override void Execute() => AppConfig.LocalSettings.AppTheme = Theme;
}
