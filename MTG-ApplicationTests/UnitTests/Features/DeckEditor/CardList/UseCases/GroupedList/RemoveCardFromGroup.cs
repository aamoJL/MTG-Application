using MTGApplication.Features.DeckEditor.Editor.Models;
using MTGApplication.Features.DeckEditor.ViewModels;
using MTGApplicationTests.TestUtility.Importers;
using MTGApplicationTests.TestUtility.Mocker;
using System.Collections.ObjectModel;

namespace MTGApplicationTests.UnitTests.Features.DeckEditor.CardList.UseCases.GroupedList;

[TestClass]
public class RemoveCardFromGroup
{
  [TestMethod]
  public void Remove_CardRemoved()
  {
    var groupKey = "first";
    var card = DeckEditorMTGCardMocker.CreateMTGCardModel(group: groupKey);
    var source = new ObservableCollection<DeckEditorMTGCard>()
      {
        card
      };
    var viewmodel = new CardGroupViewModel(groupKey, source, new TestMTGCardImporter());

    viewmodel.RemoveCardCommand.Execute(card);

    Assert.DoesNotContain(card, viewmodel.Cards);
    Assert.IsEmpty(source);

    viewmodel.UndoStack.Undo();

    Assert.Contains(card, viewmodel.Cards);
    Assert.HasCount(1, source);

    viewmodel.UndoStack.Redo();

    Assert.DoesNotContain(card, viewmodel.Cards);
    Assert.IsEmpty(source);
  }
}
