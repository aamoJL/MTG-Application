using CommunityToolkit.WinUI.UI;
using Microsoft.UI.Xaml.Controls;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTGApplication.Models;
using MTGApplication.Services;
using MTGApplication.ViewModels;
using MTGApplicationTests.API;
using MTGApplicationTests.Services;
using static MTGApplication.Enums;
using static MTGApplication.Services.DialogService;
using static MTGApplication.Services.MTGService;
using static MTGApplication.ViewModels.DeckCardlistViewModel;
using static MTGApplicationTests.Services.TestDialogService;

namespace MTGApplicationTests.ViewModels;

[TestClass]
public partial class DeckCardlistViewModelTests
{
  public class TestDeckCardlistViewDialogs : DeckCardlistViewDialogs
  {
    public TestDeckCardlistViewDialogs(DeckCardlistViewModel vm) => ViewModel = vm;

    public DeckCardlistViewModel ViewModel { get; }

    public TestDialogResult<MTGCardViewModel> CardPrintDialog { get; set; } = new();
    public TestDialogResult<string> ImportDialog { get; set; } = new();
    public TestDialogResult<string> ExportDialog { get; set; } = new();
    public TestDialogResult<bool?> MultipleCardsAlreadyInDeckDialog { get; set; } = new();
    public TestDialogResult CardAlreadyInDeckDialog { get; set; } = new();

    public override GridViewDialog<MTGCardViewModel> GetCardPrintDialog(MTGCardViewModel[] printViewModels)
    {
      ViewModel.OnGetDialogWrapper += (s, args) => args.DialogWrapper = new TestDialogWrapper(CardPrintDialog.Result);
      var dialog = base.GetCardPrintDialog(printViewModels);
      dialog.Selection = CardPrintDialog.Values;
      return dialog;
    }
    public override ConfirmationDialog GetCardAlreadyInCardlistDialog(string cardName, string listName = "")
    {
      ViewModel.OnGetDialogWrapper += (s, args) => args.DialogWrapper = new TestDialogWrapper(CardAlreadyInDeckDialog.Result);
      var dialog = base.GetCardAlreadyInCardlistDialog(cardName, listName);
      return dialog;
    }
    public override CheckBoxDialog GetMultipleCardsAlreadyInDeckDialog(string name)
    {
      ViewModel.OnGetDialogWrapper += (s, args) => args.DialogWrapper = new TestDialogWrapper(MultipleCardsAlreadyInDeckDialog.Result);
      var dialog = base.GetMultipleCardsAlreadyInDeckDialog(name);
      dialog.IsChecked = MultipleCardsAlreadyInDeckDialog.Values;
      return dialog;
    }
    public override TextAreaDialog GetExportDialog(string text)
    {
      ViewModel.OnGetDialogWrapper += (s, args) => args.DialogWrapper = new TestDialogWrapper(ExportDialog.Result);
      var dialog = base.GetExportDialog(text);
      return dialog;
    }
    public override TextAreaDialog GetImportDialog()
    {
      ViewModel.OnGetDialogWrapper += (s, args) => args.DialogWrapper = new TestDialogWrapper(ImportDialog.Result);
      var dialog = base.GetImportDialog();
      dialog.TextInputText = ImportDialog.Values;
      return dialog;
    }
  }

