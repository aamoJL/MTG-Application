using MTGApplication.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Windows.System;

namespace MTGApplication.API
{
  /// <summary>
  /// Scryfall API calls and helper functions
  /// </summary>
  public class ScryfallAPI : MTGCardAPI
  {
    private readonly string WEB_URL = "https://scryfall.com";
    private readonly string API_URL = "https://api.scryfall.com";
    //private static readonly string FILE_URL = "https://c2.scryfall.com/file";
    private readonly string CARD_FACE_URL = "https://cards.scryfall.io/normal";
    private readonly string SET_ICON_URL = "https://svgs.scryfall.io";

    /// <summary>
    /// Fetches cards from Scryfall API using given parameters
    /// </summary>
    /// <param name="searchParams">Scryfall API search parameters</param>
    /// <param name="pageLimit">Maximum page count to fetch the cards, one page has 175 cards</param>
    /// <returns></returns>
    public override async Task<MTGCardModel[]> FetchCards(string searchParams, int pageLimit)
    {
      List<MTGCardModel> cards = new();

      string searchUri = $"{API_URL}/cards/search?q={searchParams}+game:paper";
      var pageCount = 1;

      // Loop through pages the API call returns
      while (searchUri != "" && pageCount <= pageLimit)
      {
        JsonNode rootNode;
        try { rootNode = JsonNode.Parse(await IO.FetchStringFromURL(searchUri)); }
        catch (Exception) { break; }
        if (rootNode == null) { break; }

        JsonNode dataNode = rootNode?["data"];

        if (dataNode != null)
        {
          foreach (JsonNode item in dataNode.AsArray())
          {
            if (item["object"]?.GetValue<string>() == "card")
            {
              cards.Add(GetMTGCardModelFromJson(item));
            }
          }
          searchUri = rootNode["has_more"]!.GetValue<bool>() ? rootNode["next_page"]?.GetValue<string>() : "";
          pageCount++;
        }
        else { break; }
      }

      return cards.ToArray();
    }
    /// <summary>
    /// Fetches maximum of 75 MTGCardModel objects of the given identifiers using Scryfall API
    /// </summary>
    /// <param name="identifiersJson">List of card identifiers. Maximum lenght is 75 cards. The list must be in JSON format.</param>
    /// <returns></returns>
    public override async Task<MTGCardModel[]> FetchCollectionAsync(string identifiersJson)
    {
      List<MTGCardModel> cards = new();
      var fetchResult = await IO.FetchStringFromURLPost($"{API_URL}/cards/collection", identifiersJson);
      var json = JsonNode.Parse(fetchResult);
      //var notFound = json["not_found"]?.AsArray().Select(x => x.GetValue<string>()).ToArray();
      var data = json["data"]?.AsArray();

      if (data != null)
      {
        foreach (var item in data)
        {
          cards.Add(GetMTGCardModelFromJson(item));
        }
      }

      return cards.ToArray();
    }
    public override async Task<bool> OpenAPICardWebsite(MTGCardModel card)
    {
      // \u0027 = '
      return await Launcher.LaunchUriAsync(
        new($"{WEB_URL}/card/{card.Info.SetCode}/{card.Info.CollectorNumber}/" +
        $"{card.Info.Name.Replace(" // ", "-").Replace(' ', '-').Trim('\u0027')}?utm_source=api"));
    }

    public override MTGCardModel GetMTGCardModelFromJson(JsonNode cardObject)
    {
      if(cardObject == null || cardObject["object"]?.GetValue<string>() != "card") { return null; }

      var id = cardObject["id"].GetValue<string>();
      var cmc = (int)cardObject["cmc"].GetValue<float>();
      var name = cardObject["name"].GetValue<string>();
      var typeLine = cardObject["type_line"].GetValue<string>();
      var rarity = cardObject["rarity"].GetValue<string>();
      var setCode = cardObject["set"].GetValue<string>();
      var setName = cardObject["set_name"].GetValue<string>();
      var collectorNumber = cardObject["collector_number"].GetValue<string>();

      _ = float.TryParse(cardObject["prices"]["eur"]?.GetValue<string>(), out float price);

      // https://scryfall.com/docs/api/layouts
      var twoFaceLayouts = new string[] { "transform", "modal_dfc", "double_faced_token", "art_series", "reversible_card" };
      var layout = cardObject["layout"]?.GetValue<string>();
      var hasBackFace = twoFaceLayouts.Contains(layout);
      var cardFaces = new MTGCardModel.CardFace[hasBackFace ? 2 : 1];

      if (hasBackFace)
      {
        JsonNode fFaceNode = cardObject["card_faces"]?.AsArray()[0];
        JsonNode bFaceNode = cardObject["card_faces"]?.AsArray()[1];

        cardFaces[0] =
          new MTGCardModel.CardFace(
            colors: fFaceNode["colors"]?.AsArray().Select(x => x.GetValue<string>()).ToArray(),
            name: fFaceNode["name"]?.GetValue<string>()
            );
        cardFaces[1] =
          new MTGCardModel.CardFace(
            colors: bFaceNode["colors"]!.AsArray().Select(x => x.GetValue<string>()).ToArray(),
            name: bFaceNode["name"]?.GetValue<string>()
            );
      }
      else
      {
        cardFaces[0] =
          new MTGCardModel.CardFace(
            colors: cardObject["colors"]?.AsArray().Select(x => x.GetValue<string>()).ToArray(),
            name: cardObject["name"]?.GetValue<string>()
            );
      }

      return new MTGCardModel(new MTGCardModel.CardInfo(
        id: id,
        cardFaces: cardFaces,
        cmc: cmc,
        name: name,
        typeLine: typeLine,
        rarity: rarity,
        setCode: setCode,
        setName: setName,
        price: price,
        collectorNumber: collectorNumber));
    }
    public override string GetFaceUri(string id, bool back)
    {
      var side = back ? "back" : "front";
      return $"{CARD_FACE_URL}/{side}/{id[..1]}/{id[1..2]}/{id}.jpg";
    }
    public override string GetSetIconUri(string setCode)
    {
      return $"{SET_ICON_URL}/sets/{setCode}.svg";
    }
  }
}
