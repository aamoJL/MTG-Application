using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using MTGApplication.API;
using MTGApplication.Models.Structs;
using MTGApplication.ViewModels;
using System;
using Windows.ApplicationModel.DataTransfer;

namespace MTGApplication.Views.Pages;

/// <summary>
/// Page for EDHREC card search
/// </summary>
[ObservableObject]
public sealed partial class EDHRECSearchPage : Page
{
  public EDHRECSearchPage() => InitializeComponent();

  public static readonly DependencyProperty CommanderThemesProperty =
      DependencyProperty.Register(nameof(CommanderThemes), typeof(CommanderTheme[]), typeof(EDHRECCommanderAPI), new PropertyMetadata(Array.Empty<CommanderTheme>()));

  public EDHRECSearchViewModel SearchViewModel { get; } = new(new EDHRECCommanderAPI(), new ScryfallAPI());
  public CommanderTheme[] CommanderThemes
  {
    get => (CommanderTheme[])GetValue(CommanderThemesProperty);
    set
    {
      SetValue(CommanderThemesProperty, value);
      SearchViewModel.CommanderThemes = value;
    }
  }

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
