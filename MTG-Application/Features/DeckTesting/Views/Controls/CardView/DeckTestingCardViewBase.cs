using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using MTGApplication.Features.DeckTesting.Models;
using MTGApplication.Features.DeckTesting.Services;
using MTGApplication.General.Models;
using MTGApplication.General.Services.IOServices;
using MTGApplication.General.Views.Controls;
using MTGApplication.General.Views.Extensions;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace MTGApplication.Features.DeckTesting.Views.Controls.CardView;

public partial class DeckTestingCardViewBase : UserControl, INotifyPropertyChanged
{
  public static readonly DependencyProperty ModelProperty =
      DependencyProperty.Register(nameof(Model), typeof(DeckTestingMTGCard), typeof(DeckTestingCardViewBase), new PropertyMetadata(default(DeckTestingMTGCard), OnModelPropertyChangedCallback));

  public static readonly DependencyProperty HoverPreviewEnabledProperty =
      DependencyProperty.Register(nameof(HoverPreviewEnabled), typeof(bool), typeof(DeckTestingCardViewBase), new PropertyMetadata(false));

  public DeckTestingCardViewBase()
  {
    Loaded += (_, _) =>
    {
      RequestedTheme = AppConfig.LocalSettings.AppTheme;
      AppConfig.LocalSettings.PropertyChanged += AppSettings_PropertyChanged;
    };
    Unloaded += (_, _) => { AppConfig.LocalSettings.PropertyChanged -= AppSettings_PropertyChanged; };
  }

  public DeckTestingMTGCard Model
  {
    get => (DeckTestingMTGCard)GetValue(ModelProperty);
    set => SetValue(ModelProperty, value);
  }
  public bool HoverPreviewEnabled
  {
    get => (bool)GetValue(HoverPreviewEnabledProperty);
    set => SetValue(HoverPreviewEnabledProperty, value);
  }

  public string SelectedFaceUri
  {
    get;
    set
    {
      if (field != value)
      {
        field = value;
        PropertyChanged?.Invoke(this, new(nameof(SelectedFaceUri)));
      }
    }
  } = "";
  public string CardName => Model?.Info.Name ?? string.Empty;

  public event PropertyChangedEventHandler? PropertyChanged;

  /// <summary>
  /// Changes selected face image if possible
  /// </summary>
  [RelayCommand(CanExecute = nameof(CanExecuteSwitchFaceImage))]
  protected void SwitchFaceImage()
  {
    if (!CanExecuteSwitchFaceImage()) return;

    SelectedFaceUri = (Model?.Info.FrontFace.ImageUri == SelectedFaceUri
      ? Model.Info.BackFace?.ImageUri
      : Model?.Info.FrontFace.ImageUri)
      ?? string.Empty;
  }

  /// <summary>
  /// Opens card's API Website in web browser
  /// </summary>
  [RelayCommand(CanExecute = nameof(CanExecuteOpenAPIWebsite))]
  protected async Task OpenAPIWebsite()
  {
    if (CanExecuteOpenAPIWebsite())
      await NetworkIO.OpenUri(Model!.Info.APIWebsiteUri);
  }

  /// <summary>
  /// Opens card's Cardmarket page in web browser
  /// </summary>    
  [RelayCommand(CanExecute = nameof(CanExecuteOpenCardmarketWebsite))]
  protected async Task OpenCardmarketWebsite()
  {
    if (CanExecuteOpenCardmarketWebsite())
      await NetworkIO.OpenUri(Model!.Info.CardMarketUri);
  }

  protected bool CanExecuteSwitchFaceImage() => !string.IsNullOrEmpty(Model?.Info.BackFace?.ImageUri);

  protected bool CanExecuteOpenAPIWebsite() => Model != null;

  protected bool CanExecuteOpenCardmarketWebsite() => Model != null;

  protected void AppSettings_PropertyChanged(object? _, PropertyChangedEventArgs e)
  {
    // Requested theme needs to be changed to the selected theme here, because flyouts will not change theme if
    // the requested theme is Default.
    // And ActualThemeChanged event invokes only once when changing the theme
    if (e.PropertyName == nameof(AppConfig.LocalSettings.AppTheme))
      RequestedTheme = AppConfig.LocalSettings.AppTheme;
  }

  protected static void OnModelPropertyChangedCallback(DependencyObject sender, DependencyPropertyChangedEventArgs e)
  {
    if (sender is not DeckTestingCardViewBase view)
      return;

    if (e.Property.Equals(ModelProperty))
    {
      view.OnModelChanging((DeckTestingMTGCard)e.OldValue);
      view.OnModelChanged((DeckTestingMTGCard)e.NewValue);
    }
  }

  protected virtual void OnModelChanging(DeckTestingMTGCard oldValue)
  {
    if (oldValue != null) oldValue.PropertyChanged -= Model_PropertyChanged;
  }

