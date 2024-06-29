using MTGApplication.Features.CardCollectionEditor.CardCollectionList.Views.Controls;
using MTGApplication.General.Views.Dialogs.UseCases;
using System.Threading.Tasks;
using static MTGApplication.General.Services.ConfirmationService.DialogService;

namespace MTGApplication.Features.CardCollectionEditor.CardCollectionList.Views;

public partial class CardCollectionEditorViewDialogs
{
  public class ShowCollectionListContentDialog(DialogWrapper dialogWrapper) : ShowDialogUseCase<(string Name, string Query)?, (string Name, string Query)?>(dialogWrapper)
  {
    protected override async Task<(string Name, string Query)?> ShowDialog(string title, string message, (string Name, string Query)? data)
      => await DialogWrapper.ShowAsync(new CollectionListContentDialog(title)
      {
        PrimaryButtonText = data != null ? "Edit" : "Add",
        NameInputText = data?.Name ?? string.Empty,
        QueryInputText = data?.Query ?? string.Empty,
      });
  }
}