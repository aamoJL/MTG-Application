using MTGApplication.Features.DeckEditor.Models;
using MTGApplication.General.Services.Databases.Repositories.DeckRepository.Models;
using System.Linq;

namespace MTGApplication.Features.DeckEditor.Editor.Services.Converters;

public class DeckEditorMTGDeckToDTOConverter
{
  public static MTGCardDeckDTO Convert(DeckEditorMTGDeck deck)
  {
    return new MTGCardDeckDTO(
      name: deck.Name,
      commander: deck.Commander != null ? DeckEditorMTGCardToDTOConverter.Convert(deck.Commander) : null,
      partner: deck.CommanderPartner != null ? DeckEditorMTGCardToDTOConverter.Convert(deck.CommanderPartner) : null,
      deckCards: [.. deck.DeckCards.Select(DeckEditorMTGCardToDTOConverter.Convert)],
      wishlistCards: [.. deck.Wishlist.Select(DeckEditorMTGCardToDTOConverter.Convert)],
      maybelistCards: [.. deck.Maybelist.Select(DeckEditorMTGCardToDTOConverter.Convert)],
      removelistCards: [.. deck.Removelist.Select(DeckEditorMTGCardToDTOConverter.Convert)]);
  }
}
