using Microsoft.EntityFrameworkCore;
using MTGApplication.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MTGApplication.ViewModels
{
  public partial class MTGDeckViewModel : SavableMTGCardCollectionViewModel
  {
    public MTGDeckViewModel(NamedMTGCardCollectionModel model) : base(model) { }

    public async Task<bool> DeleteDeckAsync()
    {
      using var db = new Database.CardCollectionContext();

      // Find deck and cards
      var deck = await db.CardDecks.SingleOrDefaultAsync(x => x.Name == Name);
      if (deck == null) { return false; }

      db.Entry(deck).Reference(x => x.Cardlist).Load();
      db.Entry(deck).Reference(x => x.Wishlist).Load();
      db.Entry(deck).Reference(x => x.Maybelist).Load();

      // Remove lists of cards and the deck
      if (deck.Cardlist != null) { db.ListsOfCards.Remove(deck.Cardlist); }
      if (deck.Wishlist != null) { db.ListsOfCards.Remove(deck.Wishlist); }
      if (deck.Maybelist != null) { db.ListsOfCards.Remove(deck.Maybelist); }
      db.CardDecks.Remove(deck);

      await db.SaveChangesAsync();
      return true;
    }

    protected override async Task<bool> OnSave(string name)
    {
      using var db = new Database.CardCollectionContext();

      // Find deck
      var deck = db.CardDecks.SingleOrDefault(x => x.Name == name);
      if (deck != null) db.Entry(deck).Reference(x => x.Cardlist).Load();

      // Create new deck if it does not exists
      deck ??= db.CardDecks.Add(new Database.CardDeck() { Name = name }).Entity;

      // Remove old cardlist
      if (deck.Cardlist != null) db.ListsOfCards.Remove(deck.Cardlist);

      // Create new list of cards
      deck.Cardlist = new Database.ListOfCards()
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
    protected override async Task<MTGCardModel[]> OnLoadAsync(string name)
    {
      using var db = new Database.CardCollectionContext();

      // Find deck and cards
      var deck = await db.CardDecks.SingleOrDefaultAsync(x => x.Name == name);
      if (deck == null) { return null; }

      var deckCards = deck != null ? await db.Cards.Where(x => x.CardListId == deck.CardlistId).ToArrayAsync() : Array.Empty<Database.Card>();

      return await App.CardAPI.FetchCollectionAsync(deckCards);
    }
    protected override async Task<bool> OnDeleteAsync(string name)
    {
      using var db = new Database.CardCollectionContext();

      // Find deck and cards
      var deck = await db.CardDecks.SingleOrDefaultAsync(x => x.Name == name);
      if (deck == null) { return false; }
      db.Entry(deck).Reference(x => x.Cardlist).Load();

      // Remove lists of cards and the deck
      if (deck.Cardlist != null) { db.ListsOfCards.Remove(deck.Cardlist); }

      await db.SaveChangesAsync();
      return true;
    }
  }
}
