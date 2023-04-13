using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.UI.Xaml.Controls;
using MTGApplicationTests.Services;
using MTGApplication.ViewModels;
using MTGApplicationTests.API;
using MTGApplication.Models;
using CommunityToolkit.WinUI.UI;
using static MTGApplicationTests.Database.InMemoryMTGDeckRepositoryTests;
using static MTGApplicationTests.ViewModels.DeckBuilderViewModelTests;
using static MTGApplicationTests.Services.TestDialogService;
using static MTGApplication.ViewModels.DeckBuilderViewModel;
using static MTGApplication.Models.MTGCard;
using static MTGApplication.Services.DialogService;
using static MTGApplication.Enums;

namespace MTGApplicationTests.ViewModels;

[TestClass]
public class DeckBuilderViewModelTests
{
  public class TestDeckBuilderViewDialogs : DeckBuilderViewDialogs
  {
    public TestDialogResult<MTGCardViewModel> CardPrintDialog { get; set; } = new();
    public TestDialogResult<bool?> MultipleCardsAlreadyInDeckDialog { get; set; } = new();
    public TestDialogResult<string> LoadDialog { get; set; } = new();
    public TestDialogResult<string> ImportDialog { get; set; } = new();
    public TestDialogResult<string> ExportDialog { get; set; } = new();
    public TestDialogResult<string> SaveDialog { get; set; } = new();
    public TestDialogResult CardAlreadyInDeckDialog { get; set; } = new();
    public TestDialogResult SaveUnsavedDialog { get; set; } = new();
    public TestDialogResult OverrideDialog { get; set; } = new();
    public TestDialogResult DeleteDialog { get; set; } = new();

    public override ConfirmationDialog GetCardAlreadyInDeckDialog(string name)
    {
      CurrentDialogWrapper = new TestDialogWrapper(CardAlreadyInDeckDialog.Result);
      var dialog = base.GetCardAlreadyInDeckDialog(name);
      return dialog;
    }
    public override ConfirmationDialog GetOverrideDialog(string name)
    {
      CurrentDialogWrapper = new TestDialogWrapper(OverrideDialog.Result);
      var dialog = base.GetOverrideDialog(name);
      return dialog;
    }
    public override ConfirmationDialog GetDeleteDialog(string name)
    {
      CurrentDialogWrapper = new TestDialogWrapper(DeleteDialog.Result);
      var dialog = base.GetDeleteDialog(name);
      return dialog;
    }
    public override ConfirmationDialog GetSaveUnsavedDialog()
    {
      CurrentDialogWrapper = new TestDialogWrapper(SaveUnsavedDialog.Result);
      var dialog = base.GetSaveUnsavedDialog();
      return dialog;
    }
    public override CheckBoxDialog GetMultipleCardsAlreadyInDeckDialog(string name)
    {
      CurrentDialogWrapper = new TestDialogWrapper(MultipleCardsAlreadyInDeckDialog.Result);
      var dialog = base.GetMultipleCardsAlreadyInDeckDialog(name);
      dialog.IsChecked = MultipleCardsAlreadyInDeckDialog.Values;
      return dialog;
    }
    public override GridViewDialog GetCardPrintDialog(MTGCardViewModel[] printViewModels)
    {
      CurrentDialogWrapper = new TestDialogWrapper(CardPrintDialog.Result);
      var dialog = base.GetCardPrintDialog(printViewModels);
      dialog.Selection = CardPrintDialog.Values;
      return dialog;
    }
    public override ComboBoxDialog GetLoadDialog(string[] names)
    {
      CurrentDialogWrapper = new TestDialogWrapper(LoadDialog.Result);
      var dialog = base.GetLoadDialog(names);
      dialog.Selection = LoadDialog.Values;
      return dialog;
    }
    public override TextAreaDialog GetExportDialog(string text)
    {
      CurrentDialogWrapper = new TestDialogWrapper(ExportDialog.Result);
      var dialog = base.GetExportDialog(text);
      return dialog;
    }
    public override TextAreaDialog GetImportDialog()
    {
      CurrentDialogWrapper = new TestDialogWrapper(ImportDialog.Result);
      var dialog = base.GetImportDialog();
      dialog.TextInputText = ImportDialog.Values;
      return dialog;
    }
    public override TextBoxDialog GetSaveDialog(string name)
    {
      CurrentDialogWrapper = new TestDialogWrapper(SaveDialog.Result);
      var dialog = base.GetSaveDialog(name);
      dialog.TextInputText = SaveDialog.Values;
      return dialog;
    }
  }

