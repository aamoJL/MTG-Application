using Microsoft.UI.Xaml;
using System.Windows.Input;

namespace MTGApplication.Features.DeckEditor;
public sealed partial class CommanderTextView : DeckEditorCardViewBase
{
  public static readonly DependencyProperty PrefixTextProperty =
      DependencyProperty.Register(nameof(PrefixText), typeof(string), typeof(CommanderTextView), new PropertyMetadata(string.Empty));

  public static readonly DependencyProperty DragCopyCaptionTextProperty =
      DependencyProperty.Register(nameof(DragCopyCaptionText), typeof(string), typeof(CommanderTextView), new PropertyMetadata(string.Empty, OnDependencyPropertyChangedCallback));

  public static readonly DependencyProperty DragMoveCaptionTextProperty =
      DependencyProperty.Register(nameof(DragMoveCaptionText), typeof(string), typeof(CommanderTextView), new PropertyMetadata(string.Empty, OnDependencyPropertyChangedCallback));

  public static readonly DependencyProperty EdhrecButtonClickProperty =
      DependencyProperty.Register(nameof(EdhrecButtonClick), typeof(ICommand), typeof(CommanderTextView),
        new PropertyMetadata(default(ICommand)));

  public CommanderTextView()
  {
    InitializeComponent();

    DragAndDrop.IsContentVisible = false;
  }

  public string PrefixText
  {
    get => (string)GetValue(PrefixTextProperty);
    set => SetValue(PrefixTextProperty, value);
  }

  public string DragCopyCaptionText
  {
    get => (string)GetValue(DragCopyCaptionTextProperty);
    set => SetValue(DragCopyCaptionTextProperty, value);
  }

  public string DragMoveCaptionText
  {
    get => (string)GetValue(DragMoveCaptionTextProperty);
    set => SetValue(DragMoveCaptionTextProperty, value);
  }

  public ICommand EdhrecButtonClick
  {
    get => (ICommand)GetValue(EdhrecButtonClickProperty);
    set => SetValue(EdhrecButtonClickProperty, value);
  }

  private static void OnDependencyPropertyChangedCallback(DependencyObject sender, DependencyPropertyChangedEventArgs e)
  {
    if (e.Property.Equals(DragCopyCaptionTextProperty))
      (sender as CommanderTextView).DragAndDrop.CopyCaptionOverride = (string)e.NewValue;
    else if (e.Property.Equals(DragMoveCaptionTextProperty))
      (sender as CommanderTextView).DragAndDrop.MoveCaptionOverride = (string)e.NewValue;
  }
}
