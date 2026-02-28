using MTGApplication.General.Services.ConfirmationService;

namespace MTGApplication.Features.CardCollectionEditor.ViewModels.Collection;

public partial class CardCollectionViewModel
{
  public static class Confirmations
  {
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

    public static Confirmation GetSaveUnsavedChangesConfirmation(string collectionName)
    {
      return new(
        Title: "Save unsaved changes?",
        Message: $"{(string.IsNullOrEmpty(collectionName) ? "Unnamed collection" : $"'{collectionName}'")} has unsaved changes. Would you like to save the collection?");
    }
  }
}
