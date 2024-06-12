using MTGApplication.Features.CardCollection.Services;
using MTGApplication.General.Services.ConfirmationService;
using MTGApplication.General.ViewModels;
using System.Threading.Tasks;

namespace MTGApplication.Features.CardCollection.UseCases;

public partial class CardCollectionViewModelCommands
{
  public class ConfirmUnsavedChanges(CardCollectionViewModel viewmodel) : ViewModelAsyncCommand<CardCollectionViewModel, ISavable.ConfirmArgs>(viewmodel)
  {
    protected override bool CanExecute(ISavable.ConfirmArgs param) => !param.Cancelled && Viewmodel.HasUnsavedChanges;

    protected override async Task Execute(ISavable.ConfirmArgs param)
    {
      if (!CanExecute(param)) return;

      switch (await Viewmodel.Confirmers.SaveUnsavedChangesConfirmer
        .Confirm(CardCollectionConfirmers.GetSaveUnsavedChangesConfirmation(Viewmodel.Collection.Name)))
      {
        case ConfirmationResult.Yes:
          await Viewmodel.SaveCollectionCommand.ExecuteAsync(null);
          param.Cancelled = Viewmodel.HasUnsavedChanges;
          return;
        case ConfirmationResult.Cancel:
          param.Cancelled = true;
          return;
      };
    }
  }
}