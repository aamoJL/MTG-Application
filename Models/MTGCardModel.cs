using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MTGApplication.Models
{
  /// <summary>
  /// MTG card model
  /// </summary>
  public class MTGCardModel : INotifyPropertyChanged
  {
    public enum RarityTypes
    {
      Common = 0,
      Uncommon = 1,
      Rare = 2,
      Mythic = 3,
      Special = 4,
      Bonus = 5,
    }
    public enum ColorTypes
    {
      W = 0,
      U = 1,
      B = 2,
      R = 3,
      G = 4,
      M = 5,
      C = 6,
    }
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

    [Serializable]
    public readonly struct CardFace
    {
      public readonly string[] Colors { get; }
      public readonly string Name { get; }

      [JsonConstructor]
      public CardFace(string[] colors, string name)
      {
        Colors = colors ?? throw new ArgumentNullException(nameof(colors));
        Name = name ?? throw new ArgumentNullException(nameof(name));
      }
    }
    [Serializable]
    public readonly struct CardInfo
    {
      public readonly string Id { get; }
      public readonly CardFace[] CardFaces { get; }
      public readonly int CMC { get; }
      public readonly string Name { get; }
      public readonly string TypeLine { get; }
      public readonly string Rarity { get; }
      public readonly string SetCode { get; }
      public readonly string SetName { get; }
      public readonly float Price { get; }
      public readonly string CollectorNumber { get; }

      [JsonConstructor]
      public CardInfo(string id, CardFace[] cardFaces, int cmc, string name, string typeLine, string rarity, string setCode, string setName, float price, string collectorNumber)
      {
        Id = id ?? throw new ArgumentNullException(nameof(id));
        CardFaces = cardFaces ?? throw new ArgumentNullException(nameof(cardFaces));
        CMC = cmc;
        Name = name ?? throw new ArgumentNullException(nameof(name));
        TypeLine = typeLine ?? throw new ArgumentNullException(nameof(typeLine));
        Rarity = rarity ?? throw new ArgumentNullException(nameof(rarity));
        SetCode = setCode ?? throw new ArgumentNullException(nameof(setCode));
        SetName = setName ?? throw new ArgumentNullException(nameof(setName));
        Price = price;
        CollectorNumber = collectorNumber ?? throw new ArgumentNullException(nameof(collectorNumber));
      }
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
    public static ColorTypes GetColorType(CardFace[] faces)
    {
      CardFace frontFace = faces[0];
      CardFace? backFace = faces.Length == 2 ? faces[1] : null;

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

    public event PropertyChangedEventHandler PropertyChanged;

    private CardInfo info;
    private int count = 1;

    public CardInfo Info
    {
      get => info;
      private init
      {
        info = value;

        if(info.Id == "" || info.Id == null) { return; }

        ColorType = GetColorType(info.CardFaces);
        SpellTypes = GetSpellTypes(info.TypeLine);

        if (Enum.TryParse(info.Rarity, true, out RarityTypes type)) { RarityType = type; }
        else { RarityType = RarityTypes.Common; }
        RarityCode = RarityType.ToString()[..1];
      }
    }
    public int Count
    {
      get => count;
      set
      {
        count = Math.Max(1, value);
        PropertyChanged?.Invoke(this, new(nameof(Count)));
      }
    }

    public CardFace FrontFace => Info.CardFaces[0];
    public CardFace? BackFace => Info.CardFaces.Length == 2 ? Info.CardFaces[1] : null;
    public RarityTypes RarityType { get; init; }
    public string RarityCode { get; init; }
    public ColorTypes ColorType { get; init; }
    public SpellType[] SpellTypes { get; init; }
    public bool HasBackFace => BackFace != null;

    [JsonConstructor]
    public MTGCardModel(CardInfo info, int count)
    {
      Info = info;
      Count = count;
    }
    public MTGCardModel(CardInfo info)
    {
      Info = info;
    }

    public string ToJSON()
    {
      return JsonSerializer.Serialize(new
      {
        Info,
        Count
      });
    }
  }
}
