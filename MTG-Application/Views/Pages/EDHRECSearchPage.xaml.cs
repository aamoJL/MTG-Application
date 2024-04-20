using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml.Controls;
using MTGApplication.API.CardAPI;
using MTGApplication.Interfaces;
using MTGApplication.Models.Structs;
using MTGApplication.Services;
using MTGApplication.ViewModels;
using System;
using Windows.ApplicationModel.DataTransfer;

namespace MTGApplication.Views.Pages;

/// <summary>
/// Page for EDHREC card search
/// </summary>
[ObservableObject]
public sealed partial class EDHRECSearchPage : Page, IDialogPresenter
{
  public EDHRECSearchPage(CommanderTheme[] themes)
  {
    InitializeComponent();
    SearchViewModel = new(new EDHRECCommanderAPI(), new ScryfallAPI()) { CommanderThemes = themes };

    Loaded += EDHRECSearchPage_Loaded;
    Unloaded += EDHRECSearchPage_Unloaded;

    OnGetDialogWrapperHandler = (s, args) => { args.DialogWrapper = DialogWrapper; };
  }

  [ObservableProperty] private double searchDesiredItemWidth = 250;

  public EDHRECSearchViewModel SearchViewModel { get; }

  private EventHandler<DialogService.DialogEventArgs> OnGetDialogWrapperHandler { get; }

  #region IDialogPresenter implementation
  public DialogService.DialogWrapper DialogWrapper { get; private set; }
  #endregion

  private void EDHRECSearchPage_Loaded(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
  {
    DialogWrapper = new(XamlRoot);
    SearchViewModel.OnGetDialogWrapper += OnGetDialogWrapperHandler;
  }

  private void EDHRECSearchPage_Unloaded(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    => SearchViewModel.OnGetDialogWrapper -= OnGetDialogWrapperHandler;

  private void CardView_DragItemsStarting(object sender, DragItemsStartingEventArgs e)
  {
    if (e.Items[0] is MTGCardViewModel vm)
    {
      e.Data.SetText(vm.Model.ToJSON());
      e.Data.RequestedOperation = DataPackageOperation.Copy;
    }
  }
}
