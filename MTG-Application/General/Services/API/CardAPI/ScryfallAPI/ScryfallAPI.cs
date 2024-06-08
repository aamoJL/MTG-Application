using MTGApplication.General.Models.Card;
using MTGApplication.General.Services.API.CardAPI.UseCases;
using MTGApplication.General.Services.Databases.Repositories.CardRepository.Models;
using MTGApplication.General.Services.IOService;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static MTGApplication.General.Models.Card.MTGCardInfo;

namespace MTGApplication.General.Services.API.CardAPI;

/// <summary>
/// Scryfall API calls and helper functions
/// </summary>
public partial class ScryfallAPI : MTGCardAPI
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

  public override async Task<CardImportResult> FetchCardsWithSearchQuery(string searchParams)
  {
    if (string.IsNullOrEmpty(searchParams)) { return CardImportResult.Empty(); }
    return await FetchFromUri(GetSearchUri(searchParams));
  }

  public override async Task<CardImportResult> FetchFromUri(string pageUri, bool paperOnly = false, bool fetchAll = false)
  {
    var pageResults = new List<CardImportResult>();
    var currentPage = pageUri;

    do
    {
      if (await NetworkService.TryFetchStringFromUrlGetAsync(currentPage) is not string data)
        break;

      if (!JsonService.TryParseJson(data, out var rootNode))
        break;

      List<CardImportResult.Card> found = [.. await GetCardsFromJsonObject(rootNode, paperOnly)];
      var nextPage = rootNode["has_more"]?.GetValue<bool>() is true ? rootNode["next_page"]?.GetValue<string>() : "";
      var totalCount = rootNode["total_cards"]?.GetValue<int>() ?? 0;
      pageResults.Add(new CardImportResult([.. found], 0, totalCount, CardImportResult.ImportSource.External, nextPage));

      currentPage = nextPage;
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

  public override async Task<CardImportResult> FetchFromString(string importText)
  {
    if (string.IsNullOrEmpty(importText)) return CardImportResult.Empty(CardImportResult.ImportSource.External);

    var lines = importText.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

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
          Name = nameMatch.Trim(),
          CardCount = !string.IsNullOrEmpty(countMatch) ? int.Parse(countMatch) : 1,
        };
      }
    })));

    return await FetchWithIdentifiers(identifiers);
  }

  public async Task<CardImportResult> FetchFromDTOs(MTGCardDTO[] dtoArray)
  {
    var identifiers = dtoArray.Select(x => new ScryfallIdentifier(x)).ToArray();
    return await FetchWithIdentifiers(identifiers);
  }

  /// <summary>
  /// Returns <see cref="CardImportResult.Card"/> array from the given <paramref name="jsonNode"/>
  /// </summary>
  protected async Task<IEnumerable<CardImportResult.Card>> GetCardsFromJsonObject(JsonNode jsonNode, bool paperOnly = false)
  {
    if (jsonNode == null) { return []; }

    var cards = new List<CardImportResult.Card>();

    if (jsonNode?["data"] is JsonNode dataNode)
      foreach (var itemInfo in await Task.WhenAll(dataNode.AsArray().Select(x => Task.Run(() => GetCardInfoFromJSON(x, paperOnly)))))
        if (itemInfo != null)
          cards.Add(new(itemInfo));

    return cards;
  }

  /// <summary>
  /// Converts Scryfall API Json object to <see cref="MTGCardInfo"/> object
  /// </summary>
  private MTGCardInfo GetCardInfoFromJSON(JsonNode json, bool paperOnly = false)
  {
    /// <summary>
    /// Converts the <paramref name="colorArray"/> to <see cref="ColorTypes"/> array
    /// </summary>
    static ColorTypes[] GetColors(string[] colorArray)
    {
      var colors = new List<ColorTypes>();

      foreach (var color in colorArray)
        if (Enum.TryParse(color, true, out ColorTypes colorType))
          colors.Add(colorType);

      return [.. colors];
    }

    if (json == null || json["object"]?.GetValue<string>() != "card") { return null; }
    if (paperOnly && json["games"]?.AsArray().FirstOrDefault(x => x.GetValue<string>() == "paper") is null) { return null; }

    // https://scryfall.com/docs/api/cards
    var scryfallId = json["id"].GetValue<Guid>();
    var cmc = (int)(json["cmc"]?.GetValue<float>() ?? json["card_faces"].AsArray()[0]["cmc"].GetValue<float>());
    var name = json["name"].GetValue<string>();
    var typeLine = json["type_line"]?.GetValue<string>() ?? json["card_faces"].AsArray()[0]["type_line"].GetValue<string>();
    var setCode = json["set"].GetValue<string>();
    var setName = json["set_name"].GetValue<string>();
    var collectorNumber = json["collector_number"].GetValue<string>();
    _ = float.TryParse(json["prices"]?["eur"]?.GetValue<string>(), NumberStyles.Float, CultureInfo.InvariantCulture, out var price);
    var apiWebsiteUri = json["scryfall_uri"]?.GetValue<string>();
    var setIconUri = $"{SET_ICON_URL}/{setCode}.svg";
    var printSearchUri = json["prints_search_uri"]?.GetValue<string>();
    var cardMarketUri = json["purchase_uris"]?["cardmarket"]?.GetValue<string>();

    CardFace frontFace;
    CardFace backFace;

    frontFace = new CardFace(
      colors: json["colors"] != null ? GetColors(json["colors"]!.AsArray().Select(x => x.GetValue<string>()).ToArray())
        : GetColors(json["card_faces"]?.AsArray()[0]["colors"]?.AsArray().Select(x => x.GetValue<string>()).ToArray()),
      name: json["card_faces"]?.AsArray()[0]["name"]?.GetValue<string>() ?? json["name"]?.GetValue<string>(),
      imageUri: json["card_faces"]?.AsArray()[0]["image_uris"]?["normal"]?.GetValue<string>() ?? json["image_uris"]?["normal"]?.GetValue<string>(),
      illustrationId: json["card_faces"]?.AsArray()[0]["illustration_id"]?.GetValue<Guid?>() ?? json["illustration_id"]?.GetValue<Guid?>(),
      oracleText: json["card_faces"]?.AsArray()[0]["oracle_text"]?.GetValue<string>() ?? json["oracle_text"]?.GetValue<string>() ?? string.Empty
      );

    if (json["card_faces"] != null)
    {
      backFace = new CardFace(
        colors: json["card_faces"]!.AsArray()[1]["colors"] != null ? GetColors(json["card_faces"]?.AsArray()[1]["colors"]!.AsArray().Select(x => x.GetValue<string>()).ToArray())
          : [],
        name: json["card_faces"]?.AsArray()[1]["name"]?.GetValue<string>() ?? string.Empty,
        imageUri: json["card_faces"]?.AsArray()[1]["image_uris"]?["normal"]?.GetValue<string>() ?? null,
        illustrationId: json["card_faces"]?.AsArray()[1]["illustration_id"]?.GetValue<Guid?>() ?? null,
        oracleText: json["card_faces"]?.AsArray()[0]["oracle_text"]?.GetValue<string>() ?? string.Empty
        );
    }
    else { backFace = null; }

    var rarityString = json["rarity"].GetValue<string>();
    var rarityType = RarityTypes.Common;
    if (Enum.TryParse(rarityString, true, out RarityTypes type)) { rarityType = type; }

    var producedManaStringArray = json["produced_mana"]?.AsArray().Select(x => x.GetValue<string>()).ToArray();
    var producedManaList = new List<ColorTypes>();
    if (producedManaStringArray != null)
    {
      foreach (var colorString in producedManaStringArray)
      {
        if (Enum.TryParse(colorString, true, out ColorTypes color))
        { producedManaList.Add(color); }
      }
    }

    var tokens = json["all_parts"]?.AsArray().Where(x => x["component"]?.GetValue<string>() == "token").Select(x => new CardToken(x["id"]!.GetValue<Guid>())).ToArray();
    var oracle = json["oracle_id"]?.GetValue<Guid>() ?? json["card_faces"]?.AsArray()[0]["oracle_id"]?.GetValue<Guid>() ?? Guid.Empty;

    return new MTGCardInfo(
      scryfallId: scryfallId,
      frontFace: frontFace,
      backFace: backFace,
      cmc: cmc,
      name: name,
      typeLine: typeLine,
      setCode: setCode,
      setName: setName,
      price: price,
      collectorNumber: collectorNumber,
      apiWebsiteUri: apiWebsiteUri,
      setIconUri: setIconUri,
      rarityType: rarityType,
      producedMana: producedManaList.ToArray(),
      printSearchUri: printSearchUri,
      cardMarketUri: cardMarketUri,
      tokens: tokens ?? Array.Empty<CardToken>(),
      apiName: Name,
      oracleId: oracle);
  }

  /// <summary>
  /// Fetches MTGCards from the API using the given identifier objects
  /// </summary>
  private async Task<CardImportResult> FetchWithIdentifiers(ScryfallIdentifier[] identifiers)
  {
    var fetchResults = await Task.WhenAll(identifiers.Chunk(MaxFetchIdentifierCount).Select(chunk => Task.Run(async () =>
    {
      var identifiersJson = new ScryfallIdentifiersToJsonConverter().Execute(chunk);

      var fetchedCards = new List<CardImportResult.Card>();
      var notFoundCount = 0;

      // Fetch and covert the JSON to card objects
      if (JsonService.TryParseJson(await NetworkService.TryFetchStringFromUrlPostAsync(COLLECTION_URL, identifiersJson), out var rootNode))
      {
        notFoundCount += rootNode?["not_found"]?.AsArray().Count ?? 0;

        if (rootNode?["data"]?.AsArray() is JsonArray dataArray)
        {
          foreach (var item in dataArray)
          {
            if (GetCardInfoFromJSON(item) is MTGCardInfo cardInfo)
            {
              var count = chunk.FirstOrDefault(x => x.Compare(cardInfo)).CardCount;
              fetchedCards.Add(new(cardInfo, count));
            }
          }
        }
      }

      return (Found: fetchedCards, NotFoundCount: notFoundCount);
    })));

    var found = fetchResults.SelectMany(x => x.Found).ToArray();
    var notFoundCount = fetchResults.Sum(x => x.NotFoundCount);
    var totalCount = found.Length;

    return new(found, notFoundCount, totalCount, CardImportResult.ImportSource.External);
  }

  private static string GetSearchUri(string searchParams) => string.IsNullOrEmpty(searchParams) ? "" : $"{CARDS_URL}/search?q={searchParams}+game:paper";
}