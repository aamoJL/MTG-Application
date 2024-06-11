using Microsoft.UI.Xaml;
using MTGApplication.General.Models.Card;
using MTGApplication.General.Views;
using System.Windows.Input;

namespace MTGApplication.Features.DeckEditor.Controls.CardView;
public partial class DeckEditorCardViewBase : BasicCardView<DeckEditorMTGCard>
{
  public static readonly DependencyProperty DeleteButtonClickProperty =
      DependencyProperty.Register(nameof(DeleteButtonClick), typeof(ICommand), typeof(DeckEditorCardViewBase),
        new PropertyMetadata(default(ICommand)));

  public static readonly DependencyProperty CountChangeCommandProperty =
      DependencyProperty.Register(nameof(CountChangeCommand), typeof(ICommand), typeof(DeckEditorCardViewBase),
        new PropertyMetadata(default(ICommand)));

  public static readonly DependencyProperty ChangePrintCommandProperty =
      DependencyProperty.Register(nameof(ChangePrintCommand), typeof(ICommand), typeof(DeckEditorCardViewBase),
        new PropertyMetadata(default(ICommand)));

  public ICommand DeleteButtonClick
  {
    get => (ICommand)GetValue(DeleteButtonClickProperty);
    set => SetValue(DeleteButtonClickProperty, value);
  }
  public ICommand CountChangeCommand
  {
    get => (ICommand)GetValue(CountChangeCommandProperty);
    set => SetValue(CountChangeCommandProperty, value);
  }
  public ICommand ChangePrintCommand
  {
    get => (ICommand)GetValue(ChangePrintCommandProperty);
    set => SetValue(ChangePrintCommandProperty, value);
  }
}