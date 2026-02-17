using MTGApplication.General.Services.ConfirmationService;

namespace MTGApplication.Features.DeckEditor.ViewModels.DeckCardList;

public partial class DeckCardListViewModel
{
  public static class Confirmations
  {
    public static Confirmation GetAddSingleConflictConfirmation(string cardName)
    {
      return new(
        Title: "Card already exists in the list",
        Message: $"'{cardName}' already exists in the list. Do you still want to add it?");
    }

    public static Confirmation GetAddMultipleConflictConfirmation(string cardName)
    {
      return new(
        Title: "Card already exists in the list",
        Message: $"'{cardName}' already exists in the list. Do you still want to add it?");
    }

    public static Confirmation GetImportConfirmation()
    {
      return new(
        Title: "Import cards",
        Message: string.Empty);
    }

    public static Confirmation<string> GetExportConfirmation(string data)
    {
      return new(
        Title: "Export cards",
        Message: string.Empty,
        Data: data);
    }
  }
}
