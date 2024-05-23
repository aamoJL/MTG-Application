﻿using MTGApplication.General.Models.Card;
using MTGApplication.General.Services.ConfirmationService;
using System.Collections.Generic;

namespace MTGApplication.Features.DeckEditor;

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