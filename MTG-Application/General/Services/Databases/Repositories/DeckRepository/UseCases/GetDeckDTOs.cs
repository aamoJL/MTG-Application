using Microsoft.EntityFrameworkCore;
using MTGApplication.General.Services.Databases.Repositories.DeckRepository.Models;
using MTGApplication.General.ViewModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MTGApplication.General.Services.Databases.Repositories.DeckRepository.UseCases;
public class GetDeckDTOs(IRepository<MTGCardDeckDTO> repository) : UseCase<Task<IEnumerable<MTGCardDeckDTO>>>
{
  public Action<DbSet<MTGCardDeckDTO>>? SetIncludes { get; init; }

  public override async Task<IEnumerable<MTGCardDeckDTO>> Execute() => await repository.Get(SetIncludes);
}