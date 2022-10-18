using MTGApplication.Models;
using MTG_builder;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Windows.System;

namespace App1.API
{
    /// <summary>
    /// Scryfall API calls and helper functions
    /// </summary>
    public static class ScryfallAPI
    {
        private static readonly string API_URL = "https://api.scryfall.com";
        private static readonly string API_FILE_URL = "https://c2.scryfall.com/file";

        /// <summary>
        /// Fetches cards from Scryfall API using given parameters
        /// </summary>
        /// <param name="searchParams">Scryfall API search parameters</param>
        /// <param name="pageLimit">Maximum page count to fetch the cards, one page has 175 cards</param>
        /// <returns></returns>
        public static async Task<ObservableCollection<MTGCardModel>> FetchCards(string searchParams, int pageLimit = 3)
        {
            ObservableCollection<MTGCardModel> cards = new();

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

            return cards;
        }
        /// <summary>
        /// Fetches maximum of 75 MTGCardModel objects of the given identifiers using Scryfall API
        /// </summary>
        /// <param name="identifiersJson">List of card identifiers. Maximum lenght is 75 cards. The list must be in JSON format.</param>
        /// <returns></returns>
        public static async Task<List<MTGCardModel>> FetchCollection(string identifiersJson)
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

            return cards;
        }
        public static async Task<bool> OpenAPICardWebsite(MTGCardModel card)
        {
            return await Launcher.LaunchUriAsync(new($"{API_URL}/card/{card.Info.SetCode}/{card.Info.CollectorNumber}/{card.Info.Name.Replace(' ', '-')}?utm_source=api"));
        }

        public static MTGCardModel GetMTGCardModelFromJson(JsonNode cardObject)
        {
            JsonNode fFaceNode = cardObject["card_faces"]?.AsArray()[0];
            JsonNode bFaceNode = cardObject["card_faces"]?.AsArray()[1];

            var id = cardObject["id"]?.GetValue<string>();
            var cardFaces = bFaceNode != null ?
              new MTGCardModel.CardFace[2]
              {
          new MTGCardModel.CardFace(
            cmc: fFaceNode["cmc"] != null ? (int)fFaceNode["cmc"]?.GetValue<float>() : 0,
            colors: fFaceNode["colors"]?.AsArray().Select(x => x.GetValue<string>()).ToArray(),
            imageUri: fFaceNode["image_uris"]?["normal"]?.GetValue<string>(),
            name: fFaceNode["name"]?.GetValue<string>()
            ),
          new MTGCardModel.CardFace(
            cmc: (int)cardObject["cmc"]?.GetValue<float>(),
            colors: cardObject["colors"]!.AsArray().Select(x => x.GetValue<string>()).ToArray(),
            imageUri: cardObject["image_uris"]!["normal"]!.GetValue<string>(),
            name: cardObject["name"]?.GetValue<string>()
            )
              } :
              new MTGCardModel.CardFace[1]
              {
          new MTGCardModel.CardFace(
            cmc: bFaceNode["cmc"] != null ? (int)bFaceNode["cmc"]?.GetValue<float>() : 0,
            colors: bFaceNode["colors"]?.AsArray().Select(x => x.GetValue<string>()).ToArray(),
            imageUri: bFaceNode["image_uris"]?["normal"]?.GetValue<string>(),
            name: bFaceNode["name"]?.GetValue<string>()
            )
              };

            var cmc = (int)cardObject["cmc"]?.GetValue<float>();
            var name = cardObject["name"]?.GetValue<string>();
            var cardTypeLine = cardObject["type_line"]?.GetValue<string>();
            var price = cardObject["prices"]?["eur"]?.GetValue<string>();
            var rarity = cardObject["rarity"]?.GetValue<string>();
            var setCode = cardObject["set"]?.GetValue<string>();
            var setIconUri = $"{API_FILE_URL}/scryfall-symbols/sets/{setCode}.svg";
            var setName = cardObject["set_name"]?.GetValue<string>();
            var collectorNumber = cardObject["collector_number"]?.GetValue<string>();

            return new MTGCardModel(new MTGCardModel.CardInfo(
              id: id,
              cardFaces: cardFaces,
              cmc: cmc,
              name: name,
              typeLine: cardTypeLine,
              rarity: rarity,
              setCode: setCode,
              setName: setName,
              setIconUri: setIconUri,
              price: float.Parse(price),
              collectorNumber: collectorNumber));
        }
    }
}
