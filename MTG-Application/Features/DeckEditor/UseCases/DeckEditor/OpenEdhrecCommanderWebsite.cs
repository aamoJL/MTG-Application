using MTGApplication.General.Services.API.CardAPI;
using MTGApplication.General.Services.IOService;
using MTGApplication.General.ViewModels;
using System.Threading.Tasks;

namespace MTGApplication.Features.DeckEditor;

public partial class DeckEditorViewModel
{
  public class OpenEdhrecCommanderWebsite(DeckEditorViewModel viewmodel) : ViewModelAsyncCommand<DeckEditorViewModel>(viewmodel)
  {
    protected override bool CanExecute() => Viewmodel.Deck.Commander != null;

    protected override async Task Execute()
      => await NetworkService.OpenUri(
        EdhrecImporter.GetCommanderWebsiteUri(Viewmodel.Deck.Commander, Viewmodel.Deck.CommanderPartner));
  }
}