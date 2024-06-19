using CommunityToolkit.Mvvm.ComponentModel;
using MTGApplication.General.Models;

namespace MTGApplication.Features.CardCollectionEditor.CardCollectionList.Models;

public partial class CardCollectionMTGCard(MTGCardInfo info) : MTGCard(info)
{
  [ObservableProperty] private bool isOwned;
}