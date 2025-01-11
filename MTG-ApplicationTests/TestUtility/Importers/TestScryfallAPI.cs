using MTGApplication.General.Extensions;
using MTGApplication.General.Services.API.CardAPI;
using MTGApplication.General.Services.Importers.CardImporter;
using System.Text.Json.Nodes;

namespace MTGApplicationTests.TestUtility.Importers;

public class TestScryfallAPI() : ScryfallAPI
{
  private readonly string _apiSamplePath = Path.Join(PathExtensions.GetAssetDirectoryPath(), "ScryfallAPIDeckSample.json");

  public async Task<CardImportResult.Card[]> GetCardsFromSampleJSON()
  {
    var jsonNode = JsonNode.Parse(File.ReadAllText(_apiSamplePath));

    return (await GetCardsFromJsonObject(jsonNode)).ToArray();
  }
}
