using MTGApplication.General.Services.ConfirmationService;

namespace MTGApplication.Features.DeckEditor.ViewModels.DeckCardGroup.GroupedCardList;

public partial class GroupedDeckCardListViewModel
{
  public static class GroupConfirmations
  {
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
}
