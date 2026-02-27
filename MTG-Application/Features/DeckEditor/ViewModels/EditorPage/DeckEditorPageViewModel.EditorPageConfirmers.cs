using MTGApplication.General.Services.ConfirmationService;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MTGApplication.Features.DeckEditor.ViewModels.EditorPage;

public partial class DeckEditorPageViewModel
{
  public class EditorPageConfirmers
  {
    public Func<Confirmation<IEnumerable<string>>, Task<string?>> ConfirmDeckOpen { get => field ?? throw new NotImplementedException(nameof(ConfirmDeckOpen)); set; }
  }
}