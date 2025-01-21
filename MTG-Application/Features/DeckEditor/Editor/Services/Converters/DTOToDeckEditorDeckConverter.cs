using MTGApplication.Features.DeckEditor.Editor.Models;
using MTGApplication.Features.DeckEditor.Models;
using MTGApplication.General.Services.Databases.Repositories.DeckRepository.Models;
using MTGApplication.General.Services.Importers.CardImporter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MTGApplication.Features.DeckEditor.Editor.Services.Converters;

public class DTOToDeckEditorDeckConverter(IMTGCardImporter importer)
{
  /// <exception cref="ArgumentNullException"></exception>
  /// <exception cref="InvalidOperationException"></exception>
  /// <exception cref="System.Net.Http.HttpRequestException"></exception>
  /// <exception cref="UriFormatException"></exception>
  public async Task<DeckEditorMTGDeck> Convert(MTGCardDeckDTO dto)
  {
    try
    {
      if (dto == null)
        throw new ArgumentNullException(nameof(dto), "DTO is null");

      CardImportResult.Card? commander = null;
      CardImportResult.Card? partner = null;
      var deckCards = new List<CardImportResult.Card>();
      var wishCards = new List<CardImportResult.Card>();
      var maybeCards = new List<CardImportResult.Card>();
      var removeCards = new List<CardImportResult.Card>();

      await Task.WhenAll(
      [
        Task.Run(async () => commander = dto.Commander != null ? (await importer.ImportWithDTOs([dto.Commander])).Found.FirstOrDefault() : null),
        Task.Run(async () => partner = dto.CommanderPartner != null ? (await importer.ImportWithDTOs([dto.CommanderPartner])).Found.FirstOrDefault() : null),
        Task.Run(async () => deckCards.AddRange((await importer.ImportWithDTOs([.. dto.DeckCards])).Found)),
        Task.Run(async () => wishCards.AddRange((await importer.ImportWithDTOs([.. dto.WishlistCards])).Found)),
        Task.Run(async () => maybeCards.AddRange((await importer.ImportWithDTOs([.. dto.MaybelistCards])).Found)),
        Task.Run(async () => removeCards.AddRange((await importer.ImportWithDTOs([.. dto.RemovelistCards])).Found)),
      ]);

      return new DeckEditorMTGDeck()
      {
        Name = dto.Name,
        Commander = commander != null ? new DeckEditorMTGCard(commander.Info, commander.Count) : null,
        CommanderPartner = partner != null ? new DeckEditorMTGCard(partner.Info, partner.Count) : null,
        DeckCards = [.. deckCards.Select(x => new DeckEditorMTGCard(x.Info, x.Count) { Group = x.Group })],
        Wishlist = [.. wishCards.Select(x => new DeckEditorMTGCard(x.Info, x.Count))],
        Maybelist = [.. maybeCards.Select(x => new DeckEditorMTGCard(x.Info, x.Count))],
        Removelist = [.. removeCards.Select(x => new DeckEditorMTGCard(x.Info, x.Count))],
      };
    }
    catch { throw; }
  }
}