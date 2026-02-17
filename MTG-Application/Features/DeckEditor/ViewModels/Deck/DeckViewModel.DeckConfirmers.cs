using MTGApplication.Features.DeckEditor.ViewModels.DeckCardGroup.GroupedCardList;
using MTGApplication.Features.DeckEditor.ViewModels.DeckCardList;
using MTGApplication.General.Models;
using MTGApplication.General.Services.ConfirmationService;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MTGApplication.Features.DeckEditor.ViewModels.Deck;

public partial class DeckViewModel
{
  public class DeckConfirmers
  {
    public Func<Confirmation, Task<ConfirmationResult>> ConfirmUnsavedChanges { get => field ?? throw new NotImplementedException(nameof(ConfirmUnsavedChanges)); set; }
    public Func<Confirmation<string>, Task<string?>> ConfirmDeckSave { get => field ?? throw new NotImplementedException(nameof(ConfirmDeckSave)); set; }
    public Func<Confirmation, Task<ConfirmationResult>> ConfirmDeckSaveOverride { get => field ?? throw new NotImplementedException(nameof(ConfirmDeckSaveOverride)); set; }
    public Func<Confirmation, Task<ConfirmationResult>> ConfirmDeckDelete { get => field ?? throw new NotImplementedException(nameof(ConfirmDeckDelete)); set; }
    public Func<Confirmation<IEnumerable<MTGCard>>, Task> ConfirmDeckTokens { get => field ?? throw new NotImplementedException(nameof(ConfirmDeckTokens)); set; }

    public DeckCardListViewModel.CardListConfirmers ListConfirmers { get; init; } = new();
    public GroupedDeckCardListViewModel.GroupedCardListConfirmers GroupListConfirmers { get; init; } = new();
  }
}