using MTGApplication.General.Services.ConfirmationService;
using static MTGApplication.General.Services.ConfirmationService.DialogService;

namespace MTGApplication.Features.CardDeck;

public class ShowOpenDeckDialogUseCase : ShowDialogUseCase<string>
{
  public ShowOpenDeckDialogUseCase(DialogWrapper wrapper, string[] deckNames) : base(wrapper)
    => Dialog = new ComboBoxDialog("Open deck")
    {
      InputHeader = "Name",
      Items = deckNames,
      PrimaryButtonText = "Open",
      SecondaryButtonText = string.Empty
    };
}