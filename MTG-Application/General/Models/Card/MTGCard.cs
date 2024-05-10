﻿using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace MTGApplication.General.Models.Card;

/// <summary>
/// MTG card model
/// </summary>
public partial class MTGCard : ObservableObject
{
  public enum ColorTypes { W, U, B, R, G, M, C }
  public enum SpellType { Land, Creature, Artifact, Enchantment, Planeswalker, Instant, Sorcery, Other }
  public enum RarityTypes { Common, Uncommon, Rare, Mythic, Special, Bonus }

  protected int count = 1;

  [JsonConstructor]
  public MTGCard(MTGCardInfo info, int count = 1)
  {
    Info = info;
    Count = count;

    ColorType = Info.Colors.Length > 1 ? ColorTypes.M : Info.Colors[0];
    PrimarySpellType = Info.SpellTypes[0];
  }

  public ColorTypes ColorType { get; }
  public SpellType PrimarySpellType { get; }

  [ObservableProperty] private MTGCardInfo info;

  /// <summary>
  /// Name of the API, that was used to fetch this card
  /// </summary>
  public string APIName => Info.APIName;

  /// <summary>
  /// Card count. Minimum is 1
  /// </summary>
  public int Count
  {
    get => count;
    set
    {
      value = Math.Max(1, value);
      if (count != value)
      {
        count = value;
        OnPropertyChanged(nameof(Count));
      }
    }
  }

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
    public Guid OracleId { get; }
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
    public string APIName { get; }

    /// <summary>
    /// Constructor for JSON deserialization
    /// </summary>
    [JsonConstructor, Obsolete("This constructor should only be used by JSON deserializer")]
    public MTGCardInfo(Guid scryfallId, string name, int cmc, string typeLine, string setCode, string setName, float price, string collectorNumber, string aPIWebsiteUri, string setIconUri, CardFace frontFace, CardFace? backFace, RarityTypes rarityType, ColorTypes[] colors, SpellType[] spellTypes, string cardMarketUri, ColorTypes[] producedMana, string printSearchUri, CardToken[] tokens, string apiName, Guid oracleId)
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
    public MTGCardInfo(Guid scryfallId, CardFace frontFace, CardFace? backFace, int cmc, string name, string typeLine, string setCode, string setName, float price, string collectorNumber, string apiWebsiteUri, string setIconUri, ColorTypes[] producedMana, RarityTypes rarityType, string printSearchUri, string cardMarketUri, CardToken[] tokens, Guid oracleId, string apiName = "")
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
  }

  /// <summary>
  /// Returns all the <see cref="ColorTypes"/> that the given faces have
  /// </summary>
  public static ColorTypes[] GetColors(CardFace frontFace, CardFace? backFace)
  {
    var colors = new List<ColorTypes>();

    foreach (var color in frontFace.Colors)
    {
      if (!colors.Contains(color)) { colors.Add(color); }
    }

    if (backFace != null)
    {
      foreach (var color in backFace?.Colors)
      {
        if (!colors.Contains(color)) { colors.Add(color); }
      }
    }

    // Card is colorless if it has no other colors
    if (colors.Count == 0) { colors.Add(ColorTypes.C); }
    return colors.ToArray();
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

    if (types.Count == 0) { types.Add(SpellType.Other); }
    return types.OrderBy(x => x).ToArray();
  }
}