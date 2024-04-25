using CommunityToolkit.Mvvm.ComponentModel;
using MTGApplication.General.Models.Card;

namespace MTGApplication.ViewModels;

/// <summary>
/// MTGCardViewModel for deck testing
/// </summary>
public partial class DeckTestingMTGCardViewModel : MTGCardViewModel
{
  public DeckTestingMTGCardViewModel(MTGCard model) : base(model) { }

  private int plusCounters = 0;
  private int countCounters = 1;

  [ObservableProperty] private bool isTapped = false;

  public int PlusCounters
  {
    get => plusCounters;
    set
    {
      value = value < 0 ? 0 : value; // Value can't be under 0
      plusCounters = value;
      OnPropertyChanged(nameof(PlusCounters));
    }
  }
  public int CountCounters
  {
    get => countCounters;
    set
    {
      value = value < 1 ? 1 : value; // Value must ne over 0
      countCounters = value;
      OnPropertyChanged(nameof(CountCounters));
    }
  }
  public bool IsToken { get; init; }
}
