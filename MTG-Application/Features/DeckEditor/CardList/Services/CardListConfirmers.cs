using MTGApplication.General.Models;
using MTGApplication.General.Services.ConfirmationService;
using System.Collections.Generic;

namespace MTGApplication.Features.DeckEditor.CardList.Services;

public class CardListConfirmers
{
  public Confirmer<string, string> ExportConfirmer { get; init; } = new();
  public Confirmer<string, string> ImportConfirmer { get; init; } = new();
  public Confirmer<(ConfirmationResult Result, bool SkipCheck)> AddMultipleConflictConfirmer { get; init; } = new();
  public Confirmer<ConfirmationResult> AddSingleConflictConfirmer { get; init; } = new();
  public Confirmer<MTGCard, IEnumerable<MTGCard>> ChangeCardPrintConfirmer { get; init; } = new();

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

  public static Confirmation GetAddMultipleConflictConfirmation(string cardName)
  {
    return new(
      Title: "Card already exists in the list",
      Message: $"'{cardName}' already exists in the list. Do you still want to add it?");
  }

  public static Confirmation GetAddSingleConflictConfirmation(string cardName)
  {
    return new(
      Title: "Card already exists in the list",
      Message: $"'{cardName}' already exists in the list. Do you still want to add it?");
  }

  public static Confirmation<IEnumerable<MTGCard>> GetChangeCardPrintConfirmation(IEnumerable<MTGCard> data)
  {
    return new(
      Title: "Card prints",
      Message: string.Empty,
      Data: data);
  }
}

public class GroupedCardListConfirmers : CardListConfirmers
{
  public Confirmer<string> AddCardGroupConfirmer { get; init; } = new();
  public Confirmer<string, string> RenameCardGroupConfirmer { get; init; } = new();
  public Confirmer<ConfirmationResult> MergeCardGroupsConfirmer { get; init; } = new();

  public static Confirmation GetAddCardGroupConfirmation()
  {
    return new(
      Title: "Add new group",
      Message: string.Empty);
  }

  public static Confirmation<string> GetRenameCardGroupConfirmation(string oldName)
  {
    return new(
      Title: "Rename group",
      Message: string.Empty,
      Data: oldName);
  }

  public static Confirmation GetMergeCardGroupsConfirmation(string groupKey)
  {
    return new(
      Title: $"Group '{groupKey}' already exists.",
      Message: "Would you like to merge the groups?");
  }
}