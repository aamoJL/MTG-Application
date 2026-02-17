using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using MTGApplication.General.Models;
using System;

namespace MTGApplication.Features.DeckEditor.Views.Controls.CardView;

public partial class DeckEditorCardTextView : DeckEditorCardViewBase
{
  public static readonly DependencyProperty SetIconVisibleProperty =
      DependencyProperty.Register(nameof(SetIconVisible), typeof(bool), typeof(DeckEditorCardTextView), new PropertyMetadata(true));

  public static readonly DependencyProperty TypeLineVisibleProperty =
      DependencyProperty.Register(nameof(TypeLineVisible), typeof(bool), typeof(DeckEditorCardTextView), new PropertyMetadata(true));

  public DeckEditorCardTextView() : base() => InitializeComponent();

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

  [RelayCommand]
  private void ChangeCardTag(string? tag)
  {
    ContainerElement.ContextFlyout?.Hide();

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
