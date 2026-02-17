using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using MTGApplication.Features.DeckEditor.Models;
using MTGApplication.General.Models;
using System;

namespace MTGApplication.Features.DeckEditor.Views.Controls.CardView;

public partial class DeckEditorCardImageView : DeckEditorCardViewBase
{
  public DeckEditorCardImageView() : base() => InitializeComponent();

  public UIElement ImageElement => CardImageElement;

  [RelayCommand]
  private void Delete(DeckEditorMTGCard? card)
  {
    ImageElement.ContextFlyout?.Hide();

    if (DeleteButtonClick?.CanExecute(card) is true)
      DeleteButtonClick.Execute(card);
  }

  [RelayCommand]
  private void ChangeCardTag(string? tag)
  {
    ImageElement.ContextFlyout?.Hide();

    CardTag? cardTag = null;

    if (tag != null && Enum.TryParse(tag, out CardTag parsedTag))
      cardTag = parsedTag;

    if (Model.CardTag == cardTag)
      return;

    if (Model.ChangeTagCommand?.CanExecute(cardTag) is true)
      Model.ChangeTagCommand.Execute(cardTag);
  }

  private void NumberBox_ValueChanged(NumberBox _, NumberBoxValueChangedEventArgs e)
  {
    if (e.NewValue == Model.Count)
      return;

    if (Model.ChangeCountCommand?.CanExecute((int)e.NewValue) is true)
      Model.ChangeCountCommand.Execute((int)e.NewValue);
  }
}