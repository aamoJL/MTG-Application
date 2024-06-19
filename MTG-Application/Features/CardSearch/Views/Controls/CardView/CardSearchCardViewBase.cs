using Microsoft.UI.Xaml;
using MTGApplication.General.Models;
using MTGApplication.General.Views.Controls;
using System.Windows.Input;

namespace MTGApplication.Features.CardSearch.Views.Controls.CardView;
public partial class CardSearchCardViewBase : BasicCardView<MTGCard>
{
  public static readonly DependencyProperty ShowPrintsCommandProperty =
      DependencyProperty.Register(nameof(ShowPrintsCommand), typeof(ICommand), typeof(CardSearchCardViewBase),
        new PropertyMetadata(default(ICommand)));

  public ICommand ShowPrintsCommand
  {
    get => (ICommand)GetValue(ShowPrintsCommandProperty);
    set => SetValue(ShowPrintsCommandProperty, value);
  }
}