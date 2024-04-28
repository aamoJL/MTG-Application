using MTGApplication.General.UseCases;
using System.Threading.Tasks;

namespace MTGApplication.General.Services.IOService;

public class OpenUri : UseCase<string, Task>
{
  public async override Task Execute(string uri) => await IOService.OpenUri(uri);
}
