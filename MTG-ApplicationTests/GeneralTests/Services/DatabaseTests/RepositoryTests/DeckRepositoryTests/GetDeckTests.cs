﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTGApplication.General.Databases.Repositories.DeckRepository;
using MTGApplication.General.Models.CardDeck;
using MTGApplicationTests.Services;
using MTGApplicationTests.TestUtility;

namespace MTGApplicationTests.GeneralTests.Services.DatabaseTests.RepositoryTests.DeckRepositoryTests;
[TestClass]
public class GetDeckTests
{
  private readonly RepositoryDependencies _dependencies = new();
  private readonly MTGCardDeckDTO _savedDeck = MTGCardDeckDTOMocker.Mock("Saved Deck");

  public GetDeckTests() => _dependencies.ContextFactory.Populate(_savedDeck);

  [TestMethod("Should return saved deck with the given name")]
  public async Task Execute_Found_ReturnDeck()
  {
    var result = await new GetDeck(_dependencies.Repository, _dependencies.CardAPI).Execute(_savedDeck.Name);

    Assert.AreEqual(result.Name, _savedDeck.Name);
  }

  [TestMethod("Should return null if the deck was not found")]
  public async Task Execute_NotFound_ReturnNull()
  {
    var result = await new GetDeck(_dependencies.Repository, _dependencies.CardAPI).Execute("Unsaved Deck");

    Assert.IsNull(result);
  }
}
