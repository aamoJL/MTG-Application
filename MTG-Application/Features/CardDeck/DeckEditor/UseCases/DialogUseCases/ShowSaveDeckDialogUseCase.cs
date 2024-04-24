using MTGApplication.General.Services.ConfirmationService;
using System.IO;
using static MTGApplication.General.Services.ConfirmationService.DialogService;

namespace MTGApplication.Features.CardDeck;

public class ShowSaveDeckDialogUseCase : ShowDialogUseCase<string>
{
  public ShowSaveDeckDialogUseCase(DialogWrapper wrapper, string currentName) : base(wrapper)
    => Dialog = new TextBoxDialog("Save your deck?")
    {
      InvalidInputCharacters = Path.GetInvalidFileNameChars(),
      TextInputText = currentName,
      PrimaryButtonText = "Save",
      SecondaryButtonText = string.Empty
    };
}
