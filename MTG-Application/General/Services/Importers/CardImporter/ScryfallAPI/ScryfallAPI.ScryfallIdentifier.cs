using MTGApplication.General.Models;
using MTGApplication.General.Services.Databases.Repositories.CardRepository.Models;
using System;

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
    /// Returns true, if the identifier applies to the given <paramref name="info"/>
    /// </summary>
    public bool Compare(MTGCardInfo info)
    {
      if (ScryfallId != Guid.Empty) { return info?.ScryfallId == ScryfallId; }
      else if (!string.IsNullOrEmpty(Name) && !string.IsNullOrEmpty(SetCode)) { return string.Equals(info?.FrontFace.Name, Name, StringComparison.OrdinalIgnoreCase) && string.Equals(info?.SetCode, SetCode); }
      else if (!string.IsNullOrEmpty(Name)) { return string.Equals(info?.FrontFace.Name, Name, StringComparison.OrdinalIgnoreCase); }
      else { return false; }
    }
  }
}