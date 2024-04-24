using MTGApplication.General.Services.ConfirmationService;
using static MTGApplication.General.Services.ConfirmationService.DialogService;

namespace MTGApplication.Features.CardDeck;

public class ShowDeckOverrideDialogUseCase : ShowDialogUseCase<bool?>
{
  public ShowDeckOverrideDialogUseCase(DialogWrapper wrapper, string newName) : base(wrapper)
    => Dialog = new ConfirmationDialog("Override existing deck?")
    {
      Message = $"Deck '{newName}' already exist. Would you like to override the deck?",
      SecondaryButtonText = string.Empty
    };
}
