using MTGApplication.General.Services.Databases.Repositories.DeckRepository.Models;
using MTGApplicationTests.TestUtility.Importers;

namespace MTGApplicationTests.UnitTests.Features.DeckEditor.ViewModels.EditorPage;

[TestClass]
public class Properties
{
  [TestMethod]
  public void Init_DeckName()
  {
    var factory = new TestDeckEditorPageViewModelFactory();
    var vm = factory.Build();

    Assert.AreEqual(string.Empty, vm.DeckName);
  }

  [TestMethod]
  public async Task ChangeDeck_DeckName_Changed()
  {
    var factory = new TestDeckEditorPageViewModelFactory()
    {
      Notifier = new(),
      Repository = new()
      {
        GetAllResult = () => Task.FromResult<IEnumerable<MTGCardDeckDTO>>([new("Deck")]),
        GetResult = _ => Task.FromResult<MTGCardDeckDTO?>(new("Deck")),
      },
      Importer = new()
      {
        Result = TestMTGCardImporter.Success()
      },
    };
    var vm = factory.Build();

    var changed = string.Empty;
    vm.PropertyChanged += (_, e) =>
    {
      if (e.PropertyName == nameof(vm.DeckName)) changed = vm.DeckName;
    };

    await vm.OpenDeckCommand.ExecuteAsync("Deck");

    Assert.AreEqual("Deck", changed);
  }

  [TestMethod]
  public async Task ChangeDeckName_DeckName_Changed()
  {
    var factory = new TestDeckEditorPageViewModelFactory()
    {
      Notifier = new(),
      Repository = new()
      {
        ExistsResult = _ => Task.FromResult(false),
        AddResult = _ => Task.FromResult(true),
      },
      Confirmers = new()
      {
        DeckConfirmers = new()
        {
          ConfirmDeckSave = _ => Task.FromResult<string?>("Deck")
        }
      }
    };
    var vm = factory.Build();

    var changed = string.Empty;
    vm.PropertyChanged += (_, e) =>
    {
      if (e.PropertyName == nameof(vm.DeckName)) changed = vm.DeckName;
    };

    await vm.DeckViewModel.SaveDeckCommand.ExecuteAsync(null);

    Assert.AreEqual("Deck", changed);
  }
}