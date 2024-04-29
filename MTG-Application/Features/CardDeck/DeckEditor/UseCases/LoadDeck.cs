using MTGApplication.General.Databases.Repositories;
using MTGApplication.General.Databases.Repositories.MTGDeckRepository;
using MTGApplication.General.Extensions;
using MTGApplication.General.Models.Card;
using MTGApplication.General.Models.CardDeck;
using MTGApplication.General.Services.API.CardAPI;
using MTGApplication.General.Services.ConfirmationService;
using MTGApplication.General.UseCases;
using MTGApplication.General.ViewModels;
using System.Linq;
using System.Threading.Tasks;
using static MTGApplication.General.Services.ConfirmationService.ConfirmationService;

namespace MTGApplication.Features.CardDeck;

public class LoadDeck : UseCase<Task<LoadDeck.ReturnArgs>>
{
  public record ReturnArgs(MTGCardDeck Deck, ConfirmationResult ConfirmResult);

  public LoadDeck(IRepository<MTGCardDeckDTO> repository, ICardAPI<MTGCard> cardAPI)
  {
    Repository = repository;
    CardAPI = cardAPI;
  }

  public IRepository<MTGCardDeckDTO> Repository { get; }
  public ICardAPI<MTGCard> CardAPI { get; }

  public Confirmer<string, string[]> LoadConfirmation { get; set; } = new();
  public IWorker Worker { get; set; } = new DefaultWorker();

  public override async Task<ReturnArgs> Execute()
  {
    var loadName = await LoadConfirmation.Confirm(new(
      Title: "Open deck",
      Message: "Name",
      Data: (await Repository.Get(ExpressionExtensions.EmptyArray<MTGCardDeckDTO>())).Select(x => x.Name).OrderBy(x => x).ToArray()));

    return await Load(loadName);
  }

  public async Task<ReturnArgs> Execute(string loadName) => await Load(loadName);

  private async Task<ReturnArgs> Load(string loadName)
  {
    switch (!string.IsNullOrEmpty(loadName))
    {
      case true:
        var loadResult = await Worker.DoWork(new GetDeckUseCase(Repository, CardAPI).Execute(loadName));
        return new(loadResult, ConfirmationExtensions.FailureFromNull(loadResult));
      case false:
      default: return new(Deck: null, ConfirmResult: ConfirmationResult.Cancel);
    }
  }
}