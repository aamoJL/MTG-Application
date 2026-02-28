using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using MTGApplication.General.Views.Controls;

namespace MTGApplication.Features.DeckEditor.Views.Controls.CardView;

public partial class DeckEditorCardTextView : DeckEditorCardViewBase
{
  public static readonly DependencyProperty HoverPreviewEnabledProperty =
      DependencyProperty.Register(nameof(HoverPreviewEnabled), typeof(bool), typeof(DeckEditorCardTextView), new PropertyMetadata(false));

  public static readonly DependencyProperty SetIconVisibleProperty =
      DependencyProperty.Register(nameof(SetIconVisible), typeof(bool), typeof(DeckEditorCardTextView), new PropertyMetadata(true));

  public static readonly DependencyProperty TypeLineVisibleProperty =
      DependencyProperty.Register(nameof(TypeLineVisible), typeof(bool), typeof(DeckEditorCardTextView), new PropertyMetadata(true));

  public DeckEditorCardTextView() : base() => InitializeComponent();

  public bool HoverPreviewEnabled
  {
    get => (bool)GetValue(HoverPreviewEnabledProperty);
    set => SetValue(HoverPreviewEnabledProperty, value);
  }
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

  protected override void DeleteClick()
  {
    ContainerElement.ContextFlyout?.Hide();

    base.DeleteClick();
  }

  protected override void ChangeTagClick(string? tag)
  {
    ContainerElement.ContextFlyout?.Hide();

    base.ChangeTagClick(tag);
  }

  protected override void OnPointerMoved(PointerRoutedEventArgs e)
  {
    base.OnPointerMoved(e);

    if (!HoverPreviewEnabled) return;

    HoverPreviewUpdate(this, e);
  }

  protected override void OnPointerExited(PointerRoutedEventArgs e)
  {
    base.OnPointerExited(e);

    if (!HoverPreviewEnabled) return;

    CardPreview.Change(this, new(XamlRoot) { Uri = string.Empty });
  }

  protected virtual void HoverPreviewUpdate(FrameworkElement _, PointerRoutedEventArgs e)
  {
    var point = e.GetCurrentPoint(null).Position;

    CardPreview.Change(this, new(XamlRoot)
    {
      Uri = SelectedFaceUri,
      Coordinates = new((float)point.X, (float)point.Y)
    });
  }

  protected override void NumberBox_ValueChanged(NumberBox sender, NumberBoxValueChangedEventArgs e)
    => base.NumberBox_ValueChanged(sender, e);
}
