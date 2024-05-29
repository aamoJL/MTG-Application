using Microsoft.UI.Xaml;
using MTGApplication.General.Views;
using System.Windows.Input;

namespace MTGApplication.Features.CardCollection.Controls;
public partial class CardCollectionCardViewBase : BasicCardView
{
  public static readonly DependencyProperty ShowPrintsCommandProperty =
      DependencyProperty.Register(nameof(ShowPrintsCommand), typeof(ICommand), typeof(CardCollectionCardViewBase),
        new PropertyMetadata(default(ICommand)));

  public ICommand ShowPrintsCommand
  {
    get => (ICommand)GetValue(ShowPrintsCommandProperty);
    set => SetValue(ShowPrintsCommandProperty, value);
  }
}