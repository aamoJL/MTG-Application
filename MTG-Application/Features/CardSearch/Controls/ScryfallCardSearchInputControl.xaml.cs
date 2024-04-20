using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml.Controls;
using MTGApplication.General;
using System.Text;
using System.Windows.Input;

namespace MTGApplication.Features.CardSearch.Controls;
[ObservableObject]
public sealed partial class ScryfallCardSearchInputControl : UserControl
{
  public ScryfallCardSearchInputControl() => InitializeComponent();

  public ScryfallCardSearchControlViewModel ViewModel { get; } = new();

  public ICommand Submit { get; set; }

  private void SubmitButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
  {
    // Select the search box text so the user doesn't need to click the search box again to write the next search.
    SearchTextBox.Focus(Microsoft.UI.Xaml.FocusState.Programmatic);
    SearchTextBox.SelectAll();
  }
}

public partial class ScryfallCardSearchControlViewModel : ViewModelBase
{
  public string SearchQuery
  {
    get
    {
      // Search query needs to be completely empty, if the SearchText is empty,
      // Otherwise the API would return all the available cards matching the other properties
      if (string.IsNullOrEmpty(SearchText)) return string.Empty;

      var sb = new StringBuilder();

      sb.Append($"{SearchText}+");

      if (SearchGameFormat != MTGSearchGameFormat.Any)
        sb.Append($"format:{SearchGameFormat.ToString()}+");

      sb.Append($"order:{SearchOrderProperty.ToString()}+");
      sb.Append($"unique:{SearchCardUniqueness.ToString()}+");
      sb.Append($"direction:{SearchOrderDirection.ToString()}");

      return sb.ToString();
    }
  }

  [ObservableProperty, NotifyPropertyChangedFor(nameof(SearchQuery))] private string searchText;
  [ObservableProperty, NotifyPropertyChangedFor(nameof(SearchQuery))] private MTGSearchGameFormat searchGameFormat;
  [ObservableProperty, NotifyPropertyChangedFor(nameof(SearchQuery))] private MTGSearchCardUniqueness searchCardUniqueness;
  [ObservableProperty, NotifyPropertyChangedFor(nameof(SearchQuery))] private MTGSearchOrderProperty searchOrderProperty;
  [ObservableProperty, NotifyPropertyChangedFor(nameof(SearchQuery))] private MTGSearchOrderDirection searchOrderDirection;
}
