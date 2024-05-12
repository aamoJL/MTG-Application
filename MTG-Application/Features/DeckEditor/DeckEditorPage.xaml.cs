using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using MTGApplication.General.Services.ConfirmationService;
using MTGApplication.General.Services.NotificationService;
using MTGApplication.Views.Dialogs;
using static MTGApplication.General.Services.ConfirmationService.DialogService;

namespace MTGApplication.Features.DeckEditor;
public sealed partial class DeckEditorPage : Page, IDialogPresenter
{
  public DeckEditorPage()
  {
    InitializeComponent();

    RegisterConfirmDialogs(ViewModel.Confirmers);
    RegisterNotifications(ViewModel.Notifier);
  }

  public DeckEditorViewModel ViewModel { get; } = new();
  public DialogWrapper DialogWrapper => new(XamlRoot);

  protected override void OnNavigatedTo(NavigationEventArgs e)
  {
    base.OnNavigatedTo(e);

    if (e.Parameter is string deckName)
    {
      if (ViewModel.OpenDeckCommand.CanExecute(deckName)) ViewModel.OpenDeckCommand.Execute(deckName);
    }
  }

  private void RegisterConfirmDialogs(DeckEditorConfirmers confirmer)
  {
    confirmer.SaveUnsavedChanges = new() { OnConfirm = async msg => await new ShowUnsavedChangesDialog(DialogWrapper).Execute((msg.Title, msg.Message)) };
    confirmer.LoadDeck = new() { OnConfirm = async msg => await new ShowOpenDialog(DialogWrapper).Execute((msg.Title, msg.Message, msg.Data)) };
    confirmer.SaveDeck = new() { OnConfirm = async msg => await new ShowSaveDialog(DialogWrapper).Execute((msg.Title, msg.Message, msg.Data)) };
    confirmer.OverrideDeck = new() { OnConfirm = async msg => await new ShowOverrideDialog(DialogWrapper).Execute((msg.Title, msg.Message)) };
    confirmer.DeleteDeck = new() { OnConfirm = async msg => await new ShowDeleteDialog(DialogWrapper).Execute((msg.Title, msg.Message)) };
  }

  private void RegisterNotifications(DeckEditorNotifier notifier)
    => notifier.OnNotify = (arg) => { NotificationService.RaiseNotification(this, arg); };

  private void SaveDeckKeyboardAccelerator_Invoked(Microsoft.UI.Xaml.Input.KeyboardAccelerator sender, Microsoft.UI.Xaml.Input.KeyboardAcceleratorInvokedEventArgs args)
  {
    if (ViewModel.SaveDeckCommand.CanExecute(null)) ViewModel.SaveDeckCommand.Execute(null);
    args.Handled = true;
  }

  private void OpenDeckKeyboardAccelerator_Invoked(Microsoft.UI.Xaml.Input.KeyboardAccelerator sender, Microsoft.UI.Xaml.Input.KeyboardAcceleratorInvokedEventArgs args)
  {
    if (ViewModel.OpenDeckCommand.CanExecute(null)) ViewModel.OpenDeckCommand.Execute(null);
    args.Handled = true;
  }

  private void NewDeckKeyboardAccelerator_Invoked(Microsoft.UI.Xaml.Input.KeyboardAccelerator sender, Microsoft.UI.Xaml.Input.KeyboardAcceleratorInvokedEventArgs args)
  {
    if (ViewModel.NewDeckCommand.CanExecute(null)) ViewModel.NewDeckCommand.Execute(null);
    args.Handled = true;
  }

  private void ResetFiltersKeyboardAccelerator_Invoked(Microsoft.UI.Xaml.Input.KeyboardAccelerator sender, Microsoft.UI.Xaml.Input.KeyboardAcceleratorInvokedEventArgs args)
  {
    if (ViewModel.CardFilters.ResetCommand.CanExecute(null)) ViewModel.CardFilters.ResetCommand.Execute(null);
    args.Handled = true;
  }

  private void UndoKeyboardAccelerator_Invoked(Microsoft.UI.Xaml.Input.KeyboardAccelerator sender, Microsoft.UI.Xaml.Input.KeyboardAcceleratorInvokedEventArgs args)
  {
    if (ViewModel.UndoCommand.CanExecute(null)) ViewModel.UndoCommand.Execute(null);
    args.Handled = true;
  }

  private void RedoboardAccelerator_Invoked(Microsoft.UI.Xaml.Input.KeyboardAccelerator sender, Microsoft.UI.Xaml.Input.KeyboardAcceleratorInvokedEventArgs args)
  {
    if (ViewModel.RedoCommand.CanExecute(null)) ViewModel.RedoCommand.Execute(null);
    args.Handled = true;
  }
}