﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTGApplication.General.Services.Importers.CardImporter;

namespace MTGApplicationTests.IntegrationTests.APITests.EdhrecAPITests;
public partial class EdhrecAPITests
{
  [TestClass]
  public class GetThemesTests
  {
    [TestMethod]
    public async Task GetThemes_ValidCommander_ReturnsThemes()
    {
      var commander = "Atraxa, Praetors' Voice";

      var result = await EdhrecImporter.GetThemes(commander);

      Assert.IsTrue(result.Length > 1);
      Assert.IsNotNull(result.FirstOrDefault(x => x.Name == "Infect"));
    }

    [TestMethod]
    public async Task GetThemes_InvalidCommander_ReturnsEmpty()
    {
      var commander = string.Empty;

      var result = await EdhrecImporter.GetThemes(commander);

      Assert.AreEqual(0, result.Length);
    }

    [TestMethod]
    public async Task GetThemes_WithPartner_ReturnsCombinedThemes()
    {
      var commander = "The Tenth Doctor";
      var partner = "Rose Tyler";

      var resultWithoutPartner = await EdhrecImporter.GetThemes(commander);
      var resultWithPartner = await EdhrecImporter.GetThemes(commander, partner);

      CollectionAssert.AreNotEqual(
        resultWithoutPartner.Select(x => x.Name).ToArray(),
        resultWithPartner.Select(x => x.Name).ToArray());
    }
  }
}
