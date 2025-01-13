using MTGApplication.General.Models;
using MTGApplication.General.Services.Databases.Repositories.CardRepository.Models;
using MTGApplication.General.Services.Importers.CardImporter;
using MTGApplication.General.Services.Importers.CardImporter.UseCases;
using MTGApplication.General.Services.IOServices;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static MTGApplication.General.Models.MTGCardInfo;

namespace MTGApplication.General.Services.API.CardAPI;

/// <summary>
/// Scryfall API calls and helper functions
/// </summary>
public partial class ScryfallAPI : MTGCardImporter
{
  private readonly static string API_URL = "https://api.scryfall.com";
  private readonly static string SET_ICON_URL = "https://svgs.scryfall.io/sets";
  private static string CARDS_URL => $"{API_URL}/cards";
  private static string COLLECTION_URL => $"{CARDS_URL}/collection";

  public readonly static string API_REFERENCE_URL = "https://scryfall.com/docs/syntax";

  /// <summary>
  /// How many cards can be fetched in one query using identifiers
  /// </summary>
  private static int MaxFetchIdentifierCount => 75;

  public override string Name => "Scryfall";
  public override int PageSize => 175;

  public override async Task<CardImportResult> ImportCardsWithSearchQuery(string searchParams, bool pagination = true)
  {
    if (string.IsNullOrEmpty(searchParams))
      return CardImportResult.Empty();

    try
    {
      return await ImportWithUri(GetSearchUri(searchParams), fetchAll: !pagination);
    }
    catch { throw; }
  }

  public override async Task<CardImportResult> ImportWithUri(string pageUri, bool paperOnly = false, bool fetchAll = false)
  {
    var pageResults = new List<CardImportResult>();
    var currentPage = pageUri;

    do
    {
      try
      {
        if (await NetworkService.GetJsonFromUrl(currentPage) is not string data || string.IsNullOrEmpty(data))
          break;

        if (JsonNode.Parse(data) is not JsonNode rootNode)
          break;

        List<CardImportResult.Card> found = [.. await GetCardsFromJsonObject(rootNode, paperOnly)];
        var nextPage = rootNode["has_more"]?.GetValue<bool>() is true ? rootNode["next_page"]?.GetValue<string>() ?? "" : "";
        var totalCount = rootNode["total_cards"]?.GetValue<int>() ?? found.Count;
        pageResults.Add(new CardImportResult([.. found], 0, totalCount, CardImportResult.ImportSource.External, nextPage));

        currentPage = nextPage;
      }
      catch { throw; }
    } while (fetchAll && !string.IsNullOrEmpty(pageResults.LastOrDefault()?.NextPageUri));

    return pageResults.Count switch
    {
      0 => CardImportResult.Empty(),
      1 => pageResults.First(),
      _ => new(
        Found: pageResults.SelectMany(x => x.Found).ToArray(),
        NotFoundCount: pageResults.Sum(x => x.NotFoundCount),
        TotalCount: pageResults.First().TotalCount,
        Source: CardImportResult.ImportSource.External)
    };
  }

