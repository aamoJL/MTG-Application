using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using MTGApplication.Features.CardCollectionEditor.ViewModels.CollectionCard;
using MTGApplication.General.Models;
using System.ComponentModel;
using System.Windows.Input;

namespace MTGApplication.Features.CardCollectionEditor.Views.Controls;

public enum SelectionMode { SingleTap, DoubleTap }

public sealed partial class CardCollectionCardImageView : UserControl, INotifyPropertyChanged
{
  public static readonly DependencyProperty ModelProperty =
      DependencyProperty.Register(nameof(Model), typeof(CardCollectionMTGCardViewModel), typeof(CardCollectionMTGCardViewModel), new PropertyMetadata(null, OnModelPropertyChangedCallback));

  public static readonly DependencyProperty ShowPrintsCommandProperty =
      DependencyProperty.Register(nameof(ShowPrintsCommand), typeof(ICommand), typeof(CardCollectionCardImageView), new PropertyMetadata(null));

  public static readonly DependencyProperty SwitchOwnershipCommandProperty =
      DependencyProperty.Register(nameof(SwitchOwnershipCommand), typeof(ICommand), typeof(CardCollectionCardImageView), new PropertyMetadata(null));

  public static readonly DependencyProperty OpenAPIWebsiteCommandProperty =
      DependencyProperty.Register(nameof(OpenAPIWebsiteCommand), typeof(ICommand), typeof(CardCollectionCardImageView), new PropertyMetadata(null));

  public static readonly DependencyProperty OpenCardmarketWebsiteCommandProperty =
      DependencyProperty.Register(nameof(OpenCardmarketWebsiteCommand), typeof(ICommand), typeof(CardCollectionCardImageView), new PropertyMetadata(null));

  public static readonly DependencyProperty SelectionModeProperty =
      DependencyProperty.Register(nameof(SelectionMode), typeof(SelectionMode), typeof(CardCollectionCardImageView), new PropertyMetadata(SelectionMode.DoubleTap));

  public CardCollectionCardImageView()
  {
    InitializeComponent();

    Tapped += CardCollectionCardViewBase_Tapped;
    DoubleTapped += CardCollectionCardViewBase_DoubleTapped;
    Loaded += (_, _) =>
    {
      RequestedTheme = AppConfig.LocalSettings.AppTheme;
      AppConfig.LocalSettings.PropertyChanged += AppSettings_PropertyChanged;
    };
    Unloaded += (_, _) => { AppConfig.LocalSettings.PropertyChanged -= AppSettings_PropertyChanged; };
  }

  public CardCollectionMTGCardViewModel Model
  {
    get => (CardCollectionMTGCardViewModel)GetValue(ModelProperty);
    set => SetValue(ModelProperty, value);
  }
  public SelectionMode SelectionMode
  {
    get => (SelectionMode)GetValue(SelectionModeProperty);
    set => SetValue(SelectionModeProperty, value);
  }
  public ICommand ShowPrintsCommand
  {
    get => (ICommand)GetValue(ShowPrintsCommandProperty);
    set => SetValue(ShowPrintsCommandProperty, value);
  }
  public ICommand SwitchOwnershipCommand
  {
    get => (ICommand)GetValue(SwitchOwnershipCommandProperty);
    set => SetValue(SwitchOwnershipCommandProperty, value);
  }
  public ICommand OpenAPIWebsiteCommand
  {
    get => (ICommand)GetValue(OpenAPIWebsiteCommandProperty);
    set => SetValue(OpenAPIWebsiteCommandProperty, value);
  }
  public ICommand OpenCardmarketWebsiteCommand
  {
    get => (ICommand)GetValue(OpenCardmarketWebsiteCommandProperty);
    set => SetValue(OpenCardmarketWebsiteCommandProperty, value);
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

  public event PropertyChangedEventHandler? PropertyChanged;

  [RelayCommand(CanExecute = nameof(CanExecuteSwitchFaceImage))]
  private void SwitchFaceImage()
  {
    if (!CanExecuteSwitchFaceImage()) return;

    SelectedFaceUri = (Model?.Info.FrontFace.ImageUri == SelectedFaceUri
      ? Model.Info.BackFace?.ImageUri
      : Model?.Info.FrontFace.ImageUri)
      ?? string.Empty;
  }

  public double OwnedToOpacity(bool owned) => owned ? 1 : .5f;

  private void CardCollectionCardViewBase_DoubleTapped(object _, Microsoft.UI.Xaml.Input.DoubleTappedRoutedEventArgs __)
  {
    if (SelectionMode == SelectionMode.DoubleTap)
      SwitchOwnershipCommand?.Execute(Model);
  }

  private void CardCollectionCardViewBase_Tapped(object _, Microsoft.UI.Xaml.Input.TappedRoutedEventArgs __)
  {
    if (SelectionMode == SelectionMode.SingleTap)
      SwitchOwnershipCommand?.Execute(Model);
  }

  private void AppSettings_PropertyChanged(object? _, PropertyChangedEventArgs e)
  {
    // Requested theme needs to be changed to the selected theme here, because flyouts will not change theme if
    // the requested theme is Default.
    // And ActualThemeChanged event invokes only once when changing the theme
    if (e.PropertyName == nameof(AppConfig.LocalSettings.AppTheme))
      RequestedTheme = AppConfig.LocalSettings.AppTheme;
  }

  private bool CanExecuteSwitchFaceImage() => !string.IsNullOrEmpty(Model?.Info.BackFace?.ImageUri);

  private static void OnModelPropertyChangedCallback(DependencyObject sender, DependencyPropertyChangedEventArgs e)
  {
    if (sender is not CardCollectionCardImageView view) return;
    if (!e.Property.Equals(ModelProperty)) return;

    if (e.OldValue is CardCollectionMTGCardViewModel oldValue)
      view.Model_Changing(oldValue);

    if (e.NewValue is CardCollectionMTGCardViewModel newValue)
      view.Model_Changed(newValue);
  }

  private void Model_Changing(CardCollectionMTGCardViewModel oldValue)
    => oldValue.PropertyChanged -= Model_PropertyChanged;

  private void Model_Changed(CardCollectionMTGCardViewModel newValue)
  {
    SelectedFaceUri = newValue?.Info.FrontFace.ImageUri ?? string.Empty;

    if (newValue != null)
      newValue.PropertyChanged += Model_PropertyChanged;

    SwitchFaceImageCommand.NotifyCanExecuteChanged();
  }

  private void Model_PropertyChanged(object? _, PropertyChangedEventArgs e)
  {
    if (e.PropertyName == nameof(MTGCard.Info))
      SelectedFaceUri = Model?.Info.FrontFace.ImageUri ?? string.Empty;
  }
}