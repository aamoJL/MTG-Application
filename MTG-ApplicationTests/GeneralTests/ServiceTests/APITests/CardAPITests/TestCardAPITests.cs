﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTGApplication.General.Services.Databases.Repositories.CardRepository.Models;
using MTGApplication.General.Services.Importers.CardImporter;
using MTGApplicationTests.TestUtility.Importers;
using MTGApplicationTests.TestUtility.Mocker;

namespace MTGApplicationTests.GeneralTests.ServiceTests.APITests.CardAPITests;

[TestClass]
public class TestCardAPITests
{
  [TestMethod]
  public async Task FetchCardsTest()
  {
    var api = new TestMTGCardImporter();
    Assert.AreEqual(0, (await api.ImportCardsWithSearchQuery(string.Empty)).Found.Length);
    Assert.AreEqual(0, (await api.ImportCardsWithSearchQuery("params")).Found.Length);

    api.ExpectedCards =
    [
      new CardImportResult.Card(MTGCardInfoMocker.MockInfo()),
    ];
    Assert.AreEqual(0, (await api.ImportCardsWithSearchQuery(string.Empty)).Found.Length);
    Assert.AreEqual(1, (await api.ImportCardsWithSearchQuery("params")).Found.Length);
  }

  [TestMethod]
  public async Task FetchFromDTOsTest()
  {
    var api = new TestMTGCardImporter();
    var expectedCards = new List<CardImportResult.Card>
    {
      new(MTGCardInfoMocker.MockInfo()),
      new(MTGCardInfoMocker.MockInfo()),
      new(MTGCardInfoMocker.MockInfo()),
    };
    //No expected cards
    var dtos = expectedCards.Select(x => new MTGCardDTO(x.Info)).ToArray();
    Assert.AreEqual(3, (await api.ImportWithDTOs(dtos)).Found.Length);

    // Expected cards are same as DTOs
    api.ExpectedCards = [.. expectedCards];
    Assert.AreEqual(3, (await api.ImportWithDTOs(dtos)).Found.Length);
    Assert.AreEqual(0, (await api.ImportWithDTOs(dtos)).NotFoundCount);

    // Expected cards has one more card than DTOs
    expectedCards.Add(new(MTGCardInfoMocker.MockInfo()));
    api.ExpectedCards = [.. expectedCards];
    Assert.AreEqual(3, (await api.ImportWithDTOs(dtos)).Found.Length);
    Assert.AreEqual(1, (await api.ImportWithDTOs(dtos)).NotFoundCount);
  }

  [TestMethod]
  public async Task FetchFromStringTest()
  {
    var api = new TestMTGCardImporter();
    var cards = new List<CardImportResult.Card>
    {
      new(MTGCardInfoMocker.MockInfo()),
      new(MTGCardInfoMocker.MockInfo()),
      new(MTGCardInfoMocker.MockInfo()),
    };

    //No expected cards
    Assert.AreEqual(0, (await api.ImportWithString("")).Found.Length);

    // Expected cards
    api.ExpectedCards = [.. cards];
    api.NotFoundCount = 4;
    Assert.AreEqual(3, (await api.ImportWithString("")).Found.Length);
    Assert.AreEqual(4, (await api.ImportWithString("")).NotFoundCount);
  }
}
