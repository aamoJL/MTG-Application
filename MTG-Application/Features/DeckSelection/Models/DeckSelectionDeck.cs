using Microsoft.UI.Xaml;
using MTGApplication.General.Models;
using System.Linq;

namespace MTGApplication.Features.DeckSelection.Models;

public class DeckSelectionDeck(string title, MTGCardInfo.ColorTypes[] colors, string imageUri = "")
{
  public string Title { get; } = title;
  public string ImageUri { get; } = imageUri;

  public Visibility WhiteVisibility { get; } = colors.Contains(MTGCardInfo.ColorTypes.W) ? Visibility.Visible : Visibility.Collapsed;
  public Visibility BlueVisibility { get; } = colors.Contains(MTGCardInfo.ColorTypes.U) ? Visibility.Visible : Visibility.Collapsed;
  public Visibility BlackVisibility { get; } = colors.Contains(MTGCardInfo.ColorTypes.B) ? Visibility.Visible : Visibility.Collapsed;
  public Visibility RedVisibility { get; } = colors.Contains(MTGCardInfo.ColorTypes.R) ? Visibility.Visible : Visibility.Collapsed;
  public Visibility GreenVisibility { get; } = colors.Contains(MTGCardInfo.ColorTypes.G) ? Visibility.Visible : Visibility.Collapsed;
  public Visibility ColorlessVisibility { get; } = colors.Contains(MTGCardInfo.ColorTypes.C) ? Visibility.Visible : Visibility.Collapsed;
}
