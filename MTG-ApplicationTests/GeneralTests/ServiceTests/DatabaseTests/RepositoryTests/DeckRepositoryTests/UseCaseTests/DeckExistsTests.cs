﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTGApplication.General.Services.Databases.Repositories.DeckRepository.Models;
using MTGApplication.General.Services.Databases.Repositories.DeckRepository.UseCases;
using MTGApplicationTests.TestUtility.Database;
using MTGApplicationTests.TestUtility.Mocker;

namespace MTGApplicationTests.GeneralTests.ServiceTests.DatabaseTests.RepositoryTests.DeckRepositoryTests.UseCaseTests;

[TestClass]
public class DeckExistsTests
{
  private readonly DeckRepositoryDependencies _dependencies = new();
  private readonly MTGCardDeckDTO _savedDeck = MTGCardDeckDTOMocker.Mock("Deck");

  public DeckExistsTests() => _dependencies.ContextFactory.Populate(_savedDeck);

  [TestMethod("Should return true if a deck with the given name exists")]
  public async Task Execute_Exists_ReturnTrue()
  {
    var result = await new DeckDTOExists(_dependencies.Repository).Execute(_savedDeck.Name);

    Assert.IsTrue(result, "Should have returned true");
  }

  [TestMethod("Should return false if a deck with the given name does not exist")]
  public async Task Execute_NotFound_ReturnFalse()
  {
    var result = await new DeckDTOExists(_dependencies.Repository).Execute("Unsaved Deck");

    Assert.IsFalse(result, "Should have returned false");
  }
}