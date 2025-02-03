using MTGApplication.Features.CardCollectionEditor.CardCollection.Services;
using MTGApplication.Features.CardCollectionEditor.CardCollectionList.Services;

namespace MTGApplication.Features.CardCollectionEditor.Editor.Services;

public class CardCollectionEditorConfirmers
{
  public CardCollectionConfirmers CardCollectionConfirmers { get; init; } = new();
  public CardCollectionListConfirmers CardCollectionListConfirmers { get; init; } = new();
}
