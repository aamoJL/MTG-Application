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

public class GetDeckSelectionDecks(
  IRepository<MTGCardDeckDTO> repository,
  IMTGCardImporter importer) : UseCase<Task<IEnumerable<DeckSelectionDeck>>>
{
  public IRepository<MTGCardDeckDTO> Repository { get; } = repository;
  public IMTGCardImporter Importer { get; } = importer;

  /// <exception cref="Exception"></exception>
  public override async Task<IEnumerable<DeckSelectionDeck>> Execute()
  {
    try
    {
      var deckDTOs = (await new GetDeckDTOs(Repository)
      {
        SetIncludes = (set) =>
        {
          set.Include(x => x.Commander).Load();
          set.Include(x => x.CommanderPartner).Load();
        },
      }.Execute()).OrderBy(x => x.Name) ?? throw new Exception("Error: Could not get decks.");

      var commanderDTOs = new List<MTGCardDTO>();

      commanderDTOs.AddRange(deckDTOs.Where(x => x.Commander != null).Select(x => x.Commander!));
      commanderDTOs.AddRange(deckDTOs.Where(x => x.CommanderPartner != null).Select(x => x.CommanderPartner!));

      var commanders = (await Importer.ImportWithDTOs(commanderDTOs)).Found;

      return deckDTOs.Select(dto =>
      {
        List<ColorTypes> colors = [];

        if (dto.Commander != null && commanders.FirstOrDefault(c => c.Info.ScryfallId == dto.Commander?.ScryfallId)?.Info.ColorIdentity is ColorTypes[] commanderColors)
        {
          colors.AddRange(commanderColors);

          if (dto.CommanderPartner != null && commanders.FirstOrDefault(c => c.Info.ScryfallId == dto.CommanderPartner?.ScryfallId)?.Info.ColorIdentity is ColorTypes[] partnerColors)
            colors.AddRange(partnerColors);

          if (colors.Count == 0)
            colors.Add(ColorTypes.C);
        }

        var imageUri = commanders.FirstOrDefault(
          c => c.Info.ScryfallId == dto.Commander?.ScryfallId)?
          .Info.FrontFace.ArtCropUri ?? string.Empty;

        return new DeckSelectionDeck(
          title: dto.Name,
          imageUri: imageUri,
          colors: colors.Distinct().ToArray());
      });
    }
    catch { throw; }
  }
}