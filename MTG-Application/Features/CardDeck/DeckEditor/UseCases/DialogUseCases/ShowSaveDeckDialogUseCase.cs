using MTGApplication.Services.DialogService;
using System.IO;
using static MTGApplication.Services.DialogService.DialogService;

namespace MTGApplication.Features.CardDeck;

public class ShowSaveDeckDialogUseCase : ShowDialogUseCase<string>
{
  public ShowSaveDeckDialogUseCase(DialogWrapper wrapper, string currentName) : base(wrapper)
    => Dialog = new TextBoxDialog("Save your deck?") { 
      InvalidInputCharacters = Path.GetInvalidFileNameChars(), 
      TextInputText = currentName, 
      PrimaryButtonText = "Save", 
      SecondaryButtonText = string.Empty };
}
