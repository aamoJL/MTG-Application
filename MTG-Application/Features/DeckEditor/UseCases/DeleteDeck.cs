using MTGApplication.Features.DeckEditor.Models;
using MTGApplication.Features.DeckEditor.Models.Converters;
using MTGApplication.General.Services.Databases.Repositories;
using MTGApplication.General.Services.Databases.Repositories.DeckRepository.Models;
using MTGApplication.General.Services.Databases.Repositories.DeckRepository.UseCases;
using MTGApplication.General.ViewModels;
using System.Threading.Tasks;

namespace MTGApplication.Features.DeckEditor.UseCases;

public class DeleteDeck(IRepository<MTGCardDeckDTO> repository) : UseCaseFunc<DeckEditorMTGDeck, Task<bool>>
{
  public override async Task<bool> Execute(DeckEditorMTGDeck deck)
  {
    var dto = DeckEditorMTGDeckToDTOConverter.Convert(deck);

    return await new DeleteDeckDTO(repository).Execute(dto);
  }
}
