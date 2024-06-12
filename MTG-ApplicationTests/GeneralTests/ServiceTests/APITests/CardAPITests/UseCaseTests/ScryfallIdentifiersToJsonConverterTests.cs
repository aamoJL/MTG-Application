using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTGApplication.General.Services.Importers.CardImporter.UseCases;
using System.Text.Json;
using static MTGApplication.General.Services.API.CardAPI.ScryfallAPI;

namespace MTGApplicationTests.GeneralTests.Services.APITests.CardAPITests.UseCaseTests;

[TestClass]
public class ScryfallIdentifiersToJsonConverterTests
{
  [TestMethod]
  public void Execute_ID()
  {
    var scryfallId = Guid.NewGuid();
    var illustrationId = Guid.NewGuid();
    var identifier = new ScryfallIdentifier()
    {
      ScryfallId = scryfallId,
      CardCount = 5,
      Name = "Test",
      SetCode = "tst",
      IllustrationId = illustrationId,
      PreferedSchema = ScryfallIdentifier.IdentifierSchema.ID
    };

    var expected = JsonSerializer.Serialize(new { identifiers = new[] { new { id = scryfallId } } });
    var result = new ScryfallIdentifiersToJsonConverter().Execute([identifier]);

    Assert.AreEqual(expected, result);
  }

  [TestMethod]
  public void Execute_ILLUSTRATION_ID()
  {
    var scryfallId = Guid.NewGuid();
    var illustrationId = Guid.NewGuid();
    var identifier = new ScryfallIdentifier()
    {
      ScryfallId = scryfallId,
      CardCount = 5,
      Name = "Test",
      SetCode = "tst",
      IllustrationId = illustrationId,
      PreferedSchema = ScryfallIdentifier.IdentifierSchema.ILLUSTRATION_ID
    };

    var expected = JsonSerializer.Serialize(new { identifiers = new[] { new { illustration_id = illustrationId } } });
    var result = new ScryfallIdentifiersToJsonConverter().Execute([identifier]);

    Assert.AreEqual(expected, result);
  }

  [TestMethod]
  public void Execute_SET_NAME()
  {
    var scryfallId = Guid.NewGuid();
    var illustrationId = Guid.NewGuid();
    var identifier = new ScryfallIdentifier()
    {
      ScryfallId = scryfallId,
      CardCount = 5,
      Name = "Test",
      SetCode = "tst",
      IllustrationId = illustrationId,
      PreferedSchema = ScryfallIdentifier.IdentifierSchema.NAME_SET
    };

    var expected = JsonSerializer.Serialize(new { identifiers = new[] { new { name = identifier.Name, set = identifier.SetCode } } });
    var result = new ScryfallIdentifiersToJsonConverter().Execute([identifier]);

    Assert.AreEqual(expected, result);
  }

  [TestMethod]
  public void Execute_NAME()
  {
    var scryfallId = Guid.NewGuid();
    var illustrationId = Guid.NewGuid();
    var identifier = new ScryfallIdentifier()
    {
      ScryfallId = scryfallId,
      CardCount = 5,
      Name = "Test",
      SetCode = "tst",
      IllustrationId = illustrationId,
      PreferedSchema = ScryfallIdentifier.IdentifierSchema.NAME
    };

    var expected = JsonSerializer.Serialize(new { identifiers = new[] { new { name = identifier.Name } } });
    var result = new ScryfallIdentifiersToJsonConverter().Execute([identifier]);

    Assert.AreEqual(expected, result);
  }
}
