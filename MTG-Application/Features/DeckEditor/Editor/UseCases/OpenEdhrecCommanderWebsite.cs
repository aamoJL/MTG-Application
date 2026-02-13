using MTGApplication.Features.DeckEditor.ViewModels;
using MTGApplication.General.ViewModels;
using System.Threading.Tasks;

namespace MTGApplication.Features.DeckEditor.Editor.UseCases;

public partial class DeckEditorViewModelCommands
{
  public class OpenEdhrecCommanderWebsite(DeckEditorViewModel viewmodel) : AsyncCommand
  {
    public DeckEditorViewModel Viewmodel { get; } = viewmodel;

    protected override bool CanExecute() => Viewmodel.Commander.Card != null;

    protected override async Task Execute()
    {
      if (!CanExecute())
        return;

      //await NetworkIO.OpenUri(
      //  EdhrecImporter.GetCommanderWebsiteUri(Viewmodel.Commander.Card!, Viewmodel.Partner.Card));
    }
  }
}