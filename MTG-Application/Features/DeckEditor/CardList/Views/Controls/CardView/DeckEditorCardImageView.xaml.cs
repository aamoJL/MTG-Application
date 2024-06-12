using Microsoft.UI.Xaml.Controls;
using MTGApplication.Features.DeckEditor.CardList.Views.Controls.CardView;
using static MTGApplication.Features.DeckEditor.CardListViewModelCommands.ChangeCardCount;

namespace MTGApplication.Features.DeckEditor;
public sealed partial class DeckEditorCardImageView : DeckEditorCardViewBase
{
  public DeckEditorCardImageView() => InitializeComponent();

  private void NumberBox_ValueChanged(NumberBox sender, NumberBoxValueChangedEventArgs e)
  {
    if (e.NewValue == Model.Count) return;

    var args = new CardCountChangeArgs(Model, (int)e.NewValue);

    if (CountChangeCommand?.CanExecute(args) is true) CountChangeCommand.Execute(args);
  }
}