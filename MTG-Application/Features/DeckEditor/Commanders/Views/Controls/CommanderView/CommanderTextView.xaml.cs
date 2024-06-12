using Microsoft.UI.Xaml;
using MTGApplication.Features.DeckEditor.CardList.Views.Controls.CardView;
using MTGApplication.Features.DeckEditor.Commanders.Views.Controls.CommanderView;
using MTGApplication.Features.DeckEditor.Editor.Services;
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

    DragAndDrop = new(new DeckEditorMTGCardCopier())
    {
      OnCopy = async (item) => await OnDropCopy?.ExecuteAsync(item),
      OnRemove = (item) => OnDropRemove?.Execute(item),
      OnExternalImport = async (data) => await OnDropImport?.ExecuteAsync(data),
      OnBeginMoveTo = async (item) => await OnDropBeginMoveTo?.ExecuteAsync(item),
      OnBeginMoveFrom = (item) => OnDropBeginMoveFrom?.Execute(item),
      OnExecuteMove = (item) => OnDropExecuteMove?.Execute(item),
      IsDropContentVisible = false,
    };

    DragStarting += DragAndDrop.DragStarting;
    DropCompleted += DragAndDrop.DropCompleted;
    DragOver += DragAndDrop.DragOver;
    Drop += DragAndDrop.Drop;
  }

  private CommanderTextViewDragAndDrop DragAndDrop { get; }

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
