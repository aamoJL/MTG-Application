using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using MTGApplication.API;
using MTGApplication.Models.Structs;
using MTGApplication.Services;
using MTGApplication.ViewModels;
using Windows.ApplicationModel.DataTransfer;

namespace MTGApplication.Views.Pages;

/// <summary>
/// Page for EDHREC card search
/// </summary>
[ObservableObject]
public sealed partial class EDHRECSearchPage : Page
{
  public EDHRECSearchPage(CommanderTheme[] themes, FrameworkElement dialogRoot = null)
  {
    InitializeComponent();
    var dialogService = new DialogService();
    SearchViewModel = new(new EDHRECCommanderAPI(), new ScryfallAPI(), dialogService) { CommanderThemes = themes };

    // Set dialog root
    if (dialogRoot != null)
      dialogRoot.Loaded += (s, e) => { dialogService.XamlRoot = dialogRoot.XamlRoot; };
    else
      Loaded += (s, e) => { dialogService.XamlRoot = XamlRoot; };
  }

  public EDHRECSearchViewModel SearchViewModel { get; }

  [ObservableProperty] private double searchDesiredItemWidth = 250;

  private void CardView_DragItemsStarting(object sender, DragItemsStartingEventArgs e)
  {
    if (e.Items[0] is MTGCardViewModel vm)
    {
      e.Data.SetText(vm.Model.ToJSON());
      e.Data.RequestedOperation = DataPackageOperation.Copy;
    }
  }
}
