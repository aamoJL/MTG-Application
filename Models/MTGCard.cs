using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.ComponentModel.DataAnnotations;

namespace MTGApplication.Models
{
  /// <summary>
  /// MTG card model
  /// </summary>
  public class MTGCard : ObservableObject
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
    public readonly struct CardFace
    {
      public string[] Colors { get; }
      public string Name { get; }
      public string ImageUri { get; }

      [JsonConstructor]
      public CardFace(string[] colors, string name, string imageUri)
      {
        Colors = colors;
        Name = name;
        ImageUri = imageUri;
      }
    }
    [Serializable]
    public readonly struct MTGCardInfo
    {
      public Guid ScryfallId { get; }
      public string Name { get; }
      public int CMC { get; }
      public string TypeLine { get; }
      public string Rarity { get; }
      public string SetCode { get; }
      public string SetName { get; }
      public float Price { get; }
      public string CollectorNumber { get; }
      public string APIWebsiteUri { get; }
      public string SetIconUri { get; } 
      public CardFace FrontFace { get; }
      public CardFace? BackFace { get; }

      public RarityTypes RarityType { get; }
      public string RarityCode { get; }
      public ColorTypes ColorType { get; }
      public SpellType[] SpellTypes { get; }
      public string CardMarketUri { get; }

      /// <summary>
      /// Constructor for JSON deserialization
      /// </summary>
      [JsonConstructor, Obsolete("This constructor should only be used by JSON deserializer")]
      public MTGCardInfo(Guid scryfallId, string name, int cMC, string typeLine, string rarity, string setCode, string setName, float price, string collectorNumber, string aPIWebsiteUri, string setIconUri, CardFace frontFace, CardFace? backFace, RarityTypes rarityType, string rarityCode, ColorTypes colorType, SpellType[] spellTypes, string cardMarketUri)
      {
        ScryfallId = scryfallId;
        Name = name;
        CMC = cMC;
        TypeLine = typeLine;
        Rarity = rarity;
        SetCode = setCode;
        SetName = setName;
        Price = price;
        CollectorNumber = collectorNumber;
        APIWebsiteUri = aPIWebsiteUri;
        SetIconUri = setIconUri;
        FrontFace = frontFace;
        BackFace = backFace;
        RarityType = rarityType;
        RarityCode = rarityCode;
        ColorType = colorType;
        SpellTypes = spellTypes;
        CardMarketUri = cardMarketUri;
      }
      public MTGCardInfo(Guid scryfallId, CardFace frontFace, CardFace? backFace, int cmc, string name, string typeLine, string rarity, string setCode, string setName, float price, string collectorNumber, string apiWebsiteUri, string setIconUri)
      {
        ScryfallId = scryfallId;
        Name = name;
        CMC = cmc;
        TypeLine = typeLine;
        Rarity = rarity;
        SetCode = setCode;
        SetName = setName;
        Price = price;
        CollectorNumber = collectorNumber;
        APIWebsiteUri = apiWebsiteUri;
        SetIconUri = setIconUri;
        
        FrontFace = frontFace;
        BackFace = backFace;

        if (Enum.TryParse(Rarity, true, out RarityTypes type)) { RarityType = type; }
        else { RarityType = RarityTypes.Common; }
        RarityCode = RarityType.ToString()[..1];
        ColorType = GetColorType(FrontFace, BackFace);
        SpellTypes = GetSpellTypes(TypeLine);
        CardMarketUri = $"https://www.cardmarket.com/en/Magic/Products/Search?searchString={Name.Replace(" // ", "-").Replace(' ', '-').Trim('\u0027')}"; // '\u0027' == '

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

    public MTGCardInfo Info { get; set; }
    public int Count
    {
      get => count;
      set
      {
        count = Math.Max(1, value);
        OnPropertyChanged(nameof(Count));
      }
    }

    public string ToJSON()
    {
      return JsonSerializer.Serialize(new
      {
        Info,
        Count
      });
    }
    public static SpellType[] GetSpellTypes(string typeLine)
    {
      List<SpellType> types = new();

      string[] typeStrings = typeLine.Split('\u0020'); // 'Space'

      foreach (string typeString in typeStrings)
      {
        if (Enum.TryParse(typeString, true, out SpellType spellType))
        {
          types.Add(spellType);
        }
      }

      if (types.Count == 0) { types.Add(SpellType.Other); }

      return types.ToArray();
    }
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
    public static ColorTypes GetColorType(CardFace frontFace, CardFace? backFace)
    {
      ColorTypes fFaceColor =
        frontFace.Colors.Length == 0 ? ColorTypes.C :
        frontFace.Colors.Length > 1 ? ColorTypes.M :
        (ColorTypes)Enum.Parse(typeof(ColorTypes), frontFace.Colors[0]);
      ColorTypes? bFaceColor = backFace == null ? null :
        backFace?.Colors.Length == 0 ? ColorTypes.C :
        backFace?.Colors.Length > 1 ? ColorTypes.M :
        (ColorTypes)Enum.Parse(typeof(ColorTypes), backFace?.Colors[0], true);

      if (bFaceColor == null) { return fFaceColor; }
      else if (fFaceColor == ColorTypes.C) { return (ColorTypes)bFaceColor; }
      else if (bFaceColor == ColorTypes.C) { return fFaceColor; }
      else if (fFaceColor != bFaceColor) { return ColorTypes.M; }
      else { return fFaceColor; }
    }
  }

  public class CardDTO
  {
    private CardDTO() { }
    public CardDTO(MTGCard card)
    {
      Name = card.Info.Name;
      ScryfallId = card.Info.ScryfallId;
      Count = card.Count;
    }

    [Key]
    public int Id { get; init; }
    public string Name { get; init; }
    public Guid ScryfallId { get; init; }
    public int Count { get; set; }

    public MTGCardDeckDTO DeckCards { get; set; }
    public MTGCardDeckDTO DeckWishlist { get; set; }
    public MTGCardDeckDTO DeckMaybelist { get; set; }
  }
}
