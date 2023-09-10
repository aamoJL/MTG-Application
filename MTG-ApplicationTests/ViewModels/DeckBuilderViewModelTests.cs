using CommunityToolkit.WinUI.UI;
using Microsoft.UI.Xaml.Controls;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTGApplication.Models;
using MTGApplication.ViewModels;
using MTGApplicationTests.API;
using MTGApplicationTests.Services;
using static MTGApplication.Enums;
using static MTGApplication.Services.DialogService;
using static MTGApplication.ViewModels.DeckBuilderViewModel;
using static MTGApplicationTests.Database.InMemoryMTGDeckRepositoryTests;
using static MTGApplicationTests.Services.TestDialogService;

namespace MTGApplicationTests.ViewModels;

[TestClass]
public class DeckBuilderViewModelTests
{
  public class TestDeckBuilderViewDialogs : DeckBuilderViewDialogs
  {
    public TestDeckBuilderViewDialogs(DeckBuilderViewModel vm) => ViewModel = vm;

    public DeckBuilderViewModel ViewModel { get; }

    public TestDialogResult<string> LoadDialog { get; set; } = new();
    public TestDialogResult<string> SaveDialog { get; set; } = new();
    public TestDialogResult SaveUnsavedDialog { get; set; } = new();
    public TestDialogResult OverrideDialog { get; set; } = new();
    public TestDialogResult DeleteDialog { get; set; } = new();

    public override ConfirmationDialog GetOverrideDialog(string name)
    {
      ViewModel.OnGetDialogWrapper += (s, args) => args.DialogWrapper = new TestDialogWrapper(OverrideDialog.Result);
      var dialog = base.GetOverrideDialog(name);
      return dialog;
    }
    public override ConfirmationDialog GetDeleteDialog(string name)
    {
      ViewModel.OnGetDialogWrapper += (s, args) => args.DialogWrapper = new TestDialogWrapper(DeleteDialog.Result);
      var dialog = base.GetDeleteDialog(name);
      return dialog;
    }
    public override ConfirmationDialog GetSaveUnsavedDialog(string name = "")
    {
      ViewModel.OnGetDialogWrapper += (s, args) => args.DialogWrapper = new TestDialogWrapper(SaveUnsavedDialog.Result);
      var dialog = base.GetSaveUnsavedDialog();
      return dialog;
    }
    public override ComboBoxDialog GetLoadDialog(string[] names)
    {
      ViewModel.OnGetDialogWrapper += (s, args) => args.DialogWrapper = new TestDialogWrapper(LoadDialog.Result);
      var dialog = base.GetLoadDialog(names);
      dialog.Selection = LoadDialog.Values;
      return dialog;
    }
    public override TextBoxDialog GetSaveDialog(string name)
    {
      ViewModel.OnGetDialogWrapper += (s, args) => args.DialogWrapper = new TestDialogWrapper(SaveDialog.Result);
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
    DeckBuilderViewModel vm = new(null, repo);
    vm.Dialogs = new TestDeckBuilderViewDialogs(vm) // Override dialogs
    {
      SaveUnsavedDialog = new() { Result = ContentDialogResult.Secondary }, // No save
    };

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
    await vm.DeckCards.Add(Mocker.MTGCardModelMocker.CreateMTGCardModel());
    Assert.IsTrue(vm.HasUnsavedChanges);

    await vm.NewDeckDialog();
    Assert.IsFalse(vm.HasUnsavedChanges);
    Assert.AreEqual(0, vm.DeckCards.CardlistSize);
    Assert.AreEqual(string.Empty, vm.CardDeck.Name);
    Assert.AreEqual(1, (await repo.Get()).ToList().Count);
    Assert.AreEqual(2, (await repo.Get(deckName))?.DeckCards.Count);
  }

