using CommunityToolkit.Mvvm.Input;
using MTGApplication.Features.DeckEditor.ViewModels;
using MTGApplication.General.Services.Importers.CardImporter;
using MTGApplication.General.Services.IOServices;
using MTGApplication.General.ViewModels;
using System.Threading.Tasks;

namespace MTGApplication.Features.DeckEditor.Editor.UseCases;

public partial class DeckEditorViewModelCommands
{
  public IAsyncRelayCommand OpenEdhrecCommanderWebsiteCommand { get; } = new OpenEdhrecCommanderWebsite(viewmodel).Command;

  private class OpenEdhrecCommanderWebsite(DeckEditorViewModel viewmodel) : ViewModelAsyncCommand<DeckEditorViewModel>(viewmodel)
  {
    protected override bool CanExecute() => Viewmodel.Commander != null;

    protected override async Task Execute()
      => await NetworkService.OpenUri(
        EdhrecImporter.GetCommanderWebsiteUri(Viewmodel.Commander, Viewmodel.Partner));
  }
}