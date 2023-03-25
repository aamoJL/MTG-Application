using Microsoft.UI.Xaml;
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
      App.Closing += MainWindow_Closed;
    }

    public CardCollectionsViewModel CardCollectionsViewModel = new(new ScryfallAPI(), new SQLiteMTGCardCollectionRepository(new ScryfallAPI(), new()));
    
    private void MainWindow_Closed(object sender, App.WindowClosingEventArgs args)
    {
      args.ClosingTasks.Add(CardCollectionsViewModel);
    }

    private void GridViewItemImage_DoubleTapped(object sender, Microsoft.UI.Xaml.Input.DoubleTappedRoutedEventArgs e)
    {
      if (SingleTapSelectionModeSwitch.IsChecked is true) { return; }
      if (sender is FrameworkElement element && element.DataContext is MTGCardCollectionCardViewModel vm)
      {
        if (vm.IsOwned) { CardCollectionsViewModel.SelectedList.RemoveFromList(vm.Model); }
        else { CardCollectionsViewModel.SelectedList.AddToList(vm.Model); }
      }
    }

    private void GridViewItemImage_Tapped(object sender, Microsoft.UI.Xaml.Input.TappedRoutedEventArgs e)
    {
      if (SingleTapSelectionModeSwitch.IsChecked is false) { return; }
      if (sender is FrameworkElement element && element.DataContext is MTGCardCollectionCardViewModel vm)
      {
        if (vm.IsOwned) { CardCollectionsViewModel.SelectedList.RemoveFromList(vm.Model); }
        else { CardCollectionsViewModel.SelectedList.AddToList(vm.Model); }
      }
    }
  }
}
