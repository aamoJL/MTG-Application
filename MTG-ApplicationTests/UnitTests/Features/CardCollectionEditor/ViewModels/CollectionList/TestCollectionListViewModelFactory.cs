using MTGApplication.Features.CardCollectionEditor.Models;
using MTGApplication.Features.CardCollectionEditor.ViewModels.CollectionList;
using MTGApplication.General.ViewModels;
using MTGApplicationTests.TestUtility.Exporters;
using MTGApplicationTests.TestUtility.Importers;
using MTGApplicationTests.TestUtility.Services;
using static MTGApplication.Features.CardCollectionEditor.ViewModels.CollectionList.CardCollectionListViewModel;

namespace MTGApplicationTests.UnitTests.Features.CardCollectionEditor.ViewModels.CollectionList;

public class TestCollectionListViewModelFactory
{
  public Worker Worker { get; set; } = new();
  public SaveStatus SaveStatus { get; set; } = new();
  public TestStringExporter Exporter { get; set; } = new();
  public CollectionListConfirmers CollectionListConfirmers { get; set; } = new();
  public TestMTGCardImporter Importer { get; set; } = new();
  public TestNotifier Notifier { get; set; } = new NotImplementedNotifier();
  public TestNetworkService NetworkService { get; set; } = new();
  public Func<MTGCardCollectionList, Task> OnListDelete { get; set; } = (_) => throw new NotImplementedException();
  public bool? NameValidator { get => field ?? throw new NotImplementedException(); set; }

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
      NetworkService = NetworkService,
      NameValidator = (_) => NameValidator ?? throw new NotImplementedException(),
      OnDelete = OnListDelete
    };
  }
}
