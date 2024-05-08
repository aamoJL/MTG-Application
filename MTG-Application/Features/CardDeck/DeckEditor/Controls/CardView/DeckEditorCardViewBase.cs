using Microsoft.UI.Xaml;
using MTGApplication.General.Models.Card;
using System.Windows.Input;

namespace MTGApplication.Features.CardDeck;
public partial class DeckEditorCardViewBase : BasicCardView
{
  public static readonly DependencyProperty OnDeleteCommandProperty =
      DependencyProperty.Register(nameof(OnDeleteCommand), typeof(ICommand), typeof(DeckEditorCardViewBase),
        new PropertyMetadata(default(ICommand)));

  public ICommand OnDeleteCommand
  {
    get => (ICommand)GetValue(OnDeleteCommandProperty);
    set => SetValue(OnDeleteCommandProperty, value);
  }
}
