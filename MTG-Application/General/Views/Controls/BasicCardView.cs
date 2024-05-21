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
public partial class BasicCardView : UserControl
{
  public static readonly DependencyProperty ModelProperty =
      DependencyProperty.Register(nameof(Model), typeof(MTGCard), typeof(BasicCardView),
        new PropertyMetadata(default(MTGCard), OnModelPropertyChangedCallback));

  public static readonly DependencyProperty HoverPreviewEnabledProperty =
      DependencyProperty.Register(nameof(HoverPreviewEnabled), typeof(bool), typeof(BasicCardView), new PropertyMetadata(false));

  public BasicCardView()
  {
    PointerEntered += BasicCardView_PointerEntered;
    PointerExited += BasicCardView_PointerExited;
    PointerMoved += BasicCardView_PointerMoved;
  }

  public MTGCard Model
  {
    get => (MTGCard)GetValue(ModelProperty);
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
  [RelayCommand(CanExecute = nameof(SwitchFaceImageCanExecute))]
  protected void SwitchFaceImage()
  {
    if (!SwitchFaceImageCanExecute()) return;

    SelectedFaceUri = Model?.Info.FrontFace.ImageUri == SelectedFaceUri
      ? Model.Info.BackFace?.ImageUri : Model.Info.FrontFace.ImageUri;
  }

  /// <summary>
  /// Opens card's API Website in web browser
  /// </summary>
  [RelayCommand] protected async Task OpenAPIWebsite() => await NetworkService.OpenUri(Model?.Info.APIWebsiteUri);

  /// <summary>
  /// Opens card's Cardmarket page in web browser
  /// </summary>    
  [RelayCommand] protected async Task OpenCardmarketWebsite() => await NetworkService.OpenUri(Model?.Info.CardMarketUri);

  protected bool SwitchFaceImageCanExecute() => !string.IsNullOrEmpty(Model?.Info.BackFace?.ImageUri);

  protected virtual void OnModelChanging(MTGCard oldValue) { }

  protected virtual void OnModelChanged(MTGCard newValue)
  {
    SelectedFaceUri = newValue?.Info.FrontFace.ImageUri ?? string.Empty;

    SwitchFaceImageCommand.NotifyCanExecuteChanged();
    OnPropertyChanged(nameof(CardName));
  }

  private void BasicCardView_PointerEntered(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
  {
    if (!HoverPreviewEnabled) return;

    CardPreview.Change(this, new(XamlRoot) { Uri = SelectedFaceUri });
  }

  private void BasicCardView_PointerMoved(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
  {
    if (!HoverPreviewEnabled) return;

    var point = e.GetCurrentPoint(null).Position;

    CardPreview.Change(this, new(XamlRoot)
    {
      Uri = SelectedFaceUri,
      Coordinates = new((float)point.X, (float)point.Y)
    });
  }

  private void BasicCardView_PointerExited(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
  {
    if (!HoverPreviewEnabled) return;

    CardPreview.Change(this, new(XamlRoot) { Uri = string.Empty });
  }

  private static void OnModelPropertyChangedCallback(DependencyObject sender, DependencyPropertyChangedEventArgs e)
  {
    var view = (sender as BasicCardView);

    if (e.Property.Equals(ModelProperty))
    {
      view.OnModelChanging((MTGCard)e.OldValue);
      view.OnModelChanged((MTGCard)e.NewValue);
    }
  }
}