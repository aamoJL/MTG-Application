using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using MTGApplication.General.Databases.Repositories.MTGDeckRepository;
using MTGApplication.General.Extensions;
using MTGApplication.Interfaces;
using MTGApplication.Models.DTOs;
using MTGApplication.Services;
using System.Linq;
using System.Threading.Tasks;
using static MTGApplication.ViewModels.DeckBuilderViewModel;

namespace MTGApplication.Features.CardDeck;
public sealed partial class MTGDeckEditorView : Page, IDialogPresenter
{
  public MTGDeckEditorView()
  {
    InitializeComponent();

    // TODO: notification system
  }

  public MTGDeckEditorViewModel ViewModel { get; set; } = new();
  public DeckBuilderViewDialogs Dialogs { get; set; } = new();

  public DialogService.DialogWrapper DialogWrapper => new(XamlRoot);

  [RelayCommand]
  private void NewDeck()
  {
    // TODO: unsaved dialog
    if (ViewModel.NewDeckCommand.CanExecute(null)) ViewModel.NewDeckCommand.Execute(null);
  }

  [RelayCommand]
  private async Task OpenDeck()
  {
    // TODO: unsaved dialog
    var loadName = await Dialogs.GetLoadDialog(
      names: (await new GetDecksUseCase(new DeckDTORepository(), App.MTGCardAPI)
      {
        Includes = ExpressionExtensions.EmptyArray<MTGCardDeckDTO>()
      }
      .Execute())
      .Select(x => x.Name).OrderBy(x => x).ToArray())
      .ShowAsync(DialogWrapper);

    if (loadName is not null)
    {
      if (ViewModel.LoadDeckCommand.CanExecute(loadName)) ViewModel.LoadDeckCommand.Execute(loadName);
    }
  }

  [RelayCommand]
  private async Task SaveDeck()
  {
    var saveName = await Dialogs.GetSaveDialog(ViewModel.Deck.Name).ShowAsync(DialogWrapper);

    if (string.IsNullOrEmpty(saveName)) return;

    if (saveName != ViewModel.Deck.Name && await new DeckExistsUseCase(saveName, new DeckDTORepository()).Execute())
    {
      // Deck with the given name exists already
      var overrideConfirmation = await Dialogs.GetOverrideDialog(saveName).ShowAsync(DialogWrapper);

      if (overrideConfirmation is not true) return;
    }

    if (ViewModel.SaveDeckCommand.CanExecute(saveName)) ViewModel.SaveDeckCommand.Execute(saveName);
  }

  [RelayCommand]
  private void ImportToDeckCards()
  {
    // TODO: import dialog
    var importText = "";

    if (!string.IsNullOrEmpty(importText))
    {
      if (ViewModel.ImportDeckCardsCommand.CanExecute(importText)) ViewModel.ImportDeckCardsCommand.Execute(importText);
    }
  }

  [RelayCommand]
  private async Task DeleteDeck()
  {
    if (await Dialogs.GetDeleteDialog(ViewModel.Deck.Name).ShowAsync(DialogWrapper) is true)
    {
      if (ViewModel.DeleteDeckCommand.CanExecute(null)) ViewModel.DeleteDeckCommand.Execute(null);
    }
  }

  protected override void OnNavigatedTo(NavigationEventArgs e)
  {
    base.OnNavigatedTo(e);

    if (e.Parameter is string deckName && ViewModel.LoadDeckCommand.CanExecute(deckName))
      ViewModel.LoadDeckCommand.Execute(deckName);
    else if (ViewModel.NewDeckCommand.CanExecute(null))
      ViewModel.NewDeckCommand.Execute(null);
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