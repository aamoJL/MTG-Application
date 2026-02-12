using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using MTGApplication.Features.CardSearch.ViewModels.SearchPage;
using MTGApplication.General.Models;
using MTGApplication.General.Services.ConfirmationService;
using MTGApplication.General.Views.Dialogs.Controls;
using MTGApplication.General.Views.DragAndDrop;
using MTGApplication.General.Views.Styles.Templates;
using System;
using System.Linq;
using static MTGApplication.General.Services.NotificationService.NotificationService;

namespace MTGApplication.Features.CardSearch.Views;

public sealed partial class CardSearchPage : Page
{
  public CardSearchPage() => InitializeComponent();

  public CardSearchPageViewModel ViewModel => field ??= new()
  {
    Notifier = Notifier,
    CardConfirmers = new()
    {
      ConfirmCardPrints = async (msg) =>
      {
        ArgumentNullException.ThrowIfNull(XamlRoot);

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
              new DragAndDrop<CardMoveArgs>() { AcceptMove = false }.OnInternalDragStarting(new CardMoveArgs(card), out var operation);

              args.Data.RequestedOperation = operation;
            }
          }
        });
      },
    },
  };
  public ListViewDragAndDrop<MTGCard> CardDragAndDrop { get; } = new(itemToArgsConverter: (item) => new(item))
  {
    AcceptMove = false
  };

  private Notifier Notifier
  {
    get => field ??= Notifier = new();
    set
    {
      if (field == value) return;
      field?.OnNotifyEvent -= Notifier_OnNotifyEvent;
      field = value;
      field?.OnNotifyEvent += Notifier_OnNotifyEvent;
    }
  }

  private void Notifier_OnNotifyEvent(object? _, Notification e)
    => RaiseNotification(this, e);

  private void SearchCardsImageView_GettingFocus(UIElement _, Microsoft.UI.Xaml.Input.GettingFocusEventArgs args)
  {
    // Keep focus on search inputs, if the focused element is the search card list scrollviewer,
    //  so the user does not need to click the search input again after dragging and dropping a card.
    if (args.OldFocusedElement is TextBox)
      args.TryCancel();
  }
}