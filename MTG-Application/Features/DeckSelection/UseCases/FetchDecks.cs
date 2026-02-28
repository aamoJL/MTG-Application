using Microsoft.EntityFrameworkCore;
using MTGApplication.Features.DeckSelection.Models;
using MTGApplication.General.Services.Databases.Repositories;
using MTGApplication.General.Services.Databases.Repositories.CardRepository.Models;
using MTGApplication.General.Services.Databases.Repositories.DeckRepository.Models;
using MTGApplication.General.Services.Databases.Repositories.DeckRepository.UseCases;
using MTGApplication.General.Services.Importers.CardImporter;
using MTGApplication.General.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static MTGApplication.General.Models.MTGCardInfo;

namespace MTGApplication.Features.DeckSelection.UseCases;

public class FetchDecks(IRepository<MTGCardDeckDTO> repository, IMTGCardImporter importer) : UseCaseFunc<Task<IEnumerable<DeckSelectionDeck>>>
{
  /// <exception cref="Exception"></exception>
  public override async Task<IEnumerable<DeckSelectionDeck>> Execute()
  {
    var deckDTOs = (await new GetDeckDTOs(repository)
    {
      SetIncludes = (set) =>
      {
        set.Include(x => x.Commander).Load();
        set.Include(x => x.CommanderPartner).Load();
      },
    }.Execute()) ?? throw new Exception("Error: Could not get decks.");

    var commanderDTOs = new List<MTGCardDTO>([
      .. deckDTOs.Where(x => x.Commander != null).Select(x => x.Commander!),
      .. deckDTOs.Where(x => x.CommanderPartner != null).Select(x => x.CommanderPartner!)
      ]);

    var commanders = (await importer.ImportWithDTOs(commanderDTOs)).Found;

    return deckDTOs
      .Select(dto => GetDeck(dto, commanders))
      .OrderBy(x => x.Name);
  }

  private DeckSelectionDeck GetDeck(MTGCardDeckDTO dto, CardImportResult.Card[] commanders)
  {
    List<ColorTypes> colors = [];

    if (dto.Commander != null && commanders.FirstOrDefault(c => c.Info.ScryfallId == dto.Commander?.ScryfallId)?.Info.ColorIdentity is IList<ColorTypes> commanderColors)
    {
      colors.AddRange(commanderColors);

      if (dto.CommanderPartner != null && commanders.FirstOrDefault(c => c.Info.ScryfallId == dto.CommanderPartner?.ScryfallId)?.Info.ColorIdentity is IList<ColorTypes> partnerColors)
        colors.AddRange(partnerColors);

      if (colors.Count == 0)
        colors.Add(ColorTypes.C);
    }

    var imageUri = commanders.FirstOrDefault(c => c.Info.ScryfallId == dto.Commander?.ScryfallId)
      ?.Info.FrontFace.ArtCropUri ?? string.Empty;

    return new DeckSelectionDeck()
    {
      Name = dto.Name,
      ImageUri = imageUri,
      Colors = [.. colors.Distinct()],
    };
  }
}