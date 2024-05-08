
//  [TestMethod]
//  public async Task ImportToDeckCardsDialogTest()
//  {
//    var importedCards = new MTGCard[]
//    {
//      Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "First"),
//      Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "Second"),
//      Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "Third"),
//    };

//    DeckBuilderViewModel vm = new(new TestCardAPI(importedCards), null);
//    vm.DeckCards.Dialogs = new TestDeckCardlistViewDialogs(vm.DeckCards)
//    {
//      ImportDialog = new() { Values = nameof(importedCards) },
//    };

//    await vm.DeckCards.ImportToCardlistDialog();
//    Assert.AreEqual(3, vm.DeckCards.CardlistSize);
//  }

//  [TestMethod]
//  public async Task ImportToDeckCardsDialogTest_Undo()
//  {
//    var importedCards = new MTGCard[]
//    {
//      Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "First"),
//      Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "Second"),
//      Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "Third"),
//    };

//    DeckBuilderViewModel vm = new(new TestCardAPI(importedCards), null);
//    vm.DeckCards.Dialogs = new TestDeckCardlistViewDialogs(vm.DeckCards)
//    {
//      ImportDialog = new() { Values = nameof(importedCards) },
//    };

//    await vm.DeckCards.ImportToCardlistDialog();

//    vm.CommandService.Undo();
//    Assert.AreEqual(0, vm.DeckCards.CardlistSize);

//    vm.CommandService.Redo();
//    Assert.AreEqual(3, vm.DeckCards.CardlistSize);
//  }

//  [TestMethod]
//  public async Task ImportToDeckCardsDialogTest_AlreadyExists_Add()
//  {
//    var importedCards = new MTGCard[]
//    {
//      Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "First"),
//      Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "Second"),
//      Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "Third"),
//    };

//    DeckBuilderViewModel vm = new(new TestCardAPI(importedCards), null);
//    vm.DeckCards.Dialogs = new TestDeckCardlistViewDialogs(vm.DeckCards)
//    {
//      ImportDialog = new() { Values = nameof(importedCards) },
//      MultipleCardsAlreadyInDeckDialog = new() { Values = true },
//    };

//    await vm.DeckCards.ImportToCardlistDialog();
//    await vm.DeckCards.ImportToCardlistDialog();
//    Assert.AreEqual(6, vm.DeckCards.CardlistSize);
//  }

//  [TestMethod]
//  public async Task ImportToDeckCardsDialogTest_AlreadyExists_Skip()
//  {
//    var firstImport = new MTGCard[]
//    {
//      Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "First"),
//      Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "Second"),
//      Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "Third"),
//    };
//    var secondImport = new MTGCard[]
//    {
//      Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "First"),  // Old
//      Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "Second"), // Old
//      Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "Fourth"), // New
//    };

//    var cardAPI = new TestCardAPI(firstImport);
//    DeckBuilderViewModel vm = new(cardAPI, null);
//    vm.DeckCards.Dialogs = new TestDeckCardlistViewDialogs(vm.DeckCards)
//    {
//      ImportDialog = new() { Values = nameof(firstImport) },
//      MultipleCardsAlreadyInDeckDialog = new() { Result = ContentDialogResult.None, Values = true }
//    };

//    await vm.DeckCards.ImportToCardlistDialog();
//    cardAPI.ExpectedCards = secondImport;

//    await vm.DeckCards.ImportToCardlistDialog();
//    Assert.AreEqual(4, vm.DeckCards.CardlistSize);
//  }

//  [TestMethod]
//  public async Task ExportCardsDialogTest()
//  {
//    var cards = new System.Collections.ObjectModel.ObservableCollection<MTGCard>
//    {
//      Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "First", count: 1),
//      Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "Second", count: 2),
//      Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "Third", count: 3),
//    };
//    var deck = new MTGCardDeck()
//    {
//      DeckCards = cards,
//    };
//    var cardlist = new DeckCardlistViewModel(deck.DeckCards, null, null);
//    var expectedText = GetExportString(cardlist.Cardlist.ToArray(), "Name");

