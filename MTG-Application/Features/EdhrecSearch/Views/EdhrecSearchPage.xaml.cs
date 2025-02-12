using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using MTGApplication.Features.EdhrecSearch.ViewModels;
using MTGApplication.General.Models;
using MTGApplication.General.Services.ConfirmationService;
using MTGApplication.General.Services.NotificationService;
using MTGApplication.General.Views.Dialogs.Controls;
using MTGApplication.General.Views.DragAndDrop;
using MTGApplication.General.Views.Styles.Templates;
using System.Linq;
using Windows.ApplicationModel.DataTransfer;
using static MTGApplication.General.Services.Importers.CardImporter.EdhrecImporter;

namespace MTGApplication.Features.EdhrecSearch.Views;
public sealed partial class EdhrecSearchPage : Page
{
  public EdhrecSearchPage() => InitializeComponent();

  public EdhrecSearchPageViewModel ViewModel { get; } = new(App.MTGCardImporter);

  protected override void OnNavigatedTo(NavigationEventArgs e)
  {
    base.OnNavigatedTo(e);

    if (e.Parameter is CommanderTheme[] themes)
      ViewModel.CommanderThemes = themes;

    ViewModel.Confirmers.ShowCardPrintsConfirmer.OnConfirm = async (msg) =>
    {
      Application.Current.Resources.TryGetValue(nameof(MTGPrintGridViewItemTemplate), out var template);

      await DialogService.ShowAsync(XamlRoot, new GridViewDialog(
        title: msg.Title,
        items: msg.Data.ToArray(),
        itemTemplate: (DataTemplate)template)
      {
        PrimaryButtonText = string.Empty,
        CloseButtonText = "Close",
        CanDragItems = true,
        CanSelectItems = false,
        OnItemDragStarting = (e) =>
        {
          if (e.Items.FirstOrDefault() is MTGCard card)
          {
            DragAndDrop<CardMoveArgs>.Item = new(card);
            e.Data.RequestedOperation = DataPackageOperation.Copy;
          }
        }
      });
    };

    NotificationService.RegisterNotifications(ViewModel.Notifier, this);
  }
}
