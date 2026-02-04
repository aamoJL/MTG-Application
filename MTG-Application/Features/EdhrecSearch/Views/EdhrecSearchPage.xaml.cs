using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using MTGApplication.Features.CardSearch.UseCases;
using MTGApplication.Features.EdhrecSearch.ViewModels;
using static MTGApplication.General.Services.Importers.CardImporter.EdhrecImporter;
using static MTGApplication.General.Services.NotificationService.NotificationService;

namespace MTGApplication.Features.EdhrecSearch.Views;

public sealed partial class EdhrecSearchPage : Page
{
  public EdhrecSearchPage() => InitializeComponent();

  public EdhrecSearchPageViewModel ViewModel => field ??= new(App.MTGCardImporter)
  {
    Notifier = Notifier,
    ConfirmCardPrints_UC = async (msg) => await new ShowCardPrints(XamlRoot).Execute(msg),
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
