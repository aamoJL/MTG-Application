using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MTGApplication.General;
using MTGApplication.General.Databases.Repositories.MTGDeckRepository;
using MTGApplication.General.Extensions;
using MTGApplication.Interfaces;
using MTGApplication.Models;
using MTGApplication.Models.DTOs;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MTGApplication.Features.CardDeck;

public interface IWorker
{
  public bool IsBusy { get; set; }

  /// <summary>
  /// Sets the <see cref="IsBusy"/> property to <see langword="true"/> for the duration of the <paramref name="task"/>
  /// </summary>
  public abstract Task<T> DoWork<T>(Task<T> task);
}

public class Confirmation<TReturn>
{
  public record ConfirmationData(string Title, string Message);

  public Func<ConfirmationData, Task<TReturn>> OnConfirm { get; set; }

  public async Task<TReturn> Confirm(string title, string message)
    => OnConfirm == null ? default : await OnConfirm.Invoke(new(title, message));
}

public class Confirmation<TArgs, TReturn>
{
  public record ConfirmationData(string Title, string Message, TArgs Data);

  public Func<ConfirmationData, Task<TReturn>> OnConfirm { get; set; }

  public async Task<TReturn> Confirm(string title, string message, TArgs data)
    => OnConfirm == null ? default : await OnConfirm.Invoke(new(title, message, data));
}

public class MTGDeckEditorViewModelConfirmer
{
  public Confirmation<bool?> SaveUnsavedChanges { get; set; } = new();
  public Confirmation<string[], string> LoadDeck { get; set; } = new();
}

public partial class MTGDeckEditorViewModel : ViewModelBase, ISavable, IWorker
{
  [ObservableProperty] private MTGCardDeck deck = new();
  [ObservableProperty] private bool isBusy;
  [ObservableProperty] private bool hasUnsavedChanges;

  public MTGDeckEditorViewModelConfirmer Confirmer { get; init; } = new();

  [RelayCommand]
  private async Task NewDeck()
  {
    if (HasUnsavedChanges)
    {
      if (await Confirmer.SaveUnsavedChanges.Confirm(
        title: "Save unsaved changes?",
        message: $"{(string.IsNullOrEmpty(Deck.Name) ? "Unnamed deck" : $"'{Deck.Name}'")} has unsaved changes. Would you like to save the deck?"
        ) is true)
      {
        if (SaveDeckCommand.CanExecute(Deck)) SaveDeckCommand.Execute(Deck);
      }
      else return;
    }

    Deck = new();
  }

  [RelayCommand]
  private async Task OpenDeck(string loadName = null)
  {
    if (HasUnsavedChanges)
    {
      return;
    }

    loadName ??= await Confirmer.LoadDeck.Confirm(
      title: "Open deck", 
      message: "Name", 
      data: (await new GetDecksUseCase(new DeckDTORepository(), App.MTGCardAPI) { Includes = ExpressionExtensions.EmptyArray<MTGCardDeckDTO>() }
      .Execute()).Select(x => x.Name).OrderBy(x => x).ToArray());

    if (loadName is not null)
    {
      var loadedDeck = await DoWork(new GetDeckUseCase(
        name: loadName,
        repository: new DeckDTORepository(new()),
        cardAPI: App.MTGCardAPI)
      .Execute());

      if (loadedDeck != null) Deck = loadedDeck;
      else
      {
        // TODO: error
      }
    }
  }

  [RelayCommand]
  private void SaveDeck(string newName)
  {
    IsBusy = true;

    // TODO: save

    IsBusy = false;
  }

  [RelayCommand] private void RemoveDeckCard(MTGCard card) => Deck.DeckCards.Remove(card);

  [RelayCommand]
  private void ImportDeckCards(string importText)
  {
    // TODO: Import
  }

  [RelayCommand(CanExecute = nameof(CanExecuteDeleteDeckCommand))]
  private void DeleteDeck()
  {
    IsBusy = true;
    // TODO: delete deck
    var deleted = true;
    if (deleted) Deck = new();
    IsBusy = false;
  }

  public async Task<bool> SaveUnsavedChanges() => await Task.FromResult(true);

  public async Task<T> DoWork<T>(Task<T> task)
  {
    IsBusy = true;
    var result = await task;
    IsBusy = false;
    return result;
  }
}

public partial class MTGDeckEditorViewModel
{
  private bool CanExecuteDeleteDeckCommand() => !string.IsNullOrEmpty(Deck.Name);
}

public class SaveUseCase : UseCase<string, bool>
{
  public SaveUseCase(MTGDeckEditorViewModel viewModel) => ViewModel = viewModel;

  public MTGDeckEditorViewModel ViewModel { get; }

  public override bool Execute(string newName)
    => new SaveDeckUseCase(new(ViewModel.Deck)).Execute(newName);
}

public class SaveDeckUseCase : UseCase<string, bool>
{
  public SaveDeckUseCase(MTGCardDeckDTO deck)
  {
    Deck = deck;
  }

  public MTGCardDeckDTO Deck { get; }

  // TODO: Save deck to DB
  public override bool Execute(string newName) => false;
}

public abstract class UseCase<TArg, TReturn>
{
  public abstract TReturn Execute(TArg arg);
}
