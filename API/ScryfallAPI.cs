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

    private readonly static string API_URL = "https://api.scryfall.com";
    private static string CARDS_URL => $"{API_URL}/cards";
    private static string COLLECTION_URL => $"{CARDS_URL}/collection";
    private readonly static string SET_ICON_URL = "https://svgs.scryfall.io/sets";
    private static int MaxFetchIdentifierCount => 75;

    /// <summary>
    /// Converts search parameters to Scryfall API search uri
    /// </summary>
    public static string GetSearchUri(string searchParams) => $"{CARDS_URL}/search?q={searchParams}+game:paper";

    /// <summary>
    /// Fetches cards from Scryfall API using given parameters
    /// </summary>
    /// <param name="searchParams">Scryfall API search parameters</param>
    /// <param name="countLimit">Maximum page count to fetch the cards, one page has 175 cards</param>
    /// <returns></returns>
    public async Task<MTGCard[]> FetchCards(string searchParams, int countLimit)
    {
      if (string.IsNullOrEmpty(searchParams)) { return Array.Empty<MTGCard>(); }
      List<MTGCardInfo> cardInfos = new();
      string searchUri = GetSearchUri(searchParams);
      var pageCount = 1;

      // Loop through API pages
      while (searchUri != "" && cardInfos.Count < countLimit)
      {
        // TODO: enumeration
        JsonNode rootNode = await FetchScryfallJsonObject(searchUri);
        if (rootNode == null) { break; }

        JsonNode dataNode = rootNode?["data"];

        if (dataNode != null)
        {
          var infos = dataNode.AsArray().Select(x => Task.Run(() => GetCardInfoFromJSON(x)));

          foreach (var itemInfo in await Task.WhenAll(infos))
          {
            if (itemInfo != null) cardInfos.Add((MTGCardInfo)itemInfo);
          }

          searchUri = rootNode["has_more"]!.GetValue<bool>() ? rootNode["next_page"]?.GetValue<string>() : "";
          pageCount++;
        }
        else { break; }
      }

      List<MTGCard> cards = new();

      for (int i = 0; i < cardInfos.Count && i < countLimit; i++)
      {
        cards.Add(new MTGCard(cardInfos[i]));
      }

      return cards.ToArray();
    }

    /// <summary>
    /// Converts <paramref name="importText"/> to <see cref="MTGCard"/> array using the API.
    /// <paramref name="importText"/> needs to be formatted like this:
    /// <code> 
    /// {count (optional)} {name} 
    /// {count (optional)} {name} 
    /// </code>
    /// </summary>
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
    /// Fetches MTGCards from the API using the given CardDTO objects.
    /// </summary>
    public async Task<(MTGCard[] Found, int NotFoundCount)> FetchFromDTOs(CardDTO[] dtoArray)
    {
      var identifiers = dtoArray.Select(x => new ScryfallIdentifier(x));
      return await FetchWithIdentifiers(identifiers.ToArray());
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
      var rarity = json["rarity"].GetValue<string>();
      var setCode = json["set"].GetValue<string>();
      var setName = json["set_name"].GetValue<string>();
      var collectorNumber = json["collector_number"].GetValue<string>();
      _ = float.TryParse(json["prices"]?["eur"]?.GetValue<string>(), NumberStyles.Float, CultureInfo.InvariantCulture, out float price);
      var apiWebsiteUri = json["scryfall_uri"]?.GetValue<string>();
      var setIconUri = $"{SET_ICON_URL}/{setCode}.svg";

      // https://scryfall.com/docs/api/layouts
      var twoSideLayouts = new string[] { "transform", "modal_dfc", "double_faced_token", "art_series", "reversible_card" };
      var twoPartLayouts = new string[] { "split", "adventure", "flip" };
      var layout = json["layout"]?.GetValue<string>();
      CardFace frontFace;
      CardFace? backFace;

      if (twoSideLayouts.Contains(layout))
      {
        frontFace = new CardFace(
            colors: json["card_faces"]?.AsArray()[0]["colors"]?.AsArray().Select(x => x.GetValue<string>()).ToArray(),
            name: json["card_faces"]?.AsArray()[0]["name"]?.GetValue<string>(),
            imageUri: json["card_faces"]?.AsArray()[0]["image_uris"]?["normal"]?.GetValue<string>());
        backFace = new CardFace(
            colors: json["card_faces"]?.AsArray()[1]["colors"]!.AsArray().Select(x => x.GetValue<string>()).ToArray(),
            name: json["card_faces"]?.AsArray()[1]["name"]?.GetValue<string>(),
            imageUri: json["card_faces"]?.AsArray()[1]["image_uris"]?["normal"]?.GetValue<string>());
      }
      else if (twoPartLayouts.Contains(layout))
      {
        frontFace = new CardFace(
          colors: json["colors"]?.AsArray().Select(x => x.GetValue<string>()).ToArray(),
          name: json["card_faces"]?.AsArray()[0]["name"]?.GetValue<string>(),
          imageUri: json["image_uris"]?["normal"]?.GetValue<string>());
        backFace = new CardFace(
          colors: json["colors"]?.AsArray().Select(x => x.GetValue<string>()).ToArray(),
          name: json["card_faces"]?.AsArray()[1]["name"]?.GetValue<string>(),
          imageUri: null);
      }
      else
      {
        frontFace = new CardFace(
            colors: json["colors"]?.AsArray().Select(x => x.GetValue<string>()).ToArray(),
            name: json["name"]?.GetValue<string>(),
            imageUri: json["image_uris"]?["normal"]?.GetValue<string>());
        backFace = null;
      }

      return new MTGCardInfo(
        scryfallId: scryfallId,
        frontFace: frontFace,
        backFace: backFace,
        cmc: cmc,
        name: name,
        typeLine: typeLine,
        rarity: rarity,
        setCode: setCode,
        setName: setName,
        price: price,
        collectorNumber: collectorNumber,
        apiWebsiteUri: apiWebsiteUri,
        setIconUri: setIconUri
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
  }
}
