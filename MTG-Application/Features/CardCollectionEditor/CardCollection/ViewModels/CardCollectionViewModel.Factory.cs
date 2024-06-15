using MTGApplication.Features.CardCollection.Services;
using MTGApplication.General.Services.Databases.Repositories;
using MTGApplication.General.Services.Databases.Repositories.CardCollectionRepository;
using MTGApplication.General.Services.Databases.Repositories.CardCollectionRepository.Models;
using MTGApplication.General.Services.Importers.CardImporter;
using System;
using static MTGApplication.General.Services.NotificationService.NotificationService;

namespace MTGApplication.Features.CardCollection;

public partial class CardCollectionViewModel
{
  public class Factory
  {
    public CardCollectionConfirmers Confirmers { get; init; } = new();
    public Notifier Notifier { get; init; } = new();
    public IRepository<MTGCardCollectionDTO> Repository { get; init; } = new CardCollectionDTORepository();
    public Action OnDeleted { get; init; }
    public Action<MTGCardCollectionList> OnListAdded { get; init; }
    public Action<MTGCardCollectionList> OnListRemoved { get; init; }

    public CardCollectionViewModel Build(MTGCardCollection model, MTGCardImporter importer)
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
