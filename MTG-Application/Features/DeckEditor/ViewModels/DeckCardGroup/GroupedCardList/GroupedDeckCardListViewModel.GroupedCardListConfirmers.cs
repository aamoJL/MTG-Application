using MTGApplication.Features.DeckEditor.ViewModels.DeckCardGroup.CardGroup;
using MTGApplication.General.Services.ConfirmationService;
using System;
using System.Threading.Tasks;

namespace MTGApplication.Features.DeckEditor.ViewModels.DeckCardGroup.GroupedCardList;

public partial class GroupedDeckCardListViewModel
{
  public class GroupedCardListConfirmers
  {
    public Func<Confirmation, Task<string?>> ConfirmAddGroup { get => field ?? throw new NotImplementedException(); set; }

    public DeckCardGroupViewModel.GroupConfirmers GroupConfirmers { get; init; } = new();
  }
}
