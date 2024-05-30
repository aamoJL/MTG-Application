using MTGApplication.Features.CardCollection;
using MTGApplication.General.Models.CardCollection;
using MTGApplicationTests.TestUtility.Database;
using MTGApplicationTests.TestUtility.Mocker;

namespace MTGApplicationTests.FeatureTests.CardCollection.CardCollectionViewModelTests;
public partial class CardCollectionViewModelTests
{
  public class CardCollectionViewModelTestsBase
  {
    protected readonly CollectionRepositoryDependencies _dependencies = new();
    protected readonly MTGCardCollection _savedCollection = new()
    {
      Name = "Saved Collection",
      CollectionLists = [
        new(){
          Name = "Saved List",
          SearchQuery = "Search Query",
          Cards = [
            MTGCardModelMocker.CreateMTGCardModel(name: "First"),
            MTGCardModelMocker.CreateMTGCardModel(name: "Second"),
            MTGCardModelMocker.CreateMTGCardModel(name: "Third")]}
        ]
    };

    public CardCollectionViewModelTestsBase()
      => _dependencies.ContextFactory.Populate(new MTGCardCollectionDTO(_savedCollection));

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
  }
}
