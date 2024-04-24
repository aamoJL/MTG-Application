using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml.Controls;
using MTGApplication.API.CardAPI;
using MTGApplication.General.Services.ConfirmationService;
using MTGApplication.Models;
using MTGApplication.Models.Structs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using static MTGApplication.General.Services.ConfirmationService.DialogService;

namespace MTGApplication.ViewModels;

public partial class EDHRECSearchViewModel : ObservableObject, IDialogNotifier
{
  public EDHRECSearchViewModel(IMTGCommanderAPI commanderAPI, ICardAPI<MTGCard> cardAPI)
  {
    Dialogs = new();
    CommanderAPI = commanderAPI;
    APISearch = new(cardAPI);

    PropertyChanged += EDHRECSearchViewModel_PropertyChanged;
    APISearch.PropertyChanged += APISearch_PropertyChanged;
  }

  #region Properties
  [ObservableProperty] private bool isBusy;
  [ObservableProperty] private CommanderTheme[] commanderThemes;
  [ObservableProperty] private CommanderTheme selectedTheme;

  public MTGAPISearch<MTGCardViewModelSource, MTGCardViewModel> APISearch { get; }
  public EDHRECSearchViewModelDialogs Dialogs { get; }
  private IMTGCommanderAPI CommanderAPI { get; }
  #endregion

  #region IDialogNotifier implementation
  public event EventHandler<DialogEventArgs> OnGetDialogWrapper;

  public DialogWrapper GetDialogWrapper()
  {
    var args = new DialogEventArgs();
    OnGetDialogWrapper?.Invoke(this, args);
    return args.DialogWrapper;
  }
  #endregion

  #region OnPropertyChanged events
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
  #endregion

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

    await Dialogs.GetCardPrintDialog(printViewModels).ShowAsync(GetDialogWrapper());
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

// Dialogs
public partial class EDHRECSearchViewModel
{
  public class EDHRECSearchViewModelDialogs
  {
    public virtual DraggableMTGCardViewModelGridViewDialog GetCardPrintDialog(MTGCardViewModel[] printViewModels)
        => new("Card prints", "MTGPrintGridViewItemTemplate", "MTGAdaptiveGridViewStyle")
        { Items = printViewModels, SecondaryButtonText = string.Empty, PrimaryButtonText = string.Empty, CloseButtonText = "Close" };

    public class DraggableMTGCardViewModelGridViewDialog : DraggableGridViewDialog<MTGCardViewModel>
    {
      public DraggableMTGCardViewModelGridViewDialog(string title = "", string itemTemplate = "", string gridStyle = "") : base(title, itemTemplate, gridStyle) { }

      protected override void DraggableGridViewDialog_DragItemsStarting(ContentDialog dialog, DragItemsStartingEventArgs e)
      {
        if (e.Items[0] is MTGCardViewModel vm)
        {
          e.Data.SetText(vm.Model.ToJSON());
          e.Data.RequestedOperation = DataPackageOperation.Copy | DataPackageOperation.Move;
        }
        base.DraggableGridViewDialog_DragItemsStarting(dialog, e);
      }
    }
  }
}
