using MTGApplication.Features.DeckEditor.ViewModels;
using MTGApplication.General.Services.Importers.CardImporter;
using MTGApplication.General.ViewModels;
using System;
using System.Threading.Tasks;

namespace MTGApplication.Features.DeckEditor.Editor.UseCases;

public partial class DeckEditorViewModelCommands
{
  public class OpenEdhrecSearchWindow(DeckEditorViewModel viewmodel) : ViewModelAsyncCommand<DeckEditorViewModel>(viewmodel)
  {
    protected override bool CanExecute() => Viewmodel.Commander.Card != null;

    protected override async Task Execute()
    {
      if (!CanExecute())
        return;

      try
      {
        var themes = await EdhrecImporter.GetThemes(
          commander: Viewmodel.Commander.Card!.Info.Name,
          partner: Viewmodel.Partner.Card?.Info.Name);

        new AppWindows.EdhrecSearchWindow.EdhrecSearchWindow(themes).Activate();
      }
      catch (Exception e)
      {
        Viewmodel.Notifier.Notify(new(General.Services.NotificationService.NotificationService.NotificationType.Error, $"Error: {e.Message}"));
      }
    }
  }
}

