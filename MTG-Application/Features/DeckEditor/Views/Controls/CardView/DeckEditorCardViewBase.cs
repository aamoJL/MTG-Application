using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using MTGApplication.Features.DeckEditor.ViewModels.DeckCard;
using System.ComponentModel;
using System.Windows.Input;

namespace MTGApplication.Features.DeckEditor.Views.Controls.CardView;

public partial class DeckEditorCardViewBase : UserControl, INotifyPropertyChanged
{
  public static readonly DependencyProperty ModelProperty =
      DependencyProperty.Register(nameof(Model), typeof(DeckCardViewModel), typeof(DeckEditorCardViewBase), new PropertyMetadata(null, OnModelPropertyChangedCallback));

  public static readonly DependencyProperty ShowPrintsCommandProperty =
      DependencyProperty.Register(nameof(ShowPrintsCommand), typeof(ICommand), typeof(DeckEditorCardViewBase), new PropertyMetadata(null));

  public static readonly DependencyProperty OpenAPIWebsiteCommandProperty =
      DependencyProperty.Register(nameof(OpenAPIWebsiteCommand), typeof(ICommand), typeof(DeckEditorCardViewBase), new PropertyMetadata(null));

  public static readonly DependencyProperty OpenCardmarketWebsiteCommandProperty =
      DependencyProperty.Register(nameof(OpenCardmarketWebsiteCommand), typeof(ICommand), typeof(DeckEditorCardViewBase), new PropertyMetadata(null));

  public static readonly DependencyProperty DeleteButtonClickProperty =
      DependencyProperty.Register(nameof(DeleteButtonClick), typeof(ICommand), typeof(DeckEditorCardViewBase), new PropertyMetadata(default(ICommand)));

  public static readonly DependencyProperty TagVisibleProperty =
      DependencyProperty.Register(nameof(TagVisible), typeof(bool), typeof(DeckEditorCardViewBase), new PropertyMetadata(true));

  public DeckEditorCardViewBase()
  {
    Loaded += (_, _) =>
    {
      RequestedTheme = AppConfig.LocalSettings.AppTheme;
      AppConfig.LocalSettings.PropertyChanged += AppSettings_PropertyChanged;
    };
    Unloaded += (_, _) => { AppConfig.LocalSettings.PropertyChanged -= AppSettings_PropertyChanged; };
  }

  public DeckCardViewModel Model
  {
    get => (DeckCardViewModel)GetValue(ModelProperty);
    set => SetValue(ModelProperty, value);
  }
  public ICommand ShowPrintsCommand
  {
    get => (ICommand)GetValue(ShowPrintsCommandProperty);
    set => SetValue(ShowPrintsCommandProperty, value);
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
  public ICommand? DeleteButtonClick
  {
    get => (ICommand)GetValue(DeleteButtonClickProperty);
    set => SetValue(DeleteButtonClickProperty, value);
  }
  public bool? TagVisible
  {
    get => (bool)GetValue(TagVisibleProperty);
    set => SetValue(TagVisibleProperty, value);
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
    if (sender is not DeckEditorCardViewBase view) return;
    if (!e.Property.Equals(ModelProperty)) return;

    if (e.OldValue is DeckCardViewModel oldValue)
      view.Model_Changing(oldValue);

    if (e.NewValue is DeckCardViewModel newValue)
      view.Model_Changed(newValue);
  }

  private void Model_Changing(DeckCardViewModel oldValue)
    => oldValue.PropertyChanged -= Model_PropertyChanged;

  private void Model_Changed(DeckCardViewModel newValue)
  {
    SelectedFaceUri = newValue?.Info.FrontFace.ImageUri ?? string.Empty;

    if (newValue != null)
      newValue.PropertyChanged += Model_PropertyChanged;

    SwitchFaceImageCommand.NotifyCanExecuteChanged();
  }

  private void Model_PropertyChanged(object? _, PropertyChangedEventArgs e)
  {
    if (e.PropertyName == nameof(Model.Info))
      SelectedFaceUri = Model?.Info.FrontFace.ImageUri ?? string.Empty;
  }
}