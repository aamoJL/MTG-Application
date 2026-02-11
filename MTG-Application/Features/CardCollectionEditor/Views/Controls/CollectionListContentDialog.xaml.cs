using Microsoft.UI.Xaml.Controls;
using MTGApplication.General.Views.Dialogs.Controls;

namespace MTGApplication.Features.CardCollectionEditor.Views.Controls;

/// <summary>
/// Base class for <see cref="CollectionListContentDialog"/>, so it can be used in xaml
/// </summary>
/// <param name="title"></param>
public abstract class CollectionListContentDialogBase(string title) : CustomContentDialog<(string, string)>(title);

public sealed partial class CollectionListContentDialog : CollectionListContentDialogBase
{
  public CollectionListContentDialog(string title) : base(title)
  {
    InitializeComponent();

    PrimaryButtonText = "Add";
    SecondaryButtonText = string.Empty;

    Loaded += CollectionListContentDialog_Loaded;
  }

  public string NameInputText { get; set; } = string.Empty;
  public string QueryInputText { get; set; } = string.Empty;

  protected override (string, string) ProcessResult(ContentDialogResult result)
    => result switch
    {
      ContentDialogResult.Primary => (NameInputText, QueryInputText),
      _ => default
    };

  private void CollectionListContentDialog_Loaded(object _, Microsoft.UI.Xaml.RoutedEventArgs e)
  {
    Loaded -= CollectionListContentDialog_Loaded;
    IsPrimaryButtonEnabled = NameInputText != string.Empty && QueryInputText != string.Empty;
  }

  private void NameTextBox_TextChanged(object _, TextChangedEventArgs e)
    => IsPrimaryButtonEnabled = NameInputText != string.Empty && QueryInputText != string.Empty;

  private void QueryTextBox_TextChanged(object _, TextChangedEventArgs e)
    => IsPrimaryButtonEnabled = NameInputText != string.Empty && QueryInputText != string.Empty;
}
