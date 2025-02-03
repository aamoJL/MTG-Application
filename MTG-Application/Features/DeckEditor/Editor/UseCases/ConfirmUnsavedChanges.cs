using MTGApplication.Features.DeckEditor.Editor.Services;
using MTGApplication.Features.DeckEditor.ViewModels;
using MTGApplication.General.Services.ConfirmationService;
using MTGApplication.General.ViewModels;
using System.Threading.Tasks;

namespace MTGApplication.Features.DeckEditor.Editor.UseCases;

public class ConfirmUnsavedChanges(DeckEditorViewModel viewmodel) : AsyncUseCase<ISavable.ConfirmArgs, ISavable.ConfirmArgs>
{
  /// <returns><paramref name="param"/></returns>
  public override async Task<ISavable.ConfirmArgs> ExecuteAsync(ISavable.ConfirmArgs param)
  {
    if (param.Cancelled || !viewmodel.HasUnsavedChanges)
      return param;

    switch (await viewmodel.Confirmers.SaveUnsavedChangesConfirmer
      .Confirm(DeckEditorConfirmers.GetSaveUnsavedChangesConfirmation(viewmodel.Name)))
    {
      case ConfirmationResult.Yes:
        await viewmodel.SaveDeckCommand.ExecuteAsync(null);
        param!.Cancelled = viewmodel.HasUnsavedChanges;
        break;
      case ConfirmationResult.Cancel:
        param!.Cancelled = true;
        break;
    };

    return param;
  }
}