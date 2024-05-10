using static MTGApplication.General.Services.ConfirmationService.ConfirmationService;

namespace MTGApplication.Features.DeckEditor;

public class DeckEditorConfirmers
{
  public Confirmer<ConfirmationResult> SaveUnsavedChanges { get; set; } = new();
  public Confirmer<string, string[]> LoadDeck { get; set; } = new();
  public Confirmer<string, string> SaveDeck { get; set; } = new();
  public Confirmer<ConfirmationResult> OverrideDeck { get; set; } = new();
  public Confirmer<ConfirmationResult> DeleteDeck { get; set; } = new();
}
