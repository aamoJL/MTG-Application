using MTGApplication.Services.DialogService;
using static MTGApplication.Services.DialogService.DialogService;

namespace MTGApplication.Features.CardDeck;

public class ShowUnsavedChangesDialogUseCase : ShowDialogUseCase<bool?>
{
  public ShowUnsavedChangesDialogUseCase(DialogWrapper wrapper, string currentName) : base(wrapper)
   => Dialog = new ConfirmationDialog("Save unsaved changes?") { Message = $"{(string.IsNullOrEmpty(currentName) ? "Unnamed deck" : $"'{currentName}'")} has unsaved changes. Would you like to save the deck?", PrimaryButtonText = "Save" };
}
