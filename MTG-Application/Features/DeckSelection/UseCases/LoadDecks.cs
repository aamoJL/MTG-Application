using Microsoft.EntityFrameworkCore;
using MTGApplication.Features.DeckSelection.Models;
using MTGApplication.Features.DeckSelection.ViewModels;
using MTGApplication.General.Services.Databases.Repositories.CardRepository.Models;
using MTGApplication.General.Services.Databases.Repositories.DeckRepository.UseCases;
using MTGApplication.General.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static MTGApplication.General.Models.MTGCardInfo;

namespace MTGApplication.Features.DeckSelection.UseCases;

public partial class DeckSelectorViewModelCommands
{
  public class LoadDecks(DeckSelectionViewModel viewmodel) : ViewModelAsyncCommand<DeckSelectionViewModel>(viewmodel)
  {
    protected override async Task Execute()
    {
      try
      {
        var deckDTOs = (await Viewmodel.Worker.DoWork(new GetDeckDTOs(Viewmodel.Repository)
        {
          SetIncludes = (set) =>
          {
            set.Include(x => x.Commander).Load();
            set.Include(x => x.CommanderPartner).Load();
          },
        }.Execute())).OrderBy(x => x.Name);

        if (deckDTOs == null)
          return;

        var commanderDTOs = new List<MTGCardDTO>();

        commanderDTOs.AddRange(deckDTOs.Where(x => x.Commander != null).Select(x => x.Commander!));
        commanderDTOs.AddRange(deckDTOs.Where(x => x.CommanderPartner != null).Select(x => x.CommanderPartner!));

        var commanders = (await Viewmodel.Worker.DoWork(Viewmodel.Importer.ImportWithDTOs(commanderDTOs))).Found;

        foreach (var dto in deckDTOs)
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

          Viewmodel.DeckItems.Add(new DeckSelectionDeck(
            title: dto.Name,
            imageUri: commanders.FirstOrDefault(c => c.Info.ScryfallId == dto.Commander?.ScryfallId)?.Info.FrontFace.ArtCropUri ?? string.Empty,
            colors: colors.Distinct().ToArray()));
        }
      }
      catch (System.Exception e)
      {
        Viewmodel.Notifier.Notify(new(General.Services.NotificationService.NotificationService.NotificationType.Error, $"Error: {e.Message}"));
      }
    }
  }
}