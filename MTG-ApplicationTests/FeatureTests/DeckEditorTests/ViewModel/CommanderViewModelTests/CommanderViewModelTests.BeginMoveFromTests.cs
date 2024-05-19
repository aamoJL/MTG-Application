﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTGApplication.Features.DeckEditor;
using MTGApplicationTests.API;
using MTGApplicationTests.Services;

namespace MTGApplicationTests.FeatureTests.DeckEditorTests.ViewModel.CommanderViewModelTests;
public partial class CommanderViewModelTests
{
  [TestClass]
  public class BeginMoveFromTests
  {
    [TestMethod]
    public void BeginMoveFrom_CommandAddedToCombinedCommand()
    {
      var card = Mocker.MTGCardModelMocker.CreateMTGCardModel();
      var viewmodel = new CommanderViewModel(new TestCardAPI());

      viewmodel.BeginMoveFromCommand.Execute(card);

      Assert.AreEqual(1, viewmodel.UndoStack.ActiveCombinedCommand.Commands.Count);
    }
  }
}