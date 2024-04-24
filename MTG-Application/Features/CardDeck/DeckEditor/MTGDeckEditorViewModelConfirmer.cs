namespace MTGApplication.Features.CardDeck;

public class MTGDeckEditorViewModelConfirmer
{
  public Confirmation<bool?> SaveUnsavedChanges { get; set; } = new();
  public Confirmation<string, string[]> LoadDeck { get; set; } = new();
  public Confirmation<string, string> SaveDeck { get; set; } = new();
  public Confirmation<bool?> OverrideDeck { get; set; } = new();
}
