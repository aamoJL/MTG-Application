using CommunityToolkit.Mvvm.ComponentModel;
using MTGApplication.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MTGApplication.Models;

/// <summary>
/// MTG card model
/// </summary>
public partial class MTGCard : ObservableObject
{
  #region Enums
  public enum RarityTypes
  {
    Common = 0,
    Uncommon = 1,
    Rare = 2,
    Mythic = 3,
    Special = 4,
    Bonus = 5,
  }
  public enum ColorTypes { W, U, B, R, G, M, C }
  public enum SpellType
  {
    Land,
    Creature,
    Artifact,
    Enchantment,
    Planeswalker,
    Instant,
    Sorcery,
    Other
  }
  public enum CardSide { Front, Back }
  #endregion

  #region Structs
  [Serializable]
  public readonly struct CardToken
  {
    public Guid ScryfallId { get; }

    [JsonConstructor]
    public CardToken(Guid scryfallId) => ScryfallId = scryfallId;
  }
  [Serializable]
  public readonly struct CardFace
  {
    public ColorTypes[] Colors { get; }
    public string Name { get; }
    public string ImageUri { get; }
    public Guid? IllustrationId { get; }
    public string OracleText { get; }

    [JsonConstructor]
    public CardFace(ColorTypes[] colors, string name, string imageUri, Guid? illustrationId, string oracleText)
    {
      Colors = colors;
      Name = name;
      ImageUri = imageUri;
      IllustrationId = illustrationId;
      OracleText = oracleText;
    }
  }
  [Serializable]
  public readonly struct MTGCardInfo
  {
    public Guid ScryfallId { get; }
    public string Name { get; }
    public int CMC { get; }
    public string TypeLine { get; }
    public string SetCode { get; }
    public string SetName { get; }
    public float Price { get; }
    public string CollectorNumber { get; }
    public string APIWebsiteUri { get; }
    public string SetIconUri { get; }
    public CardFace FrontFace { get; }
    public CardFace? BackFace { get; }
    public string PrintSearchUri { get; }
    public CardToken[] Tokens { get; }

    public RarityTypes RarityType { get; }
    public ColorTypes[] Colors { get; }
    public SpellType[] SpellTypes { get; }
    public string CardMarketUri { get; }
    public ColorTypes[] ProducedMana { get; }

    /// <summary>
    /// Constructor for JSON deserialization
    /// </summary>
    [JsonConstructor, Obsolete("This constructor should only be used by JSON deserializer")]
    public MTGCardInfo(Guid scryfallId, string name, int cmc, string typeLine, string setCode, string setName, float price, string collectorNumber, string aPIWebsiteUri, string setIconUri, CardFace frontFace, CardFace? backFace, RarityTypes rarityType, ColorTypes[] colors, SpellType[] spellTypes, string cardMarketUri, ColorTypes[] producedMana, string printSearchUri, CardToken[] tokens)
    {
      ScryfallId = scryfallId;
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
    }
    public MTGCardInfo(Guid scryfallId, CardFace frontFace, CardFace? backFace, int cmc, string name, string typeLine, string setCode, string setName, float price, string collectorNumber, string apiWebsiteUri, string setIconUri, ColorTypes[] producedMana, RarityTypes rarityType, string printSearchUri, string cardMarketUri, CardToken[] tokens)
    {
      ScryfallId = scryfallId;
      Name = name;
      CMC = cmc;
      TypeLine = typeLine;
      SetCode = setCode;
      SetName = setName;
      Price = price;
      CollectorNumber = collectorNumber;
      APIWebsiteUri = apiWebsiteUri;
      SetIconUri = setIconUri;
      RarityType = rarityType;
      PrintSearchUri = printSearchUri;
      CardMarketUri = cardMarketUri;
      Tokens = tokens;

      FrontFace = frontFace;
      BackFace = backFace;

      Colors = GetColors(FrontFace, BackFace);
      SpellTypes = GetSpellTypes(TypeLine);
      ProducedMana = producedMana;
    }
  }
  #endregion

  [JsonConstructor]
  public MTGCard(MTGCardInfo info, int count = 1)
  {
    Info = info;
    Count = count;
  }

  protected int count = 1;

  [ObservableProperty]
  private MTGCardInfo info;

  public int Count
  {
    get => count;
    set
    {
      count = Math.Max(1, value);
      OnPropertyChanged(nameof(Count));
    }
  }

  /// <summary>
  /// Returns the card info and count as a Json string
  /// </summary>
  public string ToJSON()
  {
    return JsonSerializer.Serialize(new
    {
      Info,
      Count
    });
  }

  /// <summary>
  /// Returns the API's name that was used to fetch the given card
  /// </summary>
  public string GetAPIName()
  {
    if (Info.ScryfallId != Guid.Empty)
    { return ScryfallAPI.APIName; }
    else
    { return string.Empty; }
  }

  /// <summary>
  /// Separates the card types from the <paramref name="typeLine"/> to a <see cref="SpellType"/> array.
  /// </summary>
  public static SpellType[] GetSpellTypes(string typeLine)
  {
    List<SpellType> types = new();
    var typeStrings = typeLine.Split('\u0020'); // 'Space'

    foreach (var typeString in typeStrings)
    {
      if (Enum.TryParse(typeString, true, out SpellType spellType))
      {
        types.Add(spellType);
      }
    }

    if (types.Count == 0)
    { types.Add(SpellType.Other); }
    return types.OrderBy(x => x).ToArray();
  }

  /// <summary>
  /// Returns given <paramref name="color"/> character's full name
  /// </summary>
  public static string GetColorTypeName(ColorTypes color)
  {
    return color switch
    {
      ColorTypes.W => "White",
      ColorTypes.U => "Blue",
      ColorTypes.B => "Black",
      ColorTypes.R => "Red",
      ColorTypes.G => "Green",
      ColorTypes.M => "Multicolor",
      _ => "Colorless",
    };
  }

  /// <summary>
  /// Returns all the <see cref="ColorTypes"/> that the given faces have
  /// </summary>
  public static ColorTypes[] GetColors(CardFace frontFace, CardFace? backFace)
  {
    var colors = new List<ColorTypes>();

    foreach (var color in frontFace.Colors)
    {
      if (!colors.Contains(color))
      { colors.Add(color); }
    }

    if (backFace != null)
    {
      foreach (var color in backFace?.Colors)
      {
        if (!colors.Contains(color))
        { colors.Add(color); }
      }
    }

    // Card is colorless if it has no other colors
    if (colors.Count == 0)
    { colors.Add(ColorTypes.C); }
    return colors.ToArray();
  }
}

/// <summary>
/// Data transfer object for <see cref="MTGCard"/> class
/// </summary>
public class MTGCardDTO : CardDTO
{
  private MTGCardDTO() : base() { }
  public MTGCardDTO(MTGCard card) : base()
  {
    Name = card.Info.Name;
    ScryfallId = card.Info.ScryfallId;
    Count = card.Count;
  }

  public Guid ScryfallId { get; init; }

  public MTGCardDeckDTO DeckCards { get; set; }
  public MTGCardDeckDTO DeckWishlist { get; set; }
  public MTGCardDeckDTO DeckMaybelist { get; set; }
  public MTGCardCollectionListDTO CollectionList { get; set; }
}