  [TestMethod]
  public async Task NewDeckDialogCommandTest_Save_NoExistingDeck()
  {
    var deckName = "First";

    using TestInMemoryMTGDeckRepository repo = new();
    DeckBuilderViewModel vm = new(null, repo);
    vm.Dialogs = new TestDeckBuilderViewDialogs(vm)
    {
      SaveUnsavedDialog = new(), // Save
      SaveDialog = new() { Values = deckName }, // Give deck name
    };

    // Add a card to the deck so the unsaved dialog appears
    await vm.DeckCards.Add(Mocker.MTGCardModelMocker.CreateMTGCardModel());
    Assert.IsTrue(vm.HasUnsavedChanges);

    await vm.NewDeckDialog();
    Assert.IsFalse(vm.HasUnsavedChanges);
    Assert.AreEqual(0, vm.DeckCards.CardlistSize);
    Assert.AreEqual(string.Empty, vm.CardDeck.Name);
    Assert.AreEqual(1, (await repo.Get()).ToList().Count);
    Assert.AreEqual(1, (await repo.Get(deckName))?.DeckCards.Count);
  }

  [TestMethod]
  public async Task NewDeckDialogCommandTest_Save_ExistingDeck_NoOverride()
  {
    var deckName = "First";

    using TestInMemoryMTGDeckRepository repo = new();
    DeckBuilderViewModel vm = new(null, repo);
    vm.Dialogs = new TestDeckBuilderViewDialogs(vm)
    {
      SaveUnsavedDialog = new(), // Save
      SaveDialog = new() { Values = deckName }, // Give deck name
      OverrideDialog = new() { Result = ContentDialogResult.Secondary }, // Don't override
    };

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
    await vm.DeckCards.Add(Mocker.MTGCardModelMocker.CreateMTGCardModel());
    Assert.IsTrue(vm.HasUnsavedChanges);

    await vm.NewDeckDialog();
    Assert.IsFalse(vm.HasUnsavedChanges);
    Assert.AreEqual(0, vm.DeckCards.CardlistSize);
    Assert.AreEqual(string.Empty, vm.CardDeck.Name);
    Assert.AreEqual(1, (await repo.Get()).ToList().Count);
    Assert.AreEqual(2, (await repo.Get(deckName))?.DeckCards.Count);
  }

  [TestMethod]
  public async Task NewDeckDialogCommandTest_Save_ExistingDeck_Override()
  {
    var deckName = "First";

    using TestInMemoryMTGDeckRepository repo = new();
    DeckBuilderViewModel vm = new(null, repo);
    vm.Dialogs = new TestDeckBuilderViewDialogs(vm)
    {
      SaveUnsavedDialog = new(), // Save
      SaveDialog = new() { Values = deckName }, // Give name
      OverrideDialog = new() // Override
    };

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
    await vm.DeckCards.Add(Mocker.MTGCardModelMocker.CreateMTGCardModel());
    Assert.IsTrue(vm.HasUnsavedChanges);

    await vm.NewDeckDialog();
    Assert.IsFalse(vm.HasUnsavedChanges);
    Assert.AreEqual(0, vm.DeckCards.CardlistSize);
    Assert.AreEqual(string.Empty, vm.CardDeck.Name);
    Assert.AreEqual(1, (await repo.Get()).ToList().Count);
    Assert.AreEqual(1, (await repo.Get(deckName))?.DeckCards.Count);
  }

