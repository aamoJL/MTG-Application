using MTGApplication.Features.DeckEditor.Services.Commanders;
using MTGApplication.General.Models.Card;
using MTGApplication.General.Services.NotificationService.UseCases;
using MTGApplication.General.ViewModels;
using System.Threading.Tasks;
namespace MTGApplication.Features.DeckEditor;

public partial class CommanderViewModelCommands
{
  public class ImportCommander(CommanderViewModel viewmodel) : ViewModelAsyncCommand<CommanderViewModel, string>(viewmodel)
  {
    protected override async Task Execute(string data)
    {
      var result = await Viewmodel.Worker.DoWork(new DeckEditorCardImporter(Viewmodel.Importer).Import(data));

      if (result.Found.Length == 0)
        new SendNotification(Viewmodel.Notifier).Execute(CommanderNotifications.ImportError);
      else if (!result.Found[0].Info.TypeLine.Contains("Legendary", System.StringComparison.OrdinalIgnoreCase))
        new SendNotification(Viewmodel.Notifier).Execute(CommanderNotifications.ImportNotLegendaryError);
      else
      {
        // Only legendary cards are allowed to be commanders
        await Viewmodel.ChangeCommanderCommand.ExecuteAsync(result.Found[0]);

        new SendNotification(Viewmodel.Notifier).Execute(CommanderNotifications.ImportSuccess);
      }
    }
  }
}