  public override async Task<CardImportResult> ImportWithString(string importText)
  {
    if (string.IsNullOrEmpty(importText))
      return CardImportResult.Empty(CardImportResult.ImportSource.External);

    var separator = new[] { '\n', '\r' };
    var lines = importText.Split(separator, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

    // Convert each line to scryfall identifier objects
    var identifiers = await Task.WhenAll(lines.Select(line => Task.Run(() =>
    {
      // Format: {Count (optional)} {Name} OR {Count (optional)} {Scryfall Id}
      // Multiface cards will be formatted as {front} // {back}, so the name search will stop at '/' so only the first name will be used.
      var regexGroups = new Regex("(?:^[\\s]*(?<Count>[0-9]*(?=\\s)){0,1}\\s*(?<Name>[\\s\\S][^/]*))");
      var match = regexGroups.Match(line);

      var countMatch = match.Groups["Count"]?.Value;
      var nameMatch = match.Groups["Name"]?.Value;

      if (Guid.TryParse(nameMatch, out var id))
      {
        return new ScryfallIdentifier()
        {
          ScryfallId = id,
          CardCount = !string.IsNullOrEmpty(countMatch) ? int.Parse(countMatch) : 1,
        };
      }
      else
      {
        return new ScryfallIdentifier()
        {
          Name = nameMatch?.Trim() ?? "",
          CardCount = !string.IsNullOrEmpty(countMatch) ? int.Parse(countMatch) : 1,
        };
      }
    })));

    try
    {
      return await FetchWithIdentifiers(identifiers);
    }
    catch { throw; }
  }

  public async Task<CardImportResult> ImportWithName(string name, bool fuzzy)
  {
    var searchType = fuzzy ? "fuzzy" : "exact";

    return await ImportWithUri($"{CARDS_URL}/named?{searchType}={name.Replace(' ', '+')}");
  }

  public async Task<CardImportResult> ImportWithId(Guid id) => await ImportWithUri($"{CARDS_URL}/{id}");

  public override async Task<CardImportResult> ImportWithDTOs(IEnumerable<MTGCardDTO> dtos)
  {
    try
    {
      var identifiers = dtos.Select(x => new ScryfallIdentifier(x)).ToArray();
      return await FetchWithIdentifiers(identifiers);
    }
    catch { throw; }
  }

  /// <summary>
  /// Returns <see cref="CardImportResult.Card"/> array from the given <paramref name="jsonNode"/>
  /// </summary>
  protected async Task<IEnumerable<CardImportResult.Card>> GetCardsFromJsonObject(JsonNode jsonNode, bool paperOnly = false)
  {
    if (jsonNode == null)
      return [];

    var cards = new List<CardImportResult.Card>();
    var cardNodes = jsonNode["data"]?.AsArray() ?? new JsonArray(jsonNode);

    foreach (var itemInfo in await Task.WhenAll(cardNodes.Select(x => Task.Run(() => GetCardInfoFromJSON(x!, paperOnly)))))
      if (itemInfo != null)
        cards.Add(new(itemInfo));

    return cards;
  }

  /// <summary>
  /// Converts Scryfall API Json object to <see cref="MTGCardInfo"/> object
  /// </summary>
  private MTGCardInfo? GetCardInfoFromJSON(JsonNode json, bool paperOnly = false)
  {
    /// <summary>
    /// Converts the colorArray to <see cref="ColorTypes"/> array
    /// </summary>
    static ColorTypes[] GetColors(string[] colorArray)
    {
      var colors = new List<ColorTypes>();

      foreach (var color in colorArray)
        if (Enum.TryParse(color, true, out ColorTypes colorType))
          colors.Add(colorType);

      return [.. colors];
    }

    if (json == null || json["object"]?.GetValue<string>() != "card")
      return null;
    if (paperOnly && json["games"]?.AsArray().FirstOrDefault(x => x?.GetValue<string>() == "paper") is null)
      return null;

    // https://scryfall.com/docs/api/cards
    var scryfallId = json["id"]?.GetValue<Guid>();
    var cmc = (int?)(json["cmc"]?.GetValue<float>() ?? json["card_faces"]?.AsArray()[0]?["cmc"]?.GetValue<float>());
    var name = json["name"]?.GetValue<string>() ?? string.Empty;
    var typeLine = json["type_line"]?.GetValue<string>() ?? json["card_faces"]?.AsArray()[0]?["type_line"]?.GetValue<string>() ?? string.Empty;
    var setCode = json["set"]?.GetValue<string>() ?? string.Empty;
    var setName = json["set_name"]?.GetValue<string>() ?? string.Empty;
    var collectorNumber = json["collector_number"]?.GetValue<string>() ?? string.Empty;
    _ = float.TryParse(json["prices"]?["eur"]?.GetValue<string>(), NumberStyles.Float, CultureInfo.InvariantCulture, out var price);
    var apiWebsiteUri = json["scryfall_uri"]?.GetValue<string>() ?? string.Empty;
    var setIconUri = $"{SET_ICON_URL}/{setCode}.svg";
    var printSearchUri = json["prints_search_uri"]?.GetValue<string>() ?? string.Empty;
    var cardMarketUri = json["purchase_uris"]?["cardmarket"]?.GetValue<string>() ?? string.Empty;
    var colorIdentity = GetColors(json["color_identity"]?.AsArray().Select(x => x!.GetValue<string>()).ToArray() ?? []);

    CardFace? frontFace = null;
    CardFace? backFace = null;

    frontFace = new CardFace(
      colors: json["colors"] != null ? GetColors(json["colors"]!.AsArray().Select(x => x!.GetValue<string>()).ToArray())
        : GetColors(json["card_faces"]?.AsArray()[0]?["colors"]?.AsArray().Select(x => x!.GetValue<string>()).ToArray() ?? []),
      name: json["card_faces"]?.AsArray()[0]?["name"]?.GetValue<string>() ?? json["name"]?.GetValue<string>() ?? string.Empty,
      imageUri: json["card_faces"]?.AsArray()[0]?["image_uris"]?["normal"]?.GetValue<string>() ?? json["image_uris"]?["normal"]?.GetValue<string>() ?? string.Empty,
      artCropUri: json["card_faces"]?.AsArray()[0]?["image_uris"]?["art_crop"]?.GetValue<string>() ?? json["image_uris"]?["art_crop"]?.GetValue<string>() ?? string.Empty,
      illustrationId: json["card_faces"]?.AsArray()[0]?["illustration_id"]?.GetValue<Guid?>() ?? json["illustration_id"]?.GetValue<Guid?>(),
      oracleText: json["card_faces"]?.AsArray()[0]?["oracle_text"]?.GetValue<string>() ?? json["oracle_text"]?.GetValue<string>() ?? string.Empty
      );

    if (json["card_faces"]?.AsArray() is JsonNode faces)
      backFace = new CardFace(
        colors: GetColors(faces[1]?["colors"]?.AsArray().Select(x => x!.GetValue<string>()).ToArray() ?? []),
        name: faces[1]?["name"]?.GetValue<string>() ?? string.Empty,
        imageUri: faces[1]?["image_uris"]?["normal"]?.GetValue<string>() ?? string.Empty,
        artCropUri: faces[1]?["image_uris"]?["art_crop"]?.GetValue<string>() ?? string.Empty,
        illustrationId: faces[1]?["illustration_id"]?.GetValue<Guid?>() ?? null,
        oracleText: faces[0]?["oracle_text"]?.GetValue<string>() ?? string.Empty);

    var rarityString = json["rarity"]?.GetValue<string>();
    var rarityType = RarityTypes.Common;

    if (Enum.TryParse(rarityString, true, out RarityTypes type))
      rarityType = type;

    var producedManaStringArray = json["produced_mana"]?.AsArray().Select(x => x?.GetValue<string>()).ToArray();
    var producedManaList = new List<ColorTypes>();

    if (producedManaStringArray != null)
      foreach (var colorString in producedManaStringArray)
        if (Enum.TryParse(colorString, true, out ColorTypes color))
          producedManaList.Add(color);

    var tokens = json["all_parts"]?.AsArray().Where(x => x?["component"]?.GetValue<string>() == "token").Select(x => new CardToken(x?["id"]?.GetValue<Guid>() ?? new())).ToArray();
    var oracle = json["oracle_id"]?.GetValue<Guid>() ?? json["card_faces"]?.AsArray()[0]?["oracle_id"]?.GetValue<Guid>() ?? Guid.Empty;

    if (scryfallId == null || cmc == null)
      return null;

    return new MTGCardInfo(
      scryfallId: scryfallId!.Value,
      frontFace: frontFace,
      backFace: backFace,
      cmc: cmc!.Value,
      name: name,
      typeLine: typeLine,
      setCode: setCode,
      setName: setName,
      price: price,
      collectorNumber: collectorNumber,
      apiWebsiteUri: apiWebsiteUri,
      setIconUri: setIconUri,
      rarityType: rarityType,
      producedMana: [.. producedManaList],
      printSearchUri: printSearchUri,
      cardMarketUri: cardMarketUri,
      tokens: tokens ?? [],
      importerName: Name,
      oracleId: oracle,
      colorIdentity: colorIdentity);
  }

  /// <summary>
  /// Fetches MTGCards from the API using the given identifier objects
  /// </summary>
  /// <exception cref="InvalidOperationException"></exception>
  /// <exception cref="System.Net.Http.HttpRequestException"></exception>
  /// <exception cref="UriFormatException"></exception>
  private async Task<CardImportResult> FetchWithIdentifiers(ScryfallIdentifier[] identifiers)
  {
    var fetchResults = await Task.WhenAll(identifiers.Chunk(MaxFetchIdentifierCount).Select(chunk => Task.Run(async () =>
    {
      var identifiersJson = new ScryfallIdentifiersToJsonConverter().Execute(chunk);

      var fetchedCards = new List<CardImportResult.Card>();
      var notFoundCount = 0;

      try
      {
        // Fetch and covert the JSON to card objects
        if (JsonNode.Parse(await NetworkService.PostJsonFromUrl(COLLECTION_URL, identifiersJson)) is JsonNode rootNode)
        {
          notFoundCount += rootNode?["not_found"]?.AsArray().Count ?? 0;

          if (rootNode?["data"]?.AsArray() is JsonArray dataArray)
          {
            foreach (var item in dataArray)
            {
              if (item != null && GetCardInfoFromJSON(item) is MTGCardInfo cardInfo)
              {
                var identifier = chunk.FirstOrDefault(x => x.Compare(cardInfo));
                fetchedCards.Add(new(Info: cardInfo, Count: identifier.CardCount, Group: identifier.CardGroup));
              }
            }
          }
        }

        return (Found: fetchedCards, NotFoundCount: notFoundCount);
      }
      catch { throw; }
    })));

    var found = fetchResults.SelectMany(x => x.Found).ToArray();
    var notFoundCount = fetchResults.Sum(x => x.NotFoundCount);
    var totalCount = found.Length;

    return new(found, notFoundCount, totalCount, CardImportResult.ImportSource.External);
  }

  private static string GetSearchUri(string searchParams) => string.IsNullOrEmpty(searchParams) ? "" : $"{CARDS_URL}/search?q={searchParams}+game:paper";
}