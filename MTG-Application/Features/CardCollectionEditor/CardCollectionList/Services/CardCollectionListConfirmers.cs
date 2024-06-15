using MTGApplication.General.Models;
using MTGApplication.General.Services.ConfirmationService;
using System.Collections.Generic;

namespace MTGApplication.Features.CardCollection.Services;

public class CardCollectionListConfirmers
{
  public Confirmer<(string Name, string Query)?, (string Name, string Query)> EditCollectionListConfirmer { get; init; } = new();
  public Confirmer<string> ImportCardsConfirmer { get; init; } = new();
  public Confirmer<string, string> ExportCardsConfirmer { get; init; } = new();
  public Confirmer<MTGCard, IEnumerable<MTGCard>> ShowCardPrintsConfirmer { get; init; } = new();

  public static Confirmation<(string Name, string Query)> GetEditCollectionListConfirmation((string Name, string Query) args)
  {
    return new(
      Title: "Edit list",
      Message: string.Empty,
      Data: args);
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

  public static Confirmation<IEnumerable<MTGCard>> GetShowCardPrintsConfirmation(IEnumerable<MTGCard> data)
  {
    return new(
      Title: "Card prints",
      Message: string.Empty,
      Data: data);
  }
}