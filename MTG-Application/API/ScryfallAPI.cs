using MTGApplication.Interfaces;
using MTGApplication.Models;
using MTGApplication.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static MTGApplication.Interfaces.ICardAPI<MTGApplication.Models.MTGCard>;
using static MTGApplication.Models.MTGCard;

namespace MTGApplication.API;

/// <summary>
/// Scryfall API calls and helper functions
/// </summary>
public class ScryfallAPI : ICardAPI<MTGCard>
{
  /// <summary>
  /// Scryfall collection fetch identifier object.
  /// Scryfall documentation: <see href="https://scryfall.com/docs/api/cards/collection"/>
  /// </summary>
  public readonly struct ScryfallIdentifier
  {
    public enum IdentifierSchema
    {
      ID, ILLUSTRATION_ID, NAME, NAME_SET
    }

    public ScryfallIdentifier() { }
    public ScryfallIdentifier(MTGCardDTO card)
    {
      if (card != null)
      {
        ScryfallId = card.ScryfallId;
        Name = card.Name;
        CardCount = card.Count;
      }
    }

    public Guid ScryfallId { get; init; } = Guid.Empty;
    public int CardCount { get; init; } = 1;
    public string Name { get; init; } = string.Empty;
    public Guid IllustrationId { get; init; } = Guid.Empty;
    public string SetCode { get; init; } = string.Empty;
    public IdentifierSchema PreferedSchema { get; init; } = IdentifierSchema.ID;

    /// <summary>
    /// Return object that contains the scryfall API identifier variables. This method should only be used for JSON serialization.
    /// </summary>
    public object ToObject()
    {
      switch (PreferedSchema)
      {
        case IdentifierSchema.ID:
          if (ScryfallId != Guid.Empty)
          { return new { id = ScryfallId }; }
          break;
        case IdentifierSchema.ILLUSTRATION_ID:
          if (ScryfallId != Guid.Empty && IllustrationId != Guid.Empty)
          { return new { illustration_id = IllustrationId }; }
          break;
        case IdentifierSchema.NAME:
          if (Name != string.Empty)
          { return new { name = Name }; }
          break;
        case IdentifierSchema.NAME_SET:
          if (Name != string.Empty && SetCode != string.Empty)
          { return new { name = Name, set = SetCode }; }
          break;
        default:
          break;
      }

      // If prefered schema does not work, select secondary if possible
      if (ScryfallId != Guid.Empty)
      { return new { id = ScryfallId }; }
      else if (ScryfallId != Guid.Empty && IllustrationId != Guid.Empty)
      { return new { illustration_id = IllustrationId }; }
      else if (Name != string.Empty && SetCode != string.Empty)
      { return new { name = Name, set = SetCode }; }
      else if (Name != string.Empty)
      { return new { name = Name }; }
      else
      { return string.Empty; }
    }

    public bool Compare(MTGCardInfo? info)
    {
      if (ScryfallId != Guid.Empty)
      { return info?.ScryfallId == ScryfallId; }
      else if (Name != string.Empty && SetCode != string.Empty)
      { return string.Equals(info?.FrontFace.Name, Name, StringComparison.OrdinalIgnoreCase) && string.Equals(info?.SetCode, SetCode); }
      else if (Name != string.Empty)
      { return string.Equals(info?.FrontFace.Name, Name, StringComparison.OrdinalIgnoreCase); }
      else
      { return false; }
    }
  }

  public readonly static string APIName = "Scryfall";

  private readonly static string API_URL = "https://api.scryfall.com";
  private static string CARDS_URL => $"{API_URL}/cards";
  private static string COLLECTION_URL => $"{CARDS_URL}/collection";
  private readonly static string SET_ICON_URL = "https://svgs.scryfall.io/sets";

  /// <summary>
  /// How many cards can be fetched in one query using identifiers
  /// </summary>
  private static int MaxFetchIdentifierCount => 75;

  #region ICardAPI interface
  public int PageSize => 175;
  
  public string GetSearchUri(string searchParams) => string.IsNullOrEmpty(searchParams) ? "" : $"{CARDS_URL}/search?q={searchParams}+game:paper";

