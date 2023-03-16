using Microsoft.UI.Xaml.Controls;
using MTGApplication.API;
using MTGApplication.Database.Repositories;
using MTGApplication.ViewModels;

namespace MTGApplication.Views.Pages.Tabs
{
  public sealed partial class CollectionsTabView : UserControl
  {
    public CollectionsTabView()
    {
      this.InitializeComponent();
    }

    public CardCollectionsViewModel CardCollectionsViewModel = new(new ScryfallAPI(), new SQLiteMTGCardCollectionRepository(new ScryfallAPI(), new()));
  }
}
