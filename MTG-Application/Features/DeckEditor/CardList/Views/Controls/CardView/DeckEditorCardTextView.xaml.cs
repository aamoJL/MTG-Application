using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using static MTGApplication.Features.DeckEditor.CardList.UseCases.CardListViewModelCommands.ChangeCardCount;

namespace MTGApplication.Features.DeckEditor.CardList.Views.Controls.CardView;
public sealed partial class DeckEditorCardTextView : DeckEditorCardViewBase
{
  public static readonly DependencyProperty SetIconVisibleProperty =
      DependencyProperty.Register(nameof(SetIconVisible), typeof(bool), typeof(DeckEditorCardTextView), new PropertyMetadata(true));

  public static readonly DependencyProperty TypeLineVisibleProperty =
      DependencyProperty.Register(nameof(TypeLineVisible), typeof(bool), typeof(DeckEditorCardTextView), new PropertyMetadata(true));

  public DeckEditorCardTextView() => InitializeComponent();

  public bool SetIconVisible
  {
    get => (bool)GetValue(SetIconVisibleProperty);
    set => SetValue(SetIconVisibleProperty, value);
  }
  public bool TypeLineVisible
  {
    get => (bool)GetValue(TypeLineVisibleProperty);
    set => SetValue(TypeLineVisibleProperty, value);
  }

  private void NumberBox_ValueChanged(NumberBox sender, NumberBoxValueChangedEventArgs e)
  {
    if (e.NewValue == Model.Count) return;

    var args = new CardCountChangeArgs(Model, (int)e.NewValue);

    if (CountChangeCommand?.CanExecute(args) is true) CountChangeCommand.Execute(args);
  }
}
