using MTGApplication.General.Services.ConfirmationService;

namespace MTGApplication.Features.CardCollectionEditor.ViewModels.CollectionList;

public partial class CardCollectionListViewModel
{
  public static class Confirmations
  {
    public static Confirmation GetDeleteCollectionListConfirmation(string name)
    {
      return new(
        Title: "Delete the list?",
        Message: $"Are you sure you want to delete '{name}'?");
    }

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
}
