﻿using MTGApplication.Models;
using static MTGApplication.Models.MTGCard;

namespace MTGApplicationTests.Services
{
  public static class Mocker
  {
    public static class MTGCardModelMocker
    {
      /// <summary>
      /// Returns a mock <see cref="MTGCard"/> object
      /// </summary>
      public static MTGCard CreateMTGCardModel(
          Guid? scryfallId = null,
          CardFace? frontFace = null,
          CardFace? backFace = null,
          int cmc = 4,
          string name = "Befriending the Moths // Imperial Moth",
          string typeLine = "Enchantment — Saga // Enchantment Creature — Insect",
          RarityTypes rarity = RarityTypes.Common,
          string setCode = "neo",
          string setName = "Kamigawa: Neon Dynasty",
          string setIconUri = "https://svgs.scryfall.io/sets/neo.svg?1665979200",
          float price = 0.02f,
          string collectionNumber = "4",
          int count = 1,
          string apiWebsiteUri = "https://scryfall.com/card/neo/4/befriending-the-moths-imperial-moth?utm_source=api",
          string printSearchUri = "https://api.scryfall.com/cards/search?order=released&q=oracleid%3A2ee5f5ad-2f16-40d9-831a-2aefece31b36&unique=prints",
          ColorTypes[]? producedMana = null)
      {
        
        producedMana ??= Array.Empty<ColorTypes>();
        scryfallId ??= Guid.NewGuid();
        frontFace ??= CreateCardFace();

        return new MTGCard(new(
          scryfallId: (Guid)scryfallId,
          frontFace: (CardFace)frontFace,
          backFace: backFace,
          cmc: cmc,
          name: name,
          typeLine: typeLine,
          rarityType: rarity,
          setCode: setCode,
          setName: setName,
          price: price,
          collectorNumber: collectionNumber,
          apiWebsiteUri: apiWebsiteUri,
          setIconUri: setIconUri,
          producedMana: producedMana,
          printSearchUri: printSearchUri
          ), count);
      }

      /// <summary>
      /// Retruns a mock <see cref="CardFace"/> object array
      /// </summary>
      public static CardFace CreateCardFace(
        ColorTypes[]? colors = null,
        string name = "Befriending the Moths",
        string imageUri = "https://cards.scryfall.io/normal/front/8/a/8ad44884-ae0d-40ae-87a9-bad043d4e9ad.jpg?1656453019")
      {
        colors ??= new ColorTypes[] { ColorTypes.W };
        return new CardFace(
            colors: colors,
            name: name,
            imageUri: imageUri
            ); ;
      }

      /// <summary>
      /// Returns a mock <see cref="MTGCard"/> object from the given <see cref="CardDTO"/> object
      /// </summary>
      public static MTGCard FromDTO(CardDTO dto)
      {
        return CreateMTGCardModel(
          name: dto.Name,
          scryfallId: dto.ScryfallId,
          count: dto.Count
          );
      }
    }
  }
}