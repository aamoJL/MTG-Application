using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.WinUI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Navigation;
using MTGApplication.Features.DeckEditor.CardList.Services;
using MTGApplication.Features.DeckEditor.Editor.Models;
using MTGApplication.Features.DeckEditor.Editor.Services;
using MTGApplication.Features.DeckEditor.ViewModels;
using MTGApplication.General.Services.NotificationService;
using System;
using System.Collections;
using System.ComponentModel;

namespace MTGApplication.Features.DeckEditor.Views;
public sealed partial class DeckEditorPage : Page, INotifyPropertyChanged
{
  public enum CardViewType { Group, Image, Text }

  public DeckEditorPage()
  {
    InitializeComponent();

    Loaded += DeckEditorPage_Loaded;
  }

  private void DeckEditorPage_Loaded(object sender, RoutedEventArgs e)
  {
    Loaded -= DeckEditorPage_Loaded;

    DeckEditorViewDialogs.RegisterConfirmDialogs(ViewModel.Confirmers, root: XamlRoot);
    NotificationService.RegisterNotifications(ViewModel.Notifier, this);
  }

  public DeckEditorViewModel ViewModel { get; } = new(App.MTGCardImporter);

  public CardFilters CardFilter { get; } = new();
  public CardSorter CardSorter { get; } = new();
  public CardViewType DeckCardsViewType
  {
    get;
    set
    {
      if (field != value)
      {
        field = value;
        PropertyChanged?.Invoke(this, new(nameof(DeckCardsViewType)));
      }
    }
  } = CardViewType.Group;

  public event PropertyChangedEventHandler? PropertyChanged;

  [RelayCommand]
  private void SetDeckDisplayType(string type)
  {
    if (Enum.TryParse<CardViewType>(type, out var result))
      DeckCardsViewType = result;
  }

  protected override void OnNavigatedTo(NavigationEventArgs e)
  {
    base.OnNavigatedTo(e);

    if (e.Parameter is string deckName && ViewModel.OpenDeckCommand.CanExecute(deckName))
      ViewModel.OpenDeckCommand.Execute(deckName);
  }

  private async void SaveDeckKeyboardAccelerator_Invoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
  {
    args.Handled = true;

    if (ViewModel.SaveDeckCommand.CanExecute(null))
      await ViewModel.SaveDeckCommand.ExecuteAsync(null);
  }

  private async void OpenDeckKeyboardAccelerator_Invoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
  {
    args.Handled = true;

    if (ViewModel.OpenDeckCommand.CanExecute(null))
      await ViewModel.OpenDeckCommand.ExecuteAsync(null);
  }

  private async void NewDeckKeyboardAccelerator_Invoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
  {
    args.Handled = true;

    if (ViewModel.NewDeckCommand.CanExecute(null))
      await ViewModel.NewDeckCommand.ExecuteAsync(null);
  }

  private void ResetFiltersKeyboardAccelerator_Invoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
  {
    args.Handled = true;

    if (CardFilter.ResetCommand.CanExecute(null))
      CardFilter.ResetCommand.Execute(null);
  }

  private void UndoKeyboardAccelerator_Invoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
  {
    args.Handled = true;

    if (ViewModel.UndoCommand.CanExecute(null))
      ViewModel.UndoCommand.Execute(null);
  }

  private void RedoKeyboardAccelerator_Invoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
  {
    args.Handled = true;

    if (ViewModel.RedoCommand.CanExecute(null))
      ViewModel.RedoCommand.Execute(null);
  }

  private void DeleteCardKeyboardAccelerator_Invoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
  {
    if (args.Element is ListViewBase listview)
    {
      if (listview.DataContext is not CardListViewModel listViewViewModel
        || (listview.SelectedIndex is int index && index < 0)
        || listview.SelectedItem is not DeckEditorMTGCard selectedCard
        || !listViewViewModel.RemoveCardCommand.CanExecute(selectedCard))
        return;

      listViewViewModel.RemoveCardCommand.Execute(selectedCard);

      // Recalculate the index and focus the element in the index position if the element exists.
      if ((index = Math.Clamp(index, -1, listview.Items.Count - 1)) >= 0)
      {
        (listview.ContainerFromIndex(index) as UIElement)?.Focus(FocusState.Programmatic);

        listview.SelectedIndex = index;
      }

      args.Handled = true;
    }
    else if (args.Element is ItemsView itemsView)
    {
      if (itemsView.SelectedItem is not DeckEditorMTGCard selectedItem
        || itemsView.DataContext is not CardListViewModel itemsViewViewModel
        || itemsView.ItemsSource is not IList source
        || (source.IndexOf(selectedItem) is int index && index < 0)
        || !itemsViewViewModel.RemoveCardCommand.CanExecute(selectedItem))
        return;

      itemsViewViewModel.RemoveCardCommand.Execute(selectedItem);

      itemsView.Select(index < source.Count ? index : index - 1);

      args.Handled = true;
    }
  }

  private void ListView_LosingFocus(UIElement sender, LosingFocusEventArgs args)
  {
    // Deselect list selection if the list loses focus
    //    so the delete keyboard accelerator does not delete item in the list
    if (sender is ListViewBase listview)
    {
      if (args.NewFocusedElement is ListViewItem item
        && listview.Items.Contains(item.Content))
        return;

      listview.DeselectAll();

      args.Handled = true;
    }
  }
}