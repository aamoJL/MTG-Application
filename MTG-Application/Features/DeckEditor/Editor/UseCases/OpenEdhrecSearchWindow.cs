using CommunityToolkit.Mvvm.Input;
using MTGApplication.Features.DeckEditor.ViewModels;
using MTGApplication.General.Services.Importers.CardImporter;
using MTGApplication.General.ViewModels;
using System;
using System.Threading.Tasks;

namespace MTGApplication.Features.DeckEditor.Editor.UseCases;

public partial class DeckEditorViewModelCommands
{
  public IRelayCommand OpenEdhrecSearchWindowCommand { get; } = new OpenEdhrecSearchWindow(viewmodel).Command;

  private class OpenEdhrecSearchWindow(DeckEditorViewModel viewmodel) : ViewModelAsyncCommand<DeckEditorViewModel>(viewmodel)
  {
    protected override bool CanExecute() => Viewmodel.Commander != null;

    protected override async Task Execute()
    {
      if (!CanExecute())
        return;

      try
      {
        var themes = await EdhrecImporter.GetThemes(
          commander: Viewmodel.Commander!.Info.Name,
          partner: Viewmodel.Partner?.Info.Name);

        new AppWindows.EdhrecSearchWindow.EdhrecSearchWindow(themes).Activate();
      }
      catch (Exception e)
      {
        Viewmodel.Notifier.Notify(new(General.Services.NotificationService.NotificationService.NotificationType.Error, $"Error: {e.Message}"));
      }
    }
  }
}

