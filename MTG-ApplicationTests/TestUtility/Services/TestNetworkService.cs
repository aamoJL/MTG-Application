using MTGApplication.General.Services.IOServices;

namespace MTGApplicationTests.TestUtility.Services;

public class TestNetworkService : INetworkService
{
  public Func<string, Task<bool>> OpenAction { get; init; } = (_) => throw new NotImplementedException();

  public async Task<bool> OpenUri(string uri) => await OpenAction(uri);
}