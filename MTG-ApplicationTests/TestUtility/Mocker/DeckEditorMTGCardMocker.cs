using MTGApplication.Features.DeckEditor.Editor.Models;
using MTGApplication.General.Services.Databases.Repositories.CardRepository.Models;
using static MTGApplication.General.Models.MTGCardInfo;

namespace MTGApplicationTests.TestUtility.Mocker;

public static class DeckEditorMTGCardMocker
{
  /// <summary>
  /// Returns a mock <see cref="DeckEditorMTGCard"/> object
  /// </summary>
  public static DeckEditorMTGCard CreateMTGCardModel(
      Guid? scryfallId = null,
      Guid? oracleId = null,
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
      ColorTypes[]? producedMana = null,
      string cardMarketUri = "https://www.cardmarket.com/en/Magic/Products/Search?referrer=scryfall&searchString=Befriending+the+Moths&utm_campaign=card_prices&utm_medium=text&utm_source=scryfall",
      CardToken[]? tokens = default,
      string group = "")
  {
    /// NOTE: Remember to also update <see cref="FromDTO(MTGCardDTO)"/> method !
    return new DeckEditorMTGCard(new(
      scryfallId: scryfallId ?? Guid.NewGuid(),
      frontFace: frontFace ?? CreateCardFace(),
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
      producedMana: producedMana ?? [],
      printSearchUri: printSearchUri,
      cardMarketUri: cardMarketUri,
      tokens: tokens ?? [],
      oracleId: oracleId ?? Guid.NewGuid(),
      colorIdentity: (frontFace ?? CreateCardFace()).Colors),
      count)
    { Group = group };
  }

  /// <summary>
  /// Retruns a mock <see cref="CardFace"/> object array
  /// </summary>
  public static CardFace CreateCardFace(
    ColorTypes[]? colors = null,
    string name = "Befriending the Moths",
    string imageUri = "https://cards.scryfall.io/normal/front/8/a/8ad44884-ae0d-40ae-87a9-bad043d4e9ad.jpg?1656453019",
    Guid? illustrationId = null,
    string oracleText = "")
  {
    colors ??= [ColorTypes.W];
    illustrationId ??= Guid.Parse("a35ceece-124c-41aa-b9f1-ef95f7d20228");
    return new CardFace(
      colors: colors,
      name: name,
      imageUri: imageUri,
      illustrationId: illustrationId,
      oracleText: oracleText,
      artCropUri: imageUri);
  }

  /// <summary>
  /// Returns a mock <see cref="DeckEditorMTGCard"/> object from the given <see cref="MTGCardDTO"/> object
  /// </summary>
  public static DeckEditorMTGCard FromDTO(MTGCardDTO dto)
  {
    return CreateMTGCardModel(
      name: dto.Name,
      scryfallId: dto.ScryfallId,
      count: dto.Count,
      oracleId: dto.OracleId,
      setCode: dto.SetCode,
      collectionNumber: dto.CollectorNumber,
      group: dto.Group);
  }
}