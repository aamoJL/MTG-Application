using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Media;
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

    public readonly struct CardFace
    {
      public readonly int CMC;
      public readonly string[] Colors;
      public readonly string FaceUri;
      public readonly string Name;

      public CardFace(int cmc, string[] colors, string imageUri, string name)
      {
        CMC = cmc;
        Colors = colors ?? throw new ArgumentNullException(nameof(colors));
        FaceUri = imageUri ?? throw new ArgumentNullException(nameof(imageUri));
        Name = name ?? throw new ArgumentNullException(nameof(name));
      }
    }
    public readonly struct CardInfo
    {
      public readonly string Id;
      public readonly CardFace[] CardFaces;
      public readonly int CMC;
      public readonly string Name;
      public readonly string TypeLine;
      public readonly string Rarity;
      public readonly string SetCode;
      public readonly string SetName;
      public readonly string SetIconUri;
      public readonly float Price;
      public readonly string CollectorNumber;

      public CardInfo(string id, CardFace[] cardFaces, int cmc, string name, string typeLine, string rarity, string setCode, string setName, string setIconUri, float price, string collectorNumber)
      {
        Id = id ?? throw new ArgumentNullException(nameof(id));
        CardFaces = cardFaces ?? throw new ArgumentNullException(nameof(cardFaces));
        CMC = cmc;
        Name = name ?? throw new ArgumentNullException(nameof(name));
        TypeLine = typeLine ?? throw new ArgumentNullException(nameof(typeLine));
        Rarity = rarity ?? throw new ArgumentNullException(nameof(rarity));
        SetCode = setCode ?? throw new ArgumentNullException(nameof(setCode));
        SetName = setName ?? throw new ArgumentNullException(nameof(setName));
        SetIconUri = setIconUri ?? throw new ArgumentNullException(nameof(setIconUri));
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

    private CardInfo info;
    private int count = 1;


    public event PropertyChangedEventHandler PropertyChanged;

    public CardInfo Info
    {
      get => info; 
      private init
      {
        info = value;
        FrontFaceImg = new BitmapImage(new Uri(info.CardFaces[0].FaceUri));
        BackFaceImg = info.CardFaces.Length == 2 ? new BitmapImage(new Uri(info.CardFaces[1].FaceUri)) : null;
        SetIcon = new SvgImageSource(new Uri(info.SetIconUri));

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

    public ImageSource FrontFaceImg { get; init; }
    public ImageSource BackFaceImg { get; init; }
    public ImageSource SetIcon { get; init; }
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
