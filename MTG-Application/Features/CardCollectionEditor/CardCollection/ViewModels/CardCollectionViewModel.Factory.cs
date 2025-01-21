using MTGApplication.Features.CardCollectionEditor.CardCollection.Models;
using MTGApplication.Features.CardCollectionEditor.CardCollection.Services;
using MTGApplication.Features.CardCollectionEditor.CardCollectionList.Models;
using MTGApplication.General.Services.Databases.Repositories;
using MTGApplication.General.Services.Databases.Repositories.CardCollectionRepository;
using MTGApplication.General.Services.Databases.Repositories.CardCollectionRepository.Models;
using MTGApplication.General.Services.Importers.CardImporter;
using System;
using System.Threading.Tasks;
using static MTGApplication.General.Services.NotificationService.NotificationService;

namespace MTGApplication.Features.CardCollectionEditor.CardCollection.ViewModels;

public partial class CardCollectionViewModel
{
  public class Factory
  {
    public CardCollectionConfirmers Confirmers { get; init; } = new();
    public Notifier Notifier { get; init; } = new();
    public IRepository<MTGCardCollectionDTO> Repository { get; init; } = new CardCollectionDTORepository();

    public Func<Task>? OnDeleted { get; init; }
    public Func<MTGCardCollectionList, Task>? OnListAdded { get; init; }
    public Func<MTGCardCollectionList, Task>? OnListRemoved { get; init; }

    public CardCollectionViewModel Build(MTGCardCollection model, IMTGCardImporter importer)
    {
      var viewmodel = new CardCollectionViewModel(model, importer)
      {
        Confirmers = Confirmers,
        Notifier = Notifier,
        Repository = Repository,
        OnDelete = OnDeleted,
        OnListAdded = OnListAdded,
        OnListRemoved = OnListRemoved
      };

      return viewmodel;
    }
  }
}
