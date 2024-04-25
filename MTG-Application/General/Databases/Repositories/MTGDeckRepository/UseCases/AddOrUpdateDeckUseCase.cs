using MTGApplication.General.UseCases;
using MTGApplication.Models;
using MTGApplication.Models.DTOs;
using System.Threading.Tasks;

namespace MTGApplication.General.Databases.Repositories.MTGDeckRepository;

public class AddOrUpdateDeckUseCase : UseCase<(MTGCardDeck deck, string saveName), Task<bool>>
{
  public AddOrUpdateDeckUseCase(IRepository<MTGCardDeckDTO> repository) => Repository = repository;

  public IRepository<MTGCardDeckDTO> Repository { get; }

  public override async Task<bool> Execute((MTGCardDeck deck, string saveName) args)
  {
    var (deck, saveName) = args;

    return await Repository.AddOrUpdate(new MTGCardDeckDTO(deck) { Name = saveName });
  }
}
