using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using MTGApplication.General.Services.ConfirmationService;
using static MTGApplication.General.Services.ConfirmationService.DialogService;

namespace MTGApplication.Features.CardDeck;
public sealed partial class DeckEditorView : Page, IDialogPresenter
{
  public DeckEditorView()
  {
    InitializeComponent();

    RegisterConfirmDialogs(ViewModel.Confirmers);
  }

  // TODO: notification system

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

public sealed partial class DeckEditorView
{
  private void RegisterConfirmDialogs(DeckEditorConfirmers confirmer)
  {
    confirmer.SaveUnsavedChanges = new() { OnConfirm = async msg => await new ShowUnsavedChangesDialog(DialogWrapper).Execute((msg.Title, msg.Message)) };
    confirmer.LoadDeck = new() { OnConfirm = async msg => await new ShowOpenDialog(DialogWrapper).Execute((msg.Title, msg.Message, msg.Data)) };
    confirmer.SaveDeck = new() { OnConfirm = async msg => await new ShowSaveDialog(DialogWrapper).Execute((msg.Title, msg.Message, msg.Data)) };
    confirmer.OverrideDeck = new() { OnConfirm = async msg => await new ShowOverrideDialog(DialogWrapper).Execute((msg.Title, msg.Message)) };
    confirmer.DeleteDeckUseCase = new() { OnConfirm = async msg => await new ShowDeleteDialog(DialogWrapper).Execute((msg.Title, msg.Message)) };
  }
}