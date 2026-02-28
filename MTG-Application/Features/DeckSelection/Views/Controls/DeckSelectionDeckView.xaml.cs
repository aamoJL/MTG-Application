using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using MTGApplication.Features.DeckSelection.Models;
using System.ComponentModel;

namespace MTGApplication.Features.DeckSelection.Views.Controls;

public sealed partial class DeckSelectionDeckView : UserControl, INotifyPropertyChanged
{
  public DeckSelectionDeckView()
  {
    InitializeComponent();

    DataContextChanged += DeckSelectionDeckView_DataContextChanged;
  }

  public Visibility WhiteVisibility
  {
    get;
    set
    {
      if (field != value)
      {
        field = value;
        PropertyChanged?.Invoke(this, new(nameof(WhiteVisibility)));
      }
    }
  }
  public Visibility BlueVisibility
  {
    get;
    set
    {
      if (field != value)
      {
        field = value;
        PropertyChanged?.Invoke(this, new(nameof(BlueVisibility)));
      }
    }
  }
  public Visibility BlackVisibility
  {
    get;
    set
    {
      if (field != value)
      {
        field = value;
        PropertyChanged?.Invoke(this, new(nameof(BlackVisibility)));
      }
    }
  }
  public Visibility RedVisibility
  {
    get;
    set
    {
      if (field != value)
      {
        field = value;
        PropertyChanged?.Invoke(this, new(nameof(RedVisibility)));
      }
    }
  }
  public Visibility GreenVisibility
  {
    get;
    set
    {
      if (field != value)
      {
        field = value;
        PropertyChanged?.Invoke(this, new(nameof(GreenVisibility)));
      }
    }
  }
  public Visibility ColorlessVisibility
  {
    get;
    set
    {
      if (field != value)
      {
        field = value;
        PropertyChanged?.Invoke(this, new(nameof(ColorlessVisibility)));
      }
    }
  }

  public event PropertyChangedEventHandler? PropertyChanged;

  private void DeckSelectionDeckView_DataContextChanged(FrameworkElement _, DataContextChangedEventArgs __)
  {
    if (DataContext is not DeckSelectionDeck deck)
      return;

    WhiteVisibility = deck.Colors.Contains(General.Models.MTGCardInfo.ColorTypes.W) ? Visibility.Visible : Visibility.Collapsed;
    BlueVisibility = deck.Colors.Contains(General.Models.MTGCardInfo.ColorTypes.U) ? Visibility.Visible : Visibility.Collapsed;
    BlackVisibility = deck.Colors.Contains(General.Models.MTGCardInfo.ColorTypes.B) ? Visibility.Visible : Visibility.Collapsed;
    RedVisibility = deck.Colors.Contains(General.Models.MTGCardInfo.ColorTypes.R) ? Visibility.Visible : Visibility.Collapsed;
    GreenVisibility = deck.Colors.Contains(General.Models.MTGCardInfo.ColorTypes.G) ? Visibility.Visible : Visibility.Collapsed;
    ColorlessVisibility = deck.Colors.Contains(General.Models.MTGCardInfo.ColorTypes.C) ? Visibility.Visible : Visibility.Collapsed;
  }
}
