using MTGApplication.General.Services.ConfirmationService;

namespace MTGApplication.Features.DeckEditor;

public class CardListConfirmers
{
  public Confirmer<string, string> ExportConfirmer { get; init; } = new();
  public Confirmer<string, string> ImportConfirmer { get; init; } = new();
  public Confirmer<(ConfirmationResult Result, bool SkipCheck)> AddMultipleConflictConfirmer { get; init; } = new();
  public Confirmer<ConfirmationResult> AddSingleConflictConfirmer { get; init; } = new();

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

  public static Confirmation GetAddMultipleConflictConfirmer(string cardName)
  {
    return new(
      Title: "Card already exists in the list",
      Message: $"'{cardName}' already exists in the list. Do you still want to add it?");
  }

  public static Confirmation GetAddSingleConflictConfirmer(string cardName)
  {
    return new(
      Title: "Card already exists in the list",
      Message: $"'{cardName}' already exists in the list. Do you still want to add it?");
  }
}
