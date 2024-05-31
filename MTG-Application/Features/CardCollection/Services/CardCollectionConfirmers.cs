using MTGApplication.General.Services.ConfirmationService;
using System.Collections.Generic;

namespace MTGApplication.Features.CardCollection;

public class CardCollectionConfirmers
{
  public Confirmer<ConfirmationResult> SaveUnsavedChangesConfirmer { get; init; } = new();
  public Confirmer<string, IEnumerable<string>> LoadCollectionConfirmer { get; init; } = new();
  public Confirmer<string, string> SaveCollectionConfirmer { get; init; } = new();
  public Confirmer<ConfirmationResult> OverrideCollectionConfirmer { get; init; } = new();

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

  public static Confirmation<string> GetSaveCollectionConfirmation(string oldName)
  {
    return new(
      Title: "Save your collection?",
      Message: string.Empty,
      Data: oldName);
  }

  public static Confirmation GetOverrideCollectionConfirmation(string saveName)
  {
    return new(
      Title: "Override existing collection?",
      Message: $"Collection '{saveName}' already exist. Would you like to override the collection?");
  }
}