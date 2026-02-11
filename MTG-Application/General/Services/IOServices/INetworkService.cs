using System.Threading.Tasks;

namespace MTGApplication.General.Services.IOServices;

public interface INetworkService
{
  public Task<bool> OpenUri(string uri);
}
