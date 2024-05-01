﻿using MTGApplication.Features.CardDeck;
using MTGApplication.General.Models.CardDeck;
using MTGApplicationTests.Services;
using MTGApplicationTests.TestUtility;

namespace MTGApplicationTests.FeatureTests.CardDeckTests.DeckEditorTests;

public abstract class DeckEditorViewModelTestsBase
{
  protected readonly RepositoryDependencies _dependencies = new();
  protected readonly MTGCardDeck _savedDeck = MTGCardDeckMocker.Mock("Saved Deck");

  public DeckEditorViewModelTestsBase()
    => _dependencies.ContextFactory.Populate(new MTGCardDeckDTO(_savedDeck));

  protected DeckEditorViewModel MockVM(
    DeckEditorConfirmers? confirmers = null,
    bool hasUnsavedChanges = false,
    MTGCardDeck? deck = null)
  {
    var vm = new DeckEditorViewModel
    {
      CardAPI = _dependencies.CardAPI,
      Repository = _dependencies.Repository,
      Confirmers = confirmers ?? new(),
      Deck = deck ?? new(),
      HasUnsavedChanges = hasUnsavedChanges
    };

    return vm;
  }
}