//    using TestIO.TestClipboard clipboard = new();
//    DeckBuilderViewModel vm = new(cardAPI: null, deckRepository: null, clipboardService: clipboard);
//    vm.DeckCards.Dialogs = new TestDeckCardlistViewDialogs(vm.DeckCards)
//    {
//      ExportDialog = new() { Values = expectedText },
//    };

//    foreach (var item in cards)
//    {
//      await vm.DeckCards.Add(item);
//    }

//    await vm.DeckCards.ExportDeckCardsDialog();
//    Assert.AreEqual(expectedText, clipboard.Content);
//  }
//  #endregion

//  #region HasUnsavedChanges
//  [TestMethod]
//  public void HasUnsavedChangesTest_Init() => Assert.IsFalse(new DeckBuilderViewModel(null, null, null).HasUnsavedChanges);

//  [TestMethod]
//  public async Task HasUnsavedChangesTest_AddCard()
//  {
//    DeckBuilderViewModel vm = new(null, null, null);

//    await vm.DeckCards.Add(Mocker.MTGCardModelMocker.CreateMTGCardModel());
//    Assert.IsTrue(vm.HasUnsavedChanges);
//  }

//  [TestMethod]
//  public async Task HasUnsavedChangesTest_IncreaseCount()
//  {
//    DeckBuilderViewModel vm = new(null, null, null);

//    var card = Mocker.MTGCardModelMocker.CreateMTGCardModel();
//    await vm.DeckCards.Add(card);
//    vm.HasUnsavedChanges = false;
//    Assert.IsFalse(vm.HasUnsavedChanges);

//    card.Count += 1;
//    Assert.IsTrue(vm.HasUnsavedChanges);
//  }

//  [TestMethod]
//  public async Task HasUnsaveChangesTest_ChangePrint()
//  {
//    var card = Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "First", scryfallId: Guid.NewGuid());
//    var newPrint = new MTGCardViewModel(Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "First", scryfallId: Guid.NewGuid()));
//    DeckBuilderViewModel vm = new(new TestCardAPI(), null);
//    vm.DeckCards.Dialogs = new TestDeckCardlistViewDialogs(vm.DeckCards)
//    {
//      CardPrintDialog = new() { Values = newPrint },
//    };

//    await vm.DeckCards.Add(card);
//    vm.HasUnsavedChanges = false; // Change unsaved state to false without saving
//    Assert.IsFalse(vm.HasUnsavedChanges);

//    await vm.DeckCards.ChangePrintDialog(card);
//    Assert.IsTrue(vm.HasUnsavedChanges);
//  }
//  #endregion

//  #region Adding and Removing
//  [TestMethod]
//  public async Task AddToCardlistCommandTest()
//  {
//    var deck = new MTGCardDeck();
//    var cardlist = new DeckCardlistViewModel(deck.DeckCards, null, null);

//    await cardlist.Add(Mocker.MTGCardModelMocker.CreateMTGCardModel());
//    Assert.AreEqual(1, deck.DeckCards.Count);
//  }

//  [TestMethod]
//  public async Task AddToCardlistCommandTest_AlreadyExists_Add()
//  {
//    var deck = new MTGCardDeck();
//    var cardlist = new DeckCardlistViewModel(deck.DeckCards, null);
//    cardlist.Dialogs = new TestDeckCardlistViewDialogs(cardlist)
//    {
//      CardAlreadyInDeckDialog = new(),
//    };

//    await cardlist.Add(Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "First", count: 1));
//    await cardlist.Add(Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "First", count: 2));
//    Assert.AreEqual(3, deck.DeckCards.Sum(x => x.Count));
//  }

//  [TestMethod]
//  public async Task AddToCardlistCommandTest_AlreadyExists_Skip()
//  {
//    var deck = new MTGCardDeck();
//    var cardlist = new DeckCardlistViewModel(deck.DeckCards, null);
//    cardlist.Dialogs = new TestDeckCardlistViewDialogs(cardlist)
//    {
//      CardAlreadyInDeckDialog = new() { Result = ContentDialogResult.None }
//    };

