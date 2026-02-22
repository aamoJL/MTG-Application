using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Navigation;
using MTGApplication.Features.DeckEditor.Services;
using MTGApplication.Features.DeckEditor.ViewModels.EditorPage;
using MTGApplication.General.Models;
using MTGApplication.General.Services.ConfirmationService;
using MTGApplication.General.Services.NotificationService;
using MTGApplication.General.Views.Dialogs.Controls;
using MTGApplication.General.Views.Dialogs.UseCases;
using MTGApplication.General.Views.Styles.Templates;
using System;
using System.ComponentModel;
using System.IO;

namespace MTGApplication.Features.DeckEditor.Views;

public sealed partial class DeckEditorPage : Page, INotifyPropertyChanged
{
  public enum CardViewType { Group, Image, Text }

  public DeckEditorPage() => InitializeComponent();

  public DeckEditorPageViewModel ViewModel => field ??= new()
  {
    Notifier = Notifier,
    Confirmers = new()
    {
      ConfirmDeckOpen = async msg => await new ShowOpenDialog(XamlRoot).Execute((msg.Title, msg.Message, [.. msg.Data])),

      DeckConfirmers = new()
      {
        ConfirmUnsavedChanges = async msg => await new ShowUnsavedChangesDialog(XamlRoot).Execute((msg.Title, msg.Message)),
        ConfirmDeckSave = async msg => await new ShowSaveDialog(XamlRoot).Execute((msg.Title, msg.Message, msg.Data)),
        ConfirmDeckSaveOverride = async msg => await new ShowOverrideDialog(XamlRoot).Execute((msg.Title, msg.Message)),
        ConfirmDeckDelete = async msg => await new ShowDeleteDialog(XamlRoot).Execute((msg.Title, msg.Message)),
        ConfirmDeckTokens = async (msg) =>
        {
          Application.Current.Resources.TryGetValue(nameof(MTGPrintGridViewItemTemplate), out var template);

          await DialogService.ShowAsync(XamlRoot, new GridViewDialog(
            title: msg.Title,
            items: [.. msg.Data],
            itemTemplate: (DataTemplate)template)
          {
            PrimaryButtonText = string.Empty,
            CloseButtonText = "Close",
            CanSelectItems = false,
          });
        },

        ListConfirmers = new()
        {
          ConfirmAddSingleConflict = async (msg) => await DialogService.ShowAsync(XamlRoot, new TwoButtonConfirmationDialog(msg.Title, msg.Message)),
          ConfirmAddMultipleConflict = async (msg) => await DialogService.ShowAsync(XamlRoot, new CheckBoxDialog(msg.Title, msg.Message)
          {
            InputText = "Same for all cards.",
            CloseButtonText = "No",
          }),
          ConfirmImport = async msg => await DialogService.ShowAsync(XamlRoot, new TextAreaDialog(msg.Title)
          {
            InputPlaceholderText = "Example:\n2 Black Lotus\nMox Ruby\nbd8fa327-dd41-4737-8f19-2cf5eb1f7cdd",
            PrimaryButtonText = "Import",
          }),
          ConfirmExport = async msg => await DialogService.ShowAsync(XamlRoot, new TextAreaDialog(msg.Title)
          {
            InputText = msg.Data,
            PrimaryButtonText = "Copy to Clipboard",
          }),

          CardConfirmers = new()
          {
            ConfirmCardPrints = async (msg) =>
            {
              Application.Current.Resources.TryGetValue(nameof(MTGPrintGridViewItemTemplate), out var template);

              return (await DialogService.ShowAsync(XamlRoot, new GridViewDialog(
                title: msg.Title,
                items: [.. msg.Data],
                itemTemplate: (DataTemplate)template))) as MTGCard;
            },
          }
        },

        GroupListConfirmers = new()
        {
          ConfirmAddGroup = async msg => await DialogService.ShowAsync(XamlRoot, new TextBoxDialog(msg.Title)
          {
            InvalidInputCharacters = Path.GetInvalidFileNameChars(),
            PrimaryButtonText = "Add",
            InputValidation = input => !string.IsNullOrEmpty(input)
          }),

          GroupConfirmers = new()
          {
            ConfirmMergeGroups = async msg => await DialogService.ShowAsync(XamlRoot, new TwoButtonConfirmationDialog(msg.Title, msg.Message)
            {
              PrimaryButtonText = "Merge"
            }),
            ConfirmRenameGroup = async msg => await DialogService.ShowAsync(XamlRoot, new TextBoxDialog(msg.Title)
            {
              InvalidInputCharacters = Path.GetInvalidFileNameChars(),
              InputText = msg.Data,
              PrimaryButtonText = "Rename",
              InputValidation = input => !string.IsNullOrEmpty(input)
            }),
          }
        }
      }
    }
  };

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

  public NotificationService.Notifier Notifier
  {
    get => field ??= Notifier = new();
    private set
    {
      field?.OnNotifyEvent -= DeckEditorPage_OnNotifyEvent;
      field = value;
      field?.OnNotifyEvent += DeckEditorPage_OnNotifyEvent;
    }
  }

  public event PropertyChangedEventHandler? PropertyChanged;

  private void DeckEditorPage_OnNotifyEvent(object? _, NotificationService.Notification e)
    => NotificationService.RaiseNotification(this, e);

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

  private async void NewDeckKeyboardAccelerator_Invoked(KeyboardAccelerator _, KeyboardAcceleratorInvokedEventArgs args)
  {
    args.Handled = true;

    if (ViewModel.NewDeckCommand.CanExecute(null))
      await ViewModel.NewDeckCommand.ExecuteAsync(null);
  }

  private async void OpenDeckKeyboardAccelerator_Invoked(KeyboardAccelerator _, KeyboardAcceleratorInvokedEventArgs args)
  {
    args.Handled = true;

    if (ViewModel.OpenDeckCommand.CanExecute(null))
      await ViewModel.OpenDeckCommand.ExecuteAsync(null);
  }

  private async void SaveDeckKeyboardAccelerator_Invoked(KeyboardAccelerator _, KeyboardAcceleratorInvokedEventArgs args)
  {
    args.Handled = true;

    if (ViewModel.DeckViewModel.SaveDeckCommand.CanExecute(null))
      await ViewModel.DeckViewModel.SaveDeckCommand.ExecuteAsync(null);
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

    if (ViewModel.DeckViewModel.UndoStack.UndoCommand.CanExecute(null))
      ViewModel.DeckViewModel.UndoStack.UndoCommand.Execute(null);
  }

  private void RedoKeyboardAccelerator_Invoked(KeyboardAccelerator _, KeyboardAcceleratorInvokedEventArgs args)
  {
    args.Handled = true;

    if (ViewModel.DeckViewModel.UndoStack.RedoCommand.CanExecute(null))
      ViewModel.DeckViewModel.UndoStack.RedoCommand.Execute(null);
  }

  private void FilterKeyboardAccelerator_Invoked(KeyboardAccelerator _, KeyboardAcceleratorInvokedEventArgs args)
    // Event can't be set as handled here because the filter flyout would not be opened
    => (args.Element as FrameworkElement)?.Focus(FocusState.Programmatic);
}