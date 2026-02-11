using MTGApplication.Features.CardCollectionEditor.Models;
using MTGApplication.General.Services.Databases.Repositories;
using MTGApplication.General.Services.Databases.Repositories.CardCollectionRepository.Models;
using MTGApplication.General.Services.Exporters;
using MTGApplication.General.Services.Importers.CardImporter;
using MTGApplication.General.Services.NotificationService;
using MTGApplication.General.ViewModels;
using System;
using System.Threading.Tasks;

namespace MTGApplication.Features.CardCollectionEditor.ViewModels.Collection;

public partial class CardCollectionViewModel
{
  public class Factory
  {
    public required Worker Worker { get; init; }
    public required Func<Task> OnCollectionDeleted { get; init; }
    public required CollectionConfirmers CollectionConfirmers { get; init; }
    public required NotificationService.Notifier Notifier { get; init; }
    public required IRepository<MTGCardCollectionDTO> Repository { get; init; }
    public required IExporter<string> Exporter { get; init; }
    public required IMTGCardImporter Importer { get; init; }

    public CardCollectionViewModel Build(MTGCardCollection collection)
    {
      return new(collection)
      {
        Worker = Worker,
        Confirmers = CollectionConfirmers,
        Notifier = Notifier,
        Repository = Repository,
        Exporter = Exporter,
        Importer = Importer,
        OnDeleted = OnCollectionDeleted,
      };
    }
  }
}