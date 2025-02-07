using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using MTGApplication.Features.DeckEditor.Editor.Models;
using System.Windows.Input;

namespace MTGApplication.Features.DeckEditor.CardList.Views.Controls.CardView;

public sealed partial class DeckEditorCardItemContainer : UserControl
{
  public static readonly DependencyProperty ModelProperty =
      DependencyProperty.Register(nameof(Model), typeof(DeckEditorMTGCard), typeof(DeckEditorCardItemContainer), new PropertyMetadata(default));

  public static readonly DependencyProperty OnDropBeginMoveFromProperty =
      DependencyProperty.Register(nameof(OnDropBeginMoveFrom), typeof(ICommand), typeof(DeckEditorCardItemContainer), new PropertyMetadata(default));

  public static readonly DependencyProperty DeleteButtonClickProperty =
      DependencyProperty.Register(nameof(DeleteButtonClick), typeof(ICommand), typeof(DeckEditorCardItemContainer), new PropertyMetadata(default(ICommand)));

  public static readonly DependencyProperty CountChangeCommandProperty =
      DependencyProperty.Register(nameof(CountChangeCommand), typeof(ICommand), typeof(DeckEditorCardItemContainer), new PropertyMetadata(default(ICommand)));

  public static readonly DependencyProperty ChangePrintCommandProperty =
      DependencyProperty.Register(nameof(ChangePrintCommand), typeof(ICommand), typeof(DeckEditorCardItemContainer), new PropertyMetadata(default(ICommand)));

  public DeckEditorCardItemContainer() => InitializeComponent();

  public DeckEditorMTGCard Model
  {
    get => (DeckEditorMTGCard)GetValue(ModelProperty);
    set => SetValue(ModelProperty, value);
  }

  public ICommand OnDropBeginMoveFrom
  {
    get => (ICommand)GetValue(OnDropBeginMoveFromProperty);
    set => SetValue(OnDropBeginMoveFromProperty, value);
  }
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
