using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using MTGApplication.General.Services.IOService;
using System.Threading.Tasks;

namespace MTGApplication.General.Models.Card;

[ObservableObject]
public partial class BasicCardView : UserControl
{
  public static readonly DependencyProperty ModelProperty =
      DependencyProperty.Register(nameof(Model), typeof(MTGCard), typeof(BasicCardView),
        new PropertyMetadata(default(MTGCard), OnModelPropertyChangedCallback));

  public BasicCardViewModel ViewModel { get; set; } = new();
  
  public MTGCard Model
  {
    get => (MTGCard)GetValue(ModelProperty);
    set => SetValue(ModelProperty, value);
  }

  protected static void OnModelPropertyChangedCallback(DependencyObject sender, DependencyPropertyChangedEventArgs e)
  {
    if (sender is BasicCardView view && e.NewValue is MTGCard newModel)
    {
      if (e.Property.Equals(ModelProperty))
        view.ViewModel.Model = newModel;
    }
  }
}

public partial class BasicCardViewModel : ObservableObject
{
  public BasicCardViewModel() => PropertyChanged += BasicCardViewModel_PropertyChanged;

  [ObservableProperty, NotifyCanExecuteChangedFor(nameof(SwitchFaceImageCommand))] private MTGCard model;
  [ObservableProperty] private string selectedFaceUri = "";

  /// <summary>
  /// Changes selected face image if possible
  /// </summary>
  [RelayCommand(CanExecute = nameof(SwitchFaceImageCanExecute))]
  public void SwitchFaceImage()
  {
    if (!SwitchFaceImageCanExecute()) return;

    SelectedFaceUri = Model.Info.FrontFace.ImageUri == SelectedFaceUri
      ? Model.Info.BackFace?.ImageUri : Model.Info.FrontFace.ImageUri;
  }

  /// <summary>
  /// Opens card's API Website in web browser
  /// </summary>
  [RelayCommand] public async Task OpenAPIWebsite() => await new OpenUri().Execute(Model.Info.APIWebsiteUri);

  /// <summary>
  /// Opens card's Cardmarket page in web browser
  /// </summary>    
  [RelayCommand] public async Task OpenCardmarketWebsite() => await new OpenUri().Execute(Model.Info.CardMarketUri);
  
  private void BasicCardViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
  {
    if (e.PropertyName == nameof(Model)) SelectedFaceUri = Model.Info.FrontFace.ImageUri;
  }

  private bool SwitchFaceImageCanExecute() => !string.IsNullOrEmpty(Model?.Info.BackFace?.ImageUri);
}

