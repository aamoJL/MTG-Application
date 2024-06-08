using MTGApplication.General.Services.Databases.Repositories.DeckRepository.Models;
using MTGApplication.General.ViewModels;
using System.Threading.Tasks;

namespace MTGApplication.General.Services.Databases.Repositories.DeckRepository.UseCases;

public class AddOrUpdateDeckDTO(IRepository<MTGCardDeckDTO> repository) : UseCase<(MTGCardDeckDTO deck, string saveName), Task<bool>>
{
  public override async Task<bool> Execute((MTGCardDeckDTO deck, string saveName) args)
  {
    var (deck, saveName) = args;

    return await repository.AddOrUpdate(deck with { Name = saveName });
  }
}
