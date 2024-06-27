using Microsoft.UI.Xaml;
using MTGApplication.Features.CardCollectionEditor.CardCollectionList.Models;
using MTGApplication.General.Views.Controls;
using System.Windows.Input;

namespace MTGApplication.Features.CardCollectionEditor.CardCollectionList.Views.Controls;
public partial class CardCollectionCardViewBase : BasicCardView<CardCollectionMTGCard>
{
  public static readonly DependencyProperty ShowPrintsCommandProperty =
      DependencyProperty.Register(nameof(ShowPrintsCommand), typeof(ICommand), typeof(CardCollectionCardViewBase),
        new PropertyMetadata(default(ICommand)));

  public static readonly DependencyProperty OnSwitchOwnershipCommandProperty =
      DependencyProperty.Register(nameof(OnSwitchOwnershipCommand), typeof(ICommand), typeof(CardCollectionCardViewBase),
        new PropertyMetadata(default(ICommand)));

  public static readonly DependencyProperty SelectionModeProperty =
      DependencyProperty.Register(nameof(SelectionMode), typeof(SelectionMode), typeof(CardCollectionCardViewBase),
        new PropertyMetadata(default(SelectionMode)));

  public CardCollectionCardViewBase()
  {
    Tapped += CardCollectionCardViewBase_Tapped;
    DoubleTapped += CardCollectionCardViewBase_DoubleTapped;
  }

  public ICommand ShowPrintsCommand
  {
    get => (ICommand)GetValue(ShowPrintsCommandProperty);
    set => SetValue(ShowPrintsCommandProperty, value);
  }

  public ICommand OnSwitchOwnershipCommand
  {
    get => (ICommand)GetValue(OnSwitchOwnershipCommandProperty);
    set => SetValue(OnSwitchOwnershipCommandProperty, value);
  }

  public SelectionMode SelectionMode
  {
    get => (SelectionMode)GetValue(SelectionModeProperty);
    set => SetValue(SelectionModeProperty, value);
  }

  private void CardCollectionCardViewBase_DoubleTapped(object sender, Microsoft.UI.Xaml.Input.DoubleTappedRoutedEventArgs e)
  {
    if (SelectionMode == SelectionMode.DoubleTap)
      OnSwitchOwnershipCommand?.Execute(Model);
  }

  private void CardCollectionCardViewBase_Tapped(object sender, Microsoft.UI.Xaml.Input.TappedRoutedEventArgs e)
  {
    if (SelectionMode == SelectionMode.SingleTap)
      OnSwitchOwnershipCommand?.Execute(Model);
  }
}

public enum SelectionMode { SingleTap, DoubleTap }