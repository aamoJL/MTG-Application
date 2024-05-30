﻿using MTGApplication.General.Services.ConfirmationService;
using System.Collections.Generic;

namespace MTGApplication.Features.CardCollection;

public class CardCollectionConfirmers
{
  public Confirmer<ConfirmationResult> SaveUnsavedChangesConfirmer { get; init; } = new();
  public Confirmer<string, IEnumerable<string>> LoadCollectionConfirmer { get; init; } = new();

  public static Confirmation GetSaveUnsavedChangesConfirmation(string collectionName)
  {
    return new(
      Title: "Save unsaved changes?",
      Message: $"{(string.IsNullOrEmpty(collectionName) ? "Unnamed collection" : $"'{collectionName}'")} has unsaved changes. Would you like to save the collection?");
  }

  public static Confirmation<IEnumerable<string>> GetLoadCollectionConfirmation(IEnumerable<string> data)
  {
    return new(
      Title: "Open collection",
      Message: "Name",
      Data: data);
  }
}