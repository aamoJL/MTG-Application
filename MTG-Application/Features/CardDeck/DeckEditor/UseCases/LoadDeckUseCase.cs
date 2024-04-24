using MTGApplication.API.CardAPI;
using MTGApplication.General.Databases.Repositories;
using MTGApplication.General.Databases.Repositories.MTGDeckRepository;
using MTGApplication.General.Extensions;
using MTGApplication.General.UseCases;
using MTGApplication.General.ViewModels;
using MTGApplication.Models;
using MTGApplication.Models.DTOs;
using System.Linq;
using System.Threading.Tasks;

namespace MTGApplication.Features.CardDeck;

public class LoadDeckUseCase : UseCase<string, Task<MTGCardDeck>>
{
  public record Args(string SaveName);

  public LoadDeckUseCase(IRepository<MTGCardDeckDTO> repository, ICardAPI<MTGCard> cardAPI)
  {
    Repository = repository;
    CardAPI = cardAPI;
  }

  public IRepository<MTGCardDeckDTO> Repository { get; }
  public ICardAPI<MTGCard> CardAPI { get; }

  public Confirmation<string, string[]> LoadConfirmation { get; set; } = new();
  public IWorker Worker { get; set; }

  public override async Task<MTGCardDeck> Execute(string loadName)
  {
    loadName ??= await LoadConfirmation.Confirm(
      title: "Open deck",
      message: "Name",
      data: (await Repository.Get(ExpressionExtensions.EmptyArray<MTGCardDeckDTO>())).Select(x => x.Name).OrderBy(x => x).ToArray());

    if (loadName is not null)
    {
      var loadTask = new GetDeckUseCase(Repository, CardAPI).Execute(loadName);

      return Worker != null ? await Worker.DoWork(loadTask) : await loadTask;
    }
    else return null;
  }
}