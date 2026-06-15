namespace MTGApplication.General.Services.API.CardAPI;

public partial class ScryfallAPI
{
  public enum RateLimit : int
  {
    SEARCH = 500,
    NAMED = 500,
    COLLECTION = 500,
    OTHER = 100,
  }
}