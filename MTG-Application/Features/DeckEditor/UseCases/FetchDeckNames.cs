using Microsoft.EntityFrameworkCore;
using MTGApplication.General.Services.Databases.Repositories;
using MTGApplication.General.Services.Databases.Repositories.DeckRepository.Models;
using MTGApplication.General.Services.Databases.Repositories.DeckRepository.UseCases;
using MTGApplication.General.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MTGApplication.Features.DeckEditor.UseCases;

public class FetchDeckNames(IRepository<MTGCardDeckDTO> repository) : UseCaseFunc<Task<IEnumerable<string>>>
{
  public override async Task<IEnumerable<string>> Execute()
  {
    var decks = (await new GetDeckDTOs(repository) { SetIncludes = _ => { } }.Execute()) ?? throw new Exception("Error: Could not get decks.");

    return decks.Select(x => x.Name).Order();
  }
}