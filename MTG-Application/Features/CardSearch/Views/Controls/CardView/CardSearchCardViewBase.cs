using Microsoft.UI.Xaml;
using MTGApplication.General.Models.Card;
using MTGApplication.General.Views;
using System.Windows.Input;

namespace MTGApplication.Features.CardSearch.Controls.CardView;
public partial class CardSearchCardViewBase : BasicCardView<DeckEditorMTGCard>
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