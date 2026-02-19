using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace MTGApplication.Features.DeckEditor.Views.Controls.CardView;

public partial class DeckEditorCardImageView : DeckEditorCardViewBase
{
  public DeckEditorCardImageView() : base() => InitializeComponent();

  public UIElement ImageElement => CardImageElement;

  protected override void DeleteClick()
  {
    ImageElement.ContextFlyout?.Hide();

    base.DeleteClick();
  }

  protected override void ChangeTagClick(string? tag)
  {
    ImageElement.ContextFlyout?.Hide();

    base.ChangeTagClick(tag);
  }

  protected override void NumberBox_ValueChanged(NumberBox sender, NumberBoxValueChangedEventArgs e)
    => base.NumberBox_ValueChanged(sender, e);
}