  [TestMethod]
  public async Task NewDeckDialogCommandTest_Save_ExistingDeck_CancelOverride()
  {
    var deckName = "First";
    using TestInMemoryMTGDeckRepository repo = new();
    DeckBuilderViewModel vm = new(null, repo);
    vm.Dialogs = new TestDeckBuilderViewDialogs(vm)
    {
      SaveUnsavedDialog = new(), // Save
      SaveDialog = new() { Values = deckName }, // Give name
      OverrideDialog = new() { Result = ContentDialogResult.None } // Cancel
    };

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
    await vm.DeckCards.Add(Mocker.MTGCardModelMocker.CreateMTGCardModel());
    Assert.IsTrue(vm.HasUnsavedChanges);

    await vm.NewDeckDialog();
    Assert.IsTrue(vm.HasUnsavedChanges);
    Assert.AreEqual(1, vm.DeckCards.CardlistSize);
    Assert.AreEqual(string.Empty, vm.CardDeck.Name);
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
    DeckBuilderViewModel vm = new(null, repo);
    vm.Dialogs = new TestDeckBuilderViewDialogs(vm) // Override dialogs
    {
      SaveUnsavedDialog = new() { Result = ContentDialogResult.Secondary }, // No Save
      LoadDialog = new() { Values = deckName }, // Load deck
    };

    // Add deck with the same name to the database
    var dbDeck = new MTGCardDeck()
    {
      Name = deckName,
      DeckCards = new()
      {
        Mocker.MTGCardModelMocker.CreateMTGCardModel(),
        Mocker.MTGCardModelMocker.CreateMTGCardModel(),
      },
      Maybelist = new()
      {
        Mocker.MTGCardModelMocker.CreateMTGCardModel(),
        Mocker.MTGCardModelMocker.CreateMTGCardModel(),
        Mocker.MTGCardModelMocker.CreateMTGCardModel(),
      },
      Wishlist = new()
      {
        Mocker.MTGCardModelMocker.CreateMTGCardModel(),
        Mocker.MTGCardModelMocker.CreateMTGCardModel(),
        Mocker.MTGCardModelMocker.CreateMTGCardModel(),
        Mocker.MTGCardModelMocker.CreateMTGCardModel(),
      },
      Removelist = new()
      {
        Mocker.MTGCardModelMocker.CreateMTGCardModel(),
        Mocker.MTGCardModelMocker.CreateMTGCardModel(),
        Mocker.MTGCardModelMocker.CreateMTGCardModel(),
        Mocker.MTGCardModelMocker.CreateMTGCardModel(),
        Mocker.MTGCardModelMocker.CreateMTGCardModel(),
      },
    };
    await repo.Add(dbDeck);

    // Add a card to the deck so the unsaved dialog appears.
    await vm.DeckCards.Add(Mocker.MTGCardModelMocker.CreateMTGCardModel());
    Assert.IsTrue(vm.HasUnsavedChanges);

    await vm.LoadDeckDialog();
    Assert.IsFalse(vm.HasUnsavedChanges);
    Assert.AreEqual(2, vm.DeckCards.CardlistSize);
    Assert.AreEqual(deckName, vm.CardDeck.Name);
    Assert.AreEqual(2, vm.DeckCards.Cardlist.Count);
    Assert.AreEqual(3, vm.MaybelistCards.Cardlist.Count);
    Assert.AreEqual(4, vm.WishlistCards.Cardlist.Count);
    Assert.AreEqual(5, vm.RemovelistCards.Cardlist.Count);
  }

  [TestMethod]
  public async Task LoadDeckDialogCommandTest_Commanders()
  {
    var deckName = "First";
    var commander = Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "Commander");
    var partner = Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "Partner");

    using TestInMemoryMTGDeckRepository repo = new();
    DeckBuilderViewModel vm = new(null, repo);
    vm.Dialogs = new TestDeckBuilderViewDialogs(vm) // Override dialogs
    {
      LoadDialog = new() { Values = deckName }, // Load deck
    };

