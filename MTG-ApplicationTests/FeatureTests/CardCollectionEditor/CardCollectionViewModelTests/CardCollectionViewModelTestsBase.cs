using MTGApplication.Features.CardCollectionEditor.CardCollection.Models;
using MTGApplication.Features.CardCollectionEditor.CardCollection.Services;
using MTGApplication.Features.CardCollectionEditor.CardCollection.Services.Converters;
using MTGApplication.Features.CardCollectionEditor.CardCollection.ViewModels;
using MTGApplication.Features.CardCollectionEditor.CardCollectionList.Models;
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

    public class Mocker(CardCollectionRepositoryDependencies dependencies)
    {
      public bool HasUnsavedChanges { get; set; } = false;
      public CardCollectionConfirmers Confirmers { get; set; } = new();
      public MTGCardCollection Model { get; set; } = new();
      public Notifier Notifier { get; set; } = new();
      public Func<Task> OnDeleted { get; init; }
      public Func<MTGCardCollectionList, Task> OnListAdded { get; init; }
      public Func<MTGCardCollectionList, Task> OnListRemoved { get; init; }

      public CardCollectionViewModel MockVM()
      {
        var viewmodel = new CardCollectionViewModel.Factory()
        {
          Repository = dependencies.Repository,
          Confirmers = Confirmers,
          Notifier = Notifier,
          OnListAdded = OnListAdded,
          OnListRemoved = OnListRemoved,
          OnDeleted = OnDeleted
        }.Build(Model, dependencies.Importer);

        viewmodel.HasUnsavedChanges = HasUnsavedChanges;

        return viewmodel;
      }
    }
  }
}