  #region Import & Export
  [TestMethod]
  public async Task ImportToDeckCardsDialogTest()
  {
    var importedCards = new MTGCard[]
    {
      Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "First"),
      Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "Second"),
      Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "Third"),
    };

    DeckBuilderViewModel vm = new(new TestCardAPI(importedCards), null);
    vm.DeckCards.Dialogs = new TestDeckCardlistViewDialogs(vm.DeckCards)
    {
      ImportDialog = new() { Values = nameof(importedCards) },
    };

    await vm.DeckCards.ImportToCardlistDialog();
    Assert.AreEqual(3, vm.DeckCards.CardlistSize);
  }

  [TestMethod]
  public async Task ImportToDeckCardsDialogTest_Undo()
  {
    var importedCards = new MTGCard[]
    {
      Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "First"),
      Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "Second"),
      Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "Third"),
    };

    DeckBuilderViewModel vm = new(new TestCardAPI(importedCards), null);
    vm.DeckCards.Dialogs = new TestDeckCardlistViewDialogs(vm.DeckCards)
    {
      ImportDialog = new() { Values = nameof(importedCards) },
    };

    await vm.DeckCards.ImportToCardlistDialog();

    vm.CommandService.Undo();
    Assert.AreEqual(0, vm.DeckCards.CardlistSize);

    vm.CommandService.Redo();
    Assert.AreEqual(3, vm.DeckCards.CardlistSize);
  }

  [TestMethod]
  public async Task ImportToDeckCardsDialogTest_AlreadyExists_Add()
  {
    var importedCards = new MTGCard[]
    {
      Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "First"),
      Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "Second"),
      Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "Third"),
    };

    DeckBuilderViewModel vm = new(new TestCardAPI(importedCards), null);
    vm.DeckCards.Dialogs = new TestDeckCardlistViewDialogs(vm.DeckCards)
    {
      ImportDialog = new() { Values = nameof(importedCards) },
      MultipleCardsAlreadyInDeckDialog = new() { Values = true },
    };

    await vm.DeckCards.ImportToCardlistDialog();
    await vm.DeckCards.ImportToCardlistDialog();
    Assert.AreEqual(6, vm.DeckCards.CardlistSize);
  }

  [TestMethod]
  public async Task ImportToDeckCardsDialogTest_AlreadyExists_Skip()
  {
    var firstImport = new MTGCard[]
    {
      Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "First"),
      Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "Second"),
      Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "Third"),
    };
    var secondImport = new MTGCard[]
    {
      Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "First"),  // Old
      Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "Second"), // Old
      Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "Fourth"), // New
    };

    var cardAPI = new TestCardAPI(firstImport);
    DeckBuilderViewModel vm = new(cardAPI, null);
    vm.DeckCards.Dialogs = new TestDeckCardlistViewDialogs(vm.DeckCards)
    {
      ImportDialog = new() { Values = nameof(firstImport) },
      MultipleCardsAlreadyInDeckDialog = new() { Result = ContentDialogResult.None, Values = true }
    };

    await vm.DeckCards.ImportToCardlistDialog();
    cardAPI.ExpectedCards = secondImport;

    await vm.DeckCards.ImportToCardlistDialog();
    Assert.AreEqual(4, vm.DeckCards.CardlistSize);
  }

  [TestMethod]
  public async Task ExportCardsDialogTest()
  {
    var cards = new System.Collections.ObjectModel.ObservableCollection<MTGCard>
    {
      Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "First", count: 1),
      Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "Second", count: 2),
      Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "Third", count: 3),
    };
    var deck = new MTGCardDeck()
    {
      DeckCards = cards,
    };
    var cardlist = new DeckCardlistViewModel(deck.DeckCards, null, null);
    var expectedText = MTGService.GetExportString(cardlist.Cardlist.ToArray(), "Name");

    using TestIO.TestClipboard clipboard = new();
    DeckBuilderViewModel vm = new(cardAPI: null, deckRepository: null, clipboardService: clipboard);
    vm.DeckCards.Dialogs = new TestDeckCardlistViewDialogs(vm.DeckCards)
    {
      ExportDialog = new() { Values = expectedText },
    };

    foreach (var item in cards)
    {
      await vm.DeckCards.Add(item);
    }

    await vm.DeckCards.ExportDeckCardsDialog();
    Assert.AreEqual(expectedText, clipboard.Content);
  }
  #endregion

  #region Sorting & Filtering
  [TestMethod]
  public async Task SortDeckCommandTest()
  {
    DeckBuilderViewModel vm = new(null, null, null);

    var firstCard = Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "A", cmc: 2);
    var secondCard = Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "B", cmc: 1);
    var thirdCard = Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "C", cmc: 3);

    vm.SortProperties.PrimarySortProperty = MTGSortProperty.CMC;
    vm.SortProperties.SortDirection = SortDirection.Ascending;
    await vm.DeckCards.Add(secondCard);
    await vm.DeckCards.Add(thirdCard);
    await vm.DeckCards.Add(firstCard);
    // The viewmodels should be sorted when cards are added to the deck
    CollectionAssert.AreEqual(new[] { secondCard, firstCard, thirdCard }, vm.DeckCards.FilteredAndSortedCardViewModels.Select(x => ((MTGCardViewModel)x).Model).ToArray());

    vm.SetPrimarySortProperty(MTGSortProperty.Name.ToString());
    CollectionAssert.AreEqual(new[] { firstCard, secondCard, thirdCard }, vm.DeckCards.FilteredAndSortedCardViewModels.Select(x => ((MTGCardViewModel)x).Model).ToArray());

    vm.SortByDirection(SortDirection.Descending.ToString());
    CollectionAssert.AreEqual(new[] { thirdCard, secondCard, firstCard }, vm.DeckCards.FilteredAndSortedCardViewModels.Select(x => ((MTGCardViewModel)x).Model).ToArray());
  }

  [TestMethod]
  public async Task SortDeckCommandTest_SecondaryProperty()
  {
    DeckBuilderViewModel vm = new(null, null, null);

    var firstCard = Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "A", cmc: 2, frontFace: Mocker.MTGCardModelMocker.CreateCardFace(colors: new ColorTypes[] { ColorTypes.W }));
    var secondCard = Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "BA", cmc: 1, frontFace: Mocker.MTGCardModelMocker.CreateCardFace(colors: new ColorTypes[] { ColorTypes.W }));
    var thirdCard = Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "BB", cmc: 1, frontFace: Mocker.MTGCardModelMocker.CreateCardFace(colors: new ColorTypes[] { ColorTypes.W }));
    var fourthCard = Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "C", cmc: 3, frontFace: Mocker.MTGCardModelMocker.CreateCardFace(colors: new ColorTypes[] { ColorTypes.W }));

    vm.SortProperties.PrimarySortProperty = MTGSortProperty.Color;
    vm.SortProperties.SecondarySortProperty = MTGSortProperty.Name;
    vm.SortProperties.SortDirection = SortDirection.Ascending;
    await vm.DeckCards.Add(thirdCard);
    await vm.DeckCards.Add(secondCard);
    await vm.DeckCards.Add(firstCard);
    await vm.DeckCards.Add(fourthCard);
    // The viewmodels should be sorted by name when added, because they are the same color
    CollectionAssert.AreEqual(new[] { firstCard, secondCard, thirdCard, fourthCard }, vm.DeckCards.FilteredAndSortedCardViewModels.Select(x => ((MTGCardViewModel)x).Model).ToArray());

    vm.SetSecondarySortProperty(MTGSortProperty.CMC.ToString());
    CollectionAssert.AreEqual(new[] { secondCard.Info.CMC, thirdCard.Info.CMC, firstCard.Info.CMC, fourthCard.Info.CMC }, vm.DeckCards.FilteredAndSortedCardViewModels.Select(x => ((MTGCardViewModel)x).Model.Info.CMC).ToArray());
  }

  [TestMethod]
  public async Task DeckFilterTest()
  {
    DeckBuilderViewModel vm = new(null, null, null);

    var firstCard = Mocker.MTGCardModelMocker.CreateMTGCardModel(
      name: "A",
      cmc: 2,
      typeLine: "Artifact",
      frontFace: Mocker.MTGCardModelMocker.CreateCardFace(
        new ColorTypes[] { ColorTypes.R },
        oracleText: "Exile taget creature"));
    var secondCard = Mocker.MTGCardModelMocker.CreateMTGCardModel(
      name: "B",
      cmc: 1,
      typeLine: "Creature",
      frontFace: Mocker.MTGCardModelMocker.CreateCardFace(
        new ColorTypes[] { ColorTypes.W },
        oracleText: "Destroy taget creature"));
    var thirdCard = Mocker.MTGCardModelMocker.CreateMTGCardModel(
      name: "C",
      cmc: 3,
      typeLine: "Land",
      frontFace: Mocker.MTGCardModelMocker.CreateCardFace(
        new ColorTypes[] { ColorTypes.C },
        oracleText: "You gain 5 life"));
    var fourthCard = Mocker.MTGCardModelMocker.CreateMTGCardModel(
      name: "D",
      cmc: 4,
      typeLine: "Enchantment",
      frontFace: Mocker.MTGCardModelMocker.CreateCardFace(
        new ColorTypes[] { ColorTypes.W, ColorTypes.B },
        oracleText: "Draw a card"));

    await vm.DeckCards.Add(secondCard);
    await vm.DeckCards.Add(thirdCard);
    await vm.DeckCards.Add(firstCard);
    await vm.DeckCards.Add(fourthCard);

    // Name filter
    vm.CardFilters.NameText = "a";
    CollectionAssert.AreEquivalent(new[] { firstCard }, vm.DeckCards.FilteredAndSortedCardViewModels.Select(x => ((MTGCardViewModel)x).Model).ToArray());

    // Reset
    vm.CardFilters.Reset();
    Assert.AreEqual(4, vm.DeckCards.FilteredAndSortedCardViewModels.Select(x => ((MTGCardViewModel)x).Model).ToArray().Length);

    // Color filter
    vm.CardFilters.White = false; // Non whites
    CollectionAssert.AreEquivalent(new[] { thirdCard, firstCard }, vm.DeckCards.FilteredAndSortedCardViewModels.Select(x => ((MTGCardViewModel)x).Model).ToArray());

    // Type filter
    vm.CardFilters.Reset();
    vm.CardFilters.TypeText = "Cr"; // Creatures
    CollectionAssert.AreEquivalent(new[] { secondCard }, vm.DeckCards.FilteredAndSortedCardViewModels.Select(x => ((MTGCardViewModel)x).Model).ToArray());

    // Color group filter
    vm.CardFilters.Reset();
    vm.CardFilters.ColorGroup = MTGService.MTGCardFilters.ColorGroups.Multi; // Multicolored
    CollectionAssert.AreEquivalent(new[] { fourthCard }, vm.DeckCards.FilteredAndSortedCardViewModels.Select(x => ((MTGCardViewModel)x).Model).ToArray());

    // CMC filter
    vm.CardFilters.Reset();
    vm.CardFilters.Cmc = 3; // Multicolored
    CollectionAssert.AreEquivalent(new[] { thirdCard }, vm.DeckCards.FilteredAndSortedCardViewModels.Select(x => ((MTGCardViewModel)x).Model).ToArray());

    // Oracle filter
    vm.CardFilters.Reset();
    vm.CardFilters.OracleText = "destroy"; // Oracle says 'destroy'
    CollectionAssert.AreEquivalent(new[] { secondCard }, vm.DeckCards.FilteredAndSortedCardViewModels.Select(x => ((MTGCardViewModel)x).Model).ToArray());
  }

  [TestMethod]
  public async Task DeckFilterTest_Maybelist()
  {
    DeckBuilderViewModel vm = new(null, null, null);

    var firstCard = Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "A", cmc: 2, typeLine: "Artifact", frontFace: Mocker.MTGCardModelMocker.CreateCardFace(new ColorTypes[] { ColorTypes.R }));
    var secondCard = Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "B", cmc: 1, typeLine: "Creature", frontFace: Mocker.MTGCardModelMocker.CreateCardFace(new ColorTypes[] { ColorTypes.W }));
    var thirdCard = Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "C", cmc: 3, typeLine: "Land", frontFace: Mocker.MTGCardModelMocker.CreateCardFace(new ColorTypes[] { ColorTypes.C }));
    var fourthCard = Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "D", cmc: 4, typeLine: "Enchantment", frontFace: Mocker.MTGCardModelMocker.CreateCardFace(new ColorTypes[] { ColorTypes.W, ColorTypes.B }));

    await vm.MaybelistCards.Add(secondCard);
    await vm.MaybelistCards.Add(thirdCard);
    await vm.MaybelistCards.Add(firstCard);
    await vm.MaybelistCards.Add(fourthCard);

    // Name filter
    vm.CardFilters.NameText = "a";
    CollectionAssert.AreEquivalent(new[] { firstCard }, vm.MaybelistCards.FilteredAndSortedCardViewModels.Select(x => ((MTGCardViewModel)x).Model).ToArray());

    // Reset
    vm.CardFilters.Reset();
    Assert.AreEqual(4, vm.MaybelistCards.FilteredAndSortedCardViewModels.Select(x => ((MTGCardViewModel)x).Model).ToArray().Length);

    // Color filter
    vm.CardFilters.White = false; // Non whites
    CollectionAssert.AreEquivalent(new[] { thirdCard, firstCard }, vm.MaybelistCards.FilteredAndSortedCardViewModels.Select(x => ((MTGCardViewModel)x).Model).ToArray());

    // Type filter
    vm.CardFilters.Reset();
    vm.CardFilters.TypeText = "Cr"; // Creatures
    CollectionAssert.AreEquivalent(new[] { secondCard }, vm.MaybelistCards.FilteredAndSortedCardViewModels.Select(x => ((MTGCardViewModel)x).Model).ToArray());

    // Color group filter
    vm.CardFilters.Reset();
    vm.CardFilters.ColorGroup = MTGService.MTGCardFilters.ColorGroups.Multi; // Multicolored
    CollectionAssert.AreEquivalent(new[] { fourthCard }, vm.MaybelistCards.FilteredAndSortedCardViewModels.Select(x => ((MTGCardViewModel)x).Model).ToArray());

    // CMC filter
    vm.CardFilters.Reset();
    vm.CardFilters.Cmc = 3; // Multicolored
    CollectionAssert.AreEquivalent(new[] { thirdCard }, vm.MaybelistCards.FilteredAndSortedCardViewModels.Select(x => ((MTGCardViewModel)x).Model).ToArray());
  }
  #endregion

  #region HasUnsavedChanges
  [TestMethod]
  public void HasUnsavedChangesTest_Init() => Assert.IsFalse(new DeckBuilderViewModel(null, null, null).HasUnsavedChanges);

  [TestMethod]
  public async Task HasUnsavedChangesTest_AddCard()
  {
    DeckBuilderViewModel vm = new(null, null, null);

    await vm.DeckCards.Add(Mocker.MTGCardModelMocker.CreateMTGCardModel());
    Assert.IsTrue(vm.HasUnsavedChanges);
  }

  [TestMethod]
  public async Task HasUnsavedChangesTest_IncreaseCount()
  {
    DeckBuilderViewModel vm = new(null, null, null);

    var card = Mocker.MTGCardModelMocker.CreateMTGCardModel();
    await vm.DeckCards.Add(card);
    vm.HasUnsavedChanges = false;
    Assert.IsFalse(vm.HasUnsavedChanges);

    card.Count += 1;
    Assert.IsTrue(vm.HasUnsavedChanges);
  }

  [TestMethod]
  public async Task HasUnsaveChangesTest_ChangePrint()
  {
    var card = Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "First", scryfallId: Guid.NewGuid());
    var newPrint = new MTGCardViewModel(Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "First", scryfallId: Guid.NewGuid()));
    DeckBuilderViewModel vm = new(new TestCardAPI(), null);
    vm.DeckCards.Dialogs = new TestDeckCardlistViewDialogs(vm.DeckCards)
    {
      CardPrintDialog = new() { Values = newPrint },
    };

    await vm.DeckCards.Add(card);
    vm.HasUnsavedChanges = false; // Change unsaved state to false without saving
    Assert.IsFalse(vm.HasUnsavedChanges);

    await vm.DeckCards.ChangePrintDialog(card);
    Assert.IsTrue(vm.HasUnsavedChanges);
  }
  #endregion

  #region Adding and Removing
  [TestMethod]
  public async Task AddToCardlistCommandTest()
  {
    var deck = new MTGCardDeck();
    var cardlist = new DeckCardlistViewModel(deck.DeckCards, null, null);

    await cardlist.Add(Mocker.MTGCardModelMocker.CreateMTGCardModel());
    Assert.AreEqual(1, deck.DeckCards.Count);
  }

  [TestMethod]
  public async Task AddToCardlistCommandTest_AlreadyExists_Add()
  {
    var deck = new MTGCardDeck();
    var cardlist = new DeckCardlistViewModel(deck.DeckCards, null);
    cardlist.Dialogs = new TestDeckCardlistViewDialogs(cardlist)
    {
      CardAlreadyInDeckDialog = new(),
    };

    await cardlist.Add(Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "First", count: 1));
    await cardlist.Add(Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "First", count: 2));
    Assert.AreEqual(3, deck.DeckCards.Sum(x => x.Count));
  }

  [TestMethod]
  public async Task AddToCardlistCommandTest_AlreadyExists_Skip()
  {
    var deck = new MTGCardDeck();
    var cardlist = new DeckCardlistViewModel(deck.DeckCards, null);
    cardlist.Dialogs = new TestDeckCardlistViewDialogs(cardlist)
    {
      CardAlreadyInDeckDialog = new() { Result = ContentDialogResult.None }
    };

    await cardlist.Add(Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "First", count: 1));
    await cardlist.Add(Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "First", count: 2));
    Assert.AreEqual(1, deck.DeckCards.Sum(x => x.Count));
  }

  [TestMethod]
  public async Task AddToCardlistCommandTest_Undo()
  {
    var deck = new MTGCardDeck();
    var cardlist = new DeckCardlistViewModel(deck.DeckCards, null, null);

    await cardlist.Add(Mocker.MTGCardModelMocker.CreateMTGCardModel());

    cardlist.CommandService.Undo();
    Assert.AreEqual(0, deck.DeckCards.Count);

    cardlist.CommandService.Redo();
    Assert.AreEqual(1, deck.DeckCards.Count);
  }

  [TestMethod]
  public async Task AddToCardlistCommandTest_Move_UndoRedo()
  {
    var commandService = new CommandService();
    var deck = new MTGCardDeck();
    var deckCards = new DeckCardlistViewModel(deck.DeckCards, null, null) { CommandService = commandService };
    var wishlist = new DeckCardlistViewModel(deck.Wishlist, null, null) { CommandService = commandService };
    var card = Mocker.MTGCardModelMocker.CreateMTGCardModel();

    await deckCards.Add(card);

    await wishlist.Move(card, deckCards);
    Assert.AreEqual(0, deck.DeckCards.Count);
    Assert.AreEqual(1, deck.Wishlist.Count);

    commandService.Undo();
    Assert.AreEqual(1, deck.DeckCards.Count);
    Assert.AreEqual(0, deck.Wishlist.Count);

    commandService.Redo();
    Assert.AreEqual(0, deck.DeckCards.Count);
    Assert.AreEqual(1, deck.Wishlist.Count);
  }

  [TestMethod]
  public async Task RemoveFromCardlistCommandTest()
  {
    var deck = new MTGCardDeck();
    var cardlist = new DeckCardlistViewModel(deck.DeckCards, null, null);
    var card = Mocker.MTGCardModelMocker.CreateMTGCardModel();

    await cardlist.Add(card);

    cardlist.Remove(card);
    Assert.AreEqual(0, deck.DeckCards.Count);
  }

  [TestMethod]
  public async Task RemoveFromCardlistCommandTest_Undo_Redo()
  {
    var vm = new DeckBuilderViewModel(null, null, null);
    var card = Mocker.MTGCardModelMocker.CreateMTGCardModel();

    await vm.DeckCards.Add(card);
    vm.DeckCards.Remove(card);

    vm.CommandService.Undo();
    Assert.AreEqual(1, vm.DeckCards.FilteredAndSortedCardViewModels.Count);

    vm.CommandService.Redo();
    Assert.AreEqual(0, vm.DeckCards.FilteredAndSortedCardViewModels.Count);
  }

  [TestMethod]
  public async Task RemoveFromCardlistCommandTest_CardViewModelCommand()
  {
    var deck = new MTGCardDeck();
    var cardlist = new DeckCardlistViewModel(deck.DeckCards, null, null);
    var card = Mocker.MTGCardModelMocker.CreateMTGCardModel();

    await cardlist.Add(card);
    Assert.AreEqual(1, deck.DeckCards.Count);

    ((MTGCardViewModel)cardlist.FilteredAndSortedCardViewModels[0]).DeleteCardCommand.Execute(card);
    Assert.AreEqual(0, deck.DeckCards.Count);
  }

  [TestMethod]
  public async Task RemoveFromCardlistCommandTest_Undo()
  {
    var deck = new MTGCardDeck();
    var cardlist = new DeckCardlistViewModel(deck.DeckCards, null, null);
    var card = Mocker.MTGCardModelMocker.CreateMTGCardModel();

    await cardlist.Add(card);
    cardlist.Remove(card);

    cardlist.CommandService.Undo();
    Assert.AreEqual(1, deck.DeckCards.Count);

    cardlist.CommandService.Redo();
    Assert.AreEqual(0, deck.DeckCards.Count);
  }

  [TestMethod]
  public async Task ClearCardlistCommandTest()
  {
    var deck = new MTGCardDeck();
    var cardlist = new DeckCardlistViewModel(deck.DeckCards, null, null);

    await cardlist.Add(Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "1"));
    await cardlist.Add(Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "2"));
    await cardlist.Add(Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "3"));
    Assert.AreEqual(3, cardlist.CardlistSize);

    cardlist.Clear();
    Assert.AreEqual(0, cardlist.CardlistSize);

    cardlist.CommandService.Undo();
    Assert.AreEqual(3, cardlist.CardlistSize);

    cardlist.CommandService.Redo();
    Assert.AreEqual(0, cardlist.CardlistSize);
  }
  #endregion

  #region Price Changing
  [TestMethod]
  public async Task ChangePrintTest()
  {
    var card = Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "First", scryfallId: Guid.NewGuid());
    var newPrint = new MTGCardViewModel(Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "First", scryfallId: Guid.NewGuid()));
    DeckBuilderViewModel vm = new(new TestCardAPI(), null);
    vm.DeckCards.Dialogs = new TestDeckCardlistViewDialogs(vm.DeckCards)
    {
      CardPrintDialog = new() { Values = newPrint }
    };

    await vm.DeckCards.Add(card);

    await vm.DeckCards.ChangePrintDialog(card);
    Assert.AreEqual(card.Info.ScryfallId, newPrint.Model.Info.ScryfallId);
    Assert.AreEqual(newPrint.Model.Info.ScryfallId, ((MTGCardViewModel)vm.DeckCards.FilteredAndSortedCardViewModels[0]).Model.Info.ScryfallId);
  }

  [TestMethod]
  public async Task DeckPriceChangesTest_CardlistChanges()
  {
    var cardlist = new DeckCardlistViewModel(new MTGCardDeck().DeckCards, null, null);
    using var asserter = new PropertyChangedAssert(cardlist, nameof(cardlist.EuroPrice));

    await cardlist.Add(Mocker.MTGCardModelMocker.CreateMTGCardModel(price: 500));
    Assert.IsTrue(asserter.PropertyChanged);
  }

  [TestMethod]
  public async Task DeckPriceChangesTest_CardCountChanges()
  {
    var card = Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "First", scryfallId: Guid.NewGuid());
    var cardlist = new DeckCardlistViewModel(new MTGCardDeck() { DeckCards = new() { card } }.DeckCards, null, null);
    DeckBuilderViewModel vm = new(new TestCardAPI(), null, null);

    await vm.DeckCards.Add(card);

    using var asserter = new PropertyChangedAssert(cardlist, nameof(cardlist.EuroPrice));
    card.Count++;
    Assert.IsTrue(asserter.PropertyChanged);
  }

  [TestMethod]
  public async Task DeckPriceChangesTest_CardPrintChanges()
  {
    var card = Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "First", scryfallId: Guid.NewGuid());
    var cardlist = new DeckCardlistViewModel(new MTGCardDeck { DeckCards = new() { card } }.DeckCards, null, null);
    var newPrint = new MTGCardViewModel(Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "First", scryfallId: Guid.NewGuid()));
    DeckBuilderViewModel vm = new(new TestCardAPI(), null);
    vm.DeckCards.Dialogs = new TestDeckCardlistViewDialogs(vm.DeckCards)
    {
      CardPrintDialog = new() { Values = newPrint }
    };

    await vm.DeckCards.Add(card);

    using var asserter = new PropertyChangedAssert(cardlist, nameof(cardlist.EuroPrice));
    await vm.DeckCards.ChangePrintDialog(card);
    Assert.IsTrue(asserter.PropertyChanged);
  }
  #endregion
}