using MTGApplication.Features.CardCollectionEditor.Models;
using MTGApplication.General.Services.Exporters;
using MTGApplication.General.Services.Importers.CardImporter;
using MTGApplication.General.Services.NotificationService;
using MTGApplication.General.ViewModels;
using System;
using System.Threading.Tasks;

namespace MTGApplication.Features.CardCollectionEditor.ViewModels.CollectionList;

public partial class CardCollectionListViewModel
{
  public class Factory
  {
    public required Worker Worker { get; init; }
    public required SaveStatus SaveStatus { get; init; }
    public required Func<string, bool> NameValidator { get; init; }
    public required Func<MTGCardCollectionList, Task> OnListDelete { get; init; }
    public required IExporter<string> Exporter { get; init; }
    public required CollectionListConfirmers CollectionListConfirmers { get; init; }
    public required IMTGCardImporter Importer { get; init; }
    public required NotificationService.Notifier Notifier { get; init; }

    public CardCollectionListViewModel Build(MTGCardCollectionList list)
    {
      return new(list)
      {
        Worker = Worker,
        SaveStatus = SaveStatus,
        Exporter = Exporter,
        Confirmers = CollectionListConfirmers,
        Importer = Importer,
        Notifier = Notifier,
        NameValidator = NameValidator,
        OnDelete = OnListDelete
      };
    }
  }
}
