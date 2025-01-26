using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTGApplication.Features.DeckEditor.Editor.Models;
using MTGApplication.Features.DeckEditor.ViewModels;
using MTGApplicationTests.TestUtility.Importers;
using MTGApplicationTests.TestUtility.Mocker;
using System.Collections.ObjectModel;

namespace MTGApplicationTests.FeatureTests.DeckEditorTests.ViewModel.CardListViewModelTests.GroupedListTests;

public partial class CardGroupViewModelTests
{
  [TestClass]
  public class StateTests
  {
    [TestMethod]
    public void Init()
    {
      var groupKey = "first";
      var source = new ObservableCollection<DeckEditorMTGCard>()
      {
        DeckEditorMTGCardMocker.CreateMTGCardModel(group: string.Empty),
        DeckEditorMTGCardMocker.CreateMTGCardModel(group: groupKey, count: 2),
        DeckEditorMTGCardMocker.CreateMTGCardModel(group: groupKey, count: 4),
        DeckEditorMTGCardMocker.CreateMTGCardModel(group: "second"),
      };

      var viewmodel = new CardGroupViewModel(groupKey, source, new TestMTGCardImporter());
      var expected = source.Where(x => x.Group == groupKey).ToArray();

      CollectionAssert.AreEquivalent(expected, viewmodel.Cards.ToArray());
      Assert.AreEqual(expected.Sum(x => x.Count), viewmodel.Count);
    }

    [TestMethod]
    public void ItemAddedToSource_SameGroup_CardAdded()
    {
      var groupKey = "first";
      var source = new ObservableCollection<DeckEditorMTGCard>();
      var viewmodel = new CardGroupViewModel(groupKey, source, new TestMTGCardImporter());

      var card = DeckEditorMTGCardMocker.CreateMTGCardModel(group: groupKey);
      source.Add(card);

      Assert.IsTrue(viewmodel.Cards.Contains(card));
    }

    [TestMethod]
    public void ItemAddedToSource_DifferentGroup_NoChanges()
    {
      var groupKey = "first";
      var source = new ObservableCollection<DeckEditorMTGCard>();
      var viewmodel = new CardGroupViewModel(groupKey, source, new TestMTGCardImporter());

      var card = DeckEditorMTGCardMocker.CreateMTGCardModel(group: "second");
      source.Add(card);

      Assert.IsFalse(viewmodel.Cards.Contains(card));
    }

    [TestMethod]
    public void ItemRemovedFromSource_SameGroup_CardRemoved()
    {
      var groupKey = "first";
      var card = DeckEditorMTGCardMocker.CreateMTGCardModel(group: groupKey);
      var source = new ObservableCollection<DeckEditorMTGCard>()
      {
        card
      };
      var viewmodel = new CardGroupViewModel(groupKey, source, new TestMTGCardImporter());

      source.Remove(card);

      Assert.IsFalse(viewmodel.Cards.Contains(card));
    }

    [TestMethod]
    public void ItemRemovedFromSource_DifferentGroup_NoChanges()
    {
      var groupKey = "first";
      var card = DeckEditorMTGCardMocker.CreateMTGCardModel(group: "second");
      var source = new ObservableCollection<DeckEditorMTGCard>()
      {
        card
      };
      var viewmodel = new CardGroupViewModel(groupKey, source, new TestMTGCardImporter());

      source.Remove(card);

      Assert.IsFalse(viewmodel.Cards.Contains(card));
    }

    [TestMethod]
    public void SourceItemGroupChanged_ToSameGroup_CardAdded()
    {
      var groupKey = "first";
      var source = new ObservableCollection<DeckEditorMTGCard>();
      var viewmodel = new CardGroupViewModel(groupKey, source, new TestMTGCardImporter());

      var card = DeckEditorMTGCardMocker.CreateMTGCardModel(group: "second");
      source.Add(card);

      card.Group = groupKey;

      Assert.IsTrue(viewmodel.Cards.Contains(card));
    }

    [TestMethod]
    public void SourceItemGroupChanged_ToDifferentGroup_CardAdded()
    {
      var groupKey = "first";
      var source = new ObservableCollection<DeckEditorMTGCard>();
      var viewmodel = new CardGroupViewModel(groupKey, source, new TestMTGCardImporter());

      var card = DeckEditorMTGCardMocker.CreateMTGCardModel(group: groupKey);
      source.Add(card);

      card.Group = "second";

      Assert.IsFalse(viewmodel.Cards.Contains(card));
    }

    [TestMethod]
    public void SourceItemCountChanged_SameGroup_CountChanged()
    {
      var groupKey = "first";
      var card = DeckEditorMTGCardMocker.CreateMTGCardModel(group: groupKey);
      var source = new ObservableCollection<DeckEditorMTGCard>()
      {
        card
      };
      var viewmodel = new CardGroupViewModel(groupKey, source, new TestMTGCardImporter());

      card.Count = 2;

      Assert.AreEqual(card.Count, viewmodel.Count);
    }

    [TestMethod]
    public void SourceItemCountChanged_DifferentGroup_CountNotChanged()
    {
      var groupKey = "first";
      var card = DeckEditorMTGCardMocker.CreateMTGCardModel(group: "second");
      var source = new ObservableCollection<DeckEditorMTGCard>()
      {
        card
      };
      var viewmodel = new CardGroupViewModel(groupKey, source, new TestMTGCardImporter());

      card.Count = 2;

      Assert.AreEqual(0, viewmodel.Count);
    }
  }
}
