using MTGApplication.Features.CardCollection.Editor.ViewModels;
using MTGApplication.Features.CardCollectionEditor.CardCollection.Models;
using MTGApplication.Features.CardCollectionEditor.CardCollection.Services.Converters;
using MTGApplication.Features.CardCollectionEditor.Editor.Services;
using MTGApplicationTests.TestUtility.Database;
using MTGApplicationTests.TestUtility.Mocker;
using static MTGApplication.General.Services.NotificationService.NotificationService;

namespace MTGApplicationTests.FeatureTests.CardCollectionEditor.CardCollectionEditorViewModelTests;

public partial class CardCollectionEditorTests
{
  public class CardCollectionEditorViewModelTestsBase
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

    public CardCollectionEditorViewModelTestsBase()
      => _dependencies.ContextFactory.Populate(CardCollectionToDTOConverter.Convert(_savedCollection));

    public class Mocker(CardCollectionRepositoryDependencies dependencies)
    {
      public bool HasUnsavedChanges { get; internal set; } = false;
      public CardCollectionEditorConfirmers Confirmers { get; internal set; } = new();
      public Notifier Notifier { get; internal set; } = new();

      public CardCollectionEditorViewModel MockVM()
      {
        return new CardCollectionEditorViewModel(dependencies.Importer, Confirmers, Notifier, dependencies.Repository, new())
        {
          HasUnsavedChanges = HasUnsavedChanges,
        };
      }

      public async Task<CardCollectionEditorViewModel> MockVM(MTGCardCollection collection)
      {
        var viewmodel = MockVM();

        await viewmodel.ChangeCollection(collection);

        return viewmodel;
      }
    }
  }
}
