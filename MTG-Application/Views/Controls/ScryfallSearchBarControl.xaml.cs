using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Windows.Input;

namespace MTGApplication.Views.Controls;

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

/// <summary>
/// Control that returns formatted search query for <see cref="API.ScryfallAPI"/>
/// </summary>
[ObservableObject]
public sealed partial class ScryfallSearchBarControl : UserControl
{
  public ScryfallSearchBarControl()
  {
    InitializeComponent();

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

  #region Properties
  [ObservableProperty] private string searchText;
  [ObservableProperty] private MTGSearchFormatTypes searchFormat;
  [ObservableProperty] private MTGSearchUniqueTypes searchUnique;
  [ObservableProperty] private MTGSearchOrderTypes searchOrder;
  [ObservableProperty] private MTGSearchDirectionTypes searchDirection;

  public ICommand SearchSubmitCommand { get; set; }
  public string SearchQuery
  {
    get => (string)GetValue(SearchQueryProperty);
    set => SetValue(SearchQueryProperty, value);
  }
  #endregion

  #region Dependency Properties
  public static readonly DependencyProperty SearchQueryProperty =
      DependencyProperty.Register("SearchQuery", typeof(string), typeof(ScryfallSearchBarControl), new PropertyMetadata(null));
  #endregion

  private void SearchButton_Click(object sender, RoutedEventArgs e)
  {
    // Select the search box text so the user doesn't need to click the search box again to write the next search.
    ScryfallSearchBox.Focus(FocusState.Programmatic);
    ScryfallSearchBox.SelectAll();
  }
}
