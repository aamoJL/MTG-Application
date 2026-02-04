using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using MTGApplication.Features.CardSearch.UseCases;
using MTGApplication.Features.CardSearch.ViewModels;
using MTGApplication.General.Models;
using MTGApplication.General.Views.DragAndDrop;
using static MTGApplication.General.Services.NotificationService.NotificationService;

namespace MTGApplication.Features.CardSearch.Views;

public sealed partial class CardSearchPage : Page
{
  public CardSearchPage() => InitializeComponent();

  public CardSearchPageViewModel ViewModel => field ??= new(App.MTGCardImporter)
  {
    Notifier = Notifier,
    ConfirmCardPrints_UC = async (msg) => await new ShowCardPrints(XamlRoot).Execute(msg),
  };
  public ListViewDragAndDrop<MTGCard> CardDragAndDrop { get; } = new(itemToArgsConverter: (item) => new(item))
  {
    AcceptMove = false
  };

  private Notifier Notifier
  {
    get => field ?? (Notifier = new());
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