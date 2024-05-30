using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using MTGApplication.General.Models.Card;
using MTGApplication.General.Services.IOService;
using MTGApplication.General.Views.Controls;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MTGApplication.General.Views;

[ObservableObject]
public abstract partial class BasicCardView<TCard> : UserControl where TCard : MTGCard
{
  public static readonly DependencyProperty ModelProperty =
      DependencyProperty.Register(nameof(Model), typeof(TCard), typeof(BasicCardView<TCard>),
        new PropertyMetadata(default(TCard), OnModelPropertyChangedCallback));

  public static readonly DependencyProperty HoverPreviewEnabledProperty =
      DependencyProperty.Register(nameof(HoverPreviewEnabled), typeof(bool), typeof(BasicCardView<TCard>), new PropertyMetadata(false));

  public BasicCardView()
  {
    PointerExited += BasicCardView_PointerExited;
    PointerMoved += BasicCardView_PointerMoved;

    Loaded += (_, _) =>
    {
      RequestedTheme = AppConfig.LocalSettings.AppTheme;
      AppConfig.LocalSettings.PropertyChanged += AppSettings_PropertyChanged;
    };
    Unloaded += (_, _) => { AppConfig.LocalSettings.PropertyChanged -= AppSettings_PropertyChanged; };
  }

  public TCard Model
  {
    get => (TCard)GetValue(ModelProperty);
    set => SetValue(ModelProperty, value);
  }
  public bool HoverPreviewEnabled
  {
    get => (bool)GetValue(HoverPreviewEnabledProperty);
    set => SetValue(HoverPreviewEnabledProperty, value);
  }
  public string CardName => Model?.Info.Name ?? string.Empty;

  [ObservableProperty] protected string selectedFaceUri = "";

  public IAsyncRelayCommand OnDropCopy { get; set; }
  public ICommand OnDropRemove { get; set; }
  public IAsyncRelayCommand OnDropImport { get; set; }
  public ICommand OnDropBeginMoveFrom { get; set; }
  public IAsyncRelayCommand OnDropBeginMoveTo { get; set; }
  public ICommand OnDropExecuteMove { get; set; }

  /// <summary>
  /// Changes selected face image if possible
  /// </summary>
  [RelayCommand(CanExecute = nameof(CanExecuteSwitchFaceImage))]
  protected void SwitchFaceImage()
  {
    if (!CanExecuteSwitchFaceImage()) return;

    SelectedFaceUri = Model?.Info.FrontFace.ImageUri == SelectedFaceUri
      ? Model.Info.BackFace?.ImageUri : Model.Info.FrontFace.ImageUri;
  }

  /// <summary>
  /// Opens card's API Website in web browser
  /// </summary>
  [RelayCommand(CanExecute = nameof(CanExecuteOpenAPIWebsite))]
  protected async Task OpenAPIWebsite()
    => await NetworkService.OpenUri(Model?.Info.APIWebsiteUri);

  /// <summary>
  /// Opens card's Cardmarket page in web browser
  /// </summary>    
  [RelayCommand(CanExecute = nameof(CanExecuteOpenCardmarketWebsite))]
  protected async Task OpenCardmarketWebsite()
    => await NetworkService.OpenUri(Model?.Info.CardMarketUri);

  protected bool CanExecuteSwitchFaceImage() => !string.IsNullOrEmpty(Model?.Info.BackFace?.ImageUri);

  protected bool CanExecuteOpenAPIWebsite() => Model != null;

  protected bool CanExecuteOpenCardmarketWebsite() => Model != null;

  protected virtual void OnModelChanging(TCard oldValue)
  {
    if (oldValue != null) oldValue.PropertyChanged -= Model_PropertyChanged;
  }

  protected virtual void OnModelChanged(TCard newValue)
  {
    SelectedFaceUri = newValue?.Info.FrontFace.ImageUri ?? string.Empty;

    if (newValue != null) newValue.PropertyChanged += Model_PropertyChanged;

    OnPropertyChanged(nameof(CardName));
    SwitchFaceImageCommand.NotifyCanExecuteChanged();
    OpenAPIWebsiteCommand.NotifyCanExecuteChanged();
    OpenCardmarketWebsiteCommand.NotifyCanExecuteChanged();
  }

  protected void Model_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
  {
    if (e.PropertyName == nameof(MTGCard.Info))
      SelectedFaceUri = Model?.Info.FrontFace.ImageUri;
  }

  protected void AppSettings_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
  {
    // Requested theme needs to be changed to the selected theme here, because flyouts will not change theme if
    // the requested theme is Default.
    // And ActualThemeChanged event invokes only once when changing the theme
    if (e.PropertyName == nameof(AppConfig.LocalSettings.AppTheme))
      RequestedTheme = AppConfig.LocalSettings.AppTheme;
  }

  protected void BasicCardView_PointerMoved(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
  {
    if (!HoverPreviewEnabled) return;

    var point = e.GetCurrentPoint(null).Position;

    CardPreview.Change(this, new(XamlRoot)
    {
      Uri = SelectedFaceUri,
      Coordinates = new((float)point.X, (float)point.Y)
    });
  }

  protected void BasicCardView_PointerExited(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
  {
    if (!HoverPreviewEnabled) return;

    CardPreview.Change(this, new(XamlRoot) { Uri = string.Empty });
  }

  protected static void OnModelPropertyChangedCallback(DependencyObject sender, DependencyPropertyChangedEventArgs e)
  {
    var view = (sender as BasicCardView<TCard>);

    if (e.Property.Equals(ModelProperty))
    {
      view.OnModelChanging((TCard)e.OldValue);
      view.OnModelChanged((TCard)e.NewValue);
    }
  }
}