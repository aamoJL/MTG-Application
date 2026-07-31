using MTGApplication.General.Services.Cache;
using MTGApplication.General.Services.Cache.Caches;
using MTGApplication.General.Services.Databases.Repositories.CardRepository.Models;
using MTGApplication.General.Services.Databases.Repositories.DeckRepository.Models;
using MTGApplication.General.Services.Importers.CardImporter;
using MTGApplication.General.Services.Importers.CardImporter.UseCases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MTGApplication.Features.DeckEditor.Models.Converters;

public class DTOToDeckEditorDeckConverter(IMTGCardImporter importer)
{
  public IMemoryCache<Caching.CacheKey>? Cache { get; init; } = null;

  /// <exception cref="ArgumentNullException"></exception>
  /// <exception cref="InvalidOperationException"></exception>
  /// <exception cref="System.Net.Http.HttpRequestException"></exception>
  /// <exception cref="UriFormatException"></exception>
  public async Task<DeckEditorMTGDeck> Convert(MTGCardDeckDTO dto)
  {
    ArgumentNullException.ThrowIfNull(dto);

    if (Cache?.TryGetDeck(dto.Name, out var cachedDeck) is true)
      return cachedDeck;

    CardImportResult.Card? commander = null;
    CardImportResult.Card? partner = null;
    var deckCards = new List<CardImportResult.Card>();
    var wishCards = new List<CardImportResult.Card>();
    var maybeCards = new List<CardImportResult.Card>();
    var removeCards = new List<CardImportResult.Card>();

    await Task.WhenAll(
    [
      Task.Run(async () => commander = dto.Commander == null ? null : (await new FetchCardsWithDTOs(importer).Execute([dto.Commander])).Found.FirstOrDefault()),
      Task.Run(async () => partner = dto.CommanderPartner == null ? null : (await new FetchCardsWithDTOs(importer).Execute([dto.CommanderPartner])).Found.FirstOrDefault()),
      Task.Run(async () => deckCards.AddRange((await new FetchCardsWithDTOs(importer).Execute([.. dto.DeckCards])).Found)),
      Task.Run(async () => wishCards.AddRange((await new FetchCardsWithDTOs(importer).Execute([.. dto.WishlistCards])).Found)),
      Task.Run(async () => maybeCards.AddRange((await new FetchCardsWithDTOs(importer).Execute([.. dto.MaybelistCards])).Found)),
      Task.Run(async () => removeCards.AddRange((await new FetchCardsWithDTOs(importer).Execute([.. dto.RemovelistCards])).Found)),
    ]);

    var deck = new DeckEditorMTGDeck()
    {
      Name = dto.Name,
      Commander = commander != null ? new DeckEditorMTGCard(commander.Info) { Count = commander.Count } : null,
      CommanderPartner = partner != null ? new DeckEditorMTGCard(partner.Info) { Count = partner.Count } : null,
      DeckCards = [.. deckCards.Select(x => GetDeckCard(x, dto.DeckCards))],
      Wishlist = [.. wishCards.Select(x => new DeckEditorMTGCard(x.Info) { Count = x.Count })],
      Maybelist = [.. maybeCards.Select(x => new DeckEditorMTGCard(x.Info) { Count = x.Count })],
      Removelist = [.. removeCards.Select(x => new DeckEditorMTGCard(x.Info) { Count = x.Count })],
    };

    Cache?.CacheDeck(deck);

    return deck;
  }

  private DeckEditorMTGCard GetDeckCard(CardImportResult.Card importCard, IEnumerable<MTGCardDTO> dtoDeckCards)
  {
    if (dtoDeckCards.FirstOrDefault(x => x.Name == importCard.Info.Name) is MTGCardDTO deckCard)
    {
      return new(importCard.Info)
      {
        Count = importCard.Count,
        Group = deckCard.Group,
        CardTag = deckCard.Tag,
      };
    }
    else return new(importCard.Info) { Count = importCard.Count };
  }
}