using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTGApplicationTests.TestUtility.Services;
using MTGApplicationTests.TestUtility.ViewModel.TestInterfaces;

namespace MTGApplicationTests.FeatureTests.CardCollection.CardCollectionViewModelTests;
public partial class CardCollectionViewModelTests
{
  [TestClass]
  public class ExportCardsTests : CardCollectionViewModelTestsBase, ICanExecuteCommandTests
  {
    [TestMethod("Should not be able to execute if a list is not selected")]
    public void InvalidState_CanNotExecute()
    {
      var viewmodel = new Mocker(_dependencies).MockVM();

      Assert.IsFalse(viewmodel.ExportCardsCommand.CanExecute(null));
    }

    [TestMethod("Should be able to execute if a list is selected")]
    public void ValidState_CanExecute()
    {
      var viewmodel = new Mocker(_dependencies) { Collection = _savedCollection }.MockVM();

      viewmodel.SelectedList = viewmodel.Collection.CollectionLists.First();

      Assert.IsTrue(viewmodel.ExportCardsCommand.CanExecute(null));
    }

    [TestMethod]
    public async Task ExportCards_ExportConfirmationShown()
    {
      var viewmodel = new Mocker(_dependencies)
      {
        Collection = _savedCollection,
        Confirmers = new()
        {
          ExportCardsConfirmer = new TestExceptionConfirmer<string, string>()
        }
      }.MockVM();
      viewmodel.SelectedList = viewmodel.Collection.CollectionLists.First();

      await ConfirmationAssert.ConfirmationShown(() => viewmodel.ExportCardsCommand.ExecuteAsync(null));
    }

    [TestMethod]
    public async Task ExportCards_Cancel_NoCopy()
    {
      var clipboard = new TestClipboardService();
      var viewmodel = new Mocker(_dependencies)
      {
        Collection = _savedCollection,
        Confirmers = new()
        {
          ExportCardsConfirmer = new()
          {
            OnConfirm = async msg => await Task.FromResult<string?>(null),
          }
        },
        ClipboardService = clipboard
      }.MockVM();
      viewmodel.SelectedList = viewmodel.Collection.CollectionLists.First();

      await viewmodel.ExportCardsCommand.ExecuteAsync(null);

      Assert.IsNull(clipboard.Content);
    }

    [TestMethod]
    public async Task ExportCards_Success_ContentCopied()
    {
      var exportText = "Export";
      var clipboard = new TestClipboardService();
      var viewmodel = new Mocker(_dependencies)
      {
        Collection = _savedCollection,
        Confirmers = new()
        {
          ExportCardsConfirmer = new()
          {
            OnConfirm = async msg => await Task.FromResult<string?>(exportText),
          }
        },
        ClipboardService = clipboard
      }.MockVM();
      viewmodel.SelectedList = viewmodel.Collection.CollectionLists.First();

      await viewmodel.ExportCardsCommand.ExecuteAsync(null);

      Assert.AreEqual(exportText, clipboard.Content);
    }

    [TestMethod("Export confirmation data should be selected list's cards' ids")]
    public async Task ExportCards_ExportDataIsIds()
    {
      var exportData = string.Empty;
      var clipboard = new TestClipboardService();
      var viewmodel = new Mocker(_dependencies)
      {
        Collection = _savedCollection,
        Confirmers = new()
        {
          ExportCardsConfirmer = new()
          {
            OnConfirm = async msg =>
            {
              exportData = msg.Data;
              return await Task.FromResult<string?>(msg.Data);
            },
          }
        },
        ClipboardService = clipboard
      }.MockVM();
      viewmodel.SelectedList = viewmodel.Collection.CollectionLists.First();

      await viewmodel.ExportCardsCommand.ExecuteAsync(null);

      Assert.AreEqual(
        exportData,
        string.Join(Environment.NewLine, viewmodel.SelectedList.Cards.Select(x => x.Info.ScryfallId)));
    }
  }
}
