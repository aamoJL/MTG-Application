using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using MTGApplication.ViewModels;
using System.Collections.ObjectModel;
using System.ComponentModel;
using static MTGApplication.ViewModels.MTGCardCollectionViewModel;

namespace MTGApplication.Controls
{
  public sealed partial class MTGCardGridView : UserControl, INotifyPropertyChanged
  {
    public MTGCardGridView()
    {
      this.InitializeComponent();
      DisplayType = DisplayTypes.List;
      DesiredImageWidth = 250;
      ControlsVisibility = Visibility.Visible;
    }

    public event PropertyChangedEventHandler PropertyChanged;

    private DataTemplate selectedTemplate;
    private int selectedDesiredImageWidth;

    public DataTemplate SelectedTemplate
    {
      get => selectedTemplate;
      set
      {
        selectedTemplate = value;
        PropertyChanged?.Invoke(this, new(nameof(SelectedTemplate)));
      }
    }

    public ObservableCollection<MTGCardViewModel> ItemsSource
    {
      get { return (ObservableCollection<MTGCardViewModel>)GetValue(ItemsSourceProperty); }
      set { SetValue(ItemsSourceProperty, value); }
    }
    public DisplayTypes DisplayType
    {
      get { return (DisplayTypes)GetValue(DisplayTypeProperty); }
      set 
      { 
        SetValue(DisplayTypeProperty, value);
        
        if (value == DisplayTypes.List) { SelectedTemplate = ListTemplate; }
        else { SelectedTemplate = ImageTemplate; }

        // Update desired image width
        DesiredImageWidth = selectedDesiredImageWidth;
      }
    }
    public int DesiredImageWidth
    {
      get { return (int)GetValue(DesiredImageWidthProperty); }
      set 
      { 
        // Store user's selected width
        selectedDesiredImageWidth = value;

        // Limit value depending on the selected DisplayType
        if(DisplayType == DisplayTypes.List)
        {
          value = int.MaxValue;
        }
        
        SetValue(DesiredImageWidthProperty, value);
      }
    }
    public Visibility ControlsVisibility
    {
      get { return (Visibility)GetValue(HideControlsProperty); }
      set { SetValue(HideControlsProperty, value); }
    }

    public static readonly DependencyProperty HideControlsProperty =
        DependencyProperty.Register(nameof(ControlsVisibility), typeof(Visibility), typeof(MTGCardGridView), new PropertyMetadata(Visibility.Visible));

    public static readonly DependencyProperty DisplayTypeProperty =
        DependencyProperty.Register(nameof(DisplayType), typeof(DisplayTypes), typeof(MTGCardGridView), new PropertyMetadata(DisplayTypes.List));

    public static readonly DependencyProperty ItemsSourceProperty =
        DependencyProperty.Register(nameof(ItemsSource), typeof(ObservableCollection<MTGCardViewModel>), typeof(MTGCardGridView), new PropertyMetadata(null));

    public static readonly DependencyProperty DesiredImageWidthProperty =
        DependencyProperty.Register(nameof(DesiredImageWidth), typeof(int), typeof(MTGCardGridView), new PropertyMetadata(250));
  }
}
