using MTGApplication.General.ViewModels;
using System.Threading.Tasks;

namespace MTGApplication.General.Services.IOService;

public class OpenUri : UseCase<string, Task>
{
  public async override Task Execute(string uri) => await NetworkService.OpenUri(uri);
}
