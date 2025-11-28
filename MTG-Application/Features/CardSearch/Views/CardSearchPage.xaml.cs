using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using MTGApplication.Features.CardSearch.ViewModels;
using MTGApplication.General.Models;
using MTGApplication.General.Services.ConfirmationService;
using MTGApplication.General.Services.NotificationService;
using MTGApplication.General.Views.Dialogs.Controls;
using MTGApplication.General.Views.DragAndDrop;
using MTGApplication.General.Views.Styles.Templates;
using System.Linq;

namespace MTGApplication.Features.CardSearch.Views;
public sealed partial class CardSearchPage : Page
{
  public CardSearchPage()
  {
    InitializeComponent();

    Loaded += CardSearchPage_Loaded;
  }

  public CardSearchViewModel ViewModel { get; } = new(App.MTGCardImporter);
  public ListViewDragAndDrop<MTGCard> CardDragAndDrop { get; } = new(itemToArgsConverter: (item) => new(item))
  {
    AcceptMove = false
  };

  private void CardSearchPage_Loaded(object sender, RoutedEventArgs e)
  {
    Loaded -= CardSearchPage_Loaded;

    ViewModel.Confirmers.ShowCardPrintsConfirmer.OnConfirm = async (msg) =>
    {
      Application.Current.Resources.TryGetValue(nameof(MTGPrintGridViewItemTemplate), out var template);

      await DialogService.ShowAsync(XamlRoot, new GridViewDialog(
        title: msg.Title,
        items: [.. msg.Data],
        itemTemplate: (DataTemplate)template)
      {
        PrimaryButtonText = string.Empty,
        CloseButtonText = "Close",
        CanDragItems = true,
        CanSelectItems = false,
        OnItemDragStarting = (args) =>
        {
          if (args.Items.FirstOrDefault() is MTGCard card)
          {
            CardDragAndDrop.OnInternalDragStarting(new CardMoveArgs(card), out var operation);
            args.Data.RequestedOperation = operation;
          }
        }
      });
    };

    NotificationService.RegisterNotifications(ViewModel.Notifier, this);
  }

  private void SearchCardsImageView_GettingFocus(UIElement sender, Microsoft.UI.Xaml.Input.GettingFocusEventArgs args)
  {
    // Keep focus on search inputs, if the focused element is the search card list scrollviewer,
    //  so the user does not need to click the search input again after dragging and dropping a card.
    if (args.OldFocusedElement is TextBox)
      args.TryCancel();
  }
}