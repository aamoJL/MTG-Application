using Microsoft.EntityFrameworkCore;
using MTGApplication.General.Services.Databases.Repositories.CardCollectionRepository.Models;
using MTGApplication.General.ViewModels;
using System;
using System.Threading.Tasks;

namespace MTGApplication.General.Services.Databases.Repositories.CardCollectionRepository.UseCases;
public class GetCardCollectionDTO(IRepository<MTGCardCollectionDTO> repository) : UseCase<string, Task<MTGCardCollectionDTO>>
{
  Action<DbSet<MTGCardCollectionDTO>> SetIncludes { get; init; }

  public async override Task<MTGCardCollectionDTO> Execute(string name)
    => string.IsNullOrEmpty(name) ? null : await repository.Get(name, SetIncludes);
}