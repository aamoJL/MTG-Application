using MTGApplication.Services.DialogService;
using static MTGApplication.Services.DialogService.DialogService;

namespace MTGApplication.Features.CardDeck;

public class ShowDeleteDeckDialogUseCase : ShowDialogUseCase<bool?>
{
  public ShowDeleteDeckDialogUseCase(DialogWrapper wrapper, string currentName) : base(wrapper)
    => Dialog = new ConfirmationDialog("Delete deck?") { 
      Message = $"Are you sure you want to delete '{currentName}'?", 
      SecondaryButtonText = string.Empty };
}
