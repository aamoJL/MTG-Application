using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using MTGApplication.Features.CardSearch.ViewModels;
using MTGApplication.General.Models;
using MTGApplication.General.Views.Dialogs.Controls;
using MTGApplication.General.Views.DragAndDrop;
using System.Linq;
using Windows.ApplicationModel.DataTransfer;
using static MTGApplication.General.Services.ConfirmationService.DialogService;

namespace MTGApplication.Features.CardSearch.Views;
public sealed partial class CardSearchPage : Page
{
  public CardSearchPage()
  {
    InitializeComponent();

    Loaded += CardSearchPage_Loaded;
  }

  public CardSearchViewModel ViewModel { get; } = new(App.MTGCardImporter);
  public ListViewDragAndDrop<MTGCard> CardDragAndDrop { get; } = new(itemToArgsConverter: (item) => { return new CardMoveArgs(item); }) { AcceptMove = false };

  private void CardSearchPage_Loaded(object sender, RoutedEventArgs e)
  {
    Loaded -= CardSearchPage_Loaded;

    ViewModel.Confirmers.ShowCardPrintsConfirmer.OnConfirm = async (msg) =>
    {
      Application.Current.Resources.TryGetValue("MTGPrintGridViewItemTemplate", out var template);

      return (await new DialogWrapper(XamlRoot).ShowAsync(new GridViewDialog(
        title: msg.Title,
        items: msg.Data.ToArray(),
        itemTemplate: (DataTemplate)template)
      {
        PrimaryButtonText = string.Empty,
        CloseButtonText = "Close",
        CanDragItems = true,
        OnItemDragStarting = (e) =>
        {
          if (e.Items[0] is MTGCard card)
          {
            DragAndDrop<CardMoveArgs>.Item = new(card);
            e.Data.RequestedOperation = DataPackageOperation.Copy;
          }
        }
      })) as MTGCard;
    };
  }
}