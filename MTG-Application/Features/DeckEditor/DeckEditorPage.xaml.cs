using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using MTGApplication.General.Models.Card;
using MTGApplication.General.Services.ConfirmationService;
using MTGApplication.General.Services.NotificationService;
using MTGApplication.General.Views;
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
    RegisterDragAndDropCommands();
  }

  public DeckEditorViewModel ViewModel { get; } = new();
  public DialogWrapper DialogWrapper => new(XamlRoot);
  public ListViewDragAndDrop<MTGCard> DeckCardDragAndDrop { get; set; }
  public ListViewDragAndDrop<MTGCard> MaybeCardDragAndDrop { get; set; }

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

  private void RegisterDragAndDropCommands()
  {
    DeckCardDragAndDrop = new()
    {
      AddCommand = ViewModel.DeckCards.AddCardCommand,
      RemoveCommand = ViewModel.DeckCards.RemoveCardCommand,
      OnCopy = async (cmd, data) => await ViewModel.ExternalCardImportCommand.ExecuteAsync(new(data, cmd, null)),
      OnMove = async (cmd, cmd2, data) => await ViewModel.ExternalCardImportCommand.ExecuteAsync(new(data, cmd, cmd2))
    };
    MaybeCardDragAndDrop = new()
    {
      AddCommand = ViewModel.MaybeCards.AddCardCommand,
      RemoveCommand = ViewModel.MaybeCards.RemoveCardCommand,
      OnCopy = async (cmd, data) => await ViewModel.ExternalCardImportCommand.ExecuteAsync(new(data, cmd, null)),
      OnMove = async (cmd, cmd2, data) => await ViewModel.ExternalCardImportCommand.ExecuteAsync(new(data, cmd, cmd2))
    };
  }

  private void SaveDeckKeyboardAccelerator_Invoked(Microsoft.UI.Xaml.Input.KeyboardAccelerator sender, Microsoft.UI.Xaml.Input.KeyboardAcceleratorInvokedEventArgs args)
  {
    if (ViewModel.SaveDeckCommand.CanExecute(null)) ViewModel.SaveDeckCommand.Execute(null);
  }

  private void OpenDeckKeyboardAccelerator_Invoked(Microsoft.UI.Xaml.Input.KeyboardAccelerator sender, Microsoft.UI.Xaml.Input.KeyboardAcceleratorInvokedEventArgs args)
  {
    if (ViewModel.OpenDeckCommand.CanExecute(null)) ViewModel.OpenDeckCommand.Execute(null);
  }

  private void NewDeckKeyboardAccelerator_Invoked(Microsoft.UI.Xaml.Input.KeyboardAccelerator sender, Microsoft.UI.Xaml.Input.KeyboardAcceleratorInvokedEventArgs args)
  {
    if (ViewModel.NewDeckCommand.CanExecute(null)) ViewModel.NewDeckCommand.Execute(null);
  }

  private void ResetFiltersKeyboardAccelerator_Invoked(Microsoft.UI.Xaml.Input.KeyboardAccelerator sender, Microsoft.UI.Xaml.Input.KeyboardAcceleratorInvokedEventArgs args)
  {
    if (ViewModel.CardFilters.ResetCommand.CanExecute(null)) ViewModel.CardFilters.ResetCommand.Execute(null);
  }
}