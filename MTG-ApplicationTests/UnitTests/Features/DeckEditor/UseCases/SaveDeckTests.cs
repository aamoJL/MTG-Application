using MTGApplication.Features.DeckEditor.UseCases;
using MTGApplication.General.Services.Databases.Repositories.DeckRepository.Models;
using MTGApplicationTests.TestUtility.Database;

namespace MTGApplicationTests.UnitTests.Features.DeckEditor.UseCases;

[TestClass]
public class SaveDeckTests
{
  [TestMethod]
  public async Task Save_OldName_Saved()
  {
    var repo = new TestRepository<MTGCardDeckDTO>()
    {
      AddResult = _ => Task.FromResult(true),
    };
    var result = await new SaveDeck(repo).Execute(new() { Name = "Deck" }, "Deck", overrideOld: false);

    Assert.IsTrue(result);
  }

  [TestMethod]
  public async Task Save_NewName_Saved()
  {
    var repo = new TestRepository<MTGCardDeckDTO>()
    {
      AddResult = _ => Task.FromResult(true),
      ExistsResult = _ => Task.FromResult(false),
    };
    var result = await new SaveDeck(repo).Execute(new() { Name = string.Empty }, "Deck", overrideOld: false);

    Assert.IsTrue(result);
  }

  [TestMethod]
  public async Task Save_Override_Saved()
  {
    var repo = new TestRepository<MTGCardDeckDTO>()
    {
      AddResult = _ => Task.FromResult(true),
      ExistsResult = _ => Task.FromResult(true),
    };
    var result = await new SaveDeck(repo).Execute(new() { Name = string.Empty }, "Deck", overrideOld: true);

    Assert.IsTrue(result);
  }

  [TestMethod]
  public async Task Save_DeclineOverride_NotSaved()
  {
    var repo = new TestRepository<MTGCardDeckDTO>()
    {
      AddResult = _ => Task.FromResult(true),
      ExistsResult = _ => Task.FromResult(true),
    };
    var result = await new SaveDeck(repo).Execute(new() { Name = string.Empty }, "Deck", overrideOld: false);

    Assert.IsFalse(result);
  }

  [TestMethod]
  public async Task Save_Rename_Saved()
  {
    var repo = new TestRepository<MTGCardDeckDTO>()
    {
      AddResult = _ => Task.FromResult(true),
      ExistsResult = _ => Task.FromResult(false),
    };
    var result = await new SaveDeck(repo).Execute(new() { Name = "Old" }, "New", overrideOld: false);

    Assert.IsTrue(result);
  }

  [TestMethod]
  public async Task Save_Rename_DeclineOverride_NotSaved()
  {
    var repo = new TestRepository<MTGCardDeckDTO>()
    {
      AddResult = _ => Task.FromResult(true),
      ExistsResult = _ => Task.FromResult(true),
    };
    var result = await new SaveDeck(repo).Execute(new() { Name = "Old" }, "New", overrideOld: false);

    Assert.IsFalse(result);
  }

  [TestMethod]
  public async Task Save_Rename_Override_Saved()
  {
    var repo = new TestRepository<MTGCardDeckDTO>()
    {
      AddResult = _ => Task.FromResult(true),
      ExistsResult = _ => Task.FromResult(true),
      GetResult = _ => Task.FromResult<MTGCardDeckDTO?>(new("Old")),
      DeleteResult = _ => Task.FromResult(true)
    };
    var result = await new SaveDeck(repo).Execute(new() { Name = "Old" }, "New", overrideOld: true);

    Assert.IsTrue(result);
  }

  [TestMethod]
  public async Task Save_Rename_OldDeleted()
  {
    var deleted = false;
    var repo = new TestRepository<MTGCardDeckDTO>()
    {
      AddResult = _ => Task.FromResult(true),
      ExistsResult = name => Task.FromResult(name == "Old"),
      GetResult = _ => Task.FromResult<MTGCardDeckDTO?>(new("Old")),
      DeleteResult = async _ =>
      {
        deleted = true;
        return await Task.FromResult(true);
      }
    };
    var result = await new SaveDeck(repo).Execute(new() { Name = "Old" }, "New", overrideOld: false);

    Assert.IsTrue(deleted);
  }
}