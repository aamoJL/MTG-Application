using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Text;
using System.Threading.Tasks;

namespace MTGApplication.Features.CardSearch.Views.Controls.SearchInputControl;

public sealed partial class ScryfallCardSearchInputControl : UserControl
{
  public static readonly DependencyProperty SubmitCommandProperty =
      DependencyProperty.Register(nameof(SubmitCommand), typeof(IAsyncRelayCommand<string>), typeof(ScryfallCardSearchInputControl), new PropertyMetadata(null));

  public ScryfallCardSearchInputControl() => InitializeComponent();

  public ScryfallCardSearchInputControlViewModel ViewModel
  {
    get => field ??= new()
    {
      OnSubmit = (query) =>
      {
        SubmitCommand?.Cancel();
        SubmitCommand?.Execute(query);

        // Select the search box text so the user doesn't need to click the search box again to write the next search.
        SearchTextBox.Focus(FocusState.Programmatic);
        SearchTextBox.SelectAll();
      }
    };
  }

  public IAsyncRelayCommand<string> SubmitCommand
  {
    get => (IAsyncRelayCommand<string>)GetValue(SubmitCommandProperty);
    set => SetValue(SubmitCommandProperty, value);
  }
}

public enum GameFormat { Any, Modern, Standard, Commander }
public enum CardUniqueness { Cards, Prints, Art }
public enum SearchOrderDirection { Asc, Desc }
public enum SearchOrderProperty { Released, Set, CMC, Name, Rarity, Color, Eur }

public partial class ScryfallCardSearchInputControlViewModel : ObservableObject
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

      if (SearchGameFormat != GameFormat.Any)
        sb.AppendJoin('+', $"format:{SearchGameFormat}");

      sb.AppendJoin('+', [
        $"order:{SearchOrderProperty}",
        $"unique:{SearchCardUniqueness}",
        $"direction:{SearchOrderDirection}"]);

      return sb.ToString();
    }
  }
  [ObservableProperty, NotifyPropertyChangedFor(nameof(SearchQuery))] public partial string SearchText { get; set; }
  [ObservableProperty, NotifyPropertyChangedFor(nameof(SearchQuery))] public partial GameFormat SearchGameFormat { get; set; }
  [ObservableProperty, NotifyPropertyChangedFor(nameof(SearchQuery))] public partial CardUniqueness SearchCardUniqueness { get; set; }
  [ObservableProperty, NotifyPropertyChangedFor(nameof(SearchQuery))] public partial SearchOrderProperty SearchOrderProperty { get; set; }
  [ObservableProperty, NotifyPropertyChangedFor(nameof(SearchQuery))] public partial SearchOrderDirection SearchOrderDirection { get; set; }

  public Action<string>? OnSubmit { get; init; }

  [RelayCommand]
  private async Task Submit() => OnSubmit?.Invoke(SearchQuery);
}