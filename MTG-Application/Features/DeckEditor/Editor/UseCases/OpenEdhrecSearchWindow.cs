using MTGApplication.Features.DeckEditor.ViewModels;
using MTGApplication.General.Services.Importers.CardImporter;
using MTGApplication.General.ViewModels;
using System.Threading.Tasks;

namespace MTGApplication.Features.DeckEditor.Editor.UseCases;

public class OpenEdhrecSearchWindow(DeckEditorViewModel viewmodel) : ViewModelAsyncCommand<DeckEditorViewModel>(viewmodel)
{
  protected override bool CanExecute() => Viewmodel.Commander != null;

  protected override async Task Execute()
  {
    if (!CanExecute()) return;

    var themes = await EdhrecImporter.GetThemes(
      commander: Viewmodel.Commander.Info.Name,
      partner: Viewmodel.Partner?.Info.Name);

    new AppWindows.EdhrecSearchWindow.EdhrecSearchWindow(themes).Activate();
  }
}