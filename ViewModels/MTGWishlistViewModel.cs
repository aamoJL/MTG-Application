using Microsoft.EntityFrameworkCore;
using MTGApplication.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MTGApplication.ViewModels
{
  public class MTGWishlistViewModel : SavableMTGCardCollectionViewModel
  {
    public MTGWishlistViewModel(NamedMTGCardCollectionModel model) : base(model) { }

    protected override async Task<bool> OnDeleteAsync(string name)
    {
      using var db = new Database.CardCollectionContext();

      // Find deck and cards
      var deck = await db.CardDecks.SingleOrDefaultAsync(x => x.Name == name);
      if (deck == null) { return false; }
      db.Entry(deck).Reference(x => x.Wishlist).Load();

      // Remove lists of cards and the deck
      if (deck.Wishlist != null) { db.ListsOfCards.Remove(deck.Wishlist); }

      await db.SaveChangesAsync();
      return true;
    }

    protected override async Task<MTGCardModel[]> OnLoadAsync(string name)
    {
      using var db = new Database.CardCollectionContext();

      // Find deck and cards
      var deck = await db.CardDecks.SingleOrDefaultAsync(x => x.Name == name);
      if (deck == null) { return null; }

      var deckCards = deck != null ? await db.Cards.Where(x => x.CardListId == deck.WishlistId).ToArrayAsync() : Array.Empty<Database.Card>();

      return await App.CardAPI.FetchCollectionAsync(deckCards);
    }
    protected override async Task<bool> OnSave(string name)
    {
      using var db = new Database.CardCollectionContext();

      // Find deck
      var deck = db.CardDecks.SingleOrDefault(x => x.Name == name);
      if (deck != null) db.Entry(deck).Reference(x => x.Wishlist).Load();

      if(deck == null) { return false; }

      // Remove old cardlist
      if (deck.Wishlist != null) db.ListsOfCards.Remove(deck.Wishlist);

      // Create new list of cards
      deck.Wishlist = new Database.ListOfCards()
      {
        Cards = CardModels.Select(card => new Database.Card()
        {
          Count = card.Count,
          Name = card.Info.Name,
          ScryfallId = card.Info.Id
        }).ToList()
      };

      // Save to database
      await db.SaveChangesAsync();
      return true;
    }
  }
}
