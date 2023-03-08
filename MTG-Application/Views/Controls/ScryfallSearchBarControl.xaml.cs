using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Windows.Input;

namespace MTGApplication.Views.Controls
{
  public enum MTGSearchFormatTypes
  {
    Any, Modern, Standard, Commander,
  }
  public enum MTGSearchUniqueTypes
  {
    Cards, Prints, Art
  }
  public enum MTGSearchOrderTypes
  {
    Released, Set, CMC, Name, Rarity, Color, Eur
  }
  public enum MTGSearchDirectionTypes
  {
    Asc, Desc
  }
  
  [ObservableObject]
  public sealed partial class ScryfallSearchBarControl : UserControl
  {
    public ICommand SearchSubmitCommand { get; set; }
    public string SearchQuery
    {
      get { return (string)GetValue(SearchQueryProperty); }
      set { SetValue(SearchQueryProperty, value); }
    }

    public static readonly DependencyProperty SearchQueryProperty =
        DependencyProperty.Register("SearchQuery", typeof(string), typeof(ScryfallSearchBarControl), new PropertyMetadata(null));

    [ObservableProperty]
    private string searchText;
    [ObservableProperty]
    private MTGSearchFormatTypes searchFormat;
    [ObservableProperty]
    private MTGSearchUniqueTypes searchUnique;
    [ObservableProperty]
    private MTGSearchOrderTypes searchOrder;
    [ObservableProperty]
    private MTGSearchDirectionTypes searchDirection;

    public ScryfallSearchBarControl()
    {
      this.InitializeComponent();

      PropertyChanged += delegate
      {
        SearchQuery = searchText == "" ? "" :
        $"{searchText}+" +
        $"unique:{SearchUnique}+" +
        $"order:{SearchOrder}+" +
        $"direction:{SearchDirection}+" +
        $"format:{SearchFormat}";
      };
    }

    private void SearchButton_Click(object sender, RoutedEventArgs e)
    {
      // Select the search box text so the user doesn't need to click the search box again to write the next search.
      ScryfallSearchBox.Focus(FocusState.Programmatic);
      ScryfallSearchBox.SelectAll();
    }
  }
}
