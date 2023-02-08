using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using System.Text.Json;
using MTGApplication.Database;

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
      public readonly string[] Colors { get; }
      public readonly string Name { get; }
      public readonly string ImageUri { get; }

      [JsonConstructor]
      public CardFace(string[] colors, string name, string imageUri)
      {
        Colors = colors;
        Name = name;
        ImageUri = imageUri;
      }
    }
    [Serializable]
    public readonly struct CardInfo
    {
      public readonly string ScryfallId { get; }
      public readonly string Name { get; }
      public readonly int CMC { get; }
      public readonly string TypeLine { get; }
      public readonly string Rarity { get; }
      public readonly string SetCode { get; }
      public readonly string SetName { get; }
      public readonly float Price { get; }
      public readonly string CollectorNumber { get; }
      public readonly string APIWebsiteUri { get; }

      public readonly CardFace FrontFace { get; }
      public readonly CardFace? BackFace { get; }
      public readonly RarityTypes RarityType { get; }
      public readonly string RarityCode { get; }
      public readonly ColorTypes ColorType { get; }
      public readonly SpellType[] SpellTypes { get; }
      public readonly string CardMarketUri { get; }
      public readonly string SetIconUri { get; }

      [JsonConstructor]
      public CardInfo(string scryfallId, CardFace frontFace, CardFace? backFace, int cmc, string name, string typeLine, string rarity, string setCode, string setName, float price, string collectorNumber, string apiWebsiteUri, string setIconUri)
      {
        ScryfallId = scryfallId;
        FrontFace = frontFace;
        BackFace = backFace;
        CMC = cmc;
        Name = name;
        TypeLine = typeLine;
        Rarity = rarity;
        SetCode = setCode;
        SetName = setName;
        Price = price;
        CollectorNumber = collectorNumber;
        APIWebsiteUri = apiWebsiteUri;
        SetIconUri = setIconUri;

        CardMarketUri = $"https://www.cardmarket.com/en/Magic/Products/Search?searchString={Name.Replace(" // ", "-").Replace(' ', '-').Trim('\u0027')}"; // '\u0027' == '
        ColorType = GetColorType(FrontFace, BackFace);
        SpellTypes = GetSpellTypes(TypeLine);

        if (Enum.TryParse(Rarity, true, out RarityTypes type)) { RarityType = type; }
        else { RarityType = RarityTypes.Common; }
        RarityCode = RarityType.ToString()[..1];
      }
    }
    #endregion

    public MTGCard() { }
    [JsonConstructor]
    public MTGCard(CardInfo info, int count)
    {
      Info = info;
      Count = count;
    }
    public MTGCard(CardInfo info)
    {
      Info = info;
    }

    protected int count = 1;
    protected CardInfo info;

    [Key]
    public int MTGCardId { get; private set; }
    [Required]
    public string Name { get; private set; }
    [Column(TypeName = "varchar(36)")]
    public string ScryfallId { get; private set; }
    public int Count
    {
      get => count;
      set
      {
        count = Math.Max(1, value);
        OnPropertyChanged(nameof(Count));
      }
    }
    [NotMapped]
    public CardInfo Info
    {
      get => info;
      set
      {
        info = value;
        Name = info.Name;
        ScryfallId = info.ScryfallId;
        OnPropertyChanged(nameof(Info));
        OnPropertyChanged(nameof(Info));
      }
    }

    public int? MTGCardDeckDeckCardsId { get; set; }
    public MTGCardDeck MTGCardDeckDeckCards { get; set; }

    public int? MTGCardDeckMaybelistId { get; set; }
    public MTGCardDeck MTGCardDeckMaybelist { get; set; }

    public int? MTGCardDeckWishlistId { get; set; }
    public MTGCardDeck MTGCardDeckWishlist { get; set; }

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

      string[] typeStrings = typeLine.Split('\u0020');

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
}
