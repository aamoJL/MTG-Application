using MTGApplication.General.Services.ConfirmationService;
using System.Collections.Generic;

namespace MTGApplication.Features.CardCollectionEditor.ViewModels.EditorPage;

public partial class CardCollectionEditorPageViewModel
{
  public static class Confirmations
  {
    public static Confirmation<IEnumerable<string>> GetLoadCollectionConfirmation(IEnumerable<string> data)
    {
      return new(
        Title: "Open collection",
        Message: "Name",
        Data: data);
    }
  }
}