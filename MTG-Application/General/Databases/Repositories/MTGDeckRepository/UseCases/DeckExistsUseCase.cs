using MTGApplication.General.UseCases;
using MTGApplication.Models.DTOs;
using System.Threading.Tasks;

namespace MTGApplication.General.Databases.Repositories.MTGDeckRepository;

public class DeckExistsUseCase : UseCase<Task<bool>>
{
  public DeckExistsUseCase(string name, IRepository<MTGCardDeckDTO> repository)
  {
    Name = name;
    Repository = repository;
  }

  public string Name { get; }
  public IRepository<MTGCardDeckDTO> Repository { get; init; }

  public async override Task<bool> Execute() => await Repository.Exists(Name);
}
