using MTGApplication.General.Models.Card;
using System;
using static MTGApplication.General.Models.Card.MTGCard;

namespace MTGApplication.General.Services.API.CardAPI;

public partial class ScryfallAPI
{
  /// <summary>
  /// Scryfall collection fetch identifier object.
  /// Scryfall documentation: <see href="https://scryfall.com/docs/api/cards/collection"/>
  /// </summary>
  public readonly struct ScryfallIdentifier
  {
    public enum IdentifierSchema { ID, ILLUSTRATION_ID, NAME, NAME_SET, COLLECTORNUMBER_SET }

    public ScryfallIdentifier() { }
    public ScryfallIdentifier(MTGCardDTO card)
    {
      if (card == null) return;

      ScryfallId = card.ScryfallId;
      Name = card.Name;
      CardCount = card.Count;
      SetCode = card.SetCode;
      CollectorNumber = card.CollectorNumber;
    }

    public Guid ScryfallId { get; init; } = Guid.Empty;
    public int CardCount { get; init; } = 1;
    public string Name { get; init; } = string.Empty;
    public Guid IllustrationId { get; init; } = Guid.Empty;
    public string SetCode { get; init; } = string.Empty;
    public string CollectorNumber { get; init; } = string.Empty;
    public IdentifierSchema PreferedSchema { get; init; } = IdentifierSchema.ID;

    /// <summary>
    /// Return object that contains the scryfall API identifier variables. This method should only be used for JSON serialization.
    /// </summary>
    public object ToObject()
    {
      switch (PreferedSchema)
      {
        case IdentifierSchema.ID:
          if (ScryfallId != Guid.Empty) { return new { id = ScryfallId }; }
          break;
        case IdentifierSchema.ILLUSTRATION_ID:
          if (ScryfallId != Guid.Empty && IllustrationId != Guid.Empty) { return new { illustration_id = IllustrationId }; }
          break;
        case IdentifierSchema.NAME:
          if (!string.IsNullOrEmpty(Name)) { return new { name = Name }; }
          break;
        case IdentifierSchema.NAME_SET:
          if (!string.IsNullOrEmpty(Name) && !string.IsNullOrEmpty(SetCode)) { return new { name = Name, set = SetCode }; }
          break;
        case IdentifierSchema.COLLECTORNUMBER_SET:
          if (!string.IsNullOrEmpty(CollectorNumber) && !string.IsNullOrEmpty(SetCode)) { return new { set = SetCode, collector_number = CollectorNumber }; }
          break;
        default: break;
      }

      // If prefered schema does not work, select secondary if possible
      // Scryfall Id
      if (ScryfallId != Guid.Empty) { return new { id = ScryfallId }; }
      // Set Code + Collector Number
      else if (!string.IsNullOrEmpty(SetCode) && !string.IsNullOrEmpty(CollectorNumber)) { return new { set = SetCode, collector_number = CollectorNumber }; }
      // Illustration Id
      else if (ScryfallId != Guid.Empty && IllustrationId != Guid.Empty) { return new { illustration_id = IllustrationId }; }
      // Name + Set Code
      else if (!string.IsNullOrEmpty(Name) && !string.IsNullOrEmpty(SetCode)) { return new { name = Name, set = SetCode }; }
      // Name
      else if (!string.IsNullOrEmpty(Name)) { return new { name = Name }; }
      else { return string.Empty; }
    }

    /// <summary>
    /// Returns true, if the identifier applies to the given <paramref name="info"/>
    /// </summary>
    public bool Compare(MTGCardInfo? info)
    {
      if (ScryfallId != Guid.Empty) { return info?.ScryfallId == ScryfallId; }
      else if (!string.IsNullOrEmpty(Name) && !string.IsNullOrEmpty(SetCode)) { return string.Equals(info?.FrontFace.Name, Name, StringComparison.OrdinalIgnoreCase) && string.Equals(info?.SetCode, SetCode); }
      else if (!string.IsNullOrEmpty(Name)) { return string.Equals(info?.FrontFace.Name, Name, StringComparison.OrdinalIgnoreCase); }
      else { return false; }
    }
  }
}