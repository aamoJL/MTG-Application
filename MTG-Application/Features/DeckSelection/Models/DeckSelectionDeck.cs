using Microsoft.UI.Xaml;
using MTGApplication.General.Models;
using System.Linq;

namespace MTGApplication.Features.DeckSelection.Models;

public class DeckSelectionDeck
{
  public DeckSelectionDeck(string title, string imageUri = "", MTGCardInfo.ColorTypes[] colors = null)
  {
    Title = title;
    ImageUri = imageUri;

    if (colors != null)
    {
      if (colors.Length == 0 || colors.Contains(MTGCardInfo.ColorTypes.C))
        ColorlessVisibility = Visibility.Visible;
      else
      {
        WhiteVisibility = colors.Contains(MTGCardInfo.ColorTypes.W) ? Visibility.Visible : Visibility.Collapsed;
        BlueVisibility = colors.Contains(MTGCardInfo.ColorTypes.U) ? Visibility.Visible : Visibility.Collapsed;
        BlackVisibility = colors.Contains(MTGCardInfo.ColorTypes.B) ? Visibility.Visible : Visibility.Collapsed;
        RedVisibility = colors.Contains(MTGCardInfo.ColorTypes.R) ? Visibility.Visible : Visibility.Collapsed;
        GreenVisibility = colors.Contains(MTGCardInfo.ColorTypes.G) ? Visibility.Visible : Visibility.Collapsed;
      }
    }
  }

  public string Title { get; }
  public string ImageUri { get; }

  public Visibility WhiteVisibility { get; } = Visibility.Collapsed;
  public Visibility BlueVisibility { get; } = Visibility.Collapsed;
  public Visibility BlackVisibility { get; } = Visibility.Collapsed;
  public Visibility RedVisibility { get; } = Visibility.Collapsed;
  public Visibility GreenVisibility { get; } = Visibility.Collapsed;
  public Visibility ColorlessVisibility { get; } = Visibility.Collapsed;
}
