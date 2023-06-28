using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MTGApplication.Models;

namespace MTGApplication.ViewModels;

/// <summary>
/// MTGCardViewModel for deck testing
/// </summary>
public partial class DeckTestingMTGCardViewModel : MTGCardViewModel
{
  public DeckTestingMTGCardViewModel(MTGCard model) : base(model) { }

  private int plusCounters = 0;

  [ObservableProperty]
  private bool isTapped = false;

  public int PlusCounters
  {
    get => plusCounters;
    set
    {
      value = value < 0 ? 0 : value;
      plusCounters = value;
      OnPropertyChanged(nameof(PlusCounters));
    }
  }

  [RelayCommand]
  public void IncreasePlusCounters() => PlusCounters++;

  [RelayCommand]
  public void DecreasePlusCounters() => PlusCounters--;
}
