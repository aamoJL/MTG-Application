using MTGApplication.General.Models;
using System.Collections.Generic;

namespace MTGApplication.Features.DeckTesting.Models;

public record DeckTestingDeck(
  List<MTGCard> DeckCards, 
  MTGCard Commander, 
  MTGCard Partner, 
  List<MTGCard> Tokens)
{ }
