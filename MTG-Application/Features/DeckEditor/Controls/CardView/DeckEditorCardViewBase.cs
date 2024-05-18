using Microsoft.UI.Xaml;
using MTGApplication.General.Models.Card;
using MTGApplication.General.Views;
using System.Windows.Input;

namespace MTGApplication.Features.DeckEditor;
public partial class DeckEditorCardViewBase : BasicCardView
{
  public static readonly DependencyProperty DeleteButtonClickProperty =
      DependencyProperty.Register(nameof(DeleteButtonClick), typeof(ICommand), typeof(DeckEditorCardViewBase),
        new PropertyMetadata(default(ICommand)));

  public static readonly DependencyProperty OnModelPropertyChangedCommandProperty =
      DependencyProperty.Register(nameof(OnModelPropertyChangedCommand), typeof(ICommand), typeof(DeckEditorCardViewBase),
        new PropertyMetadata(default(ICommand)));

  public ICommand DeleteButtonClick
  {
    get => (ICommand)GetValue(DeleteButtonClickProperty);
    set => SetValue(DeleteButtonClickProperty, value);
  }
  public ICommand OnModelPropertyChangedCommand
  {
    get => (ICommand)GetValue(OnModelPropertyChangedCommandProperty);
    set => SetValue(OnModelPropertyChangedCommandProperty, value);
  }

  protected override void OnModelChanging(MTGCard oldValue)
  {
    base.OnModelChanging(oldValue);

    if (oldValue != null) oldValue.PropertyChanged -= Model_PropertyChanged;
  }

  protected override void OnModelChanged(MTGCard newValue)
  {
    base.OnModelChanged(newValue);

    if (newValue != null) newValue.PropertyChanged += Model_PropertyChanged;
  }

  protected void Model_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    => OnModelPropertyChangedCommand?.Execute(null);
}
