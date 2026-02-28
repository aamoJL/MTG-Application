using MTGApplication.Features.CardCollectionEditor.ViewModels.Collection;
using MTGApplication.General.Services.ConfirmationService;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MTGApplication.Features.CardCollectionEditor.ViewModels.EditorPage;

public partial class CardCollectionEditorPageViewModel
{
  public class CollectionEditorPageConfirmers
  {
    public Func<Confirmation<IEnumerable<string>>, Task<string?>> ConfirmCollectionOpen { get => field ?? throw new NotImplementedException(); set; }

    public CardCollectionViewModel.CollectionConfirmers CollectionConfirmers { get; init; } = new();
  }
}