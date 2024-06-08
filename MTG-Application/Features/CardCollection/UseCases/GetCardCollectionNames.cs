using MTGApplication.General.Services.Databases.Repositories;
using MTGApplication.General.Services.Databases.Repositories.CardCollectionRepository.Models;
using MTGApplication.General.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MTGApplication.Features.CardCollection.UseCases;
public class GetCardCollectionNames(IRepository<MTGCardCollectionDTO> repository) : UseCase<Task<IEnumerable<string>>>
{
  public IRepository<MTGCardCollectionDTO> Repository { get; } = repository;

  public override async Task<IEnumerable<string>> Execute()
    => (await Repository.Get([])).Select(x => x.Name).OrderBy(x => x);
}