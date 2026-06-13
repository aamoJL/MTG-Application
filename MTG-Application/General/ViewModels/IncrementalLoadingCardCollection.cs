using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.WinUI.Collections;

namespace MTGApplication.General.ViewModels;

public partial class IncrementalLoadingCardCollection<TCard>(IncrementalCardSource<TCard> source) : ObservableObject
{
  public IncrementalLoadingCollection<IncrementalCardSource<TCard>, TCard> Collection { get; private set; } = new(source: source);

  [ObservableProperty] public partial int TotalCardCount { get; set; }
}