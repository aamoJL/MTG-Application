﻿using MTGApplication.Interfaces;
using MTGApplication.Models;
using MTGApplication.Models.DTOs;
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
using static MTGApplication.Services.MTGService;

namespace MTGApplication.API;

/// <summary>
/// Scryfall API calls and helper functions
/// </summary>
public partial class ScryfallAPI : ICardAPI<MTGCard>
{
  #region Statics
  private readonly static string API_URL = "https://api.scryfall.com";
  private readonly static string SET_ICON_URL = "https://svgs.scryfall.io/sets";
  private static string CARDS_URL => $"{API_URL}/cards";
  private static string COLLECTION_URL => $"{CARDS_URL}/collection";

  /// <summary>
  /// How many cards can be fetched in one query using identifiers
  /// </summary>
  private static int MaxFetchIdentifierCount => 75;
  #endregion

  #region ICardAPI implementation
  public string Name => "Scryfall";
  public int PageSize => 175;

  public string GetSearchUri(string searchParams) => string.IsNullOrEmpty(searchParams) ? "" : $"{CARDS_URL}/search?q={searchParams}+game:paper";

  public async Task<Result> FetchCardsWithSearchQuery(string searchParams)
  {
    if (string.IsNullOrEmpty(searchParams)) { return Result.Empty(); }
    return await FetchFromUri(GetSearchUri(searchParams));
  }

  public async Task<Result> FetchFromUri(string pageUri, bool paperOnly = false)
  {
    var rootNode = await FetchScryfallJsonObject(pageUri);
    if (rootNode == null) { return Result.Empty(); }

    List<MTGCard> found = new();

    found.AddRange(await GetCardsFromJsonObject(rootNode, paperOnly));
    var nextPage = rootNode["has_more"]?.GetValue<bool>() is true ? rootNode["next_page"]?.GetValue<string>() : "";
    var totalCount = rootNode["total_cards"]?.GetValue<int>() ?? 0;

    return new Result(found.ToArray(), 0, totalCount, nextPage);
  }

  public async Task<Result> FetchFromString(string importText)
  {
    var card = new Func<MTGCard>(() =>
    {
      // Try to import from JSON
      try
      {
        var card = JsonSerializer.Deserialize<MTGCard>(importText);
        if (string.IsNullOrEmpty(card?.Info.Name))
        { throw new Exception("Card does not have name"); }
        return card;
      }
      catch { return null; }
    })();

    if (card != null)
    {
      return new Result(new[] { card }, 0, 1);
    }
    else
    {
      if (Uri.TryCreate(importText, UriKind.Absolute, out var uri) && uri.Host == "edhrec.com")
      {
        // Imported from EDHREC.com
        importText = uri.Segments[^1]; // Get the last part of the url, which should be the card's name
      }

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
  }

  public async Task<Result> FetchFromDTOs(CardDTO[] dtoArray)
  {
    var identifiers = dtoArray.Select(x => new ScryfallIdentifier(x as MTGCardDTO)).ToArray();
    return await FetchWithIdentifiers(identifiers);
  }
  #endregion

  /// <summary>
  /// Returns <see cref="MTGCard"/> array from the given <paramref name="jsonNode"/>
  /// </summary>
  private async Task<MTGCard[]> GetCardsFromJsonObject(JsonNode jsonNode, bool paperOnly = false)
  {
    var cards = new List<MTGCard>();
    if (jsonNode == null) { return cards.ToArray(); }

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
  private MTGCardInfo? GetCardInfoFromJSON(JsonNode json, bool paperOnly = false)
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
    CardFace? backFace;

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
  private async Task<Result> FetchWithIdentifiers(ScryfallIdentifier[] identifiers)
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

public partial class ScryfallAPI
{
  /// <summary>
  /// Scryfall collection fetch identifier object.
  /// Scryfall documentation: <see href="https://scryfall.com/docs/api/cards/collection"/>
  /// </summary>
  public readonly struct ScryfallIdentifier
  {
    public enum IdentifierSchema
    {
      ID, ILLUSTRATION_ID, NAME, NAME_SET, COLLECTORNUMBER_SET
    }

    public ScryfallIdentifier() { }
    public ScryfallIdentifier(MTGCardDTO card)
    {
      if (card != null)
      {
        ScryfallId = card.ScryfallId;
        Name = card.Name;
        CardCount = card.Count;
        SetCode = card.SetCode;
        CollectorNumber = card.CollectorNumber;
      }
    }

    #region Properties
    public Guid ScryfallId { get; init; } = Guid.Empty;
    public int CardCount { get; init; } = 1;
    public string Name { get; init; } = string.Empty;
    public Guid IllustrationId { get; init; } = Guid.Empty;
    public string SetCode { get; init; } = string.Empty;
    public string CollectorNumber { get; init; } = string.Empty;
    public IdentifierSchema PreferedSchema { get; init; } = IdentifierSchema.ID;
    #endregion

    /// <summary>
    /// Return object that contains the scryfall API identifier variables. This method should only be used for JSON serialization.
    /// </summary>
    public object ToObject()
    {
      switch (PreferedSchema)
      {
        case IdentifierSchema.ID:
          if (ScryfallId != Guid.Empty) { return new { id = ScryfallId }; }
          break;
        case IdentifierSchema.ILLUSTRATION_ID:
          if (ScryfallId != Guid.Empty && IllustrationId != Guid.Empty) { return new { illustration_id = IllustrationId }; }
          break;
        case IdentifierSchema.NAME:
          if (!string.IsNullOrEmpty(Name)) { return new { name = Name }; }
          break;
        case IdentifierSchema.NAME_SET:
          if (!string.IsNullOrEmpty(Name) && !string.IsNullOrEmpty(SetCode)) { return new { name = Name, set = SetCode }; }
          break;
        case IdentifierSchema.COLLECTORNUMBER_SET:
          if (!string.IsNullOrEmpty(CollectorNumber) && !string.IsNullOrEmpty(SetCode)) { return new { set = SetCode, collector_number = CollectorNumber }; }
          break;
      }

      // If prefered schema does not work, select secondary if possible
      // Scryfall Id
      if (ScryfallId != Guid.Empty) { return new { id = ScryfallId }; }
      // Set Code + Collector Number
      else if (!string.IsNullOrEmpty(SetCode) && !string.IsNullOrEmpty(CollectorNumber)) { return new { set = SetCode, collector_number = CollectorNumber }; }
      // Illustration Id
      else if (ScryfallId != Guid.Empty && IllustrationId != Guid.Empty) { return new { illustration_id = IllustrationId }; }
      // Name + Set Code
      else if (!string.IsNullOrEmpty(Name) && !string.IsNullOrEmpty(SetCode)) { return new { name = Name, set = SetCode }; }
      // Name
      else if (!string.IsNullOrEmpty(Name)) { return new { name = Name }; }
      else { return string.Empty; }
    }

    /// <summary>
    /// Returns true, if the identifier applies to the given <paramref name="info"/>
    /// </summary>
    public bool Compare(MTGCardInfo? info)
    {
      if (ScryfallId != Guid.Empty) { return info?.ScryfallId == ScryfallId; }
      else if (!string.IsNullOrEmpty(Name) && !string.IsNullOrEmpty(SetCode)) { return string.Equals(info?.FrontFace.Name, Name, StringComparison.OrdinalIgnoreCase) && string.Equals(info?.SetCode, SetCode); }
      else if (!string.IsNullOrEmpty(Name)) { return string.Equals(info?.FrontFace.Name, Name, StringComparison.OrdinalIgnoreCase); }
      else { return false; }
    }
  }
}