using MTGApplication.Features.CardCollectionEditor.Models;
using MTGApplication.Features.CardCollectionEditor.ViewModels.Collection;
using MTGApplication.General.Services.Databases.Repositories.CardCollectionRepository.Models;
using MTGApplication.General.ViewModels;
using MTGApplicationTests.TestUtility.Database;
using MTGApplicationTests.TestUtility.Exporters;
using MTGApplicationTests.TestUtility.Importers;
using MTGApplicationTests.TestUtility.Services;
using static MTGApplication.Features.CardCollectionEditor.ViewModels.Collection.CardCollectionViewModel;

namespace MTGApplicationTests.UnitTests.Features.CardCollectionEditor.ViewModels.Collection;

public class TestCollectionViewModelFactory
{
  public Worker Worker { get; set; } = new();
  public SaveStatus SaveStatus { get; set; } = new();
  public CollectionConfirmers CollectionConfirmers { get; set; } = new();
  public TestNotifier Notifier { get; set; } = new NotImplementedNotifier();
  public TestRepository<MTGCardCollectionDTO> Repository { get; set; } = new();
  public TestStringExporter Exporter { get; set; } = new();
  public TestMTGCardImporter Importer { get; set; } = new();
  public Func<Task> OnCollectionDeleted { get; set; } = () => throw new NotImplementedException();

  public CardCollectionViewModel Build(MTGCardCollection collection)
  {
    return new(collection)
    {
      Worker = Worker,
      SaveStatus = SaveStatus,
      Confirmers = CollectionConfirmers,
      Notifier = Notifier,
      Repository = Repository,
      Exporter = Exporter,
      Importer = Importer,
      OnDeleted = OnCollectionDeleted,
    };
  }
}
