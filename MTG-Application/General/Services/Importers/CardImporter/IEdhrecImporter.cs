using System.Threading.Tasks;

namespace MTGApplication.General.Services.Importers.CardImporter;

public interface IEdhrecImporter
{
  public Task<string[]> FetchNewCardNames(string uri);
  public Task<CommanderTheme[]> GetThemes(string commander, string? partner = null);
  public bool TryParseCardNameFromEdhrecUri(string uri, out string? name);
}