  #region NewDeckDialogCommand
  [TestMethod]
  public async Task NewDeckDialogCommandTest_NoSave()
  {
    var deckName = "First";

    using TestInMemoryMTGDeckRepository repo = new();
    DeckBuilderViewModel vm = new(null, repo, dialogs: new TestDeckBuilderViewDialogs() // Override dialogs
    {
      SaveUnsavedDialog = new() { Result = ContentDialogResult.Secondary }, // No save
    });

    // Add deck with the same name to the database
    var dbDeck = new MTGCardDeck()
    {
      Name = deckName,
      DeckCards = new()
      {
        Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "First"),
        Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "Second")
      }
    };
    await repo.Add(dbDeck);

    // Add card to the deck so the unsaved dialog appears
    await vm.DeckCards.AddToCardlist(Mocker.MTGCardModelMocker.CreateMTGCardModel());
    Assert.IsTrue(vm.HasUnsavedChanges);

    await vm.NewDeckDialog();
    Assert.IsFalse(vm.HasUnsavedChanges);
    Assert.AreEqual(0, vm.DeckCards.CardlistSize);
    Assert.AreEqual(string.Empty, vm.CardDeckName);
    Assert.AreEqual(1, (await repo.Get()).ToList().Count);
    Assert.AreEqual(2, (await repo.Get(deckName))?.DeckCards.Count);
  }

  [TestMethod]
  public async Task NewDeckDialogCommandTest_Save_NoExistingDeck()
  {
    var deckName = "First";

    using TestInMemoryMTGDeckRepository repo = new();
    DeckBuilderViewModel vm = new(null, repo, dialogs: new TestDeckBuilderViewDialogs() // Override dialogs
    {
      SaveUnsavedDialog = new(), // Save
      SaveDialog = new() { Values = deckName }, // Give deck name
    });

    // Add a card to the deck so the unsaved dialog appears
    await vm.DeckCards.AddToCardlist(Mocker.MTGCardModelMocker.CreateMTGCardModel());
    Assert.IsTrue(vm.HasUnsavedChanges);

    await vm.NewDeckDialog();
    Assert.IsFalse(vm.HasUnsavedChanges);
    Assert.AreEqual(0, vm.DeckCards.CardlistSize);
    Assert.AreEqual(string.Empty, vm.CardDeckName);
    Assert.AreEqual(1, (await repo.Get()).ToList().Count);
    Assert.AreEqual(1, (await repo.Get(deckName))?.DeckCards.Count);
  }

  [TestMethod]
  public async Task NewDeckDialogCommandTest_Save_ExistingDeck_NoOverride()
  {
    var deckName = "First";

    using TestInMemoryMTGDeckRepository repo = new();
    DeckBuilderViewModel vm = new(null, repo, dialogs: new TestDeckBuilderViewDialogs() // Override dialogs
    {
      SaveUnsavedDialog = new(), // Save
      SaveDialog = new() { Values = deckName}, // Give deck name
      OverrideDialog = new() { Result = ContentDialogResult.Secondary}, // Don't override
    });

    // Add deck with the same name to the database
    var dbDeck = new MTGCardDeck()
    {
      Name = deckName,
      DeckCards = new()
      {
        Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "First"),
        Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "Second")
      }
    };
    await repo.Add(dbDeck);

    // Add a card to the deck so the unsaved dialog appears.
    await vm.DeckCards.AddToCardlist(Mocker.MTGCardModelMocker.CreateMTGCardModel());
    Assert.IsTrue(vm.HasUnsavedChanges);

    await vm.NewDeckDialog();
    Assert.IsFalse(vm.HasUnsavedChanges);
    Assert.AreEqual(0, vm.DeckCards.CardlistSize);
    Assert.AreEqual(string.Empty, vm.CardDeckName);
    Assert.AreEqual(1, (await repo.Get()).ToList().Count);
    Assert.AreEqual(2, (await repo.Get(deckName))?.DeckCards.Count);
  }

  [TestMethod]
  public async Task NewDeckDialogCommandTest_Save_ExistingDeck_Override()
  {
    var deckName = "First";

    using TestInMemoryMTGDeckRepository repo = new();
    DeckBuilderViewModel vm = new(null, repo, dialogs: new TestDeckBuilderViewDialogs()
    {
      SaveUnsavedDialog = new(), // Save
      SaveDialog = new() { Values = deckName }, // Give name
      OverrideDialog = new() // Override
    });

    // Add deck with the same name to the database
    var dbDeck = new MTGCardDeck()
    {
      Name = deckName,
      DeckCards = new()
      {
        Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "First"),
        Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "Second")
      }
    };
    await repo.Add(dbDeck);

    // Add a card to the deck so the unsaved dialog appears
    await vm.DeckCards.AddToCardlist(Mocker.MTGCardModelMocker.CreateMTGCardModel());
    Assert.IsTrue(vm.HasUnsavedChanges);

    await vm.NewDeckDialog();
    Assert.IsFalse(vm.HasUnsavedChanges);
    Assert.AreEqual(0, vm.DeckCards.CardlistSize);
    Assert.AreEqual(string.Empty, vm.CardDeckName);
    Assert.AreEqual(1, (await repo.Get()).ToList().Count);
    Assert.AreEqual(1, (await repo.Get(deckName))?.DeckCards.Count);
  }

  [TestMethod]
  public async Task NewDeckDialogCommandTest_Save_ExistingDeck_CancelOverride()
  {
    var deckName = "First";
    using TestInMemoryMTGDeckRepository repo = new();
    DeckBuilderViewModel vm = new(null, repo, dialogs: new TestDeckBuilderViewDialogs()
    {
      SaveUnsavedDialog = new(), // Save
      SaveDialog = new() { Values = deckName }, // Give name
      OverrideDialog = new() { Result = ContentDialogResult.None } // Cancel
    });

    // Add deck with the same name to the database
    var dbDeck = new MTGCardDeck()
    {
      Name = deckName,
      DeckCards = new()
      {
        Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "First"),
        Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "Second")
      }
    };
    await repo.Add(dbDeck);

    // Add a card to the deck so the unsaved dialog appears.
    await vm.DeckCards.AddToCardlist(Mocker.MTGCardModelMocker.CreateMTGCardModel());
    Assert.IsTrue(vm.HasUnsavedChanges);

    await vm.NewDeckDialog();
    Assert.IsTrue(vm.HasUnsavedChanges);
    Assert.AreEqual(1, vm.DeckCards.CardlistSize);
    Assert.AreEqual(string.Empty, vm.CardDeckName);
    Assert.AreEqual(1, (await repo.Get()).ToList().Count);
    Assert.AreEqual(2, (await repo.Get(deckName))?.DeckCards.Count);
  }
  #endregion

  #region LoadDeckDialogCommand
  [TestMethod]
  public async Task LoadDeckDialogCommandTest_NoSave()
  {
    var deckName = "First";

    using TestInMemoryMTGDeckRepository repo = new();
    DeckBuilderViewModel vm = new(null, repo, dialogs: new TestDeckBuilderViewDialogs() // Override dialogs
    {
      SaveUnsavedDialog = new() { Result = ContentDialogResult.Secondary }, // No Save
      LoadDialog = new() { Values = deckName }, // Load deck
    });

    // Add deck with the same name to the database
    var dbDeck = new MTGCardDeck()
    {
      Name = deckName,
      DeckCards = new()
      {
        Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "First"),
        Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "Second")
      }
    };
    await repo.Add(dbDeck);

    // Add a card to the deck so the unsaved dialog appears.
    await vm.DeckCards.AddToCardlist(Mocker.MTGCardModelMocker.CreateMTGCardModel());
    Assert.IsTrue(vm.HasUnsavedChanges);

    await vm.LoadDeckDialog();
    Assert.IsFalse(vm.HasUnsavedChanges);
    Assert.AreEqual(2, vm.DeckCards.CardlistSize);
    Assert.AreEqual(deckName, vm.CardDeckName);
  }

  [TestMethod]
  public async Task LoadDeckDialogCommandTest_Cancel()
  {
    var deckName = "First";

    using TestInMemoryMTGDeckRepository repo = new();
    DeckBuilderViewModel vm = new(null, repo, dialogs: new TestDeckBuilderViewDialogs()
    {
      SaveUnsavedDialog = new() { Result = ContentDialogResult.None },
    });

    // Add deck with the same name to the database
    var dbDeck = new MTGCardDeck()
    {
      Name = deckName,
      DeckCards = new()
      {
        Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "First"),
        Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "Second")
      }
    };
    await repo.Add(dbDeck);

    // Add a card to the deck so the unsaved dialog appears.
    await vm.DeckCards.AddToCardlist(Mocker.MTGCardModelMocker.CreateMTGCardModel());
    Assert.IsTrue(vm.HasUnsavedChanges);

    await vm.LoadDeckDialog();
    Assert.IsTrue(vm.HasUnsavedChanges);
    Assert.AreEqual(1, vm.DeckCards.CardlistSize);
    Assert.AreEqual(string.Empty, vm.CardDeckName);
  }

  [TestMethod]
  public async Task LoadDeckDialogCommandTest_IsSorted()
  {
    var cards = new List<MTGCard>()
    {
      Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "First", cmc: 3),
      Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "Second", cmc: 2),
      Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "Third", cmc: 1),
    };
    var dbDeck = new MTGCardDeck()
    {
      Name = "First",
      DeckCards = new(cards),
    };

    using TestInMemoryMTGDeckRepository repo = new(new TestCardAPI()
    {
      ExpectedCards = cards.ToArray()
    });
    DeckBuilderViewModel vm = new(null, repo, dialogs: new TestDeckBuilderViewDialogs() // Override dialogs
    {
      LoadDialog = new() { Values = dbDeck.Name }, // Load deck
    });

    await repo.Add(dbDeck);      

    vm.SelectedSortDirection = SortDirection.Ascending;
    vm.SelectedPrimarySortProperty = MTGSortProperty.CMC;
    vm.SelectedSecondarySortProperty = MTGSortProperty.Name;
    var sortedDbCards = dbDeck.DeckCards.OrderBy(x => x.Info.CMC).ThenBy(x => x.Info.Name).ToList();

    await vm.LoadDeckDialog();
    CollectionAssert.AreEqual(sortedDbCards.Select(x => x.Info.CMC).ToArray(), vm.DeckCards.FilteredAndSortedCardViewModels.Select(x => ((MTGCardViewModel)x).Model.Info.CMC).ToArray());
  }
  #endregion

  #region SaveDeckDialogCommand
  [TestMethod]
  public async Task SaveDeckDialogCommandTest_NoExistingDeck()
  {
    var deckName = "First";

    using TestInMemoryMTGDeckRepository repo = new();
    DeckBuilderViewModel vm = new(null, repo, dialogs: new TestDeckBuilderViewDialogs() // Override dialogs
    {
      SaveDialog = new() { Values = deckName }, // Save deck
    });

    await vm.DeckCards.AddToCardlist(Mocker.MTGCardModelMocker.CreateMTGCardModel());
    Assert.IsTrue(vm.HasUnsavedChanges);

    await vm.SaveDeckDialog();
    Assert.IsFalse(vm.HasUnsavedChanges);
    Assert.AreEqual(1, vm.DeckCards.CardlistSize);
    Assert.AreEqual(1, (await repo.Get()).ToList().Count);
    Assert.AreEqual(deckName, vm.CardDeckName);
  }

  [TestMethod]
  public async Task SaveDeckDialogCommandTest_ExistingDeck_Override()
  {
    var deckName = "First";

    using TestInMemoryMTGDeckRepository repo = new();
    DeckBuilderViewModel vm = new(null, repo, dialogs: new TestDeckBuilderViewDialogs()
    {
      SaveDialog = new() { Values = deckName }, // Save
      OverrideDialog = new() // Override
    });

    // Add deck with the same name to the database
    var dbDeck = new MTGCardDeck()
    {
      Name = deckName,
      DeckCards = new()
      {
        Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "First"),
        Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "Second")
      }
    };
    await repo.Add(dbDeck);

    await vm.DeckCards.AddToCardlist(Mocker.MTGCardModelMocker.CreateMTGCardModel());

    await vm.SaveDeckDialog();
    Assert.IsFalse(vm.HasUnsavedChanges);
    Assert.AreEqual(1, vm.DeckCards.CardlistSize);
    Assert.AreEqual(1, (await repo.Get()).ToList().Count);
    Assert.AreEqual(1, (await repo.Get(deckName))?.DeckCards.Count);
    Assert.AreEqual(deckName, vm.CardDeckName);
  }

  [TestMethod]
  public async Task SaveDeckDialogCommandTest_ExistingDeck_DifferentName()
  {
    var loadName = "First";
    var saveName = "Second";

    using TestInMemoryMTGDeckRepository repo = new();
    DeckBuilderViewModel vm = new(null, repo, dialogs: new TestDeckBuilderViewDialogs()
    {
      SaveDialog = new() { Values = saveName },
      OverrideDialog = new(),
      LoadDialog = new() { Values = loadName }
    });

    // Add deck with the same name to the database
    var dbDeck = new MTGCardDeck()
    {
      Name = loadName,
      DeckCards = new()
      {
        Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "First"),
        Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "Second")
      }
    };
    await repo.Add(dbDeck);

    // Load the added deck
    await vm.LoadDeckDialog();
    Assert.AreEqual(loadName, vm.CardDeckName);

    await vm.DeckCards.AddToCardlist(Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "Third"));

    await vm.SaveDeckDialog();
    Assert.IsFalse(vm.HasUnsavedChanges);
    Assert.AreEqual(3, vm.DeckCards.CardlistSize);
    Assert.AreEqual(2, (await repo.Get()).ToList().Count);
    Assert.AreEqual(3, (await repo.Get(saveName))?.DeckCards.Count);
    Assert.AreEqual(saveName, vm.CardDeckName);
  }

  [TestMethod]
  public async Task SaveDeckDialogCommandTest_Cancel()
  {
    using TestInMemoryMTGDeckRepository repo = new();
    DeckBuilderViewModel vm = new(null, repo, dialogs: new TestDeckBuilderViewDialogs()
    {
      OverrideDialog = new() { Result = ContentDialogResult.None},
      SaveDialog = new() { Result = ContentDialogResult.None, Values = "Name"},
    });

    // Add card so the deck has unsaved changes
    await vm.DeckCards.AddToCardlist(Mocker.MTGCardModelMocker.CreateMTGCardModel());

    await vm.SaveDeckDialog();

    Assert.IsTrue(vm.HasUnsavedChanges);
    Assert.AreEqual(1, vm.DeckCards.CardlistSize);
    Assert.AreEqual(string.Empty, vm.CardDeckName);
  }

  [TestMethod]
  public async Task SaveDeckDialogCommandTest_CanExecute()
  {
    using TestInMemoryMTGDeckRepository repo = new();
    DeckBuilderViewModel vm = new(null, repo);

    // Empty deck should not be able to save using the command
    Assert.IsFalse(vm.SaveDeckDialogCommand.CanExecute(null));

    await vm.DeckCards.AddToCardlist(Mocker.MTGCardModelMocker.CreateMTGCardModel());
    Assert.IsTrue(vm.SaveDeckDialogCommand.CanExecute(null));
  }
  #endregion

  #region DeleteDeckDialogCommand
  [TestMethod]
  public async Task DeleteDeckDialogCommandTest_CanExecute()
  {
    using TestInMemoryMTGDeckRepository repo = new();
    DeckBuilderViewModel vm = new(null, repo, dialogs: new TestDeckBuilderViewDialogs()
    {
      SaveDialog = new() { Values = "First" }
    });

    // The deck should not be possible to be deleted if the deck has no name
    Assert.IsFalse(vm.DeleteDeckDialogCommand.CanExecute(null));

    await vm.SaveDeckDialog();
    Assert.IsTrue(vm.DeleteDeckDialogCommand.CanExecute(null));
  }

  [TestMethod]
  public async Task DeleteDeckDialogCommandTest()
  {
    var deckName = "First";

    using TestInMemoryMTGDeckRepository repo = new();
    DeckBuilderViewModel vm = new(null, repo, dialogs: new TestDeckBuilderViewDialogs()
    {
      SaveDialog = new() { Values = deckName }, // Save deck
      DeleteDialog = new(), // Delete
    });

    await vm.SaveDeckDialog();
    Assert.AreEqual(deckName, vm.CardDeckName);

    await vm.DeleteDeckDialog();
    Assert.AreEqual(0, (await repo.Get()).ToList().Count);
    Assert.AreEqual(string.Empty, vm.CardDeckName);
  }

  [TestMethod]
  public async Task DeleteDeckDialogCommandTest_Cancel()
  {
    var deckName = "First";

    using TestInMemoryMTGDeckRepository repo = new();
    DeckBuilderViewModel vm = new(null, repo, dialogs: new TestDeckBuilderViewDialogs()
    {
      SaveDialog = new() { Values = deckName }, // Save
      DeleteDialog = new() { Result = ContentDialogResult.None } // Cancel
    });

    await vm.SaveDeckDialog();
    Assert.AreEqual(deckName, vm.CardDeckName);

    await vm.DeleteDeckDialog();
    Assert.AreEqual(1, (await repo.Get()).ToList().Count);
    Assert.AreEqual(deckName, vm.CardDeckName);
  }

  [TestMethod]
  public async Task DeleteDeckDialogCommandTest_Reject()
  {
    var deckName = "First";

    using TestInMemoryMTGDeckRepository repo = new();
    DeckBuilderViewModel vm = new(null, repo, dialogs: new TestDeckBuilderViewDialogs()
    {
      SaveDialog = new() { Values = deckName }, // Save deck
      DeleteDialog = new() { Result = ContentDialogResult.Secondary }, // Reject deletion
    });

    await vm.SaveDeckDialog();
    Assert.AreEqual(deckName, vm.CardDeckName);

    await vm.DeleteDeckDialog();
    Assert.AreEqual(1, (await repo.Get()).ToList().Count);
    Assert.AreEqual(deckName, vm.CardDeckName);
  }
  #endregion
}

