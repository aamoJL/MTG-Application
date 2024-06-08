using CommunityToolkit.Mvvm.ComponentModel;
using MTGApplication.General.Models.Card;

namespace MTGApplication.Features.CardCollection;

public partial class CardCollectionMTGCard(DeckEditorMTGCard.MTGCardInfo info) : DeckEditorMTGCard(info)
{
  [ObservableProperty] private bool isOwned;
}