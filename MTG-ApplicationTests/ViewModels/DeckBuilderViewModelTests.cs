using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTGApplication.Models;
using MTGApplication.ViewModels;
using static MTGApplicationTests.Database.InMemoryMTGDeckRepositoryTests;
using static MTGApplicationTests.Services.TestDialogService;
using static MTGApplication.Enums;
using MTGApplicationTests.Services;
using MTGApplicationTests.API;
using Microsoft.UI.Xaml.Controls;
using static MTGApplication.ViewModels.DeckBuilderViewModel;
using CommunityToolkit.WinUI.UI;
using static MTGApplication.Models.MTGCard;

namespace MTGApplicationTests.ViewModels
{
  [TestClass]
  public class DeckBuilderViewModelTests
  {
    #region NewDeckDialogCommand
    [TestMethod]
    public async Task NewDeckDialogCommandTest_NoSave()
    {
      var deckName = "First";

      using TestInMemoryMTGDeckRepository repo = new();
      DeckBuilderViewModel vm = new(null, repo, dialogs: new DeckBuilderViewModel.DeckBuilderViewDialogs() // Override dialogs
      {
        SaveUnsavedDialog = new TestConfirmationDialog(ContentDialogResult.Secondary), // No save
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
      Assert.AreEqual(2, (await repo.Get(deckName)).DeckCards.Count);
    }

    [TestMethod]
    public async Task NewDeckDialogCommandTest_Save_NoExistingDeck()
    {
      var deckName = "First";
      
      using TestInMemoryMTGDeckRepository repo = new();
      DeckBuilderViewModel vm = new(null, repo, dialogs: new DeckBuilderViewModel.DeckBuilderViewDialogs() // Override dialogs
      {
        SaveUnsavedDialog = new TestConfirmationDialog(ContentDialogResult.Primary), // Save
        SaveDialog = new TestTextBoxDialog(ContentDialogResult.Primary, deckName), // Give deck name
      });

      // Add a card to the deck so the unsaved dialog appears
      await vm.DeckCards.AddToCardlist(Mocker.MTGCardModelMocker.CreateMTGCardModel());
      Assert.IsTrue(vm.HasUnsavedChanges);

      await vm.NewDeckDialog();
      Assert.IsFalse(vm.HasUnsavedChanges);
      Assert.AreEqual(0, vm.DeckCards.CardlistSize);
      Assert.AreEqual(string.Empty, vm.CardDeckName);
      Assert.AreEqual(1, (await repo.Get()).ToList().Count);
      Assert.AreEqual(1, (await repo.Get(deckName)).DeckCards.Count);
    }

    [TestMethod]
    public async Task NewDeckDialogCommandTest_Save_ExistingDeck_NoOverride()
    {
      var deckName = "First";
      
      using TestInMemoryMTGDeckRepository repo = new();
      DeckBuilderViewModel vm = new(null, repo, dialogs: new DeckBuilderViewModel.DeckBuilderViewDialogs() // Override dialogs
      {
        SaveUnsavedDialog = new TestConfirmationDialog(ContentDialogResult.Primary), // Save
        SaveDialog = new TestTextBoxDialog(ContentDialogResult.Primary, deckName), // Give deck name
        OverrideDialog = new TestConfirmationDialog(ContentDialogResult.Secondary), // Don't override
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
      Assert.AreEqual(2, (await repo.Get(deckName)).DeckCards.Count);
    }

    [TestMethod]
    public async Task NewDeckDialogCommandTest_Save_ExistingDeck_Override()
    {
      var deckName = "First";
      
      using TestInMemoryMTGDeckRepository repo = new();
      DeckBuilderViewModel vm = new(null, repo, dialogs: new DeckBuilderViewModel.DeckBuilderViewDialogs()
      {
        SaveUnsavedDialog = new TestConfirmationDialog(ContentDialogResult.Primary), // Save
        SaveDialog = new TestTextBoxDialog(ContentDialogResult.Primary, deckName), // Give name
        OverrideDialog = new TestConfirmationDialog(ContentDialogResult.Primary) // Override
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
      Assert.AreEqual(1, (await repo.Get(deckName)).DeckCards.Count);
    }

    [TestMethod]
    public async Task NewDeckDialogCommandTest_Save_ExistingDeck_CancelOverride()
    {
      var deckName = "First";
      using TestInMemoryMTGDeckRepository repo = new();
      DeckBuilderViewModel vm = new(null, repo, dialogs: new DeckBuilderViewModel.DeckBuilderViewDialogs()
      {
        SaveUnsavedDialog = new TestConfirmationDialog(ContentDialogResult.Primary), // Save
        SaveDialog = new TestTextBoxDialog(ContentDialogResult.Primary, deckName), // Give name
        OverrideDialog = new TestConfirmationDialog(ContentDialogResult.None) // Cancel
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
      Assert.AreEqual(2, (await repo.Get(deckName)).DeckCards.Count);
    }
    #endregion

    #region LoadDeckDialogCommand
    [TestMethod]
    public async Task LoadDeckDialogCommandTest_NoSave()
    {
      var deckName = "First";
      
      using TestInMemoryMTGDeckRepository repo = new();
      DeckBuilderViewModel vm = new(null, repo, dialogs: new DeckBuilderViewModel.DeckBuilderViewDialogs() // Override dialogs
      {
        SaveUnsavedDialog = new TestConfirmationDialog(ContentDialogResult.Secondary), // No Save
        LoadDialog = new TestComboBoxDialog(ContentDialogResult.Primary, deckName), // Load deck
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
      DeckBuilderViewModel vm = new(null, repo, dialogs: new DeckBuilderViewModel.DeckBuilderViewDialogs()
      {
        SaveUnsavedDialog = new TestConfirmationDialog(ContentDialogResult.None),
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
      var deckName = "First";

      using TestInMemoryMTGDeckRepository repo = new();
      DeckBuilderViewModel vm = new(null, repo, dialogs: new DeckBuilderViewDialogs() // Override dialogs
      {
        LoadDialog = new TestComboBoxDialog(ContentDialogResult.Primary, deckName), // Load deck
      });

      // Add deck to the database
      var dbDeck = new MTGCardDeck()
      {
        Name = deckName,
        DeckCards = new()
        {
          Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "First", cmc: 3),
          Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "Second", cmc: 2),
          Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "Third", cmc: 1),
        }
      };
      await repo.Add(dbDeck);

      vm.SelectedSortDirection = SortDirection.Ascending;
      vm.SelectedSortProperty = SortMTGProperty.CMC;
      var sortedDbCards = dbDeck.DeckCards.OrderBy(x => x.Info.CMC);

      await vm.LoadDeckDialog();
      CollectionAssert.AreEqual(sortedDbCards.Select(x => x.Info.CMC).ToList(), vm.DeckCards.FilteredAndSortedCardViewModels.Select(x => ((MTGCardViewModel)x).Model.Info.CMC).ToList());
    }
    #endregion

    #region SaveDeckDialogCommand
    [TestMethod]
    public async Task SaveDeckDialogCommandTest_NoExistingDeck()
    {
      var deckName = "First";
      
      using TestInMemoryMTGDeckRepository repo = new();
      DeckBuilderViewModel vm = new(null, repo, dialogs: new DeckBuilderViewModel.DeckBuilderViewDialogs() // Override dialogs
      {
        SaveDialog = new TestTextBoxDialog(ContentDialogResult.Primary, deckName), // Save deck
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
      DeckBuilderViewModel vm = new(null, repo, dialogs: new DeckBuilderViewModel.DeckBuilderViewDialogs()
      {
        SaveDialog = new TestTextBoxDialog(ContentDialogResult.Primary, deckName), // Save
        OverrideDialog = new TestConfirmationDialog(ContentDialogResult.Primary) // Override
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
      Assert.AreEqual(1, (await repo.Get(deckName)).DeckCards.Count);
      Assert.AreEqual(deckName, vm.CardDeckName);
    }

    [TestMethod]
    public async Task SaveDeckDialogCommandTest_ExistingDeck_DifferentName()
    {
      var loadName = "First";
      var saveName = "Second";
      
      using TestInMemoryMTGDeckRepository repo = new();
      DeckBuilderViewModel vm = new(null, repo, dialogs: new DeckBuilderViewModel.DeckBuilderViewDialogs()
      {
        SaveDialog = new TestTextBoxDialog(ContentDialogResult.Primary, saveName),
        OverrideDialog = new TestConfirmationDialog(ContentDialogResult.Primary),
        LoadDialog = new TestComboBoxDialog(ContentDialogResult.Primary, loadName)
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
      Assert.AreEqual(3, (await repo.Get(saveName)).DeckCards.Count);
      Assert.AreEqual(saveName, vm.CardDeckName);
    }

    [TestMethod]
    public async Task SaveDeckDialogCommandTest_Cancel()
    {
      using TestInMemoryMTGDeckRepository repo = new();
      DeckBuilderViewModel vm = new(null, repo, dialogs: new DeckBuilderViewModel.DeckBuilderViewDialogs()
      {
        OverrideDialog = new TestConfirmationDialog(ContentDialogResult.None),
        SaveDialog = new TestTextBoxDialog(ContentDialogResult.None, "Name"),
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
      DeckBuilderViewModel vm = new(null, repo, dialogs: new DeckBuilderViewModel.DeckBuilderViewDialogs()
      {
        SaveDialog = new TestTextBoxDialog(ContentDialogResult.Primary, "First")
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
      DeckBuilderViewModel vm = new(null, repo, dialogs: new DeckBuilderViewModel.DeckBuilderViewDialogs()
      {
        SaveDialog = new TestTextBoxDialog(ContentDialogResult.Primary, deckName), // Save deck
        DeleteDialog = new TestConfirmationDialog(ContentDialogResult.Primary), // Delete
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
      DeckBuilderViewModel vm = new(null, repo, dialogs: new DeckBuilderViewModel.DeckBuilderViewDialogs()
      {
        SaveDialog = new TestTextBoxDialog(ContentDialogResult.Primary, deckName), // Save
        DeleteDialog = new TestConfirmationDialog(ContentDialogResult.None) // Cancel
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
      DeckBuilderViewModel vm = new(null, repo, dialogs: new DeckBuilderViewModel.DeckBuilderViewDialogs()
      {
        SaveDialog = new TestTextBoxDialog(ContentDialogResult.Primary, deckName), // Save deck
        DeleteDialog = new TestConfirmationDialog(ContentDialogResult.Secondary), // Reject deletion
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
      MTGCard[] importedCards = new MTGCard[]
      {
        Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "First"),
        Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "Second"),
        Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "Third"),
      };

      DeckBuilderViewModel vm = new(new TestCardAPI(importedCards), null, dialogs: new DeckBuilderViewModel.DeckBuilderViewDialogs()
      {
        ImportDialog = new TestTextAreaDialog(ContentDialogResult.Primary, nameof(importedCards)),
      });

      await vm.DeckCards.ImportToCardlistDialog();
      Assert.AreEqual(3, vm.DeckCards.CardlistSize);
    }

    [TestMethod]
    public async Task ImportToDeckCardsDialogTest_AlreadyExists_Add()
    {
      MTGCard[] importedCards = new MTGCard[]
      {
        Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "First"),
        Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "Second"),
        Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "Third"),
      };

      DeckBuilderViewModel vm = new(new TestCardAPI(importedCards), null, dialogs: new DeckBuilderViewModel.DeckBuilderViewDialogs()
      {
        ImportDialog = new TestTextAreaDialog(ContentDialogResult.Primary, nameof(importedCards)),
        MultipleCardsAlreadyInDeckDialog = new TestCheckBoxDialog(ContentDialogResult.Primary, true)
      });

      await vm.DeckCards.ImportToCardlistDialog();
      await vm.DeckCards.ImportToCardlistDialog();
      Assert.AreEqual(6, vm.DeckCards.CardlistSize);
    }

    [TestMethod]
    public async Task ImportToDeckCardsDialogTest_AlreadyExists_Skip()
    {
      MTGCard[] firstImport = new MTGCard[]
      {
        Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "First"),
        Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "Second"),
        Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "Third"),
      };
      MTGCard[] secondImport = new MTGCard[]
      {
        Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "First"),  // Old
        Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "Second"), // Old
        Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "Fourth"), // New
      };

      var cardAPI = new TestCardAPI(firstImport);
      DeckBuilderViewModel vm = new(cardAPI, null, dialogs: new DeckBuilderViewDialogs()
      {
        ImportDialog = new TestTextAreaDialog(ContentDialogResult.Primary, nameof(firstImport)),
        MultipleCardsAlreadyInDeckDialog = new TestCheckBoxDialog(ContentDialogResult.None, true)
      });

      await vm.DeckCards.ImportToCardlistDialog();
      cardAPI.ExpectedCards = secondImport;

      await vm.DeckCards.ImportToCardlistDialog();
      Assert.AreEqual(4, vm.DeckCards.CardlistSize);
    }

    [TestMethod]
    public async Task ExportCardsDialogTest()
    {
      var deck = new MTGCardDeck()
      {
        DeckCards = new System.Collections.ObjectModel.ObservableCollection<MTGCard>
        {
          Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "First", count: 1),
          Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "Second", count: 2),
          Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "Third", count: 3),
        }
      };
      var cardlist = new DeckBuilderViewModel.Cardlist(deck, CardlistType.Deck, null, null);
      var expectedText = cardlist.GetExportString();


      using TestIO.TestClipboard clipboard = new();
      DeckBuilderViewModel vm = new(cardAPI: null, deckRepository: null, dialogs: new DeckBuilderViewModel.DeckBuilderViewDialogs()
      {
        ExportDialog = new TestTextAreaDialog(ContentDialogResult.Primary, expectedText),
      }, clipboardService: clipboard);

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

      vm.SelectedSortProperty = SortMTGProperty.CMC;
      vm.SelectedSortDirection = SortDirection.Ascending;
      await vm.DeckCards.AddToCardlist(secondCard);
      await vm.DeckCards.AddToCardlist(thirdCard);
      await vm.DeckCards.AddToCardlist(firstCard);
      // The viewmodels should be sorted when cards are added to the deck
      CollectionAssert.AreEqual(new[] { secondCard, firstCard, thirdCard }, vm.DeckCards.FilteredAndSortedCardViewModels.Select(x => ((MTGCardViewModel)x).Model).ToArray());

      vm.SortByProperty(SortMTGProperty.Name.ToString());
      CollectionAssert.AreEqual(new[] { firstCard, secondCard, thirdCard }, vm.DeckCards.FilteredAndSortedCardViewModels.Select(x => ((MTGCardViewModel)x).Model).ToArray());

      vm.SortByDirection(SortDirection.Descending.ToString());
      CollectionAssert.AreEqual(new[] { thirdCard, secondCard, firstCard }, vm.DeckCards.FilteredAndSortedCardViewModels.Select(x => ((MTGCardViewModel)x).Model).ToArray());
    }
    
    [TestMethod]
    public async Task DeckFilterTest()
    {
      DeckBuilderViewModel vm = new(null, null);

      var firstCard = Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "A", cmc: 2, typeLine: "Artifact", frontFace: Mocker.MTGCardModelMocker.CreateCardFace(new ColorTypes[] { ColorTypes.R }));
      var secondCard = Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "B", cmc: 1, typeLine: "Creature", frontFace: Mocker.MTGCardModelMocker.CreateCardFace(new ColorTypes[] {ColorTypes.W}));
      var thirdCard = Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "C", cmc: 3, typeLine: "Land", frontFace: Mocker.MTGCardModelMocker.CreateCardFace(new ColorTypes[] { ColorTypes.C }));

      await vm.DeckCards.AddToCardlist(secondCard);
      await vm.DeckCards.AddToCardlist(thirdCard);
      await vm.DeckCards.AddToCardlist(firstCard);

      vm.DeckCards.Filters.NameText = "a";
      CollectionAssert.AreEquivalent(new[] { firstCard }, vm.DeckCards.FilteredAndSortedCardViewModels.Select(x => ((MTGCardViewModel)x).Model).ToArray());

      vm.DeckCards.Filters.Reset();
      CollectionAssert.AreEquivalent(new[] { thirdCard, secondCard, firstCard }, vm.DeckCards.FilteredAndSortedCardViewModels.Select(x => ((MTGCardViewModel)x).Model).ToArray());

      vm.DeckCards.Filters.White = false; 
      CollectionAssert.AreEquivalent(new[] { thirdCard, firstCard }, vm.DeckCards.FilteredAndSortedCardViewModels.Select(x => ((MTGCardViewModel)x).Model).ToArray());

      vm.DeckCards.Filters.Reset();
      vm.DeckCards.Filters.TypeText = "Cr";
      CollectionAssert.AreEquivalent(new[] { secondCard }, vm.DeckCards.FilteredAndSortedCardViewModels.Select(x => ((MTGCardViewModel)x).Model).ToArray());
    }
    #endregion

    #region HasUnsavedChanges
    [TestMethod]
    public void HasUnsavedChangesTest_Init()
    {
      Assert.IsFalse(new DeckBuilderViewModel(null, null).HasUnsavedChanges);
    }

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
      DeckBuilderViewModel vm = new(new TestCardAPI(), null, dialogs: new()
      {
        CardPrintDialog = new TestGridViewDialog(ContentDialogResult.Primary, newPrint)
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
      var cardlist = new Cardlist(deck, CardlistType.Deck, new DeckBuilderViewDialogs()
      {
        CardAlreadyInDeckDialog = new TestConfirmationDialog(ContentDialogResult.Primary),
      }, null);

      await cardlist.AddToCardlist(Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "First", count: 1));
      await cardlist.AddToCardlist(Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "First", count: 2));
      Assert.AreEqual(3, deck.DeckCards.Sum(x => x.Count));
    }

    [TestMethod]
    public async Task AddToCardlistCommandTest_AlreadyExists_Skip()
    {
      var deck = new MTGCardDeck();
      var cardlist = new Cardlist(deck, CardlistType.Deck, new DeckBuilderViewDialogs()
      {
        CardAlreadyInDeckDialog = new TestConfirmationDialog(ContentDialogResult.None),
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
      DeckBuilderViewModel vm = new(new TestCardAPI(), null, dialogs: new()
      {
        CardPrintDialog = new TestGridViewDialog(ContentDialogResult.Primary, newPrint)
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
      DeckBuilderViewModel vm = new(new TestCardAPI(), null, dialogs: new()
      {
        CardPrintDialog = new TestGridViewDialog(ContentDialogResult.Primary, newPrint)
      });
      
      await vm.DeckCards.AddToCardlist(card);

      using var asserter = new CardlistPropertyAsserter(cardlist, nameof(cardlist.EuroPrice));
      await vm.DeckCards.ChangePrintDialog(card);
      Assert.IsTrue(asserter.PropertyChanged);
    }
    #endregion
  }
}