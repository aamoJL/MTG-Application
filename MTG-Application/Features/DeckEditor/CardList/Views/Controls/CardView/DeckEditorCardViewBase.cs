using Microsoft.UI.Xaml;
using MTGApplication.Features.DeckEditor.Editor.Models;
using MTGApplication.General.Views.Controls;
using System.Windows.Input;

namespace MTGApplication.Features.DeckEditor.CardList.Views.Controls.CardView;
public partial class DeckEditorCardViewBase : BasicCardView<DeckEditorMTGCard>
{
  public static readonly DependencyProperty DeleteButtonClickProperty =
      DependencyProperty.Register(nameof(DeleteButtonClick), typeof(ICommand), typeof(DeckEditorCardViewBase), new PropertyMetadata(default(ICommand)));

  public ICommand? DeleteButtonClick
  {
    get => (ICommand)GetValue(DeleteButtonClickProperty);
    set => SetValue(DeleteButtonClickProperty, value);
  }
}