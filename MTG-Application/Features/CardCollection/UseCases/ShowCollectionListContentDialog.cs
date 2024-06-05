using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using MTGApplication.General.Views.Dialogs;
using System;
using System.Threading.Tasks;
using static MTGApplication.General.Services.ConfirmationService.DialogService;

namespace MTGApplication.Features.CardCollection;

public partial class CardCollectionViewDialogs
{
  public class ShowCollectionListContentDialog(DialogWrapper dialogWrapper) : ShowDialogUseCase<(string Name, string Query)?, (string Name, string Query)?>(dialogWrapper)
  {
    protected override async Task<(string Name, string Query)?> ShowDialog(string title, string message, (string Name, string Query)? data)
      => await new CollectionListContentDialog(title)
      {
        PrimaryButtonText = data != null ? "Edit" : "Add",
        NameInputText = data?.Name ?? string.Empty,
        QueryInputText = data?.Query ?? string.Empty,
      }.ShowAsync(DialogWrapper);

    private class CollectionListContentDialog(string title = "") : Dialog<(string Name, string Query)?>(title)
    {
      protected TextBox nameBox;
      protected TextBox searchQueryBox;

      public string NameInputText { get; set; }
      public string QueryInputText { get; set; }

      public override ContentDialog GetDialog(XamlRoot root)
      {
        var dialog = base.GetDialog(root);

        nameBox = new()
        {
          PlaceholderText = "List name...",
          Header = "Name",
          Text = NameInputText,
          IsSpellCheckEnabled = false,
          Margin = new() { Bottom = 10 },
        };
        searchQueryBox = new()
        {
          PlaceholderText = "Search query...",
          Text = QueryInputText,
          Header = new StackPanel()
          {
            Children =
              {
                new TextBlock(){ Text = "Search query" },
                new HyperlinkButton()
                {
                  Content = "syntax?",
                  NavigateUri = new Uri("https://scryfall.com/docs/syntax"),
                  Foreground = new SolidColorBrush((Windows.UI.Color)Application.Current.Resources["SystemAccentColorDark2"]),
                  Padding = new Thickness(5, 0, 5, 0),
                  Margin = new Thickness(5, 0, 5, 0),
                }
              },
            Orientation = Orientation.Horizontal,
          },
          IsSpellCheckEnabled = false,
        };

        nameBox.TextChanged += (sender, args) =>
        {
          dialog.IsPrimaryButtonEnabled = nameBox.Text != string.Empty && searchQueryBox.Text != string.Empty;
          NameInputText = nameBox.Text;
        };
        searchQueryBox.TextChanged += (sender, args) =>
        {
          dialog.IsPrimaryButtonEnabled = nameBox.Text != string.Empty && searchQueryBox.Text != string.Empty;
          QueryInputText = searchQueryBox.Text;
        };

        dialog.Content = new StackPanel()
        {
          Children =
            {
              nameBox,
              searchQueryBox,
            },
          Orientation = Orientation.Vertical,
        };

        dialog.IsPrimaryButtonEnabled = !string.IsNullOrEmpty(nameBox.Text) && !string.IsNullOrEmpty(searchQueryBox.Text);
        dialog.SecondaryButtonText = string.Empty;
        dialog.PrimaryButtonText = "Add";

        return dialog;
      }

      public override (string Name, string Query)? ProcessResult(ContentDialogResult result)
      {
        return result switch
        {
          ContentDialogResult.Primary => (NameInputText, QueryInputText),
          _ => null
        };
      }
    }
  }
}