﻿using MTGApplication.Features.DeckEditor.Commanders.Services;
using MTGApplication.Features.DeckEditor.Commanders.ViewModels;
using MTGApplication.Features.DeckEditor.Editor.Models;
using MTGApplication.Features.DeckEditor.Editor.Services;
using MTGApplication.General.Services.NotificationService.UseCases;
using MTGApplication.General.ViewModels;
using System.Threading.Tasks;

namespace MTGApplication.Features.DeckEditor.Commanders.UseCases;

public partial class CommanderViewModelCommands
{
  public class ImportCommander(CommanderViewModel viewmodel) : ViewModelAsyncCommand<CommanderViewModel, string>(viewmodel)
  {
    protected override async Task Execute(string? data)
    {
      try
      {
        data ??= string.Empty;

        var result = await Viewmodel.Worker.DoWork(new DeckEditorCardImporter(Viewmodel.Importer).Import(data));

        if (result.Found.Length == 0)
          new SendNotification(Viewmodel.Notifier).Execute(CommanderNotifications.ImportError);
        else if (!result.Found[0].Info.TypeLine.Contains("Legendary", System.StringComparison.OrdinalIgnoreCase))
          new SendNotification(Viewmodel.Notifier).Execute(CommanderNotifications.ImportNotLegendaryError);
        else
        {
          // Only legendary cards are allowed to be commanders
          var card = new DeckEditorMTGCard(result.Found[0].Info, result.Found[0].Count);

          if (Viewmodel.ChangeCommanderCommand != null && Viewmodel.ChangeCommanderCommand.CanExecute(card))
          {
            await Viewmodel.ChangeCommanderCommand.ExecuteAsync(card);

            new SendNotification(Viewmodel.Notifier).Execute(CommanderNotifications.ImportSuccess);
          }
        }
      }
      catch (System.Exception e)
      {
        Viewmodel.Notifier.Notify(new(General.Services.NotificationService.NotificationService.NotificationType.Error, $"Error: {e.Message}"));
      }
    }
  }
}