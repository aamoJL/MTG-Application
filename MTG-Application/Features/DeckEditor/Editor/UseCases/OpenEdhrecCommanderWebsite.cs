using MTGApplication.General.Services.Importers.CardImporter;
using MTGApplication.General.Services.IOServices;
using MTGApplication.General.ViewModels;
using System.Threading.Tasks;

namespace MTGApplication.Features.DeckEditor;

public partial class DeckEditorViewModelCommands
{
  public class OpenEdhrecCommanderWebsite(DeckEditorViewModel viewmodel) : ViewModelAsyncCommand<DeckEditorViewModel>(viewmodel)
  {
    protected override bool CanExecute() => Viewmodel.Deck.Commander != null;

    protected override async Task Execute()
      => await NetworkService.OpenUri(
        EdhrecImporter.GetCommanderWebsiteUri(Viewmodel.Deck.Commander, Viewmodel.Deck.CommanderPartner));
  }
}