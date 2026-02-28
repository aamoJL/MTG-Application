using MTGApplication.General.Services.ConfirmationService;
using System;
using System.Threading.Tasks;

namespace MTGApplication.Features.DeckEditor.ViewModels.DeckCardGroup.CardGroup;

public partial class DeckCardGroupViewModel
{
  public class GroupConfirmers
  {
    public Func<Confirmation<(string OldKey, string[] InvalidNames)>, Task<string?>> ConfirmRenameGroup { get => field ?? throw new NotImplementedException(nameof(ConfirmRenameGroup)); set; }
  }
}