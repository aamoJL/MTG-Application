using CommunityToolkit.Mvvm.ComponentModel;

namespace MTGApplication.Models
{
  public partial class NamedMTGCardCollectionModel : MTGCardCollectionModel
  {
    [ObservableProperty]
    public string name = "";

    public void Reset()
    {
      Clear();
      Name = "";
    }
  }
}
