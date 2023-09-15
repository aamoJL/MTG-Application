using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml.Controls;
using MTGApplication.API;
using MTGApplication.Models.Structs;
using MTGApplication.ViewModels;
using Windows.ApplicationModel.DataTransfer;

namespace MTGApplication.Views.Pages;

/// <summary>
/// Page for EDHREC card search
/// </summary>
[ObservableObject]
public sealed partial class EDHRECSearchPage : Page
{
  public EDHRECSearchPage(CommanderTheme[] themes)
  {
    InitializeComponent();
    SearchViewModel = new(new EDHRECCommanderAPI(), new ScryfallAPI()) { CommanderThemes = themes };
  }

  [ObservableProperty] private double searchDesiredItemWidth = 250;

  public EDHRECSearchViewModel SearchViewModel { get; }

  private void CardView_DragItemsStarting(object sender, DragItemsStartingEventArgs e)
  {
    if (e.Items[0] is MTGCardViewModel vm)
    {
      e.Data.SetText(vm.Model.ToJSON());
      e.Data.RequestedOperation = DataPackageOperation.Copy;
    }
  }
}
