using MTGApplication.Models;
using MTGApplication.ViewModels;
using LiveChartsCore.SkiaSharpView;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using MTG_builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;

namespace MTGApplication
{
  /// <summary>
  /// Main Window
  /// </summary>
  public sealed partial class MainWindow : Window
  {
    public MainWindowViewModel ViewModel = new();

    private bool pointerOverCard;

    public MainWindow()
    {
      this.InitializeComponent();
      IO.InitDirectories();
      // Disable transitions when adding or modifying collection lists
      CollectionGridView.ItemContainerTransitions = null;

      CollectionCmcChart.XAxes = new List<Axis>
      {
        new Axis
        {
          MinStep = 1,
          TextSize = 10
        }
      };
      CollectionCmcChart.YAxes = new List<Axis>
      {
        new Axis
        {
          MinStep = 1,
          TextSize = 10
        }
      };
    }

    #region //-----------Scryfall search controls-----------//
    // TODO: use same view on both list types
    private void ScryfallSearchListDisplayBtn_Click(object sender, RoutedEventArgs e)
    {
      // Change card display to List
      CardGridView.Visibility = Visibility.Collapsed;
      //CardListView.Visibility = Visibility.Visible;
    }
    private void ScryfallSearchImageDisplayBtn_Click(object sender, RoutedEventArgs e)
    {
      // Change card display to Images
      CardGridView.Visibility = Visibility.Visible;
      //CardListView.Visibility = Visibility.Collapsed;
    }
    private void CollectionListDisplayBtn_Click(object sender, RoutedEventArgs e)
    {
      // Change card display to List
      CollectionGridView.Visibility = Visibility.Collapsed;
      //CollectionListView.Visibility = Visibility.Visible;
    }
    private void CollectionImageDisplayBtn_Click(object sender, RoutedEventArgs e)
    {
      // Change card display to Images
      CollectionGridView.Visibility = Visibility.Visible;
      //CollectionListView.Visibility = Visibility.Collapsed;
    }
    #endregion
    
    #region //----------------Card pointer events---------------//
    private void ScryfallCardName_PointerEntered(object sender, PointerRoutedEventArgs e)
    {
      // Change card preview image to hovered item
      PreviewImage.Visibility = Visibility.Visible;
      FrameworkElement ListElement = sender as FrameworkElement;
      if(ListElement != null)
      {
        MTGCardViewModel cardViewModel = ListElement.DataContext as MTGCardViewModel;
        ViewModel.PreviewCardViewModel = cardViewModel;
      }
    }
    private void CardListView_PointerMoved(object sender, PointerRoutedEventArgs e)
    {
      // Move card preview image to mouse position when hovering over on list view item.
      // Position is clamped to window size
      Point offset = new(50, -100);
      Point pointerPosition = e.GetCurrentPoint(null).Position;
      Rect windowBounds = this.Bounds;
      PreviewImage.SetValue(Canvas.LeftProperty, Math.Clamp(pointerPosition.X + offset.X, 0, windowBounds.Width - PreviewImage.ActualWidth));
      PreviewImage.SetValue(Canvas.TopProperty, Math.Clamp(pointerPosition.Y + offset.Y, 0, windowBounds.Height - PreviewImage.ActualHeight));
    }
    private void CardListViewItem_PointerExited(object sender, PointerRoutedEventArgs e)
    {
      PreviewImage.Visibility = Visibility.Collapsed;
    }
    private void ScryfallCard_PointerEntered(object sender, PointerRoutedEventArgs e)
    {
      pointerOverCard = true;
    }
    private void ScryfallCard_PointerExited(object sender, PointerRoutedEventArgs e)
    {
      pointerOverCard = false;
    }
    #endregion

    #region //--------------------- Drag & Drop -----------------------------//
    // TODO: MVVM Drag and Drop
    private void CardGridView_DragItemsStarting(object sender, DragItemsStartingEventArgs e)
    {
      MTGCardViewModel viewModel = (e.Items[0] as MTGCardViewModel);
      if (viewModel != null)
      {
        e.Data.SetText(viewModel.ModelToJSON());
        e.Data.RequestedOperation = DataPackageOperation.Copy;
      }
    }
    private void CollectionListView_DragOver(object sender, DragEventArgs e)
    {
      if (e.DataView.Contains(StandardDataFormats.Text))
      {
        e.AcceptedOperation = DataPackageOperation.Copy;
      }
    }
    private async void CollectionListView_Drop(object sender, DragEventArgs e)
    {
      if (e.DataView.Contains(StandardDataFormats.Text))
      {
        DragOperationDeferral def = e.GetDeferral();
        string data = await e.DataView.GetTextAsync();
        MTGCardModel model = JsonSerializer.Deserialize<MTGCardModel>(data);
        if (model != null && ViewModel.CollectionViewModel.AddCommand.CanExecute(null))
        {
          ViewModel.CollectionViewModel.AddCommand.Execute(new MTGCardViewModel(model));
        }
        e.AcceptedOperation = DataPackageOperation.Copy;
        def.Complete();
      }
    }
    #endregion

