using MTGApplication.Features.CardCollection;
using MTGApplication.Features.CardCollection.Services;
using MTGApplication.Features.CardCollection.Services.Converters;
using MTGApplication.General.Services.IOServices;
using MTGApplicationTests.TestUtility.Database;
using MTGApplicationTests.TestUtility.Mocker;
using static MTGApplication.General.Services.NotificationService.NotificationService;

namespace MTGApplicationTests.FeatureTests.CardCollection.CardCollectionViewModelTests;
public partial class CardCollectionViewModelTests
{
  public class CardCollectionViewModelTestsBase
  {
    protected readonly CardCollectionRepositoryDependencies _dependencies = new();
    protected readonly MTGCardCollection _savedCollection = new()
    {
      Name = "Saved Collection",
      CollectionLists = [
        new(){
          Name = "Saved List",
          SearchQuery = "Search Query",
          Cards = [
            new(MTGCardInfoMocker.MockInfo(name: "First")),
            new(MTGCardInfoMocker.MockInfo(name: "Second")),
            new(MTGCardInfoMocker.MockInfo(name: "Third"))]},
        new(){
          Name = "Saved List 2",
          SearchQuery = "Search Query 2",
          Cards = [
            new(MTGCardInfoMocker.MockInfo(name: "First"))]}
        ]
    };

    public CardCollectionViewModelTestsBase() => _dependencies.ContextFactory.Populate(CardCollectionToDTOConverter.Convert(_savedCollection));

    [Obsolete("Use Mocker instead")]
    public CardCollectionViewModel MockVM(
      CardCollectionConfirmers? confirmers = null,
      bool hasUnsavedChanges = false,
      MTGCardCollection? collection = null)
    {
      return new(_dependencies.CardAPI)
      {
        Repository = _dependencies.Repository,
        Confirmers = confirmers ?? new(),
        HasUnsavedChanges = hasUnsavedChanges,
        Collection = collection ?? new()
      };
    }

    public class Mocker(CardCollectionRepositoryDependencies dependencies)
    {
      public bool HasUnsavedChanges { get; set; } = false;
      public CardCollectionConfirmers Confirmers { get; set; } = new();
      public MTGCardCollection Collection { get; set; } = new();
      public Notifier Notifier { get; set; } = new();
      public ClipboardService ClipboardService { get; set; } = new();

      public CardCollectionViewModel MockVM()
      {
        return new(dependencies.CardAPI)
        {
          Repository = dependencies.Repository,
          Confirmers = Confirmers,
          HasUnsavedChanges = HasUnsavedChanges,
          Collection = Collection,
          Notifier = Notifier,
          ClipboardService = ClipboardService
        };
      }
    }
  }
}
