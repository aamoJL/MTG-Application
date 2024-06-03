using MTGApplication.General.Models.Card;
using MTGApplication.General.Services.ConfirmationService;
using System.Collections.Generic;

namespace MTGApplication.Features.DeckEditor;

public class DeckEditorConfirmers
{
  public Confirmer<ConfirmationResult> SaveUnsavedChangesConfirmer { get; init; } = new();
  public Confirmer<string, string[]> LoadDeckConfirmer { get; init; } = new();
  public Confirmer<string, string> SaveDeckConfirmer { get; init; } = new();
  public Confirmer<ConfirmationResult> OverrideDeckConfirmer { get; init; } = new();
  public Confirmer<ConfirmationResult> DeleteDeckConfirmer { get; init; } = new();
  public Confirmer<MTGCard, IEnumerable<MTGCard>> ShowTokensConfirmer { get; init; } = new();

  public CardListConfirmers CardListConfirmers { get; init; } = new();
  public CommanderConfirmers CommanderConfirmers { get; init; } = new();

  public static Confirmation GetSaveUnsavedChangesConfirmation(string deckName)
  {
    return new(
      Title: "Save unsaved changes?",
      Message: $"{(string.IsNullOrEmpty(deckName) ? "Unnamed deck" : $"'{deckName}'")} has unsaved changes. Would you like to save the deck?");
  }

  public static Confirmation<string[]> GetLoadDeckConfirmation(string[] data)
  {
    return new(
      Title: "Open deck",
      Message: "Name",
      Data: data);
  }

  public static Confirmation<string> GetSaveDeckConfirmation(string oldName)
  {
    return new(
      Title: "Save your deck?",
      Message: string.Empty,
      Data: oldName);
  }

  public static Confirmation GetOverrideDeckConfirmation(string saveName)
  {
    return new(
      Title: "Override existing deck?",
      Message: $"Deck '{saveName}' already exist. Would you like to override the deck?");
  }

  public static Confirmation GetDeleteDeckConfirmation(string name)
  {
    return new(
      Title: "Delete the deck?",
      Message: $"Are you sure you want to delete '{name}'?");
  }

  public static Confirmation<IEnumerable<MTGCard>> GetShowTokensConfirmation(IEnumerable<MTGCard> data)
  {
    return new(
      Title: "Tokens",
      Message: string.Empty,
      Data: data);
  }
}