using MTGApplication.General.Models.CardDeck;
using MTGApplication.General.ViewModels;
using System.Threading.Tasks;

namespace MTGApplication.General.Databases.Repositories.DeckRepository;

public class DeckExists(IRepository<MTGCardDeckDTO> repository) : UseCase<string, Task<bool>>
{
  public async override Task<bool> Execute(string name) => await repository.Exists(name);
}
