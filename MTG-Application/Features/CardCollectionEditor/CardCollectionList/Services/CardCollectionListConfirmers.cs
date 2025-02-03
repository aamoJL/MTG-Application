using MTGApplication.General.Services.ConfirmationService;

namespace MTGApplication.Features.CardCollectionEditor.CardCollectionList.Services;

public class CardCollectionListConfirmers
{
  public Confirmer<(string Name, string Query)?, (string Name, string Query)> EditCollectionListConfirmer { get; init; } = new();
  public Confirmer<ConfirmationResult> EditCollectionListQueryConflictConfirmer { get; init; } = new();
  public Confirmer<string> ImportCardsConfirmer { get; init; } = new();
  public Confirmer<string, string> ExportCardsConfirmer { get; init; } = new();

  public static Confirmation<(string Name, string Query)> GetEditCollectionListConfirmation((string Name, string Query) args)
  {
    return new(
      Title: "Edit list",
      Message: string.Empty,
      Data: args);
  }

  public static Confirmation GetEditCollectionListQueryConflictConfirmation(int count)
  {
    return new(
      Title: "Query conflict",
      Message: $"{count} owned cards are not in the new query. Cards will be removed from the collection list.");
  }

  public static Confirmation GetImportCardsConfirmation()
  {
    return new(
      Title: "Import cards with Scryfall IDs",
      Message: string.Empty);
  }

  public static Confirmation<string> GetExportCardsConfirmation(string data)
  {
    return new(
      Title: "Export cards",
      Message: string.Empty,
      Data: data);
  }
}