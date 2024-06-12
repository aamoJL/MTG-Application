using MTGApplication.General.Models;
using MTGApplication.General.Services.API.CardAPI;
using MTGApplication.General.Services.Importers.CardImporter;
using MTGApplication.General.Services.IOServices;
using System.Text.Json.Nodes;

namespace MTGApplicationTests.TestUtility.API;

public class TestScryfallAPI() : ScryfallAPI
{
  private readonly string _apiSamplePath = Path.Join(FileService.GetAssetDirectoryPath(), "ScryfallAPIDeckSample.json");

  public async Task<CardImportResult.Card[]> GetCardsFromSampleJSON()
  {
    FileService.TryReadTextFromFile(_apiSamplePath, out var data);

    var jsonNode = JsonNode.Parse(data);

    return (await GetCardsFromJsonObject(jsonNode)).ToArray();
  }
}
