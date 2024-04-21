using MTGApplication.General;
using System.Threading.Tasks;

namespace MTGApplication.Services.IOService;

public class OpenUriUseCase : UseCase<Task>
{
  public OpenUriUseCase(string uri) => Uri = uri;

  public string Uri { get; }

  public async override Task Execute() => await IOService.OpenUri(Uri);
}
