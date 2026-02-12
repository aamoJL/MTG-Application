using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using MTGApplication.Features.CardCollectionEditor.ViewModels.EditorPage;
using MTGApplication.Features.CardCollectionEditor.Views.Controls;
using MTGApplication.General.Services.ConfirmationService;
using MTGApplication.General.Views.AppWindows;
using MTGApplication.General.Views.AppWindows.UseCases;
using MTGApplication.General.Views.Dialogs.Controls;
using MTGApplication.General.Views.Dialogs.UseCases;
using MTGApplication.General.Views.Styles.Templates;
using System;
using static MTGApplication.General.Services.NotificationService.NotificationService;

namespace MTGApplication.Features.CardCollectionEditor.Views;

public sealed partial class CardCollectionEditorPage : Page
{
  public CardCollectionEditorPage()
  {
    InitializeComponent();

    WindowClosing.Closing += WindowClosing_Closing;
    WindowClosing.Closed += WindowClosing_Closed;
  }

  public CardCollectionEditorPageViewModel ViewModel
  {
    get => field ??= new()
    {
      Notifier = Notifier,
      Confirmers = new()
      {
        ConfirmCollectionOpen = async (msg) => await new ShowOpenDialog(XamlRoot).Execute((msg.Title, msg.Message, [.. msg.Data])),

        CollectionConfirmers = new()
        {
          // TOOD: unsaved changes
          ConfirmUnsavedChanges = async (msg) => await new ShowUnsavedChangesDialog(XamlRoot).Execute((msg.Title, msg.Message)),
          ConfirmCollectionSave = async (msg) => await new ShowSaveDialog(XamlRoot).Execute((msg.Title, msg.Message, msg.Data)),
          ConfirmCollectionSaveOverride = async (msg) => await new ShowOverrideDialog(XamlRoot).Execute((msg.Title, msg.Message)),
          ConfirmCollectionDelete = async (msg) => await new ShowDeleteDialog(XamlRoot).Execute((msg.Title, msg.Message)),
          ConfirmAddNewList = async (msg) => await DialogService.ShowAsync(XamlRoot, new CollectionListContentDialog(msg.Title) { PrimaryButtonText = "Add" }),

          CollectionListConfirmers = new()
          {
            ConfirmEditList = async msg => await DialogService.ShowAsync(XamlRoot, new CollectionListContentDialog(msg.Title)
            {
              PrimaryButtonText = "Edit",
              NameInputText = msg.Data.name,
              QueryInputText = msg.Data.query,
            }),
            ConfirmEditQueryConflict = async msg => await DialogService.ShowAsync(XamlRoot, new TwoButtonConfirmationDialog(msg.Title, msg.Message)),
            ConfirmListDelete = async msg => await new ShowDeleteDialog(XamlRoot).Execute((msg.Title, msg.Message)),
            ConfirmCardImport = async msg => await DialogService.ShowAsync(XamlRoot, new TextAreaDialog(msg.Title)
            {
              InputPlaceholderText = "Example:\ned0216a0-c5c9-4a99-b869-53e4d0256326\n45fd6e91-df76-497f-b642-33dc3d5f6a5a\nbd8fa327-dd41-4737-8f19-2cf5eb1f7cdd",
              PrimaryButtonText = "Import",
            }),
            ConfirmCardExport = async msg => await DialogService.ShowAsync(XamlRoot, new TextAreaDialog(msg.Title)
            {
              InputText = msg.Data,
              PrimaryButtonText = "Copy to Clipboard",
            }),

            CardConfirmers = new()
            {
              ConfirmCardPrints = async (msg) =>
              {
                ArgumentNullException.ThrowIfNull(XamlRoot);

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
            }
          }
        }
      },
    };
  }

  private Notifier Notifier
  {
    get => field ??= Notifier = new();
    set
    {
      if (field == value) return;
      field?.OnNotifyEvent -= Notifier_OnNotifyEvent;
      field = value;
      field?.OnNotifyEvent += Notifier_OnNotifyEvent;
    }
  }

  [RelayCommand]
  private void SwitchWindowTheme() => new ChangeWindowTheme().Execute(AppConfig.LocalSettings.AppTheme == ElementTheme.Dark ? ElementTheme.Light : ElementTheme.Dark);

  private void Notifier_OnNotifyEvent(object? _, Notification e)
    => RaiseNotification(this, e);

  private async void NewCollectionKeyboardAccelerator_Invoked(Microsoft.UI.Xaml.Input.KeyboardAccelerator sender, Microsoft.UI.Xaml.Input.KeyboardAcceleratorInvokedEventArgs args)
  {
    args.Handled = true;

    if (ViewModel.NewCollectionCommand?.CanExecute(null) is true)
      await ViewModel.NewCollectionCommand.ExecuteAsync(null);
  }

  private async void SaveCollectionKeyboardAccelerator_Invoked(Microsoft.UI.Xaml.Input.KeyboardAccelerator sender, Microsoft.UI.Xaml.Input.KeyboardAcceleratorInvokedEventArgs args)
  {
    args.Handled = true;

    if (ViewModel.CollectionViewModel?.SaveCollectionCommand?.CanExecute(null) is true)
      await ViewModel.CollectionViewModel.SaveCollectionCommand.ExecuteAsync(null);
  }

  private async void OpenCollectionKeyboardAccelerator_Invoked(Microsoft.UI.Xaml.Input.KeyboardAccelerator sender, Microsoft.UI.Xaml.Input.KeyboardAcceleratorInvokedEventArgs args)
  {
    args.Handled = true;

    if (ViewModel.OpenCollectionCommand?.CanExecute(null) is true)
      await ViewModel.OpenCollectionCommand.ExecuteAsync(null);
  }

  private void WindowClosing_Closing(object? _, WindowClosing.ClosingEventArgs e)
  {
    if (e.Root != XamlRoot) return;

    if (ViewModel.CollectionViewModel?.SaveUnsavedChangesCommand.CanExecute(null) is true)
      e.Tasks.Add(ViewModel.CollectionViewModel.SaveUnsavedChangesCommand.ExecuteAsync);
  }

  private void WindowClosing_Closed(object? _, WindowClosing.ClosedEventArgs e)
  {
    if (e.Root != XamlRoot) return;

    WindowClosing.Closing -= WindowClosing_Closing;
    WindowClosing.Closed -= WindowClosing_Closed;
  }
}