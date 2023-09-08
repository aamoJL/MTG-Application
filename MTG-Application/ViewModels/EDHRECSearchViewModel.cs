using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml.Controls;
using MTGApplication.Interfaces;
using MTGApplication.Models;
using MTGApplication.Models.Structs;
using MTGApplication.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using static MTGApplication.Services.DialogService;

namespace MTGApplication.ViewModels;

public partial class EDHRECSearchViewModel : ObservableObject
{
  public class EDHRECSearchViewModelDialogs
  {
    public EDHRECSearchViewModelDialogs(DialogService service) => Service = service;

    private DialogService Service { get; }

    public virtual DraggableMTGCardViewModelGridViewDialog GetCardPrintDialog(MTGCardViewModel[] printViewModels)
        => new(Service, "Card prints", "MTGPrintGridViewItemTemplate", "MTGAdaptiveGridViewStyle")
        { Items = printViewModels, SecondaryButtonText = string.Empty, PrimaryButtonText = string.Empty, CloseButtonText = "Close" };

    public class DraggableMTGCardViewModelGridViewDialog : DraggableGridViewDialog<MTGCardViewModel>
    {
      public DraggableMTGCardViewModelGridViewDialog(DialogService service, string title = "", string itemTemplate = "", string gridStyle = "") : base(service, title, itemTemplate, gridStyle) { }

      protected override void DraggableGridViewDialog_DragItemsStarting(object sender, DragItemsStartingEventArgs e)
      {
        if (e.Items[0] is MTGCardViewModel vm)
        {
          e.Data.SetText(vm.Model.ToJSON());
          e.Data.RequestedOperation = DataPackageOperation.Copy | DataPackageOperation.Move;
        }
        base.DraggableGridViewDialog_DragItemsStarting(sender, e);
      }
    }
  }

  public EDHRECSearchViewModel(IMTGCommanderAPI commanderAPI, ICardAPI<MTGCard> cardAPI, DialogService dialogService)
  {
    Dialogs = new(dialogService);
    CommanderAPI = commanderAPI;
    APISearch = new(cardAPI);

    PropertyChanged += EDHRECSearchViewModel_PropertyChanged;
    APISearch.PropertyChanged += APISearch_PropertyChanged;
  }

  private void APISearch_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
  {
    if (e.PropertyName == nameof(APISearch.IsBusy))
    {
      IsBusy = APISearch.IsBusy;
    }
    else if (e.PropertyName == nameof(APISearch.SearchCards))
    {
      // Add print dialog command to the card viewmodel
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

  private async void EDHRECSearchViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
  {
    if (e.PropertyName == nameof(SelectedTheme))
    {
      await APISearch.SearchWithNames(await FetchNewCardNames(SelectedTheme));
    }
  }

  public MTGAPISearch<MTGCardViewModelSource, MTGCardViewModel> APISearch { get; }
  public EDHRECSearchViewModelDialogs Dialogs { get; }

  private IMTGCommanderAPI CommanderAPI { get; }

  [ObservableProperty] private bool isBusy;
  [ObservableProperty] private CommanderTheme[] commanderThemes;
  [ObservableProperty] private CommanderTheme selectedTheme;

  /// <summary>
  /// Shows a dialog with the card's prints
  /// </summary>
  [RelayCommand]
  public async Task ShowPrintsDialog(MTGCard card)
  {
    IsBusy = true;
    var pageUri = card.Info.PrintSearchUri;
    var prints = new List<MTGCard>();

    while (pageUri != string.Empty)
    {
      var result = await APISearch.CardAPI.FetchFromUri(pageUri, paperOnly: true);
      prints.AddRange(result.Found);
      pageUri = result.NextPageUri;
    }

    var printViewModels = prints.Select(x => new MTGCardViewModel(x)).ToArray();
    IsBusy = false;

    await Dialogs.GetCardPrintDialog(printViewModels).ShowAsync();
  }

  /// <summary>
  /// Returns array of card names from the given theme that has been marked as new by the API
  /// </summary>
  private async Task<string[]> FetchNewCardNames(CommanderTheme theme)
  {
    IsBusy = true;
    var result = await CommanderAPI.FetchNewCards(theme.Uri);
    IsBusy = false;
    return result;
  }
}
