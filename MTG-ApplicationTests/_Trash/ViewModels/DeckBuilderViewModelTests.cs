//  [TestMethod]
//  public async Task LoadDeckDialogCommandTest_IsSorted()
//  {
//    var cards = new List<MTGCard>()
//    {
//      Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "First", cmc: 3),
//      Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "Second", cmc: 2),
//      Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "Third", cmc: 1),
//    };
//    var dbDeck = new MTGCardDeck()
//    {
//      Name = "First",
//      DeckCards = new(cards),
//    };

//    using TestInMemoryMTGDeckRepository repo = new(new TestCardAPI()
//    {
//      ExpectedCards = cards.ToArray()
//    });
//    DeckBuilderViewModel vm = new(null, repo);
//    vm.Dialogs = new TestDeckBuilderViewDialogs(vm) // Override dialogs
//    {
//      LoadDialog = new() { Values = dbDeck.Name }, // Load deck
//    };

//    await repo.Add(dbDeck);

//    vm.SortProperties.SortDirection = SortDirection.Ascending;
//    vm.SortProperties.PrimarySortProperty = MTGSortProperty.CMC;
//    vm.SortProperties.SecondarySortProperty = MTGSortProperty.Name;
//    var sortedDbCards = dbDeck.DeckCards.OrderBy(x => x.Info.CMC).ThenBy(x => x.Info.Name).ToList();

//    await vm.LoadDeckDialog();
//    CollectionAssert.AreEqual(sortedDbCards.Select(x => x.Info.CMC).ToArray(), vm.DeckCards.FilteredAndSortedCardViewModels.Select(x => ((MTGCardViewModel)x).Model.Info.CMC).ToArray());
//  }



//  [TestMethod]
//  public void SetCommanderCommandTest()
//  {
//    var vm = new DeckBuilderViewModel(null, null, null);
//    var commanderCard = Mocker.MTGCardModelMocker.CreateMTGCardModel();

//    vm.SetCommander(commanderCard);
//    Assert.AreEqual(commanderCard, vm.CardDeck.Commander);
//  }

//  [TestMethod]
//  public void SetCommanderCommandTest_UndoRedo()
//  {
//    var vm = new DeckBuilderViewModel(null, null, null);
//    var commanderCard = Mocker.MTGCardModelMocker.CreateMTGCardModel();

//    vm.SetCommander(commanderCard);
//    Assert.AreEqual(commanderCard, vm.CardDeck.Commander);

//    vm.CommandService.Undo();
//    Assert.AreEqual(null, vm.CardDeck.Commander);

//    vm.CommandService.Redo();
//    Assert.AreEqual(commanderCard, vm.CardDeck.Commander);
//  }



//  [TestMethod]
//  public void SetCommanderPartnerCommandTest()
//  {
//    var vm = new DeckBuilderViewModel(null, null, null);
//    var PartnerCard = Mocker.MTGCardModelMocker.CreateMTGCardModel();

//    vm.SetCommanderPartner(PartnerCard);
//    Assert.AreEqual(PartnerCard, vm.CardDeck.CommanderPartner);
//  }




//  [TestMethod]
//  public void SetCommanderPartnerCommandTest_UndoRedo()
//  {
//    var vm = new DeckBuilderViewModel(null, null, null);
//    var PartnerCard = Mocker.MTGCardModelMocker.CreateMTGCardModel();

//    vm.SetCommanderPartner(PartnerCard);
//    Assert.AreEqual(PartnerCard, vm.CardDeck.CommanderPartner);

//    vm.CommandService.Undo();
//    Assert.AreEqual(null, vm.CardDeck.CommanderPartner);

//    vm.CommandService.Redo();
//    Assert.AreEqual(PartnerCard, vm.CardDeck.CommanderPartner);
//  }




//  [TestMethod]
//  public async Task DeckPriceTest()
//  {
//    var vm = new DeckBuilderViewModel(null, null, null);
//    var card = Mocker.MTGCardModelMocker.CreateMTGCardModel(price: 1.5f);

//    using var propAsserter = new PropertyChangedAssert(vm, nameof(vm.DeckPrice));
//    await vm.DeckCards.Add(card);
//    Assert.AreEqual(card.Info.Price, vm.DeckPrice); // 1.5€
//    Assert.IsTrue(propAsserter.PropertyChanged);

//    propAsserter.Reset();
//    card.Count += 1;
//    Assert.AreEqual(card.Info.Price * 2, vm.DeckPrice); // 3€
//    Assert.IsTrue(propAsserter.PropertyChanged);

//    propAsserter.Reset();
//    vm.SetCommander(Mocker.MTGCardModelMocker.CreateMTGCardModel(price: 1));
//    Assert.AreEqual(card.Info.Price * 2 + 1, vm.DeckPrice); // 4€
//    Assert.IsTrue(propAsserter.PropertyChanged);

//    propAsserter.Reset();
//    vm.SetCommanderPartner(Mocker.MTGCardModelMocker.CreateMTGCardModel(price: 3));
//    Assert.AreEqual(card.Info.Price * 2 + 1 + 3, vm.DeckPrice); // 7€
//    Assert.IsTrue(propAsserter.PropertyChanged);

//    propAsserter.Reset();
//    vm.SetCommander(null);
//    Assert.AreEqual(card.Info.Price * 2 + 3, vm.DeckPrice); // 6€
//    Assert.IsTrue(propAsserter.PropertyChanged);
//  }