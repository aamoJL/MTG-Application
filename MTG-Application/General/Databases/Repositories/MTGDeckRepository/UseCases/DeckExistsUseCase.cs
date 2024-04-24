using MTGApplication.General.UseCases;
using MTGApplication.Models.DTOs;
using System.Threading.Tasks;

namespace MTGApplication.General.Databases.Repositories.MTGDeckRepository;

public class DeckExistsUseCase : UseCase<string, Task<bool>>
{
  public DeckExistsUseCase(IRepository<MTGCardDeckDTO> repository) => Repository = repository;

  public IRepository<MTGCardDeckDTO> Repository { get; init; }

  public async override Task<bool> Execute(string name) => await Repository.Exists(name);
}