[TestClass]
public class DeckBuilderViewModel_CardlistTests
{
  public class CardlistPropertyAsserter : IDisposable
  {
    public Cardlist Cardlist { get; }
    public string ExpectedPropertyChanged { get; }
    public bool PropertyChanged { get; private set; } = false;

    public CardlistPropertyAsserter(Cardlist cardlist, string expectedPropertyChanged)
    {
      Cardlist = cardlist;
      ExpectedPropertyChanged = expectedPropertyChanged;
      Cardlist.PropertyChanged += Cardlist_PropertyChanged;
    }

    private void Cardlist_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
      if (e.PropertyName == ExpectedPropertyChanged) { PropertyChanged = true; }
    }

    public void Dispose()
    {
      Cardlist.PropertyChanged -= Cardlist_PropertyChanged;
      GC.SuppressFinalize(this);
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

    DeckBuilderViewModel vm = new(new TestCardAPI(importedCards), null, dialogs: new TestDeckBuilderViewDialogs()
    {
      ImportDialog = new() { Values = nameof(importedCards) },
    });

    await vm.DeckCards.ImportToCardlistDialog();
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

    DeckBuilderViewModel vm = new(new TestCardAPI(importedCards), null, dialogs: new TestDeckBuilderViewDialogs()
    {
      ImportDialog = new() { Values = nameof(importedCards) },
      MultipleCardsAlreadyInDeckDialog = new() { Values = true },
    });

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
    DeckBuilderViewModel vm = new(cardAPI, null, dialogs: new TestDeckBuilderViewDialogs()
    {
      ImportDialog = new() { Values = nameof(firstImport) },
      MultipleCardsAlreadyInDeckDialog = new() { Result = ContentDialogResult.None, Values = true }
    });

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
    var cardlist = new Cardlist(deck, CardlistType.Deck, null, null);
    var expectedText = cardlist.GetExportString();

    using TestIO.TestClipboard clipboard = new();
    DeckBuilderViewModel vm = new(cardAPI: null, deckRepository: null, dialogs: new TestDeckBuilderViewDialogs()
    {
      ExportDialog = new() { Values = expectedText },
    }, clipboardService: clipboard);

    foreach (var item in cards)
    {
      await vm.DeckCards.AddToCardlist(item);
    }

    await vm.DeckCards.ExportDeckCardsDialog();
    Assert.AreEqual(expectedText, clipboard.Content);
  }
  #endregion

