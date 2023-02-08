using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MTGApplication.Charts;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTGApplication.ViewModels
{
  public partial class DeckBuildingControlViewModel : ViewModelBase
  {
    public DeckBuildingControlViewModel()
    {

      
    }

    public CMCChart CMCChart { get; }
    public SpellTypeChart SpellTypeChart { get; }

    [ObservableProperty]
    private double desiredItemWidth = 250;
  }
}
