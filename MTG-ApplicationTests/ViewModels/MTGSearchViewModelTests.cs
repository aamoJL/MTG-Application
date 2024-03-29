﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTGApplication.Models;
using MTGApplication.ViewModels;
using MTGApplicationTests.API;
using MTGApplicationTests.Services;

namespace MTGApplicationTests.ViewModels;

[TestClass]
public partial class MTGSearchViewModelTests
{
  [TestMethod]
  public async Task SearchWithQueryCommandTest()
  {
    MTGAPISearch<MTGCardViewModelSource, MTGCardViewModel> vm = new(new TestCardAPI()
    {
      ExpectedCards = new MTGCard[]
      {
        Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "First"),
        Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "Seconds"),
        Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "Third"),
      }
    })
    {
      SearchQuery = "NotEmpty"
    };

    await vm.SearchWithQuery();
    Assert.IsTrue(vm.TotalCardCount > 0);

    vm.SearchQuery = string.Empty;
    await vm.SearchWithQuery();
    Assert.AreEqual(0, vm.TotalCardCount);
  }

  [TestMethod]
  public async Task SearchWithNamesCommandTest()
  {
    var expectedCards = new MTGCard[]
      {
        Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "First"),
        Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "Seconds"),
        Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "Third"),
      };

    MTGAPISearch<MTGCardViewModelSource, MTGCardViewModel> vm = new(new TestCardAPI()
    {
      ExpectedCards = expectedCards
    });

    await vm.SearchWithNames(expectedCards.Select(x => x.Info.Name).ToArray());
    Assert.AreEqual(expectedCards.Length, vm.TotalCardCount);
  }
}