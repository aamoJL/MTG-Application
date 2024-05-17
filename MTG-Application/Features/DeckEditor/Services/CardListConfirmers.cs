using MTGApplication.General.Services.ConfirmationService;

namespace MTGApplication.Features.DeckEditor;

public class CardListConfirmers
{
  public Confirmer<string, string> ExportConfirmer { get; init; } = new();

  public static Confirmation<string> GetExportConfirmation(string data)
  {
    return new(
      Title: "Export deck",
      Message: string.Empty,
      Data: data);
  }
}
