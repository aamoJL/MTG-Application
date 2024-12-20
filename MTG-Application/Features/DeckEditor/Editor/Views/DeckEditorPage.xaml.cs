using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.WinUI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Navigation;
using MTGApplication.Features.DeckEditor.Editor.Services;
using MTGApplication.Features.DeckEditor.ViewModels;
using MTGApplication.General.Services.NotificationService;
using System;
using System.ComponentModel;

namespace MTGApplication.Features.DeckEditor.Views;
public sealed partial class DeckEditorPage : Page, INotifyPropertyChanged
{
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

  public bool DeckGroupViewVisible
  {
    get => field;
    set
    {
      if (field != value)
      {
        field = value;
        PropertyChanged?.Invoke(this, new(nameof(DeckGroupViewVisible)));
      }
    }
  } = true;
  public bool DeckImageViewVisible
  {
    get => field;
    set
    {
      if (field != value)
      {
        field = value;
        PropertyChanged?.Invoke(this, new(nameof(DeckImageViewVisible)));
      }
    }
  } = false;
  public bool DeckTextViewVisible
  {
    get => field;
    set
    {
      if (field != value)
      {
        field = value;
        PropertyChanged?.Invoke(this, new(nameof(DeckTextViewVisible)));
      }
    }
  } = false;

  public event PropertyChangedEventHandler? PropertyChanged;

  [RelayCommand]
  private void SetDeckDisplayType(string type)
  {
    DeckGroupViewVisible = type == "Group";
    DeckImageViewVisible = type == "Image";
    DeckTextViewVisible = type == "Text";
  }

  protected override void OnNavigatedTo(NavigationEventArgs e)
  {
    base.OnNavigatedTo(e);

    if (e.Parameter is string deckName)
      if (ViewModel.OpenDeckCommand.CanExecute(deckName))
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

    if (ViewModel.DeckCardList.CardFilters.ResetCommand.CanExecute(null))
      ViewModel.DeckCardList.CardFilters.ResetCommand.Execute(null);
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
    if (args.Element is ListViewBase listview && listview.SelectedIndex != -1 && listview.DataContext is CardListViewModel viewmodel)
    {
      var index = listview.SelectedIndex;
      var selectedCard = listview.Items[index];

      if (viewmodel.RemoveCardCommand.CanExecute(selectedCard))
        viewmodel.RemoveCardCommand.Execute(selectedCard);

      // Recalculate the index and focus the element in the index position if the element exists.
      if ((index = Math.Clamp(index, -1, listview.Items.Count - 1)) >= 0)
      {
        (listview.ContainerFromIndex(index) as UIElement)?.Focus(FocusState.Programmatic);

        listview.SelectedIndex = index;
      }
    }

    args.Handled = true;
  }

  private void ListView_LosingFocus(UIElement sender, LosingFocusEventArgs args)
  {
    // Deselect list selection if the list loses focus so
    // the delete keyboard accelerator does not delete item in the list
    if (sender is ListViewBase listview)
      if (args.NewFocusedElement is not ListViewItem item || !listview.Items.Contains(item.Content))
        listview.DeselectAll();

    args.Handled = true;
  }
}