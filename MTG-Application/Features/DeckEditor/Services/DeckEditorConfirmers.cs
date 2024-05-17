using MTGApplication.General.Services.ConfirmationService;

namespace MTGApplication.Features.DeckEditor;

public class DeckEditorConfirmers
{
  public Confirmer<ConfirmationResult> SaveUnsavedChangesConfirmer { get; init; } = new();
  public Confirmer<string, string[]> LoadDeckConfirmer { get; init; } = new();
  public Confirmer<string, string> SaveDeckConfirmer { get; init; } = new();
  public Confirmer<ConfirmationResult> OverrideDeckConfirmer { get; init; } = new();
  public Confirmer<ConfirmationResult> DeleteDeckConfirmer { get; init; } = new();

  public CardListConfirmers CardListConfirmers { get; init; } = new();

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

  public static Confirmation<string> GetSaveDeckConfirmation(string deckName)
  {
    return new(
      Title: "Save your deck?",
      Message: string.Empty,
      Data: deckName);
  }

  public static Confirmation GetOverrideDeckConfirmation(string saveName)
  {
    return new(
      Title: "Override existing deck?",
      Message: $"Deck '{saveName}' already exist. Would you like to override the deck?");
  }

  public static Confirmation GetDeleteDeckConfirmation(string deckName)
  {
    return new(
      Title: "Delete deck?",
      Message: $"Are you sure you want to delete '{deckName}'?");
  }
}