  protected virtual void OnModelChanged(DeckTestingMTGCard newValue)
  {
    SelectedFaceUri = newValue?.Info.FrontFace.ImageUri ?? string.Empty;

    if (newValue != null) newValue.PropertyChanged += Model_PropertyChanged;

    PropertyChanged?.Invoke(this, new(nameof(CardName)));
    SwitchFaceImageCommand.NotifyCanExecuteChanged();
    OpenAPIWebsiteCommand.NotifyCanExecuteChanged();
    OpenCardmarketWebsiteCommand.NotifyCanExecuteChanged();
  }

  protected void Model_PropertyChanged(object? _, PropertyChangedEventArgs e)
  {
    if (e.PropertyName == nameof(MTGCard.Info))
      SelectedFaceUri = Model?.Info.FrontFace.ImageUri ?? string.Empty;
  }

  protected override void OnPointerEntered(PointerRoutedEventArgs e)
  {
    // Changing preview image on pointer enter will prevent flickering
    if (!DeckTestingCardDrag.IsDragging)
      DragCardPreview.Change(this, new(XamlRoot) { Uri = SelectedFaceUri });
  }

  protected override void OnPointerPressed(PointerRoutedEventArgs e)
  {
    if (e.GetCurrentPoint(null).Properties.IsLeftButtonPressed)
    {
      var pointerPosition = e.GetCurrentPoint(null).Position;

      //Can't use pointer capturing because it will prevent other elements to fire pointer events...
      //CapturePointer(e.Pointer);

      //... Instead use the window content's pointer events
      XamlRoot.Content.PointerMoved += Root_PointerMoved;
      XamlRoot.Content.PointerReleased += Root_PointerReleased;

      CardPreview.Change(this, new(XamlRoot) { Uri = string.Empty });

      DragCardPreview.Change(this, new(XamlRoot)
      {
        Uri = SelectedFaceUri,
        Coordinates = new((float)pointerPosition.X, (float)pointerPosition.Y),
        Offset = DragCardPreview.DefaultOffset,
        Opacity = DragCardPreview.DroppableOpacity,
      });

      DeckTestingCardDrag.Completed += OnDragCompleted;
      DeckTestingCardDrag.Ended += OnDragEnded;

      DeckTestingCardDrag.Start(Model);
    }
  }

  protected override void OnPointerReleased(PointerRoutedEventArgs e)
  {
    // Cancel drag if dropped on the dragged item
    if (DeckTestingCardDrag.IsDragging && DeckTestingCardDrag.Item == Model)
      DeckTestingCardDrag.Cancel();
  }

  protected override void OnPointerMoved(PointerRoutedEventArgs e)
  {
    // Disable Hover preview if card is being dragged
    if (!DeckTestingCardDrag.IsDragging)
    {
      // Updates hover preview
      if (!HoverPreviewEnabled)
        return;

      HoverPreviewUpdate(this, e);
    }
  }

  protected override void OnPointerExited(PointerRoutedEventArgs e)
  {
    if (!HoverPreviewEnabled) return;

    CardPreview.Change(this, new(XamlRoot) { Uri = string.Empty });
  }

  protected virtual void HoverPreviewUpdate(FrameworkElement sender, PointerRoutedEventArgs e)
  {
    if (DeckTestingCardDrag.IsDragging) return; // Disable card preview if card is being dragged

    var point = e.GetCurrentPoint(null).Position;

    CardPreview.Change(this, new(XamlRoot)
    {
      Uri = SelectedFaceUri,
      Coordinates = new((float)point.X, (float)point.Y)
    });
  }

  protected virtual void Root_PointerReleased(object sender, PointerRoutedEventArgs e) => OnPointerReleased(e);

  protected virtual void Root_PointerMoved(object sender, PointerRoutedEventArgs e)
  {
    if (DeckTestingCardDrag.IsDragging
      && (e.GetCurrentPoint(null).Properties.IsRightButtonPressed
      || !e.GetCurrentPoint(null).Properties.IsLeftButtonPressed))
      DeckTestingCardDrag.Cancel();
  }

  private void OnDragCompleted(DeckTestingMTGCard? item)
  {
    if (item != null && !item.IsToken)
      (this.FindParentByType<ListViewBase>()?.ItemsSource as ObservableCollection<DeckTestingMTGCard>)?.Remove(item);
  }

  private void OnDragEnded()
  {
    XamlRoot.Content.PointerMoved -= Root_PointerMoved;
    XamlRoot.Content.PointerReleased -= Root_PointerReleased;
    DeckTestingCardDrag.Completed -= OnDragCompleted;
    DeckTestingCardDrag.Ended -= OnDragEnded;
  }

  protected void RaisePropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new(propertyName));

  protected void SetProperty<T>(ref T storage, T value, [CallerMemberName] string? propertyName = null)
  {
    if (Equals(storage, value))
      return;

    storage = value;
    PropertyChanged?.Invoke(this, new(propertyName));
  }
}
