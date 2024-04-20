namespace MTGApplication.Features.CardSearch;

public enum MTGSearchGameFormat
{
  Any, Modern, Standard, Commander,
}

public enum MTGSearchCardUniqueness
{
  Cards, Prints, Art
}

public enum MTGSearchOrderProperty
{
  Released, Set, CMC, Name, Rarity, Color, Eur
}

public enum MTGSearchOrderDirection
{
  Asc, Desc
}