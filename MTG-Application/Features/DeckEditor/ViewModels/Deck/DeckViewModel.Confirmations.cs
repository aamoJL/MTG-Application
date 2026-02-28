using MTGApplication.General.Models;
using MTGApplication.General.Services.ConfirmationService;
using System.Collections.Generic;

namespace MTGApplication.Features.DeckEditor.ViewModels.Deck;

public partial class DeckViewModel
{
  public static class Confirmations
  {
    public static Confirmation GetSaveUnsavedChangesConfirmation(string deckName)
    {
      return new(
        Title: "Save unsaved changes?",
        Message: $"{(string.IsNullOrEmpty(deckName) ? "Unnamed deck" : $"'{deckName}'")} has unsaved changes. Would you like to save the deck?");
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
}