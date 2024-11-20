using CommunityToolkit.Mvvm.ComponentModel;
using MTGApplication.General.Models;

namespace MTGApplication.Features.CardCollectionEditor.CardCollectionList.Models;

public partial class CardCollectionMTGCard(MTGCardInfo info) : MTGCard(info)
{
  [ObservableProperty] public partial bool IsOwned { get; set; }
}