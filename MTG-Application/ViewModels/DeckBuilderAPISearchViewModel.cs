﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml.Controls;
using MTGApplication.Interfaces;
using MTGApplication.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using static MTGApplication.Services.DialogService;

namespace MTGApplication.ViewModels;
public partial class DeckBuilderAPISearchViewModel : ViewModelBase
{
  public class DeckBuilderAPISearchViewModelDialogs
  {
    public virtual DraggableMTGCardViewModelGridViewDialog GetCardPrintDialog(MTGCardViewModel[] printViewModels)
        => new("Illustration prints", "MTGPrintGridViewItemTemplate", "MTGAdaptiveGridViewStyle") { Items = printViewModels, SecondaryButtonText = string.Empty, PrimaryButtonText = string.Empty, CloseButtonText = "Close" };

    public class DraggableMTGCardViewModelGridViewDialog : DraggableGridViewDialog<MTGCardViewModel>
    {
      public DraggableMTGCardViewModelGridViewDialog(string title = "", string itemTemplate = "", string gridStyle = "") : base(title, itemTemplate, gridStyle)
      {
      }

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
}
