﻿using Microsoft.UI.Xaml;
using SkiaSharp;

namespace MTGApplication.Features.DeckEditor.Charts.Models;

/// <summary>
/// Colors for charts
/// </summary>
public static class ChartColorPalette
{
  public static readonly SKColor White = new(235, 235, 235, 200);
  public static readonly SKColor Blue = new(80, 130, 186, 200);
  public static readonly SKColor Black = new(80, 80, 80, 200);
  public static readonly SKColor Red = new(186, 80, 80, 200);
  public static readonly SKColor Green = new(80, 186, 80, 200);
  public static readonly SKColor Multicolor = new(186, 186, 80, 200);
  public static readonly SKColor Colorless = new(186, 186, 186, 200);

  public static readonly SKColor LightThemeText = new(45, 45, 45);
  public static readonly SKColor DarkThemeText = new(255, 255, 255);

  /// <summary>
  /// Returns text color according to the selected theme
  /// </summary>
  public static SKColor ForegroundColor
  {
    get
    {
      return AppConfig.LocalSettings.AppTheme switch
      {
        ElementTheme.Light => LightThemeText,
        ElementTheme.Dark => DarkThemeText,
        _ => LightThemeText
      };
    }
  }
}
