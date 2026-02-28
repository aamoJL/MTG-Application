using MTGApplication.General.Services.ConfirmationService;

namespace MTGApplication.Features.DeckEditor.ViewModels.DeckCardGroup.GroupedCardList;

public partial class GroupedDeckCardListViewModel
{
  public static class GroupConfirmations
  {
    public static Confirmation<string[]> GetAddCardGroupConfirmation(string[] invalidNames)
    {
      return new(
        Title: "Add new group",
        Message: string.Empty,
        Data: invalidNames);
    }

    public static Confirmation<(string oldKey, string[] InvalidNames)> GetRenameCardGroupConfirmation(string oldName, string[] invalidNames)
    {
      return new(
        Title: "Rename group",
        Message: string.Empty,
        Data: (oldName, invalidNames));
    }
  }
}
