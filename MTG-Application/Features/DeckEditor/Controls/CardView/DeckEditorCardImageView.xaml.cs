using Microsoft.UI.Xaml.Controls;
using static MTGApplication.Features.DeckEditor.CardListViewModel;

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