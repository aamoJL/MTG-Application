using CommunityToolkit.Mvvm.ComponentModel;
using MTGApplication.General.Models.Card;

namespace MTGApplication.Features.CardCollection;

public partial class CardCollectionMTGCard(MTGCard.MTGCardInfo info) : MTGCard(info)
{
  [ObservableProperty] private bool isOwned;
}
