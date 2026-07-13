using Microsoft.Extensions.Caching.Memory;
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
  public IMemoryCache? Cache { get; init; } = null;

  /// <exception cref="ArgumentNullException"></exception>
  /// <exception cref="InvalidOperationException"></exception>
  /// <exception cref="System.Net.Http.HttpRequestException"></exception>
  /// <exception cref="UriFormatException"></exception>
  public async Task<DeckEditorMTGDeck> Convert(MTGCardDeckDTO dto)
  {
    ArgumentNullException.ThrowIfNull(dto);

    CardImportResult.Card? commander = null;
    CardImportResult.Card? partner = null;
    var deckCards = new List<CardImportResult.Card>();
    var wishCards = new List<CardImportResult.Card>();
    var maybeCards = new List<CardImportResult.Card>();
    var removeCards = new List<CardImportResult.Card>();

    var deckCache = Cache?.GetOrCreate<IMemoryCache>(dto.Name, (_) => new MemoryCache(new MemoryCacheOptions()));

    await Task.WhenAll(
    [
      Task.Run(async () => commander = dto.Commander == null ? null : (await FetchCards([dto.Commander], deckCache, DeckCacheKey.Commander)).FirstOrDefault()),
      Task.Run(async () => partner = dto.CommanderPartner == null ? null : (await FetchCards([dto.CommanderPartner], deckCache, DeckCacheKey.Partner)).FirstOrDefault()),
      Task.Run(async () => deckCards.AddRange((await FetchCards([.. dto.DeckCards], deckCache, DeckCacheKey.DeckCards)))),
      Task.Run(async () => wishCards.AddRange((await FetchCards([.. dto.WishlistCards], deckCache, DeckCacheKey.Wishlist)))),
      Task.Run(async () => maybeCards.AddRange((await FetchCards([.. dto.MaybelistCards], deckCache, DeckCacheKey.Maybelist)))),
      Task.Run(async () => removeCards.AddRange((await FetchCards([.. dto.RemovelistCards], deckCache, DeckCacheKey.Removelist)))),
    ]);

    return new DeckEditorMTGDeck()
    {
      Name = dto.Name,
      Commander = commander != null ? new DeckEditorMTGCard(commander.Info) { Count = commander.Count } : null,
      CommanderPartner = partner != null ? new DeckEditorMTGCard(partner.Info) { Count = partner.Count } : null,
      DeckCards = [.. deckCards.Select(x => GetDeckCard(x, dto.DeckCards))],
      Wishlist = [.. wishCards.Select(x => new DeckEditorMTGCard(x.Info) { Count = x.Count })],
      Maybelist = [.. maybeCards.Select(x => new DeckEditorMTGCard(x.Info) { Count = x.Count })],
      Removelist = [.. removeCards.Select(x => new DeckEditorMTGCard(x.Info) { Count = x.Count })],
    };
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

  private async Task<IEnumerable<CardImportResult.Card>> FetchCards(IEnumerable<MTGCardDTO> cards, IMemoryCache? cache, DeckCacheKey cacheKey)
  {
    if (cache?.Get(cacheKey) is IEnumerable<CardImportResult.Card> cachedCards)
      return cachedCards;

    var result = (await new FetchCardsWithDTOs(importer).Execute(cards)).Found;

    cache?.Set(cacheKey, result);

    return result;
  }
}