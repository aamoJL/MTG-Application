using MTGApplication.Features.DeckEditor.Editor.Models;
using MTGApplication.General.Services.Databases.Repositories.CardRepository.Models;

namespace MTGApplication.Features.DeckEditor.Editor.Services.Converters;

public class DeckEditorMTGCardToDTOConverter
{
  public static MTGCardDTO Convert(DeckEditorMTGCard card)
  {
    return new MTGCardDTO(
      name: card.Info.Name,
      count: card.Count,
      scryfallId: card.Info.ScryfallId,
      oracleId: card.Info.OracleId,
      setCode: card.Info.SetCode,
      collectorNumber: card.Info.CollectorNumber,
      group: card.Group);
  }
}