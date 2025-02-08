using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using MTGApplication.Features.DeckEditor.Editor.Models;
using static MTGApplication.Features.DeckEditor.CardList.UseCases.CardListViewModelCommands;

namespace MTGApplication.Features.DeckEditor.CardList.Views.Controls.CardView;
public partial class DeckEditorCardImageView : DeckEditorCardViewBase
{
  public DeckEditorCardImageView() => InitializeComponent();

  public UIElement ImageElement => CardImageElement;

  private void NumberBox_ValueChanged(NumberBox _, NumberBoxValueChangedEventArgs e)
  {
    if (e.NewValue == Model.Count)
      return;

    var args = new CardCountChangeArgs(Model, (int)e.NewValue);

    if (Model.ChangeCountCommand?.CanExecute(args) is true)
      Model.ChangeCountCommand.Execute(args);
  }

  private IRelayCommand<DeckEditorMTGCard>? DeleteCommand => field ??= new RelayCommand<DeckEditorMTGCard>((card) =>
  {
    ImageElement?.ContextFlyout?.Hide();

    if (DeleteButtonClick?.CanExecute(card) is true)
      DeleteButtonClick?.Execute(card);
  });
}