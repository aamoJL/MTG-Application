using MTGApplication.General.ViewModels;
using System;
using System.Linq;
using System.Text.Json;
using static MTGApplication.General.Services.API.CardAPI.ScryfallAPI;
using static MTGApplication.General.Services.API.CardAPI.ScryfallAPI.ScryfallIdentifier;

namespace MTGApplication.General.Services.API.CardAPI;

public class ScryfallIdentifiersToJsonConverter : UseCase<ScryfallIdentifier[], string>
{
  public override string Execute(ScryfallIdentifier[] identifiers)
    => JsonSerializer.Serialize(new { identifiers = identifiers.Select(x => GetIdentifierJsonObject(x)) });

  private static object GetIdentifierJsonObject(ScryfallIdentifier identifier)
  {
    switch (identifier.PreferedSchema)
    {
      case IdentifierSchema.ID:
        if (identifier.ScryfallId != Guid.Empty) { return new { id = identifier.ScryfallId }; }
        break;
      case IdentifierSchema.ILLUSTRATION_ID:
        if (identifier.ScryfallId != Guid.Empty && identifier.IllustrationId != Guid.Empty) { return new { illustration_id = identifier.IllustrationId }; }
        break;
      case IdentifierSchema.NAME:
        if (!string.IsNullOrEmpty(identifier.Name)) { return new { name = identifier.Name }; }
        break;
      case IdentifierSchema.NAME_SET:
        if (!string.IsNullOrEmpty(identifier.Name) && !string.IsNullOrEmpty(identifier.SetCode)) { return new { name = identifier.Name, set = identifier.SetCode }; }
        break;
      case IdentifierSchema.COLLECTORNUMBER_SET:
        if (!string.IsNullOrEmpty(identifier.CollectorNumber) && !string.IsNullOrEmpty(identifier.SetCode)) { return new { set = identifier.SetCode, collector_number = identifier.CollectorNumber }; }
        break;
      default: break;
    }

    // If prefered schema does not work, select secondary if possible
    // Scryfall Id
    if (identifier.ScryfallId != Guid.Empty) { return new { id = identifier.ScryfallId }; }
    // Set Code + Collector Number
    else if (!string.IsNullOrEmpty(identifier.SetCode) && !string.IsNullOrEmpty(identifier.CollectorNumber)) { return new { set = identifier.SetCode, collector_number = identifier.CollectorNumber }; }
    // Illustration Id
    else if (identifier.ScryfallId != Guid.Empty && identifier.IllustrationId != Guid.Empty) { return new { illustration_id = identifier.IllustrationId }; }
    // Name + Set Code
    else if (!string.IsNullOrEmpty(identifier.Name) && !string.IsNullOrEmpty(identifier.SetCode)) { return new { name = identifier.Name, set = identifier.SetCode }; }
    // Name
    else if (!string.IsNullOrEmpty(identifier.Name)) { return new { name = identifier.Name }; }
    else { return string.Empty; }
  }
}