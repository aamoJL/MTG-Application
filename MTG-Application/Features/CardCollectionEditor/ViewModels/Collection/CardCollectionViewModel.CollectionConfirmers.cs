using MTGApplication.General.Services.ConfirmationService;
using System;
using System.Threading.Tasks;
using static MTGApplication.Features.CardCollectionEditor.ViewModels.CollectionList.CardCollectionListViewModel;

namespace MTGApplication.Features.CardCollectionEditor.ViewModels.Collection;

public partial class CardCollectionViewModel
{
  public class CollectionConfirmers
  {
    public Func<Confirmation, Task<ConfirmationResult>> ConfirmUnsavedChanges { get => field ?? throw new NotImplementedException(); set; }
    public Func<Confirmation<string>, Task<string?>> ConfirmCollectionSave { get => field ?? throw new NotImplementedException(); set; }
    public Func<Confirmation, Task<ConfirmationResult>> ConfirmCollectionSaveOverride { get => field ?? throw new NotImplementedException(); set; }
    public Func<Confirmation, Task<ConfirmationResult>> ConfirmCollectionDelete { get => field ?? throw new NotImplementedException(); set; }
    public Func<Confirmation, Task<(string name, string query)?>> ConfirmAddNewList { get => field ?? throw new NotImplementedException(); set; }

    public CollectionListConfirmers CollectionListConfirmers { get; init; } = new();
  }
}
