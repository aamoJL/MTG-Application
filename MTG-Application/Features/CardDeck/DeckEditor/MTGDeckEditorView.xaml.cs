using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using MTGApplication.General.Services.ConfirmationService;
using System.IO;
using System.Threading.Tasks;
using static MTGApplication.General.Services.ConfirmationService.DialogService;

namespace MTGApplication.Features.CardDeck;
public sealed partial class MTGDeckEditorView : Page, IDialogPresenter
{
  public MTGDeckEditorView()
  {
    InitializeComponent();

    RegisterConfirmDialogs(ViewModel.Confirmer);
  }

  // TODO: notification system

  public MTGDeckEditorViewModel ViewModel { get; } = new();

  public DialogWrapper DialogWrapper => new(XamlRoot);

  protected override void OnNavigatedTo(NavigationEventArgs e)
  {
    base.OnNavigatedTo(e);

    if (e.Parameter is string deckName)
    {
      if (ViewModel.OpenDeckCommand.CanExecute(deckName)) ViewModel.OpenDeckCommand.Execute(deckName);
    }
  }

  private void CardView_DragOver(object sender, Microsoft.UI.Xaml.DragEventArgs e)
  {
    //TODO: event
  }

  private void CardView_Drop(object sender, Microsoft.UI.Xaml.DragEventArgs e)
  {
    //TODO: event
  }

  private void CardView_DragItemsStarting(object sender, DragItemsStartingEventArgs e)
  {
    //TODO: event
  }

  private void CardView_DragItemsCompleted(ListViewBase sender, DragItemsCompletedEventArgs args)
  {
    //TODO: event
  }

  private void CardView_KeyDown(object sender, Microsoft.UI.Xaml.Input.KeyRoutedEventArgs e)
  {
    //TODO: event
  }

  private void CardView_LosingFocus(Microsoft.UI.Xaml.UIElement sender, Microsoft.UI.Xaml.Input.LosingFocusEventArgs args)
  {
    //TODO: event
  }
}

public sealed partial class MTGDeckEditorView
{
  private void RegisterConfirmDialogs(MTGDeckEditorViewModelConfirmer confirmer)
  {
    confirmer.SaveUnsavedChanges = new() { OnConfirm = async msg => await ShowUnsavedChangesDialog(msg.Title, msg.Message) };
    confirmer.LoadDeck = new() { OnConfirm = async msg => await ShowOpenDeckDialog(msg.Title, msg.Message, msg.Data) };
    confirmer.SaveDeck = new() { OnConfirm = async msg => await ShowSaveDeckDialog(msg.Title, msg.Message, msg.Data) };
    confirmer.OverrideDeck = new() { OnConfirm = async msg => await ShowOverrideDeckDialog(msg.Title, msg.Message) };
    confirmer.DeleteDeckUseCase = new() { OnConfirm = async msg => await ShowDeleteDeckDialog(msg.Title, msg.Message) };
  }

  private async Task<ConfirmationResult> ShowUnsavedChangesDialog(string title, string message) => (await new ConfirmationDialog(title)
  {
    Message = message,
    PrimaryButtonText = "Save"
  }.ShowAsync(DialogWrapper)).ToConfirmationResult();

  private async Task<string> ShowOpenDeckDialog(string title, string message, string[] deckNames) => (await new ComboBoxDialog(title)
  {
    InputHeader = message,
    Items = deckNames,
    PrimaryButtonText = "Open",
    SecondaryButtonText = string.Empty
  }.ShowAsync(DialogWrapper));

  private async Task<string> ShowSaveDeckDialog(string title, string message, string initName) => (await new TextBoxDialog(title)
  {
    InvalidInputCharacters = Path.GetInvalidFileNameChars(),
    TextInputText = initName,
    PrimaryButtonText = "Save",
    SecondaryButtonText = string.Empty
  }.ShowAsync(DialogWrapper));

  private async Task<ConfirmationResult> ShowOverrideDeckDialog(string title, string message) => (await new ConfirmationDialog(title)
  {
    Message = message,
    SecondaryButtonText = string.Empty
  }.ShowAsync(DialogWrapper)).ToConfirmationResult();

  private async Task<ConfirmationResult> ShowDeleteDeckDialog(string title, string message) => (await new ConfirmationDialog(title)
  {
    Message = message,
    SecondaryButtonText = string.Empty
  }.ShowAsync(DialogWrapper)).ToConfirmationResult();
}