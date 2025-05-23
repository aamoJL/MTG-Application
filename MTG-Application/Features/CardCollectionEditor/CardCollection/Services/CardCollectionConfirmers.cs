﻿using MTGApplication.General.Models;
using MTGApplication.General.Services.ConfirmationService;
using System.Collections.Generic;

namespace MTGApplication.Features.CardCollectionEditor.CardCollection.Services;

public class CardCollectionConfirmers
{
  public Confirmer<ConfirmationResult> SaveUnsavedChangesConfirmer { get; init; } = new();
  public Confirmer<string, IEnumerable<string>> LoadCollectionConfirmer { get; init; } = new();
  public Confirmer<string, string> SaveCollectionConfirmer { get; init; } = new();
  public Confirmer<ConfirmationResult> OverrideCollectionConfirmer { get; init; } = new();
  public Confirmer<ConfirmationResult> DeleteCollectionConfirmer { get; init; } = new();
  public Confirmer<(string Name, string Query)?> NewCollectionListConfirmer { get; init; } = new();
  public Confirmer<ConfirmationResult> DeleteCollectionListConfirmer { get; init; } = new();
  public Confirmer<MTGCard, IEnumerable<MTGCard>> ShowCardPrintsConfirmer { get; init; } = new();

  public static Confirmation<string> GetSaveCollectionConfirmation(string oldName)
  {
    return new(
      Title: "Save your collection?",
      Message: string.Empty,
      Data: oldName);
  }

  public static Confirmation GetDeleteCollectionConfirmation(string name)
  {
    return new(
      Title: "Delete the collection?",
      Message: $"Are you sure you want to delete '{name}'?");
  }

  public static Confirmation GetOverrideCollectionConfirmation(string saveName)
  {
    return new(
      Title: "Override existing collection?",
      Message: $"Collection '{saveName}' already exist. Would you like to override the collection?");
  }

  public static Confirmation GetNewCollectionListConfirmation()
  {
    return new(
      Title: "Add new list",
      Message: string.Empty);
  }

  public static Confirmation GetDeleteCollectionListConfirmation(string name)
  {
    return new(
      Title: "Delete the list?",
      Message: $"Are you sure you want to delete '{name}'?");
  }

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

  public static Confirmation<IEnumerable<MTGCard>> GetShowCardPrintsConfirmation(IEnumerable<MTGCard> data)
  {
    return new(
      Title: "Card prints",
      Message: string.Empty,
      Data: data);
  }
}
