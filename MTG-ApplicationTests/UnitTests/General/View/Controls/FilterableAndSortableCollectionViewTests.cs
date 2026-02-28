using CommunityToolkit.Mvvm.ComponentModel;
using MTGApplication.General.Views.Controls;
using MTGApplicationTests.TestUtility.Mocker;
using System.Collections.ObjectModel;

namespace MTGApplicationTests.UnitTests.General.View.Controls;

[TestClass]
public partial class FilterableAndSortableCollectionViewTests
{
  private partial class TestFilter : ObservableObject, IValueFilter<object>
  {
    [ObservableProperty] public partial Predicate<object> ValidationPredicate { get; set; } = _ => true;
  }

  private partial class TestSorter : ObservableObject, IValueSorter<object>
  {
    [ObservableProperty] public partial IComparer<object> Comparer { get; set; } = Comparer<object>.Default;
  }

  [TestMethod]
  public void SetSource_EqualsView()
  {
    var source = new List<object> { 1, 2, 3, 4 };
    var view = new FilterableAndSortableCollectionView<object>(source, new TestFilter(), new TestSorter());

    CollectionAssert.AreEqual(source, view.View);
  }

  [TestMethod]
  public void RemoveFromSource_EqualsView()
  {
    var source = new ObservableCollection<object> { 1, 2, 3, 4 };
    var view = new FilterableAndSortableCollectionView<object>(source, new TestFilter(), new TestSorter());

    source.RemoveAt(0);

    CollectionAssert.AreEqual(source, view.View);
  }

  [TestMethod]
  public void AddToSource_EqualsView()
  {
    var source = new ObservableCollection<object> { 1, 2, 3, 4 };
    var view = new FilterableAndSortableCollectionView<object>(source, new TestFilter(), new TestSorter());

    source.Add(5);

    CollectionAssert.AreEqual(source, view.View);
  }

  [TestMethod]
  public void SetFilter_ViewFiltered()
  {
    var filter = new TestFilter() { ValidationPredicate = item => item.Equals(3) };
    var source = new ObservableCollection<object> { 1, 2, 3, 4 };
    var view = new FilterableAndSortableCollectionView<object>(source, filter, new TestSorter());

    var expected = new int[] { 3 };

    CollectionAssert.AreEqual(expected, view.View);

    filter.ValidationPredicate = (item) => item.Equals(1);
    expected = [1];

    CollectionAssert.AreEqual(expected, view.View);
  }

  [TestMethod]
  public void SetSortComparer_ViewFiltered()
  {
    var sorter = new TestSorter();
    var source = new ObservableCollection<object> { 4, 2, 3, 1 };
    var view = new FilterableAndSortableCollectionView<object>(source, new TestFilter(), sorter);

    var expected = new int[] { 1, 2, 3, 4 };

    CollectionAssert.AreEqual(expected, view.View);

    sorter.Comparer = Comparer<object>.Create((a, b) =>
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
    var source = new ObservableCollection<object> {
      new ObservableTestObject(1),
      new ObservableTestObject(2),
      new ObservableTestObject(3),
      new ObservableTestObject(4),
    };
    var view = new FilterableAndSortableCollectionView<object>(source, new TestFilter() { ValidationPredicate = (item) => ((ObservableTestObject)item).Value.Equals(1) }, new TestSorter());

    Assert.HasCount(1, view.View);

    (source[1] as ObservableTestObject)?.Value = 1;

    Assert.HasCount(2, view.View);
  }

  [TestMethod]
  public void ModifyObservableItem_SortApplied()
  {
    var source = new ObservableCollection<object> {
      new ObservableTestObject(2),
      new ObservableTestObject(1),
      new ObservableTestObject(4),
      new ObservableTestObject(3),
    };
    var view = new FilterableAndSortableCollectionView<object>(source, new TestFilter(), new TestSorter()
    {
      Comparer = Comparer<object>.Create((a, b) =>
    {
      return Comparer<int>.Default.Compare(
        ((ObservableTestObject)a).Value,
        ((ObservableTestObject)b).Value);
    })
    });

    var expected = new int[] { 1, 2, 3, 4 };

    CollectionAssert.AreEqual(expected, view.View.Select(x => (x as ObservableTestObject)?.Value).ToArray());

    (source[1] as ObservableTestObject)?.Value = 5; // was 1
    expected = [2, 3, 4, 5];

    CollectionAssert.AreEqual(expected, view.View.Select(x => (x as ObservableTestObject)?.Value).ToArray());

    (source[2] as ObservableTestObject)?.Value = 1; // was 4
    expected = [1, 2, 3, 5];

    CollectionAssert.AreEqual(expected, view.View.Select(x => (x as ObservableTestObject)?.Value).ToArray());
  }
}