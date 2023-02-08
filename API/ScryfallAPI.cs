﻿using MTGApplication.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
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
  public class ScryfallAPI : MTGCardAPI
  {
    private readonly static string API_URL = "https://api.scryfall.com";
    private static string CARDS_URL => $"{API_URL}/cards";
    private static string COLLECTION_URL => $"{CARDS_URL}/collection";
    private readonly static string SET_ICON_URL = "https://svgs.scryfall.io/sets";
    private static int MaxFetchIdentifierCount => 75;

    /// <summary>
    /// Fetches cards from Scryfall API using given parameters
    /// </summary>
    /// <param name="searchParams">Scryfall API search parameters</param>
    /// <param name="countLimit">Maximum page count to fetch the cards, one page has 175 cards</param>
    /// <returns></returns>
    public override async Task<MTGCard[]> FetchCards(string searchParams, int countLimit)
    {
      List<CardInfo> cardInfos = new();
      string searchUri = $"{CARDS_URL}/search?q={searchParams}+game:paper";
      var pageCount = 1;

      // Loop through API pages
      while (searchUri != "" && cardInfos.Count < countLimit)
      {
        // TODO: enumeration
        JsonNode rootNode;
        try { rootNode = JsonNode.Parse(await IO.FetchStringFromURL(searchUri)); }
        catch (Exception) { break; }
        if (rootNode == null) { break; }

        JsonNode dataNode = rootNode?["data"];

        if (dataNode != null)
        {
          foreach (JsonNode item in dataNode.AsArray())
          {
            var itemInfo = GetCardInfoFromJSON(item);
            if(itemInfo != null) cardInfos.Add((CardInfo)itemInfo);
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
    /// Updates <see cref="MTGCard.Info"/> property of the given cards from the API
    /// </summary>
    public override async Task<bool> PopulateMTGCardInfosAsync(MTGCard[] cards)
    {
      List<CardInfo> cardInfos = new();

      foreach (var chunk in cards.Chunk(MaxFetchIdentifierCount))
      {
        // Fetch cards in chunks
        var identifiersJson = string.Empty;

        using var stream = new MemoryStream();
        await JsonSerializer.SerializeAsync(stream, new
        {
          identifiers = chunk.Select(x => new
          {
            id = x.ScryfallId
          })
        });
        stream.Position = 0;
        using var reader = new StreamReader(stream);
        identifiersJson = await reader.ReadToEndAsync();

        // Fetch Scryfall JSON for the chunk
        var json = JsonNode.Parse(await IO.FetchStringFromURLPost(COLLECTION_URL, identifiersJson));
        var data = json["data"]?.AsArray();

        // Loop through json and populate infos to the cards
        foreach (var item in data)
        {
          var cardInfo = GetCardInfoFromJSON(item);
          if(cardInfo == null) { continue; }
          foreach (var card in cards)
          {
            if (!string.IsNullOrEmpty(card.Info.ScryfallId)) { continue; } // Card already has info
            if(card.ScryfallId == cardInfo?.ScryfallId) { card.Info = (CardInfo)cardInfo; break; }
          }
        }
      }

      return true;
    }

    /// <summary>
    /// Converts Scryfall API Json object to <see cref="CardInfo"/> object
    /// </summary>
    private static CardInfo? GetCardInfoFromJSON(JsonNode json)
    {
      // TODO: add tokens if available
      if (json == null || json["object"]?.GetValue<string>() != "card") { return null; }

      var scryfallId = json["id"].GetValue<string>();
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

      return new CardInfo(
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
    /// Converts <paramref name="importText"/> to <see cref="MTGCard"/> array using the API.
    /// <paramref name="importText"/> needs to be formatted like this:
    /// <code> 
    /// {count (optional)} {name} 
    /// {count (optional)} {name} 
    /// </code>
    /// </summary>
    public override async Task<MTGCard[]> FetchImportedCards(string importText)
    {
      var lines = importText.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

      // Convert each line to scryfall search objects
      var scryfallObjects = lines.Select(line =>
      {
        // Format: {Count (optional)} {Name}
        // Stops at '/' so only the first name will be used for multiface cards
        var regexGroups = new Regex("(?:^[\\s]*(?<Count>[0-9]*(?=\\s)){0,1}\\s*(?<Name>[\\s\\S][^/]*))");
        var match = regexGroups.Match(line);

        var countMatch = match.Groups["Count"]?.Value;
        var nameMatch = match.Groups["Name"]?.Value;

        if (string.IsNullOrEmpty(nameMatch)) { return null; }

        return new
        {
          Count = !string.IsNullOrEmpty(countMatch) ? int.Parse(countMatch) : 1,
          Name = nameMatch.Trim(),
        };
      });

      // Loop through the scryfall objects in chunks because scryfall API limits how many cards can be searched at once
      List<Task<MTGCard[]>> chunkTasks = new(scryfallObjects.Chunk(MaxFetchIdentifierCount).Select(chunk => Task.Run(async () =>
      {
        List<MTGCard> fetchedCards = new();
        var identifiersJson = string.Empty;

        // Convert the chunk to JSON text
        using (var stream = new MemoryStream())
        {
          await JsonSerializer.SerializeAsync(stream, new
          {
            identifiers = chunk.Select(x => new
            {
              name = x.Name
            })
          });
          stream.Position = 0;
          using var reader = new StreamReader(stream);
          identifiersJson = await reader.ReadToEndAsync();
        }

        // Fetch and covert the JSON to card objects
        try
        {
          var rootNode = JsonNode.Parse(await IO.FetchStringFromURLPost(COLLECTION_URL, identifiersJson));
          JsonNode dataNode = rootNode?["data"];
          if (dataNode != null)
          {
            foreach (JsonNode item in dataNode.AsArray())
            {
              CardInfo? cardInfo = GetCardInfoFromJSON(item);
              var count = chunk.FirstOrDefault(x => string.Equals(cardInfo?.FrontFace.Name, x.Name, StringComparison.OrdinalIgnoreCase))?.Count; // Chunk has only frontface name
              if(cardInfo != null && count != null)
              {
                fetchedCards.Add(new((CardInfo)cardInfo, (int)count));
              }
            }
          }
        }
        catch (Exception) { throw; }
        
        return fetchedCards.ToArray();
      })));

      var fetchedCardLists = await Task.WhenAll(chunkTasks);
      List<MTGCard> cards= new();

      foreach (var list in fetchedCardLists)
      {
        cards.AddRange(list);
      }

      return cards.ToArray();
    }
  }
}
