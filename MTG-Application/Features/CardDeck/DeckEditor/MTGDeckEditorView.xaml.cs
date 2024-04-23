using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using MTGApplication.Services.DialogService;
using static MTGApplication.Services.DialogService.DialogService;

namespace MTGApplication.Features.CardDeck;
public sealed partial class MTGDeckEditorView : Page, IDialogPresenter
{
  public MTGDeckEditorView()
  {
    InitializeComponent();

    SetConfirmDialogs(ViewModel.Confirmer);
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
  public void SetConfirmDialogs(MTGDeckEditorViewModelConfirmer confirmer)
  {
    confirmer.SaveUnsavedChanges = new() { OnConfirm = async (msg) => await GetUnsavedChangesDialog(msg.Title, msg.Message).ShowAsync(DialogWrapper) };
    confirmer.LoadDeck = new() { OnConfirm = async (msg) => await GetOpenDeckDialog(msg.Title, msg.Message, msg.Data).ShowAsync(DialogWrapper) };
  }

  public Dialog<bool?> GetUnsavedChangesDialog(string title, string message) => new ConfirmationDialog(title)
  {
    Message = message,
    PrimaryButtonText = "Save"
  };

  public Dialog<string> GetOpenDeckDialog(string title, string message, string[] deckNames) => new ComboBoxDialog(title)
  {
    InputHeader = message,
    Items = deckNames,
    PrimaryButtonText = "Open",
    SecondaryButtonText = string.Empty
  };
}