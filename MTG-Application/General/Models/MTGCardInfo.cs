using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace MTGApplication.General.Models.Card;

[Serializable]
public record MTGCardInfo
{
  public enum ColorTypes { W, U, B, R, G, M, C }
  public enum SpellType { Land, Creature, Artifact, Enchantment, Planeswalker, Instant, Sorcery, Other }
  public enum RarityTypes { Common, Uncommon, Rare, Mythic, Special, Bonus }

  [Serializable]
  public record CardToken
  {
    public Guid ScryfallId { get; }

    [JsonConstructor]
    public CardToken(Guid scryfallId) => ScryfallId = scryfallId;
  }

  [Serializable]
  public record CardFace
  {
    public ColorTypes[] Colors { get; }
    public string Name { get; }
    public string ImageUri { get; }
    public Guid? IllustrationId { get; }
    public string OracleText { get; }

    [JsonConstructor]
    public CardFace(ColorTypes[] colors, string name, string imageUri, Guid? illustrationId, string oracleText)
    {
      if (colors.Length == 0) colors = [ColorTypes.C];

      Colors = colors;
      Name = name;
      ImageUri = imageUri;
      IllustrationId = illustrationId;
      OracleText = oracleText;
    }
  }

  public Guid ScryfallId { get; init; }
  public Guid OracleId { get; init; }
  public string Name { get; init; }
  public int CMC { get; init; }
  public string TypeLine { get; init; }
  public string SetCode { get; init; }
  public string SetName { get; init; }
  public float Price { get; init; }
  public string CollectorNumber { get; init; }
  public string APIWebsiteUri { get; init; }
  public string SetIconUri { get; init; }
  public CardFace FrontFace { get; init; }
  public CardFace BackFace { get; init; }
  public string PrintSearchUri { get; init; }
  public CardToken[] Tokens { get; init; }
  public RarityTypes RarityType { get; init; }
  public ColorTypes[] Colors { get; init; }
  public SpellType[] SpellTypes { get; init; }
  public string CardMarketUri { get; init; }
  public ColorTypes[] ProducedMana { get; init; }
  public string APIName { get; init; }

  /// <summary>
  /// Constructor for JSON deserialization
  /// </summary>
  [JsonConstructor, Obsolete("This constructor should only be used by JSON deserializer")]
  public MTGCardInfo(Guid scryfallId, string name, int cmc, string typeLine, string setCode, string setName, float price, string collectorNumber, string aPIWebsiteUri, string setIconUri, CardFace frontFace, CardFace backFace, RarityTypes rarityType, ColorTypes[] colors, SpellType[] spellTypes, string cardMarketUri, ColorTypes[] producedMana, string printSearchUri, CardToken[] tokens, string apiName, Guid oracleId)
  {
    ScryfallId = scryfallId;
    OracleId = oracleId;
    Name = name;
    CMC = cmc;
    TypeLine = typeLine;
    SetCode = setCode;
    SetName = setName;
    Price = price;
    CollectorNumber = collectorNumber;
    APIWebsiteUri = aPIWebsiteUri;
    SetIconUri = setIconUri;
    FrontFace = frontFace;
    BackFace = backFace;
    RarityType = rarityType;
    Colors = colors;
    SpellTypes = spellTypes;
    CardMarketUri = cardMarketUri;
    ProducedMana = producedMana;
    PrintSearchUri = printSearchUri;
    Tokens = tokens;
    APIName = apiName;
  }
  public MTGCardInfo(Guid scryfallId, CardFace frontFace, CardFace backFace, int cmc, string name, string typeLine, string setCode, string setName, float price, string collectorNumber, string apiWebsiteUri, string setIconUri, ColorTypes[] producedMana, RarityTypes rarityType, string printSearchUri, string cardMarketUri, CardToken[] tokens, Guid oracleId, string apiName = "")
  {
    ScryfallId = scryfallId;
    OracleId = oracleId;
    Name = name;
    CMC = cmc;
    TypeLine = typeLine;
    SetCode = setCode;
    SetName = setName;
    Price = price;
    CollectorNumber = collectorNumber;
    APIWebsiteUri = apiWebsiteUri;
    SetIconUri = setIconUri;
    FrontFace = frontFace;
    BackFace = backFace;
    PrintSearchUri = printSearchUri;
    Tokens = tokens;
    RarityType = rarityType;
    Colors = GetColors(FrontFace, BackFace);
    SpellTypes = GetSpellTypes(TypeLine);
    CardMarketUri = cardMarketUri;
    ProducedMana = producedMana;
    APIName = apiName;
  }

  /// <summary>
  /// Returns all the <see cref="ColorTypes"/> that the given faces have
  /// </summary>
  private static ColorTypes[] GetColors(CardFace frontFace, CardFace backFace)
  {
    var colors = new List<ColorTypes>();

    foreach (var color in frontFace.Colors)
      if (!colors.Contains(color)) { colors.Add(color); }

    if (backFace != null)
    {
      foreach (var color in backFace?.Colors)
        if (!colors.Contains(color)) { colors.Add(color); }
    }

    return [.. colors];
  }

  /// <summary>
  /// Separates the card types from the <paramref name="typeLine"/> to a <see cref="SpellType"/> array.
  /// </summary>
  private static SpellType[] GetSpellTypes(string typeLine)
  {
    List<SpellType> types = [];
    var typeStrings = typeLine.Split('\u0020'); // 'Space'

    foreach (var typeString in typeStrings)
    {
      if (Enum.TryParse(typeString, true, out SpellType spellType))
        types.Add(spellType);
    }

    if (types.Count == 0) types.Add(SpellType.Other);

    return [.. types.OrderBy(x => x)];
  }
}