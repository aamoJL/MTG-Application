using MTGApplication.General.Services.ConfirmationService;

namespace MTGApplication.Features.CardCollectionEditor.CardCollection.Services;

public class CardCollectionConfirmers
{
  public Confirmer<string, string> SaveCollectionConfirmer { get; init; } = new();
  public Confirmer<ConfirmationResult> OverrideCollectionConfirmer { get; init; } = new();
  public Confirmer<ConfirmationResult> DeleteCollectionConfirmer { get; init; } = new();
  public Confirmer<(string Name, string Query)?> NewCollectionListConfirmer { get; init; } = new();
  public Confirmer<ConfirmationResult> DeleteCollectionListConfirmer { get; init; } = new();

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
}
