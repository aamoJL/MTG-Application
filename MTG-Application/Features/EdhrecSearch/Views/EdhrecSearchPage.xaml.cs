using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using MTGApplication.Features.EdhrecSearch.ViewModels;
using MTGApplication.General.Models;
using MTGApplication.General.Services.ConfirmationService;
using MTGApplication.General.Services.Importers.CardImporter;
using MTGApplication.General.Views.Dialogs.Controls;
using MTGApplication.General.Views.DragAndDrop;
using MTGApplication.General.Views.Styles.Templates;
using System;
using System.Linq;
using Windows.ApplicationModel.DataTransfer;
using static MTGApplication.General.Services.NotificationService.NotificationService;

namespace MTGApplication.Features.EdhrecSearch.Views;

public sealed partial class EdhrecSearchPage : Page
{
  public EdhrecSearchPage() => InitializeComponent();

  public EdhrecSearchPageViewModel ViewModel => field ??= new()
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
          OnItemDragStarting = (e) =>
          {
            if (e.Items.FirstOrDefault() is not MTGCard dragItem)
            {
              RaiseNotification(this, new(NotificationType.Error, "No items to drag"));
              e.Cancel = true;
              return;
            }

            e.Data.RequestedOperation = DataPackageOperation.Copy;
            e.Data.Properties.Add(nameof(CardDragArgs), new CardDragArgs(dragItem, origin: this));
          }
        });
      },
    },
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

  protected override void OnNavigatedTo(NavigationEventArgs e)
  {
    base.OnNavigatedTo(e);

    if (e.Parameter is CommanderTheme[] themes)
      ViewModel.CommanderThemes = themes;
  }

  private void Notifier_OnNotifyEvent(object? _, Notification e)
    => RaiseNotification(this, e);
}
