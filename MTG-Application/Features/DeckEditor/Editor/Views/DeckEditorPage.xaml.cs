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
using MTGApplication.General.Views.Controls;
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

  private void DeckEditorPage_Loaded(object _, RoutedEventArgs e)
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
  } = CardViewType.Image;

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

  private async void SaveDeckKeyboardAccelerator_Invoked(KeyboardAccelerator _, KeyboardAcceleratorInvokedEventArgs args)
  {
    args.Handled = true;

    if (ViewModel.SaveDeckCommand.CanExecute(null))
      await ViewModel.SaveDeckCommand.ExecuteAsync(null);
  }

  private async void OpenDeckKeyboardAccelerator_Invoked(KeyboardAccelerator _, KeyboardAcceleratorInvokedEventArgs args)
  {
    args.Handled = true;

    if (ViewModel.OpenDeckCommand.CanExecute(null))
      await ViewModel.OpenDeckCommand.ExecuteAsync(null);
  }

  private async void NewDeckKeyboardAccelerator_Invoked(KeyboardAccelerator _, KeyboardAcceleratorInvokedEventArgs args)
  {
    args.Handled = true;

    if (ViewModel.NewDeckCommand.CanExecute(null))
      await ViewModel.NewDeckCommand.ExecuteAsync(null);
  }

  private void ResetFiltersKeyboardAccelerator_Invoked(KeyboardAccelerator _, KeyboardAcceleratorInvokedEventArgs args)
  {
    args.Handled = true;

    if (CardFilter.ResetCommand.CanExecute(null))
      CardFilter.ResetCommand.Execute(null);
  }

  private void UndoKeyboardAccelerator_Invoked(KeyboardAccelerator _, KeyboardAcceleratorInvokedEventArgs args)
  {
    args.Handled = true;

    if (ViewModel.UndoStack.UndoCommand.CanExecute(null))
      ViewModel.UndoStack.UndoCommand.Execute(null);
  }

  private void RedoKeyboardAccelerator_Invoked(KeyboardAccelerator _, KeyboardAcceleratorInvokedEventArgs args)
  {
    args.Handled = true;

    if (ViewModel.UndoStack.RedoCommand.CanExecute(null))
      ViewModel.UndoStack.RedoCommand.Execute(null);
  }

  private void DeleteCardKeyboardAccelerator_Invoked(KeyboardAccelerator _, KeyboardAcceleratorInvokedEventArgs args)
  {
    if (args.Element is ListViewBase listview)
    {
      if (listview.DataContext is not ICardListViewModel listViewViewModel
        || (listview.SelectedIndex is int index && index < 0)
        || listview.SelectedItem is not DeckEditorMTGCard selectedCard
        || listViewViewModel.RemoveCardCommand?.CanExecute(selectedCard) is not true)
        return;

      listViewViewModel.RemoveCardCommand.Execute(selectedCard);

      // Recalculate the index and focus the element in the index position if the element exists.
      if ((index = Math.Clamp(index, -1, listview.Items.Count - 1)) >= 0)
      {
        (listview.ContainerFromIndex(index) as UIElement)?.Focus(FocusState.Programmatic);

        listview.SelectedIndex = index;
      }
    }
    else if (args.Element is ItemsView itemsView)
    {
      if (itemsView.SelectedItem is not DeckEditorMTGCard selectedItem
        || itemsView.DataContext is not ICardListViewModel itemsViewViewModel
        || itemsView.ItemsSource is not IList source
        || (source.IndexOf(selectedItem) is int index && index < 0)
        || itemsViewViewModel.RemoveCardCommand?.CanExecute(selectedItem) is not true)
        return;

      itemsViewViewModel.RemoveCardCommand.Execute(selectedItem);

      itemsView.Select(index < source.Count ? index : index - 1);

    }
    else if (args.Element is AdvancedItemsRepeater air)
    {
      if (air.SelectedItem is not DeckEditorMTGCard selectedItem
        || air.DataContext is not ICardListViewModel itemsViewViewModel
        || air.ItemsSource is not IList source
        || (source.IndexOf(selectedItem) is int index && index < 0)
        || itemsViewViewModel.RemoveCardCommand?.CanExecute(selectedItem) is not true)
        return;

      itemsViewViewModel.RemoveCardCommand.Execute(selectedItem);

      // Recalculate the index and focus the element in the index position if the element exists.
      if ((index = Math.Clamp(index, -1, source.Count - 1)) >= 0)
        if (source[index] is object nextItem)
          air.SelectItem(nextItem);
    }

    args.Handled = true;
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
    }
    else if (sender is AdvancedItemsRepeater air)
    {
      if (args.NewFocusedElement is ItemContainer item
        && air.GetElementIndex(item) != -1)
        return;

      air.DeselectAll();
    }

    args.Handled = true;
  }
}