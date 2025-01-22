using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using static MTGApplication.Features.DeckEditor.CardList.UseCases.CardListViewModelCommands;

namespace MTGApplication.Features.DeckEditor.CardList.Views.Controls.CardView;
public partial class DeckEditorCardImageView : DeckEditorCardViewBase
{
  public DeckEditorCardImageView() => InitializeComponent();

  public UIElement ImageElement => CardImageElement;

  private void NumberBox_ValueChanged(NumberBox sender, NumberBoxValueChangedEventArgs e)
  {
    if (e.NewValue == Model.Count) return;

    var args = new CardCountChangeArgs(Model, (int)e.NewValue);

    if (CountChangeCommand?.CanExecute(args) is true) CountChangeCommand.Execute(args);
  }
}