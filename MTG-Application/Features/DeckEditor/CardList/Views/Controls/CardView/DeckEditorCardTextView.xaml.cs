using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using MTGApplication.General.Models;
using System;
using static MTGApplication.Features.DeckEditor.CardList.UseCases.CardListViewModelCommands;

namespace MTGApplication.Features.DeckEditor.CardList.Views.Controls.CardView;

public partial class DeckEditorCardTextView : DeckEditorCardViewBase
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

  private IRelayCommand<string>? ChangeCardTagCommand => field ??= new RelayCommand<string>((tag) =>
  {
    ContainerElement.ContextFlyout?.Hide();

    CardTag? cardTag = null;

    if (tag != null && Enum.TryParse(tag, out CardTag parsedTag))
      cardTag = parsedTag;

    if (Model.CardTag == cardTag)
      return;

    var args = new CardTagChangeArgs(Model, cardTag);

    if (Model.ChangeCardTagCommand?.CanExecute(args) is true)
      Model.ChangeCardTagCommand.Execute(args);
  });

  private void NumberBox_ValueChanged(NumberBox _, NumberBoxValueChangedEventArgs e)
  {
    if (e.NewValue == Model.Count)
      return;

    var args = new CardCountChangeArgs(Model, (int)e.NewValue);

    if (Model.ChangeCountCommand?.CanExecute(args) is true)
      Model.ChangeCountCommand.Execute(args);
  }
}
