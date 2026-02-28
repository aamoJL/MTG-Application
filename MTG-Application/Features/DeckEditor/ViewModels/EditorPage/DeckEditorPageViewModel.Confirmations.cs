using MTGApplication.General.Services.ConfirmationService;
using System.Collections.Generic;

namespace MTGApplication.Features.DeckEditor.ViewModels.EditorPage;

public partial class DeckEditorPageViewModel
{
  public static class Confirmations
  {
    public static Confirmation<IEnumerable<string>> GetOpenDeckConfirmation(IEnumerable<string> data)
    {
      return new(
        Title: "Open deck",
        Message: "Name",
        Data: data);
    }
  }
}