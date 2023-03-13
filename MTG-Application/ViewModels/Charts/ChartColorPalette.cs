using SkiaSharp;

namespace MTGApplication.ViewModels.Charts
{
  public static class ChartColorPalette
  {
    public static readonly SKColor White = new(235, 235, 235, 100);
    public static readonly SKColor Blue = new(80, 130, 186, 100);
    public static readonly SKColor Black = new(80, 80, 80, 100);
    public static readonly SKColor Red = new(186, 80, 80, 100);
    public static readonly SKColor Green = new(80, 186, 80, 100);
    public static readonly SKColor Multicolor = new(186, 186, 80, 100);
    public static readonly SKColor Colorless = new(186, 186, 186, 100);
  }
}