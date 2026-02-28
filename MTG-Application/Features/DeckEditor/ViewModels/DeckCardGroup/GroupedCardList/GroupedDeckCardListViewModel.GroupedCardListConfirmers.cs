using MTGApplication.General.Services.ConfirmationService;
using System;
using System.Threading.Tasks;

namespace MTGApplication.Features.DeckEditor.ViewModels.DeckCardGroup.GroupedCardList;

public partial class GroupedDeckCardListViewModel
{
  public class GroupedCardListConfirmers
  {
    public Func<Confirmation<string[]>, Task<string?>> ConfirmAddGroup { get => field ?? throw new NotImplementedException(nameof(ConfirmAddGroup)); set; }
  }
}