//    await cardlist.Add(Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "First", count: 1));
//    await cardlist.Add(Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "First", count: 2));
//    Assert.AreEqual(1, deck.DeckCards.Sum(x => x.Count));
//  }

//  [TestMethod]
//  public async Task AddToCardlistCommandTest_Undo()
//  {
//    var deck = new MTGCardDeck();
//    var cardlist = new DeckCardlistViewModel(deck.DeckCards, null, null);

//    await cardlist.Add(Mocker.MTGCardModelMocker.CreateMTGCardModel());

//    cardlist.CommandService.Undo();
//    Assert.AreEqual(0, deck.DeckCards.Count);

//    cardlist.CommandService.Redo();
//    Assert.AreEqual(1, deck.DeckCards.Count);
//  }

//  [TestMethod]
//  public async Task AddToCardlistCommandTest_Move_UndoRedo()
//  {
//    var commandService = new CommandService();
//    var deck = new MTGCardDeck();
//    var deckCards = new DeckCardlistViewModel(deck.DeckCards, null, null) { CommandService = commandService };
//    var wishlist = new DeckCardlistViewModel(deck.Wishlist, null, null) { CommandService = commandService };
//    var card = Mocker.MTGCardModelMocker.CreateMTGCardModel();

//    await deckCards.Add(card);

//    await wishlist.Move(card, deckCards);
//    Assert.AreEqual(0, deck.DeckCards.Count);
//    Assert.AreEqual(1, deck.Wishlist.Count);

//    commandService.Undo();
//    Assert.AreEqual(1, deck.DeckCards.Count);
//    Assert.AreEqual(0, deck.Wishlist.Count);

//    commandService.Redo();
//    Assert.AreEqual(0, deck.DeckCards.Count);
//    Assert.AreEqual(1, deck.Wishlist.Count);
//  }

//  [TestMethod]
//  public async Task RemoveFromCardlistCommandTest()
//  {
//    var deck = new MTGCardDeck();
//    var cardlist = new DeckCardlistViewModel(deck.DeckCards, null, null);
//    var card = Mocker.MTGCardModelMocker.CreateMTGCardModel();

//    await cardlist.Add(card);

//    cardlist.Remove(card);
//    Assert.AreEqual(0, deck.DeckCards.Count);
//  }

//  [TestMethod]
//  public async Task RemoveFromCardlistCommandTest_Undo_Redo()
//  {
//    var vm = new DeckBuilderViewModel(null, null, null);
//    var card = Mocker.MTGCardModelMocker.CreateMTGCardModel();

//    await vm.DeckCards.Add(card);
//    vm.DeckCards.Remove(card);

//    vm.CommandService.Undo();
//    Assert.AreEqual(1, vm.DeckCards.FilteredAndSortedCardViewModels.Count);

//    vm.CommandService.Redo();
//    Assert.AreEqual(0, vm.DeckCards.FilteredAndSortedCardViewModels.Count);
//  }

//  [TestMethod]
//  public async Task RemoveFromCardlistCommandTest_CardViewModelCommand()
//  {
//    var deck = new MTGCardDeck();
//    var cardlist = new DeckCardlistViewModel(deck.DeckCards, null, null);
//    var card = Mocker.MTGCardModelMocker.CreateMTGCardModel();

//    await cardlist.Add(card);
//    Assert.AreEqual(1, deck.DeckCards.Count);

//    ((MTGCardViewModel)cardlist.FilteredAndSortedCardViewModels[0]).DeleteCardCommand.Execute(card);
//    Assert.AreEqual(0, deck.DeckCards.Count);
//  }

//  [TestMethod]
//  public async Task RemoveFromCardlistCommandTest_Undo()
//  {
//    var deck = new MTGCardDeck();
//    var cardlist = new DeckCardlistViewModel(deck.DeckCards, null, null);
//    var card = Mocker.MTGCardModelMocker.CreateMTGCardModel();

//    await cardlist.Add(card);
//    cardlist.Remove(card);

//    cardlist.CommandService.Undo();
//    Assert.AreEqual(1, deck.DeckCards.Count);

