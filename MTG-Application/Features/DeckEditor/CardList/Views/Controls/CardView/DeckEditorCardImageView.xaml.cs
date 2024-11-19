using Microsoft.UI.Xaml.Controls;
using static MTGApplication.Features.DeckEditor.CardList.UseCases.CardListViewModelCommands;

namespace MTGApplication.Features.DeckEditor.CardList.Views.Controls.CardView;
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