using Microsoft.UI.Xaml;
using MTGApplication.Features.CardCollectionEditor.CardCollectionList.Views.Controls;
using MTGApplication.General.Services.ConfirmationService;
using MTGApplication.General.Views.Dialogs.UseCases;
using System.Threading.Tasks;

namespace MTGApplication.Features.CardCollectionEditor.CardCollectionList.Views;

public partial class CardCollectionEditorViewDialogs
{
  public class ShowCollectionListContentDialog(XamlRoot root) : ShowDialogUseCase<(string Name, string Query)?, (string Name, string Query)?>(root)
  {
    protected override async Task<(string Name, string Query)?> ShowDialog(string title, string message, (string Name, string Query)? data)
      => await DialogService.ShowAsync(Root, new CollectionListContentDialog(title)
      {
        PrimaryButtonText = data != null ? "Edit" : "Add",
        NameInputText = data?.Name ?? string.Empty,
        QueryInputText = data?.Query ?? string.Empty,
      });
  }
}