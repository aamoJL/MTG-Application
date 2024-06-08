using MTGApplication.General.Models.Card;
using MTGApplication.General.Services.API.CardAPI;
using MTGApplication.General.Services.IOService;
using System.Text.Json.Nodes;

namespace MTGApplicationTests.TestUtility.API;

public class TestScryfallAPI() : ScryfallAPI
{
  private readonly string _apiSamplePath = Path.Join(FileService.GetAssetDirectoryPath(), "ScryfallAPIDeckSample.json");

  public async Task<DeckEditorMTGCard[]> GetCardsFromSampleJSON()
  {
    FileService.TryReadTextFromFile(_apiSamplePath, out var data);

    var jsonNode = JsonNode.Parse(data);

    return (await GetCardsFromJsonObject(jsonNode)).ToArray();
  }
}