//    cardlist.CommandService.Redo();
//    Assert.AreEqual(0, deck.DeckCards.Count);
//  }

//  [TestMethod]
//  public async Task ClearCardlistCommandTest()
//  {
//    var deck = new MTGCardDeck();
//    var cardlist = new DeckCardlistViewModel(deck.DeckCards, null, null);

//    await cardlist.Add(Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "1"));
//    await cardlist.Add(Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "2"));
//    await cardlist.Add(Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "3"));
//    Assert.AreEqual(3, cardlist.CardlistSize);

//    cardlist.Clear();
//    Assert.AreEqual(0, cardlist.CardlistSize);

//    cardlist.CommandService.Undo();
//    Assert.AreEqual(3, cardlist.CardlistSize);

//    cardlist.CommandService.Redo();
//    Assert.AreEqual(0, cardlist.CardlistSize);
//  }
//  #endregion

//  #region Price Changing
//  [TestMethod]
//  public async Task ChangePrintTest()
//  {
//    var card = Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "First", scryfallId: Guid.NewGuid());
//    var newPrint = new MTGCardViewModel(Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "First", scryfallId: Guid.NewGuid()));
//    DeckBuilderViewModel vm = new(new TestCardAPI(), null);
//    vm.DeckCards.Dialogs = new TestDeckCardlistViewDialogs(vm.DeckCards)
//    {
//      CardPrintDialog = new() { Values = newPrint }
//    };

//    await vm.DeckCards.Add(card);

//    await vm.DeckCards.ChangePrintDialog(card);
//    Assert.AreEqual(card.Info.ScryfallId, newPrint.Model.Info.ScryfallId);
//    Assert.AreEqual(newPrint.Model.Info.ScryfallId, ((MTGCardViewModel)vm.DeckCards.FilteredAndSortedCardViewModels[0]).Model.Info.ScryfallId);
//  }

//  [TestMethod]
//  public async Task DeckPriceChangesTest_CardlistChanges()
//  {
//    var cardlist = new DeckCardlistViewModel(new MTGCardDeck().DeckCards, null, null);
//    using var asserter = new PropertyChangedAssert(cardlist, nameof(cardlist.EuroPrice));

//    await cardlist.Add(Mocker.MTGCardModelMocker.CreateMTGCardModel(price: 500));
//    Assert.IsTrue(asserter.PropertyChanged);
//  }

//  [TestMethod]
//  public async Task DeckPriceChangesTest_CardCountChanges()
//  {
//    var card = Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "First", scryfallId: Guid.NewGuid());
//    var cardlist = new DeckCardlistViewModel(new MTGCardDeck() { DeckCards = new() { card } }.DeckCards, null, null);
//    DeckBuilderViewModel vm = new(new TestCardAPI(), null, null);

//    await vm.DeckCards.Add(card);

//    using var asserter = new PropertyChangedAssert(cardlist, nameof(cardlist.EuroPrice));
//    card.Count++;
//    Assert.IsTrue(asserter.PropertyChanged);
//  }

//  [TestMethod]
//  public async Task DeckPriceChangesTest_CardPrintChanges()
//  {
//    var card = Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "First", scryfallId: Guid.NewGuid());
//    var cardlist = new DeckCardlistViewModel(new MTGCardDeck { DeckCards = new() { card } }.DeckCards, null, null);
//    var newPrint = new MTGCardViewModel(Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "First", scryfallId: Guid.NewGuid()));
//    DeckBuilderViewModel vm = new(new TestCardAPI(), null);
//    vm.DeckCards.Dialogs = new TestDeckCardlistViewDialogs(vm.DeckCards)
//    {
//      CardPrintDialog = new() { Values = newPrint }
//    };

//    await vm.DeckCards.Add(card);

//    using var asserter = new PropertyChangedAssert(cardlist, nameof(cardlist.EuroPrice));
//    await vm.DeckCards.ChangePrintDialog(card);
//    Assert.IsTrue(asserter.PropertyChanged);
//  }
//  #endregion
//}