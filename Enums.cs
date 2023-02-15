using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTGApplication
{
  public static class Enums
  {
    public enum SortDirection { ASC, DESC }
    public enum SortMTGProperty { CMC, Name, Rarity, Color, Set, Count, Price }
    public enum CardlistType { Deck, Wishlist, Maybelist }
  }
}
