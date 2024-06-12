﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTGApplication.Features.DeckEditor;
using MTGApplicationTests.TestUtility.API;
using MTGApplicationTests.TestUtility.Mocker;

namespace MTGApplicationTests.FeatureTests.DeckEditorTests.ViewModel.CommanderViewModelTests;
public partial class CommanderViewModelTests
{
  [TestClass]
  public class RemoveTests
  {
    [TestMethod]
    public async Task Remove_CardIsNull()
    {
      var viewmodel = new CommanderViewModel(new TestMTGCardImporter())
      {
        Card = DeckEditorMTGCardMocker.CreateMTGCardModel()
      };

      await viewmodel.ChangeCommanderCommand.ExecuteAsync(null);

      Assert.IsNull(viewmodel.Card);
    }
  }
}
