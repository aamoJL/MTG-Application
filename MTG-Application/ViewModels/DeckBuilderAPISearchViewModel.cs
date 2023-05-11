using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MTGApplication.Interfaces;
using MTGApplication.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static MTGApplication.Services.DialogService;

namespace MTGApplication.ViewModels;
public partial class DeckBuilderAPISearchViewModel : ViewModelBase
{
  public class DeckBuilderAPISearchViewModelDialogs
  {
    public virtual GridViewDialog GetCardPrintDialog(MTGCardViewModel[] printViewModels)
        => new("Illustration prints", "MTGPrintGridViewItemTemplate", "MTGAdaptiveGridViewStyle") { Items = printViewModels, SecondaryButtonText = string.Empty, PrimaryButtonText = string.Empty, CloseButtonText = "Close" };
  }

  public DeckBuilderAPISearchViewModel(ICardAPI<MTGCard> cardAPI)
  {
    APISearch = new(cardAPI);
    APISearch.PropertyChanged += SearchViewModel_PropertyChanged;
  }
  
  [ObservableProperty]
  private bool isBusy;

  public MTGAPISearch<MTGCardViewModelSource, MTGCardViewModel> APISearch { get; }
  public DeckBuilderAPISearchViewModelDialogs Dialogs { get; init; } = new();

  private void SearchViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
  {
    if (e.PropertyName == nameof(APISearch.IsBusy)) { IsBusy = APISearch.IsBusy; }
    if (e.PropertyName == nameof(APISearch.SearchCards))
    {
      APISearch.SearchCards.CollectionChanged += (s, e) =>
      {
        switch (e.Action)
        {
          case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
            var newCard = e.NewItems[0] as MTGCardViewModel;
            newCard.ShowPrintsDialogCommand = ShowPrintsDialogCommand;
            break;
          default: break;
        }
      };
    }
  }

  /// <summary>
  /// Shows a dialog with the card's prints
  /// </summary>
  [RelayCommand]
  public async Task ShowPrintsDialog(MTGCard card)
  {
    var prints = new List<MTGCard>();

    IsBusy = true;
    // Get prints
    var fetchResult = await APISearch.CardAPI.FetchCardsWithParameters($"{card.Info.Name}+unique:prints+game:paper");
    prints.AddRange(fetchResult.Found);
    while (!string.IsNullOrEmpty(fetchResult.NextPageUri))
    {
      fetchResult = await APISearch.CardAPI.FetchFromUri(fetchResult.NextPageUri);
      prints.AddRange(fetchResult.Found);
    }
    var printViewModels = prints.Select(x => new MTGCardViewModel(x)).ToArray();
    IsBusy = false;

    await Dialogs.GetCardPrintDialog(printViewModels).ShowAsync();
  }
}
