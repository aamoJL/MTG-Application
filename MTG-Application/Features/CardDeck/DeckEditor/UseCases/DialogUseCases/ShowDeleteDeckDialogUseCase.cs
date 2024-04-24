using MTGApplication.General.Services.ConfirmationService;
using static MTGApplication.General.Services.ConfirmationService.DialogService;

namespace MTGApplication.Features.CardDeck;

public class ShowDeleteDeckDialogUseCase : ShowDialogUseCase<bool?>
{
  public ShowDeleteDeckDialogUseCase(DialogWrapper wrapper, string currentName) : base(wrapper)
    => Dialog = new ConfirmationDialog("Delete deck?")
    {
      Message = $"Are you sure you want to delete '{currentName}'?",
      SecondaryButtonText = string.Empty
    };
}
