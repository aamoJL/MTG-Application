using MTGApplication.General.ViewModels;
using System;
using System.Linq;
using System.Text.Json;
using static MTGApplication.General.Services.API.CardAPI.ScryfallAPI;
using static MTGApplication.General.Services.API.CardAPI.ScryfallAPI.ScryfallIdentifier;

namespace MTGApplication.General.Services.Importers.CardImporter.UseCases;

public class ScryfallIdentifiersToJsonConverter : UseCase<ScryfallIdentifier[], string>
{
  public override string Execute(ScryfallIdentifier[] identifiers)
    => GetJson(identifiers);

  private static string GetJson(ScryfallIdentifier[] identifiers)
  {
    return JsonSerializer.Serialize(new
    {
      identifiers = identifiers.Select<ScryfallIdentifier, object>(identifier =>
    {
      var schemaPriorityEnumerator = new IdentifierSchema[]
    {
      identifier.PreferedSchema,
      IdentifierSchema.ID,
      IdentifierSchema.COLLECTORNUMBER_SET,
      IdentifierSchema.ILLUSTRATION_ID,
      IdentifierSchema.NAME_SET,
      IdentifierSchema.NAME
    }.GetEnumerator();

      while (schemaPriorityEnumerator.MoveNext())
      {
        switch ((IdentifierSchema)schemaPriorityEnumerator.Current)
        {
          case IdentifierSchema.ID:
            if (identifier.ScryfallId != Guid.Empty)
              return new { id = identifier.ScryfallId };
            break;
          case IdentifierSchema.ILLUSTRATION_ID:
            if (identifier.ScryfallId != Guid.Empty && identifier.IllustrationId != Guid.Empty)
              return new { illustration_id = identifier.IllustrationId };
            break;
          case IdentifierSchema.NAME:
            if (!string.IsNullOrEmpty(identifier.Name))
              return new { name = identifier.Name };
            break;
          case IdentifierSchema.NAME_SET:
            if (!string.IsNullOrEmpty(identifier.Name) && !string.IsNullOrEmpty(identifier.SetCode))
              return new { name = identifier.Name, set = identifier.SetCode };
            break;
          case IdentifierSchema.COLLECTORNUMBER_SET:
            if (!string.IsNullOrEmpty(identifier.CollectorNumber) && !string.IsNullOrEmpty(identifier.SetCode))
              return new { set = identifier.SetCode, collector_number = identifier.CollectorNumber };
            break;
          default: break;
        }
      }

      return string.Empty;
    })
    });
  }
}