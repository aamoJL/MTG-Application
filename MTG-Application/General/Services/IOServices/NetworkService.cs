using System.Threading.Tasks;

namespace MTGApplication.General.Services.IOServices;

public class NetworkService : INetworkService
{
  public async Task<bool> OpenUri(string uri) => await NetworkIO.OpenUri(uri);
}
