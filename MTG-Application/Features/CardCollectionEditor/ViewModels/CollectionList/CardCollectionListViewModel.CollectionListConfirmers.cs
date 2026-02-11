using MTGApplication.General.Services.ConfirmationService;
using System;
using System.Threading.Tasks;
using static MTGApplication.Features.CardCollectionEditor.ViewModels.CollectionCard.CardCollectionMTGCardViewModel;

namespace MTGApplication.Features.CardCollectionEditor.ViewModels.CollectionList;

public partial class CardCollectionListViewModel
{
  public class CollectionListConfirmers
  {
    public Func<Confirmation<(string name, string query)>, Task<(string name, string query)?>> ConfirmEditList { get => field ?? throw new NotImplementedException(); init; }
    public Func<Confirmation, Task<ConfirmationResult>> ConfirmEditQueryConflict { get => field ?? throw new NotImplementedException(); init; }
    public Func<Confirmation, Task<ConfirmationResult>> ConfirmListDelete { get => field ?? throw new NotImplementedException(); init; }
    public Func<Confirmation, Task<string?>> ConfirmCardImport { get => field ?? throw new NotImplementedException(); init; }
    public Func<Confirmation<string>, Task<string?>> ConfirmCardExport { get => field ?? throw new NotImplementedException(); init; }

    public CollectionCardConfirmers CardConfirmers { get; init; } = new();
  }
}
