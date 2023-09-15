using MTGApplication.Models.Structs;
using System.Threading.Tasks;

namespace MTGApplication.Interfaces;

/// <summary>
/// Interface for MTG Commander card search APIs
/// </summary>
public interface IMTGCommanderAPI
{
  /// <summary>
  /// Returns the available commander deck themes for the given commanders
  /// </summary>
  public Task<CommanderTheme[]> GetThemes(Commanders commanders);

  /// <summary>
  /// Retrns array of card names that are in the gicen theme and has been marked as new by the API
  /// </summary>
  public Task<string[]> FetchNewCards(string uri);
}
