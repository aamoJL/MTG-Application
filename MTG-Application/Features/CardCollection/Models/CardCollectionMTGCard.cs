using CommunityToolkit.Mvvm.ComponentModel;
using MTGApplication.General.Models;

namespace MTGApplication.Features.CardCollection;

public partial class CardCollectionMTGCard(MTGCardInfo info) : MTGCard(info)
{
  [ObservableProperty] private bool isOwned;
}