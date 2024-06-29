using Microsoft.UI.Xaml.Controls;
using MTGApplication.General.Views.Dialogs.Controls;

namespace MTGApplication.Features.CardCollectionEditor.CardCollectionList.Views.Controls;

public sealed partial class CollectionListContentDialog : StringStringDialog
{
  public CollectionListContentDialog(string title) : base(title)
  {
    InitializeComponent();

    PrimaryButtonText = "Add";
    SecondaryButtonText = string.Empty;

    Loaded += CollectionListContentDialog_Loaded;
  }

  public string NameInputText { get; set; }
  public string QueryInputText { get; set; }

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
