using MTGApplication.Features.DeckEditor.Editor.Models;
using MTGApplication.Features.DeckEditor.Models;
using MTGApplication.General.Models;
using MTGApplication.General.Services.Databases.Repositories.DeckRepository.Models;
using MTGApplication.General.Services.Importers.CardImporter;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MTGApplication.Features.DeckEditor.Editor.Services.Converters;

public class DTOToDeckEditorDeckConverter(MTGCardImporter importer)
{
  public async Task<DeckEditorMTGDeck> Convert(MTGCardDeckDTO dto)
  {
    if (dto == null) return null;

    CardImportResult<MTGCardInfo>.Card commander = null;
    CardImportResult<MTGCardInfo>.Card partner = null;
    var deckCards = new List<CardImportResult<MTGCardInfo>.Card>();
    var wishCards = new List<CardImportResult<MTGCardInfo>.Card>();
    var maybeCards = new List<CardImportResult<MTGCardInfo>.Card>();
    var removeCards = new List<CardImportResult<MTGCardInfo>.Card>();

    await Task.WhenAll(
    [
      Task.Run(async () => commander = dto.Commander != null ? (await importer.ImportFromDTOs([dto.Commander])).Found.FirstOrDefault() : null),
      Task.Run(async () => partner = dto.CommanderPartner != null ? (await importer.ImportFromDTOs([dto.CommanderPartner])).Found.FirstOrDefault() : null),
      Task.Run(async () => deckCards.AddRange((await importer.ImportFromDTOs([.. dto.DeckCards])).Found)),
      Task.Run(async () => wishCards.AddRange((await importer.ImportFromDTOs([.. dto.WishlistCards])).Found)),
      Task.Run(async () => maybeCards.AddRange((await importer.ImportFromDTOs([.. dto.MaybelistCards])).Found)),
      Task.Run(async () => removeCards.AddRange((await importer.ImportFromDTOs([.. dto.RemovelistCards])).Found)),
    ]);

    return new DeckEditorMTGDeck()
    {
      Name = dto.Name,
      Commander = commander != null ? new DeckEditorMTGCard(commander.Info, commander.Count) : null,
      CommanderPartner = partner != null ? new DeckEditorMTGCard(partner.Info, partner.Count) : null,
      DeckCards = [.. deckCards.Select(x => new DeckEditorMTGCard(x.Info, x.Count))],
      Wishlist = [.. wishCards.Select(x => new DeckEditorMTGCard(x.Info, x.Count))],
      Maybelist = [.. maybeCards.Select(x => new DeckEditorMTGCard(x.Info, x.Count))],
      Removelist = [.. removeCards.Select(x => new DeckEditorMTGCard(x.Info, x.Count))],
    };
  }
}