    // Add deck with the same name to the database
    var dbDeck = new MTGCardDeck()
    {
      Name = deckName,
      DeckCards = new()
      {
        Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "First"),
        Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "Second")
      },
      Commander = commander,
      CommanderPartner = partner,
    };
    await repo.Add(dbDeck);

    await vm.LoadDeckDialog();
    Assert.IsFalse(vm.HasUnsavedChanges);
    Assert.AreEqual(commander.Info.Name, vm.CardDeck.Commander?.Info.Name);
    Assert.AreEqual(partner.Info.Name, vm.CardDeck.CommanderPartner?.Info.Name);
  }

  [TestMethod]
  public async Task LoadDeckDialogCommandTest_Cancel()
  {
    var deckName = "First";

    using TestInMemoryMTGDeckRepository repo = new();
    DeckBuilderViewModel vm = new(null, repo);
    vm.Dialogs = new TestDeckBuilderViewDialogs(vm)
    {
      SaveUnsavedDialog = new() { Result = ContentDialogResult.None },
    };

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
    await vm.DeckCards.Add(Mocker.MTGCardModelMocker.CreateMTGCardModel());
    Assert.IsTrue(vm.HasUnsavedChanges);

    await vm.LoadDeckDialog();
    Assert.IsTrue(vm.HasUnsavedChanges);
    Assert.AreEqual(1, vm.DeckCards.CardlistSize);
    Assert.AreEqual(string.Empty, vm.CardDeck.Name);
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
    DeckBuilderViewModel vm = new(null, repo);
    vm.Dialogs = new TestDeckBuilderViewDialogs(vm) // Override dialogs
    {
      LoadDialog = new() { Values = dbDeck.Name }, // Load deck
    };

    await repo.Add(dbDeck);

    vm.SortProperties.SortDirection = SortDirection.Ascending;
    vm.SortProperties.PrimarySortProperty = MTGSortProperty.CMC;
    vm.SortProperties.SecondarySortProperty = MTGSortProperty.Name;
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
    DeckBuilderViewModel vm = new(null, repo);
    vm.Dialogs = new TestDeckBuilderViewDialogs(vm) // Override dialogs
    {
      SaveDialog = new() { Values = deckName }, // Save deck
    };

    await vm.DeckCards.Add(Mocker.MTGCardModelMocker.CreateMTGCardModel());
    await vm.MaybelistCards.Add(Mocker.MTGCardModelMocker.CreateMTGCardModel());
    await vm.WishlistCards.Add(Mocker.MTGCardModelMocker.CreateMTGCardModel());
    await vm.RemovelistCards.Add(Mocker.MTGCardModelMocker.CreateMTGCardModel());
    Assert.IsTrue(vm.HasUnsavedChanges);

    await vm.SaveDeckDialog();
    Assert.IsFalse(vm.HasUnsavedChanges);
    Assert.AreEqual(1, vm.DeckCards.CardlistSize);
    Assert.AreEqual(1, (await repo.Get()).ToList().Count);
    Assert.AreEqual(deckName, vm.CardDeck.Name);

    var dbDeck = await repo.Get(deckName);
    Assert.AreEqual(vm.DeckCards.Cardlist.Count, dbDeck?.DeckCards.Count);
    Assert.AreEqual(vm.MaybelistCards.Cardlist.Count, dbDeck?.Maybelist.Count);
    Assert.AreEqual(vm.WishlistCards.Cardlist.Count, dbDeck?.Wishlist.Count);
    Assert.AreEqual(vm.RemovelistCards.Cardlist.Count, dbDeck?.Removelist.Count);
  }

  [TestMethod]
  public async Task SaveDeckDialogCommandTest_Commanders()
  {
    var deckName = "First";
    var commander = Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "Commander");
    var partner = Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "Partner");

    using TestInMemoryMTGDeckRepository repo = new();
    DeckBuilderViewModel vm = new(null, repo);
    vm.Dialogs = new TestDeckBuilderViewDialogs(vm) // Override dialogs
    {
      SaveDialog = new() { Values = deckName }, // Save deck
      OverrideDialog = new() { Result = ContentDialogResult.Primary },
    };

    vm.SetCommander(commander);
    Assert.IsTrue(vm.HasUnsavedChanges);

    await vm.SaveDeckDialog();
    Assert.IsFalse(vm.HasUnsavedChanges);
    Assert.AreEqual(commander.Info.Name, (await repo.Get(deckName))?.Commander?.Info.Name);
    Assert.AreEqual(null, (await repo.Get(deckName))?.CommanderPartner?.Info.Name);

    vm.SetCommanderPartner(partner);
    Assert.IsTrue(vm.HasUnsavedChanges);

    await vm.SaveDeckDialog();
    Assert.IsFalse(vm.HasUnsavedChanges);
    Assert.AreEqual(commander.Info.Name, (await repo.Get(deckName))?.Commander?.Info.Name);
    Assert.AreEqual(partner.Info.Name, (await repo.Get(deckName))?.CommanderPartner?.Info.Name);

    vm.SetCommander(null);
    vm.SetCommanderPartner(null);
    Assert.IsTrue(vm.HasUnsavedChanges);

    await vm.SaveDeckDialog();
    Assert.IsFalse(vm.HasUnsavedChanges);
    Assert.AreEqual(null, (await repo.Get(deckName))?.Commander?.Info.Name);
    Assert.AreEqual(null, (await repo.Get(deckName))?.CommanderPartner?.Info.Name);
  }

  [TestMethod]
  public async Task SaveDeckDialogCommandTest_ExistingDeck_Override()
  {
    var deckName = "First";

    using TestInMemoryMTGDeckRepository repo = new();
    DeckBuilderViewModel vm = new(null, repo);
    vm.Dialogs = new TestDeckBuilderViewDialogs(vm)
    {
      SaveDialog = new() { Values = deckName }, // Save
      OverrideDialog = new() // Override
    };

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

    await vm.DeckCards.Add(Mocker.MTGCardModelMocker.CreateMTGCardModel());

    await vm.SaveDeckDialog();
    Assert.IsFalse(vm.HasUnsavedChanges);
    Assert.AreEqual(1, vm.DeckCards.CardlistSize);
    Assert.AreEqual(1, (await repo.Get()).ToList().Count);
    Assert.AreEqual(1, (await repo.Get(deckName))?.DeckCards.Count);
    Assert.AreEqual(deckName, vm.CardDeck.Name);
  }

  [TestMethod]
  public async Task SaveDeckDialogCommandTest_ExistingDeck_DifferentName()
  {
    var loadName = "First";
    var saveName = "Second";

    using TestInMemoryMTGDeckRepository repo = new();
    DeckBuilderViewModel vm = new(null, repo);
    vm.Dialogs = new TestDeckBuilderViewDialogs(vm)
    {
      SaveDialog = new() { Values = saveName },
      OverrideDialog = new(),
      LoadDialog = new() { Values = loadName }
    };

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
    Assert.AreEqual(loadName, vm.CardDeck.Name);

    await vm.DeckCards.Add(Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "Third"));

    await vm.SaveDeckDialog(); // Rename with the saveName name
    Assert.IsFalse(vm.HasUnsavedChanges);
    Assert.AreEqual(3, vm.DeckCards.CardlistSize);
    Assert.AreEqual(1, (await repo.Get()).ToList().Count);
    Assert.AreEqual(3, (await repo.Get(saveName))?.DeckCards.Count);
    Assert.AreEqual(saveName, vm.CardDeck.Name);
  }

  [TestMethod]
  public async Task SaveDeckDialogCommandTest_Cancel()
  {
    using TestInMemoryMTGDeckRepository repo = new();
    DeckBuilderViewModel vm = new(null, repo);
    vm.Dialogs = new TestDeckBuilderViewDialogs(vm)
    {
      OverrideDialog = new() { Result = ContentDialogResult.None },
      SaveDialog = new() { Result = ContentDialogResult.None, Values = "Name" },
    };

    // Add card so the deck has unsaved changes
    await vm.DeckCards.Add(Mocker.MTGCardModelMocker.CreateMTGCardModel());

    await vm.SaveDeckDialog();

    Assert.IsTrue(vm.HasUnsavedChanges);
    Assert.AreEqual(1, vm.DeckCards.CardlistSize);
    Assert.AreEqual(string.Empty, vm.CardDeck.Name);
  }
  #endregion

  #region DeleteDeckDialogCommand
  [TestMethod]
  public async Task DeleteDeckDialogCommandTest_CanExecute()
  {
    using TestInMemoryMTGDeckRepository repo = new();
    DeckBuilderViewModel vm = new(null, repo);
    vm.Dialogs = new TestDeckBuilderViewDialogs(vm)
    {
      SaveDialog = new() { Values = "First" }
    };

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
    DeckBuilderViewModel vm = new(null, repo);
    vm.Dialogs = new TestDeckBuilderViewDialogs(vm)
    {
      SaveDialog = new() { Values = deckName }, // Save deck
      DeleteDialog = new(), // Delete
    };

    await vm.SaveDeckDialog();
    Assert.AreEqual(deckName, vm.CardDeck.Name);

    await vm.DeleteDeckDialog();
    Assert.AreEqual(0, (await repo.Get()).ToList().Count);
    Assert.AreEqual(string.Empty, vm.CardDeck.Name);
  }

  [TestMethod]
  public async Task DeleteDeckDialogCommandTest_Cancel()
  {
    var deckName = "First";

    using TestInMemoryMTGDeckRepository repo = new();
    DeckBuilderViewModel vm = new(null, repo);
    vm.Dialogs = new TestDeckBuilderViewDialogs(vm)
    {
      SaveDialog = new() { Values = deckName }, // Save
      DeleteDialog = new() { Result = ContentDialogResult.None } // Cancel
    };

    await vm.SaveDeckDialog();
    Assert.AreEqual(deckName, vm.CardDeck.Name);

    await vm.DeleteDeckDialog();
    Assert.AreEqual(1, (await repo.Get()).ToList().Count);
    Assert.AreEqual(deckName, vm.CardDeck.Name);
  }

  [TestMethod]
  public async Task DeleteDeckDialogCommandTest_Reject()
  {
    var deckName = "First";

    using TestInMemoryMTGDeckRepository repo = new();
    DeckBuilderViewModel vm = new(null, repo);
    vm.Dialogs = new TestDeckBuilderViewDialogs(vm)
    {
      SaveDialog = new() { Values = deckName }, // Save deck
      DeleteDialog = new() { Result = ContentDialogResult.Secondary }, // Reject deletion
    };

    await vm.SaveDeckDialog();
    Assert.AreEqual(deckName, vm.CardDeck.Name);

    await vm.DeleteDeckDialog();
    Assert.AreEqual(1, (await repo.Get()).ToList().Count);
    Assert.AreEqual(deckName, vm.CardDeck.Name);
  }
  #endregion

  #region Commander
  [TestMethod]
  public void SetCommanderCommandTest()
  {
    var vm = new DeckBuilderViewModel(null, null, null);
    var commanderCard = Mocker.MTGCardModelMocker.CreateMTGCardModel();

    vm.SetCommander(commanderCard);
    Assert.AreEqual(commanderCard, vm.CardDeck.Commander);
  }

  [TestMethod]
  public void SetCommanderCommandTest_UndoRedo()
  {
    var vm = new DeckBuilderViewModel(null, null, null);
    var commanderCard = Mocker.MTGCardModelMocker.CreateMTGCardModel();

    vm.SetCommander(commanderCard);
    Assert.AreEqual(commanderCard, vm.CardDeck.Commander);

    vm.CommandService.Undo();
    Assert.AreEqual(null, vm.CardDeck.Commander);

    vm.CommandService.Redo();
    Assert.AreEqual(commanderCard, vm.CardDeck.Commander);
  }

  [TestMethod]
  public void SetCommanderPartnerCommandTest()
  {
    var vm = new DeckBuilderViewModel(null, null, null);
    var PartnerCard = Mocker.MTGCardModelMocker.CreateMTGCardModel();

    vm.SetCommanderPartner(PartnerCard);
    Assert.AreEqual(PartnerCard, vm.CardDeck.CommanderPartner);
  }

  [TestMethod]
  public void SetCommanderPartnerCommandTest_UndoRedo()
  {
    var vm = new DeckBuilderViewModel(null, null, null);
    var PartnerCard = Mocker.MTGCardModelMocker.CreateMTGCardModel();

    vm.SetCommanderPartner(PartnerCard);
    Assert.AreEqual(PartnerCard, vm.CardDeck.CommanderPartner);

    vm.CommandService.Undo();
    Assert.AreEqual(null, vm.CardDeck.CommanderPartner);

    vm.CommandService.Redo();
    Assert.AreEqual(PartnerCard, vm.CardDeck.CommanderPartner);
  }
  #endregion

  #region Deck Properties

  [TestMethod]
  public async Task DeckSizeTest()
  {
    var vm = new DeckBuilderViewModel(null, null, null);
    var card = Mocker.MTGCardModelMocker.CreateMTGCardModel(count: 1);

    using var propAsserter = new PropertyChangedAssert(vm, nameof(vm.DeckSize));
    await vm.DeckCards.Add(card);
    Assert.AreEqual(card.Count, vm.DeckSize);
    Assert.IsTrue(propAsserter.PropertyChanged);

    propAsserter.Reset();
    card.Count += 1;
    Assert.AreEqual(card.Count, vm.DeckSize);
    Assert.IsTrue(propAsserter.PropertyChanged);

    propAsserter.Reset();
    vm.SetCommander(Mocker.MTGCardModelMocker.CreateMTGCardModel(count: 1));
    Assert.AreEqual(card.Count + 1, vm.DeckSize);
    Assert.IsTrue(propAsserter.PropertyChanged);

    propAsserter.Reset();
    vm.SetCommanderPartner(Mocker.MTGCardModelMocker.CreateMTGCardModel(count: 1));
    Assert.AreEqual(card.Count + 1 + 1, vm.DeckSize);
    Assert.IsTrue(propAsserter.PropertyChanged);

    propAsserter.Reset();
    vm.SetCommander(null);
    Assert.AreEqual(card.Count + 1, vm.DeckSize);
    Assert.IsTrue(propAsserter.PropertyChanged);
  }

  [TestMethod]
  public async Task DeckPriceTest()
  {
    var vm = new DeckBuilderViewModel(null, null, null);
    var card = Mocker.MTGCardModelMocker.CreateMTGCardModel(price: 1.5f);

    using var propAsserter = new PropertyChangedAssert(vm, nameof(vm.DeckPrice));
    await vm.DeckCards.Add(card);
    Assert.AreEqual(card.Info.Price, vm.DeckPrice); // 1.5€
    Assert.IsTrue(propAsserter.PropertyChanged);

    propAsserter.Reset();
    card.Count += 1;
    Assert.AreEqual(card.Info.Price * 2, vm.DeckPrice); // 3€
    Assert.IsTrue(propAsserter.PropertyChanged);

    propAsserter.Reset();
    vm.SetCommander(Mocker.MTGCardModelMocker.CreateMTGCardModel(price: 1));
    Assert.AreEqual(card.Info.Price * 2 + 1, vm.DeckPrice); // 4€
    Assert.IsTrue(propAsserter.PropertyChanged);

    propAsserter.Reset();
    vm.SetCommanderPartner(Mocker.MTGCardModelMocker.CreateMTGCardModel(price: 3));
    Assert.AreEqual(card.Info.Price * 2 + 1 + 3, vm.DeckPrice); // 7€
    Assert.IsTrue(propAsserter.PropertyChanged);

    propAsserter.Reset();
    vm.SetCommander(null);
    Assert.AreEqual(card.Info.Price * 2 + 3, vm.DeckPrice); // 6€
    Assert.IsTrue(propAsserter.PropertyChanged);
  }

  [TestMethod]
  public void DeckHasCommandersTest()
  {
    var vm = new DeckBuilderViewModel(null, null, null);

    Assert.IsFalse(vm.DeckHasCommanders());

    vm.SetCommander(Mocker.MTGCardModelMocker.CreateMTGCardModel());
    Assert.IsTrue(vm.DeckHasCommanders());

    vm.SetCommander(null);
    vm.SetCommanderPartner(Mocker.MTGCardModelMocker.CreateMTGCardModel());
    Assert.IsTrue(vm.DeckHasCommanders());
  }

  [TestMethod]
  public async Task DeckHasNameTest()
  {
    var deckName = "deck";
    using TestInMemoryMTGDeckRepository repo = new();
    DeckBuilderViewModel vm = new(null, repo);
    vm.Dialogs = new TestDeckBuilderViewDialogs(vm) // Override dialogs
    {
      SaveDialog = new() { Values = deckName }, // Save deck
      SaveUnsavedDialog = new() { Result = ContentDialogResult.Primary },
      LoadDialog = new() { Values = deckName, Result = ContentDialogResult.Primary }
    };
    await vm.DeckCards.Add(Mocker.MTGCardModelMocker.CreateMTGCardModel());

    Assert.IsFalse(vm.DeckHasName());

    await vm.SaveDeckDialog();
    Assert.IsTrue(vm.DeckHasName()); // Deck will have name when saved

    await vm.NewDeckDialog();
    await vm.LoadDeckDialog();
    Assert.IsTrue(vm.DeckHasName()); // Deck will have name when loaded
  }

  [TestMethod]
  public async Task DeckHasCardsTest()
  {
    var vm = new DeckBuilderViewModel(null, null, null);

    Assert.IsFalse(vm.DeckHasCards());

    await vm.DeckCards.Add(Mocker.MTGCardModelMocker.CreateMTGCardModel());
    Assert.IsTrue(vm.DeckHasCards());
  }

  #endregion
}