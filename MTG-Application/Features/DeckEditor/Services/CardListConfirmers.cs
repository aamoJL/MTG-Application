using MTGApplication.General.Services.ConfirmationService;

namespace MTGApplication.Features.DeckEditor;

public class CardListConfirmers
{
  public Confirmer<string, string> ExportConfirmer { get; init; } = new();
  public Confirmer<string, string> ImportConfirmer { get; init; } = new();
  public Confirmer<(ConfirmationResult Result, bool SkipCheck)> ImportConflictConfirmer { get; init; } = new();

  public static Confirmation<string> GetExportConfirmation(string data)
  {
    return new(
      Title: "Export cards",
      Message: string.Empty,
      Data: data);
  }

  public static Confirmation<string> GetImportConfirmation(string data)
  {
    return new(
      Title: "Import cards",
      Message: string.Empty,
      Data: data);
  }

  public static Confirmation GetExistingCardImportConfirmer(string cardName)
  {
    return new(
      Title: "Card already in the deck",
      Message: $"'{cardName}' is already in the deck. Do you still want to add it?");
  }
}