  public async Task<Result> FetchCardsWithParameters(string searchParams)
  {
    if (string.IsNullOrEmpty(searchParams))
    { return Result.Empty(); }
    return await FetchFromUri(GetSearchUri(searchParams));
  }

  public async Task<Result> FetchFromUri(string pageUri, bool paperOnly = false)
  {
    var rootNode = await FetchScryfallJsonObject(pageUri);
    if (rootNode == null)
    { return Result.Empty(); }

    List<MTGCard> found = new();

    found.AddRange(await GetCardsFromJsonObject(rootNode, paperOnly));
    var nextPage = rootNode["has_more"]?.GetValue<bool>() is true ? rootNode["next_page"]?.GetValue<string>() : "";
    var totalCount = rootNode["total_cards"]?.GetValue<int>() ?? 0;

    return new Result(found.ToArray(), 0, totalCount, nextPage);
  }

  public async Task<Result> FetchFromString(string importText)
  {
    var lines = importText.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

    // Convert each line to scryfall identifier objects
    var identifiers = await Task.WhenAll(lines.Select(line => Task.Run(() =>
    {
      // Format: {Count (optional)} {Name} OR {Count (optional)} {Scryfall Id}
      // Stops at '/' so only the first name will be used for multiface cards
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

  public async Task<Result> FetchFromDTOs(CardDTO[] dtoArray)
  {
    var identifiers = dtoArray.Select(x => new ScryfallIdentifier(x as MTGCardDTO));
    return await FetchWithIdentifiers(identifiers.ToArray());
  }
  #endregion

  /// <summary>
  /// Returns <see cref="MTGCard"/> array from the given <paramref name="jsonNode"/>
  /// </summary>
  private static async Task<MTGCard[]> GetCardsFromJsonObject(JsonNode jsonNode, bool paperOnly = false)
  {
    var cards = new List<MTGCard>();
    if (jsonNode == null)
    { return cards.ToArray(); }

    var dataNode = jsonNode?["data"];
    if (dataNode != null)
    {
      var infos = await Task.WhenAll(dataNode.AsArray().Select(x => Task.Run(() => GetCardInfoFromJSON(x, paperOnly))));

      foreach (var itemInfo in infos)
      {
        if (itemInfo != null)
        {
          var info = (MTGCardInfo)itemInfo;
          cards.Add(new(info));
        }
      }
    }

    return cards.ToArray();
  }

  /// <summary>
  /// Fetches json object that contains list of MTG cards using the Scryfall API
  /// </summary>
  private static async Task<JsonNode> FetchScryfallJsonObject(string searchUri)
  {
    try
    {
      return JsonNode.Parse(await IOService.FetchStringFromURL(searchUri));
    }
    catch (Exception) { return null; }
  }

  /// <summary>
  /// Converts Scryfall API Json object to <see cref="MTGCardInfo"/> object
  /// </summary>
  private static MTGCardInfo? GetCardInfoFromJSON(JsonNode json, bool paperOnly = false)
  {
    /// <summary>
    /// Converts the <paramref name="colorArray"/> to <see cref="ColorTypes"/> array
    /// </summary>
    static ColorTypes[] GetColors(string[] colorArray)
    {
      var colors = new List<ColorTypes>();

      foreach (var color in colorArray)
      {
        if (Enum.TryParse(color, true, out ColorTypes colorType))
        {
          colors.Add(colorType);
        }
      }

      return colors.ToArray();
    }

    if (json == null || json["object"]?.GetValue<string>() != "card")
    { return null; }
    if (paperOnly && json["games"]?.AsArray().FirstOrDefault(x => x.GetValue<string>() == "paper") is null)
    { return null; }

    // https://scryfall.com/docs/api/cards
    var scryfallId = json["id"].GetValue<Guid>();
    var cmc = (int)json["cmc"].GetValue<float>();
    var name = json["name"].GetValue<string>();
    var typeLine = json["type_line"].GetValue<string>();
    var setCode = json["set"].GetValue<string>();
    var setName = json["set_name"].GetValue<string>();
    var collectorNumber = json["collector_number"].GetValue<string>();
    _ = float.TryParse(json["prices"]?["eur"]?.GetValue<string>(), NumberStyles.Float, CultureInfo.InvariantCulture, out var price);
    var apiWebsiteUri = json["scryfall_uri"]?.GetValue<string>();
    var setIconUri = $"{SET_ICON_URL}/{setCode}.svg";
    var printSearchUri = json["prints_search_uri"]?.GetValue<string>();
    var cardMarketUri = json["purchase_uris"]?["cardmarket"]?.GetValue<string>();

    CardFace frontFace;
    CardFace? backFace;

    frontFace = new CardFace(
      colors: json["colors"] != null ? GetColors(json["colors"]!.AsArray().Select(x => x.GetValue<string>()).ToArray()) 
        : GetColors(json["card_faces"]?.AsArray()[0]["colors"]?.AsArray().Select(x => x.GetValue<string>()).ToArray()),
      name: json["card_faces"]?.AsArray()[0]["name"]?.GetValue<string>() ?? json["name"]?.GetValue<string>(),
      imageUri: json["card_faces"]?.AsArray()[0]["image_uris"]?["normal"]?.GetValue<string>() ?? json["image_uris"]?["normal"]?.GetValue<string>(),
      illustrationId: json["card_faces"]?.AsArray()[0]["illustration_id"]?.GetValue<Guid?>() ?? json["illustration_id"]?.GetValue<Guid?>(),
      oracleText: json["card_faces"]?.AsArray()[0]["oracle_text"]?.GetValue<string>() ?? json["oracle_text"]?.GetValue<string>() ?? string.Empty
      );

    if(json["card_faces"] != null)
    {
      backFace = new CardFace(
        colors: json["card_faces"]!.AsArray()[1]["colors"] != null ? GetColors(json["card_faces"]?.AsArray()[1]["colors"]!.AsArray().Select(x => x.GetValue<string>()).ToArray())
          : Array.Empty<ColorTypes>(),
        name: json["card_faces"]?.AsArray()[1]["name"]?.GetValue<string>() ?? string.Empty,
        imageUri: json["card_faces"]?.AsArray()[1]["image_uris"]?["normal"]?.GetValue<string>() ?? null,
        illustrationId: json["card_faces"]?.AsArray()[1]["illustration_id"]?.GetValue<Guid?>() ?? null,
        oracleText: json["card_faces"]?.AsArray()[0]["oracle_text"]?.GetValue<string>() ?? string.Empty
        );
    }
    else { backFace = null; }

    var rarityString = json["rarity"].GetValue<string>();
    var rarityType = RarityTypes.Common;
    if (Enum.TryParse(rarityString, true, out RarityTypes type))
    { rarityType = type; }

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
      tokens: tokens ?? Array.Empty<CardToken>());
  }

  /// <summary>
  /// Fetches MTGCards from the API using the given identifier objects
  /// </summary>
  private static async Task<Result> FetchWithIdentifiers(ScryfallIdentifier[] identifiers)
  {
    var fetchResults = await Task.WhenAll(identifiers.Chunk(MaxFetchIdentifierCount).Select(chunk => Task.Run(async () =>
    {
      var identifiersJson = JsonSerializer.Serialize(new
      {
        identifiers = chunk.Select(x => x.ToObject())
      });

      List<MTGCard> fetchedCards = new();
      var notFoundCount = 0;

      // Fetch and covert the JSON to card objects
      try
      {
        var rootNode = JsonNode.Parse(await IOService.FetchStringFromURLPost(COLLECTION_URL, identifiersJson));
        var dataNode = rootNode?["data"];
        notFoundCount += rootNode?["not_found"]?.AsArray().Count ?? 0;
        if (dataNode != null)
        {
          foreach (var item in dataNode.AsArray())
          {
            var cardInfo = GetCardInfoFromJSON(item);
            if (cardInfo != null)
            {
              var count = chunk.FirstOrDefault(x => x.Compare(cardInfo)).CardCount;
              fetchedCards.Add(new((MTGCardInfo)cardInfo, count));
            }
          }
        }
      }
      catch (Exception) { throw; }

      return (Found: fetchedCards, NotFoundCount: notFoundCount);
    })));

    var found = fetchResults.SelectMany(x => x.Found).ToArray();
    var notFoundCount = fetchResults.Sum(x => x.NotFoundCount);
    var totalCount = found.Length;

    return new(found, notFoundCount, totalCount);
  }
}