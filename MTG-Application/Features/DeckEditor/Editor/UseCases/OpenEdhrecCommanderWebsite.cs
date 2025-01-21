using MTGApplication.Features.DeckEditor.ViewModels;
using MTGApplication.General.Services.Importers.CardImporter;
using MTGApplication.General.Services.IOServices;
using MTGApplication.General.ViewModels;
using System.Threading.Tasks;

namespace MTGApplication.Features.DeckEditor.Editor.UseCases;

public partial class DeckEditorViewModelCommands
{
  public class OpenEdhrecCommanderWebsite(DeckEditorViewModel viewmodel) : ViewModelAsyncCommand<DeckEditorViewModel>(viewmodel)
  {
    protected override bool CanExecute() => Viewmodel.Commander.Card != null;

    protected override async Task Execute()
    {
      if (!CanExecute())
        return;

      await NetworkService.OpenUri(
        EdhrecImporter.GetCommanderWebsiteUri(Viewmodel.Commander.Card!, Viewmodel.Partner.Card));
    }
  }
}