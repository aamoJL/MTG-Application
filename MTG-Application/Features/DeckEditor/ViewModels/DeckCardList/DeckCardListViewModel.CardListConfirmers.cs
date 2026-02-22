using MTGApplication.Features.DeckEditor.ViewModels.DeckCard;
using MTGApplication.General.Services.ConfirmationService;
using System;
using System.Threading.Tasks;

namespace MTGApplication.Features.DeckEditor.ViewModels.DeckCardList;

public partial class DeckCardListViewModel
{
  public class CardListConfirmers
  {
    public Func<Confirmation, Task<ConfirmationResult>> ConfirmAddSingleConflict { get => field ?? throw new NotImplementedException(nameof(ConfirmAddSingleConflict)); set; }
    public Func<Confirmation, Task<(ConfirmationResult Result, bool SkipCheck)>> ConfirmAddMultipleConflict { get => field ?? throw new NotImplementedException(nameof(ConfirmAddMultipleConflict)); set; }
    public Func<Confirmation, Task<string?>> ConfirmImport { get => field ?? throw new NotImplementedException(nameof(ConfirmImport)); set; }
    public Func<Confirmation<string>, Task<string?>> ConfirmExport { get => field ?? throw new NotImplementedException(nameof(ConfirmExport)); set; }

    public DeckCardViewModel.CardConfirmers CardConfirmers { get; init; } = new();
  }
}