    #region //------------------------File Dialogs--------------------------//
    private async void CollectionControlNew_Click(object sender, RoutedEventArgs e)
    {
      // Ask if user wants to save unsaved changes,
      // Cancel press will also cancel the open dialog
      if (ViewModel.CollectionViewModel.UnsavedChanges)
      {
        if (!await ShowUnsavedConfirmationDialog()) { return; }
      }

      if (ViewModel.CollectionViewModel.ResetCommand.CanExecute(null)) ViewModel.CollectionViewModel.ResetCommand.Execute(null);
    }
    private async void CollectionControlOpen_Click(object sender, RoutedEventArgs e)
    {
      // Ask if user wants to save unsaved changes,
      // Cancel press will also cancel the open dialog
      if (ViewModel.CollectionViewModel.UnsavedChanges)
      {
        if(!await ShowUnsavedConfirmationDialog()) { return; }
      }
      var fileName = await ShowOpenDialog();
      if (!string.IsNullOrEmpty(fileName) && ViewModel.CollectionViewModel.LoadCommand.CanExecute(fileName))
      {
        ViewModel.CollectionViewModel.LoadCommand.Execute(fileName);
      }
    }
    private async void CollectionControlSave_Click(object sender, RoutedEventArgs e)
    {
      await ShowSaveDialog();
    }
    private async void CollectionControlDelete_Click(object sender, RoutedEventArgs e)
    {
      ContentDialog deleteDialog = new()
      {
        XamlRoot = Content.XamlRoot,
        Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style,
        Title = "Delete Deck?",
        PrimaryButtonText = "Yes",
        CloseButtonText = "No",
        DefaultButton = ContentDialogButton.Primary,
      };
      deleteDialog.Content = $"Delete deck '{ViewModel.CollectionViewModel.Name}'?";

      if (await deleteDialog.ShowAsync() == ContentDialogResult.Primary && ViewModel.CollectionViewModel.DeleteDeckFileCommand.CanExecute(null))
      {
        ViewModel.CollectionViewModel.DeleteDeckFileCommand.Execute(null);
      }
    }

    private async Task<string> ShowOpenDialog()
    {
      ContentDialog openDialog = new()
      {
        XamlRoot = Content.XamlRoot,
        Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style,
        Title = "Open Deck",
        PrimaryButtonText = "Open", // return true
        CloseButtonText = "Cancel", // return false
        DefaultButton = ContentDialogButton.Primary,
      };
      string[] jsonNames = IO.GetJsonFileNames(IO.CollectionsPath);
      ComboBox comboBox = new() { ItemsSource = jsonNames, Header = "Name" };
      openDialog.Content = comboBox;

      if (await openDialog.ShowAsync() == ContentDialogResult.Primary && (string)comboBox.SelectedValue != "" && comboBox.SelectedValue != null)
      {
        return comboBox.SelectedValue as string;
      }
      return String.Empty;
    }
    private async Task<bool> ShowUnsavedConfirmationDialog()
    {
      ContentDialog saveConfirmationDialog = new()
      {
        XamlRoot = Content.XamlRoot,
        Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style,
        Title = "Save unsaved changes?",
        PrimaryButtonText = "Yes",  // Show save dialog
        SecondaryButtonText = "No", // return true
        CloseButtonText = "Cancel", // return false
        DefaultButton = ContentDialogButton.Primary,
        Content = "Would you like to save unsaved changes?"
      };

      var saveResult = await saveConfirmationDialog.ShowAsync();
      if (saveResult == ContentDialogResult.None) { return false; }
      else if (saveResult == ContentDialogResult.Primary)
      {
        return await ShowSaveDialog();
      }
      return true;
    }
    private async Task<bool> ShowSaveDialog()
    {
      ContentDialog saveDialog = new()
      {
        XamlRoot = Content.XamlRoot,
        Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style,
        Title = "Save your deck?",
        PrimaryButtonText = "Save", // check override
        SecondaryButtonText = "No", // return true
        CloseButtonText = "Cancel", // return false
        DefaultButton = ContentDialogButton.Primary
      };
      var textBox = new TextBox() { Text = ViewModel.CollectionViewModel.Name, Header = "Name" };
      saveDialog.Content = textBox;

      var saveResult = await saveDialog.ShowAsync();
      if(saveResult == ContentDialogResult.None) { return false; }
      else if (saveResult == ContentDialogResult.Primary)
      {
        if (textBox.Text != ViewModel.CollectionViewModel.Name &&
          IO.GetJsonFileNames(IO.CollectionsPath).Contains(textBox.Text))
        {
          // Override confirmation
          ContentDialog OverrideDialog = new()
          {
            XamlRoot = Content.XamlRoot,
            Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style,
            Title = "Override existing deck?",
            PrimaryButtonText = "Yes",  // return true
            SecondaryButtonText = "No", // return true
            CloseButtonText = "Cancel", // return false
            DefaultButton = ContentDialogButton.Primary,
            Content = $"Deck '{textBox.Text}' already exist. Would you like to override the deck?"
          };
          
          var OverrideResult = await OverrideDialog.ShowAsync();
          if(OverrideResult == ContentDialogResult.None) { return false; }
          else if (OverrideResult == ContentDialogResult.Primary && ViewModel.CollectionViewModel.SaveCommand.CanExecute(textBox.Text))
          {
            ViewModel.CollectionViewModel.SaveCommand.Execute(textBox.Text);
          }
        }
        else if(ViewModel.CollectionViewModel.SaveCommand.CanExecute(textBox.Text))
        {
          ViewModel.CollectionViewModel.SaveCommand.Execute(textBox.Text);
        }
      }
      return true;
    }
    #endregion
  }
}

// Icon symbols https://docs.microsoft.com/en-us/uwp/api/windows.ui.xaml.controls.symbol?view=winrt-22621
