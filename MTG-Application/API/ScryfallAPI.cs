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
using static MTGApplication.Models.MTGCard;

namespace MTGApplication.API
{
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
      public ScryfallIdentifier(CardDTO card)
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
            if (ScryfallId != Guid.Empty) { return new { id = ScryfallId }; }
            break;
          case IdentifierSchema.ILLUSTRATION_ID:
            if (ScryfallId != Guid.Empty && IllustrationId != Guid.Empty) { return new { illustration_id = IllustrationId }; }
            break;
          case IdentifierSchema.NAME:
            if (Name != string.Empty) { return new { name = Name }; }
            break;
          case IdentifierSchema.NAME_SET:
            if (Name != string.Empty && SetCode != string.Empty) { return new { name = Name, set = SetCode }; }
            break;
          default: break;
        }

        // If prefered schema does not work, select secondary if possible
        if (ScryfallId != Guid.Empty) { return new { id = ScryfallId }; }
        else if (ScryfallId != Guid.Empty && IllustrationId != Guid.Empty) { return new { illustration_id = IllustrationId }; }
        else if (Name != string.Empty) { return new { name = Name }; }
        else if (Name != string.Empty && SetCode != string.Empty) { return new { name = Name, set = SetCode }; }
        else { return string.Empty; }
      }
    }

    public readonly static string APIName = "Scryfall";

    private readonly static string API_URL = "https://api.scryfall.com";
    private static string CARDS_URL => $"{API_URL}/cards";
    private static string COLLECTION_URL => $"{CARDS_URL}/collection";
    private readonly static string SET_ICON_URL = "https://svgs.scryfall.io/sets";

    private static int MaxFetchIdentifierCount => 75;

    public int PageSize => 175;

    public string GetSearchUri(string searchParams) => string.IsNullOrEmpty(searchParams) ? "" : $"{CARDS_URL}/search?q={searchParams}+game:paper";
    public async Task<MTGCard[]> FetchCardsWithParameters(string searchParams, int countLimit = 700)
    {
      if (string.IsNullOrEmpty(searchParams)) { return Array.Empty<MTGCard>(); }
      string searchUri = GetSearchUri(searchParams);
      return await FetchCardsFromUri(searchUri, countLimit);
    }
    public async Task<MTGCard[]> FetchCardsFromUri(string uri, int countLimit = int.MaxValue)
    {
      List<MTGCard> cards = new();

      // Loop through API pages
      while (uri != "" && cards.Count < countLimit)
      {
        (var fetchedCards, uri, _) = await FetchCardsFromPage(uri);
        cards.AddRange(fetchedCards);
      }

      return cards.GetRange(0, countLimit).ToArray();
    }
    public async Task<(MTGCard[] Found, int NotFoundCount)> FetchFromString(string importText)
    {
      var lines = importText.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

      // Convert each line to scryfall identifier objects
      var identifiers = await Task.WhenAll(lines.Select(line => Task.Run(() =>
      {
        // Format: {Count (optional)} {Name}
        // Stops at '/' so only the first name will be used for multiface cards
        var regexGroups = new Regex("(?:^[\\s]*(?<Count>[0-9]*(?=\\s)){0,1}\\s*(?<Name>[\\s\\S][^/]*))");
        var match = regexGroups.Match(line);

        var countMatch = match.Groups["Count"]?.Value;
        var nameMatch = match.Groups["Name"]?.Value;

        return new ScryfallIdentifier()
        {
          Name = nameMatch.Trim(),
          CardCount = !string.IsNullOrEmpty(countMatch) ? int.Parse(countMatch) : 1,
        };
      })));

      return await FetchWithIdentifiers(identifiers);
    }
    public async Task<(MTGCard[] Found, int NotFoundCount)> FetchFromDTOs(CardDTO[] dtoArray)
    {
      var identifiers = dtoArray.Select(x => new ScryfallIdentifier(x));
      return await FetchWithIdentifiers(identifiers.ToArray());
    }
    public async Task<(MTGCard[] cards, string nextPageUri, int totalCount)> FetchCardsFromPage(string pageUri)
    {
      List<MTGCard> cards = new();

      JsonNode rootNode = await FetchScryfallJsonObject(pageUri);
      if (rootNode == null) { return (cards.ToArray(), string.Empty, 0); }

      cards.AddRange(await GetCardsFromJsonObject(rootNode));
      var nextPage = rootNode["has_more"]?.GetValue<bool>() is true ? rootNode["next_page"]?.GetValue<string>() : "";
      var totalCount = rootNode["total_cards"]?.GetValue<int>();

      return (cards.ToArray(), nextPage, totalCount ?? 0);
    }
    
    /// <summary>
    /// Returns <see cref="MTGCard"/> array from the given <paramref name="jsonNode"/>
    /// </summary>
    private static async Task<MTGCard[]> GetCardsFromJsonObject(JsonNode jsonNode)
    {
      var cards = new List<MTGCard>();
      if (jsonNode == null) { return cards.ToArray(); }

      JsonNode dataNode = jsonNode?["data"];
      if (dataNode != null)
      {
        var infos = dataNode.AsArray().Select(x => Task.Run(() => GetCardInfoFromJSON(x)));

        foreach (var itemInfo in await Task.WhenAll(infos))
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
    public static async Task<JsonNode> FetchScryfallJsonObject(string searchUri)
    {
      try
      {
        return JsonNode.Parse(await IO.FetchStringFromURL(searchUri));
      }
      catch (Exception) { return null; }
    }

    /// <summary>
    /// Converts Scryfall API Json object to <see cref="MTGCardInfo"/> object
    /// </summary>
    private static MTGCardInfo? GetCardInfoFromJSON(JsonNode json)
    {
      // TODO: add tokens if available
      if (json == null || json["object"]?.GetValue<string>() != "card") { return null; }

      var scryfallId = json["id"].GetValue<Guid>();
      var cmc = (int)json["cmc"].GetValue<float>();
      var name = json["name"].GetValue<string>();
      var typeLine = json["type_line"].GetValue<string>();
      var setCode = json["set"].GetValue<string>();
      var setName = json["set_name"].GetValue<string>();
      var collectorNumber = json["collector_number"].GetValue<string>();
      _ = float.TryParse(json["prices"]?["eur"]?.GetValue<string>(), NumberStyles.Float, CultureInfo.InvariantCulture, out float price);
      var apiWebsiteUri = json["scryfall_uri"]?.GetValue<string>();
      var setIconUri = $"{SET_ICON_URL}/{setCode}.svg";
      var printSearchUri = json["prints_search_uri"]?.GetValue<string>();

      // https://scryfall.com/docs/api/layouts
      var twoSideLayouts = new string[] { "transform", "modal_dfc", "double_faced_token", "art_series", "reversible_card" };
      var twoPartLayouts = new string[] { "split", "adventure", "flip" };
      var layout = json["layout"]?.GetValue<string>();
      CardFace frontFace;
      CardFace? backFace;

      if (twoSideLayouts.Contains(layout))
      {
        frontFace = new CardFace(
            colors: GetColors(json["card_faces"]?.AsArray()[0]["colors"]?.AsArray().Select(x => x.GetValue<string>()).ToArray()),
            name: json["card_faces"]?.AsArray()[0]["name"]?.GetValue<string>(),
            imageUri: json["card_faces"]?.AsArray()[0]["image_uris"]?["normal"]?.GetValue<string>());
        backFace = new CardFace(
            colors: GetColors(json["card_faces"]?.AsArray()[1]["colors"]!.AsArray().Select(x => x.GetValue<string>()).ToArray()),
            name: json["card_faces"]?.AsArray()[1]["name"]?.GetValue<string>(),
            imageUri: json["card_faces"]?.AsArray()[1]["image_uris"]?["normal"]?.GetValue<string>());
      }
      else if (twoPartLayouts.Contains(layout))
      {
        frontFace = new CardFace(
          colors: GetColors(json["colors"]?.AsArray().Select(x => x.GetValue<string>()).ToArray()),
          name: json["card_faces"]?.AsArray()[0]["name"]?.GetValue<string>(),
          imageUri: json["image_uris"]?["normal"]?.GetValue<string>());
        backFace = new CardFace(
          colors: GetColors(json["colors"]?.AsArray().Select(x => x.GetValue<string>()).ToArray()),
          name: json["card_faces"]?.AsArray()[1]["name"]?.GetValue<string>(),
          imageUri: null);
      }
      else
      {
        frontFace = new CardFace(
            colors: GetColors(json["colors"]?.AsArray().Select(x => x.GetValue<string>()).ToArray()),
            name: json["name"]?.GetValue<string>(),
            imageUri: json["image_uris"]?["normal"]?.GetValue<string>());
        backFace = null;
      }

      var rarityString = json["rarity"].GetValue<string>();
      var rarityType = RarityTypes.Common;
      if (Enum.TryParse(rarityString, true, out RarityTypes type)) { rarityType = type; }

      var producedManaStringArray = json["produced_mana"]?.AsArray().Select(x => x.GetValue<string>()).ToArray();
      var producedManaList = new List<ColorTypes>();
      if(producedManaStringArray != null)
      {
        foreach (var colorString in producedManaStringArray)
        {
          if(Enum.TryParse(colorString, true, out ColorTypes color)) { producedManaList.Add(color); }
        }
      }

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
        printSearchUri: printSearchUri
        );
    }

    /// <summary>
    /// Fetches MTGCards from the API using the given identifier objects
    /// </summary>
    private static async Task<(MTGCard[] Found, int NotFoundCount)> FetchWithIdentifiers(ScryfallIdentifier[] identifiers)
    {
      var fetchResults = await Task.WhenAll(identifiers.Chunk(MaxFetchIdentifierCount).Select(chunk => Task.Run(async () =>
      {
        var identifiersJson = JsonSerializer.Serialize(new
        {
          identifiers = chunk.Select(x => x.ToObject())
        });

        List<MTGCard> fetchedCards = new();
        int notFoundCount = 0;

        // Fetch and covert the JSON to card objects
        try
        {
          var rootNode = JsonNode.Parse(await IO.FetchStringFromURLPost(COLLECTION_URL, identifiersJson));
          JsonNode dataNode = rootNode?["data"];
          notFoundCount += rootNode?["not_found"]?.AsArray().Count ?? 0;
          if (dataNode != null)
          {
            foreach (JsonNode item in dataNode.AsArray())
            {
              MTGCardInfo? cardInfo = GetCardInfoFromJSON(item);
              var count = chunk.FirstOrDefault(x => string.Equals(cardInfo?.FrontFace.Name, x.Name, StringComparison.OrdinalIgnoreCase)).CardCount;
              if (cardInfo != null)
              {
                fetchedCards.Add(new((MTGCardInfo)cardInfo, count));
              }
            }
          }
        }
        catch (Exception) { throw; }

        return (Found: fetchedCards.ToArray(), NotFoundCount: notFoundCount);
      })));

      return (fetchResults.SelectMany(x => x.Found).ToArray(), fetchResults.Sum(x => x.NotFoundCount));
    }

    /// <summary>
    /// Converts the <paramref name="colorArray"/> to <see cref="ColorTypes"/> array
    /// </summary>
    private static ColorTypes[] GetColors(string[] colorArray)
    {
      var colors = new List<ColorTypes>();

      foreach (var color in colorArray)
      {
        if(Enum.TryParse(color, true, out ColorTypes colorType))
        {
          colors.Add(colorType);
        }
      }

      return colors.ToArray();
    }
  }
}
