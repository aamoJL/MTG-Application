﻿using MTGApplication.Features.CardCollection;
using MTGApplication.Features.CardCollection.Services;
using MTGApplication.General.Services.IOServices;
using MTGApplicationTests.TestUtility.Database;
using MTGApplicationTests.TestUtility.Mocker;
using static MTGApplication.General.Services.NotificationService.NotificationService;

namespace MTGApplicationTests.FeatureTests.CardCollection.CardCollectionViewModelTests;
public partial class CardCollectionListViewModelTests
{
  public class CardCollectionListViewModelTestsBase
  {
    protected readonly CardCollectionRepositoryDependencies _dependencies = new();
    protected readonly MTGCardCollectionList _savedList = new()
    {
      Name = "Saved List",
      SearchQuery = "Search Query",
      Cards = [
        new(MTGCardInfoMocker.MockInfo(name: "First")),
        new(MTGCardInfoMocker.MockInfo(name: "Second")),
        new(MTGCardInfoMocker.MockInfo(name: "Third"))]
    };

    public class Mocker(CardCollectionRepositoryDependencies dependencies)
    {
      public bool HasUnsavedChanges { get; set; } = false;
      public CardCollectionListConfirmers Confirmers { get; set; } = new();
      public MTGCardCollectionList Model { get; set; } = new();
      public Notifier Notifier { get; set; } = new();
      public Func<string, bool> ExistsValidator { get; init; } = (name) => false;
      public ClipboardService ClipboardService { get; set; } = new();

      public async Task<CardCollectionListViewModel> MockVM()
      {
        var viewmodel = await new CardCollectionListViewModel.Factory()
        {
          Notifier = Notifier,
          Confirmers = Confirmers,
          ClipboardService = ClipboardService
        }.Build(Model, dependencies.Importer, ExistsValidator);

        viewmodel.HasUnsavedChanges = HasUnsavedChanges;

        return viewmodel;
      }
    }
  }
}
