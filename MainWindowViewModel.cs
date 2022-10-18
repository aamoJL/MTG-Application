using MTGApplication.ViewModels;

namespace MTGApplication
{
  public class MainWindowViewModel : ViewModelBase
  {
    // TODO: Use generic API interface instead of Scryfall
    public MainWindowViewModel() { }

    public readonly MTGCardCollectionViewModel ScryfallCardViewModels = new(new());
    public readonly MTGCardCollectionViewModel CollectionViewModel = new(new());

    private MTGCardViewModel previewCardViewModel;
    
    public MTGCardViewModel PreviewCardViewModel
    {
      get => previewCardViewModel;
      set
      {
        previewCardViewModel = value;
        OnPropertyChanged(nameof(PreviewCardViewModel));
      }
    }
    public string SearchQuery => SearchText == "" ? "" :
        $"{SearchText}+" +
        $"unique:{SearchUnique}+" +
        $"order:{SearchOrder}+" +
        $"direction:{SearchDirection}+" +
        $"format:{SearchFormat}";

    // TODO: Move initialization to XAML - *Could not get binding to work*
    #region Scryfall search parameters
    public string SearchText { get; set; } = "";
    public string SearchFormat { get; set; } = "Any";
    public string SearchUnique { get; set; } = "Cards";
    public string SearchOrder { get; set; } = "Released"; 
    public string SearchDirection { get; set; } = "Asc";
    #endregion
  }
}
