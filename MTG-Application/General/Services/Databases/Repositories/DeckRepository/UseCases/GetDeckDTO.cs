using Microsoft.EntityFrameworkCore;
using MTGApplication.General.Services.Databases.Repositories.DeckRepository.Models;
using MTGApplication.General.ViewModels;
using System;
using System.Threading.Tasks;

namespace MTGApplication.General.Services.Databases.Repositories.DeckRepository.UseCases;

public class GetDeckDTO(IRepository<MTGCardDeckDTO> repository) : UseCase<string, Task<MTGCardDeckDTO?>>
{
  public Action<DbSet<MTGCardDeckDTO>>? SetIncludes { get; init; }

  public async override Task<MTGCardDeckDTO?> Execute(string name)
    => string.IsNullOrEmpty(name) ? null : await repository.Get(name, SetIncludes);
}