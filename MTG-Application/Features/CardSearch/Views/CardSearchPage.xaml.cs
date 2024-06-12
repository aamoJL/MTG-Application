using Microsoft.UI.Xaml.Controls;
using MTGApplication.Features.CardSearch.Services;
using MTGApplication.Features.DeckEditor.Editor.Services;
using MTGApplication.General.Models;
using MTGApplication.General.Services.Importers.CardImporter;
using MTGApplication.General.Views.DragAndDrop;
using System.Linq;

namespace MTGApplication.Features.CardSearch;
public sealed partial class CardSearchPage : Page
{
  public CardSearchPage()
  {
    InitializeComponent();

    ViewModel.Confirmers.ShowCardPrintsConfirmer.OnConfirm = async (msg)
      => (await new DraggableMTGCardGridViewDialog($"{msg.Title} (Prints can be dragged)", "MTGPrintGridViewItemTemplate", "MTGAdaptiveGridViewStyle")
      {
        Items = msg.Data.Select(x => new CardImportResult<MTGCardInfo>.Card(x)).ToArray(),
        SecondaryButtonText = string.Empty,
        PrimaryButtonText = string.Empty,
        CloseButtonText = "Close"
      }.ShowAsync(new(XamlRoot)) as MTGCardInfo);
  }

  public CardSearchViewModel ViewModel { get; } = new(App.MTGCardImporter);
  public ListViewDragAndDrop CardDragAndDrop { get; } = new(new DeckEditorMTGCardCopier()) { AcceptMove = false };
}