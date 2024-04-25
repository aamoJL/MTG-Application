using MTGApplication.General.Services.ConfirmationService;

namespace MTGApplication.Features.CardDeck;

public class MTGDeckEditorViewModelConfirmer
{
  public Confirmation<ConfirmationResult> SaveUnsavedChanges { get; set; } = new();
  public Confirmation<string, string[]> LoadDeck { get; set; } = new();
  public Confirmation<string, string> SaveDeck { get; set; } = new();
  public Confirmation<ConfirmationResult> OverrideDeck { get; set; } = new();
}