  #region Sorting & Filtering
  [TestMethod]
  public async Task SortDeckCommandTest()
  {
    DeckBuilderViewModel vm = new(null, null);

    var firstCard = Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "A", cmc: 2);
    var secondCard = Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "B", cmc: 1);
    var thirdCard = Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "C", cmc: 3);

    vm.SelectedPrimarySortProperty = MTGSortProperty.CMC;
    vm.SelectedSortDirection = SortDirection.Ascending;
    await vm.DeckCards.AddToCardlist(secondCard);
    await vm.DeckCards.AddToCardlist(thirdCard);
    await vm.DeckCards.AddToCardlist(firstCard);
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
    DeckBuilderViewModel vm = new(null, null);

    var firstCard = Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "A", cmc: 2, frontFace: Mocker.MTGCardModelMocker.CreateCardFace(colors: new ColorTypes[] { ColorTypes.W }));
    var secondCard = Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "B", cmc: 1, frontFace: Mocker.MTGCardModelMocker.CreateCardFace(colors: new ColorTypes[] { ColorTypes.W }));
    var thirdCard = Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "C", cmc: 3, frontFace: Mocker.MTGCardModelMocker.CreateCardFace(colors: new ColorTypes[] { ColorTypes.W }));

    vm.SelectedPrimarySortProperty = MTGSortProperty.Color;
    vm.SelectedSecondarySortProperty = MTGSortProperty.Name;
    vm.SelectedSortDirection = SortDirection.Ascending;
    await vm.DeckCards.AddToCardlist(secondCard);
    await vm.DeckCards.AddToCardlist(thirdCard);
    await vm.DeckCards.AddToCardlist(firstCard);
    // The viewmodels should be sorted by name when added, because they are the same color
    CollectionAssert.AreEqual(new[] { firstCard, secondCard, thirdCard }, vm.DeckCards.FilteredAndSortedCardViewModels.Select(x => ((MTGCardViewModel)x).Model).ToArray());

    vm.SetSecondarySortProperty(MTGSortProperty.CMC.ToString());
    CollectionAssert.AreEqual(new[] { secondCard, firstCard, thirdCard }, vm.DeckCards.FilteredAndSortedCardViewModels.Select(x => ((MTGCardViewModel)x).Model).ToArray());
  }

  [TestMethod]
  public async Task DeckFilterTest()
  {
    DeckBuilderViewModel vm = new(null, null);

    var firstCard = Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "A", cmc: 2, typeLine: "Artifact", frontFace: Mocker.MTGCardModelMocker.CreateCardFace(new ColorTypes[] { ColorTypes.R }));
    var secondCard = Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "B", cmc: 1, typeLine: "Creature", frontFace: Mocker.MTGCardModelMocker.CreateCardFace(new ColorTypes[] { ColorTypes.W }));
    var thirdCard = Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "C", cmc: 3, typeLine: "Land", frontFace: Mocker.MTGCardModelMocker.CreateCardFace(new ColorTypes[] { ColorTypes.C }));
    var fourthCard = Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "D", cmc: 4, typeLine: "Enchantment", frontFace: Mocker.MTGCardModelMocker.CreateCardFace(new ColorTypes[] { ColorTypes.W, ColorTypes.B }));

    await vm.DeckCards.AddToCardlist(secondCard);
    await vm.DeckCards.AddToCardlist(thirdCard);
    await vm.DeckCards.AddToCardlist(firstCard);
    await vm.DeckCards.AddToCardlist(fourthCard);

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
    vm.CardFilters.ColorGroup = Cardlist.CardFilters.ColorGroups.Multi; // Multicolored
    CollectionAssert.AreEquivalent(new[] { fourthCard }, vm.DeckCards.FilteredAndSortedCardViewModels.Select(x => ((MTGCardViewModel)x).Model).ToArray());
    
    // CMC filter
    vm.CardFilters.Reset();
    vm.CardFilters.Cmc = 3; // Multicolored
    CollectionAssert.AreEquivalent(new[] { thirdCard }, vm.DeckCards.FilteredAndSortedCardViewModels.Select(x => ((MTGCardViewModel)x).Model).ToArray());
  }

  [TestMethod]
  public async Task DeckFilterTest_Maybelist()
  {
    DeckBuilderViewModel vm = new(null, null);

    var firstCard = Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "A", cmc: 2, typeLine: "Artifact", frontFace: Mocker.MTGCardModelMocker.CreateCardFace(new ColorTypes[] { ColorTypes.R }));
    var secondCard = Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "B", cmc: 1, typeLine: "Creature", frontFace: Mocker.MTGCardModelMocker.CreateCardFace(new ColorTypes[] { ColorTypes.W }));
    var thirdCard = Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "C", cmc: 3, typeLine: "Land", frontFace: Mocker.MTGCardModelMocker.CreateCardFace(new ColorTypes[] { ColorTypes.C }));
    var fourthCard = Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "D", cmc: 4, typeLine: "Enchantment", frontFace: Mocker.MTGCardModelMocker.CreateCardFace(new ColorTypes[] { ColorTypes.W, ColorTypes.B }));

    await vm.MaybelistCards.AddToCardlist(secondCard);
    await vm.MaybelistCards.AddToCardlist(thirdCard);
    await vm.MaybelistCards.AddToCardlist(firstCard);
    await vm.MaybelistCards.AddToCardlist(fourthCard);

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
    vm.CardFilters.ColorGroup = Cardlist.CardFilters.ColorGroups.Multi; // Multicolored
    CollectionAssert.AreEquivalent(new[] { fourthCard }, vm.MaybelistCards.FilteredAndSortedCardViewModels.Select(x => ((MTGCardViewModel)x).Model).ToArray());

    // CMC filter
    vm.CardFilters.Reset();
    vm.CardFilters.Cmc = 3; // Multicolored
    CollectionAssert.AreEquivalent(new[] { thirdCard }, vm.MaybelistCards.FilteredAndSortedCardViewModels.Select(x => ((MTGCardViewModel)x).Model).ToArray());
  }
  #endregion

  #region HasUnsavedChanges
  [TestMethod]
  public void HasUnsavedChangesTest_Init() => Assert.IsFalse(new DeckBuilderViewModel(null, null).HasUnsavedChanges);

  [TestMethod]
  public async Task HasUnsavedChangesTest_AddCard()
  {
    DeckBuilderViewModel vm = new(null, null);

    await vm.DeckCards.AddToCardlist(Mocker.MTGCardModelMocker.CreateMTGCardModel());
    Assert.IsTrue(vm.HasUnsavedChanges);
  }

  [TestMethod]
  public async Task HasUnsavedChangesTest_IncreaseCount()
  {
    DeckBuilderViewModel vm = new(null, null);

    var card = Mocker.MTGCardModelMocker.CreateMTGCardModel();
    await vm.DeckCards.AddToCardlist(card);
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
    DeckBuilderViewModel vm = new(new TestCardAPI(), null, dialogs: new TestDeckBuilderViewDialogs()
    {
      CardPrintDialog = new() { Values = newPrint},
    });

    await vm.DeckCards.AddToCardlist(card);
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
    var cardlist = new Cardlist(deck, CardlistType.Deck, null, null);

    await cardlist.AddToCardlist(Mocker.MTGCardModelMocker.CreateMTGCardModel());
    Assert.AreEqual(1, deck.DeckCards.Count);
  }

  [TestMethod]
  public async Task AddToCardlistCommandTest_AlreadyExists_Add()
  {
    var deck = new MTGCardDeck();
    var cardlist = new Cardlist(deck, CardlistType.Deck, new TestDeckBuilderViewDialogs()
    {
      CardAlreadyInDeckDialog = new(),
    }, null);

    await cardlist.AddToCardlist(Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "First", count: 1));
    await cardlist.AddToCardlist(Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "First", count: 2));
    Assert.AreEqual(3, deck.DeckCards.Sum(x => x.Count));
  }

  [TestMethod]
  public async Task AddToCardlistCommandTest_AlreadyExists_Skip()
  {
    var deck = new MTGCardDeck();
    var cardlist = new Cardlist(deck, CardlistType.Deck, new TestDeckBuilderViewDialogs()
    {
      CardAlreadyInDeckDialog = new() { Result = ContentDialogResult.None }
    }, null);

    await cardlist.AddToCardlist(Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "First", count: 1));
    await cardlist.AddToCardlist(Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "First", count: 2));
    Assert.AreEqual(1, deck.DeckCards.Sum(x => x.Count));
  }

  [TestMethod]
  public async Task RemoveFromCardlistCommandTest()
  {
    var deck = new MTGCardDeck();
    var cardlist = new DeckBuilderViewModel.Cardlist(deck, CardlistType.Deck, null, null);
    var card = Mocker.MTGCardModelMocker.CreateMTGCardModel();

    await cardlist.AddToCardlist(card);
    Assert.AreEqual(1, deck.DeckCards.Count);

    cardlist.RemoveFromCardlist(card);
    Assert.AreEqual(0, deck.DeckCards.Count);
  }

  [TestMethod]
  public async Task RemoveFromCardlistCommandTest_CardViewModelCommand()
  {
    var deck = new MTGCardDeck();
    var cardlist = new Cardlist(deck, CardlistType.Deck, null, null);
    var card = Mocker.MTGCardModelMocker.CreateMTGCardModel();

    await cardlist.AddToCardlist(card);
    Assert.AreEqual(1, deck.DeckCards.Count);

    ((MTGCardViewModel)cardlist.FilteredAndSortedCardViewModels[0]).DeleteCardCommand.Execute(card);
    Assert.AreEqual(0, deck.DeckCards.Count);
  }
  #endregion

  #region Price Changing
  [TestMethod]
  public async Task ChangePrintTest()
  {
    var card = Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "First", scryfallId: Guid.NewGuid());
    var newPrint = new MTGCardViewModel(Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "First", scryfallId: Guid.NewGuid()));
    DeckBuilderViewModel vm = new(new TestCardAPI(), null, dialogs: new TestDeckBuilderViewDialogs()
    {
      CardPrintDialog = new() { Values = newPrint }
    });

    await vm.DeckCards.AddToCardlist(card);

    await vm.DeckCards.ChangePrintDialog(card);
    Assert.AreEqual(card.Info.ScryfallId, newPrint.Model.Info.ScryfallId);
    Assert.AreEqual(newPrint.Model.Info.ScryfallId, ((MTGCardViewModel)vm.DeckCards.FilteredAndSortedCardViewModels[0]).Model.Info.ScryfallId);
  }

  [TestMethod]
  public async Task DeckPriceChangesTest_CardlistChanges()
  {
    var cardlist = new Cardlist(new(), CardlistType.Deck, null, null);
    using var asserter = new CardlistPropertyAsserter(cardlist, nameof(cardlist.EuroPrice));

    await cardlist.AddToCardlist(Mocker.MTGCardModelMocker.CreateMTGCardModel(price: 500));
    Assert.IsTrue(asserter.PropertyChanged);
  }

  [TestMethod]
  public async Task DeckPriceChangesTest_CardCountChanges()
  {
    var card = Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "First", scryfallId: Guid.NewGuid());
    var cardlist = new Cardlist(new() { DeckCards = new() { card } }, CardlistType.Deck, null, null);
    DeckBuilderViewModel vm = new(new TestCardAPI(), null, null);

    await vm.DeckCards.AddToCardlist(card);

    using var asserter = new CardlistPropertyAsserter(cardlist, nameof(cardlist.EuroPrice));
    card.Count++;
    Assert.IsTrue(asserter.PropertyChanged);
  }

  [TestMethod]
  public async Task DeckPriceChangesTest_CardPrintChanges()
  {
    var card = Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "First", scryfallId: Guid.NewGuid());
    var cardlist = new Cardlist(new() { DeckCards = new() { card } }, CardlistType.Deck, null, null);
    var newPrint = new MTGCardViewModel(Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "First", scryfallId: Guid.NewGuid()));
    DeckBuilderViewModel vm = new(new TestCardAPI(), null, dialogs: new TestDeckBuilderViewDialogs()
    {
      CardPrintDialog = new() { Values = newPrint }
    });

    await vm.DeckCards.AddToCardlist(card);

    using var asserter = new CardlistPropertyAsserter(cardlist, nameof(cardlist.EuroPrice));
    await vm.DeckCards.ChangePrintDialog(card);
    Assert.IsTrue(asserter.PropertyChanged);
  }
  #endregion
}