using MTGApplication.General.Views.Controls;
using MTGApplicationTests.TestUtility.Mocker;
using System.Collections.ObjectModel;

namespace MTGApplicationTests.UnitTests.General.View.Controls;

[TestClass]
public class FilterableAndSortableCollectionViewTests
{
  [TestMethod]
  public void Init_EmptyView()
  {
    var view = new FilterableAndSortableCollectionView();

    Assert.IsEmpty(view.View);
    Assert.IsNull(view.Filter);
    Assert.IsNull(view.SortComparer);
  }

  [TestMethod]
  public void SetSource_EqualsView()
  {
    var initSource = new List<int> { 1, 2, 3, 4 };
    var view = new FilterableAndSortableCollectionView()
    {
      Source = initSource,
    };

    CollectionAssert.AreEqual(initSource, view.View);

    var changedSource = new List<string> { "1", "2", "3", "4" };
    view.Source = changedSource;

    CollectionAssert.AreEqual(changedSource, view.View);
  }

  [TestMethod]
  public void RemoveFromSource_EqualsView()
  {
    var source = new ObservableCollection<int> { 1, 2, 3, 4 };
    var view = new FilterableAndSortableCollectionView()
    {
      Source = source,
    };

    source.RemoveAt(0);

    CollectionAssert.AreEqual(source, view.View);
  }

  [TestMethod]
  public void AddToSource_EqualsView()
  {
    var source = new ObservableCollection<int> { 1, 2, 3, 4 };
    var view = new FilterableAndSortableCollectionView()
    {
      Source = source,
    };

    source.Add(5);

    CollectionAssert.AreEqual(source, view.View);
  }

  [TestMethod]
  public void SetFilter_ViewFiltered()
  {
    var source = new ObservableCollection<int> { 1, 2, 3, 4 };
    var view = new FilterableAndSortableCollectionView()
    {
      Source = source,
      Filter = (item) => item.Equals(3)
    };

    var expected = new int[] { 3 };

    CollectionAssert.AreEqual(expected, view.View);

    view.Filter = (item) => item.Equals(1);
    expected = [1];

    CollectionAssert.AreEqual(expected, view.View);
  }

  [TestMethod]
  public void SetSortComparer_ViewFiltered()
  {
    var source = new ObservableCollection<int> { 4, 2, 3, 1 };
    var view = new FilterableAndSortableCollectionView()
    {
      Source = source,
      SortComparer = Comparer<object>.Default,
    };

    var expected = new int[] { 1, 2, 3, 4 };

    CollectionAssert.AreEqual(expected, view.View);

    view.SortComparer = Comparer<object>.Create((a, b) =>
    {
      // Reverse comparison
      return Comparer<object>.Default.Compare(a, b) * -1;
    });
    expected = [4, 3, 2, 1];

    CollectionAssert.AreEqual(expected, view.View);
  }

  [TestMethod]
  public void ModifyObservableItem_FilterApplied()
  {
    var source = new ObservableCollection<ObservableTestObject> {
      new(1),
      new(2),
      new(3),
      new(4),
    };
    var view = new FilterableAndSortableCollectionView()
    {
      Source = source,
      Filter = (item) => ((ObservableTestObject)item).Value.Equals(1)
    };

    Assert.HasCount(1, view.View);

    source[1].Value = 1;

    Assert.HasCount(2, view.View);
  }

  [TestMethod]
  public void ModifyObservableItem_SortApplied()
  {
    var source = new ObservableCollection<ObservableTestObject> {
      new(2),
      new(1),
      new(4),
      new(3),
    };
    var view = new FilterableAndSortableCollectionView()
    {
      Source = source,
      SortComparer = Comparer<object>.Create((a, b) =>
      {
        return Comparer<int>.Default.Compare(
          ((ObservableTestObject)a).Value,
          ((ObservableTestObject)b).Value);
      })
    };

    var expected = new int[] { 1, 2, 3, 4 };

    CollectionAssert.AreEqual(expected, view.View.Select(x => (x as ObservableTestObject).Value).ToArray());

    source[1].Value = 5; // was 1
    expected = [2, 3, 4, 5];

    CollectionAssert.AreEqual(expected, view.View.Select(x => (x as ObservableTestObject).Value).ToArray());

    source[2].Value = 1; // was 4
    expected = [1, 2, 3, 5];

    CollectionAssert.AreEqual(expected, view.View.Select(x => (x as ObservableTestObject).Value).ToArray());
  }
}